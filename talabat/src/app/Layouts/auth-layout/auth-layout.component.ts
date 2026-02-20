import { Component } from '@angular/core';
import { AuthNavComponent } from '../../Components/authLayout/auth-nav/auth-nav.component';
import { RouterOutlet } from '@angular/router';
import { AuthFooterComponent } from '../../Components/authLayout/auth-footer/auth-footer.component';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [AuthNavComponent, RouterOutlet, AuthFooterComponent],
  templateUrl: './auth-layout.component.html',
  styleUrl: './auth-layout.component.scss',
})
export class AuthLayoutComponent {}
