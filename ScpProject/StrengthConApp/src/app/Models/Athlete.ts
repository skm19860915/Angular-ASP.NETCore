import { ITaggable } from './../Interfaces/ITaggable';
import { Tag } from './Tag';
export class Athlete implements ITaggable {
    public Id: number = 0;
    public FirstName: string;
    public LastName: string;
    public Name: string;
    public Email: string;
    public ProfilePicture: AthletePictures;
    public Weight: number;
    public HeightPrimary: number;
    public HeightSecondary: number;
    public Birthday: Date;
    Tags: Tag[] = [];
    public DisplayTags: DisplayTags[] = [];
    public Checked: boolean = false;//Lazy here. This hack is just to be used on the program check all. When we start cleaning up the code
    //this needs to be changed.
    public UserId: number;
    public ValidatedEmail: boolean;
}

export class AthletePictures {

    public Id: number;
    public URL: string;
    public FileName: string;
    public Thumbnail: string;
    public Profile: string;
}

export class DisplayTags {
    public display: string
    public value: number;
}