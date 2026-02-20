import { Component } from '@angular/core';
import { ProductService } from '../../../../Core/Services/UserServices/product.service';
import { IProduct } from '../../../../Core/Interfaces/UserInterfaces/iproduct';
import { IBrand } from '../../../../Core/Interfaces/UserInterfaces/ibrand';
import { AuthModule } from '../../../../Core/modules/auth/auth.module';

@Component({
  selector: 'app-brand',
  standalone: true,
  imports: [AuthModule],
  templateUrl: './brand.component.html',
  styleUrl: './brand.component.scss',
})
export class BrandComponent {
  constructor(private _productService: ProductService) {}

  brands!: IBrand[];

  private brandImg: { [key: string]: string } = {
    Apple: 'apple',
    ASUS: 'asus',
    JBL: 'jbl',
    Razer: 'razer',
    Canon: 'canon',
    Sony: 'sony',
    Dell: 'dell',
    HP: 'hp',
    Logitech: 'logitech',
    Samsung: 'samsung',
  };

  ngOnInit(): void {
    this.getBrands();
  }
  getBrands(): void {
    this._productService
      .getAllBrands()
      .subscribe((next) => (this.brands = next));
  }

  getBrandImg(name: string): string {
    return `./assets/Brands/${this.brandImg[name]}.png`;
  }
}
