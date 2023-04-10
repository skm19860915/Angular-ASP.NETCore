import { ITaggable } from '../../Interfaces/ITaggable';
import { Tag } from '../Tag';
export class Metric implements ITaggable {
    Id: number;
    Name: string;
    UnitOfMeasurementId: number = undefined;
    Tags: Tag[] = [];
    WeekIds :number[];
    CanModify : boolean = true;
}