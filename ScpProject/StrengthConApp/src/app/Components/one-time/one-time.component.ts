import { Component, OnInit } from '@angular/core';
import { AthleteService } from '../../Services/athlete.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../Services/user.service';
import { RosterService } from '../../Services/roster.service';
import { AlertMessage } from '../../Models/AlertMessage';
import { interval } from 'rxjs';
import { take } from 'rxjs/operators';
@Component({
  selector: 'app-one-time',
  templateUrl: './one-time.component.html',
  styleUrls: ['./one-time.component.less']
})
export class OneTimeComponent implements OnInit {

  public _athleteSerivce: AthleteService;
  public _route: ActivatedRoute;
  public userName: string;
  public password: string;
  public confirmPassword: string
  public organizationName: string
  public AlertMessages: AlertMessage[] = [];
  constructor(public rosterService: RosterService, private userService: UserService, athleteSerivce: AthleteService, private route: ActivatedRoute, public router: Router) {
    this._athleteSerivce = athleteSerivce;
    this._route = route;
  }

  ngOnInit() {

  }
  FinishRegistration(userName: string, password: string, confirmPassword: string, orgName: string) {
    var emailToken = this.route.snapshot.paramMap.get("emailToken")
    var coachId = Number(this.route.snapshot.paramMap.get("coachId"))

    this.rosterService.OneTimeReg(userName, password, confirmPassword, emailToken, coachId, orgName)
      .subscribe(success => {
        this.userService.Login(userName, password).subscribe(
          success => { this.router.navigate(['/Home']); })
      },
        error => {
          var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
          this.DisplayMessage("Could Not FInish Registration", errorMessage, true)
        });

  }
  DisplayMessage(title: string, message: string, isError: boolean) {
    var newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
    interval(3000).pipe(take(1)).subscribe(x => this.AlertMessages.splice(0, 1));
  }
}
