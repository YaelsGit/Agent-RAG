import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../Service/auth-service';
import { routes } from '../../app/app.routes';
import { Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { Status } from '../../Model/User';
import { DividerModule } from 'primeng/divider';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, InputTextModule, ButtonModule, HttpClientModule, DividerModule],
  templateUrl: './register.html',
  styleUrls: ['./register.scss'],
})
export class Register {
  authService = inject(AuthService)
  id: number = 0;
  firstName: string = '';
  lastName: string = '';
  userName: string = '';
  email: string = '';
  password: string = '';
  role: Status = Status.User;
  city: string = '';
  street: string = '';
  buildingNumber: number = 0;
  phone: string = '';

  constructor(private http: HttpClient, private router: Router) { }
AuthService= inject(AuthService)
  register() {
    if (!this.id || !this.firstName || !this.lastName || !this.userName || !this.email || !this.password || !this.city || !this.street || !this.buildingNumber || !this.phone) {
      alert("Please fill all the fields");
      return;
    }

    const userData = {
      id: Number(this.id),
      firstName: this.firstName,
      lastName: this.lastName,
      userName: this.userName,
      email: this.email,
      password: this.password,
      role: this.role,
      city: this.city,
      street: this.street,
      buildingNumber: Number(this.buildingNumber),
      phone: this.phone
    };
this.AuthService.register(userData).subscribe({
    next: (res) => {
      console.log('Success!', res);
      this.router.navigate(['']);
    },
    error: (err) => {
      console.log('Detailed Server Error:', err.error); // כאן תראי את הפירוט של ה-400
      if (err.error.errors) {
         console.table(err.error.errors); // מציג טבלה של שגיאות הוולידציה
      }
    }
  });
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}