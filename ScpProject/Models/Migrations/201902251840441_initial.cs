namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignedPrograms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedDate = c.DateTime(nullable: false),
                        ProgramId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Programs", t => t.ProgramId)
                .Index(t => new { t.Id, t.AssignedDate }, unique: true, name: "IX_AssignedProgram")
                .Index(t => t.ProgramId);
            
            CreateTable(
                "dbo.Programs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(),
                        IsCoach = c.Boolean(nullable: false),
                        Email = c.String(),
                        FailedEntryAttempts = c.Int(nullable: false),
                        LockedOut = c.Boolean(nullable: false),
                        ProfilePictureId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        ImageContainerName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Athletes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        AthleteUserId = c.Int(nullable: false),
                        AssignedProgramId = c.Int(),
                        ProgramStartDate = c.DateTime(),
                        ProgramEndDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        ValidatedEmail = c.Boolean(nullable: false),
                        ProfilePictureId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedPrograms", t => t.AssignedProgramId)
                .ForeignKey("dbo.Users", t => t.AthleteUserId)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .ForeignKey("dbo.Pictures", t => t.ProfilePictureId)
                .Index(t => t.CreatedUserId)
                .Index(t => t.AthleteUserId)
                .Index(t => t.AssignedProgramId)
                .Index(t => t.ProfilePictureId);
            
            CreateTable(
                "dbo.AthleteInjuries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Notes = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedUserId = c.Int(nullable: false),
                        AthleteId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.CreatedUserId)
                .Index(t => t.AthleteId);
            
            CreateTable(
                "dbo.AthleteNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Notes = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedUserId = c.Int(nullable: false),
                        AthleteId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.CreatedUserId)
                .Index(t => t.AthleteId);
            
            CreateTable(
                "dbo.Pictures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        URL = c.String(),
                        FileName = c.String(),
                        Thumbnail = c.String(),
                        Profile = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.AthleteTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        Notes = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.CompletedMetrics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Int(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        AssignedProgramId = c.Int(nullable: false),
                        AthleteId = c.Int(nullable: false),
                        OriginalMetricId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedPrograms", t => t.AssignedProgramId)
                .ForeignKey("dbo.Metrics", t => t.OriginalMetricId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.AssignedProgramId)
                .Index(t => t.AthleteId)
                .Index(t => t.OriginalMetricId);
            
            CreateTable(
                "dbo.Metrics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        UnitOfMeasurementId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UnitOfMeasurements", t => t.UnitOfMeasurementId)
                .Index(t => t.UnitOfMeasurementId);
            
            CreateTable(
                "dbo.UnitOfMeasurements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UnitType = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CompletedQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        AthleteId = c.Int(nullable: false),
                        OriginalQuestionId = c.Int(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        AssignedProgramId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedPrograms", t => t.AssignedProgramId)
                .ForeignKey("dbo.Questions", t => t.OriginalQuestionId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.AthleteId)
                .Index(t => t.OriginalQuestionId)
                .Index(t => t.AssignedProgramId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionDisplayText = c.String(maxLength: 100),
                        QuestionTypeId = c.Int(nullable: false),
                        CreatedUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .ForeignKey("dbo.QuestionTypes", t => t.QuestionTypeId)
                .Index(t => new { t.CreatedUserId, t.QuestionDisplayText }, unique: true, name: "IX_QuestionDisplayText_CreatedUserId")
                .Index(t => t.QuestionTypeId);
            
            CreateTable(
                "dbo.QuestionTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CompletedSets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Sets = c.Int(nullable: false),
                        Reps = c.Int(nullable: false),
                        Percent = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        OriginalSetId = c.Int(nullable: false),
                        AthleteId = c.Int(nullable: false),
                        AssignedProgramId = c.Int(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedPrograms", t => t.AssignedProgramId)
                .ForeignKey("dbo.Sets", t => t.OriginalSetId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.OriginalSetId)
                .Index(t => t.AthleteId)
                .Index(t => t.AssignedProgramId);
            
            CreateTable(
                "dbo.Sets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Sets = c.Int(nullable: false),
                        Reps = c.Int(nullable: false),
                        Percent = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        ParentWeekId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Weeks", t => t.ParentWeekId)
                .Index(t => t.ParentWeekId);
            
            CreateTable(
                "dbo.Weeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        ParentWorkoutId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Workouts", t => t.ParentWorkoutId)
                .Index(t => t.ParentWorkoutId);
            
            CreateTable(
                "dbo.Workouts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Notes = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.Exercises",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Notes = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => new { t.CreatedUserId, t.Name }, unique: true, name: "IX_Exercise_Name_CreatedUserId");
            
            CreateTable(
                "dbo.ExerciseTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        Notes = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.MediaTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MetricTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        Notes = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.Movies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        URL = c.String(),
                        Title = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.MultiMediaTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        Notes = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.Notes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NoteText = c.String(),
                        Title = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.ProgramDayItemExercises",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExerciseId = c.Int(nullable: false),
                        WorkoutId = c.Int(nullable: false),
                        ProgramDayItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Exercises", t => t.ExerciseId)
                .ForeignKey("dbo.ProgramDayItems", t => t.ProgramDayItemId)
                .Index(t => t.ExerciseId)
                .Index(t => t.ProgramDayItemId);
            
            CreateTable(
                "dbo.ProgramDayItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProgramDayId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        ItemEnum = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramDays", t => t.ProgramDayId)
                .Index(t => t.ProgramDayId);
            
            CreateTable(
                "dbo.ProgramDays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        ProgramId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Programs", t => t.ProgramId)
                .Index(t => t.ProgramId);
            
            CreateTable(
                "dbo.ProgramDayItemMetrics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MetricId = c.Int(nullable: false),
                        ProgramDayItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Metrics", t => t.MetricId)
                .ForeignKey("dbo.ProgramDayItems", t => t.ProgramDayItemId)
                .Index(t => t.MetricId)
                .Index(t => t.ProgramDayItemId);
            
            CreateTable(
                "dbo.ProgramDayItemNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Note = c.String(),
                        ProgramDayItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramDayItems", t => t.ProgramDayItemId)
                .Index(t => t.ProgramDayItemId);
            
            CreateTable(
                "dbo.ProgramDayItemSurveys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        ProgramDayItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Surveys", t => t.SurveyId)
                .ForeignKey("dbo.ProgramDayItems", t => t.ProgramDayItemId)
                .Index(t => t.SurveyId)
                .Index(t => t.ProgramDayItemId);
            
            CreateTable(
                "dbo.Surveys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 100),
                        Description = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => new { t.CreatedUserId, t.Title }, unique: true, name: "IX_Survey_Title_CreatedUserId");
            
            CreateTable(
                "dbo.ProgramDayItemTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProgramSets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Sets = c.Int(nullable: false),
                        Reps = c.Int(nullable: false),
                        Percent = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        ParentProgramWeekId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramWeeks", t => t.ParentProgramWeekId)
                .Index(t => t.ParentProgramWeekId);
            
            CreateTable(
                "dbo.ProgramWeeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        ProgramDayItemExerciseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramDayItemExercises", t => t.ProgramDayItemExerciseId)
                .Index(t => t.ProgramDayItemExerciseId);
            
            CreateTable(
                "dbo.ProgramTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        Notes = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.SurveysToQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .ForeignKey("dbo.Surveys", t => t.SurveyId)
                .Index(t => t.SurveyId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.SurveyTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        Notes = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.TagsToAthletes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagId = c.Int(nullable: false),
                        AthleteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AthleteTags", t => t.TagId)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.TagId)
                .Index(t => t.AthleteId);
            
            CreateTable(
                "dbo.TagsToExercises",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagId = c.Int(nullable: false),
                        ExerciseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExerciseTags", t => t.TagId)
                .ForeignKey("dbo.Exercises", t => t.ExerciseId)
                .Index(t => t.TagId)
                .Index(t => t.ExerciseId);
            
            CreateTable(
                "dbo.TagsToMetrics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagId = c.Int(nullable: false),
                        MetricId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MetricTags", t => t.TagId)
                .ForeignKey("dbo.Metrics", t => t.MetricId)
                .Index(t => t.TagId)
                .Index(t => t.MetricId);
            
            CreateTable(
                "dbo.TagsToMultiMedia",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MultiMediaId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                        MultiMediaTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MultiMediaTags", t => t.TagId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.TagsToPrograms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagId = c.Int(nullable: false),
                        ProgramId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramTags", t => t.TagId)
                .ForeignKey("dbo.Programs", t => t.ProgramId)
                .Index(t => t.TagId)
                .Index(t => t.ProgramId);
            
            CreateTable(
                "dbo.TagsToSurveys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagId = c.Int(nullable: false),
                        SurveyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SurveyTags", t => t.TagId)
                .ForeignKey("dbo.Surveys", t => t.SurveyId)
                .Index(t => t.TagId)
                .Index(t => t.SurveyId);
            
            CreateTable(
                "dbo.TagsToWorkouts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkoutId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkoutTags", t => t.TagId)
                .ForeignKey("dbo.Workouts", t => t.WorkoutId)
                .Index(t => t.WorkoutId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.WorkoutTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        Notes = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.UserTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Token = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTokens", "UserId", "dbo.Users");
            DropForeignKey("dbo.TagsToWorkouts", "WorkoutId", "dbo.Workouts");
            DropForeignKey("dbo.TagsToWorkouts", "TagId", "dbo.WorkoutTags");
            DropForeignKey("dbo.WorkoutTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.TagsToSurveys", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.TagsToSurveys", "TagId", "dbo.SurveyTags");
            DropForeignKey("dbo.TagsToPrograms", "ProgramId", "dbo.Programs");
            DropForeignKey("dbo.TagsToPrograms", "TagId", "dbo.ProgramTags");
            DropForeignKey("dbo.TagsToMultiMedia", "TagId", "dbo.MultiMediaTags");
            DropForeignKey("dbo.TagsToMetrics", "MetricId", "dbo.Metrics");
            DropForeignKey("dbo.TagsToMetrics", "TagId", "dbo.MetricTags");
            DropForeignKey("dbo.TagsToExercises", "ExerciseId", "dbo.Exercises");
            DropForeignKey("dbo.TagsToExercises", "TagId", "dbo.ExerciseTags");
            DropForeignKey("dbo.TagsToAthletes", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.TagsToAthletes", "TagId", "dbo.AthleteTags");
            DropForeignKey("dbo.SurveyTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.SurveysToQuestions", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.SurveysToQuestions", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.ProgramTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.ProgramSets", "ParentProgramWeekId", "dbo.ProgramWeeks");
            DropForeignKey("dbo.ProgramWeeks", "ProgramDayItemExerciseId", "dbo.ProgramDayItemExercises");
            DropForeignKey("dbo.ProgramDayItemSurveys", "ProgramDayItemId", "dbo.ProgramDayItems");
            DropForeignKey("dbo.ProgramDayItemSurveys", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.Surveys", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.ProgramDayItemNotes", "ProgramDayItemId", "dbo.ProgramDayItems");
            DropForeignKey("dbo.ProgramDayItemMetrics", "ProgramDayItemId", "dbo.ProgramDayItems");
            DropForeignKey("dbo.ProgramDayItemMetrics", "MetricId", "dbo.Metrics");
            DropForeignKey("dbo.ProgramDayItemExercises", "ProgramDayItemId", "dbo.ProgramDayItems");
            DropForeignKey("dbo.ProgramDayItems", "ProgramDayId", "dbo.ProgramDays");
            DropForeignKey("dbo.ProgramDays", "ProgramId", "dbo.Programs");
            DropForeignKey("dbo.ProgramDayItemExercises", "ExerciseId", "dbo.Exercises");
            DropForeignKey("dbo.Notes", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.MultiMediaTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.Movies", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.MetricTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.ExerciseTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.Exercises", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.CompletedSets", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.CompletedSets", "OriginalSetId", "dbo.Sets");
            DropForeignKey("dbo.Sets", "ParentWeekId", "dbo.Weeks");
            DropForeignKey("dbo.Weeks", "ParentWorkoutId", "dbo.Workouts");
            DropForeignKey("dbo.Workouts", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.CompletedSets", "AssignedProgramId", "dbo.AssignedPrograms");
            DropForeignKey("dbo.CompletedQuestions", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.CompletedQuestions", "OriginalQuestionId", "dbo.Questions");
            DropForeignKey("dbo.Questions", "QuestionTypeId", "dbo.QuestionTypes");
            DropForeignKey("dbo.Questions", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.CompletedQuestions", "AssignedProgramId", "dbo.AssignedPrograms");
            DropForeignKey("dbo.CompletedMetrics", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.CompletedMetrics", "OriginalMetricId", "dbo.Metrics");
            DropForeignKey("dbo.Metrics", "UnitOfMeasurementId", "dbo.UnitOfMeasurements");
            DropForeignKey("dbo.CompletedMetrics", "AssignedProgramId", "dbo.AssignedPrograms");
            DropForeignKey("dbo.AthleteTags", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.Athletes", "ProfilePictureId", "dbo.Pictures");
            DropForeignKey("dbo.Pictures", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.AthleteNotes", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.AthleteNotes", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.AthleteInjuries", "AthleteId", "dbo.Athletes");
            DropForeignKey("dbo.AthleteInjuries", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.Athletes", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.Athletes", "AthleteUserId", "dbo.Users");
            DropForeignKey("dbo.Athletes", "AssignedProgramId", "dbo.AssignedPrograms");
            DropForeignKey("dbo.AssignedPrograms", "ProgramId", "dbo.Programs");
            DropForeignKey("dbo.Programs", "CreatedUserId", "dbo.Users");
            DropIndex("dbo.UserTokens", new[] { "UserId" });
            DropIndex("dbo.WorkoutTags", new[] { "CreatedUserId" });
            DropIndex("dbo.TagsToWorkouts", new[] { "TagId" });
            DropIndex("dbo.TagsToWorkouts", new[] { "WorkoutId" });
            DropIndex("dbo.TagsToSurveys", new[] { "SurveyId" });
            DropIndex("dbo.TagsToSurveys", new[] { "TagId" });
            DropIndex("dbo.TagsToPrograms", new[] { "ProgramId" });
            DropIndex("dbo.TagsToPrograms", new[] { "TagId" });
            DropIndex("dbo.TagsToMultiMedia", new[] { "TagId" });
            DropIndex("dbo.TagsToMetrics", new[] { "MetricId" });
            DropIndex("dbo.TagsToMetrics", new[] { "TagId" });
            DropIndex("dbo.TagsToExercises", new[] { "ExerciseId" });
            DropIndex("dbo.TagsToExercises", new[] { "TagId" });
            DropIndex("dbo.TagsToAthletes", new[] { "AthleteId" });
            DropIndex("dbo.TagsToAthletes", new[] { "TagId" });
            DropIndex("dbo.SurveyTags", new[] { "CreatedUserId" });
            DropIndex("dbo.SurveysToQuestions", new[] { "QuestionId" });
            DropIndex("dbo.SurveysToQuestions", new[] { "SurveyId" });
            DropIndex("dbo.ProgramTags", new[] { "CreatedUserId" });
            DropIndex("dbo.ProgramWeeks", new[] { "ProgramDayItemExerciseId" });
            DropIndex("dbo.ProgramSets", new[] { "ParentProgramWeekId" });
            DropIndex("dbo.Surveys", "IX_Survey_Title_CreatedUserId");
            DropIndex("dbo.ProgramDayItemSurveys", new[] { "ProgramDayItemId" });
            DropIndex("dbo.ProgramDayItemSurveys", new[] { "SurveyId" });
            DropIndex("dbo.ProgramDayItemNotes", new[] { "ProgramDayItemId" });
            DropIndex("dbo.ProgramDayItemMetrics", new[] { "ProgramDayItemId" });
            DropIndex("dbo.ProgramDayItemMetrics", new[] { "MetricId" });
            DropIndex("dbo.ProgramDays", new[] { "ProgramId" });
            DropIndex("dbo.ProgramDayItems", new[] { "ProgramDayId" });
            DropIndex("dbo.ProgramDayItemExercises", new[] { "ProgramDayItemId" });
            DropIndex("dbo.ProgramDayItemExercises", new[] { "ExerciseId" });
            DropIndex("dbo.Notes", new[] { "CreatedUserId" });
            DropIndex("dbo.MultiMediaTags", new[] { "CreatedUserId" });
            DropIndex("dbo.Movies", new[] { "CreatedUserId" });
            DropIndex("dbo.MetricTags", new[] { "CreatedUserId" });
            DropIndex("dbo.ExerciseTags", new[] { "CreatedUserId" });
            DropIndex("dbo.Exercises", "IX_Exercise_Name_CreatedUserId");
            DropIndex("dbo.Workouts", new[] { "CreatedUserId" });
            DropIndex("dbo.Weeks", new[] { "ParentWorkoutId" });
            DropIndex("dbo.Sets", new[] { "ParentWeekId" });
            DropIndex("dbo.CompletedSets", new[] { "AssignedProgramId" });
            DropIndex("dbo.CompletedSets", new[] { "AthleteId" });
            DropIndex("dbo.CompletedSets", new[] { "OriginalSetId" });
            DropIndex("dbo.Questions", new[] { "QuestionTypeId" });
            DropIndex("dbo.Questions", "IX_QuestionDisplayText_CreatedUserId");
            DropIndex("dbo.CompletedQuestions", new[] { "AssignedProgramId" });
            DropIndex("dbo.CompletedQuestions", new[] { "OriginalQuestionId" });
            DropIndex("dbo.CompletedQuestions", new[] { "AthleteId" });
            DropIndex("dbo.Metrics", new[] { "UnitOfMeasurementId" });
            DropIndex("dbo.CompletedMetrics", new[] { "OriginalMetricId" });
            DropIndex("dbo.CompletedMetrics", new[] { "AthleteId" });
            DropIndex("dbo.CompletedMetrics", new[] { "AssignedProgramId" });
            DropIndex("dbo.AthleteTags", new[] { "CreatedUserId" });
            DropIndex("dbo.Pictures", new[] { "CreatedUserId" });
            DropIndex("dbo.AthleteNotes", new[] { "AthleteId" });
            DropIndex("dbo.AthleteNotes", new[] { "CreatedUserId" });
            DropIndex("dbo.AthleteInjuries", new[] { "AthleteId" });
            DropIndex("dbo.AthleteInjuries", new[] { "CreatedUserId" });
            DropIndex("dbo.Athletes", new[] { "ProfilePictureId" });
            DropIndex("dbo.Athletes", new[] { "AssignedProgramId" });
            DropIndex("dbo.Athletes", new[] { "AthleteUserId" });
            DropIndex("dbo.Athletes", new[] { "CreatedUserId" });
            DropIndex("dbo.Programs", new[] { "CreatedUserId" });
            DropIndex("dbo.AssignedPrograms", new[] { "ProgramId" });
            DropIndex("dbo.AssignedPrograms", "IX_AssignedProgram");
            DropTable("dbo.UserTokens");
            DropTable("dbo.WorkoutTags");
            DropTable("dbo.TagsToWorkouts");
            DropTable("dbo.TagsToSurveys");
            DropTable("dbo.TagsToPrograms");
            DropTable("dbo.TagsToMultiMedia");
            DropTable("dbo.TagsToMetrics");
            DropTable("dbo.TagsToExercises");
            DropTable("dbo.TagsToAthletes");
            DropTable("dbo.SurveyTags");
            DropTable("dbo.SurveysToQuestions");
            DropTable("dbo.ProgramTags");
            DropTable("dbo.ProgramWeeks");
            DropTable("dbo.ProgramSets");
            DropTable("dbo.ProgramDayItemTypes");
            DropTable("dbo.Surveys");
            DropTable("dbo.ProgramDayItemSurveys");
            DropTable("dbo.ProgramDayItemNotes");
            DropTable("dbo.ProgramDayItemMetrics");
            DropTable("dbo.ProgramDays");
            DropTable("dbo.ProgramDayItems");
            DropTable("dbo.ProgramDayItemExercises");
            DropTable("dbo.Notes");
            DropTable("dbo.MultiMediaTags");
            DropTable("dbo.Movies");
            DropTable("dbo.MetricTags");
            DropTable("dbo.MediaTypes");
            DropTable("dbo.ExerciseTags");
            DropTable("dbo.Exercises");
            DropTable("dbo.Workouts");
            DropTable("dbo.Weeks");
            DropTable("dbo.Sets");
            DropTable("dbo.CompletedSets");
            DropTable("dbo.QuestionTypes");
            DropTable("dbo.Questions");
            DropTable("dbo.CompletedQuestions");
            DropTable("dbo.UnitOfMeasurements");
            DropTable("dbo.Metrics");
            DropTable("dbo.CompletedMetrics");
            DropTable("dbo.AthleteTags");
            DropTable("dbo.Pictures");
            DropTable("dbo.AthleteNotes");
            DropTable("dbo.AthleteInjuries");
            DropTable("dbo.Athletes");
            DropTable("dbo.Users");
            DropTable("dbo.Programs");
            DropTable("dbo.AssignedPrograms");
        }
    }
}
