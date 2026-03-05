import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../Services/ِAuthServices/auth.service';
import { inject } from '@angular/core';

export const roleGuard: CanActivateFn = (route, state) => {
  const _authServices = inject(AuthService);
  const _router = inject(Router);
  const allowedRoles = route.data?.['Roles'] as string[];
  const redirectTo = route.data?.['redirectTo'] || '/home';
  const user = _authServices.currentUserValue;

  // if user not login
  if (!user) return _router.createUrlTree([redirectTo]);
  // if roles is null go to home
  if (!allowedRoles || allowedRoles.length === 0) return true;
  // check Roles
  if (_authServices.hasAnyRole(...allowedRoles)) return true;
  // if not allowed
  return _router.createUrlTree([redirectTo]);
};
