import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { AlertMessage } from 'src/app/Models/AlertMessage';
import { Metric } from 'src/app/Models/Metric/Metric';
import { TagModel } from 'src/app/Models/TagModel';
import { UnitOfMeasurement } from 'src/app/Models/UnitOfMeasurement';
import { MetricsService } from 'src/app/Services/metrics.service';

@Component({
  selector: 'app-metric-form',
  templateUrl: './metric-form.component.html',
  styleUrls: ['./metric-form.component.less'],
  animations: [fadeInAnimation]
})
export class MetricFormComponent implements OnInit {

  @Input() View: string = "Metrics";
  @Input() SelectedMetric: Metric = new Metric();
  @Input() newMetricTagItems: TagModel[] = [];
  @Output() CancelCallBack = new EventEmitter<boolean>();
  @Output() SaveCallBack = new EventEmitter<any>();
  @Input() LabelVisible: boolean = true;
  public ShowMeasurementCreationWindow: boolean = false;
  public AlertMessages: AlertMessage[] = [];
  public AllUnitsOfMeasurement: UnitOfMeasurement[] = [];
  public SelectedMeasurement: UnitOfMeasurement = new UnitOfMeasurement();

  constructor(private metricsService: MetricsService) { }

  ngOnInit(): void {
    this.GetAllMeasurements();
  }

  GetAllMeasurements() {
    this.metricsService.GetAllMeasurements().subscribe(x => {
      if (x.length == 0) return;
      this.AllUnitsOfMeasurement = x;
      this.AllUnitsOfMeasurement.forEach(y => y.Name = y.UnitType);//lazy hack to get the searchable working);
    });
  }

  CreateMeasurement() {
    this.ShowMeasurementCreationWindow = true;
  }

  RemoveMetricTag(s: TagModel) {
    var index = this.newMetricTagItems.findIndex(x => { return x.display == s.display });
    this.newMetricTagItems.splice(index, 1);
  }

  AddNewMetricTag(s: TagModel) {
    this.newMetricTagItems.push(s);
  }

  Save(targetMetric: Metric, associatedTags: TagModel[]) {
    var data = {targetMetric, associatedTags};
    this.SaveCallBack.emit(data);
  }

  Cancel() {
    this.CancelCallBack.emit(true);
  }

  AddMeasurement(unitType: string) {
    if(unitType){
      this.metricsService.CreateUnitOfMeasurement(unitType).subscribe(
        success => {
          this.DisplayMessage('Measurement Created Successfully', "Measurement Created Successfully", false);
          this.GetAllMeasurements();
        },
        error => {
          var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
          this.DisplayMessage('Measurement Created Unsuccessfully', errorMessage, true)
        });

      this.ShowMeasurementCreationWindow = false;
      this.SelectedMeasurement = new UnitOfMeasurement();
    }
  }

  ViewMetricForm(){
    this.ShowMeasurementCreationWindow = false;
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
}
