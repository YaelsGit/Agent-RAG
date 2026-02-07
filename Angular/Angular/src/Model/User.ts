import { EmailValidator } from "../../node_modules/@angular/forms/index";

enum Role{
    Admin="Admin",
    User="User",
}
 export class User {
    public id: number=0;
    public firstName: string='';
    public lastName: string='';
    public userName: string='';
    public email: EmailValidator=new EmailValidator();
    public password: string='';
    public role: Role=Role.User;
    public city: string='';
    public street: string='';
    public biuldingNumber: number=0;
 }