import { GroupMessagePreviewDTO } from './GroupMessagePreviewDTO';
import { UserMessagePreviewDTO } from './UserMessagePreviewDTO';
export class MessagePreview {
    public GroupUserMessages: GroupMessagePreviewDTO[];
    public UserMessages: UserMessagePreviewDTO[];
    public TotalMessageCount: number;
}