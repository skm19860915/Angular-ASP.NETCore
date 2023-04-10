namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fixedOriginalSetIdPart2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CompletedSets", "AssignedProgramId", "dbo.AssignedPrograms");
            DropForeignKey("dbo.CompletedSets", "OriginalSetId", "dbo.ProgramSets");
            DropForeignKey("dbo.CompletedSets", "AthleteId", "dbo.Athletes");
            DropIndex("dbo.CompletedSets", new[] { "OriginalSetId" });
            DropIndex("dbo.CompletedSets", new[] { "AthleteId" });
            DropIndex("dbo.CompletedSets", new[] { "AssignedProgramId" });
            DropTable("dbo.CompletedSets");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.CompletedSets", "AssignedProgramId");
            CreateIndex("dbo.CompletedSets", "AthleteId");
            CreateIndex("dbo.CompletedSets", "OriginalSetId");
            AddForeignKey("dbo.CompletedSets", "AthleteId", "dbo.Athletes", "Id");
            AddForeignKey("dbo.CompletedSets", "OriginalSetId", "dbo.ProgramSets", "Id");
            AddForeignKey("dbo.CompletedSets", "AssignedProgramId", "dbo.AssignedPrograms", "Id");
        }
    }
}
