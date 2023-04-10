namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingSuperSet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProgramDayItemSuperSets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProgramDayItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramDayItems", t => t.ProgramDayItemId)
                .Index(t => t.ProgramDayItemId);
            
            CreateTable(
                "dbo.SuperSetExercises",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProgramDayItemSuperSetId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        ExerciseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Exercises", t => t.ExerciseId)
                .ForeignKey("dbo.ProgramDayItemSuperSets", t => t.ProgramDayItemSuperSetId)
                .Index(t => t.ProgramDayItemSuperSetId)
                .Index(t => t.ExerciseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SuperSetExercises", "ProgramDayItemSuperSetId", "dbo.ProgramDayItemSuperSets");
            DropForeignKey("dbo.SuperSetExercises", "ExerciseId", "dbo.Exercises");
            DropForeignKey("dbo.ProgramDayItemSuperSets", "ProgramDayItemId", "dbo.ProgramDayItems");
            DropIndex("dbo.SuperSetExercises", new[] { "ExerciseId" });
            DropIndex("dbo.SuperSetExercises", new[] { "ProgramDayItemSuperSetId" });
            DropIndex("dbo.ProgramDayItemSuperSets", new[] { "ProgramDayItemId" });
            DropTable("dbo.SuperSetExercises");
            DropTable("dbo.ProgramDayItemSuperSets");
        }
    }
}
