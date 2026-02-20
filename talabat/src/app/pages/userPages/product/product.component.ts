import { Component } from '@angular/core';
import { ProductService } from '../../../../Core/Services/UserServices/product.service';
import { IProduct } from '../../../../Core/Interfaces/UserInterfaces/iproduct';
import { CardComponent } from '../../../../Core/Shared/card/card.component';
import { InputIconModule } from 'primeng/inputicon';
import { IconFieldModule } from 'primeng/iconfield';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { ProductParams } from '../../../../Core/Interfaces/UserInterfaces/product-params';
import { DataViewModule } from 'primeng/dataview';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
@Component({
  selector: 'app-product',
  standalone: true,
  imports: [
    CardComponent,
    InputIconModule,
    IconFieldModule,
    InputTextModule,
    FormsModule,
    DataViewModule,
    PaginatorModule,
  ],
  templateUrl: './product.component.html',
  styleUrl: './product.component.scss',
})
export class ProductComponent {
  constructor(private _productService: ProductService) {}
  allProducts: IProduct[] = [];
  productCount: number = 0;
  private searchInput = new Subject<string>();
  productParams = new ProductParams();

  pageSize = 8;
  pageIndex = 1;
  first: number = 0;
  searchValue = '';

  ngOnInit() {
    this.getAllProduct();
    // Search product
    this.searchInput
      .pipe(debounceTime(500), distinctUntilChanged())
      .subscribe((search) => {
        this.searchValue = search;
        this.pageIndex = 1;
        this.getAllProduct();
      });
  }

  getAllProduct() {
    this.productParams.pageIndex = this.pageIndex;
    this.productParams.pageSize = this.pageSize;
    this.productParams.search = this.searchValue;
    this._productService.getProducts(this.productParams).subscribe({
      next: (response) => {
        this.productCount = response.count;
        this.allProducts = response.products.map((product: IProduct) => ({
          ...product,
          // isAddedToCart: this._cartService.isAddToCart(product) || false,
        }));
      },
      error: (error) => console.log(error),
    });
  }

  // Pagination
  onPageChange(event: PaginatorState) {
    this.pageIndex = (event.page ?? 0) + 1;

    this.pageSize = event.rows ?? 8;

    this.first = event.first ?? 0;

    this.getAllProduct();

    window.scrollTo({
      top: 0,
      behavior: 'smooth',
    });
  }

  onSearchChange(event: any) {
    this.searchInput.next(event.target.value);
  }
}
