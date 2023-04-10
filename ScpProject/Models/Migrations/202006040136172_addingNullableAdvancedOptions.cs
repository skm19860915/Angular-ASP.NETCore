namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingNullableAdvancedOptions : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sets", "Sets", c => c.Int());
            AlterColumn("dbo.Sets", "Reps", c => c.Int());
            AlterColumn("dbo.Sets", "Percent", c => c.Double());
            AlterColumn("dbo.Sets", "Weight", c => c.Double());
            AlterColumn("dbo.Sets", "Minutes", c => c.Int());
            AlterColumn("dbo.Sets", "Seconds", c => c.Int());
            AlterColumn("dbo.Sets", "Distance", c => c.Double());
            AlterColumn("dbo.Sets", "RepsAchieved", c => c.Boolean());
            AlterColumn("dbo.Sets", "Rest", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sets", "Rest", c => c.Int(nullable: false));
            AlterColumn("dbo.Sets", "RepsAchieved", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Sets", "Distance", c => c.Double(nullable: false));
            AlterColumn("dbo.Sets", "Seconds", c => c.Int(nullable: false));
            AlterColumn("dbo.Sets", "Minutes", c => c.Int(nullable: false));
            AlterColumn("dbo.Sets", "Weight", c => c.Double(nullable: false));
            AlterColumn("dbo.Sets", "Percent", c => c.Double(nullable: false));
            AlterColumn("dbo.Sets", "Reps", c => c.Int(nullable: false));
            AlterColumn("dbo.Sets", "Sets", c => c.Int(nullable: false));
        }
    }
}
