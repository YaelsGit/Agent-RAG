import { Gift } from "./Gift";
import { User } from "./User";

export class Purchase {
    public id: number=0;
    public date: Date = new Date();
    public giftId: number=0;
    public gift: Gift=new Gift();
    public userId: number=0;
    public user: User=new User();
    public static totalSum: number = 0;
    public isRandom: boolean = false;
    
}