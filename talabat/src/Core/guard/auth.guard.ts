import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../Services/ِAuthServices/auth.service';

export const authGuard: CanActivateFn = () => {
  // debugger;
  const _router = inject(Router);
  const _authServices = inject(AuthService);
  // if localstorage has token ==> you can go to home page
  if (_authServices.checkToken()) {
    return true;
  } else {
    return _router.createUrlTree(['login']);
  }
};
