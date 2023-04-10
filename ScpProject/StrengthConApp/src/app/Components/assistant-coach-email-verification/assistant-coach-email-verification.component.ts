import { Component, OnInit } from '@angular/core';
import { AthleteService } from '../../Services/athlete.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../Services/user.service';
import { RosterService } from '../../Services/roster.service';
import { AlertMessage } from '../../Models/AlertMessage';
import { interval } from 'rxjs';
import { take } from 'rxjs/operators';
@Component({
  selector: 'app-assistant-coach-email-verification',
  templateUrl: './assistant-coach-email-verification.component.html',
  styleUrls: ['./assistant-coach-email-verification.component.less']
})
export class AssistantCoachEmailVerificationComponent implements OnInit {

  public _athleteSerivce: AthleteService;
  public _route: ActivatedRoute;
  public userName: string;
  public password: string;
  public confirmPassword: string
  public AlertMessages: AlertMessage[] = [];
  public isFormValidated : boolean = true;
  constructor(public rosterService: RosterService, private userService: UserService, athleteSerivce: AthleteService, private route: ActivatedRoute, public router: Router) {
    this._athleteSerivce = athleteSerivce;
    this._route = route;
  }

  ngOnInit() {

  }
  FinishRegistration(userName: string, password: string, confirmPassword: string) {
    var emailToken = this.route.snapshot.paramMap.get("emailToken")
    var coachId = Number(this.route.snapshot.paramMap.get("coachId"))

    this.rosterService.FinishAssistantCoachRegistration(userName, password, confirmPassword, emailToken, coachId)
      .subscribe(success => {
        this.userService.Login(userName, password).subscribe(
          success => { this.router.navigate(['/Home']); })
      },
        error => {
          var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
          this.DisplayMessage("Verification UNSUCCESSFULL", errorMessage, true)
        });

  }
  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
  UserLoginValidator(userName: string)
  {
    if (userName.indexOf(' ') >= 0){
    this.DisplayMessage("User Name Cannot Contain Spaces", "The UserName Cannot Contain Spaces", true);
    this.isFormValidated = false;
    }
    else
    {
      this.isFormValidated = true;
    }
  }
}
