import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http'; // הוספנו withInterceptors
import { routes } from './app.routes';
import { providePrimeNG } from 'primeng/config';
import { MessageService } from 'primeng/api';
import { provideAnimations } from '@angular/platform-browser/animations';
import Aura from '@primeng/themes/aura';


export const appConfig: ApplicationConfig = {
  providers: [
    MessageService,
    provideHttpClient(),

    provideAnimations(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([])),

    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: 'none'
        }
      }
    }),

    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes)
  ]
};