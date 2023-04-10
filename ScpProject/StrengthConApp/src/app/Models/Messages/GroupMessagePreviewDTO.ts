export class GroupMessagePreviewDTO {
    public MessageGroupTitle: string = '';
    public LastSentFirstName: string = '';
    public LastSentLastName: string = '';
    public MessageContent: string = '';
    public MessageGroupId: number = 0;
    public LastSentByUserId: number = 0;
    public MessageId: number = 0;
    public UserId: number = 0;
    public AthleteId: number = 0;
    public SentTime: Date;
    public Selected: false;
    public ViewerId: number = 0;
    public ReadOnly: boolean = false;
    public Pause: boolean = false;
    public SignalRGroupId : string = '';
}