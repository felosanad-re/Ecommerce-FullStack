import { Component } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TapleComponent } from '../../../../Core/Shared/Admin/taple/taple.component';
import { IProduct } from '../../../../Core/Interfaces/UserInterfaces/iproduct';
import { AdminService } from '../../../../Core/Services/AdminServices/admin.service';
import { ProductParams } from '../../../../Core/Interfaces/UserInterfaces/product-params';
import { IPagination } from '../../../../Core/Interfaces/UserInterfaces/ipagination';
import { IBrand } from '../../../../Core/Interfaces/UserInterfaces/ibrand';
import { ICategory } from '../../../../Core/Interfaces/UserInterfaces/icategory';
@Component({
  selector: 'app-products',
  standalone: true,
  imports: [TapleComponent],
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
  constructor(private _adminService: AdminService) {}
  ngOnInit() {
    // get product and search product
    this.productParams.isDeleted = false; // not delete products
    this.productParams.isInStock = false; // product in
    this.productParams.pageIndex = 1;
    this._adminService.getProducts(this.productParams).subscribe({
      next: (res: IPagination<IProduct>) => {
        this.product = res.products;
        this.countOfProduct = res.count;
      },
      error: (error) => console.log(error),
    });

    // Get Brands
    this._adminService.getbrand().subscribe({
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
}
