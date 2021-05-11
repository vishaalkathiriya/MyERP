using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace ERP.Models
{
    public class NotificationUser
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }

    public class NotificationMessage
    {
        public string From { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }

    [HubName("notificationHub")]
    public class NotificationHub : Hub
    {
        private static Dictionary<string, NotificationUser> _connections = new Dictionary<string, NotificationUser>();

        [HubMethodName("userConnected")]
        public Task UserConnected(int userId, string userName)
        {
            var connectionId = GetConnectionIdByUserId(userId);
            if (!string.IsNullOrEmpty(connectionId))
            {
                _connections.Remove(connectionId);
            }

            NotificationUser notificationUser = new NotificationUser();
            notificationUser.UserID = userId;
            notificationUser.UserName = userName;
            _connections.Add(Context.ConnectionId, notificationUser);
            return Clients.All.onConnectUser(GetAllUsers().ToList());
        }

        [HubMethodName("sendMessageToUser")]
        public Task SendMessageToUser(NotificationMessage notifictionMessage)
        {
           return Clients.All.onNewMessage(notifictionMessage);
        }

        public override Task OnDisconnected()
        {
            var disconnectedUser = GetUserByConnectionId(Context.ConnectionId);
            _connections.Remove(Context.ConnectionId);
            return Clients.All.onDisconnectUser(disconnectedUser);
        }

        private NotificationUser GetUserByConnectionId(string connectionId)
        {
            NotificationUser notificationUser = new NotificationUser();
            if (_connections.ContainsKey(connectionId))
            {
                notificationUser = _connections[connectionId];
            }
            return notificationUser;
        }

        private string GetConnectionIdByUserId(int id)
        {
            return _connections.FirstOrDefault(c => c.Value.UserID == id).Key;
        }

        private List<NotificationUser> GetAllUsers()
        {
            List<NotificationUser> lstUsers = new List<NotificationUser>();
            foreach(var connection in _connections)
            {
                lstUsers.Add(connection.Value);
            }
            return lstUsers;
        }
    }
}