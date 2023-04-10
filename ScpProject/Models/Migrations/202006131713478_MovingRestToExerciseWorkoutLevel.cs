namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MovingRestToExerciseWorkoutLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SuperSetExercises", "Rest", c => c.Int());
            AddColumn("dbo.Workouts", "Rest", c => c.Int());
            DropColumn("dbo.ProgramDayItemSuperSet_Set", "Rest");
            DropColumn("dbo.Sets", "Rest");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sets", "Rest", c => c.Int());
            AddColumn("dbo.ProgramDayItemSuperSet_Set", "Rest", c => c.Int(nullable: false));
            DropColumn("dbo.Workouts", "Rest");
            DropColumn("dbo.SuperSetExercises", "Rest");
        }
    }
}
