import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { UserService } from '../../Services/user.service';
import { fadeInAnimation } from 'src/app/animation/fadeIn';

@Component({
  selector: 'app-user-ribbon',
  templateUrl: './user-ribbon.component.html',
  styleUrls: ['./user-ribbon.component.less'],
  animations: [fadeInAnimation],
})
export class UserRibbonComponent implements OnInit {

  public NoRibbon: boolean;
  private uService: UserService;
  private router: Router;
  public FirstName: string
  public CurrentPageTitle: string;
  public IsCoach: boolean = false;

  constructor(route: Router, userService: UserService) {

    this.uService = userService;
    this.IsCoach = this.uService.IsCoach();
    this.router = route;
    route.events.subscribe(x => {
      if (x instanceof NavigationEnd) {
        if (x.url.toLowerCase().includes('home')) {
          this.CurrentPageTitle = 'Home';
        }
        else if (x.url.toLowerCase().includes('exercise')) {
          this.CurrentPageTitle = 'Exercises';
        }
        else if (x.url.toLowerCase().includes('roster')) {
          if (x.url.toLowerCase().includes('coach')) {
            this.CurrentPageTitle = 'Coach Roster';
          } else {
            this.CurrentPageTitle = 'Roster';
          }
        }
        else if (x.url.toLowerCase().includes('metrics')) {
          this.CurrentPageTitle = 'Metrics';
        }
        else if (x.url.toLowerCase().includes('survey')) {
          this.CurrentPageTitle = 'Surveys';
        }
        else if (x.url.toLowerCase().includes('setrep')) {
          this.CurrentPageTitle = 'Sets & Reps';
        }
        else if (x.url.toLowerCase().includes('program')) {
          this.CurrentPageTitle = 'Programs';
        }
        else if (x.url.toLowerCase().includes('programbuilder')) {
          this.CurrentPageTitle = 'Program Builder';

        } else if (x.url.toLowerCase().includes('weightroom')) {
          this.CurrentPageTitle = '';
        }
        else if (x.url.toLowerCase().includes('multimedia')) {
          this.CurrentPageTitle = 'MultiMedia';
        }
        else if (x.url.toLowerCase().includes('chat')){
          this.CurrentPageTitle = 'Chat';
        }
        else if (x.url.toLowerCase().includes('notifications')){
          this.CurrentPageTitle = 'Notifications';
        }
        else if (x.url.toLowerCase().includes('document')){
          this.CurrentPageTitle = 'Documents';
        }
      }

      if (x instanceof NavigationEnd) {

        this.NoRibbon = (x.url.toLowerCase().includes("/weightroom") || x.url.toLowerCase() == "/login" || x.url.toLowerCase() == "/register" || x.url.toLowerCase().includes("/onetimeregister") || x.url.toLowerCase() == "/"
          || x.url.toLowerCase().includes("/whiteboardview") || x.url.toLowerCase().includes("/resetpassword") || x.url.toLowerCase().includes("/athleteemailverification") || x.url.toLowerCase().includes("/assistantcoachemailregistration"));
      }
    });
  }

  Logout() {
    this.uService.Logout();
    this.router.navigate(['/Login']);
  }

  ngOnInit() {
    this.FirstName = this.uService.GetUserName()
  }
}
