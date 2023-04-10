namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedIsDeletedToWhatsNeeded : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Programs", "IX_Programs_Unique_Name");
            DropIndex("dbo.Surveys", "IX_Survey_Title_CreatedUserId");
            DropIndex("dbo.Questions", "IX_QuestionDisplayText_CreatedUserId");
            DropIndex("dbo.Exercises", "IX_Exercise_Name_CreatedUserId");
            DropIndex("dbo.Workouts", new[] { "CreatedUserId" });
            AddColumn("dbo.Programs", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Metrics", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Surveys", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Questions", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Workouts", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Programs", "Name", c => c.String(maxLength: 100));
            AlterColumn("dbo.Metrics", "Name", c => c.String(maxLength: 300));
            AlterColumn("dbo.Workouts", "Name", c => c.String(maxLength: 100));
            CreateIndex("dbo.Programs", "Name", unique: true, name: "IX_Programs_Unique_Name");
            CreateIndex("dbo.Metrics", new[] { "CreatedUserId", "Name", "IsDeleted" }, unique: true, name: "IX_Metric_Name_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Surveys", new[] { "CreatedUserId", "Title", "IsDeleted" }, unique: true, name: "IX_Survey_Title_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Questions", new[] { "CreatedUserId", "QuestionDisplayText", "IsDeleted" }, unique: true, name: "IX_QuestionDisplayText_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Exercises", new[] { "CreatedUserId", "Name", "IsDeleted" }, unique: true, name: "IX_Exercise_Name_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Workouts", new[] { "CreatedUserId", "Name", "IsDeleted" }, unique: true, name: "IX_Workout_Name_CreatedUserId_IsDeleted");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Workouts", "IX_Workout_Name_CreatedUserId_IsDeleted");
            DropIndex("dbo.Exercises", "IX_Exercise_Name_CreatedUserId_IsDeleted");
            DropIndex("dbo.Questions", "IX_QuestionDisplayText_CreatedUserId_IsDeleted");
            DropIndex("dbo.Surveys", "IX_Survey_Title_CreatedUserId_IsDeleted");
            DropIndex("dbo.Metrics", "IX_Metric_Name_CreatedUserId_IsDeleted");
            DropIndex("dbo.Programs", "IX_Programs_Unique_Name");
            AlterColumn("dbo.Workouts", "Name", c => c.String());
            AlterColumn("dbo.Metrics", "Name", c => c.String());
            AlterColumn("dbo.Programs", "Name", c => c.String(maxLength: 300));
            DropColumn("dbo.Workouts", "IsDeleted");
            DropColumn("dbo.Questions", "IsDeleted");
            DropColumn("dbo.Surveys", "IsDeleted");
            DropColumn("dbo.Metrics", "IsDeleted");
            DropColumn("dbo.Programs", "IsDeleted");
            CreateIndex("dbo.Workouts", "CreatedUserId");
            CreateIndex("dbo.Exercises", new[] { "CreatedUserId", "Name" }, unique: true, name: "IX_Exercise_Name_CreatedUserId");
            CreateIndex("dbo.Questions", new[] { "CreatedUserId", "QuestionDisplayText" }, unique: true, name: "IX_QuestionDisplayText_CreatedUserId");
            CreateIndex("dbo.Surveys", new[] { "CreatedUserId", "Title" }, unique: true, name: "IX_Survey_Title_CreatedUserId");
            CreateIndex("dbo.Programs", "Name", unique: true, name: "IX_Programs_Unique_Name");
        }
    }
}
