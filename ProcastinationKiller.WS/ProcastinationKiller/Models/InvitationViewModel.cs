using System;

namespace ProcastinationKiller.Models
{
    public class InvitationViewModel
    {
        public int Id { get; set; }

        public string InviterId { get; set; }

        public string InviterName { get; set; }

        public string Icon { get; set; }

        public DateTime InvitationDate { get; set; }
    }
}