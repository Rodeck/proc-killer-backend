using System;

namespace ProcastinationKiller.Models
{
    public class MyInvitation
    {
        public int Id { get; set; }

        public string InvitedId { get; set; }

        public DateTime InvitationDate { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsRejected { get; set; }
    }
}