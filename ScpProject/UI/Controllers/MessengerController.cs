using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using BL;
using BL.BusinessObjects.Messages;
using DAL.DTOs.Messages;

namespace Controllers.Controllers
{
    [RoutePrefix("api/Messenger")]
    public class MessengerController : ApiController
    {
        private IMessageManager _messManager;
        public MessengerController(IMessageManager messman)
        {
            _messManager = messman;
        }

        [Route("GetUnreadMessageCount"), HttpGet]
        public Tuple<int, int> GetUnreadMessageCount()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _messManager.GetUnreadMessageCount(userGuid);
        }

        [Route("TogglePauseMessage"), HttpPost]
        public void TogglePauseMessage(PauseMessageDTO pauseMessageDTO)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _messManager.TogglePauseMessage(pauseMessageDTO.PauseValue, pauseMessageDTO.ParentMessageId, userGuid);
        }
        [Route("DeleteGroupMessages"), HttpPost]
        public void DeleteGroupMessages(DeleteGroupMessageDTO delete)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _messManager.DeleteGroupMessage(delete.GroupName, delete.CreatedUserId, userGuid);
        }
        [Route("DeleteUserMessages"), HttpPost]
        public void DeleteUserMessages(DeleteUserMessagesDTO delete)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _messManager.DeleteUserMessage(delete.DestinationUserId, delete.CreatedUserId, userGuid);
        }
        [Route("RespondToGroupMessage"), HttpPost]
        public void RespondToGroupMessage([FromBody] ViewModels.Message.GroupMessageResponse response)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _messManager.RespondToGroupMessage(response.MessageContent, userGuid, response.MessageGroupId, response.ParentMessageId, response.MessageGroupTitle);
        }
        [Route("RespondToUserMessage"), HttpPost]
        public void RespondToUserMessage([FromBody] ViewModels.Message.UserMessageResponse response)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _messManager.RespondToUserMessage(response.MessageContent, userGuid, response.DestinationUserId, response.ParentMessageId);
        }
        [Route("GetMessageThread/{messageId:int}")]
        public List<DAL.DTOs.Messages.MessageThread> GetMessageThread(int messageId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _messManager.GetMessageThread(messageId, userGuid);
        }

        [Route("GetMessagePreview"), HttpGet]
        public MessagePreview GetMessagePreview()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _messManager.GetAllMessagePreviews(userGuid);
        }
        [Route("CreateNewUserMessage"), HttpPost]
        public void CreateNewUserMessage([FromBody] ViewModels.Message.NewUserMessageDTO newMessage)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _messManager.CreateNewUserMessage(newMessage.MessageContent, newMessage.UserToSendTo, userGuid, newMessage.ReadOnly, newMessage.Pause);

        }
        [Route("CreateGroupMessage"), HttpPost]
        public void CreateNewGroupMessage(ViewModels.Message.NewGroupMessageDTO newMessage)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _messManager.CreateNewGroupMessage(newMessage.GroupTitle, newMessage.MessageContent, newMessage.UsersToSendTo, userGuid, newMessage.ReadOnly, newMessage.Pause);
        }
        [Route("DoesGroupExist"), HttpPost]
        public bool DoesGroupTitleExist([FromBody] NewGroup targetGroup)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _messManager.DoesGroupNameExist(targetGroup.GroupTitle, userGuid);
        }
        [Route("GetAllUnreadMessage/{pageNumber:int}/{number:int}")]
        public MessagePreview GetAllUnreadMessages(int pageNumber, int number)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _messManager.GetAllUnReadMessages(userGuid, pageNumber, number);
        }
    }

    public class NewGroup
    {
        public string GroupTitle { get; set; }
    }
    public class NewGroupMessage
    {
        public string GroupName { get; set; }
        public string MessageContent { get; set; }
        public List<int> UserIds { get; set; }
        public bool ReadOnly { get; set; }
        public bool Pause { get; set; }
    }


}
