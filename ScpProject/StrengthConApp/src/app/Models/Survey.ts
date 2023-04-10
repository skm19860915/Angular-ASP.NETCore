import { Question } from './Question';
import { Tag } from './Tag';


export class Survey {
    public Id: number;
    public Name: string;
    public Description: string;
    public Questions: Question[] = [];
    public Tags: Tag[] = [];
    public WeekIds: number[] = [];
    public CanModify: boolean = true;
}