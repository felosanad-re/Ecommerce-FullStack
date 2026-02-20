import { Component } from '@angular/core';
import { IProduct } from '../../../../Core/Interfaces/UserInterfaces/iproduct';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductService } from '../../../../Core/Services/UserServices/product.service';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { CartService } from '../../../../Core/Services/UserServices/cart.service';
import { NotificationsService } from '../../../../Core/Services/notifications.service';
import { ICartItem } from '../../../../Core/Interfaces/UserInterfaces/ICartItem';
import { ICart } from '../../../../Core/Interfaces/UserInterfaces/icart';
@Component({
  selector: 'app-details',
  standalone: true,
  imports: [CardModule, ButtonModule, RouterLink],
  templateUrl: './details.component.html',
  styleUrl: './details.component.scss',
})
export class DetailsComponent {
  productDetails!: IProduct;
  isAddedToCart: boolean = false;

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _cartService: CartService,
    private _notificationsService: NotificationsService,
  ) {}

  ngOnInit() {
    this.getProductDetails(this._activatedRoute.snapshot.params['id']);
  }

  getProductDetails(id: number): void {
    this._activatedRoute.data.subscribe(
      (responce: any) =>
        // console.log(responce.Details)
        (this.productDetails = responce.Details),
    );
  }
  addToCart(product: IProduct) {
    // Send Object from Product to cart
    const cartData: ICart = {
      items: [
        {
          price: product.price,
          count: 1,
          id: product.id,
          name: product.name,
          pictureUrl: product.pictureUrl,
        },
      ],
    };
    this._cartService.addToCart(cartData).subscribe((next) => {
      // debugger;
      this.isAddedToCart = true;
      this._cartService.cartCount.next(next.items.length); // Get Number Of Carts
      this._notificationsService.showSuccedded('Add To Cart', 'Product Added');
    });
  }
}
