import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../Service/api.service';

@Component({
    selector: 'app-basket',
    standalone: true,
    imports: [CommonModule, RouterModule, ButtonModule],
    templateUrl: './basket.html',
    styleUrls: ['./basket.scss'],
    providers: [MessageService]
})
export class Basket implements OnInit {
    basketItems: any[] = [];
    isRaffleComplete = false;
    winners: any[] = [];
    totalPrice: number = 0;
    private messageService = inject(MessageService);
    private api = inject(ApiService);
    private router = inject(Router);
    ngOnInit() {
        this.loadBasket();
    }
loadBasket() {
    this.api.getBasket().subscribe({
        next: (data: any[]) => { // הוספת : any[]
            this.basketItems = data;
            this.calculateTotal();
        },
        error: (err: any) => { // הוספת : any
            console.error('Error loading basket:', err);
        }
    });
}

    calculateTotal() {
        this.totalPrice = this.basketItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    }
    updateQty(itemId: number, delta: number) {
        if (this.isRaffleComplete) return;
        const item = this.basketItems.find(it => it.id === itemId);
        if (item) {
            item.Quentity = Math.max(0, (item.Quentity || 1) + delta);
            if (item.Quentity === 0) {
                this.basketItems = this.basketItems.filter(it => it.id !== itemId);
            }
            sessionStorage.setItem('basket', JSON.stringify(this.basketItems));
        }
    }

    removeItem(item: any) {
        if (this.isRaffleComplete) return;
        this.basketItems = this.basketItems.filter(it => it.id !== item.id);
        sessionStorage.setItem('basket', JSON.stringify(this.basketItems));
    }

    getTotal() {
        return this.basketItems.reduce((sum, item) => sum + (item.priceCard || 0) * (item.Quentity || 1), 0);
    }

    async confirmPurchase() {
        if (this.isRaffleComplete) return;

        if (!this.api.isLoggedIn()) {
            this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'משתמש לא מחובר' });
            return;
        }

        try {
            await this.api.confirmBasket();

            this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'הרכישה אושרה' });
            sessionStorage.removeItem('basket');
            this.basketItems = [];
            this.isRaffleComplete = true;
        } catch (err: any) {
            console.error(err);
            this.messageService.add({
                severity: 'error',
                summary: 'שגיאה',
                detail: err?.message || 'אירעה שגיאה באישור הסל'
            });
        }
    }

    goToGifts() {
        this.router.navigate(['/gifts']);
    }
    async handleAddGift(giftId: number, quantity: number) {
        try {
            await this.api.addToBasket(giftId, quantity);

            this.messageService.add({ severity: 'success', detail: 'נוסף לסל בהצלחה' });
        } catch (err: any) {
            this.messageService.add({ severity: 'error', detail: 'הוספה נכשלה: ' + err.message });
        }
    }
}