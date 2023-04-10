namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class _addedMappingsForOrgStuf : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 300),
                        CreatedUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_Organization");
            
            CreateTable(
                "dbo.OrganizationRoles",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserToOrganizationRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        OrganizationRoleId = c.Int(nullable: false),
                        OrganizationId = c.Int(),
                        AssignedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AssignedByUserId)
                .ForeignKey("dbo.OrganizationRoles", t => t.OrganizationRoleId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.OrganizationRoleId)
                .Index(t => t.AssignedByUserId);
            
            AddColumn("dbo.Programs", "OrganizationId", c => c.Int());
            AddColumn("dbo.Users", "IsHeadCoach", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "OrganizationId", c => c.Int());
            AddColumn("dbo.Athletes", "OrganizationId", c => c.Int());
            AddColumn("dbo.Metrics", "OrganizationId", c => c.Int());
            AddColumn("dbo.UnitOfMeasurements", "OrganizationId", c => c.Int());
            AddColumn("dbo.Surveys", "OrganizationId", c => c.Int());
            AddColumn("dbo.Questions", "OrganizationId", c => c.Int());
            AddColumn("dbo.Exercises", "OrganizationId", c => c.Int());
            AddColumn("dbo.Workouts", "OrganizationId", c => c.Int());
            CreateIndex("dbo.Programs", "OrganizationId");
            CreateIndex("dbo.Users", "OrganizationId");
            CreateIndex("dbo.Athletes", "OrganizationId");
            CreateIndex("dbo.Metrics", "OrganizationId");
            CreateIndex("dbo.UnitOfMeasurements", "OrganizationId");
            CreateIndex("dbo.Surveys", "OrganizationId");
            CreateIndex("dbo.Questions", "OrganizationId");
            CreateIndex("dbo.Exercises", "OrganizationId");
            CreateIndex("dbo.Workouts", "OrganizationId");
            AddForeignKey("dbo.Users", "OrganizationId", "dbo.Organizations", "Id");
            AddForeignKey("dbo.Programs", "OrganizationId", "dbo.Organizations", "Id");
            AddForeignKey("dbo.Athletes", "OrganizationId", "dbo.Organizations", "Id");
            AddForeignKey("dbo.UnitOfMeasurements", "OrganizationId", "dbo.Organizations", "Id");
            AddForeignKey("dbo.Metrics", "OrganizationId", "dbo.Organizations", "Id");
            AddForeignKey("dbo.Surveys", "OrganizationId", "dbo.Organizations", "Id");
            AddForeignKey("dbo.Questions", "OrganizationId", "dbo.Organizations", "Id");
            AddForeignKey("dbo.Exercises", "OrganizationId", "dbo.Organizations", "Id");
            AddForeignKey("dbo.Workouts", "OrganizationId", "dbo.Organizations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserToOrganizationRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserToOrganizationRoles", "OrganizationRoleId", "dbo.OrganizationRoles");
            DropForeignKey("dbo.UserToOrganizationRoles", "AssignedByUserId", "dbo.Users");
            DropForeignKey("dbo.Workouts", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Exercises", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Questions", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Surveys", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Metrics", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.UnitOfMeasurements", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Athletes", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Programs", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Users", "OrganizationId", "dbo.Organizations");
            DropIndex("dbo.UserToOrganizationRoles", new[] { "AssignedByUserId" });
            DropIndex("dbo.UserToOrganizationRoles", new[] { "OrganizationRoleId" });
            DropIndex("dbo.UserToOrganizationRoles", new[] { "UserId" });
            DropIndex("dbo.Workouts", new[] { "OrganizationId" });
            DropIndex("dbo.Exercises", new[] { "OrganizationId" });
            DropIndex("dbo.Questions", new[] { "OrganizationId" });
            DropIndex("dbo.Surveys", new[] { "OrganizationId" });
            DropIndex("dbo.UnitOfMeasurements", new[] { "OrganizationId" });
            DropIndex("dbo.Metrics", new[] { "OrganizationId" });
            DropIndex("dbo.Athletes", new[] { "OrganizationId" });
            DropIndex("dbo.Organizations", "IX_Organization");
            DropIndex("dbo.Users", new[] { "OrganizationId" });
            DropIndex("dbo.Programs", new[] { "OrganizationId" });
            DropColumn("dbo.Workouts", "OrganizationId");
            DropColumn("dbo.Exercises", "OrganizationId");
            DropColumn("dbo.Questions", "OrganizationId");
            DropColumn("dbo.Surveys", "OrganizationId");
            DropColumn("dbo.UnitOfMeasurements", "OrganizationId");
            DropColumn("dbo.Metrics", "OrganizationId");
            DropColumn("dbo.Athletes", "OrganizationId");
            DropColumn("dbo.Users", "OrganizationId");
            DropColumn("dbo.Users", "IsHeadCoach");
            DropColumn("dbo.Programs", "OrganizationId");
            DropTable("dbo.UserToOrganizationRoles");
            DropTable("dbo.OrganizationRoles");
            DropTable("dbo.Organizations");
        }
    }
}
