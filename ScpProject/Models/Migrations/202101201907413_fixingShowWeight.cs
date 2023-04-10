namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fixingShowWeight : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Sets", "ShowWeight");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sets", "ShowWeight", c => c.Boolean(nullable: false));
        }
    }
}
