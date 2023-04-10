using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Repositories;
using Models.Notifications;

namespace BL
{
    public interface INotificationManager
    {
        Tuple<List<Notification>, int> GetAllNewNotifications(Guid userToken, int pageNumber = 0, int notificationCount = 0);
        List<Notification> GetAllViewedNotifications(Guid userToken);
        bool HasUnreadNotifications(Guid userToken);
        void MarkNotificationAsRead(int notificationid, Guid userToken);
    }

    public class NotificationManager : INotificationManager
    {
        private INotificationRepo _notificationRepo { get; set; }
        private IUserRepo _userRepo { get; set; }
        public NotificationManager(INotificationRepo notificationRepo, IUserRepo userRepo)
        {
            _notificationRepo = notificationRepo;
            _userRepo = userRepo;

        }

        public List<Models.Notifications.Notification> GetAllViewedNotifications(Guid userToken)
        {
            var user = _userRepo.Get(userToken);
            return _notificationRepo.GetAllViewedNotifications(user.Id);
        }
        public Tuple<List<Models.Notifications.Notification>, int> GetAllNewNotifications(Guid userToken, int pageNumber = 0, int notificationCount = 0)
        {
            var user = _userRepo.Get(userToken);
            var allNotifcations = _notificationRepo.GetAllNewNotifications(user.Id); ;
            if (notificationCount > 0)
            {
                return new Tuple<List<Models.Notifications.Notification>, int>(
                        allNotifcations.OrderByDescending(x => x.SentDate).Skip(pageNumber * notificationCount).Take(notificationCount).ToList(),
                        allNotifcations.Count()
                        );

            }
            return new Tuple<List<Models.Notifications.Notification>, int>(
                        allNotifcations.OrderByDescending(x => x.SentDate).ToList(),
                        allNotifcations.Count()
                        );
        }
        public void MarkNotificationAsRead(int notificationid, Guid userToken)
        {
            var user = _userRepo.Get(userToken);
            _notificationRepo.MarkNotificationAsViewed(notificationid, user.Id);
        }
        public bool HasUnreadNotifications(Guid userToken)
        {
            var user = _userRepo.Get(userToken);
            return _notificationRepo.HasUnreadNotifications(user.Id);
        }
    }
}
