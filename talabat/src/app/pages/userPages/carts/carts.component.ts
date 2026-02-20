import { ICart } from './../../../../Core/Interfaces/UserInterfaces/icart';
import { Component, ViewEncapsulation } from '@angular/core';
import { CartService } from '../../../../Core/Services/UserServices/cart.service';
import { DataViewModule } from 'primeng/dataview';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { CommonModule } from '@angular/common';
import { NotificationsService } from '../../../../Core/Services/notifications.service';
import { Router, RouterLink } from '@angular/router';
import { ICartItem } from '../../../../Core/Interfaces/UserInterfaces/ICartItem';

import { InputNumberModule } from 'primeng/inputnumber';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-carts',
  standalone: true,
  imports: [
    DataViewModule,
    ButtonModule,
    TagModule,
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

  cartDetails!: ICart[];
  constructor(
    private _cartService: CartService,
    private _notification: NotificationsService,
  ) {}

  ngOnInit(): void {
    this.getCartDetails();
  }

  getCartDetails(): void {
    this._cartService.getCartDetails().subscribe({
      next: (res: ICart) => {
        this.allProductInCart = res.items;
      },
    });
  }

  clearCart(): void {
    this._cartService.deleteCart().subscribe({
      next: (res) => {
        this._notification.showSuccedded('Delete Cart', res.message);
        window.location.reload();
      },
    });
  }

  updateCount(): void {
    const cartDetail: ICart = {
      ...this.cartDetails,
      items: this.allProductInCart,
    };
    this._cartService.addToCart(cartDetail).subscribe((res) => {
      this.allProductInCart = res.items;
      console.log(res.items);
    });
  }
}
