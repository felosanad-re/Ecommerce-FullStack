import { NotificationsService } from './../../../../Core/Services/notifications.service';
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ILogin } from '../../../../Core/Interfaces/authInterfaces/ilogin';
import { AuthModule } from '../../../../Core/modules/auth/auth.module';
import { AuthService } from '../../../../Core/Services/ِAuthServices/auth.service';
import { Router } from '@angular/router';
import { DataUserService } from '../../../../Core/Services/UserServices/data-user.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [AuthModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  email!: FormControl;
  password!: FormControl;
  loginForm!: FormGroup;
  constructor(
    private _notificationsService: NotificationsService,
    private _authService: AuthService,
    private _router: Router,
    private _userData: DataUserService,
  ) {
    this.initiateFormControlers();
    this.initiateFormGroubs();
  }

  // Create function To Create Forms Controls
  initiateFormControlers(): void {
    this.email = new FormControl('', [Validators.email, Validators.required]);
    this.password = new FormControl('', [Validators.required]);
  }

  // Create function To Create Forms Groub
  initiateFormGroubs(): void {
    this.loginForm = new FormGroup({
      email: this.email,
      password: this.password,
    });
  }

  //submitForm
  submitForm(): void {
    // debugger;
    if (this.loginForm.valid) {
      // console.log(this.loginForm.value);
      this.loginApi(this.loginForm.value);
    } else {
      this.loginForm.markAllAsTouched();
      Object.keys(this.loginForm.controls).forEach((control) =>
        this.loginForm.controls[control].markAsDirty(),
      );
    }
  }

  // loging With Api
  loginApi(dataLogin: ILogin): void {
    this._authService.login(dataLogin).subscribe({
      next: (response) => {
        const user = this._authService.currentUserValue;

        if (this._authService.isSuperAdmin() || this._authService.isAdmin()) {
          this._router.navigate(['/admin/dashboard']);
        } else {
          this._router.navigate(['/home']);
          this._userData.userName.next(response.userName); // get user name
        }
        this._userData.userName.next(response.userName); // get user name
        // console.log(response);
      },
      error: (error) => {
        this._notificationsService.showError('Login', error.error.error);
        console.log(error);
      },
    });
  }
}
