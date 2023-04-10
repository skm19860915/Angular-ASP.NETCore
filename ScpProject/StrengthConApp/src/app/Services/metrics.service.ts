import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Tag } from "../Models/Tag";
import { Exercise } from '../Models/Exercise';
import { map, catchError } from 'rxjs/operators';
import { analyzeAndValidateNgModules } from '@angular/compiler';
import { Observable } from 'rxjs';
import { Metric } from '../Models/Metric/Metric';
import { UnitOfMeasurement } from '../Models/UnitOfMeasurement';
import { HistoricProgram } from '../Models/Program/HistoricProgram';
import { MeasuredMetric, GroupedMeasuredMetric } from '../Models/Metric/MeasuredMetric';


@Injectable({
  providedIn: 'root'
})
export class MetricsService {

  private _headers;

  constructor(private http: HttpClient, private cookieService: CookieService) {
    this._headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true'
    });
  }


  CreateMetric(newMet: Metric): Observable<number> {
    return this.http.post<number>(environment.endpointURL + "/api/Metric/AddMetric/",
      JSON.stringify(newMet)
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  UpdateMetric(updatedMet: Metric): Observable<number> {
    return this.http.post<number>(environment.endpointURL + "/api/Metric/UpdateMetric/",
      JSON.stringify(updatedMet)
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  CreateUnitOfMeasurement(unitType: string): Observable<number> {
    return this.http.post<number>(environment.endpointURL + "/api/Metric/AddMeasurement/", JSON.stringify({ unitType: unitType })
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  GetAllMetrics(): Observable<Metric[]> {

    return this.http.get<Metric[]>(environment.endpointURL + "/api/Metric/GetMetrics/", {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  GetAllMeasurements(): Observable<UnitOfMeasurement[]> {

    return this.http.get<UnitOfMeasurement[]>(environment.endpointURL + "/api/Metric/GetAllMeasurements/", {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  GetAllHistoricMetric(athleteId: number): Observable<HistoricProgram[]> {
    return this.http.get<HistoricProgram[]>(environment.endpointURL + "/api/Metric/GetHistoricProgramsWithMetrics/" + athleteId, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  GetAllMeasuredMetrics(athleteId: number): Observable<GroupedMeasuredMetric> {
    return this.http.get<GroupedMeasuredMetric>(environment.endpointURL + "/api/Metric/GetAllMeasuredMetrics/" + athleteId, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  DuplicateMetric(metricId: number): Observable<number> {
    return this.http.post<number>(environment.endpointURL + `/api/Metric/DuplicateMetric/${metricId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  HardDeleteMetric(metricId: number): any {
    return this.http.get(environment.endpointURL + `/api/Metric/HardDelete/${metricId}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  ArchiveMetric(metricId: number): any {
    return this.http.post<number>(environment.endpointURL + `/api/Metric/ArchiveMetric/${metricId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  UnArchiveMetric(metricId: number): any {
    return this.http.post<number>(environment.endpointURL + `/api/Metric/UnArchiveMetric/${metricId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
}
