import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Athlete } from '../Models/Athlete';
import { AssignProgram } from '../Models/AssignProgram';
import { AssistantCoach } from '../Models/AssistantCoach';
import { CompleteRegistration, CompleteCoachRegistration, OneTime } from '../Models/Athlete/CompleteRegistration';
import { AssignedAthleteCheck } from '../Models/Athlete/AssignedAthleteCheck';
import { DashboardAthleteWithoutProgarm } from '../Models/Athlete/DashboardAthleteWithoutProgram';


@Injectable({
  providedIn: 'root'
})
export class RosterService {

  private _headers;

  constructor(private http: HttpClient, private cookieService: CookieService) {
    this._headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true'
    });
  }

  ResendAthleteRegistartion(athleteId:number)
  {
    return this.http.post<number>(environment.endpointURL + `/api/Roster/ResendAthleteRegistartion/${athleteId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  CreateAssistantCoach(newCoach: AssistantCoach) {
    return this.http.post<number>(environment.endpointURL + "/api/Roster/AddAssistantCoach"
      , JSON.stringify(newCoach), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  CreateAthlete(newAthlete: Athlete, newMetrics): Observable<number> {
    return this.http.post<number>(environment.endpointURL + "/api/Roster/AddAthlete"
      , JSON.stringify({ Athlete: newAthlete, Metrics: newMetrics, AthleteTags: newAthlete.Tags }), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  CheckAtheletesForAssignedPrograms(athleteIds: number[]): Observable<AssignedAthleteCheck[]> {
    return this.http.post<AssignedAthleteCheck[]>(environment.endpointURL + "/api/Roster/CheckAssignedProgram"
      , JSON.stringify({ AthleteIdsToCheck: athleteIds }), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  AssignProgramToAthletes(assignDTO: AssignProgram) {
    return this.http.post<number>(environment.endpointURL + "/api/Roster/AssignProgram"
      , JSON.stringify(assignDTO), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }

  GetAllAthletes(): Observable<Athlete[]> {

    return this.http.get<Athlete[]>(environment.endpointURL + "/api/Roster/GetAllAthletes/", {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  GetAllAthletesWithoutProgram(pageNumber: number, count: number): Observable<DashboardAthleteWithoutProgarm> {

    return this.http.get<DashboardAthleteWithoutProgarm>(environment.endpointURL + `/api/Roster/GetAllAthletesWithoutProgram/${pageNumber}/${count}`, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  GetAthlete(athleteId: number): Observable<Athlete> {
    return this.http.get<Athlete>(environment.endpointURL + "/api/Roster/GetAthlete/" + athleteId, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  GetLoggedInAthlete(): Observable<Athlete> {
    return this.http.get<Athlete>(environment.endpointURL + "/api/User/GetLoggedInAthlete/", {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  public OneTimeReg(userName: string, password: string, confirmPassword: string, emailValidationToken: string, userId: number, orgName: string) {
    var completedReg = new OneTime();
    completedReg.UserName = userName;
    completedReg.Password = password;
    completedReg.ConfirmPassword = confirmPassword;
    completedReg.EmailValidationToken = emailValidationToken;
    completedReg.UserId = userId;
    completedReg.OrgName = orgName;
    return this.http.post(environment.endpointURL + "/api/User/OneTimeRegisterHeadCoach"
      , JSON.stringify(completedReg), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }

  public ResendCoachRegistrationEmail(coachId : number)
  {

    return this.http.post(`${environment.endpointURL}/api/Roster/ResendCoachEmail/${coachId}`
    ,null, {
    withCredentials: true,
    observe: 'body',
    headers: this._headers
  })
  }
  public FinishAssistantCoachRegistration(userName: string, password: string, confirmPassword: string, emailValidationToken: string, userId: number) {
    {
      var completedReg = new CompleteCoachRegistration();
      completedReg.UserName = userName;
      completedReg.Password = password;
      completedReg.ConfirmPassword = confirmPassword;
      completedReg.EmailValidationToken = emailValidationToken;
      completedReg.UserId = userId;
      return this.http.post(environment.endpointURL + "/api/Roster/FinishAssistantCoachRegistration"
        , JSON.stringify(completedReg), {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
    }
  }
}