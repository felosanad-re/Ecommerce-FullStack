import { Component } from '@angular/core';
import { Iorder } from '../../../../Core/Interfaces/UserInterfaces/iorder';
import { AdminService } from '../../../../Core/Services/AdminServices/admin.service';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { RatingModule } from 'primeng/rating';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { TableRowCollapseEvent, TableRowExpandEvent } from 'primeng/table';
import { NotificationsService } from '../../../../Core/Services/notifications.service';
@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [
    TableModule,
    TagModule,
    ToastModule,
    RatingModule,
    ButtonModule,
    CommonModule,
  ],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss',
})
export class OrdersComponent {
  orders: Iorder[] = [];

  expandedRows = {};
  constructor(
    private _adminService: AdminService,
    private _notificationService: NotificationsService,
  ) {}

  ngOnInit(): void {
    this.getOrders();
  }

  getOrders() {
    this._adminService.getOrders().subscribe({
      next: (res: Iorder[]) => {
        console.log(res);
        this.orders = res;
      },
    });
  }

  expandAll() {
    // this.expandedRows = this.products.reduce((acc, p) => (acc[p.id] = true) && acc, {});
  }

  collapseAll() {
    this.expandedRows = {};
  }

  // getSeverity(status: string) {
  //   switch (status) {
  //     case 'INSTOCK':
  //       return 'success';
  //     case 'LOWSTOCK':
  //       return 'warning';
  //     case 'OUTOFSTOCK':
  //       return 'danger';
  //   }
  // }

  // getStatusSeverity(status: string) {
  //   switch (status) {
  //     case 'PENDING':
  //       return 'warning';
  //     case 'DELIVERED':
  //       return 'success';
  //     case 'CANCELLED':
  //       return 'danger';
  //   }
  // }

  onRowExpand(event: TableRowExpandEvent) {
    this._notificationService.showSuccedded(
      'Product Expanded',
      event.data.name,
    );
  }

  onRowCollapse(event: TableRowCollapseEvent) {
    this._notificationService.showSuccedded(
      'Product Collapsed',
      event.data.name,
    );
  }
}
