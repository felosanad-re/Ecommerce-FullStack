import { Component } from '@angular/core';
import { AdminFotterComponent } from '../../Components/Admin/admin-fotter/admin-footer.component';
import { RouterOutlet } from '@angular/router';
import { TopbarComponent } from '../../Components/Admin/topbar/topbar.component';
import { SidebarComponent } from '../../Components/Admin/sidebar/sidebar.component';
// import { ConfigComponent } from '../../Components/Admin/config/config.component';
import { CommonModule } from '@angular/common';
import { LayoutService } from '../../../Core/Services/app.layout.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [
    TopbarComponent,
    AdminFotterComponent,
    RouterOutlet,
    SidebarComponent,
    // ConfigComponent,
    CommonModule,
  ],
  templateUrl: './admin-layout.component.html',
  styleUrl: './layout/DashbordLayout.scss',
})
export class AdminLayoutComponent {
  constructor(public layoutService: LayoutService) {} // ← تأكد إنه injected

  // ← أضف الـ getter ده هنا
  get containerClass() {
    const config = this.layoutService.config();
    const state = this.layoutService.state;

    return {
      'layout-overlay': config.menuMode === 'overlay',
      'layout-static': config.menuMode === 'static',
      'layout-static-inactive':
        config.menuMode === 'static' && state.staticMenuDesktopInactive,
      'layout-static-active':
        config.menuMode === 'static' && !state.staticMenuDesktopInactive,
      'layout-overlay-active': state.overlayMenuActive,
      'layout-mobile-active': state.staticMenuMobileActive,
    };
  }
}
