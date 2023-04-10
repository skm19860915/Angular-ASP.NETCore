using System;

namespace Models.Payment
{
    public class SubscriptionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AthleteCount { get; set; }
        public Boolean Recurring { get; set; }
        public string StripeSubscriptionGuid { get; set; }
        public double Cost { get; set; }
        public bool Tiered { get; set; }
    }
}
