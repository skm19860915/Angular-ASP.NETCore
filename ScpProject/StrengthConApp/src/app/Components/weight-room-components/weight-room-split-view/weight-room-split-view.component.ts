import { Component, OnInit, Input } from '@angular/core';
import { Athlete } from "../../../Models/Athlete";
import { WeightRoomBase } from "../weight-room-base";

/**
 * View used to represent the Weight Room when 2 athletes are selected.
 * Inhereits from WeightRoomBase for use of setSelectedAthlete.
 * @extends WeightRoomBase
 * @implements OnInit
 */
@Component({
  selector: 'app-weight-room-split-view',
  templateUrl: './weight-room-split-view.component.html',
  styleUrls: ['./weight-room-split-view.component.less']
})
export class WeightRoomSplitViewComponent extends WeightRoomBase implements OnInit {

  @Input() athleteArray: Athlete[] = null;

  constructor() {
    super();
  }

  ngOnInit(): void {
  }

}
