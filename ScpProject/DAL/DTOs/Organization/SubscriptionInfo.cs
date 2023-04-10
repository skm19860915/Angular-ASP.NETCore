namespace DAL.DTOs.Organization
{
    public class SubscriptionInfo
    {
        public int OrgId { get; set; }
        public int CurrentPlanId { get; set; }
        public string CurrentSubPlan { get; set; }
        public double CurrentSubPlanCost { get; set; }
        public string CurrentSubStripePlan { get; set; }
        public int CurrentSubAthleteNumber { get; set; }
        public string NextSubPlan { get; set; }
        public double NextSubPlanCost { get; set; }
        public double NextSubPlanCostAthleteNumber { get; set; }
        public string NextSubStripePlan { get; set; }
        public int NewPlanId { get; set; }
    }
}
