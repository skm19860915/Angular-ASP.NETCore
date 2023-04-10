import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { UserNotification } from '../Models/Notification/UserNotification';
import { environment } from '../../environments/environment';
import { DashboardNotification } from '../Models/Notification/DashboardNotification';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private _headers;
  constructor(private http: HttpClient, public router: Router) {
    this._headers = this._headers = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': '*',
        'Access-Control-Allow-Credentials': 'true'
      })
    }
  }

  public GetAllViewedNotifcations(): Observable<UserNotification[]> {
    return this.http.get<UserNotification[]>(environment.endpointURL + `/api/Notification/GetAllViewedNotifcations`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });

  }
  public HasUnreadNotifications(): Observable<boolean> {
    return this.http.get<boolean>(environment.endpointURL + `/api/Notification/HasUnreadNotifications`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  public GetAllNewNotifications(): Observable<UserNotification[]> {
    return this.http.get<UserNotification[]>(environment.endpointURL + `/api/Notification/GetAllNewNotifications`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  public GetAllPaginatedNewNotifications(pageNumber: number, notificationCount: number): Observable<DashboardNotification> {
    return this.http.get<DashboardNotification>(environment.endpointURL + `/api/Notification/GetAllPaginatedNewNotifications/${pageNumber}/${notificationCount}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  public MarkNotificationAsRead(notificationId: number) {
    return this.http.post(environment.endpointURL + `/api/Notification/MarkNotificationAsRead/${notificationId}`
      , {}, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });

  }
}
