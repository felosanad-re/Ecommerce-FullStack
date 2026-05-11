import { ICart } from './../../../../Core/Interfaces/UserInterfaces/icart';
import { Component, ViewEncapsulation } from '@angular/core';
import { CartService } from '../../../../Core/Services/UserServices/cart.service';
import { DataViewModule } from 'primeng/dataview';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { NotificationsService } from '../../../../Core/Services/notifications.service';
import { RouterLink } from '@angular/router';
import { ICartItem } from '../../../../Core/Interfaces/UserInterfaces/ICartItem';

import { InputNumberModule } from 'primeng/inputnumber';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-carts',
  standalone: true,
  imports: [
    DataViewModule,
    ButtonModule,
    CommonModule,
    RouterLink,
    InputNumberModule,
    FormsModule,
  ],
  templateUrl: './carts.component.html',
  styleUrl: './carts.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class CartsComponent {
  allProductInCart: ICartItem[] = [];

  value3: number = 25;

  cartDetails: ICart = { items: [] };
  constructor(
    private _cartService: CartService,
    private _notification: NotificationsService,
  ) {}

  ngOnInit(): void {
    this.getCartDetails();
  }

  get cartItemCount(): number {
    return this.allProductInCart.reduce(
      (total, item) => total + (item.count ?? 0),
      0,
    );
  }

  get cartSubtotal(): number {
    return this.allProductInCart.reduce(
      (total, item) => total + this.getItemTotal(item),
      0,
    );
  }

  getItemTotal(product: ICartItem): number {
    return (product.price ?? 0) * (product.count ?? 0);
  }

  getCartDetails(): void {
    this._cartService.getCartDetails().subscribe({
      next: (res: ICart) => {
        this.cartDetails = res;
        this.allProductInCart = res.items ?? [];
      },
    });
  }

  clearCart(): void {
    this._cartService.deleteCart().subscribe({
      next: (res) => {
        this.allProductInCart = [];
        this._notification.showSuccedded('Delete Cart', res.message);
      },
    });
  }

  removeItem(product: ICartItem): void {
    const cartDetail: ICart = {
      ...this.cartDetails,
      items: this.allProductInCart.filter((item) => item.id !== product.id),
    };

    this._cartService.addToCart(cartDetail).subscribe({
      next: (res) => {
        this.cartDetails = res;
        this.allProductInCart = res.items ?? [];
        this._notification.showSuccedded('Delete Item', `${product.name} removed from cart`);
      },
    });
  }

  updateCount(): void {
    const cartDetail: ICart = {
      ...this.cartDetails,
      items: this.allProductInCart.map((item) => ({
        ...item,
        count: Math.max(item.count ?? 1, 1),
      })),
    };
    this._cartService.addToCart(cartDetail).subscribe((res) => {
      this.cartDetails = res;
      this.allProductInCart = res.items ?? [];
    });
  }
}
