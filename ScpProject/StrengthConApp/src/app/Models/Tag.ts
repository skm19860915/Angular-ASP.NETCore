export class Tag {
    public Id: number;
    public Name: string;
    public Type: TagType;
}

export enum TagType {
    Exercise = 1,
    Program = 2,
    /// <summary>
    /// sets and reps == workouts
    /// </summary>
    Workout = 3,
    Movie = 4,
    Athlete = 5,
    Survey = 6,
    Metric = 7,
    Document = 8
}
