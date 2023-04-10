namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ChangingRestToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SuperSetExercises", "Rest", c => c.String());
            AlterColumn("dbo.Workouts", "Rest", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Workouts", "Rest", c => c.Int());
            AlterColumn("dbo.SuperSetExercises", "Rest", c => c.Int());
        }
    }
}
