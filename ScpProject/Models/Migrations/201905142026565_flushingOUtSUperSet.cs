namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class flushingOUtSUperSet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProgramDayItemSuperSet_Set",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Sets = c.Int(nullable: false),
                        Reps = c.Int(nullable: false),
                        Percent = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        ProgramDayItemSuperSetWeekId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramDayItemSuperSetWeeks", t => t.ProgramDayItemSuperSetWeekId)
                .Index(t => t.ProgramDayItemSuperSetWeekId);
            
            CreateTable(
                "dbo.ProgramDayItemSuperSetWeeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        SuperSetExerciseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SuperSetExercises", t => t.SuperSetExerciseId)
                .Index(t => t.SuperSetExerciseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProgramDayItemSuperSetWeeks", "SuperSetExerciseId", "dbo.SuperSetExercises");
            DropForeignKey("dbo.ProgramDayItemSuperSet_Set", "ProgramDayItemSuperSetWeekId", "dbo.ProgramDayItemSuperSetWeeks");
            DropIndex("dbo.ProgramDayItemSuperSetWeeks", new[] { "SuperSetExerciseId" });
            DropIndex("dbo.ProgramDayItemSuperSet_Set", new[] { "ProgramDayItemSuperSetWeekId" });
            DropTable("dbo.ProgramDayItemSuperSetWeeks");
            DropTable("dbo.ProgramDayItemSuperSet_Set");
        }
    }
}
