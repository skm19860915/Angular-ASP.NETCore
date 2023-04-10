import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Athlete } from "../../../Models/Athlete";

@Component({
  selector: 'app-weight-room-athlete-header',
  templateUrl: './weight-room-athlete-header.component.html',
  styleUrls: ['./weight-room-athlete-header.component.less']
})
export class WeightRoomAthleteHeaderComponent implements OnInit {

  @Input() athlete: Athlete = null;
  @Input() isSelected: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

}
