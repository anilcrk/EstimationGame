
namespace EstimationGame.Models
{
    public class Group
    {
        public string GroupName { get; set; }
        public bool GameStatus { get; set; }
        public bool ResultStatus { get; set; }
        public List<User> Users { get; } = new List<User>();
        public List<Option> OptionValues { get; set; } = new List<Option>();
    }
}
