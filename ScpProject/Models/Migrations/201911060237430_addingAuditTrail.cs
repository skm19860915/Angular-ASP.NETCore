namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingAuditTrail : DbMigration
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
            
            CreateTable(
                "dbo.SubscriptionTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AthleteCount = c.Int(nullable: false),
                        Recurring = c.Boolean(nullable: false),
                        StripeSubscriptionGuid = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubscriptionApprovalAudits", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.SubscriptionApprovalAudits", "PreviousPlanId", "dbo.SubscriptionTypes");
            DropForeignKey("dbo.SubscriptionApprovalAudits", "NewPlanId", "dbo.SubscriptionTypes");
            DropIndex("dbo.SubscriptionApprovalAudits", new[] { "OrganizationId" });
            DropIndex("dbo.SubscriptionApprovalAudits", new[] { "NewPlanId" });
            DropIndex("dbo.SubscriptionApprovalAudits", new[] { "PreviousPlanId" });
            DropTable("dbo.SubscriptionTypes");
            DropTable("dbo.SubscriptionApprovalAudits");
        }
    }
}
