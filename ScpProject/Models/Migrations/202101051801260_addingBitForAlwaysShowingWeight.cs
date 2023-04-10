namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingBitForAlwaysShowingWeight : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SuperSetExercises", "ShowWeight", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sets", "ShowWeight", c => c.Boolean(nullable: false));
            AddColumn("dbo.Workouts", "ShowWeight", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Workouts", "ShowWeight");
            DropColumn("dbo.Sets", "ShowWeight");
            DropColumn("dbo.SuperSetExercises", "ShowWeight");
        }
    }
}
