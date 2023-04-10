using Models.User;
using System.Data.Entity;
using Models.Exercise;
using System.Data.Entity.ModelConfiguration.Conventions;
using Models.Metric;
using Models.Program;
using Models.Payment;
using Models.Organization;
using Models.Administration;
using Models.Messages;
using Models.Enums;
using Models.Survey;
using Models.MultiMedia;
using Models.Program.AssignedProgramSnapShots;

namespace Models
{
    public class StrenContext : DbContext
    {
        public StrenContext() : base(@"Server=tcp:strengthconpro.database.windows.net, 1433; Initial Catalog =scp; Persist Security Info=False;User ID = ghp9170; Password=nap1trag!@|?@T@R; MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout = 30;") { }
        public DbSet<User.User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<ExerciseTag> ExerciseTags { get; set; }
        public DbSet<Exercise.Exercise> Exercises { get; set; }
        public DbSet<TagToExercise> TagsToExercises { get; set; }
        public DbSet<SetsAndReps.Set> Sets { get; set; }
        public DbSet<SetsAndReps.Week> Weeks { get; set; }
        public DbSet<SetsAndReps.Workout> Workouts { get; set; }
        public DbSet<SetsAndReps.WorkoutTag> WorkoutTags { get; set; }
        public DbSet<SetsAndReps.TagToWorkout> TagsToWorkouts { get; set; }
        public DbSet<Survey.Question> Questions { get; set; }
        public DbSet<Survey.Survey> Surveys { get; set; }
        public DbSet<Survey.SurveyTag> SurveyTags { get; set; }
        public DbSet<Survey.TagToSurvey> TagsToSurveys { get; set; }
        public DbSet<Enums.QuestionType> QuestionTypes { get; set; }
        public DbSet<Survey.SurveysToQuestions> SurveysToQuestions { get; set; }
        public DbSet<Enums.MediaType> MediaTypes { get; set; }
        public DbSet<MultiMedia.Picture> Pictures { get; set; }
        public DbSet<MultiMedia.Note> Notes { get; set; }
        public DbSet<MultiMedia.Movie> Movies { get; set; }
        public DbSet<MultiMedia.MultiMediaTag> MultiMediaTags { get; set; }
        public DbSet<MultiMedia.TagToMultiMedia> TagsToMultimedia { get; set; }
        public DbSet<Metric.Metric> Metrics { get; set; }
        public DbSet<Metric.UnitOfMeasurement> Measurements { get; set; }
        public DbSet<Metric.MetricTag> MetricTags { get; set; }
        public DbSet<Metric.TagToMetric> TagsToMetric { get; set; }
        public DbSet<Program.ProgramTag> ProgramTags { get; set; }
        public DbSet<Program.Program> Programs { get; set; }
        public DbSet<Program.TagToProgram> TagsToPrograms { get; set; }
        public DbSet<Athlete.Athlete> Athletes { get; set; }
        public DbSet<Athlete.TagToAthlete> TagsToAthletes { get; set; }
        public DbSet<Athlete.AthleteTag> AthleteTags { get; set; }
        public DbSet<Enums.ProgramDayItemType> ProgramDayItemTypes { get; set; }
        public DbSet<Program.ProgramDayItem> ProgramDayItems { get; set; }
        public DbSet<Program.ProgramSet> ProgramSets { get; set; }
        public DbSet<Program.ProgramWeek> ProgramWeeks { get; set; }
        public DbSet<Program.ProgramDayItemExercise> ProgramDayItemExercise { get; set; }
        public DbSet<Program.ProgramDayItemMetric> ProgramDayItemMetric { get; set; }
        public DbSet<Program.ProgramDayItemNote> ProgramDayItemNote { get; set; }
        public DbSet<Program.ProgramDayItemSurvey> ProgramDayItemSurveys { get; set; }
        public DbSet<Program.ProgramDay> ProgramDays { get; set; }
        public DbSet<Athlete.CompletedSet> CompletedSets { get; set; }
        public DbSet<Athlete.CompletedMetric> CompletedMetrics { get; set; }
        public DbSet<Athlete.CompletedQuestionYesNo> CompletedYesNoQuestions { get; set; }
        public DbSet<Athlete.CompletedQuestionOpenEnded> CompletedOpenEndedQuestions { get; set; }
        public DbSet<Athlete.CompletedQuestionScale> CompletedScaleQuestions { get; set; }
        public DbSet<Metric.MetricDisplayWeek> MetricDisplayWeeks { get; set; }
        public DbSet<Program.AssignedProgram> AssignedPrograms { get; set; }
        public DbSet<Survey.SurveyDisplayWeek> SurveyDisplayWeeks { get; set; }
        public DbSet<Program.NoteDisplayWeek> NoteDisplayWeeks { get; set; }
        public DbSet<Athlete.CompletedProgramDay> CompletedProgramDays { get; set; }
        public DbSet<Athlete.CompletedProgramWeek> CompletedProgramWeeks { get; set; }
        public DbSet<Athlete.AthleteProgramHistory> PastAssignedPrograms { get; set; }
        public DbSet<Program.CompletedAssignedProgramDay> CompletedAssignedProgramDays { get; set; }
        public DbSet<Program.ProgramDayItemSuperSet> ProgramDayItemSuperSets { get; set; }
        public DbSet<Program.SuperSetExercise> SuperSetExercises { get; set; }
        public DbSet<Program.ProgramDayItemSuperSet_Set> SuperSet_Sets { get; set; }
        public DbSet<Program.ProgramDayItemSuperSetWeek> SuperSetWeeks { get; set; }
        public DbSet<Athlete.CompletedSuperSet_Set> CompletedSuperSet_Sets { get; set; }
        public DbSet<Organization.Organization> Organizations { get; set; }
        public DbSet<Organization.UserToOrganizationRole> UserToOrganizationRoles { get; set; }
        public DbSet<Enums.OrganizationRole> OrganizationRoles { get; set; }
        public DbSet<AddedMetric> AddedMetrics { get; set; }
        public DbSet<Athlete.AthleteHeightWeight> AthleteHeightWeights { get; set; }
        public DbSet<Models.User.PasswordReset> PasswordResets { get; set; }
        public DbSet<SuperSetNote> SuperSetNotes { get; set; }
        public DbSet<SuperSetNoteDisplayWeek> SuperSetNotesDisplayWeeks { get; set; }
        public DbSet<SubscriptionType> SubscriptionTypes { get; set; }
        public DbSet<SubscriptionApprovalAudit> SubscriptionApprovalAudits { get; set; }
        public DbSet<WeightRoomAccount> WeightRoomAccounts { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageGroup> MessageGroups { get; set; }
        public DbSet<MessageToUser> MessagesToUsers { get; set; }
        public DbSet<MessageGroupToUser> MessageGroupsToUsers { get; set; }
        public DbSet<MessageToUserInGroup> MessagesToUsersInGroups { get; set; }
        public DbSet<Notifications.Notification> Notifications { get; set; }
        public DbSet<Enums.NotificationType> NotificationTypes { get; set; }
        public DbSet<QuestionThreshold> QuestionThresholds { get; set; }
        public DbSet<ScaleQuestionThreshold> ScaleQuestionThresholds { get; set; }
        public DbSet<YesNoQuestionThreshold> YesNoQuestionThresholds { get; set; }
        public DbSet<YesNoQuestionThresholdToCoach> YesNoThresholdsToCoaches { get; set; }
        public DbSet<ScaleThresholdToCoach> ScaleThresholdToCoaches { get; set; }
        public DbSet<MultiMedia.MovieTag> MovieTags { get; set; }
        public DbSet<MultiMedia.TagToMovie> TagsToMovies { get; set; }
        public DbSet<ProgramDayItemMovie> ProgramDayItemMovies { get; set; }
        public DbSet<MovieDisplayWeek> MovieDisplayWeeks { get; set; }
        public DbSet<LogMessage> Logs { get; set; }
        public DbSet<AssignedProgram_AssignedProgramHistory> AssignedProgram_AssignedProgramHistorys { get; set; }
        public DbSet<AssignedProgram_CompletedQuestionOpenEnded> AssignedProgram_CompletedQuestionOpenEndeds { get; set; }
        public DbSet<AssignedProgram_CompletedQuestionScale> AssignedProgram_CompletedQuestionScales { get; set; }
        public DbSet<AssignedProgram_CompletedQuestionYesNo> AssignedProgram_CompletedQuestionYesNos { get; set; }
        public DbSet<AssignedProgram_Program> AssignedProgram_Programs { get; set; }
        public DbSet<AssignedProgram_ProgramDay> AssignedProgram_ProgramDay { get; set; }
        public DbSet<AssignedProgram_ProgramDayItem> AssignedProgram_ProgramDayItems { get; set; }
        public DbSet<AssignedProgram_ProgramDayItemMetric> AssignedProgram_ProgramDayItemMetrics { get; set; }
        public DbSet<AssignedProgram_ProgramDayItemMovie> AssignedProgram_ProgramDayItemMovies { get; set; }
        public DbSet<AssignedProgram_ProgramDayItemSuperSet> AssignedProgram_ProgramDayItemSuperSets { get; set; }
        public DbSet<AssignedProgram_ProgramDayItemSuperSet_Set> AssignedProgram_ProgramDayItemSuperSet_Sets { get; set; }
        public DbSet<AssignedProgram_ProgramDayItemSuperSetWeek> AssignedProgram_ProgramDayItemSuperSetWeeks { get; set; }
        public DbSet<AssignedProgram_ProgramDayItemSurvey> AssignedProgram_ProgramDayItemSurveys { get; set; }
        public DbSet<AssignedProgram_SuperSetExercise> AssignedProgram_SuperSetExercises { get; set; }
        public DbSet<AssignedProgram_SuperSetNote> AssignedProgram_SuperSetNotes { get; set; }
        public DbSet<AssignedProgram_SuperSetNoteDisplayWeek> AssignedProgram_SuperSetNoteDisplayWeeks { get; set; }
        public DbSet<AssignedProgram_MovieDisplayWeek> AssignedProgram_MovieDisplayWeeks { get; set; }
        public DbSet<AssignedProgram_MetricsDisplayWeek> AssignedProgram_MetricsDisplayWeeks { get; set; }
        public DbSet<AssignedProgram_ProgramDayItemNote> AssignedProgram_ProgramDayItemNotes { get; set; }
        public DbSet<AssignedProgram_NoteDisplayWeek> AssignedProgram_NoteDisplayWeeks { get; set; }
        public DbSet<AssignedProgram_SurveyDisplayWeeks> AssignedProgram_SurveyDisplayWeek { get; set; }
        public DbSet<AssignedProgram_CompletedDay> CompletedDays { get; set; }
        public DbSet<Documents.Document> Documents { get; set; }
        public DbSet<Documents.DocumentTag> DocumentTags { get; set; }
        public DbSet<Documents.TagToDocument> TagToDocuments { get; set; }
        public DbSet<Documents.Agreement> Agreements { get; set; }
        public DbSet<Documents.AssignedDocuments> AssignedDocuments { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
    }
}
}
