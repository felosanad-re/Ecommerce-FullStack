import { Injectable } from '@angular/core';
import { IresetPasswordParams } from '../../Interfaces/authInterfaces/ireset-password-params';

@Injectable({
  providedIn: 'root',
})

// هبداء استعملها في اي مكان محتاج ابعت الايميل للباك
export class GetAccountService {
  private readonly Key_Email: string = 'reset';
  // Set Email when user try to Change Password
  set(data: IresetPasswordParams) {
    sessionStorage.setItem(this.Key_Email, JSON.stringify(data)); // set --> email and reset token in session storage
  }

  // Get Email
  get(): IresetPasswordParams | null {
    try {
      const data = sessionStorage.getItem(this.Key_Email); // get --> email and reset token
      return data ? (JSON.parse(data) as IresetPasswordParams) : null;
    } catch (error) {
      console.warn(error);
      return null;
    }
  }

  // Delete Email After Change Password
  delete(): void {
    sessionStorage.removeItem(this.Key_Email);
  }
}
