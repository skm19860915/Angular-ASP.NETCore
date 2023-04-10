namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingRepsAchievedToAthletes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompletedSuperSet_Set", "RepsAchieved", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompletedSuperSet_Set", "RepsAchieved");
        }
    }
}
