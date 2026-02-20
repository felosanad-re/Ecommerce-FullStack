import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseUrl } from '../../BaseUrl';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IProduct } from '../../Interfaces/UserInterfaces/iproduct';
import { IPagination } from '../../Interfaces/UserInterfaces/ipagination';
import { ProductParams } from '../../Interfaces/UserInterfaces/product-params';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(private _http: HttpClient) {}

  // Call Api To Get Products
  getProducts(paramsObj: ProductParams): Observable<IPagination<IProduct>> {
    let params = new HttpParams(); // send parameters with request
    if (paramsObj.search) params = params.append('search', paramsObj.search);
    if (paramsObj.pageSize)
      params = params.append('pageSize', paramsObj.pageSize);
    if (paramsObj.pageIndex)
      params = params.append('pageIndex', paramsObj.pageIndex);
    if (paramsObj.brandId) params = params.append('brandId', paramsObj.brandId);
    if (paramsObj.categoryId)
      params = params.append('categoryId', paramsObj.categoryId);

    return this._http.get<IPagination<IProduct>>(`${BaseUrl}/api/Products`, {
      params,
    });
  }

  // Get All Category
  getAllCategory(): Observable<any> {
    return this._http.get(`${BaseUrl}/api/Products/Categories`);
  }

  getAllBrands(): Observable<any> {
    return this._http.get(`${BaseUrl}/api/products/brands`);
  }

  getProductDetails(productId: number): Observable<any> {
    return this._http.get(`${BaseUrl}/api/Products/${productId}`);
  }
}
