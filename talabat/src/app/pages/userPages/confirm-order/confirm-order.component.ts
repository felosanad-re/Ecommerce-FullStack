import { Component } from '@angular/core';
import { CreateOrderService } from '../../../../Core/Services/UserServices/create-order.service';
import { NotificationsService } from '../../../../Core/Services/notifications.service';
import { RouterLink } from '@angular/router';
import { Iorder } from '../../../../Core/Interfaces/UserInterfaces/iorder';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';

@Component({
  selector: 'app-confirm-order',
  standalone: true,
  imports: [TableModule, ButtonModule, RouterLink, DialogModule],
  templateUrl: './confirm-order.component.html',
  styleUrl: './confirm-order.component.scss',
})
export class ConfirmOrderComponent {
  orderData: Iorder[] = [];

  constructor(
    private _CreateOrderService: CreateOrderService,
    private _notificationsServicce: NotificationsService,
  ) {}
  ngOnInit(): void {
    this._CreateOrderService.getOrdersDetails().subscribe((data) => {
      if (data) {
        this.orderData = data;
        console.log(data);
      } else {
        console.log('there is no data to show :(');
      }
    });
  }

  deleteOrder(id: number): void {
    this._CreateOrderService.deleteOrder(id).subscribe({
      next: (res) => {
        this._notificationsServicce.showSuccedded('Delete', res.message);
        // localStorage.removeItem()
        window.location.reload();
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  visible: boolean = false;

  showDialog() {
    this.visible = true;
  }
}
