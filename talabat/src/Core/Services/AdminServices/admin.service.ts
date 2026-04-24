import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProductParams } from '../../Interfaces/UserInterfaces/product-params';
import { Observable } from 'rxjs';
import { IPagination } from '../../Interfaces/UserInterfaces/ipagination';
import { IProduct } from '../../Interfaces/UserInterfaces/iproduct';
import { ICategory } from '../../Interfaces/UserInterfaces/icategory';
import { IBrand } from '../../Interfaces/UserInterfaces/ibrand';
import { environment } from '../../../environment';
import { Iorder } from '../../Interfaces/UserInterfaces/iorder';
import { ApplicationUser } from '../../Interfaces/application-user';
import { IupdateOrderStatus } from '../../Interfaces/iupdate-order-status';
import { IOrderStatusResponse } from '../../Interfaces/iorder-status-response';
import { Notifications } from '../../Interfaces/Notifications';
import { ParamNotification } from '../../Interfaces/Notifications/param-notification';
import { ImportResult } from '../../Interfaces/import-result';
import { throws } from 'assert';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  constructor(private _http: HttpClient) {}

  // Form Builder
  private buildImportFormData(file: File): FormData {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('config.sheetName', 'Products');
    formData.append('config.startRow', '2');
    formData.append('config.hasHeader', 'true');

    return formData;
  }

  getProducts(productParam: ProductParams): Observable<IPagination<IProduct>> {
    let params = new HttpParams();
    // ForLoop every each key and value in product params
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

  getBrand(): Observable<IBrand[]> {
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

  getAccounts(): Observable<ApplicationUser[]> {
    return this._http.get<ApplicationUser[]>(
      `${environment.apiUrl}/api/AdminUser/Accounts`,
    );
  }

  deleteAccount(id: string): Observable<any> {
    return this._http.delete<any>(
      `${environment.apiUrl}/api/AdminUser/DeleteUser`,
      {
        params: { id },
      },
    );
  }

  lockAccount(id: string, days: number): Observable<any> {
    return this._http.post<any>(
      `${environment.apiUrl}/api/AdminUser/LockAccount?id=${id}&days=${days}`,
      {},
    );
  }

  activeAccount(id: string): Observable<any> {
    return this._http.post<any>(
      `${environment.apiUrl}/api/AdminUser/ActiveAccount?id=${id}`,
      {},
    );
  }

  getOrders(): Observable<IPagination<Iorder>> {
    return this._http.get<IPagination<Iorder>>(
      `${environment.apiUrl}/api/AdminOrder/Orders`,
    );
  }

  updateOrderStatus(
    request: IupdateOrderStatus,
  ): Observable<IOrderStatusResponse> {
    return this._http.post<IOrderStatusResponse>(
      `${environment.apiUrl}/api/AdminOrder/UpdateOrderStatus`,
      request,
    );
  }

  getAllNotifications(
    data: ParamNotification,
  ): Observable<IPagination<Notifications>> {
    let params = new HttpParams();
    Object.entries(data).forEach(([key, value]) => {
      if (value != null && value != undefined) {
        params = params.append(key, value.toString());
      }
    });
    return this._http.get<IPagination<Notifications>>(
      `${environment.apiUrl}/api/AdminOrder/GetAllNotification`,
      { params },
    );
  }

  deleteNotification(id: number): Observable<any> {
    return this._http.delete(
      `${environment.apiUrl}/api/AdminOrder/DeleteNotification?id=${id}`,
    );
  }

  readNotification(id: number): Observable<any> {
    return this._http.put(
      `${environment.apiUrl}/api/AdminOrder/EditNotification?id=${id}`,
      {},
    );
  }

  exportProducts(): Observable<Blob> {
    return this._http.get(`${environment.apiUrl}/api/Export/Products`, {
      responseType: 'blob',
    });
  }

  exportOrders(): Observable<Blob> {
    return this._http.get(`${environment.apiUrl}/api/Export/Orders`, {
      responseType: 'blob',
    });
  }

  importProducts(file: File): Observable<ImportResult<unknown>> {
    const formData = this.buildImportFormData(file);
    return this._http.post<ImportResult<unknown>>(
      `${environment.apiUrl}/api/Import/Products`,
      formData,
    );
  }

  importOrders(file: File, zipFile?: File): Observable<ImportResult<unknown>> {
    const formData = this.buildImportFormData(file);
    if (zipFile) {
      formData.append('zipFile', zipFile);
    }
    return this._http.post<ImportResult<unknown>>(
      `${environment.apiUrl}/api/Import/Orders`,
      formData,
    );
  }
}
