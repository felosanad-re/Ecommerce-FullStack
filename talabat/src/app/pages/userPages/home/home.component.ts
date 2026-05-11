import { IProduct } from './../../../../Core/Interfaces/UserInterfaces/iproduct';
import { Component, ViewEncapsulation } from '@angular/core';

import { GalleriaModule } from 'primeng/galleria';
import { CardComponent } from '../../../../Core/Shared/card/card.component';
import { ProductService } from '../../../../Core/Services/UserServices/product.service';
import { ProductParams } from '../../../../Core/Interfaces/UserInterfaces/product-params';
import { CartService } from '../../../../Core/Services/UserServices/cart.service';
import { ICart } from '../../../../Core/Interfaces/UserInterfaces/icart';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';

interface GalleryImage {
  itemImageSrc: string;
  alt: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [GalleriaModule, CardComponent, PaginatorModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class HomeComponent {
  images: GalleryImage[] = [];
  smallProducts!: IProduct[];
  allProducts!: IProduct[];
  productParams = new ProductParams();
  cart?: ICart;
  pageIndex: number = 1;
  pageSize: number = 8;
  productCount!: number;
  // cartItems?: ICartItem;

  responsiveOptions: any[] = [
    {
      breakpoint: '1024px',
      numVisible: 5,
    },
    {
      breakpoint: '768px',
      numVisible: 3,
    },
    {
      breakpoint: '560px',
      numVisible: 1,
    },
  ];

  constructor(
    private _productService: ProductService,
    private _cartService: CartService,
  ) {}

  ngOnInit() {
    this.getSmallProduct();
    this._cartService.getCartDetails().subscribe({
      next: () => this.loadProducts(),
      error: () => this.loadProducts(),
    });
  }
  private loadProducts(): void {
    this.productParams.pageSize = this.pageSize;
    this.productParams.pageIndex = this.pageIndex;
    this.productParams.isDeleted = false;
    this._productService.getProducts(this.productParams).subscribe({
      next: (response: any) => {
        this.allProducts = response.data.map((product: IProduct) => ({
          ...product,
        }));
      },
      error: (err) => {
        console.error('Failed to load products', err);
      },
    });
  }

  getSmallProduct(): void {
    this._productService.getProducts(this.productParams).subscribe({
      next: (response: any) => {
        this.smallProducts = response.data.slice(0, 4);
        this.images = this.smallProducts.map((product) => ({
          itemImageSrc: product.pictureUrl,
          alt: product.name,
          title: product.name,
          description: product.descripaion,
        }));
      },
      error: (err) => {
        console.error('Failed to load products', err);
      },
    });
  }

  private isInCart(productId: number, cart: ICart | null): boolean {
    if (!cart?.items?.length) return false;
    return cart.items.some((item) => item.id === productId); // item.id = product.id
  }

  // Pagination
  first: number = 0;

  rows: number = 6;

  onPageChange(event: PaginatorState) {
    this.first = event.first ?? 0;
    this.pageIndex = (event.page ?? 0) + 1;
    this.pageSize = event.rows ?? 8;

    this.loadProducts();

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
