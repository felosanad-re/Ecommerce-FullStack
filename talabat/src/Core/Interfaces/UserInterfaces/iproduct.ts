import { StockTypes } from '../stock-types';

export interface IProduct {
  id: number;
  name: string;
  descripaion: string;
  pictureUrl: string;
  price: number;
  brandId: number;
  brand: string;
  categoryId: number;
  category: string;
  isAddedToCart: boolean; // make it optional
  stock: number;
  stockType: StockTypes;
  isDeleted: boolean;
}
