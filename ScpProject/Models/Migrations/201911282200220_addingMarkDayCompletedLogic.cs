namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingMarkDayCompletedLogic : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubscriptionApprovalAudits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApprovalFirstName = c.String(),
                        ApprovalLastName = c.String(),
                        PreviousPlanId = c.Int(nullable: false),
                        NewPlanId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                        ApprovalTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SubscriptionTypes", t => t.NewPlanId)
                .ForeignKey("dbo.SubscriptionTypes", t => t.PreviousPlanId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => t.PreviousPlanId)
                .Index(t => t.NewPlanId)
                .Index(t => t.OrganizationId);
            
            AddColumn("dbo.Programs", "DayCount", c => c.Int(nullable: false));
            AddColumn("dbo.SubscriptionTypes", "Recurring", c => c.Boolean(nullable: false));
            AddColumn("dbo.SubscriptionTypes", "StripeSubscriptionGuid", c => c.String());
            AddColumn("dbo.CompletedProgramDays", "CompletedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.CompletedProgramDays", "WeekId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubscriptionApprovalAudits", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.SubscriptionApprovalAudits", "PreviousPlanId", "dbo.SubscriptionTypes");
            DropForeignKey("dbo.SubscriptionApprovalAudits", "NewPlanId", "dbo.SubscriptionTypes");
            DropIndex("dbo.SubscriptionApprovalAudits", new[] { "OrganizationId" });
            DropIndex("dbo.SubscriptionApprovalAudits", new[] { "NewPlanId" });
            DropIndex("dbo.SubscriptionApprovalAudits", new[] { "PreviousPlanId" });
            DropColumn("dbo.CompletedProgramDays", "WeekId");
            DropColumn("dbo.CompletedProgramDays", "CompletedDate");
            DropColumn("dbo.SubscriptionTypes", "StripeSubscriptionGuid");
            DropColumn("dbo.SubscriptionTypes", "Recurring");
            DropColumn("dbo.Programs", "DayCount");
            DropTable("dbo.SubscriptionApprovalAudits");
        }
    }
}
