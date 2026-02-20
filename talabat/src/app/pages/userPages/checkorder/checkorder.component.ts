import { Component } from '@angular/core';
import { CreateOrderService } from '../../../../Core/Services/UserServices/create-order.service';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ICreateOrder } from '../../../../Core/Interfaces/UserInterfaces/Icreate-order';
import { AuthModule } from '../../../../Core/modules/auth/auth.module';
import { DelivaryMethod } from '../../../../Core/Interfaces/UserInterfaces/delivary-method';
import { DropdownModule } from 'primeng/dropdown';
import { Router } from '@angular/router';
import { NotificationsService } from '../../../../Core/Services/notifications.service';
import { CartService } from '../../../../Core/Services/UserServices/cart.service';
import { TabViewModule } from 'primeng/tabview';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ICart } from '../../../../Core/Interfaces/UserInterfaces/icart';
import { PaymentService } from '../../../../Core/Services/UserServices/payment.service';

@Component({
  selector: 'app-checkorder',
  standalone: true,
  imports: [
    AuthModule,
    DropdownModule,
    TabViewModule,
    RadioButtonModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  templateUrl: './checkorder.component.html',
  styleUrl: './checkorder.component.scss',
})
export class CheckorderComponent {
  firstName!: FormControl;
  lastName!: FormControl;
  city!: FormControl;
  street!: FormControl;

  adressForm!: FormGroup;
  delivaryForm!: FormGroup;

  delivary!: DelivaryMethod[];
  selectDelivary!: DelivaryMethod;
  activeIndex: number = 0;
  cartData!: ICart;
  constructor(
    private _createOrederService: CreateOrderService,
    private _cartService: CartService,
    private _router: Router,
    private _notificationsServicce: NotificationsService,
    private _paymentService: PaymentService,
  ) {
    this.initiateFormControlers();
    this.initiateFormGroubs();
  }

  ngOnInit(): void {
    this.getDelivaryMethods();
    this._cartService.getCartDetails().subscribe((res) => {
      this.cartData = res;
      console.log(this.cartData);
    });
  }
  // initiate formControls
  initiateFormControlers(): void {
    this.firstName = new FormControl('', [Validators.required]);
    this.lastName = new FormControl('', [Validators.required]);
    this.city = new FormControl('', [Validators.required]);
    this.street = new FormControl('', [Validators.required]);
  }

  // Initiate FormGroup
  initiateFormGroubs(): void {
    this.adressForm = new FormGroup({
      firstName: this.firstName,
      lastName: this.lastName,
      city: this.city,
      street: this.street,
    });

    this.delivaryForm = new FormGroup({
      delivary: new FormControl('', [Validators.required]),
    });
  }

  // Submit Funtion
  submitAddressForm(): void {
    if (this.adressForm.valid) {
      const data: ICreateOrder = {
        addressShiper: {
          firstName: this.adressForm.value.firstName,
          lastName: this.adressForm.value.lastName,
          city: this.adressForm.value.city,
          street: this.adressForm.value.street,
        },
        delivaryMethod: Number.parseInt(
          localStorage.getItem('deleveryMethodId') || '',
        ),
      };
      console.log(data);
      this.checkOrder(data);
    } else {
      this.adressForm.markAllAsTouched();
      Object.keys(this.adressForm.controls).forEach((control) =>
        this.adressForm.controls[control].markAsDirty(),
      );
    }
  }

  checkOrder(data: ICreateOrder): void {
    this._createOrederService.checkOutOrder(data).subscribe({
      next: (res) => {
        this._notificationsServicce.showSuccedded(
          'Check Out',
          'You Can Payment your Order now',
        );
        console.log(res);
        this._router.navigate(['confirmOrder']);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  // get delivary Method
  getDelivaryMethods() {
    this._createOrederService.getDelivaryMethods().subscribe({
      next: (res) => {
        this.delivary = res;
      },
    });
  }

  //Select DelivaryMethod
  selectDelivaryMethod(): void {
    debugger;
    const data: ICart = {
      items: this.cartData.items,
      deleveryMethodId: this.delivaryForm.value.delivary,
    };
    this._cartService.addToCart(data).subscribe((next) => {
      this._notificationsServicce.showSuccedded(
        'Select your Delivary Method',
        'Enter Your Shipping  Address',
      );
      this.activeIndex = 1;
      console.log(next.deleveryMethodId);
      localStorage.setItem(
        'deleveryMethodId',
        next.deleveryMethodId?.toString() || '',
      );
    });
    this.createPaymentIntent(); // Create PaymentIntent After Select Delivary
  }

  goToAddress(): void {
    this.activeIndex = 1;
  }

  // Create PaymentIntent
  createPaymentIntent() {
    // const data: ICart = {
    //   ...this.cartData,
    //   deleveryMethodId: Number.parseInt(
    //     localStorage.getItem('deleveryMethodId') || '',
    //   ),
    // };
    // this._paymentService.createPaymentIntent(data).subscribe((res) => {
    //   localStorage.setItem('clientSecret', res.clientSecret || '');
    //   this.cartData = res;
    //   console.log('After PaymentIntent:', res.deleveryMethodId);
    // });
  }
}
