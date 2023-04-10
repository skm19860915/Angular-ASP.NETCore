import { ITaggable } from './../Interfaces/ITaggable';
import { Tag } from './Tag';
export class Exercise implements ITaggable {
    Id: number;
    Notes: string;
    Name: string;
    CreatedUserId: number;
    IsDeleted: boolean;
    Percent: number;
    PercentMetricCalculationId: number;
    Tags: Tag[] = [];
    CanModify: boolean = true;
    VideoURL: string = '';
    CalcMetricName: string = '';
};