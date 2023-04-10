import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { MetricsService } from '../../Services/metrics.service';
import { Metric } from '../../Models/Metric/Metric';
import { TagModel } from '../../Models/TagModel';
import { TagService } from '../../Services/tag-service.service';
import { Tag, TagType } from '../../Models/Tag';
import { fadeInAnimation } from '../../animation/fadeIn';
import { TagFilterPipe } from '../../Pipes';
import { AlertMessage } from 'src/app/Models/AlertMessage';
import { DisplayPaginatedItems } from '../shared/paginator/paginator.component';

@Component({
  selector: 'app-metric',
  templateUrl: './metric.component.html',
  styleUrls: ['./metric.component.less'],
  animations: [fadeInAnimation]
})
export class MetricComponent implements OnInit {
  public ShowArchive: boolean = false;
  public AllMetrics: Metric[] = [];
  public UnModifiedMetrics: Metric[] = [];
  public AllTags: TagModel[] = [];
  public TagItems: TagModel[] = [];
  public newMetricTagItems: TagModel[] = [];
  @Input() View: string = "Metrics";
  @Input() Model: boolean = false;
  @Output() ModelClose: EventEmitter<any> = new EventEmitter();
  public SelectedMetric: Metric = new Metric();
  public searchString: string;
  public ShowMeasurementCreationWindow: boolean = false;
  public AlertMessages: AlertMessage[] = [];
  public ShowHardDeleteWindow: boolean = false;
  public hardDeleteMetricId: number = 0;
  public paginationStart: number = 0;
  public paginationEnd : number = 0;
  public AlertTagComponentReset: EventEmitter<boolean> = new EventEmitter();

  constructor(public tagFilterPipe: TagFilterPipe, private metricsService: MetricsService, public tagService: TagService, private cd: ChangeDetectorRef) {
  }

  ngOnInit() {
    this.GetAllMetrics();

    this.tagService.GetAllTags(TagType.Metric).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllTags.push(newTM)
      }
    });
  }

  UpdatePaginiationDisplay(s:DisplayPaginatedItems) {
    this.paginationStart = s.Start;
    this.paginationEnd = s.End;
    this.cd.detectChanges();
 }

  ToggleArchive() {
    this.ShowArchive = !this.ShowArchive;
  }

  OpenMeasurementWindow() {
    this.ShowMeasurementCreationWindow = true;
  }

  ResetTags() {
    this.AlertTagComponentReset.emit(true);
  }

  AddTag(s: TagModel) {
    this.TagItems.push(s);
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    this.AllMetrics = this.tagFilterPipe.transform(this.UnModifiedMetrics, this.TagItems);
  }

  RemoveTag(s: TagModel) {
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    var index = this.TagItems.findIndex(x => x.display == s.display);
    this.TagItems.splice(index, 1);
    this.AllMetrics = this.tagFilterPipe.transform(this.UnModifiedMetrics, this.TagItems);
  }

  SetSelectedTag(tag: TagModel) {
    this.TagItems = [];
    this.TagItems.push(tag);
    this.ViewAllMetrics();
  }

  ViewAllMetrics() {
    this.View = "Metrics";
  }

  ModifySelectedMetric(newSelectedMetric: Metric) {
    window.scroll(0, 0);
    this.ResetTags();
    this.SelectedMetric = newSelectedMetric;

    this.View = "CreateMetric";
    this.newMetricTagItems = [];
    this.SelectedMetric.Tags.forEach((value, index) => {//the stupid onAdd doesnt add an object it just adds a fucking string not an object
      this.newMetricTagItems.push({ display: value.Name, value: value.Id });
    });
  }

  ViewCreateMetricMenu() {
    this.SelectedMetric = new Metric();
    this.ResetTags();
    this.newMetricTagItems = [];
    this.View = "CreateMetric";
    this.TagItems.forEach((value, index) => {//the stupid onAdd doesnt add an object it just adds a fucking string not an object
      this.newMetricTagItems.push({ display: value.display, value: value.value });
    });
  }

  GetAllMetrics() {
    this.metricsService.GetAllMetrics().subscribe(x => {
      if (x.length == 0) return;
      this.UnModifiedMetrics = x
      this.AllMetrics = this.UnModifiedMetrics;
    });
  }

  Save(data:any){
    this.SaveMetric(data.targetMetric, data.associatedTags);
  }

  SaveMetric(targetMetric: Metric, associatedTags: TagModel[]) {
    if (targetMetric.Name == undefined || targetMetric.Name == '') {
      return;
    }

    var newMetric = new Metric();
    newMetric.Id = targetMetric.Id;
    newMetric.Name = targetMetric.Name;
    newMetric.UnitOfMeasurementId = targetMetric.UnitOfMeasurementId;

    associatedTags.forEach((tag) => { this.SaveNewTags(tag); })

    associatedTags.forEach((value, index) => {
      this.AllTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Metric;
          newMetric.Tags.push(newTag);
        }
      });
    });

    if (targetMetric.Id > 0) {
      this.metricsService.UpdateMetric(newMetric).subscribe(success => {
        this.GetAllMetrics();
        this.SelectedMetric = new Metric();
        this.newMetricTagItems = [];
        this.DisplayMessage('Metric Updated Successfully', 'Metric Updated Successfully', false)
        this.ResetTags();
      }, error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Metric Updated Unsuccessfully', errorMessage, true)
      });
    }
    else {
      this.metricsService.CreateMetric(newMetric).subscribe(success => {
        this.GetAllMetrics();
        this.SelectedMetric = new Metric();
        this.newMetricTagItems = [];
        this.DisplayMessage('Metric Created Successfully', 'Metric Created Successfully', false)
        this.ResetTags();
      }, error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Metric Created Unsuccessfully', errorMessage, true)
      });
    }
  }

  SaveNewTags(s: TagModel){
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Metric;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        newTag.value = data;
        this.AllTags.push(newTag);
      });
    }
  }

  DuplicateMetric(metricId: number): void {
    this.metricsService.DuplicateMetric(metricId).subscribe(
      success => {
        this.GetAllMetrics();
        this.DisplayMessage('Metric Duplicated successfully', "Metric Duplicated Successfully", false)
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Metric Duplicated Unsuccessfully', errorMessage, true)
      });
  }

  UnArchiveMetric(metricId: number) {
    this.metricsService.UnArchiveMetric(metricId).subscribe(
      success => {
        this.GetAllMetrics();
        this.DisplayMessage('Metric UnArchived Successfully', "Metric UnArchived Successfully", false)
      },
      error => {
        this.DisplayMessage('Metric UnArchived Unsuccessfully', "Metric UnArchived Uncessfully", true)
      });
  }

  ArchiveMetric(metricId: number) {
    this.metricsService.ArchiveMetric(metricId).subscribe(
      success => {
        this.GetAllMetrics();
        this.DisplayMessage('Metric Archived Unsuccessfully', "Metric Archived Sucessfully", false)
      },
      error => {
        this.DisplayMessage('Metric Archived Unsuccessfully', "Metric Archived Unsccessfully", true)
      });
  }

  ConfirmArchive(metricId: number) {
  }

  CancelArchive() {
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }

  public ToggleHardDeleteModal(exerciseId) {
    this.hardDeleteMetricId = exerciseId;
    this.ShowHardDeleteWindow = !this.ShowHardDeleteWindow;
  }

  public HardDelete() {
    this.metricsService.HardDeleteMetric(this.hardDeleteMetricId).subscribe(success =>{
      this.ToggleHardDeleteModal(0);
      this.DisplayMessage("Metric DELETED", "Metric Successfully Deleted", false);
      this.GetAllMetrics();
    },error =>{
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.ToggleHardDeleteModal(0);
      this.DisplayMessage("Exercise NOT DELETED",errorMessage,  true);
    });
  }
}
