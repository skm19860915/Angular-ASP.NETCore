import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { MeasuredMetric } from 'src/app/Models/Metric/MeasuredMetric';
import { MetricsService } from 'src/app/Services/metrics.service';
import { CompletedMetricHistory } from 'src/app/Models/Metric/CompletedMetricHistory';
import { AthleteService } from '../../Services/athlete.service';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { DragScrollComponent } from 'ngx-drag-scroll';
import { AlertMessage } from '../../Models/AlertMessage';
import { interval } from 'rxjs';
import { take } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { RosterService } from 'src/app/Services/roster.service';

@Component({
  selector: 'app-athlete-metric',
  templateUrl: './athlete-metric.component.html',
  styleUrls: ['./athlete-metric.component.less'],
  animations: [fadeInAnimation]
})
export class AthleteMetricComponent implements OnInit {
  public MetricService: MetricsService;
  public AllMeasuredMetrics: MetricChart[] = [];
  public CompletedMetricStrip: CompletedMetricHistory[];
  public AthleteService: AthleteService;
  public SelectedMetricId: number = 0;
  public ShowMetricUpdateWindow: boolean = false;
  public datePickerConfig ={ format:"DD-MM-YYYY"}
  public AlertMessages: AlertMessage[] = [];
  @ViewChild('nav', { read: DragScrollComponent }) ds: DragScrollComponent;
  mySlideOptions = { items: 1, dots: true, nav: true };
  myCarouselOptions = { items: 3, dots: true, nav: true };


  moveLeft() {
    this.ds.moveLeft();
  }

  moveRight() {
    this.ds.moveRight();
  }

  public carouselOptions = {
    margin: 25,
    nav: true,
    navText: ["<div class='nav-btn prev-slide'></div>", "<div class='nav-btn next-slide'></div>"],
    responsiveClass: true,
    responsive: {
      0: {
        items: 1,
        nav: true
      },
      600: {
        items: 3,
        nav: true
      },
      1000: {
        items: 3,
        nav: true,
        loop: false
      },
      1500: {
        items: 5,
        nav: true,
        loop: false
      }
    }
  }
  @Input() AthleteId: number = 0;
  public metricHistory: CompletedMetricHistory[];
  constructor(public metricService: MetricsService, public athleteService: AthleteService,private route: ActivatedRoute, private rosterService: RosterService) {
    this.MetricService = metricService;
    this.AthleteService = athleteService;

  }

  ngOnInit() {

    this.route.params.subscribe(params => {

      if (params['athleteId'] != undefined) {//coach navigate to athletes page
        this.AthleteId = params['athleteId'];
      }
      else {
        this.rosterService.GetLoggedInAthlete().subscribe(x => {
          this.AthleteId = x.Id;
        })
        //athlete logged in, to view their stuff
      }
    });
    this.GetCompletedMetricStrip();
  }

  ToggleMetricUpdateWindow() {
    this.ShowMetricUpdateWindow = !this.ShowMetricUpdateWindow
    if (!this.ShowMetricUpdateWindow) this.GetCompletedMetricStrip();
  }
  UpdateMetric(e) {
    this.athleteService.UpdateMetric(e.Id, e.Value, e.DisplayDate.startDate, e.IsCompletedMetric).subscribe(x => {
      this.DisplayMessage("Metric Update", "Metric Updated Successfully", false);
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage("Metric  NOT Updated", errorMessage, true);
    });
  }

  GetCompletedMetricStrip() {

    this.AthleteService.GetAthleteListOfCompletedMetrics(this.AthleteId).subscribe(x => {
      this.CompletedMetricStrip = x;
      if (this.CompletedMetricStrip.length > 0) {
        this.CompletedMetricStrip[0].IsSelected = true;
        this.SelectedMetricId = this.CompletedMetricStrip[0].MetricId;
      }
      this.ViewMetrics();
      this.GetMetricHistory(this.SelectedMetricId, this.AthleteId);
    });
  }
  GetMetricHistory(metricId: number, athleteId) {
    this.athleteService.GetMetricHistory(metricId, athleteId).subscribe(x => {
      this.metricHistory = x
      this.metricHistory.forEach(y => y.DisplayDate = {startDate: y.CompletedDate , endDate : y.CompletedDate })
      this.metricHistory.sort((a, b) => new Date(b.CompletedDate).getTime() - new Date(a.CompletedDate).getTime());
    });
  }
  SetSelectedToTrue(allCompletedMetrics: CompletedMetricHistory[], targetMetric: CompletedMetricHistory) {
    allCompletedMetrics.forEach(x => x.IsSelected = false);
    targetMetric.IsSelected = true;
    this.SelectedMetricId = targetMetric.MetricId
    this.ViewMetrics();
    this.GetMetricHistory(this.SelectedMetricId, this.AthleteId);
  }

  ViewMetrics() {

    this.MetricService.GetAllMeasuredMetrics(this.AthleteId).subscribe(success => {
      var targetMetricChart = new MetricChart();
      targetMetricChart.Series = [];
      success.CompletedMetrics.forEach(groupedMetrics => {
        var allMetricIds = [];
        this.AllMeasuredMetrics = [];

        groupedMetrics.Metrics.filter(z => z.MetricId == this.SelectedMetricId).forEach(y => {
          if (allMetricIds.indexOf(y.MetricId) == -1) {
            allMetricIds.push(y.MetricId);
          }
        })

        if (groupedMetrics.UnitOfMeasurementName != null && groupedMetrics.UnitOfMeasurementName != undefined) {
          targetMetricChart.yAxisLabel = groupedMetrics.UnitOfMeasurementName.toUpperCase();
        }


        allMetricIds.forEach(id => {
          //  ;
          var newUnitOfMeasurementChart = { name: '', series: [] };
          groupedMetrics.Metrics.sort((a: MeasuredMetric, b: MeasuredMetric) => {
            let dateOne = new Date(a.CompletedDate);
            let dateTwo = new Date(b.CompletedDate);
            if (dateOne == dateTwo) return 0;
            if (dateOne > dateTwo) return 1;
            if (dateOne < dateTwo) return -1;
          }).forEach(metric => {
            if (metric.MetricId == id) {
              newUnitOfMeasurementChart.name = metric.MetricName;//yup this is bad, but it works and we can fix it later
              newUnitOfMeasurementChart.series.push({ name: new Date(metric.CompletedDate).toLocaleDateString("en-us"), value: metric.MetricValue });
              newUnitOfMeasurementChart.series = newUnitOfMeasurementChart.series;
            }
          })

          targetMetricChart.Series.push(newUnitOfMeasurementChart);
        })



      });
      this.AllMeasuredMetrics.push(targetMetricChart);
    });

  }
  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }

}


export class MetricChart {
  public showXAxis = true;
  public showYAxis = true;
  public gradient = false;
  public showLegend = false;
  public showXAxisLabel = true;
  public xAxisLabel: "DATE";
  public showYAxisLabel = true;
  public yAxisLabel: string;
  public autoScale = true
  public colorScheme = {
    domain: ['#5AA454', '#A10A28', '#C7B42C', '#AAAAAA']
  };
  public Series: any;
  public MetricId: number;
}