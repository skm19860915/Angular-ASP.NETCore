namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v1_Of_ProgramSnapShots : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignedProgram_AssignedProgramHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedDate = c.DateTime(nullable: false),
                        AssignedProgram_ProgramId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_Program", t => t.AssignedProgram_ProgramId)
                .Index(t => new { t.Id, t.AssignedDate }, unique: true, name: "IX_AssignedProgram")
                .Index(t => t.AssignedProgram_ProgramId);
            
            CreateTable(
                "dbo.AssignedProgram_Program",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 400),
                        CreatedUserId = c.Int(nullable: false),
                        WeekCount = c.Int(nullable: false),
                        DayCount = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => new { t.CreatedUserId, t.Name }, unique: true, name: "IX_CreatedUserId_Name_Programs")
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.AssignedProgram_CompletedQuestionOpenEnded",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Response = c.String(),
                        AthleteId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        AssignedProgram_ProgramId = c.Int(nullable: false),
                        AssignedProgram_ProgramDayItemSurveyId = c.Int(nullable: false),
                        WeekId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedPrograms", t => t.AssignedProgram_ProgramId)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItemSurvey", t => t.AssignedProgram_ProgramDayItemSurveyId)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.AthleteId)
                .Index(t => t.QuestionId)
                .Index(t => t.AssignedProgram_ProgramId)
                .Index(t => t.AssignedProgram_ProgramDayItemSurveyId);
            
            CreateTable(
                "dbo.AssignedProgram_ProgramDayItemSurvey",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        AssignedProgram_ProgramDayItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Surveys", t => t.SurveyId)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItem", t => t.AssignedProgram_ProgramDayItemId)
                .Index(t => t.SurveyId)
                .Index(t => t.AssignedProgram_ProgramDayItemId);
            
            CreateTable(
                "dbo.AssignedProgram_ProgramDayItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedProgram_ProgramDayId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        ItemEnum = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_ProgramDay", t => t.AssignedProgram_ProgramDayId)
                .Index(t => t.AssignedProgram_ProgramDayId);
            
            CreateTable(
                "dbo.AssignedProgram_ProgramDay",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        AssignedProgram_ProgramId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_Program", t => t.AssignedProgram_ProgramId)
                .Index(t => t.AssignedProgram_ProgramId);
            
            CreateTable(
                "dbo.AssignedProgram_CompletedQuestionScale",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ScaleValue = c.Int(nullable: false),
                        AthleteId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        AssignedProgram_ProgramId = c.Int(nullable: false),
                        AssignedProgram_ProgramDayItemSurveyId = c.Int(nullable: false),
                        WeekId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedPrograms", t => t.AssignedProgram_ProgramId)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItemSurvey", t => t.AssignedProgram_ProgramDayItemSurveyId)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.AthleteId)
                .Index(t => t.QuestionId)
                .Index(t => t.AssignedProgram_ProgramId)
                .Index(t => t.AssignedProgram_ProgramDayItemSurveyId);
            
            CreateTable(
                "dbo.AssignedProgram_CompletedQuestionYesNo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YesNoValue = c.Boolean(nullable: false),
                        AthleteId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        AssignedProgram_ProgramId = c.Int(nullable: false),
                        AssignedProgram_ProgramDayItemSurveyId = c.Int(nullable: false),
                        WeekId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_Program", t => t.AssignedProgram_ProgramId)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItemSurvey", t => t.AssignedProgram_ProgramDayItemSurveyId)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.AthleteId)
                .Index(t => t.QuestionId)
                .Index(t => t.AssignedProgram_ProgramId)
                .Index(t => t.AssignedProgram_ProgramDayItemSurveyId);
            
            CreateTable(
                "dbo.AssignedProgram_ProgramDayItemMetric",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MetricId = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        AssignedProgram_ProgramDayItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Metrics", t => t.MetricId)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItem", t => t.AssignedProgram_ProgramDayItemId)
                .Index(t => t.MetricId)
                .Index(t => t.AssignedProgram_ProgramDayItemId);
            
            CreateTable(
                "dbo.AssignedProgram_ProgramDayItemMovie",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MovieId = c.Int(nullable: false),
                        AssignedProgram_ProgramDayItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Movies", t => t.MovieId)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItem", t => t.AssignedProgram_ProgramDayItemId)
                .Index(t => t.MovieId)
                .Index(t => t.AssignedProgram_ProgramDayItemId);
            
            CreateTable(
                "dbo.AssignedProgram_ProgramDayItemSuperSet_Set",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Sets = c.Int(),
                        Reps = c.Int(),
                        Percent = c.Double(),
                        Weight = c.Double(),
                        Minutes = c.Int(),
                        Seconds = c.Int(),
                        Distance = c.String(),
                        RepsAchieved = c.Boolean(),
                        Other = c.String(),
                        Completed_Sets = c.Int(nullable: false),
                        Completed_Reps = c.Int(nullable: false),
                        Completed_Weight = c.Double(nullable: false),
                        Completed_RepsAchieved = c.Int(nullable: false),
                        LastCompletedUpdateTime = c.DateTime(nullable: false),
                        AssignedProgram_ProgramDayItemSuperSetWeekId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItemSuperSetWeek", t => t.AssignedProgram_ProgramDayItemSuperSetWeekId)
                .Index(t => t.AssignedProgram_ProgramDayItemSuperSetWeekId);
            
            CreateTable(
                "dbo.AssignedProgram_ProgramDayItemSuperSetWeek",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        AssignedProgram_SuperSetExerciseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_SuperSetExercise", t => t.AssignedProgram_SuperSetExerciseId)
                .Index(t => t.AssignedProgram_SuperSetExerciseId);
            
            CreateTable(
                "dbo.AssignedProgram_SuperSetExercise",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedProgram_ProgramDayItemSuperSetId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        ExerciseId = c.Int(nullable: false),
                        Rest = c.String(),
                        ShowWeight = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Exercises", t => t.ExerciseId)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItemSuperSet", t => t.AssignedProgram_ProgramDayItemSuperSetId)
                .Index(t => t.AssignedProgram_ProgramDayItemSuperSetId)
                .Index(t => t.ExerciseId);
            
            CreateTable(
                "dbo.AssignedProgram_ProgramDayItemSuperSet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedProgram_ProgramDayItemId = c.Int(nullable: false),
                        SuperSetDisplayTitle = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItem", t => t.AssignedProgram_ProgramDayItemId)
                .Index(t => t.AssignedProgram_ProgramDayItemId);
            
            CreateTable(
                "dbo.AssignedProgram_SuperSetNote",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Note = c.String(),
                        AssignedProgram_ProgramDayItemSuperSetId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItemSuperSet", t => t.AssignedProgram_ProgramDayItemSuperSetId)
                .Index(t => t.AssignedProgram_ProgramDayItemSuperSetId);
            
            CreateTable(
                "dbo.AssignedProgram_SuperSetNoteDisplayWeek",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedProgram_SuperSetNoteId = c.Int(nullable: false),
                        DisplayWeek = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_SuperSetNote", t => t.AssignedProgram_SuperSetNoteId)
                .Index(t => t.AssignedProgram_SuperSetNoteId);
            
            AddColumn("dbo.Athletes", "AssignedProgram_AssignedTime", c => c.DateTime());
            AddColumn("dbo.Athletes", "AssignedProgram_AssignedProgramId", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssignedProgram_SuperSetNoteDisplayWeek", "AssignedProgram_SuperSetNoteId", "dbo.AssignedProgram_SuperSetNote");
            DropForeignKey("dbo.AssignedProgram_SuperSetNote", "AssignedProgram_ProgramDayItemSuperSetId", "dbo.AssignedProgram_ProgramDayItemSuperSet");
            DropForeignKey("dbo.AssignedProgram_ProgramDayItemSuperSetWeek", "AssignedProgram_SuperSetExerciseId", "dbo.AssignedProgram_SuperSetExercise");
            DropForeignKey("dbo.AssignedProgram_SuperSetExercise", "AssignedProgram_ProgramDayItemSuperSetId", "dbo.AssignedProgram_ProgramDayItemSuperSet");
            DropForeignKey("dbo.AssignedProgram_ProgramDayItemSuperSet", "AssignedProgram_ProgramDayItemId", "dbo.AssignedProgram_ProgramDayItem");
            DropForeignKey("dbo.AssignedProgram_SuperSetExercise", "ExerciseId", "dbo.Exercises");
            DropForeignKey("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "AssignedProgram_ProgramDayItemSuperSetWeekId", "dbo.AssignedProgram_ProgramDayItemSuperSetWeek");
            DropForeignKey("dbo.AssignedProgram_ProgramDayItemMovie", "AssignedProgram_ProgramDayItemId", "dbo.AssignedProgram_ProgramDayItem");
            DropForeignKey("dbo.AssignedProgram_ProgramDayItemMovie", "MovieId", "dbo.Movies");
            DropForeignKey("dbo.AssignedProgram_ProgramDayItemMetric", "AssignedProgram_ProgramDayItemId", "dbo.AssignedProgram_ProgramDayItem");
            DropForeignKey("dbo.AssignedProgram_ProgramDayItemMetric", "MetricId", "dbo.Metrics");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionYesNo", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionYesNo", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionYesNo", "AssignedProgram_ProgramDayItemSurveyId", "dbo.AssignedProgram_ProgramDayItemSurvey");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionYesNo", "AssignedProgram_ProgramId", "dbo.AssignedProgram_Program");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionScale", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionScale", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionScale", "AssignedProgram_ProgramDayItemSurveyId", "dbo.AssignedProgram_ProgramDayItemSurvey");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionScale", "AssignedProgram_ProgramId", "dbo.AssignedPrograms");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionOpenEnded", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionOpenEnded", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionOpenEnded", "AssignedProgram_ProgramDayItemSurveyId", "dbo.AssignedProgram_ProgramDayItemSurvey");
            DropForeignKey("dbo.AssignedProgram_ProgramDayItemSurvey", "AssignedProgram_ProgramDayItemId", "dbo.AssignedProgram_ProgramDayItem");
            DropForeignKey("dbo.AssignedProgram_ProgramDayItem", "AssignedProgram_ProgramDayId", "dbo.AssignedProgram_ProgramDay");
            DropForeignKey("dbo.AssignedProgram_ProgramDay", "AssignedProgram_ProgramId", "dbo.AssignedProgram_Program");
            DropForeignKey("dbo.AssignedProgram_ProgramDayItemSurvey", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionOpenEnded", "AssignedProgram_ProgramId", "dbo.AssignedPrograms");
            DropForeignKey("dbo.AssignedProgram_AssignedProgramHistory", "AssignedProgram_ProgramId", "dbo.AssignedProgram_Program");
            DropForeignKey("dbo.AssignedProgram_Program", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.AssignedProgram_Program", "CreatedUserId", "dbo.Users");
            DropIndex("dbo.AssignedProgram_SuperSetNoteDisplayWeek", new[] { "AssignedProgram_SuperSetNoteId" });
            DropIndex("dbo.AssignedProgram_SuperSetNote", new[] { "AssignedProgram_ProgramDayItemSuperSetId" });
            DropIndex("dbo.AssignedProgram_ProgramDayItemSuperSet", new[] { "AssignedProgram_ProgramDayItemId" });
            DropIndex("dbo.AssignedProgram_SuperSetExercise", new[] { "ExerciseId" });
            DropIndex("dbo.AssignedProgram_SuperSetExercise", new[] { "AssignedProgram_ProgramDayItemSuperSetId" });
            DropIndex("dbo.AssignedProgram_ProgramDayItemSuperSetWeek", new[] { "AssignedProgram_SuperSetExerciseId" });
            DropIndex("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", new[] { "AssignedProgram_ProgramDayItemSuperSetWeekId" });
            DropIndex("dbo.AssignedProgram_ProgramDayItemMovie", new[] { "AssignedProgram_ProgramDayItemId" });
            DropIndex("dbo.AssignedProgram_ProgramDayItemMovie", new[] { "MovieId" });
            DropIndex("dbo.AssignedProgram_ProgramDayItemMetric", new[] { "AssignedProgram_ProgramDayItemId" });
            DropIndex("dbo.AssignedProgram_ProgramDayItemMetric", new[] { "MetricId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionYesNo", new[] { "AssignedProgram_ProgramDayItemSurveyId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionYesNo", new[] { "AssignedProgram_ProgramId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionYesNo", new[] { "QuestionId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionYesNo", new[] { "AthleteId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionScale", new[] { "AssignedProgram_ProgramDayItemSurveyId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionScale", new[] { "AssignedProgram_ProgramId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionScale", new[] { "QuestionId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionScale", new[] { "AthleteId" });
            DropIndex("dbo.AssignedProgram_ProgramDay", new[] { "AssignedProgram_ProgramId" });
            DropIndex("dbo.AssignedProgram_ProgramDayItem", new[] { "AssignedProgram_ProgramDayId" });
            DropIndex("dbo.AssignedProgram_ProgramDayItemSurvey", new[] { "AssignedProgram_ProgramDayItemId" });
            DropIndex("dbo.AssignedProgram_ProgramDayItemSurvey", new[] { "SurveyId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionOpenEnded", new[] { "AssignedProgram_ProgramDayItemSurveyId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionOpenEnded", new[] { "AssignedProgram_ProgramId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionOpenEnded", new[] { "QuestionId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionOpenEnded", new[] { "AthleteId" });
            DropIndex("dbo.AssignedProgram_Program", new[] { "OrganizationId" });
            DropIndex("dbo.AssignedProgram_Program", "IX_CreatedUserId_Name_Programs");
            DropIndex("dbo.AssignedProgram_AssignedProgramHistory", new[] { "AssignedProgram_ProgramId" });
            DropIndex("dbo.AssignedProgram_AssignedProgramHistory", "IX_AssignedProgram");
            DropColumn("dbo.Athletes", "AssignedProgram_AssignedProgramId");
            DropColumn("dbo.Athletes", "AssignedProgram_AssignedTime");
            DropTable("dbo.AssignedProgram_SuperSetNoteDisplayWeek");
            DropTable("dbo.AssignedProgram_SuperSetNote");
            DropTable("dbo.AssignedProgram_ProgramDayItemSuperSet");
            DropTable("dbo.AssignedProgram_SuperSetExercise");
            DropTable("dbo.AssignedProgram_ProgramDayItemSuperSetWeek");
            DropTable("dbo.AssignedProgram_ProgramDayItemSuperSet_Set");
            DropTable("dbo.AssignedProgram_ProgramDayItemMovie");
            DropTable("dbo.AssignedProgram_ProgramDayItemMetric");
            DropTable("dbo.AssignedProgram_CompletedQuestionYesNo");
            DropTable("dbo.AssignedProgram_CompletedQuestionScale");
            DropTable("dbo.AssignedProgram_ProgramDay");
            DropTable("dbo.AssignedProgram_ProgramDayItem");
            DropTable("dbo.AssignedProgram_ProgramDayItemSurvey");
            DropTable("dbo.AssignedProgram_CompletedQuestionOpenEnded");
            DropTable("dbo.AssignedProgram_Program");
            DropTable("dbo.AssignedProgram_AssignedProgramHistory");
        }
    }
}
