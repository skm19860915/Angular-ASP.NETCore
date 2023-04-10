namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class orgIntNotNull : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Programs", new[] { "OrganizationId" });
            DropIndex("dbo.Users", new[] { "OrganizationId" });
            DropIndex("dbo.Athletes", new[] { "OrganizationId" });
            DropIndex("dbo.Metrics", new[] { "OrganizationId" });
            DropIndex("dbo.UnitOfMeasurements", new[] { "OrganizationId" });
            DropIndex("dbo.Surveys", new[] { "OrganizationId" });
            DropIndex("dbo.Questions", new[] { "OrganizationId" });
            DropIndex("dbo.Exercises", new[] { "OrganizationId" });
            DropIndex("dbo.Workouts", new[] { "OrganizationId" });
            AlterColumn("dbo.Programs", "OrganizationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "OrganizationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Athletes", "OrganizationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Metrics", "OrganizationId", c => c.Int(nullable: false));
            AlterColumn("dbo.UnitOfMeasurements", "OrganizationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Surveys", "OrganizationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Questions", "OrganizationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Exercises", "OrganizationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Workouts", "OrganizationId", c => c.Int(nullable: false));
            AlterColumn("dbo.UserToOrganizationRoles", "OrganizationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Programs", "OrganizationId");
            CreateIndex("dbo.Users", "OrganizationId");
            CreateIndex("dbo.Athletes", "OrganizationId");
            CreateIndex("dbo.Metrics", "OrganizationId");
            CreateIndex("dbo.UnitOfMeasurements", "OrganizationId");
            CreateIndex("dbo.Surveys", "OrganizationId");
            CreateIndex("dbo.Questions", "OrganizationId");
            CreateIndex("dbo.Exercises", "OrganizationId");
            CreateIndex("dbo.Workouts", "OrganizationId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Workouts", new[] { "OrganizationId" });
            DropIndex("dbo.Exercises", new[] { "OrganizationId" });
            DropIndex("dbo.Questions", new[] { "OrganizationId" });
            DropIndex("dbo.Surveys", new[] { "OrganizationId" });
            DropIndex("dbo.UnitOfMeasurements", new[] { "OrganizationId" });
            DropIndex("dbo.Metrics", new[] { "OrganizationId" });
            DropIndex("dbo.Athletes", new[] { "OrganizationId" });
            DropIndex("dbo.Users", new[] { "OrganizationId" });
            DropIndex("dbo.Programs", new[] { "OrganizationId" });
            AlterColumn("dbo.UserToOrganizationRoles", "OrganizationId", c => c.Int());
            AlterColumn("dbo.Workouts", "OrganizationId", c => c.Int());
            AlterColumn("dbo.Exercises", "OrganizationId", c => c.Int());
            AlterColumn("dbo.Questions", "OrganizationId", c => c.Int());
            AlterColumn("dbo.Surveys", "OrganizationId", c => c.Int());
            AlterColumn("dbo.UnitOfMeasurements", "OrganizationId", c => c.Int());
            AlterColumn("dbo.Metrics", "OrganizationId", c => c.Int());
            AlterColumn("dbo.Athletes", "OrganizationId", c => c.Int());
            AlterColumn("dbo.Users", "OrganizationId", c => c.Int());
            AlterColumn("dbo.Programs", "OrganizationId", c => c.Int());
            CreateIndex("dbo.Workouts", "OrganizationId");
            CreateIndex("dbo.Exercises", "OrganizationId");
            CreateIndex("dbo.Questions", "OrganizationId");
            CreateIndex("dbo.Surveys", "OrganizationId");
            CreateIndex("dbo.UnitOfMeasurements", "OrganizationId");
            CreateIndex("dbo.Metrics", "OrganizationId");
            CreateIndex("dbo.Athletes", "OrganizationId");
            CreateIndex("dbo.Users", "OrganizationId");
            CreateIndex("dbo.Programs", "OrganizationId");
        }
    }
}
