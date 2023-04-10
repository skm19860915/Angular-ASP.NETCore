import { Component, OnInit, Input } from '@angular/core';
import { Athlete } from '../../../Models/Athlete';

/**
 * View used to represent the Weight Room when 1 athlete is selected.
 * @implements OnInit
 */
@Component({
  selector: 'app-weight-room-single-view',
  templateUrl: './weight-room-single-view.component.html',
  styleUrls: ['./weight-room-single-view.component.less']
})
export class WeightRoomSingleViewComponent implements OnInit {

  @Input() athleteArray: Athlete[] = null;

  constructor() { }

  ngOnInit(): void {
  }
}
