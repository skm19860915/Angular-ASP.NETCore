import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Tag } from "../Models/Tag";
import { Exercise } from '../Models/Exercise';
import { map, catchError } from 'rxjs/operators';
import { analyzeAndValidateNgModules } from '@angular/compiler';
import { Observable } from 'rxjs';
import { WorkoutDetails } from '../Models/SetsAndReps/WorkoutDetails';
import { Workout } from '../Models/SetsAndReps/Workout';

@Injectable({
  providedIn: 'root'
})
export class WorkoutService {
  private _headers;
  constructor(private http: HttpClient, private cookieService: CookieService) {
    this._headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true'
    });
  }
  GetWorkoutOutDetails(workoutId: number) {
    return this.http.get<WorkoutDetails>(environment.endpointURL + "/api/Workout/GetWorkoutDetails/" + workoutId, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  GetWorkouts() : Observable<Workout[]>{
    return this.http.get<Workout[]>(environment.endpointURL + "/api/Workout/GetWorkouts/", {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  CreateWorkout(newWorkout: WorkoutDetails) {
    return this.http.post<number>(environment.endpointURL + "/api/Workout/CreateNewWorkout"
      , JSON.stringify(newWorkout), {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  UpdateWorkout(updatedWorkout: WorkoutDetails) {
    return this.http.post<number>(environment.endpointURL + "/api/Workout/UpdateWorkout"
      , JSON.stringify(updatedWorkout), {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  DuplicateWorkout(workoutId: number): Observable<number> {
    return this.http.post<number>(environment.endpointURL + `/api/Workout/DuplicateWorkout/${workoutId}`
      ,'', {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  
  ArchiveWorkout(workoutId: number): any {
    return this.http.post<number>(environment.endpointURL + `/api/Workout/ArchiveWorkout/${workoutId}`
      , '', {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  UnArchiveWorkout(workoutId: number): any {
    return this.http.post<number>(environment.endpointURL + `/api/Workout/UnArchiveWorkout/${workoutId}`
      , '', {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
}
