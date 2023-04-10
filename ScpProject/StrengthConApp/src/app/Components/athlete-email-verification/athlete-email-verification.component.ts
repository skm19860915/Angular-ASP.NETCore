import { Component, OnInit } from '@angular/core';
import { AthleteService } from '../../Services/athlete.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../Services/user.service';
import { AlertMessage } from '../../Models/AlertMessage';
import { interval } from 'rxjs';
import { take } from 'rxjs/operators';
@Component({
  selector: 'app-athlete-email-verification',
  templateUrl: './athlete-email-verification.component.html',
  styleUrls: ['./athlete-email-verification.component.less']
})
export class AthleteEmailVerificationComponent implements OnInit {

  public _athleteSerivce: AthleteService;
  public _route: ActivatedRoute;
  public userName: string;
  public password: string;
  public confirmPassword:string
  public isFormValidated : boolean = true;
  public AlertMessages: AlertMessage[] = [];
  constructor(private userService: UserService, athleteSerivce: AthleteService, private route: ActivatedRoute, public router: Router) {
    this._athleteSerivce = athleteSerivce;
    this._route = route;
  }

  ngOnInit() {

  }
  FinishRegistration(userName: string, password: string, confirmPassword: string) {
    var emailToken = this.route.snapshot.paramMap.get("emailToken")
    var athleteId = Number(this.route.snapshot.paramMap.get("athleteId"))

    this._athleteSerivce.FinishRegistration(userName, password, confirmPassword, emailToken, athleteId)
      .subscribe(success => {
        this.userService.Login(userName, password).subscribe(
          success => { this.router.navigate(['/AthleteHome/Workout']); })
      },
        error => {
          var errorMessage = error.error == undefined || typeof error.error ==  typeof {} ? error.error.ExceptionMessage : error.error;
          this.DisplayMessage("Verification UNSUCCESSFULL", errorMessage, true)
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
