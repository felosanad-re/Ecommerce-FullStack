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
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';
import { OrderStatus } from '../../../../Core/Interfaces/order-status';
import { IupdateOrderStatus } from '../../../../Core/Interfaces/iupdate-order-status';
import { Order } from '@stripe/stripe-js';
import { IPagination } from '../../../../Core/Interfaces/UserInterfaces/ipagination';
import { FileUploadModule } from 'primeng/fileupload';
import { ToolbarModule } from 'primeng/toolbar';
import { ImportResult } from '../../../../Core/Interfaces/import-result';

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
    DialogModule,
    InputTextModule,
    FormsModule,
    DropdownModule,
    FileUploadModule,
    ToolbarModule,
  ],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss',
})
export class OrdersComponent {
  orders: Iorder[] = [];
  selecteOrder!: number;
  visible: boolean = false;
  expandedRows = {};
  selectedZipFile: File | null = null;
  orderStatus = Object.values(OrderStatus)
    .filter((key) => isNaN(Number(key)))
    .map((key) => ({
      label: key,
      value: OrderStatus[key as keyof typeof OrderStatus],
    }));

  selectedOrderStatus!: OrderStatus;
  constructor(
    private _adminService: AdminService,
    private _notificationService: NotificationsService,
  ) {}

  ngOnInit(): void {
    this.getOrders();
  }

  getOrders() {
    this._adminService.getOrders().subscribe({
      next: (res: IPagination<Iorder>) => {
        this.orders = res.data;
        console.log(this.orders);
      },
    });
  }

  expandAll() {
    // open all rows by order Id
    this.expandedRows = this.orders.reduce(
      (acc, p: Iorder) => {
        acc[p.id] = true;
        return acc;
      },
      {} as { [key: string]: boolean },
    );
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
  showDialog(id: number) {
    this.selecteOrder = id;
    this.visible = true;
  }

  editOrderStatus(): void {
    // debugger;
    const data: IupdateOrderStatus = {
      status: Number(this.selectedOrderStatus!),
      id: this.selecteOrder,
    };
    console.log(data);
    this._adminService.updateOrderStatus(data).subscribe({
      next: (res) => {
        console.log(res);
        this.getOrders();
      },
    });
    this.visible = false;
  }

  exportOrders() {
    this._adminService.exportOrders().subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = 'Orders.xlsx';
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        this._notificationService.showError(
          'Export Orders',
          'Failed to export orders',
        );
        console.error(err);
      },
    });
  }

  onZipFileSelect(event: any) {
    const file = event.files[0];
    if (file) {
      this.selectedZipFile = file;
      this._notificationService.showSuccedded(
        'Images Zip',
        `Zip file "${file.name}" selected`,
      );
    }
  }

  onZipFileClear() {
    this.selectedZipFile = null;
  }

  importOrders(event: any) {
    const file = event.files[0];
    if (!file) {
      this._notificationService.showError('Import Orders', 'No file selected');
      return;
    }
    this._adminService
      .importOrders(file, this.selectedZipFile ?? undefined)
      .subscribe({
        next: (res: ImportResult<unknown>) => {
          const details = `Total Rows: ${res.totalRows} | Added: ${res.addedCount} | Skipped Duplicates: ${res.skippedDuplicates}`;
          if (res.errors && res.errors.length > 0) {
            const errorDetails = res.errors.join('\n');
            this._notificationService.showError(
              'Import Orders - Errors',
              `${details}\nErrors:\n${errorDetails}`,
            );
          } else {
            this._notificationService.showSuccedded(
              'Import Orders',
              `${res.message}\n${details}`,
            );
          }
          this.selectedZipFile = null;
          this.getOrders();
        },
        error: (err) => {
          this._notificationService.showError(
            'Import Orders',
            'Failed to import orders',
          );
          console.error(err);
        },
      });
  }
}
