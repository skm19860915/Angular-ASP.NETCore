import { Component, OnInit } from '@angular/core';
import { UserService } from '../../Services/user.service'
import { Router } from '@angular/router';
import { OrganizationService } from '../../Services/organization.service';
import { AlertMessage } from '../../Models/AlertMessage';
import { interval } from 'rxjs';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.less']
})
export class RegisterComponent implements OnInit {

  public userName: string;
  public email: string;
  public password: string;
  public confirmPassword: string;
  public lastName: string;
  public firstName: string;
  public registrationStep: number = 1;
  public OrganizationId: number;
  stripe_session: any;
  public AlertMessages: AlertMessage[] = [];
  constructor(private userService: UserService, private orgService: OrganizationService, public router: Router) { }

  ngOnInit() {
    this.registrationStep = 2;
  }
  ContinueWithouCreditCard() {

  }
  CreateOrganization(name: string) {
    this.orgService.RegisterOrganization(name).subscribe(success => {
      this.OrganizationId = <number>success;
      this.DisplayMessage("Organization Creation SUCCESSFULL","Organization Created ",false)

      this.registrationStep = 3;
    },
      error => {
        this.DisplayMessage("Organization Creation UNSUCCESSFULL",error.error,true)
      });
  }
  Register(userName: string, password: string, confirmPassword: string, email: string, firstName: string, lastName: string): void {
    this.userService.Register(userName, password, confirmPassword, email, firstName, lastName, this.OrganizationId)
      .subscribe(success => {
        this.router.navigate(['/StripeData']);
        //this.checkoutStripe(this.OrganizationId);
      }
        , error => {
          this.DisplayMessage("Registration UNSUCCESSFULL",error.error,true)
        }
      );
  }

  checkoutStripe(organizationId: number) {
    // this.userService.GetStripeSession(organizationId).subscribe(success => {
    //   this.stripe_session = success;
    //   var stripe = Stripe(environment.stripeKey);
    //   stripe.redirectToCheckout({
    //     sessionId: this.stripe_session.id
    //   }).then(function (result) {
    //     console.log(result);

    //     this.userService.SaveStripeGuid(organizationId, result);
    //   });
    // });
  }
  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
}
