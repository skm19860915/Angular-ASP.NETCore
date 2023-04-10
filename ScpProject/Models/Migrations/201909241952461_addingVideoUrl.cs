namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingVideoUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Exercises", "VideoURL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Exercises", "VideoURL");
        }
    }
}
