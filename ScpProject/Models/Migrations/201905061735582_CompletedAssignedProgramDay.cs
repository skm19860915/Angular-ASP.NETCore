namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CompletedAssignedProgramDay : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompletedAssignedProgramDays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedProgramId = c.Int(nullable: false),
                        ProgramDayId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedPrograms", t => t.AssignedProgramId)
                .ForeignKey("dbo.ProgramDays", t => t.ProgramDayId)
                .Index(t => t.AssignedProgramId)
                .Index(t => t.ProgramDayId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedAssignedProgramDays", "ProgramDayId", "dbo.ProgramDays");
            DropForeignKey("dbo.CompletedAssignedProgramDays", "AssignedProgramId", "dbo.AssignedPrograms");
            DropIndex("dbo.CompletedAssignedProgramDays", new[] { "ProgramDayId" });
            DropIndex("dbo.CompletedAssignedProgramDays", new[] { "AssignedProgramId" });
            DropTable("dbo.CompletedAssignedProgramDays");
        }
    }
}
