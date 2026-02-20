import { Component } from '@angular/core';
import { ProductService } from '../../../../Core/Services/UserServices/product.service';
import { ICategory } from '../../../../Core/Interfaces/UserInterfaces/icategory';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss',
})
export class CategoriesComponent {
  constructor(private _productService: ProductService) {}

  // category!: ICategory;
  categorys!: ICategory[];

  private categoryImg: { [key: string]: string } = {
    Gaming: 'gaming',
    Camera: 'camera',
    Audio: 'audio',
    Accessories: 'accessories',
    Laptop: 'laptop',
    Mobile: 'mobile',
  };

  ngOnInit(): void {
    this.getCategory();
  }
  getCategory(): void {
    this._productService.getAllCategory().subscribe((next) => {
      this.categorys = next;
    });
  }

  getCategoryImage(name: string): string {
    return `./assets/Category/${this.categoryImg[name]}.png`;
  }
}
