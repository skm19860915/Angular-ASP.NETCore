namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingHistoryToAssignedPrograms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AthleteProgramHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        ProgramId = c.Int(nullable: false),
                        AthleteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.AthleteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AthleteProgramHistories", "AthleteId", "dbo.Athletes");
            DropIndex("dbo.AthleteProgramHistories", new[] { "AthleteId" });
            DropTable("dbo.AthleteProgramHistories");
        }
    }
}
