import { Notification } from "rxjs";

import { UserNotification } from './UserNotification';

export class DashboardNotification {
    public Notifications: UserNotification[];
    public TotalNotificationCount: number;
}