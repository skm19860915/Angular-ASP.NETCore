import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { NewGroupMessage } from '../Models/Messages/NewGroupMessage';
import { NewUserMessage } from '../Models/Messages/NewUserMessage';
import { MessagePreview } from '../Models/Messages/MessagePreview';
import { MessageThread } from '../Models/Messages/MessageThread';
import { UserMessageResponse } from '../Models/Messages/UserMessageResponse';
import { GroupMessageResponse } from '../Models/Messages/GroupMessageResponse';


@Injectable({
  providedIn: 'root'
})
export class ChatService {

  private _headers;

  constructor(private http: HttpClient) {
    this._headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true'
    });
  }

  public SendGroupMessage(newMessage: NewGroupMessage) {
    return this.http.post<boolean>(environment.endpointURL + `/api/Messenger/CreateGroupMessage`
      , JSON.stringify(newMessage), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  public GroupNameTaken(groupTitle: string): Observable<boolean> {
    return this.http.post<boolean>(environment.endpointURL + `/api/Messenger/DoesGroupExist`
      , { GroupTitle: groupTitle }, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  public CreateNewGroupMessage(targetMessage: NewGroupMessage) {
    return this.http.post(environment.endpointURL + `/api/Messenger/CreateGroupMessage`
      , JSON.stringify(targetMessage), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  public CreateNewUserMessage(targetMessage: NewUserMessage) {
    return this.http.post(environment.endpointURL + `/api/Messenger/CreateNewUserMessage`
      , JSON.stringify(targetMessage), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  public GetMessagePreview() {
    return this.http.get<MessagePreview>(environment.endpointURL + `/api/Messenger/GetMessagePreview`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }

  public GetAllUnreadMessage(pageNumber :number, pageCount : number) {
    return this.http.get<MessagePreview>(environment.endpointURL + `/api/Messenger/GetAllUnreadMessage/${pageNumber}/${pageCount}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  public RespondGroupMessage(response: GroupMessageResponse) {
    return this.http.post(environment.endpointURL + `/api/Messenger/RespondToGroupMessage`
      , JSON.stringify(response), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  public RespondUserMessage(response: UserMessageResponse) {
    return this.http.post(environment.endpointURL + `/api/Messenger/RespondToUserMessage`
      , JSON.stringify(response), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  public ToggleMessagePause(pause: boolean, parentMessageId: number) {
    return this.http.post(environment.endpointURL + `/api/Messenger/TogglePauseMessage`
      , JSON.stringify({ PauseValue: pause, ParentMessageId: parentMessageId }), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  public DeleteGroupMessages(groupName: string, createdUserId: number) {
    return this.http.post(environment.endpointURL + `/api/Messenger/DeleteGroupMessages`
      , JSON.stringify({ GroupName: groupName, CreatedUserId: createdUserId }), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  public DeleteUserMessages(destinationUserId: number, createdUserId: number) {
    return this.http.post(environment.endpointURL + `/api/Messenger/DeleteUserMessages`
      , JSON.stringify({ DestinationUserId: destinationUserId, CreatedUserId: createdUserId }), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  public GetMessageThread(messageId: number) {
    return this.http.get<MessageThread[]>(environment.endpointURL + `/api/Messenger/GetMessageThread/${messageId}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
}
