import { Category } from "./category.model";
import { Feedback } from "./feedback.model";
import { Manufacturer } from "./manufacturer.model";

export class Product {
    constructor(
        public productId: number,
        public productName: string,
        public categoryId: number,
        public manufacturerId: number,
        public creationDate: Date,
        public productPhotoLink: string,
        public isVerified: boolean,
        public country: string,
        public userId: number,
        public manufacturer: Manufacturer,
        public category: Category,
        public feedbacks: Feedback[],
      ) {}
}