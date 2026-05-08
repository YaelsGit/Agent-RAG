import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GiftService } from '../../Service/gift-service';
import { MessageService } from 'primeng/api';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ButtonModule } from 'primeng/button';
import { DataViewModule } from 'primeng/dataview';
import { Gift } from '../../Model/Gift';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { Popover, PopoverModule } from 'primeng/popover';
import { Router } from '@angular/router';
import { UserService } from '../../Service/user-service';
import { ApiService } from '../../Service/api.service';

@Component({
  selector: 'app-gifts',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    DataViewModule,
    FormsModule,
    DialogModule,
    PopoverModule,
    HttpClientModule

  ],
  templateUrl: './gifts.html',
  styleUrls: ['./gifts.scss'],
  providers: [MessageService]

})
export class Gifts implements OnInit {

  serviceGifts = inject(GiftService);
  userService = inject(UserService);
  sourceGifts = signal<Gift[]>([]);
  layout: 'list' | 'grid' = 'list';
  selectedProduct = signal<any>(null);
  giftQuantities: { [key: number]: number } = {};
  api=inject(ApiService);
  quantity = 1;
  name = '';
  description = '';
  priceCard = 0;
  donorId = 0;
  categoryId = 0;
  id = 0;
  displayModal = false;
  categoryName = '';
  pictureId = 0;
  userId = 0;
  date: Date = new Date();


  constructor(private http: HttpClient, private messageService: MessageService, private router: Router) { }

  ngOnInit() {
    this.getAllGifts();
  }

  getAllGifts() {
    this.serviceGifts.getallGifts().subscribe({
      next: (res) => {
        console.log('DATA FROM API:', res);
        this.sourceGifts.set([...res]);
      },
      error: (err) => console.error('API ERROR:', err)
    });
  }

  searchByName(name: string) {
    const fName = name || "";
    this.serviceGifts.getGiftByGiftName(fName).subscribe({
      next: (res: any) => {
        if (Array.isArray(res)) {
          this.sourceGifts.set([...res]);
        } else if (res) {
          this.sourceGifts.set([res]);
        } else {
          this.sourceGifts.set([]);
        }
      },
      error: (err) => {
        console.error('Search by name failed', err);
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: 'לא נמצאו תוצאות'
        });
      }
    });
  }

  searchByDonorName(first: string, last: string) {
    this.serviceGifts.getGiftByDonorName(first || "", last || "").subscribe({
      next: (res: any) => {
        if (Array.isArray(res)) {
          this.sourceGifts.set([...res]);
        } else if (res && typeof res === 'object') {
          this.sourceGifts.set([res]);
        } else {
          this.sourceGifts.set([]);
        }
      },
      error: (err) => {
        console.error('Search by donorName failed', err);
        if (err.error?.message) alert("שגיאת שרת: " + err.error.message);
      }
    });
  }

  searchGiftByNumPurchase(num: number) {
    this.serviceGifts.GetGiftByNumPurchase(num || 0).subscribe({
      next: (res) => this.sourceGifts.set([...res]),
      error: (err) => {
        console.error('Search by numPurchase failed', err);
        if (err.error?.message) alert("שגיאת שרת: " + err.error.message);
      }
    });
  }


  deleteGift(gift: any) {
    console.log('Gift to delete:', gift);
    const giftId = gift.id;
    console.log('Deleting gift with Id:', giftId);

    if (!giftId) {
      this.messageService.add({ severity: 'warn', summary: 'שגיאה', detail: 'לא נבחר מתנה למחיקה' });
      return;
    }

    if (confirm(`האם למחוק את ${gift.name}?`)) {
      this.serviceGifts.deleteGift(giftId).subscribe({
        next: () => {
          this.getAllGifts();
          this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'המתנה נמחקה בהצלחה' });
        },
        error: (err) => {
          console.error('Delete failed:', err);
          this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'המחיקה נכשלה' });
        }
      });
    }
  }

  createGift() {
    // השתמש בשמות שדות התואמים בדיוק למחלקה (DTO) בשרת שלך
    const newGift = {
      Name: this.name,
      Description: this.description,
      PriceCard: Number(this.priceCard),
      DonorId: Number(this.donorId),
      CategoryId: Number(this.categoryId),
      PictureId: Number(this.pictureId)
    };

    this.serviceGifts.createGift(newGift).subscribe({
      next: () => {
        this.getAllGifts();
        this.resetForm();
        this.displayModal = false; 
        this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'המתנה נוצרה בהצלחה' });
      },
      error: (err) => {
        console.error('Creation failed:', err);
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'יצירת המתנה נכשלה' });
      }
    });
  }

  updateGift() {
    if (!this.id) {
      this.messageService.add({ severity: 'warn', summary: 'שגיאה', detail: 'לא נבחרה מתנה לעדכון' });
      return;
    }

    const updatedGift = {
      Id: this.id,
      Name: this.name,
      Description: this.description,
      PriceCard: Number(this.priceCard),
      DonorId: Number(this.donorId),
      CategoryId: Number(this.categoryId),
      PictureId: this.pictureId
    };

    this.serviceGifts.updateGift(this.id, updatedGift).subscribe({
      next: () => {
        this.getAllGifts();
        this.displayModal = false;
        this.resetForm();
        this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'המתנה עודכנה בהצלחה' });
      },
      error: (err) => {
        console.error('Update failed:', err);
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'העדכון נכשל, בדוק את הנתונים' });
      }
    });
  }

  createCategory() {
    this.serviceGifts.createCategory({ Name: this.categoryName }).subscribe({
      next: () => {
        this.getAllGifts();
        this.resetForm();
        this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'הקטגוריה נוצרה בהצלחה' });
      },
      error: (err) => {
        console.error('Creation failed:', err);
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'יצירת הקטגוריה נכשלה' });
      }
    });
  }

  openEdit(gift: any) {
    if (!gift) return;
    if (!this.isAdmin()) {
      this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'אין לך הרשאה לערוך' });
      return;
    }
    this.id = gift.id ?? gift.Id ?? 0;
    this.name = gift.name ?? gift.Name ?? '';
    this.description = gift.description ?? gift.Description ?? '';
    this.priceCard = gift.priceCard ?? gift.PriceCard ?? 0;
    this.donorId = gift.donorId ?? gift.DonorId ?? 0;
    this.categoryId = gift.categoryId ?? gift.CategoryId ?? 0;
    this.pictureId = gift.pictureId ?? gift.PictureId ?? 0;
    this.displayModal = true;
  }


  resetForm() {
    this.name = '';
    this.description = '';
    this.priceCard = 0;
    this.donorId = 0;
    this.categoryId = 0;
    this.pictureId = 0;
  }

  GetGiftPurchases(giftId: number) {
    if (!giftId) {
      console.error('Gift ID is undefined!');
      return;
    }
    this.serviceGifts.GetGiftPurchases(giftId).subscribe({
      next: (res) => alert(`מספר רכישות למתנה זו: ${res.length}`),
      error: (err) => alert('לא ניתן לקבל את הרכישות עבור מתנה זו')
    });
  }

  GetsPurchaseWithUser(giftId: number) {
    if (!giftId) {
      console.error('Gift ID is undefined!');
      return;
    }

    this.serviceGifts.GetsPurchaseWithUser(giftId).subscribe({
      next: (res) => alert(`מספר רכישות עם משתמשים למתנה זו: ${res.length}`),
      error: (err) => alert('לא ניתן לקבל את הרכישות עם משתמשים עבור מתנה זו')
    });
  }

  GiftRandom() {
    this.serviceGifts.GiftRandom().subscribe({
      next: (res: any) => this.sourceGifts.set([...res]),
      error: (err: any) => alert('לא ניתן לקבל מתנות אקראיות')
    });
  }


  GetTotalSum() {
    this.serviceGifts.GetTotalSum().subscribe({
      next: (res) => console.log('Total sum of purchases', res),
      error: (err) => alert('לא ניתן לקבל את הסכום הכולל של הרכישות')
    });
  }

  GetGiftBySordedCategoryOrPrice(sortedBy: string) {
    this.serviceGifts.GetGiftsBySorted(sortedBy).subscribe({
      next: (res) => {
        console.log('נתונים ממוינים:', res); // בדקי כאן אם לכל מתנה יש pictureId תקין
        this.sourceGifts.set([...res]);
      },
      error: (err) => alert('לא ניתן לקבל את המתנות ממוינות לפי קטגוריה או מחיר')
    });
  }

  GetBySorted(sorted: string) {
    this.serviceGifts.GetGiftsBySorted(sorted).subscribe({
      next: (res) => this.sourceGifts.set([...res]),
      error: (err) => alert('לא ניתן לקבל את המתנות ממוינות')
    });
  }

  trackByGiftId(index: number, gift: Gift) {
    return gift.id;
  }
  downloadWinners() {
    this.serviceGifts.DownloadGiftRandomFile().subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'giftWinners.json';
      a.click();
      window.URL.revokeObjectURL(url);
    }, err => alert('לא ניתן להוריד את הקובץ'));
  }

  updateQty(giftId: number, delta: number): void {
    if (this.giftQuantities[giftId] === undefined) {
      this.giftQuantities[giftId] = 0;
    }
    const newQty = this.giftQuantities[giftId] + delta;
    this.giftQuantities[giftId] = newQty >= 0 ? newQty : 0;
  }

  getQty(giftId: number): number {
    return this.giftQuantities[giftId] || 0;
  }

AddToBasket(): void {
  const product = this.selectedProduct();
  if (!product) return;

  const qty = Number(this.giftQuantities[product.id]) || 1;

  const purchaseData = {
    giftId: product.id,
    quantity: qty
  };

  this.userService.AddToBasket(purchaseData).subscribe({
    next: () => {
      this.messageService.add({
        severity: 'success',
        summary: 'נוסף לסל',
        detail: 'המוצר נוסף בהצלחה'
      });
    },
    error: (err) => {
      console.error('שגיאה:', err);
      this.messageService.add({
        severity: 'error',
        summary: 'שגיאה',
        detail: 'לא הצלחנו להוסיף לסל'
      });
    }
  });
}
  selectAndAddToBasket(gift: any) {
    this.selectedProduct.set(gift);
    this.AddToBasket();
  }

  hidePopover(): void {
    this.displayModal = false;
    this.selectedProduct.set(null);
  }
  buildImageSrc(pictureId: number | string | null | undefined): string {
    return pictureId ? `/Image/${pictureId}.png` : '/Image/placeholder.png';
  }

  onImgError(event: Event, gift?: any) {
    const img = event.target as HTMLImageElement;
    img.onerror = null;
    try {
      const url = new URL(img.src);
      if (url.pathname.endsWith('.png')) {
        img.src = url.pathname.replace('.png', '.jpg');
        return;
      }
      if (url.pathname.endsWith('.jpg') && !url.pathname.endsWith('placeholder.jpg') && !url.pathname.endsWith('placeholder.png')) {
        img.src = '/Image/placeholder.png';
        return;
      }
    } catch { }
    if (!img.src.endsWith('placeholder.png') && !img.src.endsWith('placeholder.jpg')) {
      img.src = '/Image/placeholder.png';
    }
    if (gift && typeof gift === 'object') {
      (gift as any).noImage = true;
    }
  }
  ticketPurchase(gift: Gift) {
    const addToBasketData = {
      giftId: gift.id,
      quentity: 1,
      busketId: 0
    };

    this.userService.AddToBasket(addToBasketData).subscribe({
      next: (basketRes) => {
        const purchaseData = {
          giftId: gift.id,
          quentity: 1,
          busketId: 0
        };
        this.userService.TicketPurchase(purchaseData).subscribe({
          next: (res) => {
                        this.router.navigate(['/basket']); // נווט לעמוד הסל לאחר הרכישה, אם תרצה

            this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'רכישת כרטיס בוצעה בהצלחה!' });
          },
          error: (err) => {
            this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'שגיאה ברכישת כרטיס: ' + (err?.error?.message || '') });
          }
        });
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'שגיאה בהוספה לסל: ' + (err?.error?.message || '') });
      }
    });
  }
  isAdmin(): boolean {
    const userStr = sessionStorage.getItem('user');
    if (!userStr) return false;
    try {
      const user = JSON.parse(userStr);
      return user.role === 1; 
    } catch {
      return false;
    }
  }
}