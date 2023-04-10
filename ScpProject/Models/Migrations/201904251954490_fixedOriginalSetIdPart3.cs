namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fixedOriginalSetIdPart3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompletedSets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Sets = c.Int(nullable: false),
                        Reps = c.Int(nullable: false),
                        Percent = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        OriginalSetId = c.Int(nullable: false),
                        AthleteId = c.Int(nullable: false),
                        AssignedProgramId = c.Int(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedPrograms", t => t.AssignedProgramId)
                .ForeignKey("dbo.ProgramSets", t => t.OriginalSetId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.OriginalSetId)
                .Index(t => t.AthleteId)
                .Index(t => t.AssignedProgramId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedSets", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.CompletedSets", "OriginalSetId", "dbo.ProgramSets");
            DropForeignKey("dbo.CompletedSets", "AssignedProgramId", "dbo.AssignedPrograms");
            DropIndex("dbo.CompletedSets", new[] { "AssignedProgramId" });
            DropIndex("dbo.CompletedSets", new[] { "AthleteId" });
            DropIndex("dbo.CompletedSets", new[] { "OriginalSetId" });
            DropTable("dbo.CompletedSets");
        }
    }
}
