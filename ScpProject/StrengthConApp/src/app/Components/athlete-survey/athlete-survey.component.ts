import { Component, OnInit } from '@angular/core';
import { Athlete } from 'src/app/Models/Athlete';
import { PastSurveyListItem } from 'src/app/Models/Survey/PastSurveyListItem';
import { ActivatedRoute } from '@angular/router';
import { RosterService } from 'src/app/Services/roster.service';
import { SurveyService } from 'src/app/Services/survey.service';
import { AthletePastWorkout } from '../../Models/AthletePastWorkout/AthletePastWorkout';

@Component({
  selector: 'app-athlete-survey',
  templateUrl: './athlete-survey.component.html',
  styleUrls: ['./athlete-survey.component.less']
})
export class AthleteSurveyComponent implements OnInit {

  public pastProgram: AthletePastWorkout = undefined;
  public CurrentAthlete: Athlete = new Athlete();
  public HistoricSurvyes: PastSurveyListItem[] = []

  constructor(private route: ActivatedRoute, private rosterService: RosterService, private surveyService: SurveyService) { }

  ngOnInit() {


    this.route.params.subscribe(params => {

      if (params['athleteId'] != undefined) {//coach navigate to athletes page
        this.surveyService.GetAllPastSurveys(params['athleteId']).subscribe(x => this.HistoricSurvyes = x);
        this.rosterService.GetAthlete(params['athleteId']).subscribe(success => {

          this.CurrentAthlete = success;
          this.CurrentAthlete.DisplayTags = [];
          this.CurrentAthlete.Birthday = new Date(success.Birthday + "z");

          success.Tags.forEach(element => {
            this.CurrentAthlete.DisplayTags.push({ display: element.Name, value: element.Id });
          });
        })
      }
      else {
        this.rosterService.GetLoggedInAthlete().subscribe(x => {

          this.CurrentAthlete = x
          this.surveyService.GetAllPastSurveys(this.CurrentAthlete.Id).subscribe(x => this.HistoricSurvyes = x);
          x.Tags.forEach(element => {
            this.CurrentAthlete.DisplayTags.push({ display: element.Name, value: element.Id });
          });
        })
        //athlete logged in, to view their stuff
      }
    });
  }
  ShowPastProgram(assignedProgramId: number, isSnapShot: number) {
    window.scroll(0, 0);
    var ret =  new AthletePastWorkout();
    ret.AssignedProgramID =assignedProgramId;
    ret.IsSnapShot = isSnapShot
    this.pastProgram = ret;
  }
}
