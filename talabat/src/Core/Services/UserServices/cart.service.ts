import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ICart } from '../../Interfaces/UserInterfaces/icart';
import { environment } from '../../../environment';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(private _http: HttpClient) {}
  // property to get cart count
  cartCount: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  cartState = new BehaviorSubject<ICart | null>(null);

  // Call Api To Get Cart Count for user --> Cart array of string
  getCartCount(): Observable<ICart> {
    return this._http.get<ICart>(`${environment.apiUrl}/api/Carts/CartDetails`);
  }

  addToCart(cartData: ICart): Observable<ICart> {
    return this._http.post<ICart>(
      `${environment.apiUrl}/api/Carts/UpdateOrCreateCart`,
      cartData,
    );
  }

  getCartDetails(): Observable<ICart> {
    return this._http.get<ICart>(`${environment.apiUrl}/api/Carts/CartDetails`);
    // .pipe(tap((cart) => this.cartState.next(cart)));
  }

  getCurrentCart(): ICart | null {
    return this.cartState.value; // ده يرجع القيمة الأخيرة المخزنة فورًا
  }

  deleteCart(): Observable<any> {
    return this._http.delete(`${environment.apiUrl}/api/Carts/DeleteCart`);
  }
}
