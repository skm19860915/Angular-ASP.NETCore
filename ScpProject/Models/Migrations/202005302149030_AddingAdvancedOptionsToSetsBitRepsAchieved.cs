namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddingAdvancedOptionsToSetsBitRepsAchieved : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sets", "RepsAchieved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sets", "RepsAchieved", c => c.Int(nullable: false));
        }
    }
}
