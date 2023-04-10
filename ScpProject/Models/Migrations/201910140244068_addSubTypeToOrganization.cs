namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addSubTypeToOrganization : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "CurrentSubscriptionPlanId", c => c.Int());
            CreateIndex("dbo.Organizations", "CurrentSubscriptionPlanId");
            AddForeignKey("dbo.Organizations", "CurrentSubscriptionPlanId", "dbo.SubscriptionTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Organizations", "CurrentSubscriptionPlanId", "dbo.SubscriptionTypes");
            DropIndex("dbo.Organizations", new[] { "CurrentSubscriptionPlanId" });
            DropColumn("dbo.Organizations", "CurrentSubscriptionPlanId");
        }
    }
}
