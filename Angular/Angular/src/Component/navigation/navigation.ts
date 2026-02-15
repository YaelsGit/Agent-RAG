import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { filter } from 'rxjs/operators';
import { Status, User } from '../../Model/User';

/**
 * Navigation component for the main app menu.
 * Handles dynamic menu items based on user authentication and role.
 * Responsive and accessible navigation bar.
 */
@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [CommonModule, RouterModule, ButtonModule],
  templateUrl: './navigation.html',
  styleUrls: ['./navigation.scss']
})
export class Navigation implements OnInit {
  /**
   * Menu items to display in the navigation bar.
   */
  items: any[] = [];

  /**
   * The current user object, or null if not logged in.
   */
  currentUser: User | null = null;

  constructor(private router: Router) {}

  /**
   * Initializes the navigation component and subscribes to route changes.
   */
  ngOnInit() {
    // מאזין לשינויי ניווט: בכל פעם שהעמוד משתנה, התפריט בודק מחדש את מצב המשתמש
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.updateMenu();
    });

    // הרצה ראשונית בטעינת הקומפוננטה
    this.updateMenu();
  }

  /**
   * Updates the navigation menu based on the user's authentication and role.
   * Handles legacy user objects and robustly checks for admin status.
   */
  updateMenu() {
    const userData = sessionStorage.getItem('user');
    console.log('Raw user data from session:', userData);

    if (!userData) {
      this.items = [
        { label: 'כניסה', icon: 'pi pi-fw pi-sign-in', routerLink: ['/'] },
        { label: 'הרשמה', icon: 'pi pi-fw pi-user-plus', routerLink: ['/register'] }
      ];
      this.currentUser = null;
      return;
    }

    try {
      // Use the User model for type safety
      const user: any = JSON.parse(userData); // Use 'any' for robust legacy support
      this.currentUser = user;
      console.log('Parsed user object:', user);

      // Robust admin check for different possible role types
      let isAdmin = false;
      if (typeof user.role === 'number' && user.role === 1) {
        isAdmin = true;
      } else if (typeof user.role === 'string' && user.role.trim().toLowerCase() === 'admin') {
        isAdmin = true;
      } else if (user.role === Status.Admin) {
        isAdmin = true;
      }

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
      console.error('Error parsing user data', e);
      this.logout(); // אם הנתונים ב-Session משובשים, עדיף לצאת
    }
  }

  /**
   * Logs out the user, clears session/local storage, and navigates to the login page.
   */
  logout() {
    // ניקוי נתונים וחזרה לדף הכניסה
    sessionStorage.removeItem('user');
    sessionStorage.removeItem('authToken');
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
    this.currentUser = null;
    this.router.navigate(['']);
    // התפריט יתעדכן אוטומטית בגלל ה-NavigationEnd
  }
}