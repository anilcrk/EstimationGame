using EstimationGame.Data;
using EstimationGame.Models;

namespace EstimationGame.Helpers
{
    public static class GroupHelper
    {
        /// <summary>
        /// Creates a new group with the specified group name.
        /// </summary>
        /// <param name="groupName">The name of the group to be created.</param>
        /// <returns>A new Group object.</returns>
        public static Group CreateGroup(string groupName)
        {
            return new Group
            {
                GroupName = groupName,
                GameStatus = false,
                ResultStatus = false
            };
        }

        /// <summary>
        /// Retrieves a group based on the provided group name.
        /// </summary>
        /// <param name="groupName">The name of the group to be retrieved.</param>
        /// <returns>The Group object if found; otherwise, null.</returns>
        public static Group GetGroup(string groupName)
        {
            return GroupSource.Groups.FirstOrDefault(x => x.GroupName == groupName);
        }

        /// <summary>
        /// Retrieves a user from a group based on the connection ID.
        /// </summary>
        /// <param name="group">The group to search for the user.</param>
        /// <param name="connectionId">The connection ID of the user to be retrieved.</param>
        /// <returns>The User object if found; otherwise, null.</returns>
        public static User GetGroupUser(Group group, string connectionId)
        {
            return group.Users.FirstOrDefault(x => x.ConnectionId == connectionId);
        }

        /// <summary>
        /// Retrieves an option from a group based on the option name.
        /// </summary>
        /// <param name="group">The group to search for the option.</param>
        /// <param name="optionName">The name of the option to be retrieved.</param>
        /// <returns>The Option object if found; otherwise, null.</returns>
        public static Option GetOption(Group group, string optionName)
        {
            return group.OptionValues.FirstOrDefault(x => x.Name == optionName);
        }

        /// <summary>
        /// Removes a user from their group.
        /// </summary>
        /// <param name="user">The user to be removed.</param>
        public static void RemoveGroupUser(User user)
        {
            Group group = GetGroup(user.GroupName);
            group.Users.Remove(user);
        }

        /// <summary>
        /// Removes a group based on the group name.
        /// </summary>
        /// <param name="groupName">The name of the group to be removed.</param>
        public static void RemoveGroup(string groupName)
        {
            Group group = GetGroup(groupName);
            GroupSource.Groups.Remove(group);
        }
    }
}
