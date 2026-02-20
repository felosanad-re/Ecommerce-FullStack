import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, EMPTY, Observable } from 'rxjs';
import { ICreateOrder } from '../../Interfaces/UserInterfaces/Icreate-order';
import { BaseUrl } from '../../BaseUrl';
import { DelivaryMethod } from '../../Interfaces/UserInterfaces/delivary-method';
import { Iorder } from '../../Interfaces/UserInterfaces/iorder';
import { ICart } from '../../Interfaces/UserInterfaces/icart';

@Injectable({
  providedIn: 'root',
})
export class CreateOrderService {
  constructor(private _http: HttpClient) {}
  // get order
  checkOutOrder(data: ICreateOrder): Observable<Iorder> {
    return this._http.post<Iorder>(`${BaseUrl}/api/Order/CreateOrder`, data);
  }

  // get Orders Details
  getOrdersDetails(): Observable<Iorder[]> {
    return this._http.get<Iorder[]>(`${BaseUrl}/api/Order/GetOrders`);
  }

  // get order details
  getOrderDetails(id: number): Observable<ICart> {
    return this._http.get<ICart>(`${BaseUrl}/api/Order/${id}`);
  }

  // delete order
  deleteOrder(id: number): Observable<any> {
    return this._http.delete<{ message: string }>(
      `${BaseUrl}/api/Order/DeleteOrder/${id}`,
    );
  }
  // Get delivary method from api
  getDelivaryMethods(): Observable<DelivaryMethod[]> {
    return this._http.get<DelivaryMethod[]>(
      `${BaseUrl}/api/Order/DeliveryMethod`,
    );
  }
}
