import { Component } from '@angular/core';
import { AdminService } from '../../../../Core/Services/AdminServices/admin.service';
import { IProduct } from '../../../../Core/Interfaces/UserInterfaces/iproduct';
import { ProductParams } from '../../../../Core/Interfaces/UserInterfaces/product-params';
import { IPagination } from '../../../../Core/Interfaces/UserInterfaces/ipagination';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { RippleModule } from 'primeng/ripple';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CommonModule } from '@angular/common';
import { FileUploadModule } from 'primeng/fileupload';
import { DropdownModule } from 'primeng/dropdown';
import { TagModule } from 'primeng/tag';
import { RadioButtonModule } from 'primeng/radiobutton';
import { RatingModule } from 'primeng/rating';
import { FormsModule } from '@angular/forms';
import { InputNumberModule } from 'primeng/inputnumber';
import { IBrand } from '../../../../Core/Interfaces/UserInterfaces/ibrand';
import { ICategory } from '../../../../Core/Interfaces/UserInterfaces/icategory';
import { NotificationsService } from '../../../../Core/Services/notifications.service';
import { StockTypes } from '../../../../Core/Interfaces/stock-types';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    TableModule,
    DialogModule,
    RippleModule,
    ButtonModule,
    ToastModule,
    ToolbarModule,
    ConfirmDialogModule,
    InputTextModule,
    InputTextareaModule,
    CommonModule,
    FileUploadModule,
    DropdownModule,
    TagModule,
    RadioButtonModule,
    RatingModule,
    InputTextModule,
    FormsModule,
    InputNumberModule,
  ],
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
  constructor(
    private _adminService: AdminService,
    private confirmationService: ConfirmationService,
    private _notificationService: NotificationsService,
  ) {}
  products!: IProduct[];
  product!: IProduct;
  brands!: IBrand[];
  categories!: ICategory[];
  countOfProduct!: number;
  selectedProducts!: IProduct[] | null;
  isAddProduct!: boolean;

  searchValue!: string;
  searchSubject = new Subject<string>();

  submitted: boolean = false;
  productDialog: boolean = false;
  statuses!: StockTypes[];
  productParams = new ProductParams();
  ngOnInit() {
    // get product and search product
    this.productParams.isDeleted = false; // not delete products
    this.productParams.isInStock = false; // product in
    this.productParams.pageIndex = 1;
    this._adminService.getProducts(this.productParams).subscribe({
      next: (res: IPagination<IProduct>) => {
        this.products = res.products;
        this.countOfProduct = res.count;
        console.log(res);
      },
      error: (error) => console.log(error),
    });
    this.searchSubject.next('');
    this.searchSubject
      .pipe(debounceTime(500), distinctUntilChanged())
      .subscribe((value) => {
        this.productParams.search = value ?? undefined;
      });
    // Get Brands
    this._adminService.getbrand().subscribe({
      next: (res: IBrand[]) => {
        this.brands = res;
        console.log(this.brands);
      },
      error: (error) => console.log(error),
    });
    // Get Categories
    this._adminService.getCategory().subscribe({
      next: (res: ICategory[]) => {
        this.categories = res;
        console.log(this.categories);
      },
      error: (error) => console.log(error),
    });
  }

  // Add Product
  openNew() {
    this.product = {} as IProduct;
    this.isAddProduct = true;
    this.submitted = false;
    this.productDialog = true;
  }

  editProduct(product: IProduct) {
    this.product = { ...product };
    this.isAddProduct = false;
    this.productDialog = true;
  }

  deleteProduct(product: IProduct) {
    this.confirmationService.confirm({
      message: `Are you sure you want to delete <b>${product.name}</b>?`,
      header: 'Confirm Delete Product',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Yes',
      rejectLabel: 'Cancel',
      accept: () => {
        this._adminService.deleteProduct(product.id).subscribe({
          next: () => {
            this._notificationService.showSuccedded(
              'DeleteProduct',
              'Product Deleted Succsesfully',
            );
          },
          error: (err) => {
            this._notificationService.showError(
              'DeleteProduct',
              'There Is A Probelm',
            );
            console.error(err);
          },
        });
      },
    });
  }

  hideDialog() {
    this.productDialog = false;
    this.submitted = false;
  }

  getSeverity(stockType: StockTypes) {
    switch (stockType) {
      case StockTypes.INSTOCK:
        return 'success';
      case StockTypes.LOWSTOCK:
        return 'warning';
      case StockTypes.OUTOFSTOCK:
        return 'danger';
    }
  }

  save() {
    this.submitted = true;

    if (this.product.name?.trim()) {
      // Send Request To back To Add Product
      if (this.isAddProduct) {
        this._adminService.addProduct(this.product).subscribe({
          next: () => {
            this._notificationService.showSuccedded(
              'AddProduct',
              'Product Added Succsesfully',
            );
            window.location.reload();
          },
          error: (err) => {
            this._notificationService.showError(
              'AddProduct',
              'There Is A Probelm',
            );
            console.error(err);
          },
        });
      } else {
        // edit product
        this._adminService.editProduct(this.product).subscribe({
          next: () => {
            this._notificationService.showSuccedded(
              'UpdateProduct',
              'Product Updated Succsesfully',
            );
            window.location.reload();
          },
          error: (err) => {
            this._notificationService.showError(
              'UpdateProduct',
              'There Is A Probelm',
            );
            console.error(err);
          },
        });
      }

      this.products = [...this.products];
      this.productDialog = false;
      this.product = {} as IProduct;
    }
  }

  onSearch(event: any) {
    const value = event.target.value;
    this.searchSubject.next(value.trim() === '' ? null : value);
  }
}
