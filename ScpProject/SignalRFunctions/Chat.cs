using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using BL;
using System.Security.Claims;
using System.Collections.Generic;
using System.Text;
using DAL.Repositories;

namespace SignalRFunctions
{
    public class Chat : ServerlessHub
    {
        private HttpClient httpClient = new HttpClient();
        private const string NewMessageTarget = "newMessage";
        private const string NewConnectionTarget = "newConnection";
        private const string RegisteredToNewGroup = "RegisteredToNewGroup";


        [FunctionName("test")]
        public IActionResult test([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req)
        {
            return new ContentResult
            {
                Content = @"test",
                ContentType = "text"

            };
        }
        [FunctionName("index")]
        public IActionResult Index([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            return new ContentResult
            {
                Content = @"<html>

<head>
<meta content='text/html;charset=utf-8' http-equiv='Content-Type'>
<meta content='utf-8' http-equiv='encoding'>

  <title>Serverless Chat</title>
  <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@4.1.3/dist/css/bootstrap.min.css'>
  <script>
    window.apiBaseUrl = window.location.origin;
  </script>
  <style>
    .slide-fade-enter-active,
    .slide-fade-leave-active {
      transition: all 1s ease;
    }

    .slide-fade-enter,
    .slide-fade-leave-to {
      height: 0px;
      overflow-y: hidden;
      opacity: 0;
    }
  </style>
</head>

<body>
  <p>&nbsp;</p>
  <div id='app' class='container'>
    <h3>Serverless chat</h3>
    <div class='row' v-if='ready'>
      <div class='signalr-demo col-sm'>
        <hr />
        <div id='groupchecked'>
          <input type='checkbox' id='checkbox' v-model='checked'>
          <label for='checkbox'>Send To Default Group: {{ this.defaultgroup }}</label>
        </div>
        <form v-on:submit.prevent='sendNewMessage(checked)'>
          <input type='text' v-model='newMessage' id='message-box' class='form-control' placeholder='Type message here...' />
        </form>
      </div>
    </div>
    <div class='row' v-if='!ready'>
      <div class='col-sm'>
        <div>Loading...</div>
      </div>
    </div>
    <div v-if='ready'>
      <transition-group name='slide-fade' tag='div'>
        <div class='row' v-for='message in messages' v-bind:key='message.id'>
          <div class='col-sm'>
            <hr />
            <div>
              <div style='display: inline-block; padding-left: 12px;'>
                <div>
                  <a href='#' v-on:click.prevent='sendPrivateMessage(message.Sender)'>
                    <span class='text-info small'>
                      <strong>{{ message.Sender || message.sender }}</strong>
                    </span>
                  </a>
                  <span v-if='message.ConnectionId || message.connectionId'>
                    <a href='#' v-on:click.prevent='sendToConnection(message.ConnectionId || message.connectionId)'>
                      <span class='badge badge-primary'>Connection: {{ message.ConnectionId || message.connectionId }}</span>
                    </a>
                  </span>
                  <a href='#' v-on:click.prevent='addUserToGroup(message.Sender || message.sender)'>
                    <span class='badge badge-primary'>AddUserToGroup</span>
                  </a>
                  <a href='#' v-on:click.prevent='removeUserFromGroup(message.Sender || message.sender)'>
                    <span class='badge badge-primary'>RemoveUserFromGroup</span>
                  </a>
                  <a href='#' v-on:click.prevent='addConnectionToGroup(message.ConnectionId || message.connectionId)'>
                    <span v-if='message.ConnectionId || message.connectionId' class='badge badge-primary'>AddConnectionToGroup</span>
                  </a>
                  <a href='#' v-on:click.prevent='removeConnectionIdFromGroup(message.ConnectionId || message.connectionId)'>
                    <span v-if='message.ConnectionId || message.connectionId' class='badge badge-primary'>RemoveConnectionFromGroup</span>
                  </a>
                  <span v-if='message.IsPrivate || message.isPrivate' class='badge badge-secondary'>private message
                  </span>
                </div>
                <div>
                  {{ message.Text || message.text }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </transition-group>
    </div>

    <script src='https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.min.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/@aspnet/signalr@1.0.3/dist/browser/signalr.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/axios@0.18.0/dist/axios.min.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/crypto-js@3.1.9-1/crypto-js.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/crypto-js@3.1.9-1/enc-base64.js'></script>
    <script src='https://code.jquery.com/jquery-3.6.0.min.js' integrity='sha256-/xUj+3OJU5yExlq6GSYGSHk7tPXikynS7ogEvDej/m4=' crossorigin='anonymous'></script>
    <script>
      const data = {
        username: '',
        defaultgroup: 'AzureSignalR',
        checked: false,
        newMessage: '',
        messages: [],
        myConnectionId: '',
        ready: false
      };
      const app = new Vue({
        el: '#app',
        data: data,
        methods: {
          sendNewMessage: function (isToGroup) {
            if (isToGroup) {
              connection.invoke('sendToGroup', this.defaultgroup, this.newMessage);
            }
            else {
              connection.invoke('broadcast', this.newMessage);
            }
            this.newMessage = '';
          },
          sendPrivateMessage: function (user) {
            const messageText = prompt('Send private message to ' + user);

            if (messageText) {
              connection.invoke('sendToUser', user, messageText);
            }
          },
          sendToConnection: function (connectionId) {
            const messageText = prompt('Send private message to connection ' + connectionId);

            if (messageText) {
              connection.invoke('sendToConnection', connectionId, messageText);
            }
          },
          addConnectionToGroup: function(connectionId) {
            confirm('Add connection ' + connectionId + ' to group: ' + this.defaultgroup);
            connection.invoke('joinGroup', connectionId, this.defaultgroup);
          },
          addUserToGroup: function (user) {
            r = confirm('Add user ' + user + ' to group: ' + this.defaultgroup);
            connection.invoke('joinUserToGroup', user, this.defaultgroup);
          },
          removeConnectionIdFromGroup: function(connectionId) {
            confirm('Remove connection ' + connectionId + ' from group: ' + this.defaultgroup);
            connection.invoke('leaveGroup', connectionId, this.defaultgroup);
          },
          removeUserFromGroup: function(user) {
            confirm('Remove user ' + user + ' from group: ' + this.defaultgroup);
            connection.invoke('leaveUserFromGroup', user, this.defaultgroup);
          }
        }
      });
      const apiBaseUrl = window.location.origin;
      data.username = prompt('Enter your username');
      const isAdmin = confirm('Work as administrator? (only an administrator can broadcast messages)');
      if (!data.username) {
        alert('No username entered. Reload page and try again.');
        throw 'No username entered';
      }
document.cookie = 'username = John Doe'; 
      const connection = new signalR.HubConnectionBuilder()
        .withUrl(apiBaseUrl + '/api')
        .configureLogging(signalR.LogLevel.Information)
        .build();
      connection.on('newMessage', onNewMessage);
      connection.on('newConnection', onNewConnection)
      connection.onclose(() => console.log('disconnected'));
      console.log('connecting...');
      connection.start()
        .then(() => {
          data.ready = true;
          console.log('connected!');
        })
        .catch(console.error);
      function getAxiosConfig() {
        const config = {
          headers: {
            'x-ms-signalr-user-id':  data.username,
            'Authorization': 'Bearer ' + generateAccessToken(data.username)
          }
        };
        return config;
      }
      let counter = 0;
      function onNewMessage(message) {
        message.id = counter++; // vue transitions need an id
        data.messages.unshift(message);
      };
      function onNewConnection(message) {
console.log('test')
        data.myConnectionId = message.ConnectionId;
        authEnabled = false;
        if (message.Authentication)
        {
          authEnabled = true;
        }
        newConnectionMessage = {
          id : counter++,
          text : `${message.ConnectionId} has connected, with Authorization: ${authEnabled.toString()}`
        };

        $.ajax({
          type: 'POST',
          url: `http://localhost:7074/api/RegisterUserToGroups?connectionId=${message.ConnectionId}&userToken=1234`
          }).done(function() {
          console.log('test');
        });
        data.messages.unshift(newConnectionMessage);
      }

      function base64url(source) {
        // Encode in classical base64
        encodedSource = CryptoJS.enc.Base64.stringify(source);

        // Remove padding equal characters
        encodedSource = encodedSource.replace(/=+$/, '');

        // Replace characters according to base64url specifications
        encodedSource = encodedSource.replace(/\+/g, '-');
        encodedSource = encodedSource.replace(/\//g, '_');

        return encodedSource;
      }

      // this function should be in auth server, do not expose your secret
      function generateAccessToken(userName) {
        var header = {
          'alg': 'HS256',
          'typ': 'JWT'
        };

        var stringifiedHeader = CryptoJS.enc.Utf8.parse(JSON.stringify(header));
        var encodedHeader = base64url(stringifiedHeader);

        // customize your JWT token payload here 
        var data = {
          'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': userName,
          'exp': 1699819025,
          'admin': isAdmin
        };

        var stringifiedData = CryptoJS.enc.Utf8.parse(JSON.stringify(data));
        var encodedData = base64url(stringifiedData);

        var token = encodedHeader + '.' + encodedData;

        var secret = 'myfunctionauthtest'; // do not expose your secret here

        var signature = CryptoJS.HmacSHA256(token, secret);
        signature = base64url(signature);

        var signedToken = token + '.' + signature;

        return signedToken;
      }
    </script>
</body>

</html> ",
                ContentType = "text/html",
            };
        }
        [FunctionName("negotiate")]
        public SignalRConnectionInfo Negotiate([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, [SignalRConnectionInfo(HubName = "chat")] SignalRConnectionInfo connectionInfo)
        {
            //req.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value;
            //req.Headers.
            //https://stackoverflow.com/questions/59652214/how-do-i-add-custom-headers-to-signalrs-typescript-client/59686849


            //var messageRepo = new DAL.Repositories.MessageRepo(Config.SqlConn);
            //var allUserGroups = messageRepo.GetUserGroups(userToken);
            //var user = new DAL.Repositories.UserRepo(Config.SqlConn).Get(userToken);

            var claims = new List<Claim>() { new Claim("userToken", "test") };
            var x = Negotiate(req.Headers["x-ms-signalr-user-id"], claims);
            var bob = string.Empty;
            return x;
        }

        [FunctionName(nameof(OnConnected))]
        public async Task OnConnected([SignalRTrigger] InvocationContext invocationContext)
        {

            invocationContext.Headers.TryGetValue("Authorization", out var auth);
            await Clients.All.SendAsync(NewConnectionTarget, new NewConnection(invocationContext.ConnectionId, auth));
        }


        [FunctionName(nameof(SendGroupMessageById))]
        public async Task SendGroupMessageById([SignalRTrigger] InvocationContext invocationContext, string message, Guid userToken, int messageGroupId)
        {
            var messageRepo = new MessageRepo(Config.SqlConn);
            var userrepo = new UserRepo(Config.SqlConn);
            var targetUser = userrepo.Get(userToken);
            if (targetUser == null) return;
            var _messManager = new MessageManager(userrepo, messageRepo);
            var group = _messManager.GetGroupSignalRGuid(messageGroupId, userToken);
            if (Config.logEverything)
            {
                var logRepo = new DAL.Repositories.LogRepo(Config.SqlConn);
                logRepo.LogShit(new Models.LogMessage()
                {
                    Message = $"{userToken} {messageGroupId} SendGroupMessageById"
                ,
                    UserId = targetUser.Id,
                    LoggedDate = DateTime.Now,
                    StackTrace = new System.Diagnostics.StackTrace().ToString()
                });

            }
            await Clients.Group(group.SignalRGroupId.ToString()).SendAsync(NewMessageTarget, new NewMessage(invocationContext, message));
        }
        [FunctionName(nameof(SendGroupMessageGroupTitle))]
        public async Task SendGroupMessageGroupTitle([SignalRTrigger] InvocationContext invocationContext, string message, string userToken, string groupTitle)
        {
            var messageRepo = new MessageRepo(Config.SqlConn);
            var userrepo = new UserRepo(Config.SqlConn);
            var targetUser = userrepo.Get(userToken);
            if ( targetUser == null) return;
            var _messManager = new MessageManager(userrepo, messageRepo);
            var group = _messManager.GetMessageGroup(groupTitle, Guid.Parse(userToken));

            //since this is called on creation of group messages we need to 
            //register all users with this group so we can send them messages
            var allUsers = messageRepo.GetAllUsersForMessageGroup(group.Id);
            if (Config.logEverything)
            {
                var logRepo = new DAL.Repositories.LogRepo(Config.SqlConn);
                logRepo.LogShit(new Models.LogMessage()
                {
                    Message = $"userCount:{allUsers.Count}, event:{RegisteredToNewGroup}, userToken: {userToken}, groupTitle {groupTitle}, groupId : {group.Id}, groupSignalr : {group.SignalRGroupId}"
                ,
                    UserId = targetUser.Id,
                    LoggedDate = DateTime.Now,
                    StackTrace = new System.Diagnostics.StackTrace().ToString()
                });

            }

            for (int i = 0; i < allUsers.Count; i++)
            {
                if (Config.logEverything)
                {
                    var logRepo = new DAL.Repositories.LogRepo(Config.SqlConn);
                    logRepo.LogShit(new Models.LogMessage()
                    {
                        Message = $"userSignalRid:{allUsers[i].SignalRGroupID}, event:{RegisteredToNewGroup}, userToken: {userToken}, groupTitle {groupTitle}, groupId : {group.Id}, groupSignalr : {group.SignalRGroupId}"
                    ,
                        UserId = targetUser.Id,
                        LoggedDate = DateTime.Now,
                        StackTrace = new System.Diagnostics.StackTrace().ToString()
                    });

                }
                await Clients.Group(allUsers[i].SignalRGroupID.ToString()).SendAsync(RegisteredToNewGroup, new NewMessage(invocationContext, message));
            }


            if (Config.logEverything)
            {
                var logRepo = new DAL.Repositories.LogRepo(Config.SqlConn);
                logRepo.LogShit(new Models.LogMessage()
                {
                    Message = $"{message}, {groupTitle}. "
                ,
                    UserId = targetUser.Id,
                    LoggedDate = DateTime.Now,
                    StackTrace = new System.Diagnostics.StackTrace().ToString()
                });

            }
              //  await Clients.Group(group.SignalRGroupId.ToString()).SendAsync(NewMessageTarget, new NewMessage(invocationContext, message));
        }
        [FunctionName(nameof(RespondSendGroupMessageGroupTitle))]
        public async Task RespondSendGroupMessageGroupTitle([SignalRTrigger] InvocationContext invocationContext, string message, string userToken, string groupTitle)
        {
            var messageRepo = new MessageRepo(Config.SqlConn);
            var userrepo = new UserRepo(Config.SqlConn);
            var targetUser = userrepo.Get(userToken);
            if (targetUser == null) return;
            var _messManager = new MessageManager(userrepo, messageRepo);
            var group = _messManager.GetMessageGroup(groupTitle, Guid.Parse(userToken));

            //since this is called on creation of group messages we need to 
            //register all users with this group so we can send them messages
            var allUsers = messageRepo.GetAllUsersForMessageGroup(group.Id);
            if (Config.logEverything)
            {
                var logRepo = new DAL.Repositories.LogRepo(Config.SqlConn);
                logRepo.LogShit(new Models.LogMessage()
                {
                    Message = $"userCount:{allUsers.Count}, event:{RegisteredToNewGroup}, userToken: {userToken}, groupTitle {groupTitle}, groupId : {group.Id}, groupSignalr : {group.SignalRGroupId}"
                ,
                    UserId = targetUser.Id,
                    LoggedDate = DateTime.Now,
                    StackTrace = new System.Diagnostics.StackTrace().ToString()
                });

            }
            for (int i = 0; i < allUsers.Count; i++)
            {
                if (Config.logEverything)
                {
                    var logRepo = new DAL.Repositories.LogRepo(Config.SqlConn);
                    logRepo.LogShit(new Models.LogMessage()
                    {
                        Message = $"userSignalRid:{allUsers[i].SignalRGroupID}, event:{RegisteredToNewGroup}, userToken: {userToken}, groupTitle {groupTitle}, groupId : {group.Id}, groupSignalr : {group.SignalRGroupId}"
                    ,
                        UserId = targetUser.Id,
                        LoggedDate = DateTime.Now,
                        StackTrace = new System.Diagnostics.StackTrace().ToString()
                    });

                }
                await Clients.Group(allUsers[i].SignalRGroupID.ToString()).SendAsync(NewMessageTarget, new NewMessage(invocationContext, message));
            }

            if (Config.logEverything)
            {
                var logRepo = new DAL.Repositories.LogRepo(Config.SqlConn);
                logRepo.LogShit(new Models.LogMessage()
                {
                    Message = $"{message}, {groupTitle}. "
                ,
                    UserId = targetUser.Id,
                    LoggedDate = DateTime.Now,
                    StackTrace = new System.Diagnostics.StackTrace().ToString()
                });

            }
            //  await Clients.Group(group.SignalRGroupId.ToString()).SendAsync(NewMessageTarget, new NewMessage(invocationContext, message));
        }
        [FunctionName(nameof(SendUserMessageById))]
        public async Task SendUserMessageById([SignalRTrigger] InvocationContext invocationContext, string message, string userToken, int destinationUserId)
        {
            var messageRepo = new MessageRepo(Config.SqlConn);
            var userrepo = new UserRepo(Config.SqlConn);
            if (userrepo.Get(userToken) == null) return; 
            var _messManager = new MessageManager(userrepo, messageRepo);
            var targetUser = userrepo.Get(destinationUserId);

            await Clients.Group(targetUser.SignalRGroupID.ToString()).SendAsync(NewMessageTarget, new NewMessage(invocationContext, message));
        }

        [FunctionName(nameof(JoinUserToGroup))]
        public async Task JoinUserToGroup([SignalRTrigger] InvocationContext invocationContext, int userId, int groupId, string userToken)
        {

            await UserGroups.AddToGroupAsync(userId.ToString(), groupId.ToString());
        }

        [FunctionName(nameof(LeaveUserFromGroup))]
        public async Task LeaveUserFromGroup([SignalRTrigger] InvocationContext invocationContext, string userName, string groupName, string userToken)
        {

            await UserGroups.RemoveFromGroupAsync(userName, groupName);
        }

        [FunctionName(nameof(OnDisconnected))]
        public void OnDisconnected([SignalRTrigger] InvocationContext invocationContext)
        {
        }

        [FunctionName("CreateNewMessageGroup")]
        public async Task CreateNewMessageGroup([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            Guid userToken = Guid.Parse(req.Query["userToken"]);
            string messageGroupTitle = req.Query["MessageGroupTitle"];
            var messageRepo = new MessageRepo(Config.SqlConn);
            var messageGroup = messageRepo.GetMessageGroup(messageGroupTitle, userToken);

        }
        [FunctionName("RegisterUserToGroups")]
        public async Task RegisterUserConnectionToGroups([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            var logRepo = new DAL.Repositories.LogRepo(Config.SqlConn);
            Guid userToken = Guid.Parse(req.Query["userToken"]);
            string connectionId = req.Query["ConnectionId"];
            var messageRepo = new MessageRepo(Config.SqlConn);
            var userrepo = new UserRepo(Config.SqlConn);
            var _messManager = new MessageManager(userrepo, messageRepo);
            var allGroups = _messManager.GetAllGroupsForUser(userToken);
            var userRepo = new DAL.Repositories.UserRepo(Config.SqlConn);
            var targetUser = userRepo.Get(userToken);
            try
            {
                // var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
                if (Config.logEverything)
                {
                    logRepo.LogShit(new Models.LogMessage()
                    {
                        Message = $"{userToken}, {connectionId}. "
                    ,
                        UserId = targetUser.Id,
                        LoggedDate = DateTime.Now,
                        StackTrace = new System.Diagnostics.StackTrace().ToString()
                    });


                    logRepo.LogShit(new Models.LogMessage()
                    {
                        Message = $"trying to send message to  {connectionId}. ",
                        UserId = targetUser.Id,
                        LoggedDate = DateTime.Now,
                        StackTrace = new System.Diagnostics.StackTrace().ToString()
                    });
                    logRepo.LogShit(new Models.LogMessage()
                    {
                        Message = $"test pshing works",
                        UserId = targetUser.Id,
                        LoggedDate = DateTime.Now,
                        StackTrace = new System.Diagnostics.StackTrace().ToString()
                    });
                    var logStringGroups = new StringBuilder();

                    allGroups.ForEach(x => logStringGroups.Append(x));
                    logRepo.LogShit(new Models.LogMessage()
                    {
                        Message = $"{logStringGroups.ToString()}",
                        UserId = targetUser.Id,
                        LoggedDate = DateTime.Now,
                        StackTrace = new System.Diagnostics.StackTrace().ToString()
                    });
                }

                for (int i = 0; i < allGroups.Count; i++)
                {
                    await Groups.AddToGroupAsync(connectionId, allGroups[i].ToString());
                }

                await Clients.Client(connectionId).SendAsync(NewMessageTarget, new NewMessage(new InvocationContext(), "Hello all George "));

            }
            catch (Exception ex)
            {
                logRepo.LogShit(new Models.LogMessage()
                {
                    Message = $"{ex.ToString()} " ,
                    UserId = targetUser.Id,
                    LoggedDate = DateTime.Now,
                    StackTrace = new System.Diagnostics.StackTrace().ToString()
                });
            }
        }
    }
    public class UserMessage
    {
        public Guid UserToken { get; set; }
    }
    public class NewMessage
    {
        public string ConnectionId { get; }
        public string Sender { get; }
        public string Text { get; }

        public NewMessage(InvocationContext invocationContext, string message)
        {
            Sender = string.IsNullOrEmpty(invocationContext.UserId) ? string.Empty : invocationContext.UserId;
            ConnectionId = invocationContext.ConnectionId;
            Text = message;
        }
    }
    public class NewConnection
    {
        public string ConnectionId { get; }

        public string Authentication { get; }

        public NewConnection(string connectionId, string authentication)
        {
            ConnectionId = connectionId;
            Authentication = authentication;
        }
    }
    public class NewGroupMessageDTO
    {
        public string GroupTitle { get; set; }
        public int ParentMessageId { get; set; }
        public string MessageContent { get; set; }
        public List<int> UsersToSendTo { get; set; }
        public bool ReadOnly { get; set; }
        public bool Pause { get; set; }
    }
}
