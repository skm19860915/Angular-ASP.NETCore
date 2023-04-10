import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { AlertMessage } from '../../../Models/AlertMessage';
import { ClientInfo } from '../../../Models/Registration/ClientInfo';
import { UserService } from '../../../Services/user.service';

@Component({
  selector: 'app-step3',
  templateUrl: './step3.component.html',
  styleUrls: ['./step3.component.less']
})
export class Step3Component implements OnInit {

  constructor(private router: Router, private userService: UserService) { }
  @Input() wizardState: number;
  @Output() wizardStateChange: EventEmitter<number> = new EventEmitter(true);
  @Output() clientInfoChange: EventEmitter<ClientInfo> = new EventEmitter(true);
  public UserNameAlreadyExist: boolean = false
  public isFormValidated : boolean = true;
  public AlertMessages: AlertMessage[] = [];
  public newInfo: ClientInfo = new ClientInfo();
  public emailInUse: boolean = false;
  public validFirstEmail: boolean = false;
  ngOnInit() {
  }
  Cancel() {
    this.wizardStateChange.emit(1);
    this.router.navigate(['/Login']);
  }
  DoesUserNameExist(name: string) {
    this.userService.CheckUserNameExists(name).subscribe((success: boolean) => {
      this.UserNameAlreadyExist = success;
    });
    if (name.indexOf(' ') >= 0){
      this.DisplayMessage("User Name Cannot Contain Spaces", "The UserName Cannot Contain Spaces", true);
      this.isFormValidated = false;
      }
      else
      {
        this.isFormValidated = true;
      }
  }

  ValidateFirstEmail(email: string) {
    this.validFirstEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(email)
    this.userService.EmailInUse(email).subscribe((success: boolean) => {
      this.emailInUse = success;
    });
  }
  UserInfoFilledOut() {
    this.wizardStateChange.emit(++this.wizardState);
    this.clientInfoChange.emit(this.newInfo)
    //   this.orgService.RegisterOrganization(name).subscribe(success => {
    //     //  this.OrganizationId = <number>success;
    //   },
    //     error => {
    //     });
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
}
