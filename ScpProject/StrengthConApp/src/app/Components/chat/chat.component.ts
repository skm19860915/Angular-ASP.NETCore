import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AlertMessage } from '../../Models/AlertMessage';
import { interval } from 'rxjs';
import { take } from 'rxjs/operators';
import { RosterService } from '../../Services/roster.service'
import { Athlete } from '../../Models/Athlete';
import { ExcludeTagFilterPipe, TagFilterPipe, SearchTaggableFilterPipe, ArraySortPipe } from '../../Pipes';
import { TagModel } from '../../Models/TagModel';
import { TagService } from '../../Services/tag-service.service';
import { Tag, TagType } from '../../Models/Tag';
import { fadeInAnimation } from '../../animation/fadeIn';
import { OrganizationService } from '../../Services/organization.service';
import { BackendAssistantCoach } from '../../Models/AssistantCoach';
import { ChatService } from '../../Services/chat.service';
import { NewGroupMessage } from '../../Models/Messages/NewGroupMessage';
import { read } from 'fs';
import { NewUserMessage } from '../../Models/Messages/NewUserMessage';
import { MessagePreview } from '../../Models/Messages/MessagePreview';
import { MessageThread } from '../../Models/Messages/MessageThread';
import { UserMessageResponse } from '../../Models/Messages/UserMessageResponse';
import { GroupMessageResponse } from '../../Models/Messages/GroupMessageResponse';
import { UserService } from '../../Services/user.service';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { SignalrService } from '../../Services/signalr.service';



@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.less'],
  animations: [fadeInAnimation]
})
export class ChatComponent implements OnInit {
  public AlertMessages: AlertMessage[] = [];
  public ShowCreateMessageModel: boolean = false;
  public CheckedAthletes: number[] = [];
  public AllAthletes: Athlete[];
  public UnmodifiedAthetes: Athlete[] = [];
  public AthleteFilterTags: TagModel[] = [];
  public AllChecked: boolean = false;
  public NewMessageReadOnly: boolean = false;
  public NewMessagePause: boolean = false;
  public ViewAthletes: boolean = true;//if this is false then we are viewing coaches
  public Coaches: BackendAssistantCoach[] = [];
  public NewMessageContent: string = "";
  public CoachSearchString: string = "";
  public AllCoaches: BackendAssistantCoach[] = [];
  public AllCheckedCoaches: boolean;
  public CheckedCoaches: number[] = [];
  public NewMessageGroupTitle: string = "";
  public GroupAlreadyExists: boolean = false;
  public ShowMessageWindow: boolean = false;
  public OrderedMessagePreviews: any[] = [];
  public OrderedUserMessageThread: MessageThread[] = [];
  public SelectedMessage: any = {};
  public NewResponse: string = '';
  public IsCoach: boolean = false;
  public ShowDeleteConfirmationMenu: boolean = false;
  public DeleteSelectedMessage: any = {};
  public messageDeleteConfirmation : '';
  public WYSIWYGConfig : AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: 'auto',
    minHeight: '5rem',
    maxHeight: 'auto',
    width: 'auto',
    minWidth: '0',
    placeholder: 'Enter text here...',
    defaultParagraphSeparator: '',
    defaultFontName: '',
    defaultFontSize: '',
    fonts: [
      { class: 'arial', name: 'Arial' },
      { class: 'times-new-roman', name: 'Times New Roman' },
      { class: 'calibri', name: 'Calibri' },
      { class: 'comic-sans-ms', name: 'Comic Sans MS' }
    ],
    sanitize: true,
    toolbarPosition: 'top'
  };//hacking the fact that we cannot use the toolBarHiddon method. look for .editor ::ng-deep insertVideo. Its a css class that I am using to maks the button



  constructor(public signalRService: SignalrService,  public userService: UserService, public chatService: ChatService, public orgSerivce: OrganizationService, public RosterService: RosterService, public ExcludedTagFilterPipe: ExcludeTagFilterPipe, public tagService: TagService, public TagFilterPipe: TagFilterPipe) { }

  ngOnInit() {
    this.IsCoach = this.userService.IsCoach();
    this.ViewAthletes = this.IsCoach;
    this.GetAllAthletes();
    this.GetAllCoaches();
    this.GetMessagePreview();
    this.signalRService.signalReceived.subscribe(x => {
      this.GetMessagePreview();
    })
  }

  DisplayChatMessage(targetMessage: any) {

    this.OrderedMessagePreviews.forEach(x => x.Selected = false);
    targetMessage.Selected = true;
    this.SelectedMessage = targetMessage;
    this.chatService.GetMessageThread(targetMessage.MessageId).subscribe((x: MessageThread[]) => {
      this.OrderedUserMessageThread = x.sort((a: any, b: any) => {
        return new Date(a.SentTime).getTime() - new Date(b.SentTime).getTime();
      });
    });
  }

  GetMessagePreview() {
    this.chatService.GetMessagePreview().subscribe((x: MessagePreview) => {
      let allMessage: any[] = [];
      x.GroupUserMessages.forEach(element => allMessage.push(element));
      x.UserMessages.forEach(element => allMessage.push(element));
      this.OrderedMessagePreviews = allMessage.sort((a: any, b: any) => {
        return new Date(a.SentTime).getTime() - new Date(b.SentTime).getTime();
      });
      if (this.OrderedMessagePreviews.length > 0) {
        this.OrderedMessagePreviews[0].Selected = true;
        this.DisplayChatMessage(this.OrderedMessagePreviews[0])
      }
    });
  }

  GetAllAthletes() {
    this.RosterService.GetAllAthletes().subscribe(x => {
      x.forEach(y => y.Name = y.FirstName + ' ' + y.LastName) //need to rig up the seach
      this.AllAthletes = x;
      this.UnmodifiedAthetes = x;
    });
  }

  GetAllCoaches() {
    this.orgSerivce.GetAllCoachesButSelf().subscribe(success => {
      this.Coaches = success;
    });
  }
  ToggleViewAthletesWindow() {
    this.ViewAthletes = !this.ViewAthletes;
  }
  ToggleNewMessageReadOnly() {
    this.NewMessageReadOnly = !this.NewMessageReadOnly;
  }
  ToggleNewMessagePause() {
    this.NewMessagePause = !this.NewMessagePause;
  }
  ToggleCreateMessageModel() {
    this.ShowCreateMessageModel = !this.ShowCreateMessageModel;
    if (!this.ShowCreateMessageModel) {
      this.CheckedAthletes = [];
      this.CheckedCoaches = [];
      this.NewMessageContent = '';
      this.NewMessagePause = false;
      this.NewMessageReadOnly = false;
      this.NewMessageGroupTitle = '';
      this.AllAthletes.forEach(x => x.Checked = false);
      this.Coaches.forEach(x => x.Selected = false);
      this.ShowMessageWindow = false;
    }
  }
  public ToggleAthleteIdInList(athleteUserId: number, athlete) {
    var index = this.CheckedAthletes.findIndex(x => { return x == athleteUserId });
    // if the index is found then the item is already in the array, meaning they are untoggleing
    //if it isnt found it means the are toggling
    if (index != -1) {
      this.CheckedAthletes.splice(index, 1);
      athlete.Checked = false;
    }
    else {
      athlete.Checked = true;
      this.CheckedAthletes.push(athleteUserId);
    }
  }
  AthleteExcludeRemoveTag(s: TagModel) {
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    var index = this.AthleteFilterTags.findIndex(x => { return x.display == s.display });// this.TagItems.findIndex(x => x.display == s.display);
    this.AthleteFilterTags.splice(index, 1);
    this.AllAthletes = this.ExcludedTagFilterPipe.transform(this.UnmodifiedAthetes, this.AthleteFilterTags)
  }
  AthleteExcludeAddTag(s: TagModel) {
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.AthleteFilterTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Athlete;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        var newTag = new TagModel()
        newTag.value = data;
      });
    }
    this.AthleteFilterTags.push(s);
    this.AllAthletes = this.ExcludedTagFilterPipe.transform(this.UnmodifiedAthetes, this.AthleteFilterTags)
  };
  AthleteIncludeRemoveTag(s: TagModel) {
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    var index = this.AthleteFilterTags.findIndex(x => { return x.display == s.display });// this.TagItems.findIndex(x => x.display == s.display);
    this.AthleteFilterTags.splice(index, 1);
    this.AllAthletes = this.TagFilterPipe.transform(this.UnmodifiedAthetes, this.AthleteFilterTags)
  }
  AthleteInculdeAddTag(s: TagModel) {
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.AthleteFilterTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Athlete;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {

        newTag.value = data;
      });
    }
    this.AthleteFilterTags.push(s);
    this.AllAthletes = this.TagFilterPipe.transform(this.UnmodifiedAthetes, this.AthleteFilterTags)
  };
  public CheckAllAthletes() {
    this.AllChecked = !this.AllChecked;
    if (this.AllChecked) {
      this.CheckedAthletes = [];//wipe out all the checked athletes, then toggle them. The toggle will see they are not in there and add
      this.AllAthletes.forEach(a => {
        this.ToggleAthleteIdInList(a.UserId, a);
      });
    }
    else {
      this.AllAthletes.forEach(z => { z.Checked = false })
      this.CheckedAthletes = []
    }
  };
  public ToggleCheckCoach(targetCoach: BackendAssistantCoach) {
    if (targetCoach.Selected === undefined || targetCoach.Selected === null || !targetCoach.Selected) {
      targetCoach.Selected = true;
      this.CheckedCoaches.push(targetCoach.Id);
    }
    else {
      var index = this.CheckedCoaches.findIndex(x => x === targetCoach.Id);
      this.CheckedCoaches.splice(index, 1)
      targetCoach.Selected = false;
    }

  }
  public CheckAllCoaches() {
    this.AllCheckedCoaches = !this.AllCheckedCoaches;
    if (this.AllCheckedCoaches) {
      this.CheckedCoaches = [];
      this.Coaches.forEach(a => {
        a.Selected = true;
        this.CheckedCoaches.push(a.Id);
      });
    }
    else {
      this.CheckedCoaches = [];
      this.Coaches.forEach(a => {
        a.Selected = false;
      });
    }
  }
  DoesGroupExist(groupName) {
    this.chatService.GroupNameTaken(groupName).subscribe((success: boolean) => this.GroupAlreadyExists = success);

  }
  SendGroupMessage(athleteIds: number[], coachIds: number[], content, readOnly, pause, groupTitle) {
    let newMessage = new NewGroupMessage();
    newMessage.GroupTitle = groupTitle;
    newMessage.MessageContent = content;
    coachIds.forEach(x => athleteIds.push(x));
    newMessage.UsersToSendTo = athleteIds;
    newMessage.ReadOnly = readOnly;
    newMessage.Pause = pause;
    let response: GroupMessageResponse = { MessageContent: content, MessageGroupId: 0, ParentMessageId: 0, MessageGroupTitle: groupTitle };
    this.chatService.CreateNewGroupMessage(newMessage).subscribe(x => {
      this.DisplayMessage("Message Sent", "Group And Message Sent", false)
      this.GetMessagePreview();
      this.signalRService.SendGroupMessageGroupTitle(response);
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage("Message Not Sent", errorMessage, true)
    });
    this.ToggleCreateMessageModel();
  }
  SendUserMessage(athleteIds: number[], coachIds: number[], content, readOnly, pause) {
    if (athleteIds.length > 1) {
      this.DisplayMessage("Error occured", " You have multiple Athletes being sent info via SendUserMessage", true)
    }
    if (coachIds.length > 1) {
      this.DisplayMessage("Error occured", " You have multiple Coaches being sent info via SendUserMessage", true)
    }
    if (coachIds.length + athleteIds.length > 1) {
      this.DisplayMessage("Error occured", " You have multiple Users being sent info via SendUserMessage", true)
    }
    var wtf = athleteIds.length === 1 ? athleteIds[0] : coachIds[0];//the checks should make sure that at most we have 1 element in either array
    let newMessage = new NewUserMessage();
    newMessage.MessageContent = content;
    newMessage.UserToSendTo = wtf;
    newMessage.ReadOnly = readOnly;
    newMessage.Pause = pause;
    let newSignRMessage: UserMessageResponse = { MessageContent: content, DestinationUserId: wtf, ParentMessageId: 0 };
    this.chatService.CreateNewUserMessage(newMessage).subscribe(x => {
      this.DisplayMessage("Message Sent", "Group And Message Sent", false)
      this.GetMessagePreview();
      this.signalRService.SendUserMessageById(newSignRMessage)
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage("Message Not Sent", error, true)
    });
    this.ToggleCreateMessageModel();
  }

  RespondGroupMessage(messageContent, groupMessageId, parentMessageId, groupTitle) {
    let response: GroupMessageResponse = { MessageContent: messageContent, MessageGroupId: groupMessageId, ParentMessageId: parentMessageId, MessageGroupTitle: groupTitle };
    this.chatService.RespondGroupMessage(response).subscribe(sucess => {
      this.NewResponse = '';
      this.chatService.GetMessageThread(parentMessageId).subscribe((x: MessageThread[]) => {
        this.signalRService.RespondSendGroupMessageGroupTitle(response);
        this.OrderedUserMessageThread = x.sort((a: any, b: any) => {
          return new Date(a.SentTime).getTime() - new Date(b.SentTime).getTime();
        });
      });
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage("Message Not Sent", error, true)
    });
  }
  RespondUserMessage(messageContent, destinationUserId, parentMessageId) {
    let response: UserMessageResponse = { MessageContent: messageContent, DestinationUserId: destinationUserId, ParentMessageId: parentMessageId };
    this.chatService.RespondUserMessage(response).subscribe(sucess => {
      this.NewResponse = '';
      this.chatService.GetMessageThread(parentMessageId).subscribe((x: MessageThread[]) => {
        this.signalRService.SendUserMessageById(response)
        this.OrderedUserMessageThread = x.sort((a: any, b: any) => {
          return new Date(a.SentTime).getTime() - new Date(b.SentTime).getTime();
        });
      });
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage("Message Not Sent", error, true)
    });
  }
  TogglePause(message: any) {
    message.Pause = !message.Pause
    this.chatService.ToggleMessagePause(message.Pause, message.MessageId).subscribe(success => {
      this.GetMessagePreview();
      this.DisplayMessage(`Message ${message.Pause ? ' Paused ' : 'UnPaused'}`, `Message ${message.Pause ? ' Paused ' : 'UnPaused'}`, false);
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage(`Message ${message.Pause ? ' Paused ' : 'UnPaused'}`, errorMessage, true)
    })
  }
  DeleteUserMessages(message: any) {
    this.chatService.DeleteUserMessages(message.DestinationUserId,message.UserId).subscribe(success => {
      this.DisplayMessage("Message Deleted", "Messages Deleted", false);
      this.GetMessagePreview();
      this.ShowDeleteConfirmationMenu = false;
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage("Message Not Deleted", errorMessage, true)
    });
  }

  DeleteGroupMessages(message: any) {
    this.chatService.DeleteGroupMessages(message.MessageGroupTitle, message.UserId).subscribe(success => {
      this.DisplayMessage("Message Deleted", "Messages Deleted", false);
      this.GetMessagePreview();
      this.ShowDeleteConfirmationMenu = false;
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage("Message Not Deleted", errorMessage, true)
    });
  }

  ShowDeleteConfirmation(message: any) {
    this.ShowDeleteConfirmationMenu = true;

    this.DeleteSelectedMessage = message;
  }
  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }

}
