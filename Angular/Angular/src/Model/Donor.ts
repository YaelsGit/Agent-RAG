import { EmailValidator } from "../../node_modules/@angular/forms/index";
import { Gift } from "./Gift";

export class Donor {
    public id: number= 0;
    public firstName: string = '';
    public lastName: string = '';
    public gifts: Gift[] = [];
    public email:  string = '';
    public phone: string = '';
  
}