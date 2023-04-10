//@ts-ignore
/// <reference types="@types/googlemaps" />
import { Component, ViewChild,  OnInit, AfterViewInit,  ElementRef } from '@angular/core';
import { UserService } from "../../Services/user.service";
import { environment } from "../../../environments/environment";
import { Router } from "@angular/router";
import { AlertMessage } from "../../Models/AlertMessage";
import { Observable, of, interval } from "rxjs";
import { take } from "rxjs/operators";
import { fadeInAnimation } from "../../animation/fadeIn";
import { OrganizationService } from "../../Services/organization.service";
import { SubscriptionInfo } from "../../Models/Organization/SubscriptionInfo";
import { AthleteCount } from "../../Models/Organization/AthleteCount";
import { FileUploader } from 'ng2-file-upload';
import { Organization } from '../../Models/Organization/Organization';
import { AccountSettings } from '../../Models/AccountSettings/AccountSettings';
import { OrganizationVM } from "../../Models/Organization/OrganizationVM";
import { ClientInfo } from "../../Models/Registration/ClientInfo";
import { FinalClientRegInfo } from "../../Models/Registration/FinalClientRegInfo";
@Component({
  selector: "app-account-settings",
  templateUrl: "./account-settings.component.html",
  styleUrls: ["./account-settings.component.less"],
  animations: [fadeInAnimation],
})
export class AccountSettingsComponent implements OnInit, AfterViewInit {
  public userService: UserService;
  public ShowChangePassword: boolean = false;
  FirstName: string;
  LastName: string;
  Birthday: Date;
  Email: string;
  Tag: string;
  UserName: string;
  Password: string;
  ConfirmPassword: string;
  stripe_session: any;
  processDowngrade: boolean;
  deleteSubscription: boolean;
  public subscriptionOption = 4;
  public SubscriptionPlan = "Head Coach Yearly";
  public AlertMessages: AlertMessage[] = [];
  public confirmDeleteSubscription: boolean = false;
  public deleteConfirmation: string;
  public feedback: string;
  public UserIsACustomer: boolean = true;
  public subscriptionPrice: number;
  public athleteCount: AthleteCount = { TotalAthletes: 0, MaxAthletes: 0 };
  public subPlan: SubscriptionInfo = new SubscriptionInfo();
  public OrgId: number = 0;
  public ChangeSubscriptionModal: boolean = false;
  public UpgradeProccessing: boolean = false;
  public IsCoach: boolean = false;
  public IsHeadCoach: boolean = false;
  public OrganizationName: string = '';
  public UpdateCreditCardProcessing: boolean = false;
  public primaryColor: string;
  public secondaryColor: string;
  public secondaryFontColor: string
  public primaryFontColor: string
  public OrganizationHasExpiredCard: boolean = false;
  public StripeFailedToProcessOrganizationCard: boolean = false;
  public HasBadCreditCard: boolean = false;
  public IsSubscriptionEnded: boolean = false;
  public HasCreditCardExpiring: boolean = false;
  public OrgProfileURL : string = undefined;
  public newInfo: ClientInfo = new ClientInfo();
  public clientRegInfo: FinalClientRegInfo = new FinalClientRegInfo();
  public autocompleteInput: string = '';
  public ProcessingUpdate : boolean = false;

  @ViewChild('addresstext') addresstext: any;
  @ViewChild('ccNumber') ccNumberField: ElementRef;

  public uploadURL: string = environment.endpointURL + '/api/MultiMedia/CreateOrganizationImage';
  public uploader: FileUploader = new FileUploader({
    url: this.uploadURL,
    disableMultipart: false,
    headers:
      [
        {
          name: 'Access-Control-Allow-Origin',
          value: '*'
        }, {
          name: 'Access-Control-Allow-Credentials',
          value: 'true'
        }]
  });//todo: Bad hack to get this to work.
  constructor(public uService: UserService, private router: Router, private orgService: OrganizationService) {
    this.userService = uService;
  }


  RedirectToPortal() {
    this.userService.GetPortalURL().subscribe((success: string) => {
      window.open(
        success,
        '_blank'
      );
    });
  }

  ngAfterViewInit() {
    this.getPlaceAutocomplete();
  }


  ngOnInit() {

    this.userService.GetUserDetails().subscribe(x => {
      this.FirstName = x.FirstName;
      this.LastName = x.LastName;
      this.FirstName = x.FirstName;
      this.Email = x.Email;
      this.UserName = x.UserName;
      this.OrgId = x.OrganizationId;
      this.IsCoach = x.IsCoach;
      this.IsHeadCoach = x.IsHeadCoach;

      if (x.IsCoach) {
        this.GetTotalAthleteCount();
        //check if they are a valid customer if not lets put them in the create customer workflow

        this.orgService.GetOrg(x.OrganizationId).subscribe((x: OrganizationVM) => {

          this.UserIsACustomer = x.Org.IsCustomer;
          this.HasBadCreditCard = x.Org.StripeFailedToProcess || x.Org.BadCreditCard;
          this.IsSubscriptionEnded = x.Org.SubscriptionEnded;
          this.HasCreditCardExpiring = x.Org.CreditCardExpiring;
          this.OrganizationName = x.Org.Name;
          this.primaryColor = x.Org.PrimaryColorHex;
          this.secondaryColor = x.Org.SecondaryColorHex;
          this.primaryFontColor = x.Org.PrimaryFontColorHex;
          this.secondaryFontColor = x.Org.SecondaryFontColorHex;
          this.SetSubscriptionOption(x.Org.CurrentSubscriptionPlanId);
          this.OrganizationHasExpiredCard = x.Org.ExpiredCard;
          this.OrgProfileURL = x.profilePictureURL;
        })
      }
    });
  }

  ClearUploaderQueue(uploader) {
    uploader.queue.pop();
  }
  ForceUploaderQueueToBeJustOne(uploader) {
    if (uploader.queue.length > 1) {
      uploader.queue[0] = uploader.queue[1];
      uploader.queue.pop();
    }
  }
  GetTotalAthleteCount() {
    this.orgService.GetCurrentAthleteCountAndMax().subscribe((x) => {
      this.athleteCount = x;
      this.orgService.GetOrganizationSubscription().subscribe((x) => {
        this.subPlan = x;
      });
    });
  }
  checkoutStripeUpdate() {
    this.userService.GetStripeSessionForUpdate().subscribe((success) => {
      this.stripe_session = success;
      var stripe = Stripe(environment.stripeKey);
      stripe
        .redirectToCheckout({ sessionId: this.stripe_session.id })
        .then(function (result) {
          this.UpdateCreditCardProcessing = false;
        });
    });
  }

  public ToggleDeleteConfirmation() {
    this.confirmDeleteSubscription = !this.confirmDeleteSubscription;
  }
  public UpdateSubscription() {
    this.UpdateCreditCardProcessing = true;
    this.checkoutStripeUpdate();
  }
  public ChangeSubscription() {
    this.processDowngrade = true;
    this.userService.DowngradeStripe().subscribe((response) => {
      this.processDowngrade = false;
      this.DisplayMessage(
        "Account Updated Successfully",
        "Account Updated Successfully",
        false
      );
    });
  }
  public ToggleChangeSubscriptionModalVisible() {
    this.ChangeSubscriptionModal = !this.ChangeSubscriptionModal;
  }

  public CancelSubscription(feedBack: string) {
    this.deleteSubscription = true;
    this.userService.CancelStripe(feedBack).subscribe((response) => {
      this.deleteSubscription = false;
      this.DisplayMessage(
        "Account Deleted",
        "We will keep your data for two months incase you want to come back",
        true
      );
      interval(3000)
        .pipe(take(1))
        .subscribe((x) => this.userService.Logout());
    });
  }
  public Save() {

    if (this.IsHeadCoach) {
      let newOrgInfo: Organization = new Organization();
      newOrgInfo.Name = this.OrganizationName;
      newOrgInfo.PrimaryColorHex = this.primaryColor;
      newOrgInfo.SecondaryColorHex = this.secondaryColor;
      newOrgInfo.PrimaryFontColorHex = this.primaryFontColor;
      newOrgInfo.SecondaryFontColorHex = this.secondaryFontColor;
      this.orgService.UpdateOrganization(newOrgInfo).subscribe(x => {
        this.DisplayMessage("Organization Updated", "Organization Updated Successfully, Some settings will require you to Logout and Login to view the changes", false);
      }, error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Organization NOT Updated", errorMessage, true);
      });

      if (this.uploader != undefined && this.uploader.queue[0] != undefined) {
        this.uploader.queue[0].upload();
      }

      this.uploader.queue.pop();
    }

    let newAccountSettings: AccountSettings = { FirstName: this.FirstName, LastName: this.LastName, Email: this.Email };
    this.userService.UpdateUser(newAccountSettings).subscribe(x => {
      this.DisplayMessage("Account Updated", "Account Updated Successfully", false);
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage("Account NOT Updated", errorMessage, true);
    });
  }
  public Cancel() {
    this.router.navigateByUrl("Home");
  }
  public Feedback() {
    window.location.href = "https://strength-coach-pro.malcolm.app/";
  }
  CreateCustomer() {
    this.checkoutStripe(this.OrgId, this.subscriptionOption);
  }

  checkoutStripe(organizationId: number, subscriptionOption: number) {
    this.userService
      .GetStripeSession(organizationId, subscriptionOption)
      .subscribe((success) => {
        this.stripe_session = success;
        var stripe = Stripe(environment.stripeKey);
        stripe
          .redirectToCheckout({ sessionId: this.stripe_session.id })
          .then(function (result) {});
      });
  }

  SetSubscriptionOption(option: number) {
    this.subscriptionOption = option;
    this.subscriptionPrice = 0;

    switch (option) {
      case 2:
        this.SubscriptionPlan = "Head Coach";
        this.subscriptionPrice = 49.99;
        break;
      case 4:
        this.SubscriptionPlan = "Yearly Head Coach";
        this.subscriptionPrice = 499.99;
        break;
      default:
        break;
    }
  }

  // Register(userName: string, password: string, confirmPassword: string, email: string, firstName: string, lastName: string): void {
  //   this.orgService.RegisterOrganization(this.organizationName).subscribe(success => {
  //     this.Processing = true;
  //     this.OrganizationId = <number>success;
  //     this.userService.Register(userName, password, confirmPassword, email, firstName, lastName, this.OrganizationId)
  //       .subscribe(success => {
  //         //this.router.navigate(['/Home']);
  //         this.checkoutStripe(this.OrganizationId, this.subscriptionOption);
  //       }
  //       );
  //   });
  // }

  ToggleChangePassword() {
    this.ShowChangePassword = !this.ShowChangePassword;
  }

  UpgradeSubscription(planId: number) {
    this.UpgradeProccessing = true;
    this.orgService.ManualOrganizationSubscription(planId).subscribe(
      (success) => {
        this.DisplayMessage("Upgrade Successful", "Upgrade successful", false);
        this.orgService.GetOrganizationSubscription().subscribe((x) => {
          this.subPlan = x;
        });
        this.ToggleChangeSubscriptionModalVisible();

        this.UpgradeProccessing = false;
      },
      (error) => {
        var errorMessage =
          error.error == undefined || typeof error.error == typeof {}
            ? error.error.ExceptionMessage
            : error.error;
        this.DisplayMessage("Upgrade Unsuccesfull", errorMessage, true);
        this.UpgradeProccessing = false;
      }
    );
  }
  UpdatePassword(password: string, confirmPassword: string) {
    this.userService.UpdatePassword(password, confirmPassword).subscribe(x => {
      this.DisplayMessage("Password Updated", "Password Updated Successfully", false);
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage("Password Updated NOT Updated", errorMessage, true);
    });

  }

  FinishRegistration() {
    this.ProcessingUpdate = true;
   this.clientRegInfo.planId = this.subscriptionOption;
    this.clientRegInfo.orgId =this.OrgId;
      this.orgService.CreateNewStripeCustomerCreation(this.clientRegInfo).subscribe(x => {
        this.UserIsACustomer = true;
        this.ProcessingUpdate = false;
      }, error =>{
        this.DisplayMessage("Registration Error", error.error.ExceptionMessage,true);
        this.ProcessingUpdate = false;
      });
  }

  private getPlaceAutocomplete() {
    //@ts-ignore
    const autocomplete = new google.maps.places.Autocomplete(this.addresstext.nativeElement,
      {
      });
    //@ts-ignore
    google.maps.event.addListener(autocomplete, 'place_changed', () => {
      const place = autocomplete.getPlace();
      this.clientRegInfo.State = this.getState(place);
      this.clientRegInfo.PostalCode = this.getPostCode(place);
      this.clientRegInfo.Addr1 = this.getStreetNumber(place) + ' ' + this.getStreet(place);
      this.clientRegInfo.Country = this.getCountryShort(place);
      this.clientRegInfo.City = this.getCity(place);
    });
  }

  getAddrComponent(place, componentTemplate) {
    let result;

    for (let i = 0; i < place.address_components.length; i++) {
      const addressType = place.address_components[i].types[0];
      if (componentTemplate[addressType]) {
        result = place.address_components[i][componentTemplate[addressType]];
        return result;
      }
    }
    return;
  }

  getPostCode(place) {
    const COMPONENT_TEMPLATE = { postal_code: 'long_name' },
      postCode = this.getAddrComponent(place, COMPONENT_TEMPLATE);
    return postCode;
  }
  getCountryShort(place) {
    const COMPONENT_TEMPLATE = { country: 'short_name' },
      countryShort = this.getAddrComponent(place, COMPONENT_TEMPLATE);
    return countryShort;
  }
  getState(place) {
    const COMPONENT_TEMPLATE = { administrative_area_level_1: 'short_name' },
      state = this.getAddrComponent(place, COMPONENT_TEMPLATE);
    return state;
  }
  getCity(place) {
    const COMPONENT_TEMPLATE = { locality: 'long_name' },
      city = this.getAddrComponent(place, COMPONENT_TEMPLATE);
    return city;
  }
  getStreetNumber(place) {
    const COMPONENT_TEMPLATE = { street_number: 'short_name' },
      streetNumber = this.getAddrComponent(place, COMPONENT_TEMPLATE);
    return streetNumber;
  }

  getStreet(place) {
    const COMPONENT_TEMPLATE = { route: 'long_name' },
      street = this.getAddrComponent(place, COMPONENT_TEMPLATE);
    return street;
  }


  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage);
  }

}
