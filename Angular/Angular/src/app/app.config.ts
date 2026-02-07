import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http'; // הוספנו withInterceptors
import { routes } from './app.routes';
import { providePrimeNG } from 'primeng/config';
import { MessageService } from 'primeng/api';
import { provideAnimations } from '@angular/platform-browser/animations';
import Aura from '@primeng/themes/aura'; 

// ייבוא של ה-Interceptor שיצרת (שימי לב שהנתיב נכון לפי המקום שבו שמרת אותו)
import { authInterceptor } from './interceptors/auth.interceptor'; 

export const appConfig: ApplicationConfig = {
  providers: [
    MessageService,
    provideAnimations(),
    
    // כאן העדכון המרכזי:
    provideHttpClient(
      withInterceptors([authInterceptor])
    ), 

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