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

@Component({
  selector: 'app-gifts',
  standalone: true,
  imports: [
    CommonModule,
    HttpClientModule,
    ButtonModule,
    DataViewModule,
    FormsModule,
    DialogModule,
    PopoverModule
  ],
  templateUrl: './gifts.html',
  styleUrls: ['./gifts.scss'],
  providers: [MessageService]

})
export class Gifts implements OnInit {

  serviceGifts = inject(GiftService);
  sourceGifts = signal<Gift[]>([]);  // מערך מתנות
  layout: 'list' | 'grid' = 'list';
selectedProduct = signal<any>(null);
giftQuantities: { [key: number]: number } = {};

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
  

  constructor(private http: HttpClient, private messageService: MessageService, private router: Router) {}

  ngOnInit() {
    this.getAllGifts();
  }

  getAllGifts() {
    this.serviceGifts.getallGifts().subscribe({
      next: (res) => {
        console.log('DATA FROM API:', res);
        this.sourceGifts.set([...res]); // יצירת array חדש כדי לטריגר רינדור
      },
      error: (err) => console.error('API ERROR:', err)
    });
  }

searchByName(name: string) {
  const fName = name || "";
  this.serviceGifts.getGiftByGiftName(fName).subscribe({
    next: (res: any) => {
      // בדיקה: האם השרת החזיר מערך?
      if (Array.isArray(res)) {
        this.sourceGifts.set([...res]);
      } else if (res) {
        // אם זה אובייקט בודד, נכניס אותו למערך
        this.sourceGifts.set([res]);
      } else {
        // אם חזר null או undefined
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
      // בדיקה אם res הוא מערך או אובייקט בודד
      if (Array.isArray(res)) {
        this.sourceGifts.set([...res]);
      } else if (res && typeof res === 'object') {
        // אם זה אובייקט בודד, נשים אותו בתוך מערך כדי שה-HTML יוכל להציג אותו
        this.sourceGifts.set([res]);
      } else {
        // אם לא חזר כלום, נרוקן את הרשימה
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

 createGift() {
  const newGift = { 
    name: this.name, // שימי לב אם השרת מצפה ל-name או Name
    description: this.description, 
    priceCard: Number(this.priceCard), // לוודא שזה מספר
    donorId: Number(this.donorId),     // לוודא שזה מספר ולא סטרינג
    categoryId: Number(this.categoryId), 
    pictureId: Number(this.pictureId) 
  };

  console.log('Sending to server:', newGift); // בדיקה חשובה!

  this.serviceGifts.createGift(newGift).subscribe({
    next: () => {
      this.getAllGifts();
      this.resetForm();
      this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'המתנה נוצרה בהצלחה' });
    },
    error: (err) => {
      console.error('Full Error Object:', err); // כאן תראי את הפירוט
      this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'יצירת המתנה נכשלה' });
    }
  });
}
// הקודם
// updateGift(gift: any) { ... }

// חדש – ללא פרמטר
updateGift() {
  if (!this.id) {
    alert('לא נבחרה מתנה לעדכון');
    return;
  }

  const updatedGift = {
    name: this.name,
    description: this.description,
    priceCard: this.priceCard,
    donorId: this.donorId,
    categoryId: this.categoryId,
    pictureId: this.pictureId
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
      this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'העדכון נכשל, בדקי את הנתונים' });
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
    console.log('נתוני מיון:', res); // בדקי כאן אם ה-Name קיים ב-Console
    this.sourceGifts.set([...res]);
  },
  error: (err) => alert('לא ניתן לקבל את המתנות ממוינות לפי קטגוריה או מחיר')
});
  }

  GetBySorted(sorted:string) {
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
  }, err => alert('לא ניתן להוריד את הקובץ') );
  }

  // פונקציות כמויות (שגיאה 3)
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
  if (!product) {
    this.messageService.add({ severity: 'warn', summary: 'שגיאה', detail: 'לא נבחרה מתנה' });
    return;
  }
  const qty = Number(this.giftQuantities[product.id]) || 1;
  if (qty < 1) {
    this.messageService.add({ severity: 'warn', summary: 'שגיאה', detail: 'כמות לא תקינה' });
    return;
  }
  const purchaseData = {
    id: product.id,
    Name: product.name,
    Description: product.description,
    priceCard: product.priceCard,
    Quentity: qty
  };
  const basket = JSON.parse(sessionStorage.getItem('basket') || '[]');
  basket.push(purchaseData);
  sessionStorage.setItem('basket', JSON.stringify(basket));
  this.messageService.add({ severity: 'success', summary: 'התווסף!', detail: 'המתנה הוספה לסל' });
  this.hidePopover();
  this.router.navigate(['/basket']);
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
    const isValid = !(pictureId === null || pictureId === undefined || pictureId === '' || pictureId === 0);
    return isValid ? `/assets/Image/${String(pictureId)}.png` : '/assets/Image/placeholder.png';
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
        img.src = '/assets/Image/placeholder.png';
        return;
      }
    } catch {}
    if (!img.src.endsWith('placeholder.png') && !img.src.endsWith('placeholder.jpg')) {
      img.src = '/assets/Image/placeholder.png';
    }
    if (gift && typeof gift === 'object') {
      (gift as any).noImage = true;
    }
  }
}