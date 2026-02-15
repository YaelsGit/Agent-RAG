import { Routes } from '@angular/router';
import { Login } from '../Component/login/login';
import { Register } from '../Component/register/register';
import { Donors } from '../Component/donors/donors';
import { Gifts } from '../Component/gifts/gifts';
import { Basket } from '../Component/basket/basket';
// import { User } from '../Component/user/user';
import { Navigation } from '../Component/navigation/navigation';

export const routes: Routes = [
    { path: '', component: Navigation ,children: [
        { path: '', component: Login },
        { path: 'register', component: Register },
        { path: 'donors', component: Donors },
        { path: 'gifts', component: Gifts },
        { path: 'basket', component: Basket },
        // { path: 'user', component: User },
    ]}, // נתיב ברירת מחדל לכל נתיב לא מוכר
];
