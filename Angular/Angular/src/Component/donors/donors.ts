import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { MessageService } from 'primeng/api';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { DonorService } from '../../Service/donors-service';
import { Donor } from '../../Model/Donor';
import { Gift } from '../../Model/Gift';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-donors',
  templateUrl: './donors.html',
  styleUrls: ['./donors.scss'],
  standalone: true,
  providers: [MessageService],
  imports: [FormsModule, InputTextModule, ButtonModule, TableModule, DialogModule, CommonModule]
})
export class Donors implements OnInit {

  serviceDonors = inject(DonorService);
  private route = inject(ActivatedRoute);
  sourceDonors = signal<Donor[]>([]);
  id: number = 0;
  firstName: string = '';
  lastName: string = '';
  gifts: Gift[] = [];
  email: string = '';
  phone: string = '';
  displayModal: boolean = false;

  constructor(private http: HttpClient, private messageService: MessageService) { }
  craeteDonor() {
    const newDonor = {
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      phone: this.phone,
    };
    this.serviceDonors.createDonor(newDonor).subscribe({
      next: () => {
        this.getAllDonors();
        this.resetForm();
        this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'התורם נוצר בהצלחה' });
      },
      error: (err) => {
        console.error('Creation failed:', err);
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'יצירת התורם נכשלה' });
      }
    });
  }

  updateDonor() {
    console.log("Current ID:", this.id);
    console.log("Current Data:", this.firstName, this.lastName);
    console.log("Attempting to update donor with ID:", this.id);
    if (!this.id) {
      alert('לא נבחר תורם לעדכון');
      return;
    }
    const updatedDonor = {
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      phone: this.phone,
    };
    this.serviceDonors.updateDonor(this.id, updatedDonor).subscribe({
      next: () => {
        this.getAllDonors();
        this.displayModal = false;
        this.resetForm();
        this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'התורם עודכן בהצלחה' });
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'העדכון נכשל, בדקי את הנתונים' });
      }
    });
  }
  ngOnInit() {
    this.getAllDonors();
    this.route.queryParams.subscribe(params => {
      if (params['id']) {
        this.id = +params['id'];
      }
    });
  }

  getAllDonors() {
    this.serviceDonors.getallDonors().subscribe({
      next: (res: Donor[]) => this.sourceDonors.set(res),
      error: (err) => console.error(err)
    });
  }

  searchByName(first: string, last: string) {
    const fName = first || "";
    const lName = last || "";

    this.serviceDonors.getDonorByName(fName, lName).subscribe({
      next: (res) => this.sourceDonors.set(res),
      error: (err) => {
        console.error('Search by name failed', err);
        if (err.error && err.error.message) {
          alert("שגיאת שרת: " + err.error.message);
        }
      }
    });
  }
  searchByEmail(email: string) {
    if (!email) {
      this.getAllDonors();
      return;
    }

    this.serviceDonors.getDonorByEmail(email).subscribe({
      next: (res: Donor[]) => {
        this.sourceDonors.set(res);
      },
      error: (err) => {
        console.error('Search by email failed:', err);
        alert('חיפוש לפי אימייל נכשל');
      }
    });
  }

  searchByGift(giftName: string) {
    if (!giftName) {
      this.getAllDonors();
      return;
    }

    this.serviceDonors.getDonorByGift(giftName).subscribe({
      next: (res: Donor[]) => this.sourceDonors.set(res),
      error: (err) => console.error('Search by gift failed', err)
    });
  }

  deleteDonor(donor: any) {
    const donorId = donor.id || donor.Id;
    if (!donorId) {
      this.messageService.add({ severity: 'warn', summary: 'שגיאה', detail: 'לא נבחר תורם למחיקה' });
      return;
    }
    if (confirm(`האם למחוק את ${donor.firstName}?`)) {
      this.serviceDonors.deleteDonor(donorId).subscribe({
        next: () => {
          this.getAllDonors();
          this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'התורם נמחק בהצלחה' });
        },
        error: (err) => {
          console.error('Delete failed:', err);
          this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'המחיקה נכשלה' });
        }
      });
    }
  }

  resetForm() {
    this.id = 0; this.firstName = ''; this.lastName = ''; this.email = ''; this.phone = ''; this.gifts = [];
  }
  openEdit(donor: any) {
    console.log("Donor object received from table:", donor);

    this.id = donor.id || donor.Id;
    this.firstName = donor.firstName;
    this.lastName = donor.lastName;
    this.email = donor.email;
    this.phone = donor.phone;
    this.gifts = donor.gifts || [];
    this.displayModal = true;
  }
  editDonor(donor: any) {
    console.log("זה האובייקט המלא:", donor);
    this.id = donor.id;
    this.firstName = donor.firstName;
    this.lastName = donor.lastName;
    this.email = donor.email;
    this.phone = donor.phone;
    this.displayModal = true;
    console.log("Selected Donor ID:", this.id);
  }
}