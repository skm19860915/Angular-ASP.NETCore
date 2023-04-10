namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class moreMEssagingTablesForGroups_II : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.MessagesToUsersInGroup", newName: "MessagesToUsersInGroups");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.MessagesToUsersInGroups", newName: "MessagesToUsersInGroup");
        }
    }
}
