namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedWeekCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Programs", "WeekCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Programs", "WeekCount");
        }
    }
}
