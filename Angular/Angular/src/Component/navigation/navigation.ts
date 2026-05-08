import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { filter } from 'rxjs/operators';
import { Status, User } from '../../Model/User';

@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [CommonModule, RouterModule, ButtonModule],
  templateUrl: './navigation.html',
  styleUrls: ['./navigation.scss']
})
export class Navigation implements OnInit {
  items: any[] = [];

  constructor(private router: Router) { }

  ngOnInit() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => this.updateMenu());
    this.updateMenu();
  }

  updateMenu() {
    const userData = sessionStorage.getItem('user');
    console.log('--- שלב 1: בדיקת נתונים מ-sessionStorage ---', userData);
    if (!userData) {
      console.log('--- שלב 2: אין נתונים, מציג תפריט אורח ---');
      this.setGuestMenu();
      return;
    }

    try {
      const user: any = JSON.parse(userData);
      console.log('Parsed user object from session:', user);

      // Support different JSON shapes: 'role' or 'Role', numeric or string
      const rawRole = user.role ?? user.Role ?? null;
      console.log('Raw role value detected:', rawRole);

      let isAdmin = false;

      if (rawRole !== null && rawRole !== undefined) {
        if (typeof rawRole === 'number') {
          isAdmin = rawRole === Status.Admin || rawRole === 1;
        } else if (typeof rawRole === 'string') {
          const r = rawRole.trim().toLowerCase();
          isAdmin = r === 'admin' || r === String(Status.Admin) || r === '1';
        } else if (typeof rawRole === 'object') {
          // Handle possible enum object shapes
          const numeric = Number(rawRole.value ?? rawRole.Value ?? rawRole);
          isAdmin = !isNaN(numeric) && numeric === Status.Admin;
        }
      }

      console.log('isAdmin resolved to:', isAdmin);

      if (isAdmin) {
        this.items = [
          { label: 'ניהול תורמים', icon: 'pi pi-fw pi-users', routerLink: ['/donors'] },
          { label: 'מתנות/תמונות', icon: 'pi pi-fw pi-images', routerLink: ['/gifts'] },
          { label: 'יציאה', icon: 'pi pi-fw pi-sign-out', command: () => this.logout() }
        ];
      } else {
        this.items = [
          { label: 'מתנות', icon: 'pi pi-fw pi-images', routerLink: ['/gifts'] },
          { label: 'סל', icon: 'pi pi-fw pi-shopping-cart', routerLink: ['/basket'] },
          { label: 'יציאה', icon: 'pi pi-fw pi-sign-out', command: () => this.logout() }
        ];
      }
    } catch (e) {
      console.error('Error parsing user:', e);
      this.logout();
    }
  }

  private setGuestMenu() {
    this.items = [
      { label: 'כניסה', icon: 'pi pi-fw pi-sign-in', routerLink: ['/'] },
      { label: 'הרשמה', icon: 'pi pi-fw pi-user-plus', routerLink: ['/register'] }
    ];
  }

  logout() {
    sessionStorage.clear();
    this.setGuestMenu();
    this.router.navigate(['/']);
  }
}