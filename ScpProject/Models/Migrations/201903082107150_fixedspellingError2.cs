namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fixedspellingError2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompletedQuestionOpenEndeds", "Response", c => c.String());
            DropColumn("dbo.CompletedQuestionOpenEndeds", "Resposne");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CompletedQuestionOpenEndeds", "Resposne", c => c.String());
            DropColumn("dbo.CompletedQuestionOpenEndeds", "Response");
        }
    }
}
