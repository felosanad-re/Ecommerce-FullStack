import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import {
  HttpClientModule,
  provideHttpClient,
  withFetch,
  withInterceptors,
} from '@angular/common/http';

import { authInterceptor } from '../Core/Interceptors/auth.interceptor';
import { MessageService } from 'primeng/api';
import { loadingscreenInterceptor } from '../Core/Interceptors/loadingscreen.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideClientHydration(),
    MessageService, // Handdle message services from app-component
    provideAnimations(),
    // importProvidersFrom(HttpClientModule),
    provideHttpClient(
      withFetch(),
      withInterceptors([authInterceptor, loadingscreenInterceptor]),
    ), // can call api and use interseptor
  ],
};
