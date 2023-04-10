import { Component, OnInit, Input, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { UserService } from '../../Services/user.service'
import { fadeInAnimation } from '../../animation/fadeIn';
import { NotificationService } from '../../Services/notification.service';
import { OrganizationService } from '../../Services/organization.service'
import { Organization } from '../../Models/Organization/Organization';
import * as $ from 'jquery';
import { OrganizationVM } from '../../Models/Organization/OrganizationVM';


@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.less'],
  animations: [fadeInAnimation]
})
export class NavMenuComponent implements OnInit {
  @Input() HideMenu: boolean;
  public IsCoach: boolean;
  public ShowWeightRoomViewWarning: boolean = false;
  public ShowYouAreInWeightRoomViewWarning: boolean = false;
  public HasNewNotifications: boolean = false;
  public thumbNailURL: string = '';
  public profilePic: string  = '';
  constructor(public route: Router, private userService: UserService, public notificationService: NotificationService, public orgService: OrganizationService) {
    this.IsCoach = userService.IsCoach();
    route.events.subscribe(x => {
      if (x instanceof NavigationEnd && this.IsCoach) {
        this.notificationService.HasUnreadNotifications().subscribe(x => this.HasNewNotifications = x);
      }
    });
    if (!userService.IsWeightRoomAccount())
    {
    this.userService.GetUserDetails().subscribe(x => {
      this.orgService.GetOrg(x.OrganizationId).subscribe((x: OrganizationVM) => {
        this.thumbNailURL = x.thumbnailPictureURL;
        this.profilePic = x.profilePictureURL;
      });
    });
  }
    //this.orgService.g
  }

  ngOnInit() {
    //for some reason this.route.url was comming up as '/' instead of '/weightroom'
    this.ShowYouAreInWeightRoomViewWarning = !window.location.href.toLowerCase().includes("weightroom") && this.userService.IsWeightRoomAccount();

  }
  LogOut() {
    this.ShowYouAreInWeightRoomViewWarning = false;
    this.userService.Logout();
  }
  GoToWeightRoomView() {
    this.route.navigate(['/WeightRoom'])
    this.ShowWeightRoomViewWarning = false;
    this.ShowYouAreInWeightRoomViewWarning = false;
  }
}


