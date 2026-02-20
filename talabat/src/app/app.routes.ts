import { Routes } from '@angular/router';
import { authGuard } from '../Core/guard/auth.guard';
import { detailsResolver } from '../Core/Resolvers/details.resolver';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./Layouts/user-layout/user-layout.component').then(
        (m) => m.UserLayoutComponent,
      ),
    // canActivate: [authGuard], // guard to block user from go to home without login
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      // home
      {
        path: 'home',
        loadComponent: () =>
          import('./pages/userPages/home/home.component').then(
            (m) => m.HomeComponent,
          ),
      },
      // products
      {
        path: 'products',
        loadComponent: () =>
          import('./pages/userPages/product/product.component').then(
            (m) => m.ProductComponent,
          ),
      },
      // Category
      {
        path: 'categories',
        loadComponent: () =>
          import('./pages/userPages/categories/categories.component').then(
            (c) => c.CategoriesComponent,
          ),
      },
      // Product In Specific Category
      {
        path: 'productCategory/:name/:id',
        loadComponent: () =>
          import('./pages/userPages/product-in-category/product-in-category.component').then(
            (c) => c.ProductInCategoryComponent,
          ),
      },
      // Brand
      {
        path: 'brands',
        loadComponent: () =>
          import('./pages/userPages/brand/brand.component').then(
            (c) => c.BrandComponent,
          ),
      },
      // product in specific brand
      {
        path: 'productBrand/:name/:id',
        loadComponent: () =>
          import('./pages/userPages/product-in-brand/product-in-brand.component').then(
            (c) => c.ProductInBrandComponent,
          ),
      },
      // product Details
      {
        path: 'details/:id/:name', // Dynamic Value --> productId
        loadComponent: () =>
          import('./pages/userPages/details/details.component').then(
            (c) => c.DetailsComponent,
          ),
        resolve: { Details: detailsResolver },
      },
      // cart
      {
        path: 'cart',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./pages/userPages/carts/carts.component').then(
            (c) => c.CartsComponent,
          ),
      },
      // Checkout order
      {
        path: 'checkout',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./pages/userPages/checkorder/checkorder.component').then(
            (c) => c.CheckorderComponent,
          ),
      },
      // Confirm order
      {
        path: 'confirmOrder',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./pages/userPages/confirm-order/confirm-order.component').then(
            (c) => c.ConfirmOrderComponent,
          ),
      },
      {
        path: 'orderDetails/:id',
        loadComponent: () =>
          import('./pages/userPages/order-details/order-details.component').then(
            (c) => c.OrderDetailsComponent,
          ),
      },
      {
        path: 'paymentSuccess',
        loadComponent: () =>
          import('./pages/userPages/payment-success/payment-success.component').then(
            (c) => c.PaymentSuccessComponent,
          ),
      },
    ],
  },
  //Auth Routes
  {
    path: '',
    loadComponent: () =>
      import('./Layouts/auth-layout/auth-layout.component').then(
        (c) => c.AuthLayoutComponent,
      ),
    children: [
      {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full',
      },
      {
        path: 'login',
        loadComponent: () =>
          import('./pages/authPages/login/login.component').then(
            (c) => c.LoginComponent,
          ),
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./pages/authPages/register/register.component').then(
            (c) => c.RegisterComponent,
          ),
      },
      {
        path: 'forgetPassword',
        loadComponent: () =>
          import('./pages/authPages/forget-passowrd/forget-passowrd.component').then(
            (c) => c.ForgetPassowrdComponent,
          ),
      },
      {
        path: 'verify',
        loadComponent: () =>
          import('./pages/authPages/verify/verify.component').then(
            (c) => c.VerifyComponent,
          ),
      },
      {
        path: 'resetpassword',
        loadComponent: () =>
          import('./pages/authPages/reset-password/reset-password.component').then(
            (c) => c.ResetPasswordComponent,
          ),
      },
    ],
  },
  // Admin Dashbord
  {
    path: 'admin',
    // canActivate: [adminGuard],
    loadComponent: () =>
      import('./Layouts/admin/admin-layout.component').then(
        (c) => c.AdminLayoutComponent,
      ),
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./pages/Admin/dashbord/dashbord.component').then(
            (c) => c.DashbordComponent,
          ),
      },
      {
        path: 'Products',
        loadComponent: () =>
          import('./pages/Admin/products/products.component').then(
            (c) => c.ProductsComponent,
          ),
      },
      {
        path: 'Categories',
        loadComponent: () =>
          import('./pages/Admin/categories/categories.component').then(
            (c) => c.CategoriesComponent,
          ),
      },
      {
        path: 'Brands',
        loadComponent: () =>
          import('./pages/Admin/brands/brands.component').then(
            (c) => c.BrandsComponent,
          ),
      },
    ],
  },
];
