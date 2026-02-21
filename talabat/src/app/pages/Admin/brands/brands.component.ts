import { Component } from '@angular/core';
import { AdminService } from '../../../../Core/Services/AdminServices/admin.service';
import { IBrand } from '../../../../Core/Interfaces/UserInterfaces/ibrand';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { CommonModule } from '@angular/common';
import { TagModule } from 'primeng/tag';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { NotificationsService } from '../../../../Core/Services/notifications.service';
import { FormsModule } from '@angular/forms';
import { ShowListComponent } from '../../../../Core/Shared/Admin/show-list/show-list.component';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-brands',
  standalone: true,
  imports: [
    TableModule,
    ToastModule,
    CommonModule,
    TagModule,
    DropdownModule,
    ButtonModule,
    InputTextModule,
    FormsModule,
    ShowListComponent,
  ],
  templateUrl: './brands.component.html',
  styleUrl: './brands.component.scss',
  providers: [ConfirmationService],
})
export class BrandsComponent {
  brands!: IBrand[];
  brand!: IBrand;
  List = 'Brands';

  constructor(
    private _adminService: AdminService,
    private _notifications: NotificationsService,
    private confirmationService: ConfirmationService,
    private _notificationService: NotificationsService,
  ) {}

  ngOnInit() {
    this.getBrands();
  }

  getBrands(): void {
    this._adminService.getbrand().subscribe({
      next: (res: IBrand[]) => {
        this.brands = res;
        console.log(this.brands);
      },
    });
  }

  saveBrand(data: IBrand): void {
    this._adminService.editBrand(data).subscribe({
      next: (res: IBrand) => {
        this._notifications.showSuccedded(
          'Update',
          'Brand Updated Successfully',
        );
      },
    });
  }

  addBrand(data: IBrand): void {
    this._adminService.addBrand(data).subscribe({
      next: (res: IBrand) => {
        this._notifications.showSuccedded('Add', 'Brand Added Successfully');
      },
    });
  }

  deleteBrand(data: IBrand): void {
    this._adminService.deleteBrand(data.id).subscribe({
      next: (res: string) => {
        this._notifications.showSuccedded(
          'Delete',
          'Brand Deleted Successfully',
        );
      },
    });
    this.confirmationService.confirm({
      message: `Are you sure you want to delete <b>${data.name}</b>?`,
      header: 'Confirm Delete Product',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Yes',
      rejectLabel: 'Cancel',
      accept: () => {
        this._adminService.deleteProduct(data.id).subscribe({
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
}
