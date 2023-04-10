namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingUserIdtoAudit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubscriptionApprovalAudits", "UserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubscriptionApprovalAudits", "UserId");
        }
    }
}
