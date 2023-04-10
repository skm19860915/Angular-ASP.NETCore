import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core'
import { UserService } from '../Services/user.service'
import { Router } from '@angular/router';

@Injectable()
export class CustomerValidatorInterceptor implements HttpInterceptor {

    constructor(public userService: UserService, public route: Router) { }
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request)
            .pipe(
                retry(1),
                catchError((error: HttpErrorResponse) => {
                    if (error.error instanceof ErrorEvent) {
                        // client-side error
                        return throwError(error);
                    } else {
                        // server-side error
                        if (error.error.ExceptionType === "BL.CustomExceptions.InvalidCustomerException")
                        if (this.route.url.toLocaleLowerCase().includes('/AccountSettings')) {
                            return;
                        } else {
                            this.route.navigate(['AccountSettings']);
                        }
                        return throwError(error)
                    }

                })
            )
    }
}

