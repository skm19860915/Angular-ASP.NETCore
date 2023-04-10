namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingsurveyThresholds : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionThresholds",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScaleQuestionThresholds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        Comparer = c.Int(nullable: false),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.YesNoQuestionThresholds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        Threshold = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.YesNoQuestionThresholds");
            DropTable("dbo.ScaleQuestionThresholds");
            DropTable("dbo.QuestionThresholds");
        }
    }
}
