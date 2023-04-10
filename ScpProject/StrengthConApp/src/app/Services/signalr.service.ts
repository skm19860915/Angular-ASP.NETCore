
import { EventEmitter, Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { environment } from '../../environments/environment';
import { UserMessageResponse } from '../Models/Messages/UserMessageResponse';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { UserService } from './user.service';
import { GroupMessageResponse } from '../Models/Messages/GroupMessageResponse';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  private hubConnection: signalR.HubConnection;
  public signalReceived = new EventEmitter<SignalRMessage>();
  private connectionState: connectionState = connectionState.disconnected;
  private _headers;


  constructor(private http: HttpClient, private userService: UserService) {
    this.BuildConnection();


    this._headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true'
    });
  }

  public BuildConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalREndPoint)
      .build();
  }

  public StartConnection() {
    var token = this.userService.GetUserToken();
    if (this.connectionState === connectionState.disconnected && token !== '') {
      this.connectionState = connectionState.tryingToConnect;
      this.hubConnection
        .start()
        .then(() => {
          this.connectionState = connectionState.connected;
          this.RegisterSignalREvents();
        })
        .catch(err => {
          this.connectionState = connectionState.disconnected;

        });
    }
  }
  public SendUserMessageById(response: UserMessageResponse) {

    if (this.hubConnection) {
      var token = this.userService.GetUserToken();
      this.hubConnection.invoke('SendUserMessageById', response.MessageContent, token, response.DestinationUserId);
    }
  }
  public SendGroupMessageById(groupMessage: GroupMessageResponse) {
    if (this.hubConnection) {
      var token = this.userService.GetUserToken();
      this.hubConnection.invoke('SendGroupMessageById', groupMessage.MessageContent, token, groupMessage.MessageGroupId);
    }
  }
  public SendGroupMessageGroupTitle(groupMessage: GroupMessageResponse) {
    if (this.hubConnection) {
      var token = this.userService.GetUserToken();
      this.hubConnection.invoke('SendGroupMessageGroupTitle', groupMessage.MessageContent, token, groupMessage.MessageGroupTitle);
    }
  }
  public RespondSendGroupMessageGroupTitle(groupMessage: GroupMessageResponse) {
    if (this.hubConnection) {
      var token = this.userService.GetUserToken();
      this.hubConnection.invoke('RespondSendGroupMessageGroupTitle', groupMessage.MessageContent, token, groupMessage.MessageGroupTitle);
    }
  }
  private RegisterSignalREvents() {
    this.hubConnection.on("SignalMessageReceived", (data: SignalRMessage) => {
      this.signalReceived.emit(data);
    })
    this.hubConnection.on("newMessage", data => {
      this.signalReceived.emit(data);
    });
    this.hubConnection.on("RegisteredToNewGroup", data => {
      var token = this.userService.GetUserToken();
      this.http.post(environment.signalREndPoint + `RegisterUserToGroups?connectionId=${data.ConnectionId}&userToken=${token}`
        , {}, {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      }).subscribe(x => { this.signalReceived.emit(data); });
    });

    this.hubConnection.on("newConnection", data => {
      this.connectionState = connectionState.connected;
      var token = this.userService.GetUserToken();
      this.http.post(environment.signalREndPoint + `RegisterUserToGroups?connectionId=${data.ConnectionId}&userToken=${token}`
        , {}, {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      }).subscribe();
    })
  }

}


export class SignalRMessage {
  public Message: String;
  public SentFromUser: String;
  public SentFromuserId: string;
}
//cannot realy on hubconnection State because of asynch shit
enum connectionState {
  disconnected = 0,
  tryingToConnect = 1,
  connected = 2,
  connection = 3,
}
