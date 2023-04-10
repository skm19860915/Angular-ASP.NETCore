namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedArchiveDate1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AthleteProgramHistories", "ArchivedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AthleteProgramHistories", "ArchivedDate", c => c.Int(nullable: false));
        }
    }
}
