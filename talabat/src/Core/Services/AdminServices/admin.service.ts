import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProductParams } from '../../Interfaces/UserInterfaces/product-params';
import { Observable } from 'rxjs';
import { IPagination } from '../../Interfaces/UserInterfaces/ipagination';
import { IProduct } from '../../Interfaces/UserInterfaces/iproduct';
import { BaseUrl } from '../../BaseUrl';
import { ICategory } from '../../Interfaces/UserInterfaces/icategory';
import { IBrand } from '../../Interfaces/UserInterfaces/ibrand';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  constructor(private _http: HttpClient) {}

  getProducts(productParam: ProductParams): Observable<IPagination<IProduct>> {
    let params = new HttpParams();
    // ForLoop everyeach key and value in product params
    Object.entries(productParam).forEach(([key, value]) => {
      if (value != undefined && value != null) {
        params = params.append(key, value.toString());
      }
    });
    return this._http.get<IPagination<IProduct>>(
      `${BaseUrl}/api/Admin/GetProducts`,
      { params },
    );
  }

  getProductDetails(productId: number): Observable<IProduct> {
    return this._http.get<IProduct>(
      `${BaseUrl}/api/Admin/GetProduct/${productId}`,
    );
  }

  addProduct(data: IProduct): Observable<IProduct> {
    return this._http.post<IProduct>(`${BaseUrl}/api/Admin/AddProduct`, data);
  }

  editProduct(data: IProduct): Observable<IProduct> {
    return this._http.put<IProduct>(`${BaseUrl}/api/Admin/UpdateProduct`, data);
  }

  deleteProduct(id: number): Observable<any> {
    return this._http.delete(`${BaseUrl}/api/Admin/DeleteProduct`, {
      params: { id },
    });
  }

  getCategory(): Observable<ICategory[]> {
    return this._http.get<ICategory[]>(`${BaseUrl}/api/Admin/Categories`);
  }

  getbrand(): Observable<IBrand[]> {
    return this._http.get<IBrand[]>(`${BaseUrl}/api/Admin/Brands`);
  }
}
