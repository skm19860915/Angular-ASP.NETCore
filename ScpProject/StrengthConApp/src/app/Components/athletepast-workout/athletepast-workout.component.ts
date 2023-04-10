import { Component, OnInit } from '@angular/core';
import { HistoricProgram } from 'src/app/Models/Program/HistoricProgram';
import { ProgramBuilderService } from 'src/app/Services/program-builder.service';
import { ActivatedRoute } from '@angular/router';
import { RosterService } from 'src/app/Services/roster.service';
import { Athlete } from 'src/app/Models/Athlete';
import { AthletePastWorkout } from '../../Models/AthletePastWorkout/AthletePastWorkout';

@Component({
  selector: 'app-athletepast-workout',
  templateUrl: './athletepast-workout.component.html',
  styleUrls: ['./athletepast-workout.component.less']
})
export class AthletepastWorkoutComponent implements OnInit {

  public pastProgram: AthletePastWorkout = undefined;
  public HistoricPrograms: HistoricProgram[] = [];
  public CurrentAthlete: Athlete = new Athlete();
  constructor(private ProgramBuilderService: ProgramBuilderService, private route: ActivatedRoute, private rosterService: RosterService) { }


  ngOnInit() {

    this.route.params.subscribe(params => {

      if (params['athleteId'] != undefined) {//coach navigate to athletes page
        this.rosterService.GetAthlete(params['athleteId']).subscribe(success => {

          this.CurrentAthlete = success;
          this.CurrentAthlete.DisplayTags = [];
          this.CurrentAthlete.Birthday = new Date(success.Birthday + "z");

          success.Tags.forEach(element => {
            this.CurrentAthlete.DisplayTags.push({ display: element.Name, value: element.Id });
          });
          this.ProgramBuilderService.GetAllPastPrograms(this.CurrentAthlete.Id).subscribe(success => { this.HistoricPrograms = success; });
        })
      }
      else {
        this.rosterService.GetLoggedInAthlete().subscribe(x => {

          this.CurrentAthlete = x
          this.ProgramBuilderService.GetAllPastPrograms(this.CurrentAthlete.Id).subscribe(success => { this.HistoricPrograms = success; });
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
