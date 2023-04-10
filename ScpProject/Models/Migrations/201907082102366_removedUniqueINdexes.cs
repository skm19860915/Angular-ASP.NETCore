namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class removedUniqueINdexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Programs", "IX_Programs_Unique_Name");
            DropIndex("dbo.Metrics", "IX_Metric_Name_CreatedUserId_IsDeleted");
            DropIndex("dbo.Surveys", "IX_Survey_Title_CreatedUserId_IsDeleted");
            DropIndex("dbo.Exercises", "IX_Exercise_Name_CreatedUserId_IsDeleted");
            DropIndex("dbo.Workouts", "IX_Workout_Name_CreatedUserId_IsDeleted");
            AlterColumn("dbo.Programs", "Name", c => c.String());
            AlterColumn("dbo.Metrics", "Name", c => c.String());
            AlterColumn("dbo.Surveys", "Title", c => c.String());
            AlterColumn("dbo.Exercises", "Name", c => c.String());
            AlterColumn("dbo.Workouts", "Name", c => c.String());
            CreateIndex("dbo.Surveys", "CreatedUserId");
            CreateIndex("dbo.Exercises", "CreatedUserId");
            CreateIndex("dbo.Workouts", "CreatedUserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Workouts", new[] { "CreatedUserId" });
            DropIndex("dbo.Exercises", new[] { "CreatedUserId" });
            DropIndex("dbo.Surveys", new[] { "CreatedUserId" });
            AlterColumn("dbo.Workouts", "Name", c => c.String(maxLength: 100));
            AlterColumn("dbo.Exercises", "Name", c => c.String(maxLength: 100));
            AlterColumn("dbo.Surveys", "Title", c => c.String(maxLength: 100));
            AlterColumn("dbo.Metrics", "Name", c => c.String(maxLength: 300));
            AlterColumn("dbo.Programs", "Name", c => c.String(maxLength: 100));
            CreateIndex("dbo.Workouts", new[] { "CreatedUserId", "Name", "CanModify" }, unique: true, name: "IX_Workout_Name_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Exercises", new[] { "CreatedUserId", "Name", "IsDeleted" }, unique: true, name: "IX_Exercise_Name_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Surveys", new[] { "CreatedUserId", "Title", "CanModify" }, unique: true, name: "IX_Survey_Title_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Metrics", new[] { "CreatedUserId", "Name", "CanModify" }, unique: true, name: "IX_Metric_Name_CreatedUserId_IsDeleted");
            CreateIndex("dbo.Programs", "Name", unique: true, name: "IX_Programs_Unique_Name");
        }
    }
}
