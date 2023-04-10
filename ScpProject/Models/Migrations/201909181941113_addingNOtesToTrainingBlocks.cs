namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingNOtesToTrainingBlocks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SuperSetNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Note = c.Int(nullable: false),
                        ProgramDayItemSuperSetId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramDayItemSuperSets", t => t.ProgramDayItemSuperSetId)
                .Index(t => t.ProgramDayItemSuperSetId);
            
            CreateTable(
                "dbo.SuperSetNoteDisplayWeeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SuperSetNoteId = c.Int(nullable: false),
                        DisplayWeek = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SuperSetNotes", t => t.SuperSetNoteId)
                .Index(t => t.SuperSetNoteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SuperSetNoteDisplayWeeks", "SuperSetNoteId", "dbo.SuperSetNotes");
            DropForeignKey("dbo.SuperSetNotes", "ProgramDayItemSuperSetId", "dbo.ProgramDayItemSuperSets");
            DropIndex("dbo.SuperSetNoteDisplayWeeks", new[] { "SuperSetNoteId" });
            DropIndex("dbo.SuperSetNotes", new[] { "ProgramDayItemSuperSetId" });
            DropTable("dbo.SuperSetNoteDisplayWeeks");
            DropTable("dbo.SuperSetNotes");
        }
    }
}
