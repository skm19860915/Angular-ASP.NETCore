namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedNotificationsHasBeenViewed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "HasBeenViewed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "HasBeenViewed");
        }
    }
}
