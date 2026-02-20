import { NotificationsService } from './../../../../Core/Services/notifications.service';
import { Component } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../../../Core/Services/ِAuthServices/auth.service';
import { Message } from 'primeng/api';
import { IRegister } from '../../../../Core/Interfaces/authInterfaces/iregister';

import { NgxSpinnerService } from 'ngx-spinner';
import { Router } from '@angular/router';
import { AuthModule } from '../../../../Core/modules/auth/auth.module';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [AuthModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  userName!: FormControl;
  firstName!: FormControl;
  lastName!: FormControl;
  password!: FormControl;
  email!: FormControl;
  confirmPassword!: FormControl;
  address!: FormControl;
  regiterForm!: FormGroup;
  messages!: Message[]; // For toaster Message

  Message!: string[]; // Validation Error when Create Account
  constructor(
    private _authService: AuthService,
    private _notificationsService: NotificationsService,
    private _spinner: NgxSpinnerService,
    private _router: Router,
  ) {
    this.initiatFormControls();
    this.initiatFormGroubs();
  }

  // Create function To Create Forms Controls
  initiatFormControls(): void {
    this.userName = new FormControl('', [
      Validators.required,
      Validators.minLength(3),
    ]);
    this.firstName = new FormControl('', [
      Validators.required,
      Validators.minLength(3),
    ]);
    this.lastName = new FormControl('', [
      Validators.required,
      Validators.minLength(3),
    ]);
    this.userName = new FormControl('', [
      Validators.required,
      Validators.minLength(5),
    ]);
    this.email = new FormControl('', [Validators.email, Validators.required]);

    this.address = new FormControl('');

    this.password = new FormControl('', [
      Validators.required,
      Validators.minLength(5),
    ]);
    this.confirmPassword = new FormControl('', [
      Validators.required,
      this.checkPass(this.password), // Custom Valitator Function
    ]);
  }

  // Create function To Create Forms Groub
  initiatFormGroubs(): void {
    this.regiterForm = new FormGroup({
      userName: this.userName,
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      password: this.password,
      confirmPassword: this.confirmPassword,
      address: this.address,
    });
  }

  // Create Custom Event To Compart Between Password And RePassword
  checkPass(pass: AbstractControl): ValidatorFn {
    return (rePass: AbstractControl): null | { [key: string]: boolean } => {
      if (pass.value === rePass.value) {
        return null;
      } else {
        return { notSame: true };
      }
    };
  }

  // Submit Form
  submitForm(): void {
    // debugger;
    // Check on Form Valid
    if (this.regiterForm.valid) {
      this.registerWithApi(this.regiterForm.value);
      console.log(this.regiterForm.value);
    } else {
      // Mark All Form Controls As Touched
      this.regiterForm.markAllAsTouched();
      // Mark All Form Controls As Dirty
      Object.keys(this.regiterForm.controls).forEach((control) =>
        this.regiterForm.controls[control].markAsDirty(),
      );
    }
  }

  //Call Api To Register
  registerWithApi(dataRegister: IRegister): void {
    this._spinner.show(); // show Loading
    this._authService.register(dataRegister).subscribe({
      next: (response) => {
        if (response.token) {
          this._notificationsService.showSuccedded(
            'Account Created',
            'Create Account Successfully You receive an email to verify your account',
          );
          // الجزء ده اعمله لو عاوز اخلي اليوسر بعد ما يسجل الاكونت ينتقل علي طول لصفحه الهوم
          const { email, password } = dataRegister; // Destructuring data To Send To Login Component
          localStorage.setItem('token', response.token); // TO resolve refresh page
          this._authService.login({ email, password }).subscribe((next) => {
            this._router.navigate(['home']);
          });
        }
        this._spinner.hide();
        // console.log(res);
      },
      error: (error) => {
        this.Message = error.error.error;
        this._notificationsService.showError(
          'Account Not Created',
          this.Message.join('\n'),
        );
        this._spinner.hide();
        // console.log(error);
      },
    });
  }
}
