namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedCanMOdify : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Metrics", "IX_Metric_Name_CreatedUserId_IsDeleted");
            DropIndex("dbo.Surveys", "IX_Survey_Title_CreatedUserId_IsDeleted");
            DropIndex("dbo.Workouts", "IX_Workout_Name_CreatedUserId_IsDeleted");
            AddColumn("dbo.Programs", "CanModify", c => c.Boolean(nullable: false));
            AddColumn("dbo.Metrics", "CanModify", c => c.Boolean(nullable: false));
            AddColumn("dbo.Surveys", "CanModify", c => c.Boolean(nullable: false));
            AddColumn("dbo.Questions", "CanModify", c => c.Boolean(nullable: false));
            AddColumn("dbo.Exercises", "CanModify", c => c.Boolean(nullable: false));
            AddColumn("dbo.Workouts", "CanModify", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Metrics", new[] { "CreatedUserId", "Name", "CanModify" }, unique: true, name: "IX_Metric_Name_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Surveys", new[] { "CreatedUserId", "Title", "CanModify" }, unique: true, name: "IX_Survey_Title_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Workouts", new[] { "CreatedUserId", "Name", "CanModify" }, unique: true, name: "IX_Workout_Name_CreatedUserId_IsDeleted");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Workouts", "IX_Workout_Name_CreatedUserId_IsDeleted");
            DropIndex("dbo.Surveys", "IX_Survey_Title_CreatedUserId_IsDeleted");
            DropIndex("dbo.Metrics", "IX_Metric_Name_CreatedUserId_IsDeleted");
            DropColumn("dbo.Workouts", "CanModify");
            DropColumn("dbo.Exercises", "CanModify");
            DropColumn("dbo.Questions", "CanModify");
            DropColumn("dbo.Surveys", "CanModify");
            DropColumn("dbo.Metrics", "CanModify");
            DropColumn("dbo.Programs", "CanModify");
            CreateIndex("dbo.Workouts", new[] { "CreatedUserId", "Name", "IsDeleted" }, unique: true, name: "IX_Workout_Name_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Surveys", new[] { "CreatedUserId", "Title", "IsDeleted" }, unique: true, name: "IX_Survey_Title_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Metrics", new[] { "CreatedUserId", "Name", "IsDeleted" }, unique: true, name: "IX_Metric_Name_CreatedUserId_IsDeleted");
        }
    }
}
