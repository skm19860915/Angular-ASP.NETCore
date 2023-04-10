import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { LoggedInData } from '../Models/LoggedInData';
import { CookieService } from 'ngx-cookie-service';
import { subscribeOn, map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { Role, BackendAssistantCoach } from '../Models/AssistantCoach';
import { CompleteRegistration } from '../Models/Athlete/CompleteRegistration';
import { AthleteCount } from '../Models/Organization/AthleteCount';
import { SubscriptionInfo } from '../Models/Organization/SubscriptionInfo';
import { Organization } from '../Models/Organization/Organization';
import { FinalClientRegInfo } from '../Models/Registration/FinalClientRegInfo';
import { OrganizationVM } from '../Models/Organization/OrganizationVM';
@Injectable({
  providedIn: 'root'
})
export class OrganizationService {

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
  public UpdateOrganization(newInfo: Organization) {
    return this.http.post(environment.endpointURL + `/api/Organization/UpdateOrganization`
      , newInfo, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  public CheckOrganizationExists(name: string) {
    return this.http.post(environment.endpointURL + `/api/Organization/CheckOrganizationExists`
      , { Name: name }, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  public GetLoggedInOrg() {
    return this.http.get<Organization>(environment.endpointURL + `/api/Organization/GetLoggedInUsersOrg`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  public GetOrg(orgId: number) {
    return this.http.get<OrganizationVM>(environment.endpointURL + `/api/Organization/GetOrg/${orgId}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  public GetCurrentAthleteCountAndMax() {
    return this.http.get<AthleteCount>(environment.endpointURL + `/api/Organization/GetAthleteCount`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  public GetOrganizationSubscription() {
    return this.http.get<SubscriptionInfo>(environment.endpointURL + `/api/Organization/GetSubscriptionInfo`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  public UpgradeSubscription() {
    return this.http.post(environment.endpointURL + `/api/Organization/UpgradeOrganizationSubscription/`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  public ManualOrganizationSubscription(planId: number) {
    return this.http.post(environment.endpointURL + `/api/Organization/ManualOrganizationSubscription/${planId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  public ArchiveCoach(coachId: number) {
    return this.http.post(environment.endpointURL + `/api/Organization/ArchiveCoach/${coachId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }

  RegisterOrganization(newName: string) {
    return this.http.post<number>(environment.endpointURL + `/api/Organization/CreateOrganization/`
      , { name: newName }, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  DeleteOrganization(name: string) {
    return this.http.post(environment.endpointURL + `/api/Organization/DeleteOrganization/${name}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  GetAllRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(environment.endpointURL + `/api/Organization/GetAllRoles`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  GetAllCoaches(): Observable<BackendAssistantCoach[]> {
    return this.http.post<BackendAssistantCoach[]>(environment.endpointURL + `/api/Organization/GetAllCoaches`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  GetAllNonHeadCoaches(): Observable<BackendAssistantCoach[]> {
    return this.http.post<BackendAssistantCoach[]>(environment.endpointURL + `/api/Organization/GetAllNonHeadCoaches`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  GetAllCoachesButSelf(): Observable<BackendAssistantCoach[]> {
    return this.http.post<BackendAssistantCoach[]>(environment.endpointURL + `/api/Organization/GetAllCoachesButSelf`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  UnAssignRole(roleId: number, coachId: number) {
    return this.http.post<BackendAssistantCoach[]>(environment.endpointURL + `/api/Organization/UnAssignRoles`
      , { UserId: coachId, RoleId: roleId }, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  AssignRole(roleId: number, coachId: number) {
    return this.http.post<BackendAssistantCoach[]>(environment.endpointURL + `/api/Organization/AssignRoles`
      , { UserId: coachId, RoleId: roleId }, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  StartStripeCustomerCreation(clientInfo: FinalClientRegInfo) {
    return this.http.post(environment.endpointURL + `/api/stripe/CreateCustomer`
      ,clientInfo, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  CreateNewStripeCustomerCreation(clientInfo: FinalClientRegInfo) {
    return this.http.post(environment.endpointURL + `/api/stripe/FixBadCustomerByRecreating`
      ,clientInfo, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
}
