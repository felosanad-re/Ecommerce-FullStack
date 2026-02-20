import { ICart } from './../../../../Core/Interfaces/UserInterfaces/icart';
import { Component } from '@angular/core';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { RippleModule } from 'primeng/ripple';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { CommonModule } from '@angular/common';
import { FileUploadModule } from 'primeng/fileupload';
import { RatingModule } from 'primeng/rating';
import { FormsModule } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { PaymentService } from '../../../../Core/Services/UserServices/payment.service';
import { CartService } from '../../../../Core/Services/UserServices/cart.service';
import { loadStripe, Stripe } from '@stripe/stripe-js';
import { ActivatedRoute } from '@angular/router';
import { environment } from '../../../../environment';
@Component({
  selector: 'app-order-details',
  standalone: true,
  imports: [
    TableModule,
    DialogModule,
    RippleModule,
    ButtonModule,
    ToolbarModule,
    ConfirmDialogModule,
    CommonModule,
    FileUploadModule,
    RatingModule,
    FormsModule,
  ],
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.scss',
  providers: [ConfirmationService],
})
export class OrderDetailsComponent {
  constructor(
    private _cartService: CartService,
    private _paymentService: PaymentService,
    private _activatedRoute: ActivatedRoute,
  ) {}

  products!: ICart;
  ngOnInit(): void {
    // جلب بيانات الكارت
    this._cartService.getCartDetails().subscribe({
      next: (data: ICart) => {
        this.products = data;
        console.log('Cart data:', data);
      },
      error: (err) => console.error('Cart fetch error:', err),
    });
  }

  pay(): void {
    const orderId = Number(
      this._activatedRoute.snapshot.paramMap.get('id') || '0',
    );
    if (orderId <= 0) {
      alert('Invalid order ID');
      return;
    }

    this._paymentService.redirectToCheckout(orderId);
  }
}
