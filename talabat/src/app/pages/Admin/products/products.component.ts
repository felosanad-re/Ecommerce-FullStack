import { Component } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { IProduct } from '../../../../Core/Interfaces/UserInterfaces/iproduct';
import { AdminService } from '../../../../Core/Services/AdminServices/admin.service';
import { ProductParams } from '../../../../Core/Interfaces/UserInterfaces/product-params';
import { IPagination } from '../../../../Core/Interfaces/UserInterfaces/ipagination';
import { IBrand } from '../../../../Core/Interfaces/UserInterfaces/ibrand';
import { ICategory } from '../../../../Core/Interfaces/UserInterfaces/icategory';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { TableComponent } from '../../../../Core/Shared/Admin/table/table.component';
@Component({
  selector: 'app-products',
  standalone: true,
  imports: [PaginatorModule, TableComponent],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss',
  providers: [MessageService, ConfirmationService],
  styles: [
    `
      :host ::ng-deep .p-dialog .product-image {
        width: 150px;
        margin: 0 auto 2rem auto;
        display: block;
      }
    `,
  ],
})
export class ProductsComponent {
  product: IProduct[] = [];
  brand!: IBrand[];
  category!: ICategory[];
  countOfProduct!: number;
  productParams = new ProductParams();
  pageIndex: number = 1;
  pageSize: number = 8;
  constructor(private _adminService: AdminService) {}
  ngOnInit() {
    // get product and search product
    this.loadProducts();

    // Get Brands
    this._adminService.getBrand().subscribe({
      next: (res: IBrand[]) => {
        this.brand = res;
      },
      error: (error) => console.log(error),
    });
    // Get Categories
    this._adminService.getCategory().subscribe({
      next: (res: ICategory[]) => {
        this.category = res;
      },
      error: (error) => console.log(error),
    });
  }

  loadProducts() {
    this.productParams.isDeleted = false; // not delete products
    this.productParams.isInStock = false; // product in
    this.productParams.pageIndex = this.pageIndex;
    this.productParams.pageSize = this.pageSize;
    this._adminService.getProducts(this.productParams).subscribe({
      next: (res: IPagination<IProduct>) => {
        console.log(res);
        this.product = res.data;
        this.countOfProduct = res.count;
      },
      error: (error) => console.log(error),
    });
  }
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
