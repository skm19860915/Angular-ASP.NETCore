namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedCosttoSubscriptionTypes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubscriptionTypes", "Cost", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubscriptionTypes", "Cost");
        }
    }
}
