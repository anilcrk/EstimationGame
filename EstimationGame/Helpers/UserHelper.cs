using EstimationGame.Models;

namespace EstimationGame.Helpers
{
    public static class UserHelper
    {
        /// <summary>
        /// Creates a new user with the specified full name, connection ID, and group name.
        /// The created user is assigned as a moderator.
        /// </summary>
        /// <param name="fullName">The full name of the user to be created.</param>
        /// <param name="connectionId">The connection ID of the user.</param>
        /// <param name="groupName">The name of the group the user belongs to.</param>
        /// <returns>A new User object with moderator privileges.</returns>
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

        /// <summary>
        /// Adds a new user to a group with the specified full name, connection ID, and group name.
        /// The added user is not assigned as a moderator.
        /// </summary>
        /// <param name="fullName">The full name of the user to be added.</param>
        /// <param name="connectionId">The connection ID of the user.</param>
        /// <param name="groupName">The name of the group the user belongs to.</param>
        /// <returns>A new User object without moderator privileges.</returns>
        public static User AddUser(string fullName, string connectionId, string groupName)
        {
            return new User
            {
                ConnectionId = connectionId,
                FullName = fullName,
                Moderator = false,
                GroupName = groupName
            };
        }
    }
}
