using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Models.Messages;
using Dapper;
using DAL.DTOs.Messages;

namespace DAL.Repositories
{
    public interface IMessageRepo
    {
        int CreateGroup(string groupName, List<int> userIds, int userId);
        int CreateMessage(Message newMessage, int orgId);
        void DeleteGroupMessageThread(string groupName, int createdUserId);
        void DeleteUserMessageThread(int destinationUserId, int createdUserId);
        int? DoesGroupMessageThreadExist(string groupName, int createdUserId);
        int? DoesUserToMessageThreadExist(int targetUserId, int createdUserId);
        List<Message> GetAllMessages(int userId);
        Tuple<int, int> GetCountOfUnReadMessages(int targetUserId);
        Message GetGroupMessages(string groupName, Guid userToken);
        List<GroupMessagePreviewDTO> GetGroupMessagesPreview(int targetUserId);
        Message GetMessage(int messageId, int orgId);
        MessageGroup GetMessageGroup(int messageGroupId, int createdUserId);
        List<MessageThread> GetMessageThread(int messageId);
        List<Message> GetPagedMessages(int userId, int numMsgsOnPage, int numMsgsSkipped);
        int GetParentMessageId(int messageId);
        List<Guid> GetUserGroups(Guid userToken);
        List<UserMessagePreviewDTO> GetUserMessagesPreview(int targetUserId);
        List<MessageGroupUsers> GetUsersInGroupMessage(int groupId);
        bool IsGroupNameTaken(string groupName, Guid userToken);
        void MarkMessageAsRead(int messageId, int userId);
        void RemoveUsersFromGroupMessage(List<int> userIds, int groupId);
        void SendMessageToAUser(int targetUser, int messageId, int orgId);
        void SendMessageToGroup(int messageGroupId, int messageId, int createdUserId, int orgId);
        void TogglePauseMessage(int parentMessageId, bool pauseValue);
        List<MessageGroup> GetAllMessageGroups(int userId);
        MessageGroup GetMessageGroup(string groupTitle, Guid createdUserToken);
        List<Models.User.User> GetAllUsersForMessageGroup(int messageGroupId);
    }

    public class MessageRepo : IMessageRepo
    {
        private string ConnectionString;
        public MessageRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public int GetParentMessageId(int messageId)
        {
            var getString = "SELECT isnull(parentMessageId, Id) FROM messages WHERE id = @MessageId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<int>(getString, new { MessageId = messageId }).SingleOrDefault();
            }
        }
        public List<Guid> GetUserGroups(Guid userToken)
        {
            var sql = @"SELECT SignalRGroupId
                    FROM MessageGroups AS mg
                    INNER JOIN messageGroupsToUsers AS mgtu ON mgtu.MessageGroupId = mg.Id
                    INNER JOIN userTokens AS ut ON ut.UserId = mgtu.UserId
                    WHERE UT.Token = @userToken";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Guid>(sql, new { userToken }).ToList();
            }
        }

        //todo: security bug, needs fixing. We need to ratchet down hwhocan look at this thread based off of their current token. that way 
        //arbitary users  do not just view everyons messages, why i didnt do it at write time.
        //For the security to proplery work we need to check every message that the user accessing this message belongs to the same org as the original message creator
        //Super simple, but i am running on empty/lazy
        public List<DTOs.Messages.MessageThread> GetMessageThread(int messageId)
        {

            var getString = $@"SELECT m.Id AS 'MessageId', m.Content AS 'MessageContent' , m.SentTime AS 'SentTime',m.CreatedUserId AS 'CreatedUserId',isNull                                   (u.FirstName,a.firstName) as 'UserFirstName', ISNULL(u.LastName,a.lastName) AS 'UserLastname'
                            FROM [messages] AS m
                            LEFT JOIN USERS as U ON u.Id = m.CreatedUserId
                            LEFT JOIN Athletes AS a ON a.AthleteUserId  =m.CreatedUserId
                            WHERE m.id = @MessageId
                            union
                            SELECT m.Id AS 'MessageId', m.Content AS 'MessageContent' , m.SentTime AS 'SentTime',m.CreatedUserId AS 'CreatedUserId', isNull(u.FirstName,a.firstName) as 'UserFirstName', ISNULL(u.LastName,a.lastName) AS 'UserLastname'
                            FROM [messages] AS m 
                            LEFT JOIN USERS as U ON u.Id = m.CreatedUserId
                            LEFT JOIN Athletes AS a ON a.AthleteUserId  =m.CreatedUserId
                            WHERE m.ParentMessageId = @MessageId
                            ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.Messages.MessageThread>(getString, new { MessageId = messageId }).ToList();
            }
        }

        public bool IsGroupNameTaken(string groupName, Guid userToken)
        {
            var getString = $@"SELECT 1
                                FROM [MessageGroups] AS m
                                INNER JOIN users AS u ON u.Id = m.CreatedUserId
                                WHERE GroupTitle = @GroupName AND u.Id = ({ConstantSqlStrings.GetUserIdFromToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<int>(getString, new { GroupName = groupName, token = userToken }).SingleOrDefault() == 1;
            }
        }
        public Message GetGroupMessages(string groupName, Guid userToken)
        {
            var getString = $@"SELECT m.* 
                                FROM [MessageGroups] 
                                INNER JOIN users AS u ON u.Id = m.CreatedUserId
                                WHERE GroupTitle = @GroupName AND u.Id = (${ConstantSqlStrings.GetUserIdFromToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Message>(getString, new { GroupName = groupName, token = userToken }).Single();
            }
        }

        public Message GetMessage(int messageId, int orgId)
        {
            var getString = $@"SELECT m.* 
                                FROM [messages] as m
                                INNER JOIN users AS u ON u.Id = m.CreatedUserId
                                WHERE m.Id = @MessageId AND u.OrganizationId = @OrgId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Message>(getString, new { MessageId = messageId, OrgId = orgId }).Single();
            }
        }
        public List<DTOs.Messages.MessageGroupUsers> GetUsersInGroupMessage(int groupId)
        {
            var getString = $@"select ISNULL(u.firstName, a.FirstName) as 'UserFirstName', ISNULL(u.firstName,a.lastName) as 'UserLastName', u.id as 'UserId'
                                from MessageGroupsToUsers AS mgu
                                inner join USERS as U ON mgu.userid = u.Id
                                left join athletes as a on a.AthleteUserId = u.Id
                                where mgu.MessageGroupId = @MessageGroupId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.Messages.MessageGroupUsers>(getString, new { MessageGroupId = groupId }).ToList();
            }
        }
        public void RemoveUsersFromGroupMessage(List<int> userIds, int groupId)
        {
            var remSQL = $@"DELETE FROM MessageGroupsToUsers where Userid in  @UserIds AND MessageGroupId = @MessageGroupId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(remSQL, new { UserIds = userIds, MessageGroupId = groupId });
            }
        }

        public int CreateGroup(string groupName, List<int> userIds, int userId)
        {
            var createGroupSQL = $@"INSERT INTO [dbo].[MessageGroups]([GroupTitle],[CreatedUserId],SignalRGroupId) 
                                    SELECT @GroupTitle , @UserId, newId();

                                    SELECT CAST(SCOPE_IDENTITY() as int)";

            var addUserToGroupSql = $@"INSERT INTO [dbo].[MessageGroupsToUsers] (UserId, MessageGroupId) SELECT @UserId, @GroupId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var groupId = sqlConn.Query<int>(createGroupSQL, new { GroupTitle = groupName, UserId = userId }).Single();

                userIds.ForEach(x =>
                {
                    sqlConn.Execute(addUserToGroupSql, new { UserId = x, GroupId = groupId });
                });
                return groupId;
            }
        }
        public int? DoesUserToMessageThreadExist(int targetUserId, int createdUserId)
        {
            var checkSQL = $@"SELECT TOP 1 m.id
                                FROM[Messages] AS m
                                INNER JOIN[MessagesToUsers] AS mu ON m.Id = mu.MessageId
                                WHERE (m.CreatedUserId = @CreatedUserId AND mu.DestinationUserId = @TargetDestinationId OR m.CreatedUserId = @TargetDestinationId AND mu.DestinationUserId =  @CreatedUserId)
                                ORDER BY 1 ASC";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<int>(checkSQL, new { CreatedUserId = createdUserId, TargetDestinationId = targetUserId }).SingleOrDefault();
            }
        }
        public int? DoesGroupMessageThreadExist(string groupName, int createdUserId)
        {
            var checkSQL = $@"SELECT TOP 1 m.Id FROM MessageGroups AS m WHERE m.groupTitle = @GroupName AND m.createdUserId = @CreatedUserId ORDER BY 1 DESC ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<int>(checkSQL, new { CreatedUserId = createdUserId, GroupName = groupName }).SingleOrDefault();
            }
        }
        public void SendMessageToGroup(int messageGroupId, int messageId, int createdUserId, int orgId)
        {
            var insertSQL = $@"INSERT INTO MessagesToUsersinGroups (destinationuserId, messageId, MessageGroupId , Readtime)
                            SELECT mgu.UserId, @MessageId, mg.Id, NULL
                            FROM messageGroups AS mg
                            INNER JOIN messageGroupsToUsers AS mgu ON mgu.MessageGroupId = mg.Id
                            WHERE mg.id = @MessageGroupId";

            using (var sqlconn = new SqlConnection(ConnectionString))
            {
                sqlconn.Execute(insertSQL, new { MessageId = messageId, MessageGroupId = messageGroupId });
            }
        }

        public int CreateMessage(Message newMessage, int orgId)
        {
            var insertString = $@"INSERT INTO [dbo].[Messages]
                                   ([Content]
                                   ,[SentTime]
                                   ,[CreatedUserId]
                                   ,[ParentMessageId]
                                   ,[ReadOnly]
                                   ,[Pause])
                            SELECT  @Content, @SentTime, @SentUserId , @ParentMessageId, @ReadOnly, @Pause
                            FROM users AS u
                            WHERE u.Id = @SentUserId AND OrganizationId = @OrgId;
                                
                                    SELECT CAST(SCOPE_IDENTITY() as int)";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<int>(insertString, new
                {
                    ParentMessageId = newMessage.ParentMessageId,
                    Content = newMessage.Content,
                    SentTime = newMessage.SentTime,
                    SentUserId = newMessage.CreatedUserId,
                    OrgId = orgId,
                    ReadOnly = newMessage.ReadOnly,
                    Pause = newMessage.Pause,
                }).Single();
            }
        }
        public List<DTOs.Messages.GroupMessagePreviewDTO> GetGroupMessagesPreview(int targetUserId)
        {

            var getQuery = $@"SELECT mg.SignalRGroupId as 'SignalRGroupId', mu.ReadTime, m.ReadOnly AS 'ReadOnly' , m.Pause AS 'Pause', mg.GroupTitle AS 'MessageGroupTitle',mg.Id AS 'MessageGroupId' , m.CreatedUserId as 'LastSentByUserId', m.id as 'MessageId',isNull(u.FirstName,a.firstName) as 'LastSentFirstName', ISNULL(u.LastName,a.lastName) AS 'LastSentLastName',u.id as 'UserId',a.id as 'AthleteId',m.content AS 'MessageContent',m.sentTime AS 'SentTime'
                            FROM MessagesToUsersInGroups AS mu 
                            INNER JOIN [messages] AS m ON mu.MessageId = m.Id
                            INNER JOIN MessageGroups AS mg ON mg.Id = mu.MessageGroupId
                            LEFT JOIN USERS as U ON u.Id = m.CreatedUserId
                            LEFT JOIN Athletes AS a ON a.AthleteUserId  =m.CreatedUserId
                            WHERE MU.DestinationUserId =@TargetUserId and m.ParentMessageId is null
                            union
                            SELECT  mg.SignalRGroupId as 'SignalRGroupId',mu.ReadTime,m.ReadOnly AS 'ReadOnly' , m.Pause AS 'Pause', mg.GroupTitle AS 'MessageGroupTitle',mg.Id AS 'MessageGroupId' ,m.CreatedUserId as 'LastSentByUserId', m.id as 'MessageId',isNull(u.FirstName,a.firstName) as 'LastSentFirstName', ISNULL(u.LastName,a.lastName) AS 'LastSentLastName',u.id as 'UserId',a.id as 'AthleteId',m.content AS 'MessageContent',m.sentTime AS 'SentTime'
                            FROM MessagesToUsersInGroups AS mu 
                            INNER JOIN [messages] AS m ON mu.MessageId = m.Id
                            INNER JOIN MessageGroups AS mg ON mg.Id = mu.MessageGroupId
                            LEFT JOIN USERS as U ON u.Id = m.CreatedUserId
                            LEFT JOIN Athletes AS a ON a.AthleteUserId  =m.CreatedUserId
                            WHERE M.CreatedUserId = @TargetUserId and m.ParentMessageId is null
                            order by sentTime desc";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.Messages.GroupMessagePreviewDTO>(getQuery, new { TargetUserId = targetUserId }).ToList();
            }
        }
        public List<DTOs.Messages.UserMessagePreviewDTO> GetUserMessagesPreview(int targetUserId)
        {
            var getQuery = $@"SELECT u.SignalRGroupId as 'SignalRGroupId', mu.ReadTime,  @TargetUserId as 'ViewerId' ,ISNULL(da.FirstName,du.FirstName) AS 'DestinationFirstName' , ISNULL(da.lastname,du.firstname) AS 'DestinationLastName'  , m.ReadOnly AS 'ReadOnly' 
                            , m.Pause AS 'Pause', mu.DestinationUserId AS 'DestinationUserId',  m.CreatedUserId as 'LastSentByUserId', m.id as 'MessageId',isNull(u.FirstName,a.firstName) as 'LastSentFirstName'
                            , ISNULL(u.LastName,a.lastName) AS 'LastSentLastName',u.id as 'UserId',a.id as 'AthleteId',m.content AS 'MessageContent',m.sentTime AS 'SentTime'
                            FROM messagesToUsers AS mu 
                            INNER JOIN [messages] AS m ON mu.MessageId = m.Id
                            left join athletes as da ON da.AthleteUserId = mu.DestinationUserId
                            left JOIN [users] AS du ON du.Id = mu.DestinationUserid
                            LEFT JOIN USERS as U ON u.Id = m.CreatedUserId
                            LEFT JOIN Athletes AS a ON a.AthleteUserId  =m.CreatedUserId
                            WHERE MU.DestinationUserId = @TargetUserId AND m.ParentMessageId is null
                            UNION
                            SELECT u.SignalRGroupId as 'SignalRGroupId',  mu.ReadTime, @TargetUserId as 'ViewerId' , ISNULL(da.FirstName,du.FirstName) AS 'DestinationFirstName' , ISNULL(da.lastname,du.firstname) AS 'DestinationLastName'  , m.ReadOnly AS 'ReadOnly' 
                            , m.Pause AS 'Pause', mu.DestinationUserId AS 'DestinationUserId',  m.CreatedUserId as 'LastSentByUserId', m.id as 'MessageId'
                            ,isNull(u.FirstName,a.firstName) as 'LastSentFirstName', ISNULL(u.LastName,a.lastName) AS 'LastSentLastName',u.id as 'UserId',a.id as 'AthleteId',m.content AS 'MessageContent'
                            ,m.sentTime AS 'SentTime'
                            FROM messagesToUsers AS mu 
                            INNER JOIN [messages] AS m ON mu.MessageId = m.Id
                            left join athletes as da ON da.AthleteUserId = mu.DestinationUserId
                            left JOIN [users] AS du ON du.Id = mu.DestinationUserId
                            LEFT JOIN USERS as U ON u.Id = m.CreatedUserId
                            LEFT JOIN Athletes AS a ON a.AthleteUserId  =m.CreatedUserId
                            WHERE m.CreatedUserId = @TargetUserId AND m.ParentMessageId is null
                            order by sentTime desc";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.Messages.UserMessagePreviewDTO>(getQuery, new { TargetUserId = targetUserId }).ToList();
            }
        }
        /// <summary>
        /// Gets count of one-to-one message and groupmessages that were unread. Returning tuple incase in the future someone needs to split them apart
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <returns>tuple(int,int)  item1 = one-to-on message count, item2 = group message count</returns>
        public Tuple<int, int> GetCountOfUnReadMessages(int targetUserId)
        {
            var oneToOneMessagesQuery = "SELECT COUNT(1) FROM MessagesToUsers WHERE DestinationUserId = @TargetUserId AND ReadTime IS NULL";
            var groupMessagesQuery = "SELECT COUNT(1) FROM [MessagesToUsersInGroups] WHERE DestinationUserId = @TargetUserId AND ReadTime IS NULL";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var oneToOne = sqlConn.ExecuteScalar<int>(oneToOneMessagesQuery, new { TargetUserId = targetUserId });
                var groupMessages = sqlConn.ExecuteScalar<int>(groupMessagesQuery, new { TargetUserId = targetUserId });
                return new Tuple<int, int>(oneToOne, groupMessages);
            }
        }

        public void SendMessageToAUser(int targetUser, int messageId, int orgId)
        {
            var insertString = $@" INSERT INTO [dbo].[MessagesToUsers]
                                    ([DestinationUserId]
                                   ,[ReadTime]
                                   ,[MessageId])
                                SELECT Id,null,@MessageId 
                                FROM users
                                WHERE id = @TargetUserId AND OrganizationId = @OrgId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insertString, new { TargetUserId = targetUser, MessageId = messageId, OrgId = orgId });
            }
        }
        public void MarkMessageAsRead(int messageId, int userId)
        {
            var updateString = $@"UPDATE [dbo].[MessageToUsers]
                                   SET,[ReadTime] = @Now
                                 WHERE DestinationUserId = @TargetUser AND MessageId = @MessageId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Now = DateTime.Now, TargetUser = userId, MessageId = messageId });
            }
        }

        /// <summary>
        /// if this shit becomes a problem we can introduce pagination
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Message> GetAllMessages(int userId)
        {
            var getString = $@"SELECT m.* 
                                FROM [messages] AS m 
                                INNER JOIN MessageToUsers AS mtu ON M.Id = MTU.MessageId
                                WHERE mtu.DestinationUserId = @UserId ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Message>(getString, new { UserId = userId }).ToList();
            }
        }

        public List<Message> GetPagedMessages(int userId, int numMsgsOnPage, int numMsgsSkipped)
        {
            var getString = $@"SELECT m.* 
                                FROM [messages] AS m 
                                INNER JOIN MessagesToUsers AS mtu ON M.Id = MTU.MessageId
                                WHERE mtu.DestinationUserId = @UserId 
                                ORDER BY SentTime DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Message>(getString, new { UserId = userId, Skip = numMsgsSkipped, Take = numMsgsOnPage }).ToList();
            }
        }

        public void TogglePauseMessage(int parentMessageId, bool pauseValue)
        {
            var updateSQL = $@"UPDATE m
                               SET m.Pause = @PauseValue
                               FROM[Messages] AS m
                               WHERE id = @ParentMessageId OR parentMessageId = @ParentMessageId ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateSQL, new { ParentMessageId = parentMessageId, PauseValue = pauseValue });
            }
        }
        public void DeleteUserMessageThread(int destinationUserId, int createdUserId)
        {
            var delMUSQL = $@"DELETE mu
                           FROM[Messages] AS m
                           INNER JOIN[MessagesToUsers] AS mu ON m.Id = mu.MessageId
                           WHERE (m.CreatedUserId = @CreatedUserId AND mu.DestinationUserId = @TargetDestinationId OR m.CreatedUserId = @TargetDestinationId AND mu.DestinationUserId =  @CreatedUserId)";

            //inner joining to messagetousergroups to make sure that this message is orphaned
            //i guess the FK would make it burp if it wasn't truely orphaned
            //Also deleting all orphaned records lets us delete all messages with the parentId of x where I dont know the parent Id at delete time.
            var deleteMessageSQL = $@"delete m 
                                        FROM MESSAGES AS m
                                        LEFT JOIN MessagesToUsers AS mu ON MU.MessageId = M.Id
                                        LEFT JOIN MessagesToUsersInGroups AS mug ON mug.MessageId = m.Id
                                        WHERE mu.id is null and mug.id is null";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(delMUSQL, new { CreatedUserId = createdUserId, TargetDestinationId = destinationUserId });
                sqlConn.Execute(deleteMessageSQL);
            }
        }
        public void DeleteGroupMessageThread(string groupName, int createdUserId)
        {
            var deleteReceipients = $@" DELETE FROM messagesToUsersInGroups WHERE MessageGroupId = (SELECT id from MessageGroups where GroupTitle = @GroupName AND createdUserId = @CreatedUserId);";
            var deleteMessageGroup = $@" DELETE FROM MessageGroups WHERE groupTitle=@GroupName AND createdUserId = @CreatedUserId; ";
            var deleteMessageGroupToUsers = $@"DELETE FROM messageGroupsToUsers where MessageGroupId=  (SELECT id from MessageGroups where GroupTitle = @GroupName 
                                                AND createdUserId = @CreatedUserId)";
            //inner joining to messagetousergroups to make sure that this message is orphaned
            //i guess the FK would make it burp if it wasn't truely orphaned
            //Also deleting all orphaned records lets us delete all messages with the parentId of x where I dont know the parent Id at delete time.
            var deleteMessageSQL = $@"delete m 
                                        FROM MESSAGES AS m
                                        LEFT JOIN MessagesToUsers AS mu ON MU.MessageId = M.Id
                                        LEFT JOIN MessagesToUsersInGroups AS mug ON mug.MessageId = m.Id
                                        WHERE mu.id is null and mug.id is null";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(deleteReceipients, new { CreatedUserId = createdUserId, GroupName = @groupName });
                sqlConn.Execute(deleteMessageGroupToUsers, new { CreatedUserId = createdUserId, GroupName = @groupName });
                sqlConn.Execute(deleteMessageGroup, new { CreatedUserId = createdUserId, GroupName = @groupName });
                sqlConn.Execute(deleteMessageSQL);
            }
        }
        public MessageGroup GetMessageGroup(int messageGroupId, int userId)
        {
            var getSQL = $@" SELECT m.* 
                            FROM messagegroups AS m 
                            INNER JOIN MessageGroupsToUsers AS mu ON m.Id = mu.MessageGroupId
                            WHERE m.Id = @messageGroupId AND mu.userId = @userId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<MessageGroup>(getSQL, new { messageGroupId, userId }).FirstOrDefault();
            }
        }
        public MessageGroup GetMessageGroup(string groupTitle, Guid createdUserToken)
        {
            var getSQL = $@" SELECT m.* 
                            FROM messageGroups AS m 
                            INNER JOIN users  AS u ON m.createdUserId = u.id
                            WHERE m.groupTitle = @groupTitle AND u.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<MessageGroup>(getSQL, new { groupTitle = groupTitle, token = createdUserToken }).First();
            }
        }
        public List<MessageGroup> GetAllMessageGroups(int userId)
        {
            var getSQL = @" SELECT m.* 
                            FROM messageGroups AS m 
                            INNER JOIN MessageGroupsToUsers AS mu ON m.Id = mu.MessageGroupId
                            WHERE  mu.userId = @userId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<MessageGroup>(getSQL, new {  userId }).ToList();
            }
        }
        public List<Models.User.User> GetAllUsersForMessageGroup(int messageGroupId)
        {
            var getSQL = @"select u.*
                        from MessageGroupsToUsers AS mu
                        INNER JOIN users AS u ON mu.UserId = u.Id
                        WHERE mu.MessageGroupId = @messageGroupId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Models.User.User>(getSQL, new { messageGroupId }).ToList();
            }
        }

    }
}
