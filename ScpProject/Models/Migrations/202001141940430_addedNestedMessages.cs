namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedNestedMessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "ParentMessageId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "ParentMessageId");
        }
    }
}
