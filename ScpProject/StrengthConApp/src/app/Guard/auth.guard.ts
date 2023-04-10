import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { VerifyToken } from '../Models/Token/VerifyToken';
import { UserService } from '../Services/user.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  private isOnboardFlowInitiated: boolean = false;

  constructor(public userService: UserService, public router: Router) { }
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {


    if (this.userService.IsLoggedIn() != '' && this.userService.IsLoggedIn() != undefined) {

      //Every route we validate the token, independantly of checking if they have a token.
      //So worse case scenario, They have an invalid token. In that case the if(this.userservice.isloggedin) logic
      //will still return true. But after we try to verify the token and the token doesnt verify we will log out the user.\
      // so the next time they navigate they will be kicked out
      this.userService.VerifyToken().subscribe((success : VerifyToken)=> {
        if (!success && !success.IsUser)//they have a bad token
        {
          this.userService.Logout();
        }
        else if (!success.IsOrganizationACustomer) {
          this.userService.MarkCustomerAsInvalid();//they can toggle with at the customer level if they are smart enough.
          //but what will happen is they will get stuck in an infinite loop of having to clear out the cookie seeting.
          //and if they are determined enough to clear it ever navigation and dealwith that hassle they are more than welcome to use the app
          //unless you are enterprising to fix this (another 12 pack in it)
          this.router.navigate(['/AccountSettings'])
        }
        else if (success.HasBadCreditCard)
        {
          this.userService.MarkCustomerBadCard();
          this.router.navigate(['/AccountSettings'])
        }
        else if (success.IsCreditCardExpiring)
        {
          this.userService.MarkCustomerCreditCardExpiring();
          this.router.navigate(['/AccountSettings'])
        }
        else if (success.HasSubscriptionEnded)
        {
          this.userService.MarkCustomerSubscriptionEnded();
          this.router.navigate(['/AccountSettings'])
        }
        //else if (success.bad)
        if (success.IsHeadCoach && !this.isOnboardFlowInitiated) {
          this.isOnboardFlowInitiated = true;
          //@ts-ignore: the onboard script is being added in the index
          window.onboardFlowSettings = {
            "site_key":             "hpAfXB9L",
            "user": {
                "id":               success.UserId, // Your internal User ID for the logged in user
                "customer_id":      success.StripeGuid, // The payment providers Customer ID for the logged in user
                "email":            "", // Email address of the logged in user (optional)
                "image_url":        ""  // Profile Image URL of the logged in user (optional)
            },
            "custom_properties": {
                'athletes_created': 0,
                'programs_made': 0
            }
        };


        }



        else {
          //at this point they are a valid user and are in good customer standing return true
          return true;
        }
      });

      return true;
    }
    //at this point they have a userToken, if it is valid or not that is unknown (unless this at least the second time they have used a route.
    //if it is the second time they have used a route the verifyToken call will log them out after the first route navigation and the second route
    //navigation should pick that up). So know we check if they are in good standing
    else {
      this.router.navigate(['/Login'])
    }
  }
}
