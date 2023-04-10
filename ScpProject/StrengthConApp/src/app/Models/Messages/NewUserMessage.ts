export class NewUserMessage {
    public ParentMessageId: number = 0;
    public MessageContent: string;
    public UserToSendTo: number;
    public ReadOnly: boolean = false;
    public Pause: boolean = false;
}