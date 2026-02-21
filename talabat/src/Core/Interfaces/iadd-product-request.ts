export interface IAddProductRequest {
  name: string;
  descripaion: string;
  productPic: File;
  pictureUrl?: string;
  price: number;
  brandId: number;
  categoryId: number;
  stock: number;
}
