namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class testingUniqueConstraintsAgain : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Exercises", "Name", c => c.String(maxLength: 200));
            CreateIndex("dbo.Exercises", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Exercises", new[] { "Name" });
            AlterColumn("dbo.Exercises", "Name", c => c.String());
        }
    }
}
