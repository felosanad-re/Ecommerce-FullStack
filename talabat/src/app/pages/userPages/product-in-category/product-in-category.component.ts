import { Component } from '@angular/core';
import { IProduct } from '../../../../Core/Interfaces/UserInterfaces/iproduct';
import { ProductService } from '../../../../Core/Services/UserServices/product.service';
import { ProductParams } from '../../../../Core/Interfaces/UserInterfaces/product-params';
import { CardComponent } from '../../../../Core/Shared/card/card.component';
import { ActivatedRoute } from '@angular/router';
import { ICategory } from '../../../../Core/Interfaces/UserInterfaces/icategory';

@Component({
  selector: 'app-product-in-category',
  standalone: true,
  imports: [CardComponent],
  templateUrl: './product-in-category.component.html',
  styleUrl: './product-in-category.component.scss',
})
export class ProductInCategoryComponent {
  productWithCategory: IProduct[] = [];
  productParams = new ProductParams();
  categoryType?: IProduct;
  constructor(
    private _productService: ProductService,
    private _activatedRoute: ActivatedRoute,
  ) {}
  ngOnInit() {
    console.log(this.categoryType?.category);
    const id = (this.productParams.categoryId =
      this._activatedRoute.snapshot.params['id']);
    this.getProductWithCategory(id);
  }

  getProductWithCategory(id: number): void {
    this._productService.getProducts(this.productParams).subscribe({
      next: (response) => {
        this.productWithCategory = response.products;
        console.log(response.products);
        if (this.productWithCategory.length > 0) {
          this.categoryType = this.productWithCategory[0]; // Get Category name
        }
        console.log(this.categoryType);
      },
      error: (error) => console.log(error),
    });
  }
}
