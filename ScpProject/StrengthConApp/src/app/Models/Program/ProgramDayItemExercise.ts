import {Week} from "../Week";
import {Exercise} from "../Exercise"
import { WorkoutDetails } from "../SetsAndReps/WorkoutDetails";
import { Workout } from "../SetsAndReps/Workout";

export class ProgramDayItemExercise{
    public Name: string;
    public Weeks: Week[];
    public SelectedSetRep: Week[];
    public SelectedExercise: Exercise;
    public SelectedWorkout : Workout;
    public Workout : WorkoutDetails;
  }