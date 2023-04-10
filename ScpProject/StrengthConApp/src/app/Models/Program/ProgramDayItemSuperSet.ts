import { SuperSet_Week } from "../SuperSet/SuperSet_Week";
import { Exercise } from "../Exercise"
import { WorkoutDetails } from "../SetsAndReps/WorkoutDetails";
import { Workout } from "../SetsAndReps/Workout";

export class ProgramDayItemSuperSet {
  public Exercises: SuperSet_Exercise[] = [];
  public Notes: SuperSet_Note[] = [];
  public SelectedExerciseId: number;
  public SuperSetDisplayTitle: string = "";
}

export class SuperSet_Note {
  public Id: number;
  public Note: string = "";
  public WeekIds: number[] = [];
  public Position: number = 0;
}

export class SuperSet_Exercise {
  public ExerciseId: number;
  public Name: string;
  public Weeks: SuperSet_Week[] = [];
  public Position: number;
  public SelectedSetRep: SuperSet_Week[];
  public SelectedWorkout: WorkoutDetails = new WorkoutDetails();
  public SelectedExercise: Exercise = new Exercise();
  public SelectedExerciseID: number
  public Id: number;
  public Rest: string;
  public ShowTime: boolean = false;
  public ShowReps: boolean = false;
  public ShowWeight: boolean = false;
  public ShowDistance: boolean = false;
  public ShowRest: boolean = false;
  public ShowRepsAchieved: boolean = false;
  public ShowSets : boolean = false;

}
