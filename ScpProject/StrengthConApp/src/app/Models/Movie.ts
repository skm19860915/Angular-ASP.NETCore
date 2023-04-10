import { ITaggable } from './../Interfaces/ITaggable';
import { Tag } from './Tag';

export class Movie implements ITaggable  {
    public Id: number = 0;
    public URL: string = '';
    public Name : string = '';
    public CanModify : boolean = true;
    public IsDeleted : boolean = false;
    WeekIds :number[];
    Tags: Tag[] = [];
}