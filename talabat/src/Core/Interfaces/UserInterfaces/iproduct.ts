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
}
