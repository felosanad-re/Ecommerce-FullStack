import { Component } from '@angular/core';
import { LayoutService } from '../../../../Core/Services/app.layout.service';

@Component({
  selector: 'app-admin-footer',
  standalone: true,
  imports: [],
  templateUrl: './admin-footer.component.html',
  styleUrl: './admin-footer.component.scss',
})
export class AdminFotterComponent {
  constructor(public layoutService: LayoutService) {}
}
