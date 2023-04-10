namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class renameCOlumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AthleteProgramHistories", "AssignedProgramId", c => c.Int(nullable: false));
            DropColumn("dbo.AthleteProgramHistories", "ProgramId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AthleteProgramHistories", "ProgramId", c => c.Int(nullable: false));
            DropColumn("dbo.AthleteProgramHistories", "AssignedProgramId");
        }
    }
}
