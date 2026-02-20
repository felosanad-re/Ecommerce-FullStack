import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { BaseUrl } from '../../BaseUrl';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  constructor(private _http: HttpClient) {}
  clientSecret: BehaviorSubject<string> = new BehaviorSubject<string>('');
  async redirectToCheckout(orderId: number): Promise<void> {
    try {
      const res = await this._http
        .post<{
          checkoutUrl: string; // will reseve the checkoutUrl from api
        }>(`${BaseUrl}/api/Payment/createCheckoutSession/${orderId}`, {})
        .toPromise();

      if (!res?.checkoutUrl) {
        console.error('did not return a checkoutUrl');
        alert(
          'there is a problem with the payment service, please try again later',
        );
        return;
      }

      console.log('Redirecting to:', res.checkoutUrl);
      window.location.href = res.checkoutUrl; // redirect to payment page [Stripe]
    } catch (err) {
      console.error('there is a problem with the payment service:', err);
      alert(
        'there is a problem with the payment service, please try again later',
      );
    }
  }
}
