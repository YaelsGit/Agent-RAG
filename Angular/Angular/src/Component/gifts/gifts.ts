import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GiftService } from '../../Service/gift-service';
import { Gift } from '../../Model/Gift';
import { HttpClient } from '@angular/common/http';
import { MessageService } from 'primeng/api';
import { DataViewModule } from 'primeng/dataview';
import { SelectButtonModule } from 'primeng/selectbutton';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';

// ייבוא ה-CommonModule וה-CurrencyPipe
import { CommonModule, CurrencyPipe } from '@angular/common'; 

@Component({
  selector: 'app-gifts',
  standalone: true, 
  imports: [
    CommonModule,     // כולל בתוכו את ה-CurrencyPipe
    FormsModule, 
    DataViewModule, 
    SelectButtonModule, 
    TagModule, 
    ButtonModule, 
    ToastModule,
    CurrencyPipe      // הוספה מפורשת למען הסר ספק
  ],
  templateUrl: './gifts.html',
  styleUrl: './gifts.scss',
  providers: [MessageService]
})

export class Gifts {
  serviceGifts = inject(GiftService);
    sourceGifts = signal<Gift[]>([]);

  // הגדרות עבור ה-DataView וה-SelectButton שחסרו לך:
  layout: 'list' | 'grid' = 'list'; // מצב תצוגה ברירת מחדל
  options: any[] = [
      { label: 'רשימה', value: 'list', icon: 'pi pi-bars' },
      { label: 'גריד', value: 'grid', icon: 'pi pi-table' }
    ];
    name: string = '';
    description: string = '';
    priceCard: number = 0;
    donorId: number = 0;
    categoryId: number = 0;
    id: number = 0;
    displayModal: boolean = false;
    categoryName: string = '';

  constructor(private http: HttpClient, private messageService: MessageService) { }

  getAllGifts() {
    this.serviceGifts.getallGifts().subscribe({
      next: (res: Gift[]) => this.sourceGifts.set(res),
      error: (err) => console.error(err)
    });
  }
   searchByName(name: string) {
    const fName = name || "";
    this.serviceGifts.getGiftByGiftName(fName).subscribe({
      next: (res) => this.sourceGifts.set(res),
      error: (err) => {
        console.error('Search by name failed', err);
        if (err.error && err.error.message) {
          alert("שגיאת שרת: " + err.error.message);
        }
      }
    });
  }
  searchByDonorName(first: string, last: string) {
    const FirstName = first || "";
    const LastName = last || "";

    this.serviceGifts.getGiftByDonorName(FirstName, LastName).subscribe({
      next: (res) => this.sourceGifts.set(res),
      error: (err) => {
        console.error('Search by donorName failed', err);
        if (err.error && err.error.message) {
          alert("שגיאת שרת: " + err.error.message);
        }
      }
    });
  }
  searchGiftByNumPurchase(num: number) {
    const number = num || 0;

    this.serviceGifts.GetGiftByNumPurchase(number).subscribe({
      next: (res) => this.sourceGifts.set(res),
      error: (err) => {
        console.error('Search by numPurchase failed', err);
        if (err.error && err.error.message) {
          alert("שגיאת שרת: " + err.error.message);
        }
      }
    });
  }
  createGift() {
    const newGift = {
      Name: this.name,
      Description: this.description,
      PriceCard: this.priceCard,
      DonorId: this.donorId,
      CategoryId: this.categoryId
    };
      this.serviceGifts.createGift(newGift).subscribe({
      next: () => {
        this.getAllGifts();
        this.resetForm();
        this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'המתנה נוצרה בהצלחה' });
      },
      error: (err) => {
        console.error('Creation failed:', err);
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'יצירת המתנה נכשלה' });
      }
    });
  }
   updateGift() {
    console.log("Current ID:", this.id);
    console.log("Current Data:", this.name, this.description, this.priceCard, this.donorId, this.categoryId);
    console.log("Attempting to update gift with ID:", this.id);
    if (!this.id) {
      alert('לא נבחר מתנה לעדכון');
      return;
    }
    const updatedGift = {
      name: this.name,
      description: this.description,
      priceCard: this.priceCard,
      donorId: this.donorId,
      categoryId: this.categoryId
    };
    this.serviceGifts.updateGift(this.id, updatedGift).subscribe({
      next: () => {
        this.getAllGifts();
        this.displayModal = false;
        this.resetForm();
        this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'המתנה עודכנה בהצלחה' });
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'העדכון נכשל, בדקי את הנתונים' });
      }
    });
  }

  deleteGift(gift: any) {
    const giftId = gift.id || gift.Id;
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
    const newCategory = {
      Name: this.categoryName
    };
      this.serviceGifts.createCategory(newCategory).subscribe({
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
    console.log("Gift object received from table:", gift);

    this.id = gift.id || gift.Id;
    this.name = gift.name;
    this.description = gift.description;
    this.priceCard = gift.priceCard;
    this.donorId = gift.donorId;
    this.categoryId = gift.categoryId;
    this.displayModal = true;
}
editGift(gift: any) {
  console.log("זה האובייקט המלא:", gift); 
  this.id = gift.id; 
  this.name = gift.name;
  this.description = gift.description;
  this.priceCard = gift.priceCard;
  this.donorId = gift.donorId;
  this.categoryId = gift.categoryId;
  this.displayModal = true;
  console.log("Selected Gift ID:", this.id);
}
 resetForm() {
     this.name = ''; this.description = ''; this.priceCard = 0; this.donorId = 0; this.categoryId = 0;
  }
  GetGiftPurchases(giftId: number) {
    this.serviceGifts.GetGiftPurchases(giftId).subscribe({
      next: (res) => {
        console.log('Purchases for gift ID', giftId, res);
        alert(`מספר רכישות למתנה זו: ${res.length}`);
      },
      error: (err) => {
        console.error('Failed to get purchases for gift ID', giftId, err);
        alert('לא ניתן לקבל את הרכישות עבור מתנה זו');
      }
    });
  }
  GetBySorted(sorted:string){
    this.serviceGifts.GetGiftsBySorted(sorted).subscribe({
      next: (res) => this.sourceGifts.set(res),
      error: (err) => {
        console.error('Failed to get sorted gifts', err);
        alert('לא ניתן לקבל את המתנות ממוינות');
      }
    });
  }
  GetsPurchaseWithUser(giftId: number) {
    this.serviceGifts.GetsPurchaseWithUser(giftId).subscribe({
      next: (res) => {
        console.log('Purchases with users for gift ID', giftId, res);
        alert(`מספר רכישות עם משתמשים למתנה זו: ${res.length}`);
      },
      error: (err) => {
        console.error('Failed to get purchases with users for gift ID', giftId, err);
        alert('לא ניתן לקבל את הרכישות עם משתמשים עבור מתנה זו');
      }
    });
  }
  GiftRandom() {
    this.serviceGifts.GiftRandom().subscribe({
      next: (res) => this.sourceGifts.set(res),
      error: (err) => {
        console.error('Failed to get random gifts', err);
        alert('לא ניתן לקבל מתנות אקראיות');
      }
    });
  }
  GetTotalSum() {
    this.serviceGifts.GetTotalSum().subscribe({
      next: (res) => {
        console.log('Total sum of purchases', res);
      },
      error: (err) => {
        console.error('Failed to get total sum of purchases', err);
        alert('לא ניתן לקבל את הסכום הכולל של הרכישות');
      }
    });
  }
  GetGiftBySordedCategoryOrPrice(sortedBy: string) {
    this.serviceGifts.GetGiftsBySortedCategoryOrPrice(sortedBy).subscribe({
      next: (res) => this.sourceGifts.set(res),
      error: (err) => {
        console.error('Failed to get gifts sorted by category or price', err);
        alert('לא ניתן לקבל את המתנות ממוינות לפי קטגוריה או מחיר');
      }
    });
  }
  ngOnInit() {
    this.getAllGifts();
  }
}