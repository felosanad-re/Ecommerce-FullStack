import { Pipe, PipeTransform } from '@angular/core';
import { IProduct } from '../Interfaces/UserInterfaces/iproduct';

@Pipe({
  name: 'categoryProduct',
  standalone: true,
})
export class CategoryProductPipe implements PipeTransform {
  transform(products: IProduct[]): IProduct[] {
    return products.filter((product) => product?.brandId === 1); // show in home page product with category id = 1
  }
}
