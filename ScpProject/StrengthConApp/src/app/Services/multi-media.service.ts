import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Movie } from '../Models/Movie';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MultiMediaService {


  private _headers;

  constructor(private http: HttpClient, private cookieService: CookieService) {
    this._headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true'
    });

  }

  CreateMovie(newMovie: Movie): Observable<number> {
    return this.http.post<number>(environment.endpointURL + "/api/MultiMedia/CreateMovie"
      , JSON.stringify(newMovie), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }

  HardDeleteMovie(movieId: number): any {
    return this.http.get(environment.endpointURL + `/api/MultiMedia/HardDelete/${movieId}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      })
  }
  ArchiveMovie(movieId: number): any {
    return this.http.post<number>(environment.endpointURL + `/api/MultiMedia/ArchiveMovie/${movieId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  UnArchiveMovie(movieId: number): any {
    return this.http.post<number>(environment.endpointURL + `/api/MultiMedia/UnArchiveMovie/${movieId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  UpdateMovie(newMovie: Movie): Observable<number> {
    return this.http.post<number>(environment.endpointURL + "/api/MultiMedia/UpdateMovie"
      , JSON.stringify(newMovie), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  GetAllMovies(): Observable<Movie[]> {

    return this.http.get<Movie[]>(environment.endpointURL + "/api/MultiMedia/GetAllMovies", {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
}
