namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class REMOVEDrefrencesTOuser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AthleteTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.ExerciseTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.MetricTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.MovieTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.MultiMediaTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.ProgramTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.SurveyTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.WorkoutTags", "CreatedUserId", "dbo.Users");
            DropIndex("dbo.AthleteTags", new[] { "CreatedUserId" });
            DropIndex("dbo.ExerciseTags", new[] { "CreatedUserId" });
            DropIndex("dbo.MetricTags", new[] { "CreatedUserId" });
            DropIndex("dbo.MovieTags", new[] { "CreatedUserId" });
            DropIndex("dbo.MultiMediaTags", new[] { "CreatedUserId" });
            DropIndex("dbo.ProgramTags", new[] { "CreatedUserId" });
            DropIndex("dbo.SurveyTags", new[] { "CreatedUserId" });
            DropIndex("dbo.WorkoutTags", new[] { "CreatedUserId" });
            DropColumn("dbo.AthleteTags", "CreatedUserId");
            DropColumn("dbo.ExerciseTags", "CreatedUserId");
            DropColumn("dbo.MetricTags", "CreatedUserId");
            DropColumn("dbo.MovieTags", "CreatedUserId");
            DropColumn("dbo.MultiMediaTags", "CreatedUserId");
            DropColumn("dbo.ProgramTags", "CreatedUserId");
            DropColumn("dbo.SurveyTags", "CreatedUserId");
            DropColumn("dbo.WorkoutTags", "CreatedUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkoutTags", "CreatedUserId", c => c.Int(nullable: false));
            AddColumn("dbo.SurveyTags", "CreatedUserId", c => c.Int(nullable: false));
            AddColumn("dbo.ProgramTags", "CreatedUserId", c => c.Int(nullable: false));
            AddColumn("dbo.MultiMediaTags", "CreatedUserId", c => c.Int(nullable: false));
            AddColumn("dbo.MovieTags", "CreatedUserId", c => c.Int(nullable: false));
            AddColumn("dbo.MetricTags", "CreatedUserId", c => c.Int(nullable: false));
            AddColumn("dbo.ExerciseTags", "CreatedUserId", c => c.Int(nullable: false));
            AddColumn("dbo.AthleteTags", "CreatedUserId", c => c.Int(nullable: false));
            CreateIndex("dbo.WorkoutTags", "CreatedUserId");
            CreateIndex("dbo.SurveyTags", "CreatedUserId");
            CreateIndex("dbo.ProgramTags", "CreatedUserId");
            CreateIndex("dbo.MultiMediaTags", "CreatedUserId");
            CreateIndex("dbo.MovieTags", "CreatedUserId");
            CreateIndex("dbo.MetricTags", "CreatedUserId");
            CreateIndex("dbo.ExerciseTags", "CreatedUserId");
            CreateIndex("dbo.AthleteTags", "CreatedUserId");
            AddForeignKey("dbo.WorkoutTags", "CreatedUserId", "dbo.Users", "Id");
            AddForeignKey("dbo.SurveyTags", "CreatedUserId", "dbo.Users", "Id");
            AddForeignKey("dbo.ProgramTags", "CreatedUserId", "dbo.Users", "Id");
            AddForeignKey("dbo.MultiMediaTags", "CreatedUserId", "dbo.Users", "Id");
            AddForeignKey("dbo.MovieTags", "CreatedUserId", "dbo.Users", "Id");
            AddForeignKey("dbo.MetricTags", "CreatedUserId", "dbo.Users", "Id");
            AddForeignKey("dbo.ExerciseTags", "CreatedUserId", "dbo.Users", "Id");
            AddForeignKey("dbo.AthleteTags", "CreatedUserId", "dbo.Users", "Id");
        }
    }
}
