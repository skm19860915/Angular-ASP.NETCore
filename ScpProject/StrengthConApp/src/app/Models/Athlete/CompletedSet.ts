export class CompletedSet {
    public Id: number;
    public Position: number;
    public Sets: number;
    public Reps: number;
    public Percent: number;
    public Weight: number;
    public AssignedProgramId: number;
    public OriginalSetId: number
    public AthleteId : number;
}

export class CompletedSuperSet {
    public Id: number;
    public Position: number;
    public Sets: number;
    public Reps: number;
    public Percent: number;
    public Weight: number;
    public AssignedProgramId: number;
    public OriginalSuperSet_SetId: number
    public AthleteId : number;
    public RepsAchieved : number; 
}