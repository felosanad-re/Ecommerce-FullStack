import {
  Component,
  ElementRef,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { LayoutService } from '../../../../Core/Services/app.layout.service';
import { MenuItem } from 'primeng/api';
import { CommonModule } from '@angular/common';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';
import { BadgeModule } from 'primeng/badge';
import { SignalRService } from '../../../../Core/Services/signal-r.service';
import { Notifications } from '../../../../Core/Interfaces/Notifications';
import { AdminService } from '../../../../Core/Services/AdminServices/admin.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, OverlayPanelModule, ButtonModule, BadgeModule],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class TopbarComponent {
  status: Notifications[] = [];
  data: Date = new Date();
  constructor(
    public layoutService: LayoutService,
    private _adminService: AdminService,
    private _signalRService: SignalRService,
  ) {}
  notificationCount: string = '0';

  ngOnInit() {
    this._signalRService.startConnection();
    this._signalRService.addMessageListener();

    this._adminService.getAllNotifications().subscribe((data) => {
      console.log(data);
      this.status = data;
      this.notificationCount = this.status.length.toString();
    });

    this._signalRService.notifications$.subscribe((data) => {
      this.status = data;
    });
  }
  removeNotification(index: number) {
    this.status.splice(index, 1);
  }

  items!: MenuItem[];

  @ViewChild('menubutton') menuButton!: ElementRef;

  @ViewChild('topbarmenubutton') topbarMenuButton!: ElementRef;

  @ViewChild('topbarmenu') menu!: ElementRef;
}
