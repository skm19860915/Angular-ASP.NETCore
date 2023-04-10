export class AssistantCoach {
    public Id: number = 0;
    public FirstName: string;
    public LastName: string;
    public Name: string;
    public Email: string;
    public ProfilePicture: AssistantCoachPicture;
}

export class AssistantCoachPicture {

    public Id: number;
    public URL: string;
    public FileName: string;
    public Thumbnail: string;
    public Profile: string;
}

export class Role {

    public Id: number;
    public Name: string;
}

export class BackendAssistantCoach {
    public Id: number = 0;// This is the userId
    public FirstName: string;
    public LastName: string;
    public Roles: Role[] = [];
    public IsDeleted: boolean;
    public Selected: boolean;
    public FullName: string;
}