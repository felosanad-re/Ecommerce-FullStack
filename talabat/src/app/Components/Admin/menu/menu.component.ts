import { Component, ViewEncapsulation } from '@angular/core';
import { LayoutService } from '../../../../Core/Services/app.layout.service';
import { MenuitemComponent } from '../menuitem/menuitem.component';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [MenuitemComponent],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class MenuComponent {
  model: any[] = [];

  constructor(public layoutService: LayoutService) {}

  ngOnInit() {
    this.model = [
      {
        label: 'Home',
        items: [
          {
            label: 'Dashboard',
            icon: 'pi pi-fw pi-home',
            routerLink: ['/admin/dashboard'],
          },
        ],
      },
      {
        label: 'Pages',
        icon: 'pi pi-fw pi-briefcase',
        items: [
          {
            label: 'Ecommerance',
            icon: 'pi pi-fw pi-globe',
            routerLink: ['/home'],
          },
          {
            label: 'User',
            icon: 'pi pi-fw pi-user',
            items: [
              {
                label: 'Products',
                icon: 'pi pi-star',
                routerLink: ['Products'],
              },
              {
                label: 'Categories',
                icon: 'pi pi-th-large',
                routerLink: ['Categories'],
              },
              {
                label: 'Brands',
                icon: 'pi pi-list',
                routerLink: ['Brands'],
              },
            ],
          },
        ],
      },
    ];
  }
}
