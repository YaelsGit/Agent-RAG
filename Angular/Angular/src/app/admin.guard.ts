import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Status } from '../Model/User';

export const adminGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const userData = sessionStorage.getItem('user') || localStorage.getItem('user');
  if (!userData) {
    router.navigate(['/']);
    return false;
  }

  try {
    const user: any = JSON.parse(userData);
    const rawRole = user.role ?? user.Role ?? null;

    let isAdmin = false;
    if (rawRole !== null && rawRole !== undefined) {
      if (typeof rawRole === 'number') {
        isAdmin = rawRole === Status.Admin || rawRole === 1;
      } else if (typeof rawRole === 'string') {
        const r = rawRole.trim().toLowerCase();
        isAdmin = r === 'admin' || r === String(Status.Admin) || r === '1';
      } else if (typeof rawRole === 'object') {
        const numeric = Number(rawRole.value ?? rawRole.Value ?? rawRole);
        isAdmin = !isNaN(numeric) && numeric === Status.Admin;
      }
    }

    if (!isAdmin) {
      router.navigate(['/gifts']);
      return false;
    }

    return true;
  } catch (e) {
    console.error('adminGuard parse error', e);
    router.navigate(['/']);
    return false;
  }
};