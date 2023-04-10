import { ViewEncapsulation, Component, OnInit } from "@angular/core";
import { fadeInAnimation } from "../../animation/fadeIn";
import { ActivatedRoute, Router, RouterOutlet } from "@angular/router";
import { Athlete } from "../../Models/Athlete";
import { RosterService } from "../../Services/roster.service";
import { UserService } from '../../Services/user.service'

@Component({
  selector: 'app-athlete-home',
  templateUrl: './athlete-home.component.html',
  styleUrls: ['./athlete-home.component.less'],
  encapsulation: ViewEncapsulation.Emulated,
  animations: [fadeInAnimation],
})

export class AthleteHomeComponent implements OnInit {
  public AthleteId: number = 0;
  public CurrentAthlete: Athlete = new Athlete();
  public IsCoach: boolean = false;
  public tabs = [
    { IsActive: true, TabText: "Current Workout", AthleteId: this.AthleteId, URL: 'Workout' },
    { IsActive: false, TabText: "Past Workouts", AthleteId: this.AthleteId, URL: 'PastWorkouts' },
    { IsActive: false, TabText: "Metrics", AthleteId: this.AthleteId, URL: 'Metric' },
    { IsActive: false, TabText: "Surveys", AthleteId: this.AthleteId, URL: 'Survey' },
    { IsActive: false, TabText: "Bio", AthleteId: this.AthleteId, URL: 'Bio' }
  ]




  constructor(private route: ActivatedRoute, private router: Router, private rosterService: RosterService, private userService: UserService) {
    this.IsCoach = this.userService.IsCoach();

    this.route.params.subscribe(params => {

      if (params['id'] != undefined) {//coach navigate to athletes page
        this.AthleteId = params['id']
        this.TabSwitch(this.tabs[0]);
        this.rosterService.GetAthlete(params['id']).subscribe(success => {
          this.CurrentAthlete = success;
        })
      }
      else {
        this.rosterService.GetLoggedInAthlete().subscribe(x => {
          this.CurrentAthlete = x
          this.AthleteId = x.Id;
        })
        //athlete logged in, to view their stuff
      }
    });
  }

  ngOnInit() {
  }
  prepareRoute(outlet: RouterOutlet) {
    return outlet.isActivated ? outlet.activatedRoute : '';
  }
  TabSwitch(tab) {
    var parentString = '';
    this.tabs.forEach(x => x.IsActive = false);
    tab.IsActive = true;
    // @ts-ignore: for know the value property exists, we will see when we upgrade angular later (v7.x)
    this.route.url.value.forEach(x => parentString = parentString + '/' + x);
    this.router.navigate([parentString + '/' + tab.URL + '/' + this.AthleteId]);
  }
}
