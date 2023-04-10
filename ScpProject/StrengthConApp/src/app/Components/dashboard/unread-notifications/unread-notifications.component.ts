import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../../../Services/notification.service';
import { UserNotification } from '../../../Models/Notification/UserNotification'

@Component({
  selector: 'app-unread-notifications',
  templateUrl: './unread-notifications.component.html',
  styleUrls: ['./unread-notifications.component.less']
})
export class UnreadNotificationsComponent implements OnInit {
  public NotificationList: UserNotification[] = [];
  public pageNumber: number = 0;
  public notificationCount: number = 3;
  public TotalNotificationCount : number =0;
  constructor(public notificationService: NotificationService) { }

  ngOnInit() {
    this.LoadNewNotifications();
  }
  AdvancePage() {
    this.pageNumber++;
    this.LoadNewNotifications();
  }
  DecrementPage() {
    if (this.pageNumber === 0) return;
    this.pageNumber--;
    this.LoadNewNotifications();
  }
  public LoadNewNotifications() {
    this.notificationService.GetAllPaginatedNewNotifications(this.pageNumber, this.notificationCount).subscribe(x => {
      this.NotificationList = x.Notifications;
      this.TotalNotificationCount = x.TotalNotificationCount;

      this.NotificationList.sort((a: any, b: any) => {
        return new Date(a.SentTime).getTime() - new Date(b.SentTime).getTime();
      });
    });
  }

}
