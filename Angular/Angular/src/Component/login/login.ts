import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { InputTextModule } from 'primeng/inputtext';
import { ToolbarModule } from 'primeng/toolbar';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { AuthService } from '../../Service/auth-service';
import { HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router'; 
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
  providers: [MessageService, AuthService],
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
})
export class Login {
  username = '';
  password = '';
  authService = inject(AuthService)

  constructor(
    private messageService: MessageService,
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
        console.log('Login successful', res);
        this.messageService.add({ severity: 'success', summary: 'כניסה', detail: 'התחברת בהצלחה' });
        
        if (res.Token) {
          localStorage.setItem('authToken', res.Token);
          sessionStorage.setItem('user', JSON.stringify(res.User));
          this.router.navigate(['/gifts']);
      }
    },
      error: (err) => {
        console.error('Login failed', err);
        this.messageService.add({ 
          severity: 'error', 
          summary: 'שגיאה', 
          detail: 'משתמש לא קיים או פרטים שגויים' 
        });
      },
      complete: () => {
        this.router.navigate(['/gifts']);
      }
    });
  }

  signup() {
    this.router.navigate(['/register']);
  }
}
