namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class bb : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Organizations", "IX_Organization");
            AlterColumn("dbo.Organizations", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Organizations", "Name", c => c.String(maxLength: 300));
            CreateIndex("dbo.Organizations", "Name", unique: true, name: "IX_Organization");
        }
    }
}
