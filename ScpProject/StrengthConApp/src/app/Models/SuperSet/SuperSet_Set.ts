export class SuperSet_Set {
    public Id: number;
    public Position: number =0;//the position in the order of sets
    public Percent: number ;
    public Sets: number ;
    public Reps: number ;
    public Weight: number ;
    public ParentSuperSetWeekId: number = 0;
    public Minutes:number;
    public Seconds: number;
    public Distance: number;
    public RepsAchieved : boolean;
    public Other: string;
    public ShowWeight: boolean;
}