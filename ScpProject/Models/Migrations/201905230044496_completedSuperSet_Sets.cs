namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class completedSuperSet_Sets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompletedSuperSet_Set",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Sets = c.Int(nullable: false),
                        Reps = c.Int(nullable: false),
                        Percent = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        OriginalSuperSet_SetId = c.Int(nullable: false),
                        AthleteId = c.Int(nullable: false),
                        AssignedProgramId = c.Int(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedPrograms", t => t.AssignedProgramId)
                .ForeignKey("dbo.ProgramDayItemSuperSet_Set", t => t.OriginalSuperSet_SetId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.OriginalSuperSet_SetId)
                .Index(t => t.AthleteId)
                .Index(t => t.AssignedProgramId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedSuperSet_Set", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.CompletedSuperSet_Set", "OriginalSuperSet_SetId", "dbo.ProgramDayItemSuperSet_Set");
            DropForeignKey("dbo.CompletedSuperSet_Set", "AssignedProgramId", "dbo.AssignedPrograms");
            DropIndex("dbo.CompletedSuperSet_Set", new[] { "AssignedProgramId" });
            DropIndex("dbo.CompletedSuperSet_Set", new[] { "AthleteId" });
            DropIndex("dbo.CompletedSuperSet_Set", new[] { "OriginalSuperSet_SetId" });
            DropTable("dbo.CompletedSuperSet_Set");
        }
    }
}
