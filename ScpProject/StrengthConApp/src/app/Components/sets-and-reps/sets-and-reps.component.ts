import { Component, OnInit, AfterViewInit, OnDestroy, Input, Output, EventEmitter, ViewEncapsulation } from '@angular/core';
import { Week } from '../../Models/Week';
import { Set } from '../../Models/SetsAndReps/Set';
import { TagModel } from '../../Models/TagModel';
import { TagService } from '../../Services/tag-service.service';
import { Tag, TagType } from '../../Models/Tag';
import { WorkoutDetails } from '../../Models/SetsAndReps/WorkoutDetails';
import { Observable, Subscription, interval, of } from 'rxjs';
import { WorkoutService } from '../../Services/workout.service'
import { Workout } from '../../Models/SetsAndReps/Workout';

import { AlertMessage } from 'src/app/Models/AlertMessage';
import { take } from 'rxjs/operators';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { TagFilterPipe } from 'src/app/Pipes';

@Component({
  selector: 'app-sets-and-reps',
  templateUrl: './sets-and-reps.component.html',
  styleUrls: ['./sets-and-reps.component.less'],
  animations: [fadeInAnimation]
})
export class SetsAndRepsComponent implements OnInit, AfterViewInit {
  public SelectedWorkout: WorkoutDetails = new WorkoutDetails();
  public ShowArchive: boolean = false;
  public NewSetRep: string;
  public TagItems: TagModel[] = [];
  public AllTags: TagModel[] = [];
  @Input() View: string = "Workouts";
  @Input() Model: boolean = false;
  @Output() ModelClose: EventEmitter<any> = new EventEmitter();
  public AllWorkouts: Observable<Workout[]>;
  public UnModifiedWorkouts: Observable<Workout[]>;
  public newWorkoutTagItems: TagModel[] = [];
  public _workoutService
  public subs: Subscription = new Subscription();
  public searchString: string = '';
  public AlertMessages: AlertMessage[] = [];

  constructor(public tagFilterPipe: TagFilterPipe, public workoutService: WorkoutService, public tagService: TagService) {
    this._workoutService = workoutService;
    this.UnModifiedWorkouts = workoutService.GetWorkouts();
    this.AllWorkouts = this.UnModifiedWorkouts;
  }

  ngOnDestroy() {

    this.subs.unsubscribe();
  }

  ngOnInit() {
    this.TagItems = [];

    this.tagService.GetAllTags(TagType.Workout).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllTags.push(newTM)
      }
    });
  }

  ViewAllWorkouts() {
    this.View = "Workouts"
  }

  ToggleArchive() {
    this.ShowArchive = !this.ShowArchive;
  }

  ModifySelectedWorkout(workout: Workout) {
    window.scroll(0, 0);
    this.workoutService.GetWorkoutOutDetails(workout.Id).subscribe(x => {
      this.SelectedWorkout = x;
      this.SelectedWorkout.ShowRestBox = x.Rest !== undefined && x.Rest !== null;
      for (var i = 0; i < x.TotalWorkout.length; i++) {
        var element = x.TotalWorkout[i];
        this.AddSetToWeek(element)
        if (element.SetsAndReps != undefined)
        {
          for (var s = 0; s < element.SetsAndReps.length; s++)
          {
            var newS = element.SetsAndReps[s];
            if (newS.Reps !== undefined) {
              this.SelectedWorkout.ShowRepsBox = true;
            }
            if (newS.Percent !== undefined) {
              this.SelectedWorkout.ShowPercentageBox = true;
            }
            if (newS.Sets !== undefined) {
              this.SelectedWorkout.ShowSetsBox = true;
            }
            if (newS.Weight !== undefined) {
              this.SelectedWorkout.ShowWeight = true;
            }
            if (newS.Minutes !== undefined) {
              this.SelectedWorkout.ShowTimeBox = true;
            }
            if (newS.Seconds !== undefined) {
              this.SelectedWorkout.ShowTimeBox = true;
            }
            if (newS.Other !== undefined) {
              this.SelectedWorkout.ShowOtherBox = true;
            }
            if (newS.Distance !== undefined) {
              this.SelectedWorkout.ShowDistanceBox = true;
            }
            if (newS.RepsAchieved !== undefined) {
              this.SelectedWorkout.ShowRepsAchievedBox = true;
            }
          }
        }
      }

      x.Tags.forEach((value, index) => {//the stupid onAdd doesnt add an object it just adds a fucking string not an object
        this.newWorkoutTagItems.push({ display: value.Name, value: value.Id });
      });
    });
    this.View = "CreateWorkout"
  }

  ViewCreateNewWorkout() {
    this.View = "CreateWorkout"
    this.SelectedWorkout = new WorkoutDetails();
    this.newWorkoutTagItems = [];
    this.AddWeek(this.SelectedWorkout.TotalWorkout);
    this.TagItems.forEach((value, index) => {//the stupid onAdd doesnt add an object it just adds a fucking string not an object
      this.newWorkoutTagItems.push({ display: value.display, value: value.value });
    });
  }

  AddWeek(target: Week[]) {
    var newWeek = new Week();
    newWeek.Position = this.SelectedWorkout.TotalWorkout.length + 1;
    newWeek.SetsAndReps = [];
    this.AddSetToWeek(newWeek);
    target.push(newWeek);
  }

  AddSetToWeek(targetWeek: Week) {
    var set = new Set();
    set.Position = targetWeek.SetsAndReps.length;
    targetWeek.SetsAndReps.push(set);
  }

  Save(data:any){
    this.SaveWorkout(data.targetWorkout, data.associatedTags);
  }

  SaveWorkout(targetWorkout: WorkoutDetails, associatedTags: TagModel[]) {
    if (targetWorkout.Name == '' || targetWorkout.Name == undefined) {
      this.DisplayMessage("Save UNSUCCESSFULL", "The Set and Rep Needs To Have A Name To Save", true)
      return;
    }
    targetWorkout.Tags = [];
    associatedTags.forEach((value, index) => {
      this.AllTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Workout;
          targetWorkout.Tags.push(newTag);
        }
      });
    });
    if (targetWorkout.Id != 0 && targetWorkout.Id != undefined) {
      this.workoutService.UpdateWorkout(targetWorkout).subscribe(
        success => {
          this.DisplayMessage("Update SUCCESSFULL", "Workout Saved", false)
          this.ViewCreateNewWorkout();
          if (this.Model) {
            this.ModelClose.emit(true);
          }
          this.UnModifiedWorkouts = this.workoutService.GetWorkouts();
          this.AllWorkouts = this.UnModifiedWorkouts;
          this.newWorkoutTagItems = [];//overrideing the viewCreateNewWorkout because we want a clean tag list
        },
        error => {
          var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
          this.DisplayMessage("Update UNSUCCESSFULL", errorMessage, true)
        });
    }
    else {
      this.workoutService.CreateWorkout(targetWorkout).subscribe(
        success => {
          this.DisplayMessage("Save SUCCESSFULL", "Workout Saved", false)
          this.ViewCreateNewWorkout();
          if (this.Model) {
            this.ModelClose.emit(true);
          }
          this.UnModifiedWorkouts = this.workoutService.GetWorkouts();
          this.AllWorkouts = this.UnModifiedWorkouts;
          this.newWorkoutTagItems = [];//overrideing the viewCreateNewWorkout because we want a clean tag list
        },
        error => {
          var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
          this.DisplayMessage("Save UNSUCCESSFULL", errorMessage, true)
        });
    }
  }

  AddTag(s: TagModel) {
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Workout;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {

        newTag.value = data;
        this.AllTags.push(newTag);
      });
    }
    this.TagItems.push(newTag);
    this.UnModifiedWorkouts.subscribe(x => {
      this.AllWorkouts = (of(this.tagFilterPipe.transform(x, this.TagItems)))
    })
  };

  RemoveTag(s: TagModel) {
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    var index = this.TagItems.findIndex(x => { return x.display == s.display });// this.TagItems.findIndex(x => x.display == s.display);
    this.TagItems.splice(index, 1);
    this.UnModifiedWorkouts.subscribe(x => {
      this.AllWorkouts = (of(this.tagFilterPipe.transform(x, this.TagItems)))
    });
  }

  SetSelectedTag(tag: TagModel) {
    this.TagItems = [];
    this.TagItems.push(tag);
    this.ViewAllWorkouts();
  }

  AddTagToNewWorkout(s: TagModel) {

    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Workout;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        var newTag = new TagModel()
        newTag.display = s.display;
        newTag.value = data;
        this.AllTags.push(newTag);
      });
    }
  };

  ngAfterViewInit() {
  }

  DuplicateWorkout(workoutId: number): void {
    this._workoutService.DuplicateWorkout(workoutId).subscribe(
      success => {
        this.DisplayMessage("DUPLICATION SUCCESSFULL", "Workout Duplicated", false)
        this.AllWorkouts = this._workoutService.GetWorkouts();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("DUPLICATION UNSUCCESSFULL", errorMessage, true)
      });
  }

  UnArchiveWorkout(workoutId: number) {
    this._workoutService.UnArchiveWorkout(workoutId).subscribe(
      success => {
        this.DisplayMessage("UNARCHIVE SUCCESSFULL", "Workout UnArchived", false)
        this.AllWorkouts = this._workoutService.GetWorkouts();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("UNARCHIVE UNSUCCESSFULL", errorMessage, true)
      });
  }

  ArchiveWorkout(workoutId: number) {
    this._workoutService.ArchiveWorkout(workoutId).subscribe(
      success => {
        this.DisplayMessage("ARCHIVE SUCCESSFULL", "Workout Archived", false)
        this.AllWorkouts = this._workoutService.GetWorkouts();
      },
      error => {

        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Question Save Unsuccessfull", errorMessage, true)
      });
  }

  ConfirmArchive(workoutId: number) {
  }

  CancelArchive() {
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    var newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
    interval(3000).pipe(take(1)).subscribe(x => this.AlertMessages.splice(0, 1));
  }
}
