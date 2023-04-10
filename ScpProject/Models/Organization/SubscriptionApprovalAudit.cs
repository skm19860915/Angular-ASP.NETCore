using System;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Payment;
namespace Models.Organization
{
    public class SubscriptionApprovalAudit
    {
        public int Id { get; set; }
        public string ApprovalFirstName { get; set; }
        public string ApprovalLastName { get; set; }
        public int PreviousPlanId { get; set; }
        public int NewPlanId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime ApprovalTime { get; set; }
        public int UserId { get; set; }//this isnt foreignkeyed because. What if the user who approves the account to go to a higher plan leaves?
        //this way we store who it was(even if the leave) and we also store their first/last name

        [ForeignKey("PreviousPlanId")]
        public virtual SubscriptionType PreviousPlan { get; set; }
        [ForeignKey("NewPlanId")]
        public virtual SubscriptionType NewPlan { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization TargetOrganization { get; set; }

    }
}

