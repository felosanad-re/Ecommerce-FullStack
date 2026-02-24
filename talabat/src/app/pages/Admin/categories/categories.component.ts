import { Component } from '@angular/core';
import { AdminService } from '../../../../Core/Services/AdminServices/admin.service';
import { ICategory } from '../../../../Core/Interfaces/UserInterfaces/icategory';
import { ShowListComponent } from '../../../../Core/Shared/Admin/show-list/show-list.component';
import { NotificationsService } from '../../../../Core/Services/notifications.service';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [ShowListComponent],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss',
  providers: [ConfirmationService],
})
export class CategoriesComponent {
  constructor(
    private _adminService: AdminService,
    private confirmationService: ConfirmationService,
    private _notificationService: NotificationsService,
  ) {}

  list = 'Categories';
  categories!: ICategory[];
  ngOnInit() {
    this.getCategories();
  }
  getCategories(): void {
    this._adminService.getCategory().subscribe((next) => {
      console.log(next);
      this.categories = next;
    });
  }

  addCategory(data: ICategory): void {
    this._adminService.addCategory(data).subscribe((next) => {
      this._notificationService.showSuccedded(
        'Add',
        'Category Added Successfully',
      );
    });
  }

  editCategory(data: ICategory): void {
    this._adminService.editCategory(data).subscribe((next) => {
      this._notificationService.showSuccedded(
        'Update',
        'Category Updated Successfully',
      );
    });
  }

  deleteCategory(data: ICategory): void {
    this.confirmationService.confirm({
      message: `Are you sure you want to delete <b>${data.name}</b>?`,
      header: 'Confirm Delete Category',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Yes',
      rejectLabel: 'Cancel',
      accept: () => {
        this._adminService.deleteCategory(data.id).subscribe({
          next: () => {
            this._notificationService.showSuccedded(
              'Delete Category',
              'Category Deleted Succsesfully',
            );
          },
          error: (err) => {
            this._notificationService.showError(
              'Delete Category',
              'There Is A Probelm',
            );
            console.error(err);
          },
        });
      },
    });
  }
}
