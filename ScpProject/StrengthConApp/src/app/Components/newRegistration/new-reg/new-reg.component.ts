import { Component, EventEmitter, OnInit } from '@angular/core';
import { ClientInfo } from '../../../Models/Registration/ClientInfo';
import { UserService } from '../../../Services/user.service';
import { environment } from '../../../../environments/environment';
import { OrganizationService } from '../../../Services/organization.service';
import { Router } from '@angular/router';
import { FinalClientRegInfo } from '../../../Models/Registration/FinalClientRegInfo';
import { AlertMessage } from '../../../Models/AlertMessage';
import { interval } from 'rxjs';
import { take } from 'rxjs/operators';
@Component({
  selector: 'app-new-reg',
  templateUrl: './new-reg.component.html',
  styleUrls: ['./new-reg.component.less']
})
export class NewRegComponent implements OnInit {

  constructor(public router: Router, public userService: UserService, public organizationService: OrganizationService) { }
  public alertChildProcessingFinished: EventEmitter<boolean> = new EventEmitter();
  public wizardState: number = 1;
  orgName: string = '';
  planId: number = 0;
  public clientInfo: ClientInfo = new ClientInfo();
  stripe_session: any;
  public AlertMessages: AlertMessage[] = [];
  ngOnInit() {

  }
  FinishRegistration(finalClientInfo: FinalClientRegInfo) {
    console.log(finalClientInfo)
    finalClientInfo.planId = this.planId;
    this.organizationService.RegisterOrganization(this.orgName).subscribe(orgId => {
      finalClientInfo.orgId = orgId;
    //  this.organizationService.StartStripeCustomerCreation(finalClientInfo).subscribe(x => {
        this.userService.Register(this.clientInfo.UserName, this.clientInfo.Password, this.clientInfo.ConfirmPassword, this.clientInfo.Email, this.clientInfo.FirstName, this.clientInfo.LastName, orgId).subscribe(x => {
          // this.userService.Login(this.clientInfo.UserName, this.clientInfo.Password).subscribe(z => {
          //   this.router.navigate(['/Home']);
      //    })
        });
      }, error =>{
        this.DisplayMessage("Registration Error", error.error.ExceptionMessage,true)
        this.alertChildProcessingFinished.emit(true);
      });
    //});
  }
  advanceWizardState(newState: number) {
    this.wizardState = newState;
  }
  advanceWizardStateByStep(newState: number) {
    if (this.wizardState < newState) {
      return;
    }
    this.wizardState = newState;
  }
  CollectOrganizationName(newOrgName: string) {
    this.orgName = newOrgName
  }
  CollectPlanId(planId: number) {
    this.planId = planId;
  }
  CollectClientInfo(newInf: ClientInfo) {
    this.clientInfo = newInf;
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage);
  }
}
