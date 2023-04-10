import { Component, OnInit, Input, EventEmitter, Output, ViewChild, ChangeDetectorRef } from '@angular/core';
import { ExerciseService } from '../../Services/exercise.service';
import { Tag, TagType } from '../../Models/Tag';
import { Exercise } from '../../Models/Exercise';
import { Observable, of } from 'rxjs';
import { TagModel } from '../../Models/TagModel';
import { TagService } from '../../Services/tag-service.service';
import { ITaggable } from '../../Interfaces/ITaggable';
import { MetricsService } from '../../Services/metrics.service';
import { Metric } from '../../Models/Metric/Metric';
import { HideDeletedSortPipe } from '../../Pipes';
import { TagFilterPipe, } from '../../Pipes';
import { fadeInAnimation } from '../../animation/fadeIn';
import { ScpTagInputComponent } from '../shared/scp-tag-input/scp-tag-input.component';
import { AlertMessage } from '../../Models/AlertMessage';
import { NgxSmartModalService } from 'ngx-smart-modal';
import { DisplayPaginatedItems } from '../shared/paginator/paginator.component';

@Component({
  selector: 'app-exercise',
  templateUrl: './exercise.component.html',
  styleUrls: ['./exercise.component.less'],
  animations: [fadeInAnimation]
})
export class ExerciseComponent implements OnInit {
  @ViewChild(ScpTagInputComponent) scpTagInputChild: ScpTagInputComponent;
  private scpTagInput: ScpTagInputComponent;
  public ShowArchive: boolean = false;
  public searchString: string = '';
  public AllTags: TagModel[] = [];
  public TagItems: TagModel[] = [];
  public newExerciseTagItems: TagModel[] = [];
  public AllExercises: Observable<Exercise[]>;
  public UnModifedExercises: Observable<Exercise[]>;//fuck this is bad. Because I had to re-write the tags and now it cant us NGModel this has to be done. Assign someone task to make it work with NGModel
  public CreateNewExercise: boolean = false;
  @Input() View: string = 'Exercises';
  @Input() Model: boolean = false;
  @Output() ModelClose: EventEmitter<any> = new EventEmitter();
  public ExerciseFilterTags: TagModel[] = [];
  public SelectedExercise: Exercise = new Exercise();
  public AllMetrics: Metric[] = [];
  public SelectedMetric: Metric;
  public ExerciseComputePercent: number;
  public IsModal: boolean;
  public ShowMetricCreationWindow: boolean = false;
  public AlertMessages: AlertMessage[] = [];
  public AllMetricTags: TagModel[] = [];
  public targetVideoToDisplay: string;
  public ShowHardDeleteWindow: boolean = false;
  public hardDeleteExerciseId: number = 0;
  public paginationStart: number = 0;
  public paginationEnd: number = 0;
  public AlertTagComponentReset: EventEmitter<boolean> = new EventEmitter();
  public newMetricTagItems: TagModel[] = [];

  constructor(private cd: ChangeDetectorRef, public modalController: NgxSmartModalService, public tagFilterPipe: TagFilterPipe, public hidePipe: HideDeletedSortPipe,
    public MetricService: MetricsService, public exerciseService: ExerciseService, public tagService: TagService) {
  }

  ngOnInit() {
    this.MetricService.GetAllMetrics().subscribe(x => { this.AllMetrics = this.hidePipe.transform(x, this.ShowArchive); });
    this.GetAllExercises();

    this.tagService.GetAllTags(TagType.Metric).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllMetricTags.push(newTM)
      }
    });

    this.tagService.GetAllTags(TagType.Exercise).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllTags.push(newTM)
      }
    });
  }

  UpdatePaginiationDisplay(s: DisplayPaginatedItems) {
    this.paginationStart = s.Start;
    this.paginationEnd = s.End;
    this.cd.detectChanges();
  }

  ToggleArchive() {
    this.ShowArchive = !this.ShowArchive;
  }

  DisplayTargetVideo(videoURL: string) {
    this.targetVideoToDisplay = videoURL;
    this.modalController.setModalData({ url: videoURL }, 'exerciseVideoModal');
    this.modalController.open('exerciseVideoModal')
  }

  SelectSearch(term: string, item: ITaggable) {
    var terms = term.split(' ');
    term = term.toLocaleLowerCase();
    var foundCount = 0;
    var termsCount = terms.length;
    for (var x = 0; x < terms.length; x++) {
      if (terms[x].length == 0) { foundCount++; continue; }
      for (var i = 0; i < item.Tags.length; i++) {
        if (item.Tags[i].Name.toLocaleLowerCase().indexOf(terms[x]) > -1) {
          foundCount++;
        }
      }
    }
    return item.Name.toLocaleLowerCase().indexOf(term) > -1 || foundCount == termsCount;
  }

  ResetTags() {
    this.AlertTagComponentReset.emit(true);
  }

  ModifySelectedExercise(exer: Exercise) {
    window.scroll(0, 0);
    this.ResetTags();
    this.SelectedExercise = exer;
    this.newExerciseTagItems = [];
    this.SelectedExercise.Tags.forEach((value, index) => {//the stupid onAdd doesnt add an object it just adds a fucking string not an object
      this.newExerciseTagItems.push({ display: value.Name, value: value.Id });
    });
    this.View = 'CreateExercise';
  }

  AddTag(s: TagModel) {
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Exercise;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        newTag.value = data;
        this.AllTags.push(newTag);
      });
    }
    this.TagItems.push(s);
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    this.UnModifedExercises.subscribe(x => {
      this.AllExercises = (of(this.tagFilterPipe.transform(x, this.TagItems)))
    });
  }

  RemoveTag(s: TagModel) {
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    var index = this.TagItems.findIndex(x => { return x.display == s.display });// this.TagItems.findIndex(x => x.display == s.display);
    this.TagItems.splice(index, 1);
    this.UnModifedExercises.subscribe(x => {
      this.AllExercises = (of(this.tagFilterPipe.transform(x, this.TagItems)))
    });
  }

  ViewCreateExerciseMenu() {
    this.View = 'CreateExercise';
    this.ResetTags();
    this.SelectedExercise = new Exercise();
    this.newExerciseTagItems = [];
    this.TagItems.forEach((value, index) => {//the stupid onAdd doesnt add an object it just adds a fucking string not an object
      this.newExerciseTagItems.push({ display: value.display, value: value.value });
    });
  }

  ReturnToTagSearch() {
    this.View = 'Exercises';
  }

  ViewAllExercises() {
    this.View = 'Exercises';
  }

  SaveExercise(exer: Exercise, associatedTags: TagModel[]) {
    var newExer: Exercise = new Exercise();
    newExer.IsDeleted = false;
    newExer.Name = exer.Name;
    newExer.Percent = exer.Percent
    newExer.PercentMetricCalculationId = exer.PercentMetricCalculationId;
    newExer.VideoURL = exer.VideoURL
    if (exer.Name == '' || exer.Name == undefined) {
      return;
    }
    newExer.Notes = exer.Notes;
    newExer.Id = 0;
    newExer.CreatedUserId = 0;
    associatedTags.forEach((value, index) => {
      this.AllTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Exercise;
          newExer.Tags.push(newTag);
        }
      });
    });
    if (exer.Id > 0) {
      newExer.Id = exer.Id;
      this.exerciseService.UpdateExercise(newExer)
        .subscribe(
          success => {
            this.SelectedExercise = new Exercise();
            this.newExerciseTagItems = [];
            this.scpTagInputChild.ClearTags();
            // this.scpTagInput.ClearTags();
            this.DisplayMessage('Exercise Updated Successfully', 'Exercise Updated Successfully', false)
            this.ReturnToTagSearch();
            this.ResetTags();
          },
          error => {
            var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
            this.DisplayMessage('Exercise Updated Unsuccessfully', error, true)
          });
    }
    else {
      this.exerciseService.CreateExercise(newExer)
        .subscribe(
          success => {
            this.SelectedExercise = new Exercise();
            this.newExerciseTagItems = [];
            this.scpTagInputChild.ClearTags();
            this.GetAllExercises();
            this.ReturnToTagSearch();
            this.DisplayMessage('Exercise Created Successfully', 'Exercise Updated Successfully', false)
            this.ResetTags();
          },
          error => {
            var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
            this.DisplayMessage('Exercise Created Unsuccessfully', errorMessage, true)
          });
    }
  }

  GetAllExercises(): void {
    this.UnModifedExercises = this.exerciseService.GetAllExercises();
    this.AllExercises = this.UnModifedExercises;
  }

  DuplicateExercise(exerciseId: number): void {
    this.exerciseService.DuplicateExercise(exerciseId).subscribe(
      success => {
        this.GetAllExercises();
        this.DisplayMessage('Exercise Duplicated Successfully', 'Exercise Duplicated Successfully', false)
      })
  }

  UnArchiveExercise(exerciseId: number) {

    this.exerciseService.UnArchiveExercise(exerciseId).subscribe(
      success => {
        this.GetAllExercises();
        this.DisplayMessage('Exercise Unarchived Successfully', 'Exercise Unarchived Successfully', false)
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Exercise Unarchived Unsuccessfully', 'Exercise Unarchived Unsuccessfully', true)
      });
  }

  ArchiveExercise(exerciseId: number) {
    this.exerciseService.ArchiveExercise(exerciseId).subscribe(
      success => {
        this.GetAllExercises();
        this.DisplayMessage('Exercise Archived Successfully', 'Exercise Archived Successfully', false)
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Exercise Archived Unsuccessfully', 'Exercise Archived Unsuccessfully', false)
      });
  }

  AddNewEditExerciseTags(s: TagModel) {
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Exercise;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        var newTM = new TagModel();
        newTM.display = s.display;
        newTM.value = data;
        this.AllTags.push(newTM)

      });
    }
    this.newExerciseTagItems.push(s);
  }

  RemoveNewEditExerciseTags(s: TagModel) {
    var index = this.newExerciseTagItems.findIndex(x => { return x.display == s.display });
    this.newExerciseTagItems.splice(index, 1);
  }

  Save(data:any){
    this.SaveMetric(data.targetMetric, data.associatedTags);
  }

  SaveMetric(targetMetric: Metric, associatedTags: TagModel[]) {
    if (targetMetric.Name == undefined || targetMetric.Name == '') {
      this.DisplayMessage('Metric Saved Unsuccessfully', 'The Metric Name cannot be empty', true)
      return;
    }

    var newMetric = new Metric();
    newMetric.Id = targetMetric.Id;
    newMetric.Name = targetMetric.Name;
    newMetric.UnitOfMeasurementId = targetMetric.UnitOfMeasurementId

    associatedTags.forEach((value, index) => {
      this.AllMetricTags.forEach((sourceTag: TagModel) => {
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
      this.MetricService.UpdateMetric(newMetric).subscribe(success => {
        this.MetricService.GetAllMetrics().subscribe(x => { this.AllMetrics = this.hidePipe.transform(x, this.ShowArchive); });
        this.SelectedMetric = new Metric();
        this.DisplayMessage('Metric Updated Successfully', 'Metric Updated Successfully', false)
      }, error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Metric Updated Unsuccessfully', errorMessage, true)
      });
    }
    else {
      this.MetricService.CreateMetric(newMetric).subscribe(success => {
        this.MetricService.GetAllMetrics().subscribe(x => { this.AllMetrics = this.hidePipe.transform(x, this.ShowArchive); });
        this.SelectedMetric = new Metric();
        this.DisplayMessage('Metric Saved Successfully', 'Metric Saved Successfully', false)
      }, error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Metric Saved Unsuccessfully', errorMessage, true)
      });
    }
  }

  public ToggleHardDeleteModal(exerciseId) {
    this.hardDeleteExerciseId = exerciseId;
    this.ShowHardDeleteWindow = !this.ShowHardDeleteWindow;
  }

  public HardDelete() {
    this.exerciseService.HardDeleteExercise(this.hardDeleteExerciseId).subscribe(success => {
      this.ToggleHardDeleteModal(0);
      this.DisplayMessage("Exercises DELETED", "Exercises Successfully Deleted", false);
      this.GetAllExercises();
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.ToggleHardDeleteModal(0);
      this.DisplayMessage("Exercise NOT DELETED", errorMessage, true);
    });
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }

  ToggleMetricCreationWindow() {
    this.ShowMetricCreationWindow = !this.ShowMetricCreationWindow;
    this.SelectedMetric = new Metric();
    this.newMetricTagItems = [];
  }
}

