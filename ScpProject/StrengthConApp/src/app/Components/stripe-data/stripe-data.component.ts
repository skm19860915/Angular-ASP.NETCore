import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/Services/user.service';
import { StripeCustomerData } from './StripeCustomerData';
import { AlertMessage } from 'src/app/Models/AlertMessage';
import { Observable, of, interval } from 'rxjs';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-stripe-data',
  templateUrl: './stripe-data.component.html',
  styleUrls: ['./stripe-data.component.less']
})
export class StripeDataComponent implements OnInit {
  Processing: boolean;
  $: any;
  FirstName: string;
  LastName: string;
  Address1: string = "";
  Address2: string = "";
  Phone: number;
  Zip: number;
  Email: string;
  City: string;
  State: string;
  Country: string;
  CardNumber: string;
  ExpiryMonth: number;
  ExpiryYear: number;
  CVC: number;
  Coupon: string;
  public AlertMessages: AlertMessage[] = [];
  statusfailed: boolean;
  public Adddress1:string;
  public Adddress2:string;
  constructor(private userService: UserService) { }

  public StripeClick() {
    this.Processing = true;
    this.userService.CreateStripeCustomer(1, 1, new StripeCustomerData(this.FirstName,
      this.LastName,
      this.Address1,
      this.Address2,
      this.Phone,
      this.Zip,
      this.Email,
      this.City,
      this.State,
      this.Country,
      this.CardNumber,
      this.ExpiryMonth,
      this.CVC, this.ExpiryYear, this.Coupon)).subscribe(response => {
        this.Processing = false;
        if (response.toString() == "true") {
          this.DisplayMessage('Registration Successful', 'Stripe Registration Successful', false);
        } else {
          this.statusfailed = true;
          this.DisplayMessage('Registration Failed', 'Stripe Registration Failed', true);
  }
      });
  }

  ngOnInit() {
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
