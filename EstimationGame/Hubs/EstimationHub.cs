using EstimationGame.Data;
using EstimationGame.Helpers;
using EstimationGame.Models;
using EstimationGame.Result;
using Microsoft.AspNetCore.SignalR;

namespace EstimationGame.Hubs
{
    public class EstimationHub : Hub
    {
        // Handles the creation of a group and adding a user to the group
        public async Task<ServiceResultExt<GroupCreationResult>> CreateGroup(string fullName)
        {
            string groupName = Guid.NewGuid().ToString();

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            User user = UserHelper.CreateUser(fullName, Context.ConnectionId, groupName);

            Group group = GroupHelper.CreateGroup(groupName);

            group.Users.Add(user);

            GroupSource.Groups.Add(group);

            return new ServiceResultExt<GroupCreationResult>
            {
                Status = true,
                Explanation = "Group Successfully Created",
                ResultObject = new GroupCreationResult
                {
                    GroupUrl = groupName,
                    User = user,
                    Group = group
                }
            };
        }

        // Adds a user to the specified group
        public async Task<ServiceResultExt<GroupJoinResult>> AddUserToGroup(string fullName, string groupName)
        {
            Group group = GroupHelper.GetGroup(groupName);
            if (group is null)
            {
                return new ServiceResultExt<GroupJoinResult>
                {
                    Status = false,
                    Explanation = "Group Not Available. You are redirected to the main page.",
                    ResultObject = new GroupJoinResult
                    {
                        User = null,
                        Group = null
                    }
                };             
            }
            else
            {
                User user = UserHelper.AddUser(fullName, Context.ConnectionId, groupName);

                group.Users.Add(user);

                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                await Clients.Group(groupName).SendAsync("Information", $"{user.FullName} Joined The Group.");

                return new ServiceResultExt<GroupJoinResult>
                {
                    Status = true,
                    Explanation = $"{user.FullName} Joined The Group.",
                    ResultObject = new GroupJoinResult
                    {
                        User = user,
                        Group = group
                    }
                };
            }
        }

        // Sends the users in the group to the clients
        public async Task GetUserToGroup(string groupName)
        {
            Group group = GroupHelper.GetGroup(groupName);
            if (group != null)
            {
                await Clients.Group(groupName).SendAsync("Users", group.Users);
            }
        }

        // Processes the selected option for the user
        public async Task ProcessSelectedOption(string groupName, string optionValue)
        {
            Group group = GroupHelper.GetGroup(groupName);

            if (group != null)
            {
                User user = GroupHelper.GetGroupUser(group, Context.ConnectionId);

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
        }

        // Processes the result for the group and sends it to the clients
        public async Task ProcessResult(string groupName)
        {
            Group group = GroupHelper.GetGroup(groupName);

            if (group != null)
            {
                group.ResultStatus = true;
                await Clients.Group(groupName).SendAsync("Users", group.Users);
                await Clients.Group(groupName).SendAsync("UpdateGroup", group);
            }
        }

        // Starts the game and notifies the group
        public async Task StartGame(string groupName)
        {
            Group group = GroupHelper.GetGroup(groupName);
            if (group != null)
            {
                group.GameStatus = true;
                await Clients.Group(groupName).SendAsync("UpdateGroup", group);
            }
        }

        // Updates the connection ID for the user
        public async Task UpdateConnectionId(string groupName, string connectionId)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                Group group = GroupHelper.GetGroup(groupName);
                if (group != null)
                {
                    User user = GroupHelper.GetGroupUser(group, connectionId);
                    if (user != null)
                    {
                        user.ConnectionId = Context.ConnectionId;
                        await Groups.RemoveFromGroupAsync(connectionId, groupName);
                        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                        await Clients.Client(user.ConnectionId).SendAsync("UpdateConnectionId", user);
                    }
                }  
            }
        }

        // Resets the game and notifies the group
        public async Task ResetGame(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                Group group = GroupHelper.GetGroup(groupName);

                if(group != null)
                {
                    group.Users.ForEach(x => { x.Option = null; x.Status = false; });
                    await Clients.Group(groupName).SendAsync("Users", group.Users);
                    group.ResultStatus = false;
                    group.OptionValues.Clear();
                    await Clients.Group(groupName).SendAsync("UpdateGroup", group);
                }
            }
        }

        // Handles the user logging out of the group and updates the group
        public async Task Logout(string groupName)
        {
            Group group = GroupHelper.GetGroup(groupName);
            if (group != null)
            {
                User user = GroupHelper.GetGroupUser(group, Context.ConnectionId);
                if (user != null)
                {
                    if (user.Moderator && group.Users.Count > 1)
                    {
                        User newModerator = group.Users.FirstOrDefault(x => x.ConnectionId != user.ConnectionId);
                        newModerator.Moderator = true;
                        await Clients.Client(newModerator.ConnectionId).SendAsync("UpdateUser", user);
                    }
                    GroupHelper.RemoveGroupUser(user);
                    await Clients.Group(groupName).SendAsync("Information", $"{user.FullName} Left the Group");
                    await Clients.Group(groupName).SendAsync("Users", group.Users);
                    await Clients.Group(groupName).SendAsync("UpdateGroup", group);
                }
                if (group.Users.Count < 1)
                {
                    GroupHelper.RemoveGroup(groupName);
                }
            }  
        }

        // Updates or adds an option for the group
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

        // Switches the user's selected option
        private void SwitchUserOption(Group group, User user, string optionValue)
        {
            user.Option.Value--;
            group.OptionValues.Remove(user.Option);

            user.Option = UpdateOrAddOption(group, optionValue);
        }
    }
}
