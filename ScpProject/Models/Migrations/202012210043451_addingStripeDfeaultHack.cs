namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingStripeDfeaultHack : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "CurrentPaymentMethod", c => c.String());
            AddColumn("dbo.Metrics", "Note", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Metrics", "Note");
            DropColumn("dbo.Organizations", "CurrentPaymentMethod");
        }
    }
}
