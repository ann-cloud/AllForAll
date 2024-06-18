export class User {
    constructor(
        public userId: number,
        public username: string,
        public email: string,
        public password: string,
        public userRoleId: number,
        public dateJoined: Date,
        public userPhotoLink: string,
      ) {}
}