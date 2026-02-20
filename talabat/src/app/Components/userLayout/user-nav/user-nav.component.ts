import { Component, ViewEncapsulation } from '@angular/core';
import { AuthUserNavModule } from '../../../../Core/modules/auth_user/auth-user-nav.module';
import { MenuItem } from 'primeng/api';
import { DataUserService } from '../../../../Core/Services/UserServices/data-user.service';
import { Router } from '@angular/router';
import { CartService } from '../../../../Core/Services/UserServices/cart.service';

@Component({
  selector: 'app-user-nav',
  standalone: true,
  imports: [AuthUserNavModule],
  templateUrl: './user-nav.component.html',
  styleUrl: './user-nav.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class UserNavComponent {
  items: MenuItem[] | undefined;
  isLogOut: boolean = false;
  userName: string = '';
  cartCount: number = 0;

  constructor(
    private _userData: DataUserService,
    private _cartService: CartService,
    private _router: Router,
  ) {}
  ngOnInit() {
    this.getUserName();
    this.getUserCartCount();
    this.items = [
      {
        label: 'Home',
        icon: 'pi pi-user-plus',
        path: 'home',
      },
      {
        label: 'Products',
        icon: 'pi pi-star',
        path: 'products',
      },
      {
        label: 'Brands',
        icon: 'pi pi-list',
        path: 'brands',
      },
      {
        label: 'Categories',
        icon: 'pi pi-th-large',
        path: 'categories',
      },
    ];
  }

  // Create function To Get UserName
  getUserName(): void {
    this._userData.userName.subscribe((res) => (this.userName = res));
  }

  // Function To LogOut User without Api
  logOut(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('userName');
    localStorage.removeItem('current_user');
    this._router.navigate(['login']);
  }

  // Get Cart Count --> حاليا الكود ده مش شغال لحد ما اعمل جزء الكارت
  getUserCartCount(): void {
    const cartId = localStorage.getItem('cartId') ?? '';
    this._cartService
      .getCartCount()
      .subscribe((res) => (this.cartCount = res.items.length));
  }
}
