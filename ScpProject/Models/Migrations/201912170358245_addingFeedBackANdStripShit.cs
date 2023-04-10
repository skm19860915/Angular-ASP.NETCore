namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingFeedBackANdStripShit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeedBacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        feedBack = c.String(),
                        UserId = c.Int(nullable: false),
                        Sent = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Organizations", "FirstName", c => c.String());
            AddColumn("dbo.Organizations", "LastName", c => c.String());
            AddColumn("dbo.Organizations", "Phone", c => c.String());
            AddColumn("dbo.Organizations", "Email", c => c.String());
            AddColumn("dbo.Organizations", "Address1", c => c.String());
            AddColumn("dbo.Organizations", "Address2", c => c.String());
            AddColumn("dbo.Organizations", "City", c => c.String());
            AddColumn("dbo.Organizations", "State", c => c.String());
            AddColumn("dbo.Organizations", "Country", c => c.String());
            AddColumn("dbo.Organizations", "Zip", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "Zip");
            DropColumn("dbo.Organizations", "Country");
            DropColumn("dbo.Organizations", "State");
            DropColumn("dbo.Organizations", "City");
            DropColumn("dbo.Organizations", "Address2");
            DropColumn("dbo.Organizations", "Address1");
            DropColumn("dbo.Organizations", "Email");
            DropColumn("dbo.Organizations", "Phone");
            DropColumn("dbo.Organizations", "LastName");
            DropColumn("dbo.Organizations", "FirstName");
            DropTable("dbo.FeedBacks");
        }
    }
}
