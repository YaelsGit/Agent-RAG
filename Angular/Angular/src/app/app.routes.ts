import { Routes } from '@angular/router';
import { Login } from '../Component/login/login';
import { Register } from '../Component/register/register';
import { Donors } from '../Component/donors/donors';
import { Gifts } from '../Component/gifts/gifts';

export const routes: Routes = [
  { path: '', component: Login },
  { path: 'register', component: Register },
  { path: 'donors', component: Donors },
  { path: 'gifts', component: Gifts },
];
