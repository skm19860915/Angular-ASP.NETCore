namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedUniqueKeyToPrograms : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Programs", "Name", c => c.String(maxLength: 300));
            CreateIndex("dbo.Programs", "Name", unique: true, name: "IX_Programs_Unique_Name");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Programs", "IX_Programs_Unique_Name");
            AlterColumn("dbo.Programs", "Name", c => c.String());
        }
    }
}
