//@ts-ignore
/// <reference types="@types/googlemaps" />
import { Component, Output, ViewChild, EventEmitter,  OnInit, AfterViewInit, Input, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { ClientInfo } from '../../../Models/Registration/ClientInfo';
import { FormGroup, FormBuilder } from '@angular/forms';
import { FinalClientRegInfo } from '../../../Models/Registration/FinalClientRegInfo';
import { UserService } from 'src/app/Services/user.service';
import { AlertMessage } from 'src/app/Models/AlertMessage';

declare var Stripe: any;


@Component({
  selector: 'app-step4',
  templateUrl: './step4.component.html',
  styleUrls: ['./step4.component.less']
})
export class Step4Component implements OnInit, AfterViewInit {

  @Input() wizardState: number;
  @Output() wizardStateChange: EventEmitter<number> = new EventEmitter(true);
  @Input() clientInfo: ClientInfo;
  @Output() FinishRegistration: EventEmitter<FinalClientRegInfo> = new EventEmitter(true)
  @Input() public finishedProcessing: EventEmitter<boolean>;
  @ViewChild('addresstext') addresstext: any;
  @ViewChild('ccNumber') ccNumberField: ElementRef;

  constructor(public router: Router, private userService: UserService) { }
  public clientRegInfo: FinalClientRegInfo = new FinalClientRegInfo();
  public autocompleteInput: string = '';

  public cvc: number ;
  public ccNum: string ;
  public expMM: number ;
  public expYYYY: number ;
  public processing : boolean = false;
  public validEmail: boolean = false;
  public emailInUse: boolean = false;
  public UserNameAlreadyExist: boolean = false;
  public AlertMessages: AlertMessage[] = [];
  public isFormValidated : boolean = true;

  ngOnInit() {
    if (this.finishedProcessing){
      this.finishedProcessing.subscribe(x => this.processing = false)
    }
  }

  ngAfterViewInit() {
    this.getPlaceAutocomplete();
  }

  /* Insert spaces to make the user's credit card number more legible */
  creditCardNumberSpacing() {
  }

  UserInfoFilledOut() {
    this.processing = true;
    this.clientRegInfo.FirstName = this.clientInfo.FirstName;
    this.clientRegInfo.LastName = this.clientInfo.LastName;
    this.clientRegInfo.Email = this.clientInfo.Email;
    this.clientRegInfo.Phone = this.clientInfo.Phone;
    // this.clientRegInfo.CVC = this.cvc;
    // this.clientRegInfo.CCNum = this.ccNum;
    // this.clientRegInfo.expirationMonth = this.expMM;
    // this.clientRegInfo.expirationYear = this.expYYYY;
    this.FinishRegistration.emit(this.clientRegInfo);
  }

  private getPlaceAutocomplete() {
    //@ts-ignore
    const autocomplete = new google.maps.places.Autocomplete(this.addresstext.nativeElement, { });
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

  Cancel() {
    this.wizardStateChange.emit(1);
    this.router.navigate(['/Login']);
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

  ValidateEmail(email: string) {
    this.validEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(email)
    this.userService.EmailInUse(email).subscribe((success: boolean) => {
      this.emailInUse = success;
    });
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

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
}
