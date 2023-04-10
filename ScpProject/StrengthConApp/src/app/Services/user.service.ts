import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { LoggedInData } from '../Models/LoggedInData';
import { CookieService } from 'ngx-cookie-service';
import { subscribeOn, map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { StripeCustomerData } from '../Components/stripe-data/StripeCustomerData';
import { VerifyToken } from '../Models/Token/VerifyToken';
import { UpdatePassword } from '../Models/AccountSettings/UpdatePassword';
import { AccountSettings } from '../Models/AccountSettings/AccountSettings';
@Injectable({
    providedIn: 'root'
})
export class UserService {

    private _headers;
    constructor(private http: HttpClient, private cookieService: CookieService, public router: Router) {
        this._headers = this._headers = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Credentials': 'true'
            })
        }
    }
    UpdateUser(newSettings: AccountSettings) {
        return this.http.post(environment.endpointURL + `/api/User/UpdateUserInfo`
            , newSettings, {
            withCredentials: true,
            observe: 'body',
            headers: this._headers
        });
    }
    UpdatePassword(password: string, confirmPassword: string) {
        let passWordChangeModel: UpdatePassword = { Password: password, ConfirmPassword: confirmPassword };
        return this.http.post(environment.endpointURL + `/api/User/Logout`
            , passWordChangeModel, {
            withCredentials: true,
            observe: 'body',
            headers: this._headers
        });
    }
    MarkCustomerAsInvalid() {
        this.cookieService.set(environment.userCookieIsCustomer, "0", 1, "/", environment.userCookieDomain);
    }
    MarkCustomerBadCard() {
        this.cookieService.set(environment.userCookieBadCreditCard, "0", 1, "/", environment.userCookieDomain);
    }
    MarkCustomerCreditCardExpiring() {
        this.cookieService.set(environment.userCookieCreditCardExpiring, "0", 1, "/", environment.userCookieDomain);
    }
    MarkCustomerSubscriptionEnded() {
        this.cookieService.set(environment.userCookieSubscriptionEnded, "0", 1, "/", environment.userCookieDomain);
    }
    GetUserToken() {
        return this.cookieService.get(environment.userCookieSettingName);
    }

    Logout() {
        //subdomains cannot delete root domain cookies, but we can set them to nothing so that is what we are doing
        this.http.post(environment.endpointURL + `/api/User/Logout`
            , '', {
            withCredentials: true,
            observe: 'body',
            headers: this._headers
        }).subscribe(success => {
            this.cookieService.set(environment.userCookieSettingName, "", 1, "/", environment.userCookieDomain);
            this.cookieService.set(environment.userCookieCoachName, "", 1, "/", environment.userCookieDomain);
            this.cookieService.set(environment.userCookieName, "", 1, "/", environment.userCookieDomain)
            this.cookieService.set(environment.userCookieWeightRoom, "", 1, "/", environment.userCookieDomain);
            this.cookieService.set(environment.userCookieRoles, "", 1, "/", environment.userCookieDomain)
            this.cookieService.set(environment.userCookieEmail, "", 1, "/", environment.userCookieDomain);
            this.cookieService.set(environment.userCookieIsCustomer, "0", 1, "/", environment.userCookieDomain);
            this.cookieService.set(environment.userCookieIsHeadCoach, "0", 1, "/", environment.userCookieDomain)
            this.router.navigate(['/Login']);
        },
            failure => {
                this.cookieService.set(environment.userCookieSettingName, "", 1, "/", environment.userCookieDomain);
                this.cookieService.set(environment.userCookieCoachName, "", 1, "/", environment.userCookieDomain);
                this.cookieService.set(environment.userCookieName, "", 1, "/", environment.userCookieDomain)
                this.cookieService.set(environment.userCookieWeightRoom, "", 1, "/", environment.userCookieDomain);
                this.cookieService.set(environment.userCookieRoles, "", 1, "/", environment.userCookieDomain)
                this.cookieService.set(environment.userCookieEmail, "", 1, "/", environment.userCookieDomain);
                this.cookieService.set(environment.userCookieIsCustomer, "0", 1, "/", environment.userCookieDomain);
                this.cookieService.set(environment.userCookieIsHeadCoach, "0", 1, "/", environment.userCookieDomain)
                this.router.navigate(['/Login']);
            });
    }

    LogInWeightRoomUser() {
        this.http.get(environment.endpointURL + `/api/Organization/GetWeightRoomToken`, {
            withCredentials: true,
            observe: 'body',
            headers: this._headers
        }).subscribe(success => {
            this.cookieService.set(environment.userCookieSettingName, "", 1, "/", environment.userCookieDomain);
            this.cookieService.set(environment.userCookieCoachName, "", 1, "/", environment.userCookieDomain);
            this.cookieService.set(environment.userCookieName, "", 1, "/", environment.userCookieDomain)
            this.cookieService.set(environment.userCookieSettingName, success.toString(), 1, "/", environment.userCookieDomain);
            this.cookieService.set(environment.userCookieWeightRoom, "1", 1, "/", environment.userCookieDomain);
            this.cookieService.set(environment.userCookieRoles, "", 1, "/", environment.userCookieDomain);
            this.cookieService.set(environment.userCookieEmail, "", 1, "/", environment.userCookieDomain);
            this.cookieService.set(environment.userCookieIsHeadCoach, "0", 1, "/", environment.userCookieDomain)
        })
    }
    ///Returns the usersToken, if it exists we assume they are logged in and will double check.
    ///if the token doesnt exists there is 0 chance of them being logged in
    IsLoggedIn() {
        return this.cookieService.get(environment.userCookieSettingName);
    }
    IsCoach(): boolean {
        return this.cookieService.get(environment.userCookieCoachName) === '1' ? true : false;
    }
    IsWeightRoomAccount(): boolean {
        return this.cookieService.get(environment.userCookieWeightRoom) === '1' ? true : false;
    }
    IsHeadCoach(): boolean {
        return this.cookieService.get(environment.userCookieIsHeadCoach) === '1' ? true : false;
    }
    GetUserName(): string {
        return this.cookieService.get(environment.userCookieName);
    }
    GetEmail(): string {
        return this.cookieService.get(environment.userCookieEmail);
    }
    IsAdmin(): boolean {
        return this.cookieService.get(environment.userCookieRoles).split(',')[0] === "true";
    }
    Login(userName: string, password: string) {
        return this.http.post(environment.endpointURL + "/api/User/Login", JSON.stringify({ UserName: userName, Password: password })
            , {
                observe: 'body', withCredentials: true, headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin': '*',
                    'Access-Control-Allow-Credentials': 'true'
                })
            }).pipe(map(
                (data: LoggedInData) => {
                    var isCoach = data.IsCoach ? "1" : "0";
                    var isCustomer = data.IsCustomer ? "1" : "0";
                    var isHeadCoach = data.IsHeadCoach ? "1" : "0"
                    this.cookieService.set(environment.userCookieSettingName, "", 1, "/", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieSettingName, data.UserToken, 1, "/", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieCoachName, "", 1, "/", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieCoachName, isCoach, 1, "/", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieName, "", 1, "/", environment.userCookieDomain)
                    this.cookieService.set(environment.userCookieName, data.Name, 1, "/", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieRoles, "", 1, "/", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieRoles, data.Roles, 1, "/", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieEmail, data.Email, 1, "/", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieWeightRoom, "0", 1, "/", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieIsCustomer, "", 1, "/", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieIsCustomer, isCustomer, 1, "/", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieIsHeadCoach, isHeadCoach, 1, "/", environment.userCookieDomain)
                    return true;
                }));
    }
    RegisterCoach(emailToken: string) {
        return this.http.get(environment.endpointURL + `/api/User/UserEmailVerification/${emailToken}`
            , {
                withCredentials: true,
                observe: 'body',
                headers: this._headers
            })
    }
    VerifyToken(): Observable<VerifyToken> {
        return this.http.get<VerifyToken>(environment.endpointURL + `/api/User/VerifyToken/`
            , {
                withCredentials: true,
                observe: 'body',
                headers: this._headers
            })
    }
    FinishPasswordReset(password: string, confirmPassword: string, passwordValidationToken: string, userId: number) {
        return this.http.post(environment.endpointURL + "/api/User/FinishResettingPassword"
            , JSON.stringify({ UserId: userId, ValidationToken: passwordValidationToken, Password: password, ConfirmPassword: confirmPassword })
            , {
                observe: 'body', withCredentials: true, headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin': '*',
                    'Access-Control-Allow-Credentials': 'true'
                })
            });

    }
    Register(userName: string, password: string, confirmPassword: string, email: string, firstName: string, lastName: string, orgId: number) {
        return this.http.post(environment.endpointURL + "/api/User/Register"
            , JSON.stringify({ UserName: userName, Password: password, ConfirmPassword: confirmPassword, Email: email, FirstName: firstName, LastName: lastName, OrgId: orgId })
            , {
                observe: 'body', withCredentials: true, headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin': '*',
                    'Access-Control-Allow-Credentials': 'true'
                })
            }).pipe(map(
                (data: LoggedInData) => {
                    this.cookieService.set(environment.userCookieSettingName, data.UserToken, 1, "", environment.userCookieDomain);
                    this.cookieService.set(environment.userCookieWeightRoom, "0", 1, "", environment.userCookieDomain);
                    return true;
                }));
    }
    RecoverUserName(email: string) {
        return this.http.post(environment.endpointURL + `/api/User/ResendUserName/`
            , JSON.stringify(email), {
            withCredentials: true,
            observe: 'body',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Credentials': 'true'
            })
        })

    }
    //this doesnt reset the password, this triggers the email to reset passwords
    ResetPassword(email: string) {
        return this.http.post(environment.endpointURL + `/api/User/ResetPassword/`
            , JSON.stringify(email), {
            withCredentials: true,
            observe: 'body',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Credentials': 'true'
            })
        })
    }
    public CheckUserNameExists(name: string) {
        return this.http.post(environment.endpointURL + `/api/User/UserNameInUse`
            , { userName: name }, {
            withCredentials: true,
            observe: 'body',
            headers: this._headers
        });
    }
    public EmailInUse(email: string) {
        return this.http.post(environment.endpointURL + `/api/User/EmailInUse`
            , { Email: email }, {
            withCredentials: true,
            observe: 'body',
            headers: this._headers
        });
    }
    //this resets the password, this triggers the email to reset passwords
    ConfirmPassword(email: string) {
        return this.http.post(environment.endpointURL + `/api/User/ResetPassword/`
            , JSON.stringify(email), {
            withCredentials: true,
            observe: 'body',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Credentials': 'true'
            })
        })
    }
    DeleteOrgId(orgId: number) {
        return this.http.get(environment.endpointURL + `/api/Organization/DeleteOrganization/${orgId}`
            , {
                withCredentials: true,
                observe: 'body',
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin': '*',
                    'Access-Control-Allow-Credentials': 'true'
                })
            });
    }

    SaveStripeGuid(SessionId: string, FinalUrl: string) {
        return this.http.get(environment.endpointURL + `/api/Organization/UpdateStripeForOrganization?SessionId=` + SessionId
            , {
                withCredentials: true,
                observe: 'body',
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin': '*',
                    'Access-Control-Allow-Credentials': 'true'
                })
            }).subscribe(data => {
                window.location.href = FinalUrl;
            });
    }


    /**
    * Creates a new stripe session in the backend & returns the session object
    */
    GetStripeSession(organizationId: number, subscriptionPlan: number) {
        return this.http.get(environment.endpointURL + `/api/User/StripeCart/${organizationId}/${subscriptionPlan}`
            , {
                withCredentials: false,
                observe: 'body',
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin': '*',
                    'Access-Control-Allow-Credentials': 'true'
                })
            });
    }

    /**
    * Creates a new stripe session in the backend & returns the session object
    */
    CreateStripeCustomer(organizationId: number, subscriptionPlan: number, stripeCustomerData: StripeCustomerData) {
        return this.http.post(environment.endpointURL + `/api/User/StripeCart/${organizationId}/${subscriptionPlan}`
            , stripeCustomerData, {
            withCredentials: false,
            observe: 'body',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Credentials': 'true'
            })
        });
    }

    GetStripeSessionForUpdate() {
        return this.http.get(environment.endpointURL + `/api/User/UpdateStripeCart`
            , {
                withCredentials: true,
                observe: 'body',
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin': '*',
                    'Access-Control-Allow-Credentials': 'true'
                })
            });
    }

    public UpdateViewCount(Name: number) {
        this.http.get(environment.endpointURL + `/api/User/UpdatedVisited/${Name}`
            , {
                withCredentials: true,
                observe: 'body',
                headers: this._headers
            }).toPromise();
    }


    public GetViewCount(): Observable<any> {
        return this.http.get(environment.endpointURL + `/api/User/GetVisited`
            , {
                withCredentials: true,
                observe: 'body',
                headers: this._headers
            });
    }

    public DowngradeStripe(): Observable<any> {
        return this.http.get(environment.endpointURL + `/Api/User/StripeCart/Downgrade`
            , {
                withCredentials: true,
                observe: 'body',
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin': '*',
                    'Access-Control-Allow-Credentials': 'true'
                })
            });
    }


    public CancelStripe(feedBack: string): Observable<any> {
        return this.http.post(environment.endpointURL + `/Api/User/StripeCart/Cancel`
            , { feedBack: feedBack }, {
            withCredentials: true,
            observe: 'body',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Credentials': 'true'
            })
        });
    }

    public GetUserDetails(): Observable<any> {
        return this.http.get(environment.endpointURL + `/Api/User/Details`
            , {
                withCredentials: true,
                observe: 'body',
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin': '*',
                    'Access-Control-Allow-Credentials': 'true'
                })
            });
    }
    public GetPortalURL() {
        return this.http.get(environment.endpointURL + `/Api/stripe/GetPortalURL`
            , {
                withCredentials: true,
                observe: 'body',
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin': '*',
                    'Access-Control-Allow-Credentials': 'true'
                })
            });
    }
    public genericLog(log) {
        return this.http.post(environment.endpointURL + `/Api/ThriveCart/Log`
            , { Log: log }, {
            withCredentials: true,
            observe: 'body',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Credentials': 'true'
            })
        });
    }
}
