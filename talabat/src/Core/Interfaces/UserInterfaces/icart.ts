import { ICartItem } from './ICartItem';

export interface ICart {
  // id: string;
  items: ICartItem[];
  deleveryMethodCost?: number;
  deleveryMethodId?: number;
  paymentIntentId?: string;
  clientSecret?: string;
}
