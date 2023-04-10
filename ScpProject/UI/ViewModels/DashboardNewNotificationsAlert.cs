using System.Collections.Generic;

namespace Controllers.ViewModels
{
    public class DashboardNewNotificationsAlert
    {
        public List<Models.Notifications.Notification> Notifications { get; set; }
        public int TotalNotificationCount { get; set; }
    }
}