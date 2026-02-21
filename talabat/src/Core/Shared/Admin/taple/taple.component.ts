import { Component, Input, input } from '@angular/core';
import { AdminService } from '../../../../Core/Services/AdminServices/admin.service';
import { IProduct } from '../../../../Core/Interfaces/UserInterfaces/iproduct';
import { ProductParams } from '../../../../Core/Interfaces/UserInterfaces/product-params';
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
import { Subject } from 'rxjs';
@Component({
  selector: 'app-taple',
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
  templateUrl: './taple.component.html',
  styleUrl: './taple.component.scss',
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
export class TapleComponent {
  constructor(
    private _adminService: AdminService,
    private confirmationService: ConfirmationService,
    private _notificationService: NotificationsService,
  ) {}

  @Input({ required: true }) products!: IProduct[];
  product!: IProduct;
  @Input({ required: true }) brands!: IBrand[];
  @Input({ required: true }) categories!: ICategory[];
  countOfProduct!: number;
  selectedProducts!: IProduct[] | null;
  isAddProduct!: boolean;

  searchValue!: string;
  searchSubject = new Subject<string>();

  submitted: boolean = false;
  productDialog: boolean = false;
  statuses!: StockTypes[];
  productParams = new ProductParams();
  imageFile: File | null = null; // Add Product
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

  // Select Product image
  onFileSelect(event: any) {
    const file = event.files?.[0];
    if (!file) return;

    // تحقق من الحجم والنوع (اختياري)
    if (file.size > 2000000) {
      this._notificationService.showError(
        'Add Product Error',
        'the size of picture is much',
      );
      return;
    }
    this.imageFile = file;
  }

  addProduct() {
    this.submitted = true;

    if (
      !this.product.name?.trim() ||
      !this.product.descripaion?.trim() ||
      !this.product.categoryId ||
      !this.product.brandId
    ) {
      return;
    }

    const data = {
      name: this.product.name.trim(),
      descripaion: this.product.descripaion.trim(),
      price: this.product.price || 0,
      brandId: this.product.brandId,
      categoryId: this.product.categoryId,
      stock: this.product.stock || 0,
    };

    this._adminService.addProduct(data, this.imageFile).subscribe({
      next: () => {
        this._notificationService.showSuccedded(
          'AddProduct',
          'product Added Successfully',
        );
        this.productDialog = false;
        this.resetForm();
        window.location.reload(); // أو تحديث الجدول بدون reload لو ممكن
      },
      error: (err) => {
        this._notificationService.showError('AddProduct', 'add error');
        console.error(err);
      },
    });
  }

  resetForm() {
    this.product = {} as IProduct;
    this.imageFile = null;
    this.submitted = false;
  }

  onSearch(event: any) {
    const value = event.target.value;
    this.searchSubject.next(value.trim() === '' ? null : value);
  }
}
