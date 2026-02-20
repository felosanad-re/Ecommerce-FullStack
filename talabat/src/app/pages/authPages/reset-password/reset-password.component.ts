import { Component } from '@angular/core';
import { AuthModule } from '../../../../Core/modules/auth/auth.module';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../../../Core/Services/ِAuthServices/auth.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router } from '@angular/router';
import { GetAccountService } from '../../../../Core/Services/ِAuthServices/get-account.service';
import { IResetPassword } from '../../../../Core/Interfaces/authInterfaces/ireset-password';
import { NotificationsService } from '../../../../Core/Services/notifications.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [AuthModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss',
})
export class ResetPasswordComponent {
  password!: FormControl;
  confirmPassword!: FormControl;
  resetPassForm!: FormGroup;
  email!: string; // API Need Email
  resetPassToken!: string; // API Need Email

  ngOnInit(): void {
    // debugger;
    const data = this._getAccountService.get(); // get data from session storage
    if (!data) {
      this._router.navigate(['forgetPassword']);
      return;
    }
    this.email = data.email;
    this.resetPassToken = data.resetToken;
  }
  constructor(
    private _authService: AuthService,
    private _notificationsService: NotificationsService,
    private _getAccountService: GetAccountService,
    private _spinner: NgxSpinnerService,
    private _router: Router,
  ) {
    this.initiatFormControls();
    this.initiatFormGroubs();
  }

  initiatFormControls(): void {
    this.password = new FormControl('', [
      Validators.required,
      Validators.minLength(5),
    ]);
    this.confirmPassword = new FormControl('', [
      Validators.required,
      this.checkPass(this.password),
    ]);
  }

  initiatFormGroubs(): void {
    this.resetPassForm = new FormGroup({
      password: this.password,
      confirmPassword: this.confirmPassword,
    });
  }

  // Custom Validation
  checkPass(pass: AbstractControl): ValidatorFn {
    return (rePass: AbstractControl): null | { [key: string]: boolean } => {
      if (pass.value === rePass.value) {
        return null;
      } else {
        return { notSame: true };
      }
    };
  }

  submitForm(): void {
    debugger;
    if (this.resetPassForm.valid) {
      const data: IResetPassword = {
        newPassword: this.resetPassForm.value.password,
        confairmPassword: this.resetPassForm.value.confirmPassword,
        resetToken: this.resetPassToken,
        email: this.email,
      };
      console.log(data);
      this.resetPassApi(data);
    } else {
      this.resetPassForm.markAllAsTouched();
      Object.keys(this.resetPassForm.controls).forEach((control) => {
        this.resetPassForm.controls[control].markAsDirty();
      });
    }
  }

  resetPassApi(data: IResetPassword): void {
    // debugger;
    this._spinner.show();
    this._authService.resetPassword(data).subscribe({
      next: (response) => {
        if (response.message) {
          this._notificationsService.showSuccedded(
            'Reset Password',
            response.message,
          );
          this._spinner.hide();
          this._getAccountService.delete(); // delete email and resetToken
          this._router.navigate(['login']);
        }
      },
      error: (error) => {
        this._notificationsService.showError('Reset Password', error.error);
        this._spinner.hide();
      },
    });
  }
}
