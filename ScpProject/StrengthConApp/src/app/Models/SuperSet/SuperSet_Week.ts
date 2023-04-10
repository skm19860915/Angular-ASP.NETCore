import {  SuperSet_Set } from "./SuperSet_Set";

export class SuperSet_Week {
    public Id : number = 0;
    public Position: number = 0; //Week number
    public ParentWorkoutId: number = 0;
    public SetsAndReps: SuperSet_Set[] = [];
}