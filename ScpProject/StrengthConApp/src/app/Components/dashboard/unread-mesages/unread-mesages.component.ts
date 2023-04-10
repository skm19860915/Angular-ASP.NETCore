import { Component, OnInit } from '@angular/core';
import { MessagePreview } from '../../../Models/Messages/MessagePreview';
import { ChatService } from '../../../Services/chat.service';

@Component({
  selector: 'app-unread-mesages',
  templateUrl: './unread-mesages.component.html',
  styleUrls: ['./unread-mesages.component.less']
})
export class UnreadMesagesComponent implements OnInit {

  public pageNumber: number = 0;
  public messageCount: number = 2;//its two instead of 3/4 because messages are broken up into two categoris
  //group and one-one. Meaning we take a messageCount of two for each category, so the total count could end up being 4.
  //two categories times the messageCount
  public OrderedMessagePreviews: any[] = [];
  public TotalMessageCount: number = 0;
  constructor(private chatService: ChatService) { }

  ngOnInit() {
    this.GetMessages();
  }
  AdvancePageNumber() {
    this.pageNumber++;
    this.GetMessages();
  }
  DecrementPageNumber() {
    if (this.pageNumber == 0) {
      return;
    }
    this.pageNumber--;
    this.GetMessages();
  }
  GetMessages() {
    this.chatService.GetAllUnreadMessage(this.pageNumber, this.messageCount).subscribe((x: MessagePreview) => {
      let allMessage: any[] = [];
      this.TotalMessageCount = x.TotalMessageCount;
      x.GroupUserMessages.forEach(element => allMessage.push(element));
      x.UserMessages.forEach(element => allMessage.push(element));
      this.OrderedMessagePreviews = allMessage.sort((a: any, b: any) => {
        return new Date(a.SentTime).getTime() - new Date(b.SentTime).getTime();
      });
    });

  }

}
