using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Models.Notifications;

namespace DAL.Repositories
{
    public interface INotificationRepo
    {
        List<Notification> GetAllNewNotifications(int targetUserId);
        List<Notification> GetAllViewedNotifications(int targetUserId);
        bool HasUnreadNotifications(int targetUserId);
        void MarkNotificationAsViewed(int notificationId, int targetUserId);
    }

    public class NotificationRepo : INotificationRepo
    {
        private string ConnectionString;
        public NotificationRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public List<Models.Notifications.Notification> GetAllNewNotifications(int targetUserId)
        {
            return GetAllNotifications(false, targetUserId);
        }
        public List<Models.Notifications.Notification> GetAllViewedNotifications(int targetUserId)
        {
            return GetAllNotifications(true, targetUserId);
        }
        private List<Models.Notifications.Notification> GetAllNotifications(bool onlyViewedNotifications, int targetUserId)
        {
            var getString = $@"SELECT * FROM notifications as n
                               INNER JOIN athletes AS a on n.GeneratedAthleteId = a.id
                               WHERE HasBeenViewed = @HasbeenViewed AND DestinationUserId = @TargetUserId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Models.Notifications.Notification, Models.Athlete.Athlete, Models.Notifications.Notification>(getString, (n, a) =>
                {
                    n.GeneratedAthlete = a;
                    return n;
                }, new { HasBeenViewed = onlyViewedNotifications, TargetUserId = targetUserId }).ToList();
            }
        }
        public bool HasUnreadNotifications(int targetUserId)
        {
            var getString = $@"SELECT top 1 1 FROM notifications WHERE HasBeenViewed = 0 AND DestinationUserId = @TargetUserId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<int>(getString, new { TargetUserId = targetUserId }).SingleOrDefault() == 1;
            }
        }
        public void MarkNotificationAsViewed(int notificationId, int targetUserId)
        {
            var updateString = $@"UPDATE Notifications SET HasBeenViewed = 1 WHERE id = @NotificationId AND DestinationUserId = @TargetUserId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { NotificationId = notificationId, TargetUserId = targetUserId });
            }

        }

    }
}
