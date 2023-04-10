import { Component, OnInit, Input } from '@angular/core';
import { Athlete } from '../../../Models/Athlete';
import { WeightRoomBase } from '../weight-room-base';

/**
 * View used to represent the Weight Room when 3 athletes are selected.
 * Inhereits from WeightRoomBase for use of setSelectedAthlete.
 * @extends WeightRoomBase
 * @implements OnInit
 */
@Component({
  selector: 'app-weight-room-trifold-view',
  templateUrl: './weight-room-trifold-view.component.html',
  styleUrls: ['./weight-room-trifold-view.component.less']
})
export class WeightRoomTrifoldViewComponent extends WeightRoomBase implements OnInit {

  @Input() athleteArray: Athlete[] = null;


  constructor() {
    super();
   }

  ngOnInit(): void {
  }


}
