import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../services/api.service';

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
    // This flag is currently local UI state; backend does not yet expose raffle status
    isRaffleComplete = false;
    // Winners returned from GET /api/User/Winners (users with purchases)
    winners: any[] = [];
    messageService = inject(MessageService);
    private api = inject(ApiService);

    ngOnInit() {
        this.loadBasket();
        this.api.getWinners<any[]>()
            .then(w => this.winners = w || [])
            .catch(() => this.winners = []);
    }

    loadBasket() {
        const data = sessionStorage.getItem('basket');
        this.basketItems = data ? JSON.parse(data) : [];
    }

    removeItem(index: number) {
        if (this.isRaffleComplete) return;
        this.basketItems.splice(index, 1);
        sessionStorage.setItem('basket', JSON.stringify(this.basketItems));
    }

    getTotal() {
        return this.basketItems.reduce((sum, item) => sum + (item.priceCard || 0) * (item.Quentity || 1), 0);
    }

    confirmPurchase() {
        const userIdStr = sessionStorage.getItem('userId');

        if (!userIdStr) {
            this.messageService.add({
                severity: 'error',
                summary: 'שגיאה',
                detail: 'משתמש לא מחובר'
            });
            return;
        }

        const basketData = {
            userId: Number(userIdStr),
            items: this.basketItems.map(item => ({
                giftId: item.id,
                quantity: item.Quentity ?? 1
            }))
        };
        if (this.isRaffleComplete) return;
        this.api.confirmBasket(basketData)
            .then(() => {
                this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'הרכישה אושרה' });
                sessionStorage.removeItem('basket');
                this.basketItems = [];
                this.isRaffleComplete = true; // prevent further changes per rules
            })
            .catch(err => {
                this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: err?.message || 'שגיאת שרת' });
            });
    }
}
