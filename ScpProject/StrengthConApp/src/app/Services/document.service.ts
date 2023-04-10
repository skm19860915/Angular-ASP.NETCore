import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { subscribeOn, map } from 'rxjs/operators';
import { CookieService } from 'ngx-cookie-service';
import { Document } from '../Models/Document';


@Injectable({
    providedIn: 'root'
  })

  export class DocumentService {

    private _headers;
    constructor(private http: HttpClient, private cookieService: CookieService) {
        this._headers = this._headers = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Credentials': 'true'
            })
        }
    }

    CreateDocument(newDocument: Document){
        return this.http.post(environment.endpointURL + `/api/Document/CreateDocument`
        , newDocument, {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
    });   
    }
  }