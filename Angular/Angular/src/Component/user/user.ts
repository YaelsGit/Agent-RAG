import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';

import { ButtonModule } from 'primeng/button';
import { Popover, PopoverModule } from 'primeng/popover';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { UserService } from '../../Service/user-service';
import { GiftService } from '../../Service/gift-service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [FormsModule, ButtonModule, PopoverModule, TableModule, TagModule],
  templateUrl: './user.html',
  styleUrls: ['./user.scss'],
})
export class User implements OnInit {

  @ViewChild('op', { static: false }) op!: Popover;

  private userService = inject(UserService);
  private giftService = inject(GiftService);

  products = signal<any[]>([]);
  selectedProduct = signal<any | null>(null);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  userId = 0;
  Date: Date = new Date();
  Quantity = 1;

  constructor(private http: HttpClient, private router: Router) {}

ngOnInit(): void {
  this.giftService.getallGifts().subscribe({
    next: (products: any[]) => {
      this.products.set(products);
    },
    error: () => {
      this.products.set([]);
      this.error.set('שגיאה בטעינת מתנות');
    }
  });
}

  displayProduct(event: Event, product: any): void {
    if (this.selectedProduct() && this.selectedProduct()?.id === product.id) {
      this.op.hide();
      this.selectedProduct.set(null);
    } else {
      this.selectedProduct.set(product);
      this.op.show(event);
    }
  }

  hidePopover(): void {
    this.op.hide();
    this.selectedProduct.set(null);
  }

  getSeverity(product: any) {
    switch (product.inventoryStatus) {
      case 'INSTOCK': return 'success';
      case 'LOWSTOCK': return 'warn';
      case 'OUTOFSTOCK': return 'danger';
      default: return null;
    }
  }

  // Add to basket
  addToBasket(product: any) {
    const purchase = {
      giftId: product.id,
      quentity: 1, // or this.Quantity if you want to use a user-selected value
      busketId: 0 // or your logic for basket id
    };
    this.userService.AddToBasket(purchase).subscribe({
      next: (res) => {
        alert('המתנה נוספה לסל בהצלחה!');
      },
      error: (err) => {
        alert('שגיאה בהוספה לסל: ' + (err?.error?.message || '')); 
      }
    });
  }

  // Ticket purchase
  ticketPurchase(product: any) {
    const purchase = {
      giftId: product.id,
      quentity: 1, // or this.Quantity
      busketId: 0
    };
    this.userService.TicketPurchase(purchase).subscribe({
      next: (res) => {
        alert('רכישת כרטיס בוצעה בהצלחה!');
      },
      error: (err) => {
        alert('שגיאה ברכישת כרטיס: ' + (err?.error?.message || ''));
      }
    });
  }

  // Confirm basket
  confirmBasket() {
    this.userService.ConfirmBasket().subscribe({
      next: (res) => {
        alert('הסל אושר בהצלחה!');
      },
      error: (err) => {
        alert('שגיאה באישור הסל: ' + (err?.error?.message || ''));
      }
    });
  }

  // Remove from basket
  removeFromBasket(purchaseId: number) {
    this.userService.RemoveFromBasket(purchaseId).subscribe({
      next: (res) => {
        alert('הפריט הוסר מהסל בהצלחה!');
      },
      error: (err) => {
        alert('שגיאה בהסרת פריט מהסל: ' + (err?.error?.message || ''));
      }
    });
  }

  // Get gifts with winners
  getGiftsWithWinners() {
    this.userService.GetGiftsWithWinners().subscribe({
      next: (res) => {
        console.log('Gifts with winners:', res);
        alert('התקבלו מתנות עם זוכים. ראה קונסול');
      },
      error: (err) => {
        alert('שגיאה בקבלת מתנות עם זוכים: ' + (err?.error?.message || ''));
      }
    });
  }

}