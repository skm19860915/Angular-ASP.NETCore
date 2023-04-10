import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Tag, TagType } from "../Models/Tag";
import { Exercise } from '../Models/Exercise';
import { map, catchError } from 'rxjs/operators';
import { analyzeAndValidateNgModules } from '@angular/compiler';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class TagService {

  private _headers;
  private _allTags: Tag[];
  private _hackHeaders;

  constructor(private http: HttpClient, private cookieService: CookieService) {
    this._hackHeaders = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true'
    });
    this._headers = this._headers = {
      withCredentials: true,
      observe: 'body',
      headers: this._hackHeaders
    };
    this._allTags = [];
  }

  GetAllTags(tagType: TagType): Observable<Tag[]> {

    return this.http.get<Tag[]>(environment.endpointURL + "/api/Tag/GetAllTags/" + tagType, {
      withCredentials: true,
      observe: 'body',
      headers: this._hackHeaders
    });

  }

  CreateTag(newCategory: Tag): Observable<number> {
    return this.http.post<number>(environment.endpointURL + "/api/Tag/CreateTag"
      , JSON.stringify(newCategory), {
        withCredentials: true,
        observe: 'body',
        headers:  this._hackHeaders});
  }

}
