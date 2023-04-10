using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using BL;
using Controllers.ViewModels;

namespace Controllers.Controllers
{
    [RoutePrefix("api/Notification")]
    public class NotificationController : ApiController
    {
        public INotificationManager _notifcationManager { get; set; }
        public NotificationController(INotificationManager notifcationManager)
        {
            _notifcationManager = notifcationManager;
        }
        [Route("GetAllViewedNotifcations"), HttpGet]
        public List<Models.Notifications.Notification> GetAllViewedNotifcations()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return  _notifcationManager.GetAllViewedNotifications(userGuid);
        }
        [Route("GetAllNewNotifications"), HttpGet]
        public List<Models.Notifications.Notification> GetAllNewNotifcations()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return  _notifcationManager.GetAllNewNotifications(userGuid).Item1;
        }
        [Route("GetAllPaginatedNewNotifications/{pageNumber:int}/{count:int}"), HttpGet]
        public DashboardNewNotificationsAlert GetAllNewNotifcations(int pageNumber, int count)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            var ret =  _notifcationManager.GetAllNewNotifications(userGuid, pageNumber, count);

            return new DashboardNewNotificationsAlert() { Notifications = ret.Item1, TotalNotificationCount = ret.Item2 };
        }
        [Route("MarkNotificationAsRead/{notificationId:int}"), HttpPost]
        public void MarkNotificationAsRead(int notificationId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
             _notifcationManager.MarkNotificationAsRead(notificationId, userGuid);
        }
        [Route("HasUnreadNotifications"), HttpGet]
        public bool HasUnreadNotifications()
        {
            try
            {
                var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
                return  _notifcationManager.HasUnreadNotifications(userGuid);
            }
            catch
            {
                //todo: still need an error logging platofrm
                return false;
            }

        }
    }
}