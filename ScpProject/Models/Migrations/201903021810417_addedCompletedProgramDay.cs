namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedCompletedProgramDay : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompletedProgramDays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AthleteId = c.Int(nullable: false),
                        AssignedProgramId = c.Int(nullable: false),
                        ProgramDayId = c.Int(nullable: false),
                        Completed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedPrograms", t => t.AssignedProgramId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .ForeignKey("dbo.ProgramDays", t => t.ProgramDayId)
                .Index(t => t.AthleteId)
                .Index(t => t.AssignedProgramId)
                .Index(t => t.ProgramDayId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedProgramDays", "ProgramDayId", "dbo.ProgramDays");
            DropForeignKey("dbo.CompletedProgramDays", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.CompletedProgramDays", "AssignedProgramId", "dbo.AssignedPrograms");
            DropIndex("dbo.CompletedProgramDays", new[] { "ProgramDayId" });
            DropIndex("dbo.CompletedProgramDays", new[] { "AssignedProgramId" });
            DropIndex("dbo.CompletedProgramDays", new[] { "AthleteId" });
            DropTable("dbo.CompletedProgramDays");
        }
    }
}
