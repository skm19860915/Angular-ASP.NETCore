import { ITaggable } from './../../Interfaces/ITaggable';
import { Tag } from './../Tag';

export class Movie implements ITaggable {
    Id: number;
    Name: string;
    IsDeleted: boolean;
    CanModify: boolean;
    Url: string;
    CreatedUserId: number;
    WeekIds :number[];
    Tags: Tag[] = [];
}