import { Component } from '@angular/core';
import { Router, NavigationEnd, RouterOutlet } from '@angular/router';
import { animate, state, style, transition, trigger, query, group, animateChild } from '@angular/animations';
import { UserService} from "./Services/user.service";
//   style({ opacity: '0' }),
//   animate(750),
//   style({ opacity: '1' }),
//   animate(750)
// ]),
// transition(':leave', [
//   animate(750, style({ opacity: '0' }))
// ]),
// state('*', style({ opacity: '1'  })),




@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less'],
  animations: [
    trigger('fadeAnimation', [

      transition('* => *', [
        query(':enter',
          [
            style({ opacity: 0 })
          ],
          { optional: true }
        ),
        query(':leave',
          [
            style({ opacity: 1 }),
            animate('0.4s', style({ opacity: 0 }))
          ],
          { optional: true }
        ),
        query(':enter',
          [
            style({ opacity: 0 }),
            animate('0.4s', style({ opacity: 1 }))
          ],
          { optional: true }
        )
      ])
    ])
  ]
})
export class AppComponent {
  title = 'StrengthConApp';

  public DisplayLoginImage: boolean;
  public HideMenu: boolean
  public IsWeightRoomView : boolean;
  public IsCoach : boolean;

  constructor(route: Router, userService: UserService) {
    this.IsCoach = userService.IsCoach();
    route.events.subscribe(x => {
      if (x instanceof NavigationEnd) {
        this.DisplayLoginImage = (x.url.toLowerCase() == "/login" || x.url.toLowerCase() == "/register" || x.url.toLowerCase().includes("/onetimeregister")
          || x.url.toLowerCase() == "/" || x.url.toLowerCase().includes("/resetpassword") || x.url.toLowerCase().includes("/athleteemailverification") || x.url.toLowerCase().includes("/assistantcoachemailregistration"));

        this.HideMenu = x.url.toLowerCase().includes("/whiteboardview") ||  x.url.toLowerCase().includes("/weightroom") ;
        this.IsWeightRoomView =  x.url.toLowerCase().includes("/weightroom");
      }
      return true;
    });
  }
  prepareRoute(outlet: RouterOutlet) {
    return outlet.isActivated ? outlet.activatedRoute : '';
  }
}



















// style({ position: 'relative' }),
// query(':enter, :leave', [
//   style({
//     position: 'absolute',
//     top: 0,
//     left: 0,
//     width: '100%'
//   })
// ]),
// query(':enter', [
//   style({ left: '-100%'})
// ]),
// query(':leave', animateChild()),
// group([
//   query(':leave', [
//     animate('300ms ease-out', style({ left: '100%'}))
//   ]),
//   query(':enter', [
//     animate('300ms ease-out', style({ left: '0%'}))
//   ])
// ]),
// query(':enter', animateChild()),
// ]),
// transition('* <=> FilterPage', [
// style({ position: 'relative' }),
// query(':enter, :leave', [
//   style({
//     position: 'absolute',
//     top: 0,
//     left: 0,
//     width: '100%'
//   })
// ]),
// query(':enter', [
//   style({ left: '-100%'})
// ]),
// query(':leave', animateChild()),
// group([
//   query(':leave', [
//     animate('200ms ease-out', style({ left: '100%'}))
//   ]),
//   query(':enter', [
//     animate('300ms ease-out', style({ left: '0%'}))
//   ])
// ]),
// query(':enter', animateChild()),
// ])
