import { Component, ViewEncapsulation } from '@angular/core';
import { AuthUserNavModule } from '../../../../Core/modules/auth_user/auth-user-nav.module';
import { MenuItem } from 'primeng/api';
import { DataUserService } from '../../../../Core/Services/UserServices/data-user.service';
import { Router } from '@angular/router';
import { CartService } from '../../../../Core/Services/UserServices/cart.service';
import { AuthService } from '../../../../Core/Services/ِAuthServices/auth.service';

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
  showDashboardButton: boolean = false;
  isLoggedIn: boolean = false;

  constructor(
    private _userData: DataUserService,
    private _cartService: CartService,
    private _router: Router,
    public _authService: AuthService,
  ) {}
  ngOnInit() {
    this.getUserName();
    this.getUserCartCount();
    this.updateAuthState();
    this.items = [
      {
        label: 'Home',
        icon: 'pi pi-home',
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

    this._authService.currentUser$.subscribe(() => {
      this.updateAuthState();
    });
  }

  // Create function To Get UserName
  getUserName(): void {
    this._userData.userName.subscribe((res) => {
      if (res) {
        this.userName = res;
        return;
      }

      this.userName = localStorage.getItem('userName') ?? '';
    });
  }

  handleUserClick(): void {
    const token = localStorage.getItem('token');
    if (!token) {
      this._router.navigate(['login']);
      return;
    }

    this.isLogOut = !this.isLogOut;
  }

  // Function To LogOut User without Api
  logOut(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('userName');
    localStorage.removeItem('current_user');
    this.userName = '';
    this.isLogOut = false;
    this.updateAuthState();
    this._router.navigate(['login']);
  }

  // Get Cart Count --> حاليا الكود ده مش شغال لحد ما اعمل جزء الكارت
  getUserCartCount(): void {
    const cartId = localStorage.getItem('cartId') ?? '';
    this._cartService
      .getCartCount()
      .subscribe((res) => (this.cartCount = res.items.length));
  }

  private updateAuthState(): void {
    this.isLoggedIn = this._authService.checkToken();
    this.showDashboardButton =
      this.isLoggedIn &&
      (this._authService.isAdmin() || this._authService.isSuperAdmin());

    if (!this.isLoggedIn) {
      this.userName = '';
      this.isLogOut = false;
      return;
    }

    if (!this.userName) {
      this.userName = localStorage.getItem('userName') ?? '';
    }
  }
}
