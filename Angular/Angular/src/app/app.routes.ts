import { Routes } from '@angular/router';
import { Login } from '../Component/login/login';
import { Register } from '../Component/register/register';
import { Donors } from '../Component/donors/donors';
import { Gifts } from '../Component/gifts/gifts';
import { Basket } from '../Component/basket/basket';
import { User } from '../Component/user/user';
import { authGuard } from './auth.guard';
import { adminGuard } from './admin.guard';
import { userGuard } from './user.guard';

export const routes: Routes = [
    { path: '', component: Login },
    { path: 'register', component: Register },
    { path: 'donors', component: Donors, canActivate: [authGuard, adminGuard] },
    { path: 'gifts', component: Gifts, canActivate: [authGuard] },
    { path: 'basket', component: Basket, canActivate: [authGuard, userGuard] },
    { path: 'user', component: User, canActivate: [authGuard] },
    { path: '**', redirectTo: '' }
];
