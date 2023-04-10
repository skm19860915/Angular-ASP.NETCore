namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddNullableToParentMessageid : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Messages", "ParentMessageId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Messages", "ParentMessageId", c => c.Int(nullable: false));
        }
    }
}
