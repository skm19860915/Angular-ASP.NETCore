import { ProgramDayItemEnum } from "./ProgramDayItemEnum";
import { CompletedDay } from "./CompletedDay";

export class AssignedProgram {
    public CurrentWorkoutDayId: number;
    public CurrentWorkoutWeekId: number;
    public Days: AssignedDays[] = [];
    public Id: number;
    public Name: string;
    public WeekCount: number;
    public AthleteId: number;
    public CompletedDays: CompletedDay[];
    public IsSnapShot: boolean;
}

export class AssignedDays {
    public Id: number;
    public Position: number;
    public AssignedProgramDayItem: AssignedProgramDayItem[] = [];
    public IsActive: boolean = false;

}

export class AssignedProgramDayItem {
    public Id: number;
    public ItemType: ProgramDayItemEnum;
    public Position: number;
    public ProgramItem: any = {};
}

export class AssignedSurveys {
    public DisplayWeeks: number[];
    public ProgramDayId: number;
    public ProgramDayItemSurveyId: number;
    public SurveyId: number;
    public SurveyName: string;
    public Questions: AssignedQuestion[] = [];
    public ItemType: number;
}

export class AssignedQuestion {
    public Answer: string;
    public AssignedProgramId: number;
    public DisplayWeekId: number;
    public ProgramId: number;
    public QuestionDisplayText: string;
    public QuestionId: number;
    public QuestionTypeId: number;
}

export class AssignedMetric {
    public AssignedProgramId: number
    public CompletedWeight: number;
    public MetricId: number;
    public MetricName: string;
    public Position: number;
    public ProgramDayId: number;
    public DisplayWeekId: number;
    public ProgramDayItemMetricId: number;
}
export class AssignedVideo {
    public AssignedProgramId: number
    public MovieId: number;
    public MovieName: string;
    public Position: number;
    public ProgramDayId: number;
    public DisplayWeekId: number;
    public ProgramDayItemMovieId: number;
    public MovieURL: string;
}
export class AssignedExercise {
    public Name: string;
    public ProgramDayItemPosition: number;
    public ProgramDayId: number;
    public AssignedSetsAndReps: AssignedSetRep[] = [];
}

export class AssignedSetRep {
    public AssignedProgramId: number;
    public AssignedWorkoutPercent: number;
    public AssignedReps: number;
    public AssignedSets: number;
    public AssignedWeight: number;
    public CompletedSetPercent: number
    public CompletedSetSets: number;
    public CompletedSetWeight: number;
    public PercentMaxCalcSubPercent: number;
    public PercentMaxCalc: number;
    public OriginalSetId: number;
    public PositionInSet: number;
    public SetWeekId: number;
    public AthleteId: number;
}



export class AssignedSuperSet {
    public SuperSetId: number;
    public Exercises: AssignedSuperSetExercise[] = [];
    public ProgramDayItemPosition: number;
}
export class AssignedSuperSetNote {
    public Id: number;
    public Note: string;
    public Position: number;
    public ProgramDayItemSuperSetId: number;
}

export class AssignedNote {
    public AssignedProgramId: number;
    public ProgramDayItemNoteId: number;
    public ProgramDayId: number;
    public Position: number;
    public Name: string;
    public NoteText: string;
    public DisplayWeekId: number;
}

export class AssignedSuperSetExercise {
    public Name: string;
    public PositionInSuperSet: number;
    public ProgramDayId: number;
    public SetsAndReps: AssignedSuperSet_SetRep[] = [];
    public SuperSetId: number;
    public VideoURL: string;
    public VideoProvider: VideoHoster = 0;
    public Rest: string;
    public ShowTime: boolean = false;
    public ShowReps: boolean = false;
    public ShowWeight: boolean = false;
    public ShowDistance: boolean = false;
    public ShowRest: boolean = false;
    public ShowRepsAchieved: boolean = false;
    public ShowSets : boolean = false;


}
export enum VideoHoster {
    none = 0,
    youtube = 1,
    vimeo = 2
}

export class AssignedSuperSet_SetRep {
    public AssignedProgramId: number;
    public AssignedWorkoutPercent: number;
    public AssignedWorkoutReps: number;
    public AssignedWorkoutSets: number;
    public AssignedWorkoutWeight: number;
    public CompletedRepsAchieved: number;
    public CompletedSetPercent: number
    public CompletedSetSets: number;
    public CompletedSetWeight: number;
    public PercentMaxCalcSubPercent: number;
    public PercentMaxCalc: number;
    public OriginalSuperSetSetId: number;
    public PositionInSet: number;
    public SuperSetWeekId: number;
    public AthleteId: number;
    public WeekPosition: number;
    public AssignedRest: number;
    public AssignedWorkoutDistance: number;
    public AssignedWorkoutMinutes: number;
    public AssignedWorkoutSeconds: number;
    public AssignedOther: number;
    public RepsAchieved: boolean;


}