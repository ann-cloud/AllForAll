import { User } from "./user.model";

export class Feedback {
    constructor(
        public feedbackId: number,
        public productId: number,
        public userId: number,
        public rating: number,
        public comment: string,
        public feedbackDate: Date,
        public user: User
      ) {}
}