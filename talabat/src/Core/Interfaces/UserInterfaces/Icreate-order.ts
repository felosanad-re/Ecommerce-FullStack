import { IAddressShiper } from './iaddress-shiper';

export interface ICreateOrder {
  addressShiper: IAddressShiper;
  delivaryMethod?: number;
}
