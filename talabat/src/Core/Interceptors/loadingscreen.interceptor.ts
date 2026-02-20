import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize } from 'rxjs';

export const loadingscreenInterceptor: HttpInterceptorFn = (req, next) => {
  const sppiner = inject(NgxSpinnerService);
  sppiner.show();
  return next(req).pipe(
    finalize(() => {
      sppiner.hide();
    }),
  );
};
