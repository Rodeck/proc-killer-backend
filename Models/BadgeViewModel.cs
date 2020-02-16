using System;
using System.Collections.Generic;

namespace DoItWebApi.Models
{
    public class BadgeViewModel
    {
        public int Id { get; set; }

        public DateTime? AcquiredDate { get; set; }

        public bool IsFulfiled { get; set; }

        public float Percentage { get; set; }

        public string Image { get; set; }

        public IEnumerable<BadgeConditionViewModel> Conditions { get; set; }
    }
}