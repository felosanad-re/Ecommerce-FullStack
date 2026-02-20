import { NotificationsService } from './../../../../Core/Services/notifications.service';
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { AuthModule } from '../../../../Core/modules/auth/auth.module';
import { AuthService } from '../../../../Core/Services/ِAuthServices/auth.service';
import { IForgetPassword } from '../../../../Core/Interfaces/authInterfaces/iforget-password';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { NgxSpinnerService } from 'ngx-spinner';
import { GetAccountService } from '../../../../Core/Services/ِAuthServices/get-account.service';

@Component({
  selector: 'app-forget-passowrd',
  standalone: true,
  imports: [AuthModule],
  templateUrl: './forget-passowrd.component.html',
  styleUrl: './forget-passowrd.component.scss',
})
export class ForgetPassowrdComponent {
  email!: FormControl;
  forgetPasswordForm!: FormGroup;

  constructor(
    private _authService: AuthService,
    private _notificationsService: NotificationsService,
    private _spinner: NgxSpinnerService,
    private _router: Router,
    private _getAccountService: GetAccountService, //
  ) {
    this.initiateFormControlers();
    this.initiateFormGroubes();
  }
  initiateFormControlers(): void {
    this.email = new FormControl('', [Validators.email, Validators.required]);
  }

  initiateFormGroubes(): void {
    this.forgetPasswordForm = new FormGroup({
      email: this.email,
    });
  }

  // submitForm
  submitForm(): void {
    debugger;
    if (this.forgetPasswordForm.valid) {
      this.forgetPassApi(this.forgetPasswordForm.value);
    } else {
      this.forgetPasswordForm.markAllAsTouched();
      Object.keys(this.forgetPasswordForm.controls).forEach((control) =>
        this.forgetPasswordForm.controls[control].markAsDirty(),
      );
    }
  }

  // Call Api
  forgetPassApi(dataForgetPass: IForgetPassword): void {
    this._spinner.show(); // Loading Screan
    // Call Services To Send Request To Api
    this._authService.forgetPassword(dataForgetPass).subscribe({
      next: (res) => {
        if (res.userId) {
          this._notificationsService.showSuccedded(
            'Check Email',
            'You Recived Code In your Email',
          );
          // console.log(dataForgetPass.email);
          // debugger;
          // Set email and resetToken in session storage for Check Code
          this._getAccountService.set({
            email: dataForgetPass.email, // set email in session storage
            resetToken: '',
          }); // set email in service
          this._router.navigate(['verify']);
        }
        this._spinner.hide();
      },
      error: (error) => {
        this._notificationsService.showError('Error', error.error.error);
        console.log(error);
        this._spinner.hide();
      },
    });
  }
}
