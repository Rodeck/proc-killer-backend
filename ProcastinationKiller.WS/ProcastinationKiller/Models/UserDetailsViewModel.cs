namespace ProcastinationKiller.Models
{
    public class UserDetailsViewModel
    {
        public string Uid { get; set; }

        public string Name { get; set; }

        public bool IsFriend { get; set; }

        public UserState State { get; set; }
    }
}