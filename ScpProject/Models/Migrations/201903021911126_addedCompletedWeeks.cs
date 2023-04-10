namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedCompletedWeeks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompletedProgramWeeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AthleteId = c.Int(nullable: false),
                        AssignedProgramId = c.Int(nullable: false),
                        ProgramWeek = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedPrograms", t => t.AssignedProgramId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.AthleteId)
                .Index(t => t.AssignedProgramId);
            
            DropColumn("dbo.CompletedProgramDays", "Completed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CompletedProgramDays", "Completed", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.CompletedProgramWeeks", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.CompletedProgramWeeks", "AssignedProgramId", "dbo.AssignedPrograms");
            DropIndex("dbo.CompletedProgramWeeks", new[] { "AssignedProgramId" });
            DropIndex("dbo.CompletedProgramWeeks", new[] { "AthleteId" });
            DropTable("dbo.CompletedProgramWeeks");
        }
    }
}
