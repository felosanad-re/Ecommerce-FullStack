import { ProductParams } from './../../../../Core/Interfaces/UserInterfaces/product-params';
import { Component } from '@angular/core';
import { IProduct } from '../../../../Core/Interfaces/UserInterfaces/iproduct';
import { CardComponent } from '../../../../Core/Shared/card/card.component';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../../Core/Services/UserServices/product.service';

@Component({
  selector: 'app-product-in-brand',
  standalone: true,
  imports: [CardComponent],
  templateUrl: './product-in-brand.component.html',
  styleUrl: './product-in-brand.component.scss',
})
export class ProductInBrandComponent {
  productWithBrand: IProduct[] = [];
  ProductParams = new ProductParams();
  brandType?: IProduct;

  constructor(
    private _productService: ProductService,
    private _activatedRoute: ActivatedRoute,
  ) {}

  ngOnInit() {
    const id = (this.ProductParams.brandId =
      this._activatedRoute.snapshot.params['id']);
    this.getProductWithBrand(id);
  }

  getProductWithBrand(id: number): void {
    this._productService.getProducts(this.ProductParams).subscribe({
      next: (response) => {
        this.productWithBrand = response.products;
        if (this.productWithBrand.length > 0) {
          this.brandType = this.productWithBrand[0];
        }
        console.log(response.products);
      },
      error: (error) => console.log(error),
    });
  }
}
