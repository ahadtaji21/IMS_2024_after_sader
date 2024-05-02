
export enum AccountStatus {
    NotLogged,
    LoggingIn,
    LoggedIn,
    LoggingOut
}
export interface IAccount {
    status: AccountStatus;
    user: any; //User; // commentato perché ancora non abbiamo definito il tipo User, arriverà dall'api
}