import { Week } from "../Week";
import { ITaggable } from '../../Interfaces/ITaggable';
import { Tag } from '../Tag';

export class WorkoutDetails implements ITaggable {
    public Id: number;
    public Name: string;
    public Notes : string;
    public CreatedUserId: number;
    public CreatedDateTime: Date;
    public TotalWorkout: Week[] = [];
    public Rest : string;
    Tags: Tag[] = [];
    public CanModify : boolean = true;
    public ShowRepsAchievedBox: boolean = false;
    public ShowOtherBox: boolean = false;
    public ShowRestBox: boolean = false;
    public ShowDistanceBox: boolean = false;
    public ShowTimeBox: boolean = false;
    public ShowWeight: boolean = true;
    public ShowRepsBox: boolean = true;
    public ShowSetsBox: boolean = true;
    public ShowPercentageBox: boolean = true;
    public ShowAdvancedOptions: boolean = false;

}
