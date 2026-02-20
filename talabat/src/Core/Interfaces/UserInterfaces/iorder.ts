import { IAddressShiper } from './iaddress-shiper';
import { IOrderItems } from './iorder-items';

export interface Iorder {
  id: number;
  orderStatus: string;
  delivaryMethodId: number;
  delivaryMethod: string;
  delivaryMethodCost: number;
  addressShiper: IAddressShiper;
  items: IOrderItems[];
  orderDate: string;
  subTotal: number; // For Total Items
  total: number; // Total Price with Delevary Method
  paymentId?: string;
}
