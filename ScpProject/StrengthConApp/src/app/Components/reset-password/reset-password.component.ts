import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../Services/user.service';
import { AlertMessage } from '../../Models/AlertMessage';
import { interval } from 'rxjs';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.less']
})
export class ResetPasswordComponent implements OnInit {

  public _route: ActivatedRoute;
  public password: string = '';
  public confirmPassword: string = '';
  public AlertMessages: AlertMessage[] = [];
  constructor(private route: ActivatedRoute, public userService: UserService, public router: Router) {
    this._route = route;
  }

  ngOnInit() {
  }
  ResetPassword(password: string, confirmPassword: string) {
    var validationToken = this.route.snapshot.paramMap.get("emailToken")
    var userId = Number(this.route.snapshot.paramMap.get("userId"))

    this.userService.FinishPasswordReset(password, confirmPassword, validationToken, userId).subscribe(
      success => {
        this.DisplayMessage("PASSWORD RESET SUCCESFULL", "Password reset", false)
        this.router.navigate(['/Login'])
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("PASSWORD RESET UNSUCCESFULL", errorMessage, true)
      });
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
}
