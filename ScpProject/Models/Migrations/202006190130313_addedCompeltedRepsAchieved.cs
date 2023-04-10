namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedCompeltedRepsAchieved : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompletedSuperSet_Set", "CompletedRepsAchieved", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompletedSuperSet_Set", "CompletedRepsAchieved");
        }
    }
}
