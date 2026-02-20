import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';

import { AuthUserNavModule } from '../../../../Core/modules/auth_user/auth-user-nav.module';

@Component({
  selector: 'app-auth-nav',
  standalone: true,
  imports: [AuthUserNavModule],
  templateUrl: './auth-nav.component.html',
  styleUrl: './auth-nav.component.scss',
})
export class AuthNavComponent {
  items: MenuItem[] | undefined;

  ngOnInit() {
    this.items = [
      {
        label: 'Register',
        icon: 'pi pi-user-plus',
        path: 'register',
      },
      {
        label: 'Login',
        icon: 'pi pi-sign-in',
        path: 'login',
      },
    ];
  }
}
