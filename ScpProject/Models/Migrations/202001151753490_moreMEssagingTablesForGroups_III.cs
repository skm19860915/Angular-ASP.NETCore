namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class moreMEssagingTablesForGroups_III : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MessagesToUsersInGroups", "MessageGroupId", c => c.Int(nullable: false));
            DropColumn("dbo.MessagesToUsersInGroups", "GroupId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MessagesToUsersInGroups", "GroupId", c => c.Int(nullable: false));
            DropColumn("dbo.MessagesToUsersInGroups", "MessageGroupId");
        }
    }
}
