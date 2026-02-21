import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProductParams } from '../../Interfaces/UserInterfaces/product-params';
import { Observable } from 'rxjs';
import { IPagination } from '../../Interfaces/UserInterfaces/ipagination';
import { IProduct } from '../../Interfaces/UserInterfaces/iproduct';
import { ICategory } from '../../Interfaces/UserInterfaces/icategory';
import { IBrand } from '../../Interfaces/UserInterfaces/ibrand';
import { environment } from '../../../environment';

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
      `${environment.apiUrl}/api/Admin/GetProducts`,
      { params },
    );
  }

  getProductDetails(productId: number): Observable<IProduct> {
    return this._http.get<IProduct>(
      `${environment.apiUrl}/api/Admin/GetProduct/${productId}`,
    );
  }

  addProduct(productData: any, imageFile: File | null = null): Observable<any> {
    const formData = new FormData();

    formData.append('Name', productData.name);
    formData.append('Descripaion', productData.descripaion);
    formData.append('Price', productData.price.toString());
    formData.append('BrandId', productData.brandId.toString());
    formData.append('CategoryId', productData.categoryId.toString());
    formData.append('Stock', productData.stock.toString());

    if (imageFile) {
      formData.append('ProductPic', imageFile, imageFile.name);
    }

    return this._http.post<any>(
      `${environment.apiUrl}/api/Admin/AddProduct`,
      formData,
    );
  }

  editProduct(data: IProduct): Observable<IProduct> {
    return this._http.put<IProduct>(
      `${environment.apiUrl}/api/Admin/UpdateProduct`,
      data,
    );
  }

  deleteProduct(id: number): Observable<any> {
    return this._http.delete(`${environment.apiUrl}/api/Admin/DeleteProduct`, {
      params: { id },
    });
  }

  getCategory(): Observable<ICategory[]> {
    return this._http.get<ICategory[]>(
      `${environment.apiUrl}/api/Admin/Categories`,
    );
  }

  editCategory(data: ICategory): Observable<ICategory> {
    return this._http.put<ICategory>(
      `${environment.apiUrl}/api/Admin/EditCategory`,
      data,
    );
  }

  addCategory(data: ICategory): Observable<ICategory> {
    return this._http.post<ICategory>(
      `${environment.apiUrl}/api/Admin/AddCategory`,
      data,
    );
  }

  deleteCategory(id: number): Observable<string> {
    return this._http.delete<string>(
      `${environment.apiUrl}/api/Admin/DeleteCategory`,
      {
        params: { id },
      },
    );
  }

  getbrand(): Observable<IBrand[]> {
    return this._http.get<IBrand[]>(`${environment.apiUrl}/api/Admin/Brands`);
  }

  addBrand(data: IBrand): Observable<IBrand> {
    return this._http.post<IBrand>(
      `${environment.apiUrl}/api/Admin/addBrand`,
      data,
    );
  }

  editBrand(data: IBrand): Observable<IBrand> {
    return this._http.put<IBrand>(
      `${environment.apiUrl}/api/Admin/EditBrand`,
      data,
    );
  }
  deleteBrand(id: number): Observable<string> {
    return this._http.delete<string>(
      `${environment.apiUrl}/api/Admin/DeleteBrand`,
      {
        params: { id },
      },
    );
  }
}
