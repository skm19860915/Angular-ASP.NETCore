import { ITaggable } from '../../Interfaces/ITaggable';
import { Tag } from '../Tag';

export class Workout implements ITaggable {
    public Id: number;
    public Name: string;
    Tags: Tag[] = [];
}
