namespace DoItWebApi.Models
{
    public class BadgeConditionViewModel
    {
        public int Id { get; set; }

        public string Condition { get; set; }

        public int ActualAmount { get; set; }

        public int RequiredAmount { get; set; }
    }
}