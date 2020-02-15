using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class Badge
    {
        public int Id { get; set; }

        public BadgeDefinition Definition { get; set; }

        public string UserId { get; set; }

        public DateTime? AcquiredDate { get; set; }

        public ICollection<RewardCondition> Conditions { get; set; }

        public bool IsFulfiled => Conditions.All(x => Definition.Conditions.Single(y => y.Condition == x.Condition).Amount <= x.Amount);

        public RewardCondition GetConditionDefinitin(RewardCondition definition) => Definition.Conditions.First(x => x.Condition == definition.Condition);
    }
}
