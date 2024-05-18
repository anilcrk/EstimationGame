using EstimationGame.Models;

namespace EstimationGame.Helpers
{
    public static class UserHelper
    {
        public static User CreateUser(string fullName, string connectionId, string groupName)
        {
            return new User
            {
                ConnectionId = connectionId,
                FullName = fullName,
                Moderator = true,
                GroupName = groupName
            };
        }

        public static User AddUser(string fullName, string connectionId, string groupName)
        {
            User user = new User
            {
                ConnectionId = connectionId,
                FullName = fullName,
                Moderator = false,
                GroupName = groupName
            };

            return user;
        }
    }
}
