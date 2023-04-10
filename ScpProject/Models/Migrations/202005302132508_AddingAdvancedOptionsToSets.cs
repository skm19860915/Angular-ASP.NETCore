namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddingAdvancedOptionsToSets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sets", "Minutes", c => c.Int(nullable: false));
            AddColumn("dbo.Sets", "Seconds", c => c.Int(nullable: false));
            AddColumn("dbo.Sets", "Distance", c => c.Double(nullable: false));
            AddColumn("dbo.Sets", "RepsAchieved", c => c.Int(nullable: false));
            AddColumn("dbo.Sets", "Rest", c => c.Int(nullable: false));
            AddColumn("dbo.Sets", "Other", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sets", "Other");
            DropColumn("dbo.Sets", "Rest");
            DropColumn("dbo.Sets", "RepsAchieved");
            DropColumn("dbo.Sets", "Distance");
            DropColumn("dbo.Sets", "Seconds");
            DropColumn("dbo.Sets", "Minutes");
        }
    }
}
