import { HttpClient } from '@angular/common/http';
import { Component, inject, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../Service/user-service';

@Component({
  selector: 'app-user',
  imports: [FormsModule],
  templateUrl: './user.html',
  styleUrl: './user.scss',
})
export class User {
  GiftId: number = 0;
  userId: number = 0;
  Date: Date = new Date();
  Quantity: number = 1;
  userService = inject(UserService)
  constructor(private http: HttpClient, private router: Router) { }
  AddToBasket() {
    if (!this.GiftId || !this.userId || !this.Date || !this.Quantity) {
      alert("Please fill all the fields");
      return;
    }
    const purchaseData = {
      GiftId: this.GiftId,
      UserId: this.userId,
      Date: this.Date,
      Quantity: this.Quantity
    };
    this.userService.AddToBasket(purchaseData).subscribe({
      next: (res) => {
        console.log('Added to basket successfully:', res);
      },
      error: (err) => {
        console.error('Failed to add to basket:', err);
      }
    });
  }
TicketPurchase() {
    if (!this.GiftId || !this.userId || !this.Date || !this.Quantity) {
      alert("Please fill all the fields");
      return;
    }
    const ticketData = {
      GiftId: this.GiftId,
      UserId: this.userId,
      Date: this.Date,
      Quantity: this.Quantity
    };
    this.userService.TicketPurchase(ticketData).subscribe({
      next: (res) => {
        console.log('Ticket purchased successfully:', res);
      },
      error: (err) => {
        console.error('Failed to purchase ticket:', err);
      }
    });
  }
  }
