namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedIsEmailValidedBit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsEmailValidated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "IsEmailValidated");
        }
    }
}
