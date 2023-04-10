namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingIXToTags : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AthleteTags", "Name", c => c.String(maxLength: 200));
            AlterColumn("dbo.ExerciseTags", "Name", c => c.String(maxLength: 200));
            AlterColumn("dbo.MetricTags", "Name", c => c.String(maxLength: 200));
            AlterColumn("dbo.MovieTags", "Name", c => c.String(maxLength: 200));
            AlterColumn("dbo.MultiMediaTags", "Name", c => c.String(maxLength: 200));
            AlterColumn("dbo.ProgramTags", "Name", c => c.String(maxLength: 200));
            AlterColumn("dbo.SurveyTags", "Name", c => c.String(maxLength: 200));
            AlterColumn("dbo.WorkoutTags", "Name", c => c.String(maxLength: 200));
            CreateIndex("dbo.AthleteTags", new[] { "Name", "OrganizationId" }, unique: true, name: "IX_UniqueTag");
            CreateIndex("dbo.ExerciseTags", new[] { "Name", "OrganizationId" }, unique: true, name: "IX_UniqueTag");
            CreateIndex("dbo.MetricTags", new[] { "Name", "OrganizationId" }, unique: true, name: "IX_UniqueTag");
            CreateIndex("dbo.MovieTags", new[] { "Name", "OrganizationId" }, unique: true, name: "IX_UniqueTag");
            CreateIndex("dbo.MultiMediaTags", new[] { "Name", "OrganizationId" }, unique: true, name: "IX_UniqueTag");
            CreateIndex("dbo.ProgramTags", new[] { "Name", "OrganizationId" }, unique: true, name: "IX_UniqueTag");
            CreateIndex("dbo.SurveyTags", new[] { "Name", "OrganizationId" }, unique: true, name: "IX_UniqueTag");
            CreateIndex("dbo.WorkoutTags", new[] { "Name", "OrganizationId" }, unique: true, name: "IX_UniqueTag");
        }
        
        public override void Down()
        {
            DropIndex("dbo.WorkoutTags", "IX_UniqueTag");
            DropIndex("dbo.SurveyTags", "IX_UniqueTag");
            DropIndex("dbo.ProgramTags", "IX_UniqueTag");
            DropIndex("dbo.MultiMediaTags", "IX_UniqueTag");
            DropIndex("dbo.MovieTags", "IX_UniqueTag");
            DropIndex("dbo.MetricTags", "IX_UniqueTag");
            DropIndex("dbo.ExerciseTags", "IX_UniqueTag");
            DropIndex("dbo.AthleteTags", "IX_UniqueTag");
            AlterColumn("dbo.WorkoutTags", "Name", c => c.String());
            AlterColumn("dbo.SurveyTags", "Name", c => c.String());
            AlterColumn("dbo.ProgramTags", "Name", c => c.String());
            AlterColumn("dbo.MultiMediaTags", "Name", c => c.String());
            AlterColumn("dbo.MovieTags", "Name", c => c.String());
            AlterColumn("dbo.MetricTags", "Name", c => c.String());
            AlterColumn("dbo.ExerciseTags", "Name", c => c.String());
            AlterColumn("dbo.AthleteTags", "Name", c => c.String());
        }
    }
}
