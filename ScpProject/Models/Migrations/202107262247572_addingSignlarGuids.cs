namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingSignlarGuids : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "SignalRGroupID", c => c.Guid(nullable: false));
            AddColumn("dbo.MessageGroups", "SignalRGroupId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MessageGroups", "SignalRGroupId");
            DropColumn("dbo.Users", "SignalRGroupID");
        }
    }
}
