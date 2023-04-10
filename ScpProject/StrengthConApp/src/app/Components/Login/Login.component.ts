import { Component, OnInit } from '@angular/core';
import { UserService } from '../../Services/user.service'
import { Router } from '@angular/router';
import { fadeInAnimation } from '../../animation/fadeIn';
import { OrganizationService } from '../../Services/organization.service';
import { environment } from '../../../environments/environment';
import { AlertMessage } from 'src/app/Models/AlertMessage';
import { take } from 'rxjs/operators';
import { interval } from 'rxjs';
import { StripeCustomerData } from '../stripe-data/StripeCustomerData';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less'],
  animations: [fadeInAnimation]
})
export class LoginComponent implements OnInit {
  public userName: string;
  public ForgotUserName: boolean = false;
  public ForgotPassword: boolean = false;
  public TargetUserEmail: string;
  public register: boolean = false;
  public stepOne: boolean = false;
  public stepTwo: boolean = false;
  public stepThree: boolean = false;
  public stepFour: boolean = false;
  public OrganizationId: number;
  public password: string;
  public confirmPassword: string;
  public lastName: string;
  public firstName: string;
  public subscriptionOption = 4;
  public SubscriptionPlan = 'Head Coach Yearly';
  public stripe_session: any;
  public organizationName: string;
  public DoesOrgAlreadyExist: boolean = false;
  public UserNameAlreadyExist: boolean = false;
  public AlertMessages: AlertMessage[] = [];
  public ShowStripeScreen: boolean;
  public subscriptionPrice: number;
  public validFirstEmail: boolean = false;
  public validSecondEmail: boolean = false;
  public email: string;
  public Processing: boolean = false;

  PreProcess: boolean;
  $: any;
  StripeAddress1: string = "";
  StripeAddress2: string = "";
  StripePhone: string;
  StripeZip: number;
  StripeCity: string;
  StripeState: string;
  StripeCountry: string;
  StripeCardNumber: string;
  ExpiryMonth: number;
  ExpiryYear: number;
  CVC: number;
  Coupon: string;
  statusfailed: boolean;
  public emailInUse: boolean;
  public accountEmail: string;
  constructor(private orgService: OrganizationService, private userService: UserService, public router: Router, ) { }

  ngOnInit() {
    if (this.userService.IsLoggedIn() != '' && this.userService.IsLoggedIn() != undefined) {
      if (this.userService.IsCoach())
            this.router.navigate(['/Home'])
    }
    else{
      this.router.navigate(['AthleteHome/Workout'])
    }
  }
  ToggleRegister() {
    this.register = !this.register;
    this.stepOne = true;
    this.organizationName = '';
    this.firstName = '';
    this.lastName = '';
    this.password = '';
    this.confirmPassword = '';
    this.TargetUserEmail = '';
    this.DoesOrgAlreadyExist = false;
    this.stepTwo = false;
    this.stepThree = false;
  }
  SetSubscriptionOption(option: number) {
    this.subscriptionOption = option;
    this.subscriptionPrice = 0;
    switch (option) {
      case 1:
        this.SubscriptionPlan = 'Trainer'
        this.subscriptionPrice = 12.99;
        break;
      case 2:
        this.SubscriptionPlan = 'Head Coach'
        this.subscriptionPrice = 49.99;
        break;
      case 3:
        this.SubscriptionPlan = 'Coach'
        this.subscriptionPrice = 24.99;
        break;
      case 4:
        this.SubscriptionPlan = 'Yearly Head Coach'
        this.subscriptionPrice = 499.99;
        break;
      default:
        break;
    }
  }

  ValidateFirstEmail(email: string) {
    this.validFirstEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(email)
    this.userService.EmailInUse(email).subscribe((success: boolean) => {
      this.emailInUse = success;
    });
  }
  formatCC(cardNumber: string) {
    this.StripeCardNumber = cardNumber.replace(/\W/gi, '').replace(/(.{4})/g, '$1 ');
  }

  DoesOrgExists(name: string) {

    this.orgService.CheckOrganizationExists(name).subscribe((success: boolean) => {
      this.DoesOrgAlreadyExist = success
    });
  }
  DoesUserNameExist(name: string) {
    this.userService.CheckUserNameExists(name).subscribe((success: boolean) => {
      this.UserNameAlreadyExist = success;
    });
  }

  CreateOrganization(name: string) {
    this.stepOne = false;
    this.stepTwo = true;
    this.stepThree = false;
    this.stepFour = false;
    this.organizationName = name;
  }

  SelectedPricePlan() {

    this.stepOne = false;
    this.stepTwo = false;
    this.stepThree = true;
    this.stepFour = false;
  }

  Register(userName: string, password: string, confirmPassword: string, email: string, firstName: string, lastName: string): void {
    this.orgService.RegisterOrganization(this.organizationName).subscribe(success => {
      this.Processing = true;
      this.OrganizationId = <number>success;
      this.userService.Register(userName, password, confirmPassword, email, firstName, lastName, this.OrganizationId)
        .subscribe(success => {
          //this.router.navigate(['/Home']);
          this.checkoutStripe(this.OrganizationId, this.subscriptionOption);
        }
        );
    });
  }

  checkoutStripe(organizationId: number, subscriptionOption: number) {

    this.userService.GetStripeSession(organizationId, subscriptionOption).subscribe(success => {
      this.stripe_session = success;
      var stripe = Stripe(environment.stripeKey);
      stripe.redirectToCheckout({ sessionId: this.stripe_session.id }).then(function (result) {
        //https://stripe.com/docs/js/checkout/redirect_to_checkout
        //per above , the then is only supposed to display an error message. I dont know what the contracter was thinking when he did save
        // this.userService.SaveStripeGuid(organizationId, result);

      });
    });
  }

  Login(userName: string, password: string, ): void {

    this.userService.Login(userName, password).subscribe(
      success => {
        if (this.userService.IsCoach()) {
          this.router.navigate(['/Home']);
        }
        else {
          this.router.navigate(['/AthleteHome/Workout'])
        }
      }
      , error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Login UNSUCCESSFULL', errorMessage, true)
      }
    );
  }
  CloseRecovery() {
    this.ForgotUserName = false;
    this.ForgotPassword = false;
  }
  UserNameRecovery() {
    this.ForgotUserName = true;
    this.ForgotPassword = false;

  }
  PasswordRecovery() {
    this.ForgotPassword = true;
    this.ForgotUserName = false;
  }
  GetUserName(email: string) {
    this.Processing = true;
    this.userService.RecoverUserName(email).subscribe(success => {
      this.Processing = false;
      this.DisplayMessage("Email Sent", "If this email exists, you will receive instructions on recovering your User Name", false)
    });
  }
  ResetPassword(email: string) {
    this.Processing = true;
    this.userService.ResetPassword(email).subscribe(success => {
      this.Processing = false;
      this.DisplayMessage("Email Sent", "If this email exists, you will receive instructions on resseting your Password", false)
    });

  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }

  public StripeClick() {
    this.Processing = true;
    this.orgService.RegisterOrganization(this.organizationName).subscribe(success => {
      this.OrganizationId = <number>success;
      this.userService.Register(this.userName, this.password, this.confirmPassword, this.email, this.firstName, this.lastName, this.OrganizationId)
        .subscribe(success => {
          this.userService.CreateStripeCustomer(this.OrganizationId, this.subscriptionOption, new StripeCustomerData(this.firstName,
            this.lastName,
            this.StripeAddress1,
            this.StripeAddress2,
            this.StripePhone,
            this.StripeZip,
            this.email,
            this.StripeCity,
            this.StripeState,
            this.StripeCountry,
            this.StripeCardNumber,
            this.ExpiryMonth,
            this.CVC, this.ExpiryYear, this.Coupon)).subscribe(response => {
              this.Processing = false;
              if (response.toString() == "true") {
                this.DisplayMessage('Registration Successful', 'Stripe Registration Successful', false);
                this.router.navigate(['/Home']);
              } else {
                this.statusfailed = true;
                this.DisplayMessage('Registration Failed', 'Stripe Registration Failed', true);
              }
            });
        }
        );
    });
  }
}
