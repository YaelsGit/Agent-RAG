import { EmailValidator } from "@angular/forms";

    export enum Status
    {
        User=0,
        Admin=1
    }
 export class User {
    public id: number=0;
    public firstName: string='';
    public lastName: string='';
    public userName: string='';
    public email: EmailValidator=new EmailValidator();
    public password: string='';
    public role: Status=Status.User;
    public city: string='';
    public street: string='';
    public biuldingNumber: number=0;
 }