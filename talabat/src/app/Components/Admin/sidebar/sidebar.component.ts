import { Component, ElementRef, ViewEncapsulation } from '@angular/core';
import { LayoutService } from '../../../../Core/Services/app.layout.service';
import { MenuComponent } from '../menu/menu.component';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [MenuComponent],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class SidebarComponent {
  constructor(
    public layoutService: LayoutService,
    public el: ElementRef,
  ) {}
}
