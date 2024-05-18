using EstimationGame.Data;
using EstimationGame.Helpers;
using EstimationGame.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace EstimationGame.Hubs
{
    public class EstimationHub : Hub
    {
        public async Task<GroupCreationResult> CreateGroup(string fullName)
        {
            string groupName = Guid.NewGuid().ToString();

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            User user = UserHelper.CreateUser(fullName, Context.ConnectionId,groupName);

            Group group = GroupHelper.CreateGroup(groupName);

            group.Users.Add(user);

            GroupSource.Groups.Add(group);

            return new GroupCreationResult 
            {
                GroupUrl = groupName, 
                User = user, 
                Group = group 
            };
        }

        public async Task<GroupJoinResult> AddUserToGroup(string fullName, string groupName)
        {
            Group group = GroupHelper.GetGroup(groupName);
            if (group != null)
            {
                User user = UserHelper.AddUser(fullName, Context.ConnectionId, groupName);

                group.Users.Add(user);

                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                return new GroupJoinResult 
                { 
                    User = user, 
                    Group = group 
                };
            }
            return null;
        }

        public async Task GetUserToGroup(string groupName)
        {
            Group group = GroupHelper.GetGroup(groupName);
            await Clients.Group(groupName).SendAsync("Users", group.Users);
        }

        public async Task ProcessSelectedOption(string groupName, string optionValue)
        {
            Group group = GroupHelper.GetGroup(groupName);
            if (group == null) return;

            User user = GroupHelper.GetGroupUser(group, Context.ConnectionId);
            if (user == null) return;

            if (user.Option == null)
            {
                user.Option = UpdateOrAddOption(group, optionValue);
            }
            else
            {
                SwitchUserOption(group, user, optionValue);
            }

            user.Status = true;
            await Clients.Client(user.ConnectionId).SendAsync("UpdateUser", user);
            await Clients.Group(groupName).SendAsync("Users", group.Users);
        }

        public async Task ProcessResult(string groupName)
        {
            Group group = GroupHelper.GetGroup(groupName);
            User user = GroupHelper.GetGroupUser(group, Context.ConnectionId);

            if (user != null)
            {
                string values = JsonSerializer.Serialize(group.OptionValues);
                await Clients.Group(groupName).SendAsync("Result", values);
            }
        }

        public async Task StartGame(string groupName)
        {
            Group group = GroupHelper.GetGroup(groupName);
            group.GameStatus = true;
            await Clients.Group(groupName).SendAsync("UpdateGroup", group);
        }

        public async Task UpdateConnectionId(string groupName, string connectionId)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                Group group = GroupHelper.GetGroup(groupName);
                User user = GroupHelper.GetGroupUser(group, Context.ConnectionId);
                user.ConnectionId = Context.ConnectionId;
                await Groups.RemoveFromGroupAsync(connectionId, groupName);
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                await Clients.Client(user.ConnectionId).SendAsync("UpdateConnectionId", user);
            }
        }

        private Option UpdateOrAddOption(Group group, string optionName)
        {
            Option option = GroupHelper.GetOption(group, optionName);
            if (option != null && option.Value > 0)
            {
                option.Value++;
                return option;
            }
            else
            {
                var newOption = new Option
                {
                    Name = optionName,
                    Value = 1,
                };
                group.OptionValues.Add(newOption);
                return newOption;
            }
        }

        private void SwitchUserOption(Group group, User user, string optionValue)
        {
            user.Option.Value--;
            group.OptionValues.Remove(user.Option);

            user.Option = UpdateOrAddOption(group, optionValue);
        }

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    RemoveUser(Context.ConnectionId);
        //    await base.OnDisconnectedAsync(exception);
        //}

        //private async void RemoveUser(string connectionId)
        //{
        //    User userToRemove = UserSource.Users.FirstOrDefault(u => u.ConnectionId == connectionId);
        //    if (userToRemove != null)
        //    {
        //        UserSource.Users.Remove(userToRemove);
        //        await RemoveGroup(userToRemove); 
        //    }
        //}

        //private async Task RemoveGroup(User user)
        //{
        //    if (user != null)
        //    {
        //        Group group = GroupSource.Groups.FirstOrDefault(x => x.GroupName == user.GroupName);
        //        group.Users.Remove(user);
        //        if (group.Users.Count == 0)
        //        {
        //            GroupSource.Groups.Remove(group);
        //        }
        //        await Clients.Group(user.GroupName).SendAsync("Users", group.Users);
        //    }
        //}
    }
}
