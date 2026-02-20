import {
  Component,
  ElementRef,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { LayoutService } from '../../../../Core/Services/app.layout.service';
import { MenuItem } from 'primeng/api';
import { CommonModule } from '@angular/common';
import { AuthModule } from '../../../../Core/modules/auth/auth.module';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, AuthModule],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class TopbarComponent {
  items!: MenuItem[];

  @ViewChild('menubutton') menuButton!: ElementRef;

  @ViewChild('topbarmenubutton') topbarMenuButton!: ElementRef;

  @ViewChild('topbarmenu') menu!: ElementRef;

  constructor(public layoutService: LayoutService) {}
}
