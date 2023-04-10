namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingStripedExpiredCardAndFailedToProcess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "ExpiredCard", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "StripeFailedToProcess", c => c.Boolean(nullable: false));
            DropColumn("dbo.Organizations", "InGoodStanding");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Organizations", "InGoodStanding", c => c.Boolean(nullable: false));
            DropColumn("dbo.Organizations", "StripeFailedToProcess");
            DropColumn("dbo.Organizations", "ExpiredCard");
        }
    }
}
