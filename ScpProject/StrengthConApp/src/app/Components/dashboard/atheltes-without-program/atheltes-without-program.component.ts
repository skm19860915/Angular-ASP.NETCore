import { Component, OnInit } from '@angular/core';
import { Athlete } from '../../../Models/Athlete';
import { RosterService } from '../../../Services/roster.service';

@Component({
  selector: 'app-atheltes-without-program',
  templateUrl: './atheltes-without-program.component.html',
  styleUrls: ['./atheltes-without-program.component.less']
})
export class AtheltesWithoutProgramComponent implements OnInit {

  public pageNumber: number = 0;
  public athleteCount: number = 3;
  public AllAthletes: Athlete[] ;
  public TotalAthleteCount : number = 0;
  constructor(private roster: RosterService) { }

  ngOnInit() {
    this.GetAthletes();
  }
  AdvancePageNumber() {
    this.pageNumber++;
    this.GetAthletes();
  }

DecrementPageNumber() {
  if (this.pageNumber == 0) {
    return;
  }
  this.pageNumber--;
  this.GetAthletes();
}
  GetAthletes() {
    this.roster.GetAllAthletesWithoutProgram(this.pageNumber, this.athleteCount).subscribe(x => {
      x.Athletes.forEach(y => y.Name = (y.FirstName === null || y.FirstName === undefined ? '-': y.FirstName) + ' ' + (y.LastName === null || y.LastName === undefined ? '-': y.LastName));
      this.AllAthletes = x.Athletes;
      this.TotalAthleteCount =x.AthleteCount;
    });

  }



}
