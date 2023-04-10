namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedSubscriptionFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "HasSubscription", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "HasSubscription");
        }
    }
}
