namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedWeekId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompletedQuestionOpenEndeds", "WeekId", c => c.Int(nullable: false));
            AddColumn("dbo.CompletedQuestionScales", "WeekId", c => c.Int(nullable: false));
            AddColumn("dbo.CompletedQuestionYesNoes", "WeekId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompletedQuestionYesNoes", "WeekId");
            DropColumn("dbo.CompletedQuestionScales", "WeekId");
            DropColumn("dbo.CompletedQuestionOpenEndeds", "WeekId");
        }
    }
}
