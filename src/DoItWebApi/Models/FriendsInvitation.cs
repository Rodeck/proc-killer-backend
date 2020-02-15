using System;

namespace ProcastinationKiller.Models
{
    public class FriendsInvitation
    {
        public int Id { get; set; }

        public string InviterId { get; set; }

        public string Icon { get; set; }

        public DateTime InvitationDate { get; set; }

        public bool Accepted { get; set; }

        public bool Rejected { get; set; }

        public DateTime AcceptedDate { get; set; }

        public DateTime RejectedDate { get; set; }

        public string InviterName { get; set; }
    }
}