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

}