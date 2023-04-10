namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingTiered : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubscriptionTypes", "Tiered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubscriptionTypes", "Tiered");
        }
    }
}
