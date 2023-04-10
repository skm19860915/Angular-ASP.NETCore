import { Component, OnInit, Input, DoCheck } from '@angular/core';
import { Athlete } from "../../../Models/Athlete";
import { WeightRoomBase } from '../weight-room-base';

/**
 * View used to represent the Weight Room when 4 athletes are selected.
 * Inhereits from WeightRoomBase for use of setSelectedAthlete.
 * @extends WeightRoomBase
 * @implements OnInit
 */
@Component({
  selector: 'app-weight-room-grid-view',
  templateUrl: './weight-room-grid-view.component.html',
  styleUrls: ['./weight-room-grid-view.component.less']
})
export class WeightRoomGridViewComponent extends WeightRoomBase implements OnInit {

  @Input() athleteArray: Athlete[] = null;


  constructor() { 
    super();
  }

  ngOnInit(): void {
  }

}
