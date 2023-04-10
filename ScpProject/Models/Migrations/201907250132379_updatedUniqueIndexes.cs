namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class updatedUniqueIndexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Programs", new[] { "Name" });
            DropIndex("dbo.Programs", new[] { "CreatedUserId" });
            DropIndex("dbo.Metrics", new[] { "Name" });
            DropIndex("dbo.UnitOfMeasurements", new[] { "UnitType" });
            DropIndex("dbo.Surveys", new[] { "Title" });
            DropIndex("dbo.Surveys", new[] { "CreatedUserId" });
            DropIndex("dbo.Questions", new[] { "QuestionDisplayText" });
            DropIndex("dbo.Questions", new[] { "CreatedUserId" });
            DropIndex("dbo.Exercises", new[] { "Name" });
            DropIndex("dbo.Exercises", new[] { "CreatedUserId" });
            DropIndex("dbo.Workouts", new[] { "Name" });
            DropIndex("dbo.Workouts", new[] { "CreatedUserId" });
            CreateIndex("dbo.Programs", new[] { "CreatedUserId", "Name" }, unique: true, name: "IX_CreatedUserId_Name_Programs");
            CreateIndex("dbo.Metrics", new[] { "CreatedUserId", "Name" }, unique: true, name: "IX_CreatedUserId_Name_Metrics");
            CreateIndex("dbo.UnitOfMeasurements", new[] { "CreatedUserId", "UnitType" }, unique: true, name: "IX_CreatedUser_Id_UnitType");
            CreateIndex("dbo.Surveys", new[] { "CreatedUserId", "Title" }, unique: true, name: "IX_CreatedUserId_Title_Surveys");
            CreateIndex("dbo.Questions", new[] { "CreatedUserId", "QuestionDisplayText" }, unique: true, name: "IX_CreatedUserId_QuestionDisplayText_Questions");
            CreateIndex("dbo.Exercises", new[] { "CreatedUserId", "Name" }, unique: true, name: "IX_CreatedUserId_Name_Exercises");
            CreateIndex("dbo.Workouts", new[] { "CreatedUserId", "Name" }, unique: true, name: "IX_CreatedUserId_Name_Workouts");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Workouts", "IX_CreatedUserId_Name_Workouts");
            DropIndex("dbo.Exercises", "IX_CreatedUserId_Name_Exercises");
            DropIndex("dbo.Questions", "IX_CreatedUserId_QuestionDisplayText_Questions");
            DropIndex("dbo.Surveys", "IX_CreatedUserId_Title_Surveys");
            DropIndex("dbo.UnitOfMeasurements", "IX_CreatedUser_Id_UnitType");
            DropIndex("dbo.Metrics", "IX_CreatedUserId_Name_Metrics");
            DropIndex("dbo.Programs", "IX_CreatedUserId_Name_Programs");
            CreateIndex("dbo.Workouts", "CreatedUserId");
            CreateIndex("dbo.Workouts", "Name", unique: true);
            CreateIndex("dbo.Exercises", "CreatedUserId");
            CreateIndex("dbo.Exercises", "Name", unique: true);
            CreateIndex("dbo.Questions", "CreatedUserId");
            CreateIndex("dbo.Questions", "QuestionDisplayText", unique: true);
            CreateIndex("dbo.Surveys", "CreatedUserId");
            CreateIndex("dbo.Surveys", "Title", unique: true);
            CreateIndex("dbo.UnitOfMeasurements", "UnitType", unique: true);
            CreateIndex("dbo.Metrics", "Name", unique: true);
            CreateIndex("dbo.Programs", "CreatedUserId");
            CreateIndex("dbo.Programs", "Name", unique: true);
        }
    }
}
