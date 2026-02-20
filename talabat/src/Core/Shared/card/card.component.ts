import { ICartItem } from './../../Interfaces/UserInterfaces/ICartItem';
import { NgClass } from '@angular/common';
import { Component, Input } from '@angular/core';
import { IProduct } from '../../Interfaces/UserInterfaces/iproduct';
import { Button } from 'primeng/button';
import { AuthModule } from '../../modules/auth/auth.module';
import { CartService } from '../../Services/UserServices/cart.service';
import { NotificationsService } from '../../Services/notifications.service';
import { PageNotFoundComponent } from '../page-not-found/page-not-found.component';
import { AuthService } from '../../Services/ِAuthServices/auth.service';
import { Router } from '@angular/router';
import { ICart } from '../../Interfaces/UserInterfaces/icart';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [NgClass, Button, AuthModule, PageNotFoundComponent],
  templateUrl: './card.component.html',
  styleUrl: './card.component.scss',
})
export class CardComponent {
  @Input({ required: true }) isSmallCard: boolean = false;
  @Input({ required: true }) products!: IProduct[];
  @Input() searchKey: string = '';
  // isAddedToCart: boolean = false;
  constructor(
    private _cartService: CartService,
    private _authService: AuthService,
    private _router: Router,
    private _notificationsService: NotificationsService,
  ) {}

  addToCart(product: IProduct) {
    // debugger;
    // check if user login or not
    if (!this._authService.checkToken()) {
      this._notificationsService.showError('Add To Cart', 'Please Login First');
      this._router.navigate(['login']);
    }
    // Send Object from Product to cart
    const cartData: ICart = {
      items: [
        {
          id: product.id,
          name: product.name,
          pictureUrl: product.pictureUrl,
          price: product.price,
          count: 1,
        },
      ],
    };
    this._cartService.addToCart(cartData).subscribe((next) => {
      console.log(next);
      product.isAddedToCart = true;
      this._cartService.cartCount.next(next.items.length);
      this._notificationsService.showSuccedded('Add To Cart', 'Product Added');
    });
  }
}
