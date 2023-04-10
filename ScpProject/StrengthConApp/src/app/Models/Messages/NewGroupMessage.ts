export class NewGroupMessage {
    public GroupTitle: string;
    public MessageContent: string;
    public UsersToSendTo: number[];
    public ReadOnly: boolean = false;
    public Pause: boolean = false;
}