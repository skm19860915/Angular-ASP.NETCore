using System;
using System.Collections.Generic;
using System.Linq;
using BL.BusinessObjects.Messages;
using DAL.DTOs.Messages;
using DAL.Repositories;
using Models.Messages;
namespace BL
{
    public interface IMessageManager
    {
        IMessageRepo _messRepo { get; set; }
        IUserRepo _userRepo { get; set; }

        void CreateNewGroupMessage(string groupName, string messageContent, List<int> userIds, Guid createdToken, bool ReadOnly, bool Pause);
        void CreateNewUserMessage(string messageContent, int userId, Guid createdToken, bool readOnly, bool pause);
        void DeleteGroupMessage(string groupName, int createdUserId, Guid userToken);
        void DeleteUserMessage(int destinationUserId, int createdUserId, Guid userToken);
        bool DoesGroupNameExist(string groupName, Guid userToken);
        List<Guid> GetAllGroupsForUser(Guid userToken);
        MessagePreview GetAllMessagePreviews(Guid userToken);
        List<Message> GetAllMessages(Guid readerToken);
        MessagePreview GetAllUnReadMessages(Guid readerToken, int pageNumber = 0, int count = 0);
        MessageGroup GetGroupSignalRGuid(int messageGroupId, Guid userToken);
        List<MessageThread> GetMessageThread(int messageId, Guid userToken);
        Tuple<int, int> GetUnreadMessageCount(Guid userToken);
        void MarkMessageAsRead(int messageId, Guid readerToken);
        void RespondToGroupMessage(string messageContent, Guid userToken, int groupId, int parentMessageId, string groupName);
        void RespondToUserMessage(string messageContent, Guid userToken, int destinationUserId, int parentMessageId);
        void TogglePauseMessage(bool pauseValue, int parentMessageId, Guid userToken);
        MessageGroup GetMessageGroup(string messageGroupTitle, Guid createdUserToken);
        List<Models.User.User> GetAllUsersForMessageGroup(int messageGroupId);
    }

    public class MessageManager : IMessageManager
    {
        public IMessageRepo _messRepo { get; set; }
        public IUserRepo _userRepo { get; set; }
        public MessageManager(IUserRepo userRepo, IMessageRepo messageRepo)
        {
            _messRepo = messageRepo;
            _userRepo = userRepo;
        }
        public bool DoesGroupNameExist(string groupName, Guid userToken)
        {
            return _messRepo.IsGroupNameTaken(groupName, userToken);
        }
        public List<Guid> GetAllGroupsForUser(Guid userToken)
        {
            var createdUser = _userRepo.Get(userToken);
            var ret = new List<Guid>();
            ret.Add(createdUser.SignalRGroupID);
            _messRepo.GetAllMessageGroups(createdUser.Id).ForEach(x => ret.Add(x.SignalRGroupId));
            return ret;

        }
        public List<DAL.DTOs.Messages.MessageThread> GetMessageThread(int messageId, Guid userToken)
        {
            var createdUser = _userRepo.Get(userToken);
            var parentMessageId = _messRepo.GetParentMessageId(messageId);
            var ret = _messRepo.GetMessageThread(parentMessageId);
            ret.ForEach(x => x.ViewerId = createdUser.Id);
            return ret;
        }
        public void CreateNewUserMessage(string messageContent, int userId, Guid createdToken, bool readOnly, bool pause)
        {
            var createdUser = _userRepo.Get(createdToken);
            var parentMessageId = _messRepo.DoesUserToMessageThreadExist(userId, createdUser.Id);//gets the original messageId that instagited all of this.

            if (!createdUser.IsCoach)
            {//if user isnt a coach make sure athlete is only sending messages to coaches
                var targetUser = _userRepo.Get(userId);
                if (!targetUser.IsCoach) throw new ApplicationException("Athletes Can Only Create New Messages To Send To Coaches");
            }

            var newMessageId = _messRepo.CreateMessage(new Message()
            {
                Content = messageContent,
                SentTime = DateTime.Now,
                ReadOnly = readOnly,
                Pause = pause,
                CreatedUserId = createdUser.Id,
                ParentMessageId = parentMessageId == 0 ? null : parentMessageId,
            }, createdUser.OrganizationId);

            _messRepo.SendMessageToAUser(userId, newMessageId, createdUser.OrganizationId);
        }

        public MessagePreview GetAllMessagePreviews(Guid userToken)
        {
            var targetUser = _userRepo.Get(userToken);
            var group = _messRepo.GetGroupMessagesPreview(targetUser.Id);
            var users = _messRepo.GetUserMessagesPreview(targetUser.Id);
            group.ForEach(x => x.ViewerId = targetUser.Id);
            users.ForEach(x => x.ViewerId = targetUser.Id);
            return new MessagePreview()
            {
                GroupUserMessages = group,
                UserMessages = users
            };
        }

        public void CreateNewGroupMessage(string groupName, string messageContent, List<int> userIds, Guid createdToken, bool ReadOnly, bool Pause)
        {
            var createdUser = _userRepo.Get(createdToken);

            if (!createdUser.IsCoach)
            {//if user isnt a coach make sure athlete is only sending messages to coaches
                var users = _userRepo.Get(userIds);
                users.ForEach(x =>
                {
                    if (!x.IsCoach) throw new ApplicationException("Athletes Can Only Create New Messages To Send To Coaches");
                });
            }

            var existingGroup = _messRepo.IsGroupNameTaken(groupName, createdToken);

            if (existingGroup) throw new ApplicationException("You Have Already Created A Group With This Title");

            var newGroupId = _messRepo.CreateGroup(groupName, userIds, createdUser.Id);
            var newMessageId = _messRepo.CreateMessage(new Message()
            {
                Content = messageContent,
                SentTime = DateTime.Now,
                ReadOnly = ReadOnly,
                Pause = Pause,
                CreatedUserId = createdUser.Id,
                ParentMessageId = null
            }, createdUser.OrganizationId);
            _messRepo.SendMessageToGroup(newGroupId, newMessageId, createdUser.Id, createdUser.OrganizationId);
        }
        private int CreateMessage(Message newMessage, Guid userToken)
        {
            var createUser = _userRepo.Get(userToken);
            newMessage.CreatedUserId = createUser.Id;
            if (newMessage.ParentMessageId.HasValue)
            {
                var parentMessage = _messRepo.GetMessage(newMessage.ParentMessageId.Value, createUser.OrganizationId);
                if (parentMessage == null) throw new ApplicationException("You Can Only add Messages To Messages Within Your Organzation");
                if (parentMessage.Pause) throw new ApplicationException("This Messages Is Paused");
            }
            return _messRepo.CreateMessage(newMessage, createUser.OrganizationId);
        }

        public void RespondToGroupMessage(string messageContent, Guid userToken, int groupId, int parentMessageId, string groupName)
        {
            var createUser = _userRepo.Get(userToken);
            var verifiedGroupId = _messRepo.DoesGroupMessageThreadExist(groupName, createUser.Id);
            var targetParentMessage = _messRepo.GetMessage(parentMessageId, createUser.OrganizationId);

            if (targetParentMessage == null) throw new ApplicationException("Cannot Create Message, Because Parent Message Doesnt Exist");

            if (targetParentMessage.Pause) throw new ApplicationException("The Message Thread Is Paused And Cannot Be Responded To");
            var messageId = CreateMessage(new Message()
            {
                Content = messageContent,
                ParentMessageId = parentMessageId,
                CreatedUserId = createUser.Id,
                Pause = false,
                ReadOnly = false,
                SentTime = DateTime.Now,
            }, userToken);

            _messRepo.SendMessageToGroup(groupId, messageId, createUser.Id, createUser.OrganizationId);
        }

        public void RespondToUserMessage(string messageContent, Guid userToken, int destinationUserId, int parentMessageId)
        {
            var createUser = _userRepo.Get(userToken);
            var targetParentMessage = _messRepo.GetMessage(parentMessageId, createUser.OrganizationId);
            if (targetParentMessage == null) throw new ApplicationException("The Message Has Been Depleted Or Cannot Be Found");


            if (targetParentMessage.Pause) throw new ApplicationException("The Message Thread Is Paused And Cannot Be Responded To");

            var messageId = CreateMessage(new Message()
            {
                Content = messageContent,
                ParentMessageId = parentMessageId,
                CreatedUserId = createUser.Id,
                Pause = false,
                ReadOnly = false,
                SentTime = DateTime.Now,
            }, userToken);

            _messRepo.SendMessageToAUser(destinationUserId, messageId, createUser.OrganizationId);
        }
        /// <summary>
        /// Gets count of one-to-one message and groupmessages that were unread. Returning tuple incase in the future someone needs to split them apart
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <returns>tuple(int,int)  item1 = one-to-on message count, item2 = group message count</returns>
        public Tuple<int, int> GetUnreadMessageCount(Guid userToken)
        {
            var reader = _userRepo.Get(userToken);
            return _messRepo.GetCountOfUnReadMessages(reader.Id);
        }

        public void MarkMessageAsRead(int messageId, Guid readerToken)
        {
            var reader = _userRepo.Get(readerToken);
            _messRepo.MarkMessageAsRead(messageId, reader.Id);
        }
        public List<Message> GetAllMessages(Guid readerToken)
        {
            var reader = _userRepo.Get(readerToken);
            return _messRepo.GetAllMessages(reader.Id);
        }
        public MessagePreview GetAllUnReadMessages(Guid readerToken, int pageNumber = 0, int count = 0)
        {
            var reader = _userRepo.Get(readerToken);
            if (count > 0)
            {
                var item1 = _messRepo.GetGroupMessagesPreview(reader.Id).Where(x => x.ReadTime == null).ToList();
                var item2 = _messRepo.GetUserMessagesPreview(reader.Id).Where(x => x.ReadTime == null).ToList();
                var totalMessageCount = item1.Count() + item2.Count();

                return new MessagePreview()
                {
                    GroupUserMessages = item1.OrderByDescending(x => x.SentTime).Skip(pageNumber * count).Take(count).ToList(),
                    UserMessages = item2.OrderByDescending(x => x.SentTime).Skip(pageNumber * count).Take(count).ToList(),
                    TotalMessageCount = totalMessageCount
                };
            }
            var item1_1 = _messRepo.GetGroupMessagesPreview(reader.Id).Where(x => x.ReadTime == null).OrderByDescending(x => x.SentTime).ToList();
            var item2_2 = _messRepo.GetUserMessagesPreview(reader.Id).Where(x => x.ReadTime == null).OrderByDescending(x => x.SentTime).ToList();
            return new MessagePreview() { GroupUserMessages = item1_1, UserMessages = item2_2, TotalMessageCount = (item1_1.Count() + item2_2.Count()) };
        }
        public void TogglePauseMessage(bool pauseValue, int parentMessageId, Guid userToken)
        {
            var deletingUser = _userRepo.Get(userToken);
            var parentMessage = _messRepo.GetMessage(parentMessageId, deletingUser.OrganizationId);
            var createdUser = _userRepo.Get(parentMessage.CreatedUserId);

            if (createdUser.OrganizationId != deletingUser.OrganizationId) throw new ApplicationException("You can ONLY modify messages sent from your organization");
            _messRepo.TogglePauseMessage(parentMessageId, pauseValue);
        }
        public void DeleteGroupMessage(string groupName, int createdUserId, Guid userToken)
        {
            var createdUser = _userRepo.Get(createdUserId);
            var deletingUser = _userRepo.Get(userToken);

            if (createdUser.OrganizationId != deletingUser.OrganizationId) throw new ApplicationException("You can ONLY modify messages sent from your organization");
            _messRepo.DeleteGroupMessageThread(groupName, createdUserId);
        }
        public void DeleteUserMessage(int destinationUserId, int createdUserId, Guid userToken)
        {
            var createdUser = _userRepo.Get(createdUserId);
            var deletingUser = _userRepo.Get(userToken);

            if (createdUser.OrganizationId != deletingUser.OrganizationId) throw new ApplicationException("You can ONLY modify messages sent from your organization");
            _messRepo.DeleteUserMessageThread(destinationUserId, createdUserId);
        }
        public MessageGroup GetGroupSignalRGuid(int messageGroupId, Guid userToken)
        {
            var userInGroup = _userRepo.Get(userToken);
            return _messRepo.GetMessageGroup(messageGroupId, userInGroup.Id);
        }
        public MessageGroup GetMessageGroup(string messageGroupTitle, Guid createdUserToken)
        {
            return _messRepo.GetMessageGroup(messageGroupTitle, createdUserToken);
        }
        public List<Models.User.User> GetAllUsersForMessageGroup(int messageGroupId)
        {
            return _messRepo.GetAllUsersForMessageGroup(messageGroupId);
        }
    }
}
