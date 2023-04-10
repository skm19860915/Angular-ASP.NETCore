import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Tag } from "../Models/Tag";
import { Exercise } from '../Models/Exercise';
import { map, catchError } from 'rxjs/operators';
import { analyzeAndValidateNgModules } from '@angular/compiler';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class ExerciseService {

  private _headers;

  constructor(private http: HttpClient, private cookieService: CookieService) {
    this._headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true'
    });
  }


  CreateExercise(newExercise: Exercise): Observable<number> {
    return this.http.post<number>(environment.endpointURL + "/api/Exercises/CreateNewExercise"
      , JSON.stringify(newExercise), {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  
  DuplicateExercise(exerciseId: number): Observable<number> {
    return this.http.post<number>(environment.endpointURL + `/api/Exercises/DuplicateExercise/${exerciseId}`
      ,'', {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }

 HardDeleteExercise(exerciseId: number): any {
    return this.http.get(environment.endpointURL + `/api/Exercises/HardDelete/${exerciseId}`
      ,  {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  ArchiveExercise(exerciseId: number): any {
    return this.http.post<number>(environment.endpointURL + `/api/Exercises/ArchiveExercise/${exerciseId}`
      , '', {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  UnArchiveExercise(exerciseId: number): any {
    return this.http.post<number>(environment.endpointURL + `/api/Exercises/UnArchiveExercise/${exerciseId}`
      , '', {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
UpdateExercise(newExercise: Exercise): Observable<number> {
    return this.http.post<number>(environment.endpointURL + "/api/Exercises/UpdateExercise"
      , JSON.stringify(newExercise), {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  GetAllExercises(): Observable<Exercise[]> {

    return this.http.get<Exercise[]>(environment.endpointURL + "/api/Exercises/GetAllExercises/", {
      withCredentials: true,
      observe: 'body',
      headers:  this._headers});
  }
}
