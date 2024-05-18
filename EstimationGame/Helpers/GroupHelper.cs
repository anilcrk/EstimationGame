using EstimationGame.Data;
using EstimationGame.Models;

namespace EstimationGame.Helpers
{
    public static class GroupHelper
    {
        public static Group CreateGroup(string groupName)
        {
            return new Group
            {
                GroupName = groupName,
                GameStatus = false
            };
        }

        public static Group GetGroup(string groupName)
        {
            return GroupSource.Groups.FirstOrDefault(x => x.GroupName == groupName);
        }

        public static User GetGroupUser(Group group, string connectionId)
        {
            return group.Users.FirstOrDefault(x => x.ConnectionId == connectionId);
        }

        public static Option GetOption(Group group, string optionName)
        {
            return group.OptionValues.FirstOrDefault(x => x.Name == optionName);
        }
    }
}
