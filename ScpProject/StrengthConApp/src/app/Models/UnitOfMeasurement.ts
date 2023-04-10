import { ITaggable } from './../Interfaces/ITaggable';
import { Tag } from './Tag';

export class UnitOfMeasurement implements ITaggable {
    public Id: number;
    public UnitType: string = "";
    public Tags: Tag[] = [];//not used, need to refacter an Itaggable vs an INameable
    public Name: string = "";
}
