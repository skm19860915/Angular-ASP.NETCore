namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class moreBilling : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "IsCustomer", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "InGoodStanding", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "HowMuchTheyOwe", c => c.Double(nullable: false));
            DropColumn("dbo.Organizations", "FirstName");
            DropColumn("dbo.Organizations", "LastName");
            DropColumn("dbo.Organizations", "Phone");
            DropColumn("dbo.Organizations", "Email");
            DropColumn("dbo.Organizations", "Address1");
            DropColumn("dbo.Organizations", "Address2");
            DropColumn("dbo.Organizations", "City");
            DropColumn("dbo.Organizations", "State");
            DropColumn("dbo.Organizations", "Country");
            DropColumn("dbo.Organizations", "Zip");
            Sql("UPDATE organizations SET isCustomer = 1,InGoodStanding = 1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Organizations", "Zip", c => c.String());
            AddColumn("dbo.Organizations", "Country", c => c.String());
            AddColumn("dbo.Organizations", "State", c => c.String());
            AddColumn("dbo.Organizations", "City", c => c.String());
            AddColumn("dbo.Organizations", "Address2", c => c.String());
            AddColumn("dbo.Organizations", "Address1", c => c.String());
            AddColumn("dbo.Organizations", "Email", c => c.String());
            AddColumn("dbo.Organizations", "Phone", c => c.String());
            AddColumn("dbo.Organizations", "LastName", c => c.String());
            AddColumn("dbo.Organizations", "FirstName", c => c.String());
            DropColumn("dbo.Organizations", "HowMuchTheyOwe");
            DropColumn("dbo.Organizations", "InGoodStanding");
            DropColumn("dbo.Organizations", "IsCustomer");
        }
    }
}
