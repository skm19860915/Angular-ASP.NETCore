namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingStripGuid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "StripeGuid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "StripeGuid");
        }
    }
}
