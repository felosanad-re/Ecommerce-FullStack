import { ResolveFn } from '@angular/router';
import { EMPTY, Observable } from 'rxjs';
import { ProductService } from '../Services/UserServices/product.service';
import { inject } from '@angular/core';

export const detailsResolver: ResolveFn<Observable<any>> = (route, state) => {
  const id = Number.parseInt(route.paramMap.get('id') || '');
  const _product = inject(ProductService);
  return id ? _product.getProductDetails(id) : EMPTY;
};
