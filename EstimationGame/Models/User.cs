namespace EstimationGame.Models
{
    public class User
    {
        public string ConnectionId { get; set; }
        public String FullName { get; set; }
        public bool Moderator { get; set; }
        public bool Status { get; set; }
        public Option Option { get; set; }
        public string GroupName { get; set; }
    }
}
