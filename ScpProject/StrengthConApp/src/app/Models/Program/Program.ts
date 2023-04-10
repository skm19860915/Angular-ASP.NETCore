import { ITaggable } from './../../Interfaces/ITaggable';
import { Tag } from './../Tag';
import { Day } from './Day';
import { Metric } from '../Metric/Metric';

export class Program implements ITaggable {
    public Id: number =0;
    public Name: string ='';
    public Days: Day[]=[];
    public CurrentWorkoutWeekId: number;
    public CurrentWorkoutDayId: number;
    public WeekCount: number = 0;
    public CanModify: boolean = true;
    public IsDeleted: boolean = false;
    public DayCount : number = 0;
    public HasAdvancedOptions : boolean = false;
    Tags: Tag[] = [];
}


export class ProgramDTO implements ITaggable {
    public Id: number;
    public Name: string;
    public Days: ProgramDayDTO[];
    public WeekCount: number;
    Tags: Tag[] = [];
}

export class ProgramDayDTO {
    public Id: number;
    public Position: number;
    public Exercises: ProgramDayItemExerciseDTO[];
    public Metrics: ProgramMetricDTO[];
    public Surveys: ProgramSurveyDTO[];
    public Notes: ProgramDayItemNoteDTO[];
    public Videos: ProgramDayItemVideoDTO[];
    public SuperSets: any[] = [];//for this dto I mimmicked whats being used in the VIEW so no need to convert
}

export class ProgramDayItemVideoDTO {
    public MovieId: number;
    public Position: number;
    public DisplayWeeks: number[] = [];
}
export class ProgramSurveyDTO {
    public SurveyId: number;
    public Position: number;
    public DisplayWeeks: number[] = [];
}
export class ProgramMetricDTO {
    public MetricId: number;
    public Position: number;
    public DisplayWeeks: number[] = [];
}
export class ProgramDayItemNoteDTO {
    public Name: string;
    public Note: string;
    public Position: number;
    public DisplayWeeks: number[] = [];
}

export class ProgramDayItemExerciseDTO {
    public ExerciseId: number;
    public WorkoutId: number;
    public Position: number;
    public Weeks: ProgramWeekDTO[];
}

export class ProgramWeekDTO {
    public Id: number;
    public Position: number;
    public SetsAndReps: ProgramSetDTO[];
}

export class ProgramSetDTO {
    public Id: number;
    public Position: number;
    public Sets: number;
    public Reps: number;
    public Percent: number;
    public Weight: number;
}
export class ProgramSuperSetNoteDTO{
    public Note : string;
    public Position: number;
    public DisplayWeeks: number[] = [];
}