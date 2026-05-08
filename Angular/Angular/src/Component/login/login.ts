import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { InputTextModule } from 'primeng/inputtext';
import { ToolbarModule } from 'primeng/toolbar';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { AuthService } from '../../Service/auth-service';
import { HttpClient, HttpClientModule, provideHttpClient } from '@angular/common/http';
import { Router } from '@angular/router'; 
import { bootstrapApplication } from '@angular/platform-browser';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    FormsModule,
    DividerModule,
    ButtonModule,
    InputTextModule,
    ToolbarModule,
    ToastModule,
    HttpClientModule
  ],
  providers: [MessageService],
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
  

})
export class Login {
  username = '';
  password = '';
  authService = inject(AuthService)

  constructor(
    private messageService: MessageService,
    private http: HttpClient,
    private router: Router
  ) {}

 login() {
  if (!this.username || !this.password) {
    this.messageService.add({ severity: 'warn', summary: 'שגיאה', detail: 'נא למלא פרטים' });
    return;
  }

  this.authService.login({ username: this.username, password: this.password })
    .subscribe({
      next: (res) => {
        const token = res?.Token ?? res?.token ?? null;
        if (token) {
          localStorage.setItem('authToken', token);

          const payload = JSON.parse(atob(token.split('.')[1]));
          const roleClaim = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
          
          const userRaw = res?.User ?? res?.user ?? {};
          
          const finalRole = (roleClaim === 'Admin') ? 1 : 0;
          
          const normalizedUser = { ...userRaw, role: finalRole };
          sessionStorage.setItem('user', JSON.stringify(normalizedUser));
          
          console.log('Login successful. Role detected from JWT:', finalRole);
        }

        this.messageService.add({ severity: 'success', summary: 'כניסה', detail: 'התחברת בהצלחה' });
        this.router.navigate(['/gifts']);
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'פרטים שגויים' });
      }
    });
  }

  signup() {
    this.router.navigate(['/register']);
  }
}
