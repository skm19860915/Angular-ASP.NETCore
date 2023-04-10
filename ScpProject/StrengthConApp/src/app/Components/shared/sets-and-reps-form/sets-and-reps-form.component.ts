import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { WorkoutDetails } from 'src/app/Models/SetsAndReps/WorkoutDetails';
import { Tag, TagType } from 'src/app/Models/Tag';
import { TagModel } from 'src/app/Models/TagModel';
import { Week } from 'src/app/Models/Week';
import { TagService } from 'src/app/Services/tag-service.service';
import { Set } from '../../../Models/SetsAndReps/Set'

@Component({
  selector: 'app-sets-and-reps-form',
  templateUrl: './sets-and-reps-form.component.html',
  styleUrls: ['./sets-and-reps-form.component.less'],
  animations: [fadeInAnimation]
})
export class SetsAndRepsFormComponent implements OnInit {
  @Input() View: string = "";
  @Input() SelectedWorkout: WorkoutDetails = new WorkoutDetails();
  @Input() WeekViewMode: string = 'col-sm-12 col-md-6 col-lg-4 col-xl-3';
  @Input() FormViewMode: string = 'row sets-and-reps-form no-gutters';
  @Input() ShowTitle: boolean = true;
  @Input() newWorkoutTagItems: TagModel[] = [];
  @Output() CancelCallBack = new EventEmitter<boolean>();
  @Output() SaveCallBack = new EventEmitter<any>();
  public AllTags: TagModel[] = [];
  @Input() SetModalStyle: boolean = true;

  constructor(public tagService: TagService) {
  }

  ngOnInit() {
  }

  AddWeek(target: Week[]) {
    var newWeek = new Week();
    newWeek.Position = this.SelectedWorkout.TotalWorkout.length + 1;
    newWeek.SetsAndReps = [];
    this.AddSetToWeek(newWeek);
    target.push(newWeek);
  }

  RemoveWeek(target: Week[]) {
    if (target.length > 0) target.pop();
  }

  AddSetToWeek(targetWeek: Week) {
    var set = new Set();
    set.Position = targetWeek.SetsAndReps.length;
    targetWeek.SetsAndReps.push(set);
  }

  CheckAddAnotherSet(workout: WorkoutDetails, targetWeek: Week) {
    var removingASet = false;
    for (let i = 0; i < targetWeek.SetsAndReps.length - 1; i++)
    {
      let targetSet = targetWeek.SetsAndReps[i];

      if
      ( // @ts-ignore
        (!workout.ShowPercentageBox || (targetSet.Percent === undefined || targetSet.Percent === null)) &&
        // @ts-ignore
        (!workout.ShowSetsBox || (targetSet.Sets === undefined || targetSet.Sets === null || targetSet.Sets === '')) &&
        // @ts-ignore
        (!workout.ShowRepsBox || (targetSet.Reps === undefined || targetSet.Reps === null || targetSet.Reps === '')) &&
        // @ts-ignore
        (!workout.ShowTimeBox || ((targetSet.Minutes === undefined || targetSet.Minutes === null || targetSet.Minutes === '') && (targetSet.Seconds === undefined || targetSet.Seconds === null || targetSet.Seconds === ''))) &&
        // @ts-ignore
        (!workout.ShowDistanceBox || (targetSet.Distance === undefined || targetSet.Distance === null || targetSet.Distance === ''))
      ) {
        targetWeek.SetsAndReps.splice(i, 1);
        removingASet = true;
      }
    }

    if (!removingASet) {
      let lastSet = targetWeek.SetsAndReps[targetWeek.SetsAndReps.length - 1]

      if (
        !(workout.ShowPercentageBox && (lastSet.Percent === undefined || lastSet.Percent === null)) &&
        !(workout.ShowSetsBox && (lastSet.Sets === undefined || lastSet.Sets === null)) &&
        !(workout.ShowRepsBox && (lastSet.Reps === undefined || lastSet.Reps === null)) &&
        !(workout.ShowTimeBox && ((lastSet.Minutes === undefined || lastSet.Minutes === null) || (lastSet.Seconds === undefined || lastSet.Seconds === null))) &&
        !(workout.ShowDistanceBox && (lastSet.Distance === undefined || lastSet.Distance === null))
      ) {
        this.AddSetToWeek(targetWeek);
      }
    }
  }

  RemoveNewSetAndRepTags(s: TagModel) {
    var index = this.newWorkoutTagItems.findIndex(x => { return x.display == s.display });
    this.newWorkoutTagItems.splice(index, 1);
  }

  AddNewNewSetAndRepTags(s: TagModel) {
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Workout;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        var newTM = new TagModel();
        newTM.display = s.display;
        newTM.value = data;
        this.AllTags.push(newTM)
      });
    }
    this.newWorkoutTagItems.push(s);
  }

  Save(targetWorkout: WorkoutDetails, associatedTags: TagModel[]) {
    var data = {targetWorkout, associatedTags};
    this.SaveCallBack.emit(data);
  }

  Cancel() {
    this.CancelCallBack.emit(true)
  }
}
