import { Component, Input, OnInit } from '@angular/core';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { WorkoutDetails } from 'src/app/Models/SetsAndReps/WorkoutDetails';
import { Set } from '../../../Models/SetsAndReps/Set'

@Component({
  selector: 'app-sets-and-reps-table',
  templateUrl: './sets-and-reps-table.component.html',
  styleUrls: ['./sets-and-reps-table.component.less'],
  animations: [fadeInAnimation]
})
export class SetsAndRepsTableComponent implements OnInit {
  @Input() subSelectedWorkout: WorkoutDetails = new WorkoutDetails();
  @Input() eId: number = 0;
  @Input() dId: number = 0;

  constructor() { }

  ngOnInit(): void {
  }

  ToggleAdvancedOptions(s: WorkoutDetails) {
    s.ShowAdvancedOptions = !s.ShowAdvancedOptions;
  }

  TogglePercentageBox(s: WorkoutDetails) {
    s.ShowPercentageBox = !s.ShowPercentageBox;
    if (!s.ShowPercentageBox) {
      this.BlankOutSetsAndRepsOptions(s, function (targetSet: Set) {
        targetSet.Percent = null;
      });
    }
  }

  ToggleSetsBox(s: WorkoutDetails) {
    s.ShowSetsBox = !s.ShowSetsBox;
    if (!s.ShowSetsBox) {
      this.BlankOutSetsAndRepsOptions(s, function (targetSet: Set) {
        targetSet.Sets = null;
      });
    }
  }

  ToggleRepsBox(s: WorkoutDetails) {
    s.ShowRepsBox = !s.ShowRepsBox;
    if (!s.ShowRepsBox) {
      this.BlankOutSetsAndRepsOptions(s, function (targetSet: Set) {
        targetSet.Reps = null;
      });
    }
  }

  ToggleWeightBox(s: WorkoutDetails) {
    s.ShowWeight = !s.ShowWeight;
    if (!s.ShowWeight) {
      this.BlankOutSetsAndRepsOptions(s, function (targetSet: Set) {
        targetSet.Weight = null;
      });
    }
  }

  ToggleTimeBox(s: WorkoutDetails) {
    s.ShowTimeBox = !s.ShowTimeBox;
    if (!s.ShowTimeBox) {
      this.BlankOutSetsAndRepsOptions(s, function (targetSet: Set) {
        targetSet.Minutes = null;
        targetSet.Seconds = null;
      });
    }
  }

  ToggleDistanceBox(s: WorkoutDetails) {
    s.ShowDistanceBox = !s.ShowDistanceBox;
    if (!s.ShowDistanceBox) {
      this.BlankOutSetsAndRepsOptions(s, function (targetSet: Set) {
        targetSet.Distance = null;
      });
    }
  }

  ToggleShowOtherBox(s: WorkoutDetails) {
    s.ShowOtherBox = !s.ShowOtherBox;
    if (!s.ShowOtherBox) {
      this.BlankOutSetsAndRepsOptions(s, function (targetSet: Set) {
        targetSet.Other = null;
      });
    }
  }

  ToggleRestBox(s: WorkoutDetails) {
    s.ShowRestBox = !s.ShowRestBox;
    if (!s.ShowRestBox) {
      s.Rest = null;
    }
  }

  ToggleRepsAchieved(s: WorkoutDetails) {
    debugger;
    s.ShowRepsAchievedBox = !s.ShowRepsAchievedBox;
    this.BlankOutSetsAndRepsOptions(s, function (targetSet: Set) {
      targetSet.RepsAchieved = s.ShowRepsAchievedBox;
    });
  }

  BlankOutSetsAndRepsOptions(s: WorkoutDetails, clearOutFunction) {
    s.TotalWorkout.forEach(x => {
      x.SetsAndReps.forEach(y => {
        clearOutFunction(y);
      })
    })
  }
}
