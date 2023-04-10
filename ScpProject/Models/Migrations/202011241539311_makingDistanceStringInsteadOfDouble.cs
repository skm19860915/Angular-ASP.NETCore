namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class makingDistanceStringInsteadOfDouble : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "CreditCardExpiring", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "SubscriptionEnded", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "BadCreditCard", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Sets", "Distance", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sets", "Distance", c => c.Double());
            DropColumn("dbo.Organizations", "BadCreditCard");
            DropColumn("dbo.Organizations", "SubscriptionEnded");
            DropColumn("dbo.Organizations", "CreditCardExpiring");
        }
    }
}
