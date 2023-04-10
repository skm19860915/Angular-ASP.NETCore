export class UserNotification {
    public Id: number = 0;
    public Title: string = '';
    public Description: string = '';
    public Type: NotificationType;
    public URL: string = '';
    public GeneratedUserId: number;
    public SentDate: Date;
    public IsSnapShot: number;
}

enum NotificationType {
    NewProgram = 1,
    SurveyThreshold = 2,
}