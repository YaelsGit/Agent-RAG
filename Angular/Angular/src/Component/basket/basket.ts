import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../Service/api.service';
import { from } from 'rxjs';

@Component({
    selector: 'app-basket',
    standalone: true,
    imports: [CommonModule, RouterModule, ButtonModule],
    templateUrl: './basket.html',
    styleUrls: ['./basket.scss'],
    providers: [MessageService, ApiService]
})
export class Basket implements OnInit {
    basketItems: any[] = [];
    isRaffleComplete = false;
    winners: any[] = [];
    messageService = inject(MessageService);
    private api = inject(ApiService);
    private router = inject(Router);

    ngOnInit() {
        this.loadBasket();
        from(this.api.getWinners<any[]>())
            .subscribe({
                next: w => this.winners = w || [],
                error: () => this.winners = []
            });
    }

    loadBasket() {
        const data = sessionStorage.getItem('basket');
        this.basketItems = data ? JSON.parse(data) : [];
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

    goToGifts() {
        this.router.navigate(['/gifts']);
    }

   confirmPurchase() {
        // Try to get userId from sessionStorage, or extract from stored user object
        let userId: number | null = null;

        const userIdStr = sessionStorage.getItem('userId');
        if (userIdStr) {
            userId = Number(userIdStr);
        } else {
            const userStr = sessionStorage.getItem('user');
            if (userStr) {
                try {
                    const user = JSON.parse(userStr);
                    userId = user.id;
                } catch (e) {
                    console.error('Failed to parse user from sessionStorage', e);
                }
            }
        }

        if (!userId) {
            this.messageService.add({
                severity: 'error',
                summary: 'שגיאה',
                detail: 'משתמש לא מחובר'
            });
            return;
        }

        const basketData = {
            userId: userId,
            items: this.basketItems.map(item => ({
                giftId: item.id,
                quantity: item.Quentity ?? 1
            }))
        };
        if (this.isRaffleComplete) return;
        from(this.api.confirmBasket(basketData))
            .subscribe({
                next: () => {
                    this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'הרכישה אושרה' });
                    sessionStorage.removeItem('basket');
                    this.basketItems = [];
                    this.isRaffleComplete = true;
                },
                error: err => {
                    this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: err?.message || 'שגיאת שרת' });
                }
            });
    }
}
