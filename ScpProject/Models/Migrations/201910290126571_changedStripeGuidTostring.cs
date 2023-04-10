namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class changedStripeGuidTostring : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Organizations", "StripeGuid", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Organizations", "StripeGuid", c => c.Guid(nullable: false));
        }
    }
}
