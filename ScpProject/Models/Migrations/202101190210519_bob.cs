namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class bob : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "Phone", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "Phone");
        }
    }
}
