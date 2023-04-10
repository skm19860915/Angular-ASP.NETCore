import {Tag} from "../Models/Tag";

export interface ITaggable
{
    Tags: Tag[] ;
    Name:string;
    Id: number;
}