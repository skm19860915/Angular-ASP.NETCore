import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { NotificationService } from '../../Services/notification.service'
import { UserNotification } from '../../Models/Notification/UserNotification';
import { fadeInAnimation } from '../../animation/fadeIn';
import { AlertMessage } from '../../Models/AlertMessage';
import { interval } from 'rxjs/internal/observable/interval';
import { take } from 'rxjs/internal/operators/take';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.less'],
  animations: [fadeInAnimation]
})
export class NotificationsComponent implements OnInit {
  public NotificationLists: UserNotification[] = [];
  public AlertMessages: AlertMessage[] = [];
  public ShowViewedNotifcations: boolean = false;

  constructor(public notificationService: NotificationService) { }


  ngOnInit() {
    this.LoadNewNotifications();
  }
  public ToggleViewedNotification() {
    this.ShowViewedNotifcations = !this.ShowViewedNotifcations;
    if (this.ShowViewedNotifcations) {
      this.LoadViewedNotifications();

    }
    else {
      this.LoadNewNotifications();
    }
  }
  public LoadNewNotifications() {
    this.notificationService.GetAllNewNotifications().subscribe(x => {
      this.NotificationLists = x;
      this.NotificationLists.sort((a: any, b: any) => {
        return new Date(a.SentTime).getTime() - new Date(b.SentTime).getTime();
      });
    });
  }

  public LoadViewedNotifications() {
    this.notificationService.GetAllViewedNotifcations().subscribe(x => {
      this.NotificationLists = x;
      this.NotificationLists.sort((a: any, b: any) => {
        return new Date(a.SentTime).getTime() - new Date(b.SentTime).getTime();
      });
    });
  }

  public MarkAsViewed(notificationId: number) {
    this.notificationService.MarkNotificationAsRead(notificationId).subscribe(success => {
      this.DisplayMessage("Notification Archived Successfull", "Notification Was Archived", false)
      for (var i = 0; i < this.NotificationLists.length; i++) {
        if (this.NotificationLists[i].Id === notificationId) {
          this.NotificationLists.splice(i, 1);
        }
      }
    },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Notification Archived UNSUCCESSFULL", errorMessage, true)
      });

  }
  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
}
