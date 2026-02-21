import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { BehaviorSubject, map, Observable } from 'rxjs';
import { IRegister } from '../../Interfaces/authInterfaces/iregister';
import { ILogin } from '../../Interfaces/authInterfaces/ilogin';
import { IForgetPassword } from '../../Interfaces/authInterfaces/iforget-password';
import { ICheckCode } from '../../Interfaces/authInterfaces/icheck-code';
import { IResetPassword } from '../../Interfaces/authInterfaces/ireset-password';
import { ILoginToReturnDto } from '../../Interfaces/authInterfaces/ilogin-to-return-dto';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../../environment';

// InterFaces
interface DecodedToken {
  sub?: string; // user id
  nameidentifier?: string;
  email?: string;
  role?: string | string[];
  exp?: number;
  iat?: number;
  [key: string]: any;
}

interface CurrentUser {
  id: string;
  email?: string;
  username?: string;
  role: string | string[];
  isAdmin: boolean;
  isSuperAdmin: boolean;
}
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // get CuterrentUser
  private currentUserSubject = new BehaviorSubject<CurrentUser | null>(null);

  curentUser$ = this.currentUserSubject.asObservable();

  constructor(private _http: HttpClient) {
    this.loadUserFromStorage();
  }

  // Call Api To Register
  register(dataRegister: IRegister): Observable<any> {
    return this._http.post(
      `${environment.apiUrl}/api/Account/Register`,
      dataRegister,
    );
  }

  // Call Api To Login
  login(dataLogin: ILogin): Observable<ILoginToReturnDto> {
    return this._http
      .post<ILoginToReturnDto>(
        `${environment.apiUrl}/api/Account/Login`,
        dataLogin,
      )
      .pipe(
        map((res) => {
          if (res.token) this.handleSuccessfulLogin(res);
          return res;
        }),
      );
  }

  // set Token After User LogIn
  private handleSuccessfulLogin(response: ILoginToReturnDto): void {
    // set token
    localStorage.setItem('token', response.token);
    localStorage.setItem('userName', response.userName);

    // decode token and save user data
    try {
      const decoded = jwtDecode<DecodedToken>(response.token);
      const user: CurrentUser = this.mapDecodedToUser(decoded, response);

      localStorage.setItem('current_user', JSON.stringify(user));
      this.currentUserSubject.next(user);
    } catch (error) {
      console.warn('Error decoding token', error);
      localStorage.removeItem('token');
      localStorage.removeItem('current_user');
      this.currentUserSubject.next(null);
    }
  }

  // decoded Token
  private mapDecodedToUser(
    decoded: DecodedToken,
    res: ILoginToReturnDto,
  ): CurrentUser {
    const role =
      decoded.role ||
      decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      'User';

    return {
      id: decoded.sub || decoded.nameidentifier || '',
      email: decoded.email,
      username: res.userName || decoded.nameidentifier,
      role,
      isAdmin: this.hasRole(role, 'ADMIN'),
      isSuperAdmin: this.hasRole(role, 'SUPER_ADMIN'),
    };
  }

  // Check On User Role
  private hasRole(role: string | string[], target: string): boolean {
    if (Array.isArray(role)) {
      return role.includes(target);
    }
    return role === target;
  }

  // get Current User From Storage
  private loadUserFromStorage(): void {
    const token = localStorage.getItem('token');
    const userJson = localStorage.getItem('current_user');

    if (!token) {
      this.currentUserSubject.next(null);
      return;
    }

    // check token time
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      if (decoded.exp && Date.now() >= decoded.exp * 1000) {
        return;
      }

      if (userJson) {
        const user = JSON.parse(userJson) as CurrentUser;
        this.currentUserSubject.next(user);
      } else {
        // لو مفيش current_user → نعيد بناءه من التوكن
        const rebuiltUser = this.mapDecodedToUser(decoded, {} as any);
        localStorage.setItem('current_user', JSON.stringify(rebuiltUser));
        this.currentUserSubject.next(rebuiltUser);
      }
    } catch (e) {
      console.warn('Invaild Token', e);
      localStorage.removeItem('token');
      localStorage.removeItem('current_user');
      this.currentUserSubject.next(null);
    }
  }
  get currentUserValue(): CurrentUser | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken() && !!this.currentUserValue;
  }

  isAdmin(): boolean {
    return this.currentUserValue?.isAdmin === true;
  }

  isSuperAdmin(): boolean {
    return this.currentUserValue?.isSuperAdmin === true;
  }

  hasAnyRole(...roles: string[]): boolean {
    const userRole = this.currentUserValue?.role;
    if (!userRole) return false;

    if (Array.isArray(userRole)) {
      return roles.some((r) => userRole.includes(r));
    }
    return roles.includes(userRole as string);
  }
  // Call Api To Forget Password
  forgetPassword(dataForgetPassword: IForgetPassword): Observable<any> {
    // debugger;
    // URl انا هنا محتاج ابعت الايميل وكمان ال
    const forgetPassDto: IForgetPassword = {
      ...dataForgetPassword, // Destructuring
      // ClientURL: 'http://localhost:4200/forgetPassword' --> Send to back
      ClientURL: `${window.location.origin}/forgetPassword`, // Get Current Url
    };
    return this._http.post(
      `${environment.apiUrl}/api/Account/ForgetPassword`,
      forgetPassDto,
    );
  }

  // Call Api To Check Otp
  checkOtp(dataCheckOtp: ICheckCode): Observable<any> {
    return this._http.post(
      `${environment.apiUrl}/api/Account/CheckCode`,
      dataCheckOtp,
    );
  }

  // Reset Password
  resetPassword(dataResetPassword: IResetPassword): Observable<any> {
    return this._http.post(
      `${environment.apiUrl}/api/Account/ResetPassword`,
      dataResetPassword,
    );
  }

  // check Token
  checkToken(): boolean {
    if (localStorage.getItem('token')) return true;
    return false;
  }
}
