import { stringify } from "querystring";

export class LoggedInData {
    private _UserToken: string;
    private _IsCoach: boolean;
    private _Name: string;
    private _Roles: string;
    private _IsCustomer: boolean;
    private _IsHeadCoach: boolean; 

    private _email: string;
    constructor(UserToken: string, IsCoach: boolean, name: string, email: string, roles: string[], isCustomer: boolean, isHeadCoach: boolean) {
        this._UserToken = UserToken;
        this._IsCoach = IsCoach;
        this._Name = name;
        this._email = email;
        this._IsCustomer = isCustomer;
        this._IsHeadCoach = isHeadCoach
    }

    get IsHeadCoach(): boolean{
        return this._IsHeadCoach;
    }
    get IsCustomer(): boolean {
        return this._IsCustomer;
    }
    get Email(): string {
        return this._email;
    }
    get UserToken(): string {
        return this._UserToken;
    }

    get IsCoach(): boolean {
        return this._IsCoach;
    }
    get Name(): string {
        return this._Name;
    }
    get Roles(): string {
        return this._Roles;
    }
}