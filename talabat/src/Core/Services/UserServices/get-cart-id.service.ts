import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class GetCartIdService {
  private readonly KEY_Cart = 'cartId';
  // 1. Get Or update Cart Id
  getOrCreateCartId(): string {
    let cartId = localStorage.getItem(this.KEY_Cart);
    if (!cartId) {
      cartId = crypto.randomUUID(); // create new cart id --> guid
      localStorage.setItem(this.KEY_Cart, cartId);
    }
    return cartId;
  }

  clearCart(): void {
    localStorage.removeItem(this.KEY_Cart);
  }
}
