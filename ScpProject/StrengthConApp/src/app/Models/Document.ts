import { ITaggable } from './../Interfaces/ITaggable';
import { Agreement } from './Agreement';
import { Tag } from './Tag';
export class Document implements ITaggable {
    Id: number;
    Description: string;
    Name: string;
    IsDeleted: boolean;
    Tags: Tag[] = [];
    Agreements: Agreement[] = [];
};
