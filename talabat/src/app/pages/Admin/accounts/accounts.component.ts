import { Component } from '@angular/core';
import { ApplicationUser } from '../../../../Core/Interfaces/application-user';
import { AdminService } from '../../../../Core/Services/AdminServices/admin.service';
import { NotificationsService } from '../../../../Core/Services/notifications.service';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { RatingModule } from 'primeng/rating';
import { CommonModule } from '@angular/common';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { FormsModule } from '@angular/forms';
import { InputNumberModule } from 'primeng/inputnumber';

@Component({
  selector: 'app-accounts',
  standalone: true,
  imports: [
    TableModule,
    TagModule,
    RatingModule,
    CommonModule,
    ConfirmDialogModule,
    ToastModule,
    ButtonModule,
    DialogModule,
    FormsModule,
    InputNumberModule,
  ],
  templateUrl: './accounts.component.html',
  styleUrl: './accounts.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class AccountsComponent {
  users!: ApplicationUser[];
  user!: ApplicationUser;

  // For Dialog
  submitted: boolean = false;
  dataDialog: boolean = false;

  lockDialog: boolean = false;
  selectedUserId!: string;
  lockDays!: number;
  constructor(
    private _adminService: AdminService,
    private confirmationService: ConfirmationService,
    private _notification: NotificationsService,
  ) {}

  ngOnInit(): void {
    this.getAccounts();
  }
  getAccounts(): void {
    this._adminService.getAccounts().subscribe({
      next: (res: ApplicationUser[]) => {
        this.users = res;
        console.log(this.users);
      },
    });
  }

  deleteUser(id: string): void {
    this.confirmationService.confirm({
      message: 'Are you sure that you want to proceed?',
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      acceptIcon: 'none',
      rejectIcon: 'none',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        console.log(id);
        this._adminService.deleteAccount(id).subscribe({
          next: (res) => {
            this._notification.showSuccedded('Delete', res.message);
            this.getAccounts();
          },
        });
      },
    });
  }

  activeUser(id: string): void {
    this.confirmationService.confirm({
      message: 'Are you sure that you want to proceed?',
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      acceptIcon: 'none',
      rejectIcon: 'none',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        console.log(id);
        this._adminService.activeAccount(id).subscribe({
          next: (res) => {
            this._notification.showSuccedded('Active', res.message);
            this.getAccounts();
          },
        });
      },
    });
  }

  openLockDialog(id: string) {
    this.selectedUserId = id;
    this.lockDays = 0;
    this.lockDialog = true;
    console.log(this.selectedUserId);
  }
  confirmLock() {
    console.log(this.selectedUserId);
    console.log(this.lockDays);
    this._adminService
      .lockAccount(this.selectedUserId, this.lockDays)
      .subscribe({
        next: (res) => {
          this._notification.showSuccedded('Lock', res.lockDays);
          this.lockDialog = false;
          this.getAccounts();
        },
      });
  }
}
