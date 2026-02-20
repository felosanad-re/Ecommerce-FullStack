import { Component } from '@angular/core';
import { InputOtpModule } from 'primeng/inputotp';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Button } from 'primeng/button';
import { ICheckCode } from '../../../../Core/Interfaces/authInterfaces/icheck-code';
import { AuthService } from '../../../../Core/Services/ِAuthServices/auth.service';
import { MessageService } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';
import { GetAccountService } from '../../../../Core/Services/ِAuthServices/get-account.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastModule } from 'primeng/toast';
import { NotificationsService } from '../../../../Core/Services/notifications.service';

@Component({
  selector: 'app-verify',
  standalone: true,
  imports: [
    FormsModule,
    InputOtpModule,
    Button,
    ReactiveFormsModule,
    ToastModule,
  ],
  templateUrl: './verify.component.html',
  styleUrl: './verify.component.scss',
  providers: [MessageService],
})
export class VerifyComponent {
  code!: FormControl;
  verifyCode!: FormGroup;
  // for Api
  email!: string;

  constructor(
    private _authService: AuthService,
    private _notificationsService: NotificationsService,
    private _getAccountService: GetAccountService,
    private _spinner: NgxSpinnerService,
    private _router: Router,
  ) {
    this.initiateFormControlers();
    this.initiateFormGroubs();
  }

  ngOnInit() {
    // debugger;
    // Data --> email & resetToken [BackEnd]
    const data = this._getAccountService.get(); // Get Email From Service --> receved from [ForgetPassword]

    if (!data) {
      this._router.navigate(['forgetPassword']);
      return;
    }
    this.email = data.email; // save Email from [ForgetPassword]
  }
  initiateFormControlers(): void {
    this.code = new FormControl('', [Validators.required]);
  }

  initiateFormGroubs(): void {
    this.verifyCode = new FormGroup({
      code: this.code,
    });
  }

  //Submit Form for calling Api
  submitForm(): void {
    // debugger;
    if (this.verifyCode.valid) {
      const data: ICheckCode = {
        code: String(this.verifyCode.value.code), // Get Code from end user convert to string
        email: this.email, // Get Email by services
      };
      console.log(data);

      this.checkOtp(data);
    } else {
      this.verifyCode.markAllAsTouched();
      Object.keys(this.verifyCode.controls).forEach((control) => {
        this.verifyCode.controls[control].markAsDirty();
      });
    }
  }

  // Function Call Api
  checkOtp(dataCheckOtp: ICheckCode): void {
    this._spinner.show();
    this._authService.checkOtp(dataCheckOtp).subscribe({
      next: (response) => {
        if (response.message) {
          this._notificationsService.showSuccedded(
            'Verify Code',
            response.message,
          );
          // debugger;
          // Set email and resetToken in service for reset Password
          this._getAccountService.set({
            ...dataCheckOtp,
            resetToken: response.resetToken,
          });
          // console.log(response);
          this._router.navigate(['resetpassword']);
          this._spinner.hide();
        }
      },
      error: (error) => {
        this._notificationsService.showError('Verify Code', error.error.error);
        this._spinner.hide();
      },
    });
  }
}
