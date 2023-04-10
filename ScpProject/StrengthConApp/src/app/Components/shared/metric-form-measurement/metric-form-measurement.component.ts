import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { UnitOfMeasurement } from 'src/app/Models/UnitOfMeasurement';

@Component({
  selector: 'app-metric-form-measurement',
  templateUrl: './metric-form-measurement.component.html',
  styleUrls: ['./metric-form-measurement.component.less'],
  animations: [fadeInAnimation]
})
export class MetricFormMeasurementComponent implements OnInit {
  @Input() ShowMetricFormMeasurementModal: boolean = false;
  @Input() SelectedMeasurement: UnitOfMeasurement = new UnitOfMeasurement();
  @Output() SaveCallBack = new EventEmitter<string>();
  @Output() CancelCallBack = new EventEmitter<boolean>();

  constructor() { }

  ngOnInit(): void {
  }

  Save(unitType: string) {
    this.SaveCallBack.emit(unitType);
  }

  Cancel() {
    this.SelectedMeasurement.UnitType = "";
    this.CancelCallBack.emit(true);
  }
}
