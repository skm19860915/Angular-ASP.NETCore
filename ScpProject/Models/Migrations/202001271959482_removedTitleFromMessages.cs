namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class removedTitleFromMessages : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Messages", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "Title", c => c.String());
        }
    }
}
