using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SRTest
{
    public class ETreatSignalRHub : Hub
    {
        public ETreatSignalRHub()
        {

        }

        public static List<ConnectedUser> Users = new List<ConnectedUser>();


        public void BroadcastMessage(string name, string message)
        {
            Clients.All.SendAsync("broadcastMessage", name, message);
        }

        public void Echo(string name, string message)
        {
            Clients.Client(Context.ConnectionId).SendAsync("echo", name, message + " (echo from server)");
        }

        public async void JoinGroup(string name, string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("echo", "_SYSTEM_", $"{name} joined \"{groupName}\""); // with connectionId {Context.ConnectionId}");
            AddUser(name, groupName, Context.ConnectionId);
        }

        public async void LeaveGroup(string name, string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Client(Context.ConnectionId).SendAsync("echo", "_SYSTEM_", $"{name} leaved \"{groupName}\"");
            await Clients.Group(groupName).SendAsync("echo", "_SYSTEM_", $"{name} leaved \"{groupName}\"");
            RemoveUser(Context.ConnectionId);
        }

        public void SendGroup(string name, string groupName, string message)
        {
            Clients.Group(groupName).SendAsync("echo", name, message);
        }

        public void SendGroups(string name, IReadOnlyList<string> groups, string message)
        {
            Clients.Groups(groups).SendAsync("echo", name, message);
        }

        public void SendGroupExcept(string name, string groupName, IReadOnlyList<string> connectionIdExcept, string message)
        {
            Clients.GroupExcept(groupName, connectionIdExcept).SendAsync("echo", name, message);
        }

        public void SendUser(string name, string userId, string message)
        {
            Clients.User(userId).SendAsync("echo", name, message);
        }

        public void SendUsers(string name, IReadOnlyList<string> userIds, string message)
        {
            Clients.Users(userIds).SendAsync("echo", name, message);
        }

        private void AddUser(string name, string group, string cid)
        {
            ConnectedUser cu = new ConnectedUser(name, cid, group);
            Users.Add(cu);
            List<ConnectedUser> grouplist = Users.FindAll(x => x.groupname.Equals(group));
            SendGroup("_GROUPCOUNT_", group, "Users count in group: " + grouplist.Count);
        }

        private void RemoveUser(string cid)
        {
            ConnectedUser cu = Users.Find(x => x.connectionid.Equals(cid));
            if (cu != null)
            {
                string groupname = cu.groupname;
                Users.Remove(cu);
                List<ConnectedUser> grouplist = Users.FindAll(x => x.groupname.Equals(groupname));
                SendGroup("_GROUPCOUNT_", groupname, "Users count in group: " + grouplist.Count);
            }
        }

    }
}
