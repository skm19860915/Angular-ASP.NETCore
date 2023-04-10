namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedArchiveDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AthleteProgramHistories", "ArchivedDate", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AthleteProgramHistories", "ArchivedDate");
        }
    }
}
