import { Category } from "./Category";
import { Donor } from "./Donor";
import { Purchase } from "./Purchase";

export class Gift{
    public id: number=  0;
    public name: string = '';
    public description: string = '';
    public purchases: Purchase[] = [];
    public categoryId: number= 0;
    public category: Category= new Category();
    public donorId: number= 0;
    public donor: Donor= new Donor();
    public priceCard: number= 0;
    public quantity: number= 0;
    public static isRandom: boolean = false; 
    categoryName?: string; // הוספת השדה החסר
    pictureId: number = 0; // הוספת שדה תמונה
    public noImage?: boolean; // allow runtime fallback flag
}
