export class Set {
    public Id: number;
    public Position: number;//the position in the order of sets
    public Percent: number;
    public Sets: number;
    public Reps: number;
    public Weight: number;
    public Minutes:number;
    public Seconds: number;
    public Distance: number;
    public RepsAchieved : boolean;
    public Other: string;
    public ParentWeekId: number = 0;
}