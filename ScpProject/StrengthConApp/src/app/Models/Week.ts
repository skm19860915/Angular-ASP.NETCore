import { Set } from "./SetsAndReps/Set";

export class Week {
    public Id : number = 0;
    public Position: number = 0; //Week number
    public ParentWorkoutId: number = 0;
    public SetsAndReps: Set[] = [];
}