import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CompletedMetric } from '../Models/Athlete/CompletedMetric';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { CompletedSet, CompletedSuperSet } from '../Models/Athlete/CompletedSet';
import { CompleteRegistration } from '../Models/Athlete/CompleteRegistration';
import { Athlete } from '../Models/Athlete';
import { CompletedMetricHistory } from '../Models/Metric/CompletedMetricHistory'
import { HistoricProgram } from '../Models/Program/HistoricProgram';

@Injectable({
  providedIn: 'root'
})
export class AthleteService {

  private _headers;

  constructor(private http: HttpClient, private cookieService: CookieService) {
    this._headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true'
    });
  }

  public FinishRegistration(userName: string, password: string, confirmPassword: string, emailValidationToken: string, athleteId: number) {
    var completedReg = new CompleteRegistration();
    completedReg.UserName = userName;
    completedReg.Password = password;
    completedReg.ConfirmPassword = confirmPassword;
    completedReg.EmailValidationToken = emailValidationToken;
    completedReg.AthleteId = athleteId;
    return this.http.post(environment.endpointURL + "/api/Athletes/FinishAthleteRegistration"
      , JSON.stringify(completedReg), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  public GetMetricHistory(metricId: number, athleteId: number): Observable<CompletedMetricHistory[]> {
    return this.http.get<CompletedMetricHistory[]>(environment.endpointURL + `/api/Athletes/GetMetricHistory/${metricId}/${athleteId}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  
  public GetAthleteProgramHistory(athleteId: number): Observable<CompletedMetricHistory[]> {
    return this.http.get<CompletedMetricHistory[]>(environment.endpointURL + `/api/Athletes/GetAthleteProgramHistory/${athleteId}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  public GetAthleteListOfCompletedMetrics(athleteId: number): Observable<CompletedMetricHistory[]> {
    return this.http.get<CompletedMetricHistory[]>(environment.endpointURL + `/api/Athletes/GetAthleteListOfCompletedMetrics/${athleteId}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  public UpdateMetric(id: number, value: number, completedDate: Date, isCompleted: boolean) {
    return this.http.post<number>(environment.endpointURL + `/api/Athletes/UpdateMetric/`
      , JSON.stringify({ Id: id, Value: value, CompletedDate: completedDate, IsCompleted: isCompleted }), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  public PrintWorkout(programId: number, athleteId: number) {
    return this.http.post<number>(environment.endpointURL + `/api/Athletes/PrintAthleteWorkout/${programId}/${athleteId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  public CompleteSet(set: CompletedSet) {
    return this.http.post<number>(environment.endpointURL + "/api/Athletes/AddCompletedSet"
      , JSON.stringify(set), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  public CompleteSuperSet(set: CompletedSuperSet) {
    return this.http.post<number>(environment.endpointURL + "/api/Athletes/AddCompletedSuperSet"
      , JSON.stringify(set), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  public DeleteAthlete(athleteId: number) {
    return this.http.post(environment.endpointURL + `/api/Athletes/ArchiveAthlete/${athleteId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }

  public UpdateAthlete(newAthlete: Athlete, newMetrics) {
    return this.http.post<number>(environment.endpointURL + "/api/Athletes/UpdateAthlete"
      , JSON.stringify({ Athlete: newAthlete, Metrics: newMetrics, AthleteTags: newAthlete.Tags }), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })

  }

  public AddCompletedMetric(value: number, metricId: number, programDayItemMetricId: number, week: number, athleteId: number, assignedProgramId: number): Observable<number> {
    var completedMetric = new CompletedMetric();
    completedMetric.Value = value;//the amount that the athlete entered
    completedMetric.MetricId = metricId;//the metricId of the metric theycompleted
    completedMetric.ProgramDayItemMetricId = programDayItemMetricId; // The programDayItem (metric) id, let me know if you have a question on this one.
    completedMetric.WeekId = week; //the week in which they compelted
    completedMetric.AthleteId = athleteId; //the athleteId
    completedMetric.AssignedProgramId = assignedProgramId; //THe ProgramId
    return this.http.post<number>(environment.endpointURL + "/api/Athletes/AddCompletedMetric"
      , JSON.stringify(completedMetric), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
}
