import { Component } from '@angular/core';
import { TableModule } from 'primeng/table';
import { RippleModule } from 'primeng/ripple';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { ConfirmationService } from 'primeng/api';
import { Iorder } from '../../../../Core/Interfaces/UserInterfaces/iorder';
import { CreateOrderService } from '../../../../Core/Services/UserServices/create-order.service';
import { CartService } from '../../../../Core/Services/UserServices/cart.service';
import { ICart } from '../../../../Core/Interfaces/UserInterfaces/icart';
@Component({
  selector: 'app-payment-success',
  standalone: true,
  imports: [TableModule, RippleModule, ButtonModule, CommonModule],
  templateUrl: './payment-success.component.html',
  styleUrl: './payment-success.component.scss',
  providers: [ConfirmationService],
})
export class PaymentSuccessComponent {
  orderData: Iorder[] = [];

  constructor(private _CreateOrderService: CreateOrderService) {}
  ngOnInit(): void {
    this._CreateOrderService.getOrdersDetails().subscribe((data) => {
      if (data) {
        this.orderData = data;
        console.log(data);
      } else {
        console.log('there is no data to show :(');
      }
    });
  }
}
