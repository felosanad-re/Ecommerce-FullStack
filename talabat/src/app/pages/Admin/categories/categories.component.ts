import { Component } from '@angular/core';
import { AdminService } from '../../../../Core/Services/AdminServices/admin.service';
import { ICategory } from '../../../../Core/Interfaces/UserInterfaces/icategory';
import { ShowListComponent } from '../../../../Core/Shared/Admin/show-list/show-list.component';
import { NotificationsService } from '../../../../Core/Services/notifications.service';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [ShowListComponent],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss',
})
export class CategoriesComponent {
  constructor(
    private _adminService: AdminService,
    private _notifications: NotificationsService,
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
      this._notifications.showSuccedded('Add', 'Category Added Successfully');
    });
  }

  editCategory(data: ICategory): void {
    this._adminService.editCategory(data).subscribe((next) => {
      this._notifications.showSuccedded(
        'Update',
        'Category Updated Successfully',
      );
    });
  }

  deleteCategory(data: ICategory): void {
    this._adminService.deleteCategory(data.id).subscribe((next) => {
      this._notifications.showSuccedded(
        'Delete',
        'Category Deleted Successfully',
      );
    });
  }
}
