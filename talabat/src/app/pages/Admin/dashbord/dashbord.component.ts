import { AdminService } from './../../../../Core/Services/AdminServices/admin.service';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChartModule } from 'primeng/chart';
import { MenuModule } from 'primeng/menu';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { StyleClassModule } from 'primeng/styleclass';
import { PanelMenuModule } from 'primeng/panelmenu';
import { MenuItem } from 'primeng/api';
import { Subscription, debounceTime } from 'rxjs';
import { IProduct } from '../../../../Core/Interfaces/UserInterfaces/iproduct';
import { LayoutService } from '../../../../Core/Services/app.layout.service';
import { Iorder } from '../../../../Core/Interfaces/UserInterfaces/iorder';
import { ApplicationUser } from '../../../../Core/Interfaces/application-user';
import { IOrderItems } from '../../../../Core/Interfaces/UserInterfaces/iorder-items';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { IPagination } from '../../../../Core/Interfaces/UserInterfaces/ipagination';
@Component({
  selector: 'app-dashbord',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ChartModule,
    MenuModule,
    TableModule,
    StyleClassModule,
    PanelMenuModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
  ],
  templateUrl: './dashbord.component.html',
  styleUrl: './dashbord.component.scss',
})
export class DashbordComponent implements OnInit, OnDestroy {
  items!: MenuItem[];

  allProductItems: IOrderItems[] = [];
  productItem: IProduct = {} as IProduct;
  chartData: any;

  chartOptions: any;

  subscription!: Subscription;

  getOrderCount!: number;

  getAccountCount!: number;
  visible: boolean = false;
  constructor(
    public layoutService: LayoutService,
    private _adminService: AdminService,
  ) {
    this.subscription = this.layoutService.configUpdate$
      .pipe(debounceTime(25))
      .subscribe((config) => {
        this.initChart();
      });
  }

  ngOnInit() {
    this.initChart();
    this._adminService.getOrders().subscribe({
      next: (response: IPagination<Iorder>) => {
        this.allProductItems = response.data.flatMap((order) => order.items); // loop + map
      },
      error: (error) => console.log(error),
    });

    this.items = [
      { label: 'Add New', icon: 'pi pi-fw pi-plus' },
      { label: 'Remove', icon: 'pi pi-fw pi-minus' },
    ];

    this._adminService.getOrders().subscribe({
      next: (res: IPagination<Iorder>) => {
        this.getOrderCount = res.data.length;
      },
    });

    this._adminService.getAccounts().subscribe({
      next: (res: ApplicationUser[]) => {
        this.getAccountCount = res.length;
      },
    });
  }

  initChart() {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--text-color');
    const textColorSecondary = documentStyle.getPropertyValue(
      '--text-color-secondary',
    );
    const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

    this.chartData = {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
      datasets: [
        {
          label: 'First Dataset',
          data: [65, 59, 80, 81, 56, 55, 40],
          fill: false,
          backgroundColor: documentStyle.getPropertyValue('--bluegray-700'),
          borderColor: documentStyle.getPropertyValue('--bluegray-700'),
          tension: 0.4,
        },
        {
          label: 'Second Dataset',
          data: [30, 48, 40, 19, 86, 27, 90],
          fill: false,
          backgroundColor: documentStyle.getPropertyValue('--green-600'),
          borderColor: documentStyle.getPropertyValue('--green-600'),
          tension: 0.4,
        },
      ],
    };

    this.chartOptions = {
      plugins: {
        legend: {
          labels: {
            color: textColor,
          },
        },
      },
      scales: {
        x: {
          ticks: {
            color: textColorSecondary,
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false,
          },
        },
        y: {
          ticks: {
            color: textColorSecondary,
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false,
          },
        },
      },
    };
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  getProductDetails(id: number) {
    console.log(id);
    this.visible = true;
    this._adminService.getProductDetails(id).subscribe({
      next: (res: IProduct) => {
        console.log(res);
        this.productItem = res;
      },
    });
  }
}
