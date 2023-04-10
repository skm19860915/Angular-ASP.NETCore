namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class reAddedAllUniqueConstraints : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Questions", "IX_QuestionDisplayText_CreatedUserId_IsDeleted");
            AlterColumn("dbo.Programs", "Name", c => c.String(maxLength: 400));
            AlterColumn("dbo.Metrics", "Name", c => c.String(maxLength: 200));
            AlterColumn("dbo.UnitOfMeasurements", "UnitType", c => c.String(maxLength: 200));
            AlterColumn("dbo.Surveys", "Title", c => c.String(maxLength: 300));
            AlterColumn("dbo.Workouts", "Name", c => c.String(maxLength: 200));
            CreateIndex("dbo.Programs", "Name", unique: true);
            CreateIndex("dbo.Metrics", "Name", unique: true);
            CreateIndex("dbo.UnitOfMeasurements", "UnitType", unique: true);
            CreateIndex("dbo.Surveys", "Title", unique: true);
            CreateIndex("dbo.Questions", "QuestionDisplayText", unique: true);
            CreateIndex("dbo.Questions", "CreatedUserId");
            CreateIndex("dbo.Workouts", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Workouts", new[] { "Name" });
            DropIndex("dbo.Questions", new[] { "CreatedUserId" });
            DropIndex("dbo.Questions", new[] { "QuestionDisplayText" });
            DropIndex("dbo.Surveys", new[] { "Title" });
            DropIndex("dbo.UnitOfMeasurements", new[] { "UnitType" });
            DropIndex("dbo.Metrics", new[] { "Name" });
            DropIndex("dbo.Programs", new[] { "Name" });
            AlterColumn("dbo.Workouts", "Name", c => c.String());
            AlterColumn("dbo.Surveys", "Title", c => c.String());
            AlterColumn("dbo.UnitOfMeasurements", "UnitType", c => c.String());
            AlterColumn("dbo.Metrics", "Name", c => c.String());
            AlterColumn("dbo.Programs", "Name", c => c.String());
            CreateIndex("dbo.Questions", new[] { "CreatedUserId", "QuestionDisplayText", "IsDeleted" }, unique: true, name: "IX_QuestionDisplayText_CreatedUserId_IsDeleted");
        }
    }
}
