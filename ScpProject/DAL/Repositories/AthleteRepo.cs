using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DAL.DTOs;
using DAL.DTOs.Athlete;
using DAL.DTOs.Metrics;
using DAL.DTOs.Program;
using Dapper;
using Models.Athlete;
using Models.Enums;

namespace DAL.Repositories
{
    public interface IAthleteRepo
    {
        int AddCompletedMetric(CompletedMetric targetCompletedMetric);
        void AddCompletedSet(CompletedSet targetCompletedSet);
        void AddCompletedSuperSet(CompletedSuperSet_Set targetCompletedSet);
        void AddProfilePictureToAthlete(int athleteId, int pictureId, Guid createdUserToke);
        void AddStandAloneMetric(int athleteId, int metricId, double value, int enteredByPerson, DateTime timeOfCompletion);
        void Archive(int athleteId, Guid userToken);
        int CreateAthlete(string firstName, string lastName, int createdUserId, int athleteUserId, string EmailValidationToken, DateTime tokenIssuedDate, DateTime? birthday);
        void FinishAthleteCreation(string userName, string password, int athleteId, string emailValidation);
        void fixDuplicateAccounts();
        List<AssignedProgramAthleteDTO> GetAllAssignedProgramAthletes(List<int> athleteIds);
        List<Athlete> GetAllAtheltesWithoutProgram(Guid createdUserToken);
        List<Athlete> GetAllAthletes(Guid createdUserToken);
        List<Athlete> GetAllAthletes(int orgId);
        List<AthleteWithTagsDTO> GetAllAthletesTagMappings(Guid userToken);
        List<AthleteWithTagsDTO> GetAllAthletesTagMappings(int orgId);
        Athlete GetAthlete(Guid athleteToken);
        Athlete GetAthlete(int athleteId);
        Athlete GetAthlete(int athleteId, Guid createdUserToken);
        Athlete GetAthleteByAthleteUserId(int athleteUserId);
        Athlete GetAthleteByWeightRoom(int id, Guid weightRoomTokenId);
        List<CompletedMetricDisplay> GetAthleteListOfCompletedMetrics(int athleteId);
        List<ProgramHistory> GetAthleteProgramHistory(int athleteId);
        int GetAthletesCountForCoach(int OrganizationId);
        KeyValuePair<string, int> GetCurrentSubscriptionPlanForOrganization(int OrganizationId);
        AthleteHeightWeight GetLatestAthleteBioMetrics(int athleteId);
        List<CompletedMetricHistory> GetMetricHistory(int metricId, int athleteId);
        void MarkDayCompleted(int programDayId, int assignedProgramId, Athlete assignedAthlete);
        void MarkWeekCompleted(int weekId, int assignedProgramId, Athlete assignedAthlete);
        void UnassignProgramToAthlete(int athleteId, Guid createdUserToken);
        void UpdateAthlete(AthleteDTO newInfo, Guid updatingCoachGuid);
        void UpdateAthleteHeightWeight(int athleteId, double? heightPrimary, double? heightSecondary, double? weight);
        void UpdateEmailValidationToken(string emailValidationToken, int athleteId);
        void UpdateMetric(int id, bool IsCompletedMetric, double value, DateTime updatedDate);
        bool ValidateAthleteToken(Guid Token, int athleteId);
        Task AssignProgramToAthlete(int athleteId, int snapShotProgramIdId);
        void AddCompletedAssignedProgram_ProgramDayItemSuperSet_Set(Models.Athlete.CompletedSuperSet_Set targetCompletedSet);
        void AddCompletedMetricAssignedProgram_MetricsDisplayWeek(CompletedMetric targetCompletedMetric);
    }
    public class AthleteRepo : IAthleteRepo
    {
        private string ConnectionString;
        public AthleteRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public void UnassignProgramToAthlete(int athleteId, Guid createdUserToken)
        {
            var updateString = $@"UPDATE athlete 
                            SET assignedProgramId = null
                            WHERE athleteId = @AthleteId AND a.OrganizationId = (SELECT OrganizationId FROM users WHERE Id = ({ConstantSqlStrings.GetUserIdFromToken})) ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { AthleteId = athleteId, token = createdUserToken });
            }
        }
        public List<Models.Athlete.Athlete> GetAllAtheltesWithoutProgram(Guid createdUserToken)
        {
            string getString = $@" SELECT * FROM athletes AS a
                                  LEFT JOIN pictures AS p on p.Id = a.profilePictureId
                                  WHERE (a.AssignedProgramId IS NULL AND a.AssignedProgram_AssignedProgramId IS NULL )AND a.OrganizationId = (SELECT OrganizationId FROM users WHERE Id = ({ConstantSqlStrings.GetUserIdFromToken})) ";
            //var ret = new List<Models.Athlete.Athlete>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                return sqlConn.Query<Models.Athlete.Athlete, Models.MultiMedia.Picture, Models.Athlete.Athlete>(getString, (a, p) =>
                {
                    a.ProfilePicture = p;
                    return a;
                }, new { Token = createdUserToken }).ToList();
            }
        }
        public List<ProgramHistory> GetAthleteProgramHistory(int athleteId)
        {
            var getString = $@"SELECT p.Id AS ProgramId, ap.Id AS AssignedProgramId,aph.Id AS AssignedProgramHistoryId,p.[Name], aph.StartDate, aph.EndDate
                            FROM AssignedPrograms AS ap
                            INNER JOIN AthleteProgramHistories AS aph ON aph.AssignedProgramId = ap.id and HideProgramOnHistoryPage = 0
                            INNER JOIN programs AS p ON P.ID = AP.ProgramId
                            WHERE aph.AthleteId = @AthleteId ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<ProgramHistory>(getString, new { AthleteId = athleteId }).ToList();
            }
        }
        public List<CompletedMetricHistory> GetMetricHistory(int metricId, int athleteId)
        {
            var getString = $@"select cm.Id,  m.[name],  cm.CompletedDate, cm.Value, 1 as 'IsCompletedMetric'
                                from completedMetrics AS cm
                                INNER JOIN metrics AS m ON CM.MetricId = M.Id
                                WHERE M.id = @MetricId AND cm.AthleteId = @AthleteId

                                UNION 

                                select cm.Id, m.[name],  cm.CompletedDate, cm.Value, 0 as 'IsCompletedMetric'
                                from AddedMetrics AS cm
                                INNER JOIN metrics AS m ON CM.MetricId = M.Id
                                WHERE M.id = @MetricId AND cm.AthleteId = @AthleteId

                                UNION 

                                SELECT mdw.id, m.[name],mdw.CompletedDate, mdw.Value,  1 as 'IsCompletedMetric'
                                FROM [dbo].[AssignedProgram_MetricsDisplayWeek] AS mdw
                                INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdim ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.Id
                                INNER JOIN  AssignedProgram_ProgramDayItem AS pdi ON pdi.Id = pdim.AssignedProgram_ProgramDayItemId
                                INNER JOIN assignedProgram_programday AS pd ON pd.Id = pdi.AssignedProgram_ProgramDayId
                                INNER JOIN AssignedProgram_Program AS p ON pd.AssignedProgram_ProgramId = p.Id
                                INNER JOIN Athletes AS a ON a.AssignedProgram_AssignedProgramId =p.Id
                                INNER JOIN Metrics AS m ON m.id = pdim.MetricId
                                WHERE a.Id = @athleteId AND M.id = @MetricId and MDW.[VALUE] is not null

                                UNION
                        
                                SELECT mdw.id, m.[name],mdw.CompletedDate, mdw.Value,  1 as 'IsCompletedMetric'
                                FROM  AssignedProgram_AssignedProgramHistory AS a  
                                INNER JOIN assignedProgram_programday AS pd ON a.AssignedProgram_ProgramId = pd.AssignedProgram_ProgramId 
								INNER JOIN  AssignedProgram_ProgramDayItem AS pdi ON pdi.AssignedProgram_ProgramDayId = pd.id
								INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdim ON pdim.AssignedProgram_ProgramDayItemId = pdi.Id
								INNER JOIN [dbo].[AssignedProgram_MetricsDisplayWeek] AS mdw on MDW.AssignedProgram_ProgramDayItemMetricId = PDIM.Id
								INNER JOIN Metrics AS m ON m.id = pdim.MetricId
                                WHERE a.AthleteId = @athleteId AND M.id = @MetricId and MDW.[VALUE] is not null";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<CompletedMetricHistory>(getString, new { MetricId = metricId, AthleteId = athleteId }).ToList();
            }
        }
        public List<CompletedMetricDisplay> GetAthleteListOfCompletedMetrics(int athleteId)
        {
            var getString = $@"

                                    SELECT o.id as MetricId, o.[name], o.CompletedDate, o.[value],o.* from 
                                    (
                                        SELECT  a.id, a.[name],a.completedDate,a.[value],  row_number() over (partition by a.metricid order by completedDate desc) as rowNum
                                        FROM 
                                                (SELECT m.id, m.[name],cm.completedDate, cm.[value] , cm.MetricId
                                                 FROM CompletedMetrics AS cm 
                                                INNER JOIN Metrics AS m on cm.MetricId = m.id
                                                WHERE CM.AthleteId = @ATHLETEiD
                                                UNION
                                                SELECT m.id, m.[name],cm.completedDate, cm.[value] ,cm.MetricId
                                                 FROM AddedMetrics AS cm 
                                                INNER JOIN Metrics AS m on cm.MetricId = m.id
                                                WHERE CM.AthleteId = @ATHLETEiD
												UNION
                                                SELECT m.id, m.[name], mdw.completedDate, mdw.[value], pdim.metricid
                                                FROM [dbo].[AssignedProgram_MetricsDisplayWeek] AS mdw
                                                INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdim ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.Id
                                                INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.Id = pdim.AssignedProgram_ProgramDayItemId
                                                INNER JOIN assignedProgram_programday AS pd ON pd.Id = pdi.AssignedProgram_ProgramDayId
                                                INNER JOIN AssignedProgram_Program AS p ON pd.AssignedProgram_ProgramId = p.Id
                                                INNER JOIN AssignedProgram_AssignedProgramHistory AS aph ON aph.AssignedProgram_ProgramId =p.Id
                                                INNER JOIN Metrics AS m ON m.id = pdim.MetricId
                                                WHERE aph.AthleteId = @athleteId AND MDW.[value] IS NOT NULL
												UNION
                                                SELECT m.id, m.[name], mdw.completedDate, mdw.[value], pdim.metricid
                                                FROM [dbo].[AssignedProgram_MetricsDisplayWeek] AS mdw
                                                INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdim ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.Id
                                                INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.Id = pdim.AssignedProgram_ProgramDayItemId
                                                INNER JOIN assignedProgram_programday AS pd ON pd.Id = pdi.AssignedProgram_ProgramDayId
                                                INNER JOIN AssignedProgram_Program AS p ON pd.AssignedProgram_ProgramId = p.Id
                                                INNER JOIN athletes AS aph ON aph.AssignedProgram_AssignedProgramId =p.Id
                                                INNER JOIN Metrics AS m ON m.id = pdim.MetricId
                                                WHERE aph.ID = @athleteId AND MDW.[value] IS NOT NULL
												) AS a
                                        ) as o
                                WHERE o.rowNum = 1";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<CompletedMetricDisplay>(getString, new { AthleteId = athleteId }).ToList();
            }
        }
        public void UpdateMetric(int id, bool IsCompletedMetric, double value, DateTime updatedDate)
        {
            var table = IsCompletedMetric ? "completedMetrics" : "addedMetrics";
            var updateString = $@"UPDATE {table}
                                  SET value = @Value, completedDate = @CompletedDate 
                                  WHERE id = @Id";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Value = value, CompletedDate = updatedDate, Id = id });
            }
        }
        public void FinishAthleteCreation(string userName, string password, int athleteId, string emailValidation)
        {
            var updateString = $@" UPDATE u
                                    SET  u.UserName = @UserName, u.[Password] = @Password
                                    FROM users AS u
                                    INNER JOIN athletes AS a on a.AthleteUserId = u.Id
                                    WHERE a.id = @AthleteId and a.EmailValidationToken = @Token ";

            updateString += $@" ;UPDATE athletes
                                    SET   validatedEmail = 1
                                    WHERE athletes.id = @AthleteId and athletes.EmailValidationToken = @Token ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new
                {
                    AthleteId = athleteId,
                    UserName = userName,
                    Password = password,
                    Token = emailValidation
                });
            }
        }

        /// <summary>
        /// Fix Duiplicate Accounts with respect to the user's email
        /// </summary>
        public void fixDuplicateAccounts()
        {
            var deleteString = ";WITH duplicateEmailCTE AS (SELECT email, count(id) as count FROM users GROUP by email HAVING count(id) > 1) select (STRING_AGG (u.Id, ',')) FROM users u INNER JOIN duplicateEmailCTE AS d ON d.email = u.email left JOIN athletes AS  a ON a.CreatedUserId = u.Id";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                SqlDataReader reader = new SqlCommand(deleteString, sqlConn).ExecuteReader();
                string duiplicateAccountIds = "";
                while (reader.Read())
                {
                    try { if (reader != null && reader.GetString(0) != null) { duiplicateAccountIds = reader.GetString(0); } else { return; } } catch (Exception e) { return; }
                }
                sqlConn.Close();
                if (duiplicateAccountIds.Length > 0)
                {
                    var deleteReferences = "DELETE from Programs where CreatedUserId in (" + duiplicateAccountIds + ")";
                    var deleteDuiplicates = "DELETE from Users where Id in (" + duiplicateAccountIds + ")";
                    string queryDeleteAssignedPrograms = "DELETE from AssignedPrograms where ProgramId in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + "))";
                    string queryDeleteAssignedAthletes = "DELETE from Athletes where id in (select Id from AssignedPrograms where ProgramId in (select id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))";
                    string queryDeleteAthleteReferences = "DELETE from CompletedSuperSet_Set where AthleteId in (SELECT Id from AssignedPrograms where ProgramId in(select id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))";
                    string queryDeleteAddedReferencesToAthlete = "DELETE from AddedMetrics where AthleteId in (SELECT Id from AssignedPrograms where ProgramId in(select id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))";
                    string queryReferencesAthleteHeightWeight = "DELETE from AthleteHeightWeights where AthleteId in (SELECT Id from AssignedPrograms where ProgramId in(select id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))";
                    string queryReferencesTagsToAthlete = "DELETE from TagsToAthletes where AthleteId in (SELECT Id from AssignedPrograms where ProgramId in(select id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))";
                    string queryReferencesAthleteHistories = "DELETE from AthleteProgramHistories where AthleteId in (SELECT Id from AssignedPrograms where ProgramId in(select id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))";
                    string queryDeleteAthletes = "DELETE from Athletes where AssignedProgramId in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + "))";
                    string queryAthleteReferences = "DELETE from Athletes where AssignedProgramId in (SELECT Id from AssignedPrograms where ProgramId in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))";
                    string queryAthleteHistoryReferences = "DELETE from AthleteProgramHistories where AssignedProgramId in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + "))";
                    string queryAtheleteHistory = "DELETE from AthleteProgramHistories where AthleteId in (SELECT id from Athletes where AssignedProgramId in (SELECT Id from AssignedPrograms where ProgramId in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + "))))";
                    string queryDeleteTagsToAthletes = "DELETE from TagsToAthletes where AthleteId in (SELECT id from Athletes where AssignedProgramId in (SELECT Id from AssignedPrograms where ProgramId in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + "))))";
                    string queryDeleteProgramDays = "DELETE from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))";
                    string queryDeleteProgramDayItems = "DELETE from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + "))))";
                    string queryDeleteProgramDayMetrics = "DELETE from ProgramDayItemMetrics where ProgramDayItemId in(SELECT Id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))))";
                    string queryDeleteMetricDisplayWeeks = "DELETE from MetricDisplayWeeks where ProgramDayItemMetricId in(SELECT Id from ProgramDayItemMetrics where ProgramDayItemId in(SELECT Id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + "))))))";
                    string queryDeleteProgramDaySurveys = "DELETE from ProgramDayItemSurveys where ProgramDayItemId in(SELECT id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))))";
                    string queryDeleteSurveyDisplayWeeks = "DELETE from SurveyDisplayWeeks where ProgramDayItemSurveyId in (SELECT id from ProgramDayItemSurveys where ProgramDayItemId in(SELECT id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (41, 53, 54, 69, 82, 91, 92))))))";
                    string queryDeleteProgramDayItemNotes = "DELETE from ProgramDayItemNotes where ProgramDayItemId in (SELECT Id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))))";
                    string queryNoteDisplayWeeks = "DELETE from NoteDisplayWeeks where ProgramDayItemNoteId in(SELECT Id from ProgramDayItemNotes where ProgramDayItemId in (SELECT Id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + "))))))";
                    string queryDeleteProgramDayItemSuperSets = "DELETE from ProgramDayItemSuperSets where ProgramDayItemId in(SELECT Id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))))";
                    string queryDeleteSuperSetExercises = "DELETE from SuperSetExercises where ProgramDayItemSuperSetId in(SELECT id from ProgramDayItemSuperSets where ProgramDayItemId in(SELECT Id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + "))))))";
                    string queryDeleteProgramDayItemSuperSetWeeks = "DELETE from ProgramDayItemSuperSetWeeks where SuperSetExerciseId in (SELECT Id from SuperSetExercises where ProgramDayItemSuperSetId in(SELECT id from ProgramDayItemSuperSets where ProgramDayItemId in(SELECT Id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))))))";
                    string queryDeleteProgramDayItemSuperSet_Set = "DELETE from ProgramDayItemSuperSet_Set where ProgramDayItemSuperSetWeekId in(SELECT Id from ProgramDayItemSuperSetWeeks where SuperSetExerciseId in (SELECT Id from SuperSetExercises where ProgramDayItemSuperSetId in(SELECT id from ProgramDayItemSuperSets where ProgramDayItemId in(SELECT Id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + "))))))))";
                    string queryDeleteReferencesFromSuperSetNotes = "DELETE from SuperSetNotes where ProgramDayItemSuperSetId in (SELECT Id from ProgramDayItemSuperSets where ProgramDayItemId in(SELECT Id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + "))))))";
                    string queryDeleteSuperSetNoteDisplayWeeks = "DELETE from SuperSetNoteDisplayWeeks where SuperSetNoteId in(SELECT Id from SuperSetNotes where ProgramDayItemSuperSetId in (SELECT Id from ProgramDayItemSuperSets where ProgramDayItemId in(SELECT Id from ProgramDayItems where ProgramDayId in (SELECT Id from ProgramDays where ProgramId in (SELECT Id from Programs where id in(select Id from Programs where CreatedUserId in (" + duiplicateAccountIds + ")))))))";
                    string queryDeletePictures = "DELETE from Pictures where CreatedUserId in(SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";
                    string queryDeleteAthleteTags = "DELETE from AthleteTags where CreatedUserId in(SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";
                    string queryDeleteQuestions = "DELETE from Questions where CreatedUserId in(SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";
                    string queryDeleteQuestionSurveys = "DELETE from SurveysToQuestions where QuestionId in (SELECT Id from Questions where CreatedUserId in(SELECT Id from Users where Id in (" + duiplicateAccountIds + ")))";
                    string queryDeleteWorkoutReferences = "DELETE from Workouts where CreatedUserId in(SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";
                    string queryDeleteWeekReferences = "DELETE from Weeks where ParentWorkoutId in (SELECT Id from Workouts where CreatedUserId in(SELECT Id from Users where Id in (" + duiplicateAccountIds + ")))";
                    string queryDeleteSets = "DELETE from Sets where ParentWeekId in (SELECT Id from Weeks where ParentWorkoutId in (SELECT Id from Workouts where CreatedUserId in(SELECT Id from Users where Id in (" + duiplicateAccountIds + "))))";
                    string queryDeleteWorkOutTags = "DELETE from TagsToWorkouts where WorkoutId in (SELECT Id from Workouts where CreatedUserId in(SELECT Id from Users where Id in (" + duiplicateAccountIds + ")))";
                    string queryDeleteExercises = "DELETE from Exercises where CreatedUserId in (  SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";
                    string queryDeleteExerciseTags = "DELETE from TagsToExercises where ExerciseId in (  SELECT Id from Exercises where CreatedUserId in (  SELECT Id from Users where Id in (" + duiplicateAccountIds + ")))";
                    string queryExerciseTags = "DELETE from ExerciseTags where CreatedUserId in ( SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";
                    string queryDeleteMetricTags = "DELETE from MetricTags where CreatedUserId in (SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";
                    string queryDeleteTagsToMetrics = "DELETE from TagsToMetrics where TagId in (SELECT Id from MetricTags where CreatedUserId in (SELECT Id from Users where Id in (" + duiplicateAccountIds + ")))";
                    string queryDeleteOrganizationRoles = "DELETE from UserToOrganizationRoles where AssignedByUserId in (  SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";
                    string queryDeleteSurveyReferences = "DELETE from Surveys where CreatedUserId in (  SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";
                    string queryDeletePasswordResets = "DELETE from PasswordResets where UserId in (  SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";
                    string queryDeleteDirectWorkOutTags = "DELETE from WorkoutTags where CreatedUserId in (  SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";
                    string queryDeleteUserTokens = "DELETE from UserTokens where UserId in (  SELECT Id from Users where Id in (" + duiplicateAccountIds + "))";

                    sqlConn.Open();
                    sqlConn.Execute(queryReferencesAthleteHistories);
                    sqlConn.Execute(queryReferencesTagsToAthlete);
                    sqlConn.Execute(queryReferencesAthleteHeightWeight);
                    sqlConn.Execute(queryDeleteAddedReferencesToAthlete);
                    sqlConn.Execute(queryDeleteAthleteReferences);
                    sqlConn.Execute(queryDeleteAssignedAthletes);
                    sqlConn.Execute(queryDeleteAthletes);
                    sqlConn.Execute(queryAthleteHistoryReferences);
                    sqlConn.Execute(queryAtheleteHistory);
                    sqlConn.Execute(queryDeleteTagsToAthletes);
                    sqlConn.Execute(queryAthleteReferences);
                    sqlConn.Execute(queryDeleteAssignedPrograms);
                    sqlConn.Execute(queryDeleteMetricDisplayWeeks);
                    sqlConn.Execute(queryDeleteProgramDayMetrics);
                    sqlConn.Execute(queryDeleteSurveyDisplayWeeks);
                    sqlConn.Execute(queryDeleteProgramDaySurveys);
                    sqlConn.Execute(queryNoteDisplayWeeks);
                    sqlConn.Execute(queryDeleteProgramDayItemNotes);
                    sqlConn.Execute(queryDeleteProgramDayItemSuperSet_Set);
                    sqlConn.Execute(queryDeleteProgramDayItemSuperSetWeeks);
                    sqlConn.Execute(queryDeleteSuperSetExercises);
                    sqlConn.Execute(queryDeleteSuperSetNoteDisplayWeeks);
                    sqlConn.Execute(queryDeleteReferencesFromSuperSetNotes);
                    sqlConn.Execute(queryDeleteProgramDayItemSuperSets);
                    sqlConn.Execute(queryDeleteProgramDayItems);
                    sqlConn.Execute(queryDeleteProgramDays);
                    sqlConn.Execute(deleteReferences);
                    sqlConn.Execute(queryDeletePictures);
                    sqlConn.Execute(queryDeleteAthleteTags);
                    sqlConn.Execute(queryDeleteQuestionSurveys);
                    sqlConn.Execute(queryDeleteQuestions);
                    sqlConn.Execute(queryDeleteSets);
                    sqlConn.Execute(queryDeleteWeekReferences);
                    sqlConn.Execute(queryDeleteWorkOutTags);
                    sqlConn.Execute(queryDeleteWorkoutReferences);
                    sqlConn.Execute(queryDeleteExerciseTags);
                    sqlConn.Execute(queryDeleteExercises);
                    sqlConn.Execute(queryExerciseTags);
                    sqlConn.Execute(queryDeleteTagsToMetrics);
                    sqlConn.Execute(queryDeleteMetricTags);
                    sqlConn.Execute(queryDeleteOrganizationRoles);
                    sqlConn.Execute(queryDeleteSurveyReferences);
                    sqlConn.Execute(queryDeletePasswordResets);
                    sqlConn.Execute(queryDeleteDirectWorkOutTags);
                    sqlConn.Execute(queryDeleteUserTokens);
                    sqlConn.Execute(deleteDuiplicates);
                    sqlConn.Close();
                }
            }
        }


        public Int32 GetAthletesCountForCoach(Int32 OrganizationId)
        {
            string query = "select count(*) from athletes where OrganizationID = " + OrganizationId;
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(query);
            }
        }
        public KeyValuePair<string, int> GetCurrentSubscriptionPlanForOrganization(int OrganizationId)
        {
            KeyValuePair<string, int> cutomerSubscription = new KeyValuePair<string, int>();
            string query = "SELECT StripeGuid,CurrentSubscriptionPlanId from Organizations where Id =  " + OrganizationId;
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                SqlDataReader reader = new SqlCommand(query, sqlConn).ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        if (reader != null && reader.GetString(0) != null)
                        {
                            cutomerSubscription = new KeyValuePair<string, int>(reader.GetString(0), reader.GetInt32(1));
                        }
                    }
                    catch (Exception e)
                    { throw e; }
                }
                sqlConn.Close();
            }
            return cutomerSubscription;
        }

        public bool ValidateAthleteToken(Guid Token, int athleteId)
        {
            var validateString = @" SELECT a.Id 
                                     FROM athletes AS a 
                                     INNER JOIN users AS u ON u.id = a.athleteUserId 
                                     INNER JOIN userTokens AS ut on ut.UserId = u.Id 
                                     WHERE ut.Token = @Token ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(validateString, new { Token = Token }) == athleteId;
            }
        }
        public void AddCompletedMetricAssignedProgram_MetricsDisplayWeek(CompletedMetric targetCompletedMetric)
        {
            var updateString = $@"UPDATE AssignedProgram_MetricsDisplayWeek
                                  SET  [value] = @value, CompletedDate = @CompletedDate
                                  WHERE assignedProgram_ProgramDayItemMetricId = @AssignedProgramId AND DisplayWeek = @weekId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new
                {
                    AssignedProgramId = targetCompletedMetric.ProgramDayItemMetricId,
                    WeekId = targetCompletedMetric.WeekId,
                    CompletedDate = DateTime.Now,
                    value = targetCompletedMetric.Value,
                });
            }

        }
        public int AddCompletedMetric(Models.Athlete.CompletedMetric targetCompletedMetric)
        {
            var upsertDetermination = @" IF EXISTS(
                                            SELECT id
                                            FROM completedMetrics 
                                            WHERE athleteID = @AthleteId 
                                            AND assignedProgramId = @AssignedProgramId
                                            AND weekId =@WeekId
                                            AND programDayItemMetricId = @ProgramDayItemMetricId )
                                            BEGIN
                                            SELECT 1
                                            END
                                            ELSE
                                            BEGIN
                                            SELECT 0
                                            END";

            var updateNeeded = false;

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                updateNeeded = sqlConn.ExecuteScalar<int>(upsertDetermination, new
                {
                    AthleteId = targetCompletedMetric.AthleteId,
                    AssignedProgramId = targetCompletedMetric.AssignedProgramId,
                    WeekId = targetCompletedMetric.WeekId,
                    ProgramDayItemMetricId = targetCompletedMetric.ProgramDayItemMetricId
                }) == 1;
            }

            if (updateNeeded)
            {
                var updateString = @" UPDATE completedMetrics 
                                  SET value = @value , completedDate = @completedDate
                                  WHERE athleteID = @AthleteId 
                                  AND assignedProgramId = @AssignedProgramId
                                  AND weekId = @WeekId
                                  AND programDayItemMetricId = @ProgramDayItemMetricId "; //adding athlete id so they dont update other athletes shit
                using (var sqlConn = new SqlConnection(ConnectionString))
                {
                    return sqlConn.ExecuteScalar<int>(updateString, new
                    {
                        CompletedDate = DateTime.Now.ToShortDateString(),
                        value = targetCompletedMetric.Value,
                        AssignedProgramId = targetCompletedMetric.AssignedProgramId,
                        WeekId = targetCompletedMetric.WeekId,
                        ProgramDayItemMetricId = targetCompletedMetric.ProgramDayItemMetricId,
                        AthleteId = targetCompletedMetric.AthleteId
                    });
                }
            }
            else
            {
                var insertString = @" INSERT INTO[dbo].[CompletedMetrics] 
                                ([Value] 
                                ,[CompletedDate] 
                                ,[AssignedProgramId] 
                                ,[AthleteId] 
                                ,[MetricId]
                                ,[ProgramDayItemMetricId]
                                ,[WeekId]) 
                                VALUES 
                                (@Value 
                                ,@CompletedDate 
                                ,@AssignedProgramId  
                                ,@AthleteId 
                                ,@MetricId
                                ,@ProgramDayItemMetricId
                                ,@WeekId);  SELECT SCOPE_IDENTITY(); ";
                using (var sqlConn = new SqlConnection(ConnectionString))
                {
                    return sqlConn.ExecuteScalar<int>(insertString, new
                    {
                        Value = targetCompletedMetric.Value,
                        CompletedDate = DateTime.Now.ToShortDateString(),
                        AssignedProgramId = targetCompletedMetric.AssignedProgramId,
                        AthleteId = targetCompletedMetric.AthleteId,
                        MetricId = targetCompletedMetric.MetricId,
                        ProgramDayItemMetricId = targetCompletedMetric.ProgramDayItemMetricId,
                        WeekId = targetCompletedMetric.WeekId
                    });
                }
            }
        }

        public void AddCompletedSet(Models.Athlete.CompletedSet targetCompletedSet)
        {
            var upsertString = @" IF EXISTS (SELECT 1 FROM completedSets as CS where CS.AssignedProgramId = @AssignedProgramID AND cs.AthleteId = @AthleteID AND cs.OriginalSetId = @OriginalSetId)
                                BEGIN
                                    UPDATE completedSets 
                                    SET[weight] = @Weight
                                    where AssignedProgramId = @AssignedProgramID AND AthleteId = @AthleteID AND OriginalSetId = @OriginalSetId
                                END
                                ELSE
                                BEGIN
                                    INSERT INTO [dbo].[CompletedSets] 
                                    ([Position] 
                                    ,[Sets] 
                                    ,[Reps] 
                                    ,[Percent] 
                                    ,[Weight] 
                                    ,[AthleteId] 
                                    ,[AssignedProgramId] 
                                    ,[CompletedDate] 
                                    ,[OriginalSetId]) 
                                    VALUES 
                                    (@Position 
                                    ,@Sets 
                                    ,@Reps 
                                    ,@Percent 
                                    ,@Weight 
                                    ,@AthleteId
                                    ,@AssignedProgramId
                                    ,@CompletedDate
                                    ,@OriginalSetId); 
                                END;
                                ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(upsertString, new
                {
                    Position = targetCompletedSet.Position,
                    Sets = targetCompletedSet.Sets,
                    Reps = targetCompletedSet.Reps,
                    Percent = targetCompletedSet.Percent,
                    Weight = targetCompletedSet.Weight,
                    OriginalSetId = targetCompletedSet.OriginalSetId,
                    CompletedDate = DateTime.Now,
                    AssignedProgramId = targetCompletedSet.AssignedProgramId,
                    AthleteId = targetCompletedSet.AthleteId
                });
            }
        }
        public void AddCompletedAssignedProgram_ProgramDayItemSuperSet_Set(Models.Athlete.CompletedSuperSet_Set targetCompletedSet)
        {
            var updateString = @"UPDATE AssignedProgram_ProgramDayItemSuperSet_Set
                                SET Completed_Sets = @Sets,
	                                Completed_Reps = @Reps,
	                                Completed_Weight = @Weight,
	                                Completed_RepsAchieved = @CompletedRepsAchieved,
	                                LastCompletedUpdateTime = @CompletedDate
                                WHERE id = @AssignedProgramSuperSet_SetId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new
                {
                    Sets = targetCompletedSet.Sets,
                    Reps = targetCompletedSet.Reps,
                    Weight = targetCompletedSet.Weight,
                    CompletedDate = DateTime.Now,
                    AssignedProgramSuperSet_SetId = targetCompletedSet.OriginalSuperSet_SetId,
                    AthleteId = targetCompletedSet.AthleteId,
                    CompletedRepsAchieved = targetCompletedSet.RepsAchieved
                });
            }
        }

        public void AddCompletedSuperSet(Models.Athlete.CompletedSuperSet_Set targetCompletedSet)
        {
            var upsertString = @" IF EXISTS (SELECT 1 FROM CompletedSuperSet_Set as CS where CS.AssignedProgramId = @AssignedProgramID AND cs.AthleteId = @AthleteID AND cs.OriginalSuperSet_setId = @OriginalSuperSet_SetId)
                                BEGIN
                                    UPDATE CompletedSuperSet_Set 
                                    SET[weight] = @Weight, CompletedRepsAchieved = @CompletedRepsAchieved
                                    where AssignedProgramId = @AssignedProgramID AND AthleteId = @AthleteID AND OriginalSuperSet_setId = @OriginalSuperSet_SetId
                                END
                                ELSE
                                BEGIN
                                    INSERT INTO [dbo].[CompletedSuperSet_Set] 
                                    ([Position] 
                                    ,[Sets] 
                                    ,[Reps] 
                                    ,[Percent] 
                                    ,[Weight] 
                                    ,[OriginalSuperSet_Setid]
                                    ,[AthleteId] 
                                    ,[AssignedProgramId] 
                                    ,[CompletedDate]
                                    ,[CompletedRepsAchieved]) 
                                    VALUES 
                                    (@Position 
                                    ,@Sets 
                                    ,@Reps 
                                    ,@Percent 
                                    ,@Weight 
                                    ,@OriginalSuperSet_SetId
                                    ,@AthleteId
                                    ,@AssignedProgramId
                                    ,@CompletedDate
                                    ,@CompletedRepsAchieved); 
                                END;
                                ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(upsertString, new
                {
                    Position = targetCompletedSet.Position,
                    Sets = targetCompletedSet.Sets,
                    Reps = targetCompletedSet.Reps,
                    Percent = targetCompletedSet.Percent,
                    Weight = targetCompletedSet.Weight,
                    OriginalSuperSet_SetId = targetCompletedSet.OriginalSuperSet_SetId,
                    CompletedDate = DateTime.Now,
                    AssignedProgramId = targetCompletedSet.AssignedProgramId,
                    AthleteId = targetCompletedSet.AthleteId,
                    CompletedRepsAchieved = targetCompletedSet.RepsAchieved
                });
            }
        }

        public async Task AssignProgramToAthlete(int athleteId, int snapShotProgramIdId)
        {
            var assign = @"IF EXISTS(SELECT AssignedProgram_assignedProgramId FROM athletes WHERE id = @athleteId AND AssignedProgram_assignedProgramId IS NOT NULL)
                            BEGIN
                                INSERT INTO AssignedProgram_AssignedProgramHistory (AssignedDate, AssignedProgram_ProgramId,AthleteId)
                                SELECT @today,AssignedProgram_assignedProgramId,@athleteId FROM athletes WHERE id = @athleteId
                            END
                        UPDATE athletes
                        SET  AssignedProgram_assignedProgramId = @programId
                        WHERE id = @athleteId
                            ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                await sqlConn.ExecuteAsync(assign, new { today = DateTime.Now, programId = snapShotProgramIdId, athleteId = athleteId });
            }
        }

        public void AssignProgramToAthletes(List<int> athleteIds, int programId, int organizationId)
        {


            string updateString = $@"IF EXISTS (SELECT assignedProgramId FROM athletes WHERE id = @athleteId AND assignedProgramId IS NOT NULL)
                                    BEGIN
                                           INSERT INTO [AthleteProgramHistories](AthleteId,startDate,endDate,AssignedProgramId,ArchivedDate)
                                             SELECT @athleteId,ProgramStartDate,ProgramEndDate,AssignedProgramId,getDate()  FROM athletes WHERE Id = @AthleteId 
                                    END
                                            
                                    UPDATE athletes 
                                    SET AssignedProgramId = @ProgramId 
                                    WHERE Id = @AthleteId AND OrganizationId = @organizationId";

            var markItemsAsCantModify = $@"UPDATE Metrics 
                                        SET CanModify = 0
                                        WHERE Id IN (
                                        SELECT pdim.MetricId
                                        FROM programs AS p 
                                        INNER JOIN ProgramDays AS pd ON pd.ProgramId = p.Id
                                        INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                                        INNER JOIN ProgramDayItemMetrics AS pdim ON pdi.Id = pdim.Id
                                        WHERE p.id = {programId} AND pdi.ItemEnum = {(int)ProgramDayItemEnum.Metric })

                                        UPDATE Movies 
                                        SET CanModify = 0
                                        WHERE Id IN (
                                        SELECT pdim.MovieId
                                        FROM programs AS p 
                                        INNER JOIN ProgramDays AS pd ON pd.ProgramId = p.Id
                                        INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                                        INNER JOIN ProgramDayItemMovies AS pdim ON pdi.Id = pdim.Id
                                        WHERE p.id = {programId} AND pdi.ItemEnum = {(int)ProgramDayItemEnum.Video })


                                        UPDATE Exercises 
                                        SET CanModify = 0
                                        WHERE id IN (SELECT sse.exerciseId
                                                    FROM programs AS p 
                                                    INNER JOIN ProgramDays AS pd ON  p.id = pd.programid
                                                    INNER JOIN programDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                                                    INNER JOIN ProgramDayItemSuperSets AS pdiss ON pdiss.ProgramDayItemId = pdi.id
                                                    INNER JOIN SuperSetExercises AS sse ON sse.ProgramDayItemSuperSetId = pdiss.id
                                                    WHERE p.id = {programId} AND pdi.ItemEnum = {(int)ProgramDayItemEnum.SuperSet })

                                        UPDATE Exercises 
                                        SET CanModify = 0
                                        WHERE id IN 
                                        (SELECT pdie.ExerciseId
                                        FROM programs AS p 
                                        INNER JOIN ProgramDays AS pd ON pd.ProgramId = p.Id
                                        INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                                        INNER JOIN ProgramDayItemExercises  AS pdie ON pdie.ProgramDayItemId = pdi.Id
                                        WHERE p.id = {programId} AND pdi.ItemEnum = {(int)ProgramDayItemEnum.Workout})

                                        UPDATE Surveys
                                        SET CanModify = 0
                                        WHERE id IN(
                                        SELECT pdis.Id
                                        FROM programs AS p 
                                        INNER JOIN ProgramDays AS pd ON pd.ProgramId = p.Id
                                        INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                                        INNER JOIN ProgramDayItemSurveys AS pdis ON pdis.ProgramDayItemId = pdi.Id
                                        WHERE p.id = {programId} AND pdi.ItemEnum = {(int)ProgramDayItemEnum.Survey})

                                        UPDATE programs 
                                        SET CanModify = 0
                                        WHERE id = {programId}";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(markItemsAsCantModify);
                athleteIds.ForEach(x =>
                {
                    sqlConn.Execute(updateString, new { AthleteId = x, organizationId = organizationId, ProgramId = programId });
                });
            }
        }
        public void Archive(int athleteId, Guid userToken)
        {
            var deleteString = $@"
                                DECLARE @targetUserId INT = 0;

                                DELETE FROM CompletedAssignedProgramDays where athleteId = @AthleteId
                                SELECT @targetUserId = athleteUserId FROM Athletes WHERE id = @AthleteId
                                DELETE FROM CompletedSuperSet_Set WHERE athleteId = @AthleteId
                                DELETE FROM AddedMetrics WHERE athleteId = @AthleteId
                                DELETE FROM AthleteHeightWeights WHERE athleteId = @AthleteId
                                DELETE FROM AthleteInjuries WHERE athleteId = @AthleteId
                                DELETE FROM AthleteNotes WHERE athleteId = @AthleteId
                                DELETE FROM CompletedMetrics WHERE athleteId = @AthleteId
                                DELETE FROM CompletedQuestionOpenEndeds WHERE athleteId =@AthleteId 
                                DELETE FROM CompletedProgramDays WHERE athleteId = @AthleteId
                                DELETE FROM CompletedProgramWeeks WHERE athleteId = @AthleteId
                                DELETE FROM CompletedQuestionScales WHERE athleteId = @AthleteId
                                DELETE FROM CompletedQuestionYesNoes WHERE athleteId = @AthleteId
                                DELETE FROM AthleteProgramHistories WHERE athleteId = @AthleteId
                                DELETE FROM CompletedSets WHERE athleteId = @AthleteId
                                DELETE FROM TagsToAthletes WHERE athleteId = @AthleteId

                                DELETE FROM UserTokens where userId = @targetUserId

                                DECLARE @messageId int = 0

                                select @messageId = id from Messages where createdUserId = @targetUserId

                                DELETE FROM MessagesToUsersInGroups where DestinationUserId = @TargetUserId
                                DELETE FROM messagesToUsers WHERE  DestinationUserId = @TargetUserId
                                DELETE FROM [MessageGroupsToUsers] WHERE userId = @TargetUserId
                                DELETE FROM MessagesToUsersInGroups where messageId = @messageId
                                DELETE FROM messagesToUsers WHERE  messageId = @messageId
                                DELETE FROM [messages] where id = @messageId

                                DELETE FROM Notifications WHERE DestinationUserId = @TargetUserId
                                DELETE FROM Notifications WHERE GeneratedAthleteId = @AthleteId
                                DELETE FROM passwordresets WHERE userId = @TargetUserId


								INSERT INTO #assignedProgramsWithDayIds
								select A.id, pd.id, pdi.Id
								FROM AssignedProgram_Program AS a
								INNER JOIN AssignedProgram_ProgramDay AS pd ON a.Id = pd.Id
								INNER JOIN AssignedProgram_AssignedProgramHistory AS aph ON aph.AssignedProgram_ProgramId = a.Id
								INNER JOIN AssignedProgram_ProgramDayItem AS pdi on PDI.AssignedProgram_ProgramDayId = pd.ID
								WHERE aph.AthleteId = @athleteid


								INSERT INTO #assignedProgramsWithDayIds
								select A.AssignedProgram_AssignedProgramId, pd.id, pdi.Id
								FROM Athletes AS a
								INNER JOIN AssignedProgram_ProgramDay AS pd ON a.AssignedProgram_AssignedProgramId = pd.Id
								INNER JOIN AssignedProgram_ProgramDayItem AS pdi on PDI.AssignedProgram_ProgramDayId = pd.id
								WHERE a.Id = @athleteid

								DELETE mdw
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN AssignedProgram_ProgramDayItemMovie AS pdim on ap.ap_pdi_id =pdim.AssignedProgram_ProgramDayItemId
								INNER JOIN AssignedProgram_MovieDisplayWeek AS mdw ON mdw.AssignedProgram_ProgramDayItemMovieId = pdim.Id

								DELETE pdim
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN AssignedProgram_ProgramDayItemMovie AS pdim on ap.ap_pdi_id =pdim.AssignedProgram_ProgramDayItemId

								DELETE ndw
								FROM #assignedProgramsWithDayIds AS ap
								inner join assignedProgram_programdayitemnote AS pdimn ON pdimn.AssignedProgram_ProgramDayItemId = ap_pdi_id
								INNER JOIN AssignedProgram_NoteDisplayWeek AS ndw ON ndw.AssignedProgram_ProgramDayItemNoteId = pdimn.Id

								DELETE pdimn
								FROM #assignedProgramsWithDayIds AS ap
								inner join assignedProgram_programdayitemnote AS pdimn ON pdimn.AssignedProgram_ProgramDayItemId = ap_pdi_id

								DELETE mdw
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdim ON pdim.AssignedProgram_ProgramDayItemId = ap_pdi_id
								INNER JOIN AssignedProgram_MetricsDisplayWeek AS mdw ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.Id

								DELETE pdim
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdim ON pdim.AssignedProgram_ProgramDayItemId = ap_pdi_id

								DELETE CD
								FROM #assignedProgramsWithDayIds AS ap
								inner join AssignedProgram_CompletedDay as CD on CD.AssignedProgram_ProgramDayId = AP.ap_pd_id

								DELETE ssndw
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN  AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.AssignedProgram_ProgramDayItemId = ap_pdi_id
								INNER join AssignedProgram_SuperSetNote AS ssn ON ssn.AssignedProgram_ProgramDayItemSuperSetId = pdiss.Id
								INNER JOIN AssignedProgram_SuperSetNoteDisplayWeek AS ssndw ON ssndw.AssignedProgram_SuperSetNoteId = ssn.Id

								
								DELETE ssn
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN  AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.AssignedProgram_ProgramDayItemId = ap_pdi_id
								INNER join AssignedProgram_SuperSetNote AS ssn ON ssn.AssignedProgram_ProgramDayItemSuperSetId = pdiss.Id


								DELETE pdiss_s
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN  AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.AssignedProgram_ProgramDayItemId = ap_pdi_id
								INNER JOIN AssignedProgram_SuperSetExercise AS sse ON sse.AssignedProgram_ProgramDayItemSuperSetId = pdiss.Id
								INNER JOIN AssignedProgram_ProgramDayItemSuperSetWeek AS ssw ON ssw.AssignedProgram_SuperSetExerciseId = sse.id
								INNER JOIN AssignedProgram_ProgramDayItemSuperSet_Set AS pdiss_s ON pdiss_s.AssignedProgram_ProgramDayItemSuperSetWeekId = ssw.Id

								DELETE ssw
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN  AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.AssignedProgram_ProgramDayItemId = ap_pdi_id
								INNER JOIN AssignedProgram_SuperSetExercise AS sse ON sse.AssignedProgram_ProgramDayItemSuperSetId = pdiss.Id
								INNER JOIN AssignedProgram_ProgramDayItemSuperSetWeek AS ssw ON ssw.AssignedProgram_SuperSetExerciseId = sse.id

								DELETE sse
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN  AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.AssignedProgram_ProgramDayItemId = ap_pdi_id
								INNER JOIN AssignedProgram_SuperSetExercise AS sse ON sse.AssignedProgram_ProgramDayItemSuperSetId = pdiss.Id

								DELETE pdiss
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN  AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.AssignedProgram_ProgramDayItemId = ap_pdi_id

								DELETE 
								FROM AssignedProgram_CompletedQuestionYesNo 
								where AthleteId = @athleteid

								
								DELETE 
								FROM AssignedProgram_CompletedQuestionOpenEnded 
								where AthleteId = @athleteid

								DELETE 
								FROM AssignedProgram_CompletedQuestionScale 
								where AthleteId = @athleteid
								DELETE SDW
								FROM #assignedProgramsWithDayIds AS ap
								inner join AssignedProgram_ProgramDayItemSurvey as PDIS on PDIS.AssignedProgram_ProgramDayItemId = ap_pdi_id
								inner join AssignedProgram_SurveyDisplayWeeks as SDW on SDW.AssignedProgram_ProgramDayItemSurveyId = PDIS.Id

								DELETE pdis
								FROM #assignedProgramsWithDayIds AS ap
								inner join AssignedProgram_ProgramDayItemSurvey as PDIS on PDIS.AssignedProgram_ProgramDayItemId = ap_pdi_id

								DELETE APH
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN AssignedProgram_AssignedProgramHistory AS aph ON aph.AssignedProgram_ProgramId = AP.ap_id

								DELETE pdi
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN AssignedProgram_ProgramDayItem AS pdi on PDI.AssignedProgram_ProgramDayId = ap.ap_pd_id

								DELETE pd
								FROM #assignedProgramsWithDayIds AS ap
								inner join AssignedProgram_ProgramDay AS pd ON ap.ap_pd_id = pd.Id

								DELETE AP
								FROM #assignedProgramsWithDayIds AS ap
								INNER JOIN AssignedProgram_Program AS p ON ap.ap_id = p.Id

								
                                DELETE FROM Athletes Where Id = @AthleteId
                                DELETE FROM Users where id = @targetUserId

								DROP TABLE #assignedProgramsWithDayIds
                              ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(deleteString, new { AthleteId = athleteId });
            }
        }
        public void AddProfilePictureToAthlete(int athleteId, int pictureId, Guid createdUserToke)
        {
            string updateString = $@" UPDATE athletes 
                                  SET ProfilePictureId = @PictureId 
                                 WHERE createdUserId = ({ConstantSqlStrings.GetUserIdFromToken})
                                 AND Id = @AthleteId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { PictureId = pictureId, AthleteId = athleteId, Token = createdUserToke });
            }
        }
        public void AddStandAloneMetric(int athleteId, int metricId, double value, int enteredByPerson, DateTime timeOfCompletion)
        {
            if (timeOfCompletion == DateTime.MinValue) timeOfCompletion = DateTime.Now;//somehow the ui widget sends in null for the current timestamp which is turned into datetime.min
            string insertString = @" INSERT INTO [dbo].[AddedMetrics] (MetricId,Value,CompletedDate,AthleteId,EnteredByUserId)
                                     VALUES (@MetricId, @Value,@CompletedDate,@AthleteId, @EnteredByUserId)";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insertString, new { MetricId = metricId, Value = value, CompletedDate = timeOfCompletion, AthleteId = athleteId, EnteredByUserId = enteredByPerson });
            }

        }
        public void UpdateAthlete(AthleteDTO newInfo, Guid updatingCoachGuid)
        {
            string update = $@"
                            UPDATE[dbo].[Athletes]
                                    SET[FirstName] = @FirstName
                                  ,[LastName] = @LastName
                                  ,[Birthday] = @Birthday
                             WHERE Id = @athleteId AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(update, new { AthleteId = newInfo.Id, FirstName = newInfo.FirstName, LastName = newInfo.LastName, Birthday = newInfo.Birthday, Token = updatingCoachGuid });
            }

        }
        public AthleteHeightWeight GetLatestAthleteBioMetrics(int athleteId)
        {
            string getString = $@"SELECT 
                                        [AthleteId]
                                        ,[HeightPrimary]
                                        ,[HeightSecondary]
                                        ,[Weight]
                                        , [AddedDate]
                                     FROM [dbo].[AthleteHeightWeights] where athleteID = @AthleteId ORDER BY addedDate Desc ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<AthleteHeightWeight>(getString, new { AthleteId = athleteId }).FirstOrDefault();
            }
        }
        public void UpdateAthleteHeightWeight(int athleteId, double? heightPrimary, double? heightSecondary, double? weight)
        {
            string insertString = $@"INSERT INTO [dbo].[AthleteHeightWeights]
                                        ([AthleteId]
                                        ,[HeightPrimary]
                                        ,[HeightSecondary]
                                        ,[Weight]
                                        ,[AddedDate])
                                    VALUES
                                        (@AthleteId
                                        ,@HeightPrimary
                                        ,@HeightSecondary
                                        ,@Weight
                                        ,@AddedDate)";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insertString, new { AthleteId = athleteId, HeightPrimary = heightPrimary, HeightSecondary = heightSecondary, Weight = weight, AddedDate = DateTime.Now });
            }
        }
        public void UpdateEmailValidationToken(string emailValidationToken, int athleteId)
        {
            string updateString = "UPDATE athletes SET [EmailValidationToken] = @EmailValidationToken WHERE Id = @AthleteId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { EmailValidationToken = emailValidationToken, AthleteId = athleteId });
            }
        }
        public int CreateAthlete(string firstName, string lastName, int createdUserId, int athleteUserId, string EmailValidationToken, DateTime tokenIssuedDate, DateTime? birthday)
        {
            string insertString = @" INSERT INTO [dbo].[Athletes] 
                  ([FirstName] 
                  ,[LastName] 
                  ,[CreatedUserId] 
                  ,[AthleteUserId]
                  ,[IsDeleted]
                  ,[ValidatedEmail]
                  ,[EmailValidationToken]
                  ,[TokenIssued]
                  ,OrganizationId
                   ,BirthDay)
                  VALUES 
                  (@FirstName,@LastName,@CreatedUserId,@UserId,0,0, @EmailValidationTOken, @TokenIssuedDate, (SELECT OrganizationId FROM users WHERE id = @CreatedUserId), @BirthDay); SELECT SCOPE_IDENTITY()";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return int.Parse(sqlConn.ExecuteScalar(insertString,
                    new
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        CreatedUserId = createdUserId,
                        UserId = athleteUserId,
                        EmailValidationToken = EmailValidationToken,
                        TokenIssuedDate = tokenIssuedDate,
                        BirthDay = birthday
                    }).ToString());
            }
        }
        public Athlete GetAthleteByAthleteUserId(int athleteUserId)
        {
            string getString = $@" SELECT * FROM athletes AS a
                                   LEFT JOIN pictures AS p on p.Id = a.profilePictureId
                                   WHERE a.AthleteUserId = @AthleteId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Models.Athlete.Athlete, Models.MultiMedia.Picture, Models.Athlete.Athlete>(getString, (a, p) =>
                {
                    a.ProfilePicture = p;
                    return a;
                }, new { AthleteId = athleteUserId }).FirstOrDefault();
            }
        }
        public Models.Athlete.Athlete GetAthlete(int athleteId, Guid createdUserToken)
        {
            string getString = $@" SELECT * FROM athletes AS a
                                   LEFT JOIN pictures AS p on p.Id = a.profilePictureId
                                   WHERE a.OrganizationId = (SELECT OrganizationId FROM users WHERE Id = ({ConstantSqlStrings.GetUserIdFromToken}))  AND a.Id = @AthleteId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Models.Athlete.Athlete, Models.MultiMedia.Picture, Models.Athlete.Athlete>(getString, (a, p) =>
                {
                    a.ProfilePicture = p;
                    return a;
                }, new { Token = createdUserToken, AthleteId = athleteId }).FirstOrDefault();
            }
        }
        public Models.Athlete.Athlete GetAthlete(int athleteId)
        {
            string getString = " SELECT * FROM athletes AS a"
                             + " LEFT JOIN pictures AS p on p.Id = a.profilePictureId"
                             + $" WHERE  a.Id = @AthleteId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Models.Athlete.Athlete, Models.MultiMedia.Picture, Models.Athlete.Athlete>(getString, (a, p) =>
                {
                    a.ProfilePicture = p;
                    return a;
                }, new { AthleteId = athleteId }).FirstOrDefault();
            }
        }
        public Athlete GetAthleteByWeightRoom(int id, Guid weightRoomTokenId)
        {
            string getString = $@" SELECT * FROM athletes AS a 
                                   LEFT JOIN pictures AS p on p.Id = a.profilePictureId
                                   WHERE a.id = @AthleteId
                                    AND a.organizationId = (SELECT OrganizationId FROM WeightRoomAccounts WHERE token = @WeightRoomTokenId) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Models.Athlete.Athlete, Models.MultiMedia.Picture, Models.Athlete.Athlete>(getString, (a, p) =>
                {
                    a.ProfilePicture = p;
                    return a;
                }, new { AthleteId = id, WeightRoomTokenId = weightRoomTokenId }).FirstOrDefault();
            }

        }
        public Models.Athlete.Athlete GetAthlete(Guid athleteToken)
        {
            string getString = " SELECT * FROM athletes AS a "
                             + " LEFT JOIN pictures AS p on p.Id = a.profilePictureId"
                             + $" WHERE a.AthleteUserId = ({ConstantSqlStrings.GetUserIdFromToken})  ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Models.Athlete.Athlete, Models.MultiMedia.Picture, Models.Athlete.Athlete>(getString, (a, p) =>
                {
                    a.ProfilePicture = p;
                    return a;
                }, new { Token = athleteToken }).FirstOrDefault();
            }
        }

        public List<Models.Athlete.Athlete> GetAllAthletes(Guid createdUserToken)
        {
            //i have no fucking clue how the nulls are getting in
            string getString = $@" SELECT * FROM athletes AS a
                                  LEFT JOIN pictures AS p on p.Id = a.profilePictureId
                                  WHERE a.OrganizationId = (SELECT OrganizationId FROM users WHERE Id = ({ConstantSqlStrings.GetUserIdFromToken})) 
                                    AND (FirstName IS NOT NULL OR LastName IS NOT NULL)";
            //var ret = new List<Models.Athlete.Athlete>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                return sqlConn.Query<Models.Athlete.Athlete, Models.MultiMedia.Picture, Models.Athlete.Athlete>(getString, (a, p) =>
                      {
                          a.ProfilePicture = p;
                          return a;
                      }, new { Token = createdUserToken }).ToList();
            }
        }
        public List<DTOs.Athlete.AssignedProgramAthleteDTO> GetAllAssignedProgramAthletes(List<int> athleteIds)
        {
            string OldProgramsString = $@"SELECT a.id as AthleteId, a.FirstName, a.LastName, p.[name] as Programname, pic.Thumbnail, pic.[URL] as PictureBaseURL, ap.AssignedDate, profile as 'ProfilePicture'
                                    FROM athletes AS a
                                    INNER JOIN AssignedPrograms AS ap on a.AssignedProgramId = ap.Id
                                    INNER JOIN programs AS p ON p.id = ap.ProgramId
                                    LEFT JOIN pictures AS pic ON pic.id = a.ProfilePictureId
                                    WHERE a.id in @AthleteIds ";

            string Snapshots = $@"SELECT a.id as AthleteId, a.FirstName, a.LastName, p.[name] as Programname, pic.Thumbnail, pic.[URL] as PictureBaseURL, a.AssignedProgram_AssignedTime, profile as 'ProfilePicture'
                                    FROM athletes AS a 
                                    INNER JOIN AssignedProgram_Program AS p ON a.AssignedProgram_AssignedProgramId = p.Id
                                    LEFT JOIN pictures AS pic ON pic.id = a.ProfilePictureId
                                    WHERE a.id in @AthleteIds";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var athletesWithOldProgramsAssigned = sqlConn.Query<DTOs.Athlete.AssignedProgramAthleteDTO>(OldProgramsString, new { AthleteIds = athleteIds }).ToList();
                var athletesWithSnapShotsAssigend = sqlConn.Query<DTOs.Athlete.AssignedProgramAthleteDTO>(Snapshots, new { AthleteIds = athleteIds }).ToList();

                var athletesWithOldProgarmsAndNoSnapShots = athletesWithOldProgramsAssigned.Select(x => x.AthleteId).Except(athletesWithSnapShotsAssigend.Select(y => y.AthleteId));

                var allPreviouslyAssignedPrograms = new List<AssignedProgramAthleteDTO>();

                allPreviouslyAssignedPrograms.AddRange(athletesWithSnapShotsAssigend);
                allPreviouslyAssignedPrograms.AddRange(athletesWithOldProgramsAssigned.Where(x => athletesWithOldProgarmsAndNoSnapShots.Contains(x.AthleteId)).ToList());

                return allPreviouslyAssignedPrograms;

            }
        }
        public List<Models.Athlete.Athlete> GetAllAthletes(int orgId)
        {
            string getString = $@" SELECT * FROM athletes AS a
                                  LEFT JOIN pictures AS p on p.Id = a.profilePictureId
                                  WHERE a.OrganizationId = @OrgId
                                    AND (FirstName IS NOT NULL OR LastName IS NOT NULL)";
            //var ret = new List<Models.Athlete.Athlete>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                return sqlConn.Query<Models.Athlete.Athlete, Models.MultiMedia.Picture, Models.Athlete.Athlete>(getString, (a, p) =>
                {
                    a.ProfilePicture = p;
                    return a;
                }, new { OrgId = orgId }).ToList();
            }
        }
        public List<AthleteWithTagsDTO> GetAllAthletesTagMappings(Guid userToken)
        {
            var tagMappings = $"SELECT t.AthleteId ,t.TagId, ta.Name FROM TagsToAthletes AS t INNER JOIN AthleteTags AS ta ON ta.Id = t.TagId INNER JOIN Athletes AS E on t.AthleteId = e.Id WHERE e.OrganizationId = (SELECT OrganizationId FROM users WHERE Id = ({ConstantSqlStrings.GetUserIdFromToken}))";
            var tagMappingDTOs = new List<AthleteWithTagsDTO>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var reader = sqlConn.ExecuteReader(tagMappings, new { token = userToken });
                while (reader.Read())
                {
                    var targetDtoMapping = tagMappingDTOs.FirstOrDefault(x => x.AthleteId == reader.GetInt32(0));
                    var newTagDTO = new TagDTO() { Id = reader.GetInt32(1), Name = reader.GetString(2) };
                    if (targetDtoMapping == null)
                    {

                        tagMappingDTOs.Add(new AthleteWithTagsDTO() { AthleteId = reader.GetInt32(0), Tags = new List<TagDTO>() { newTagDTO } });
                    }
                    else
                    {
                        targetDtoMapping.Tags.Add(newTagDTO);
                    }
                }
            }
            return tagMappingDTOs;
        }
        public List<AthleteWithTagsDTO> GetAllAthletesTagMappings(int orgId)
        {
            var tagMappings = $@"SELECT t.AthleteId ,t.TagId, ta.Name 
                                 FROM TagsToAthletes AS t
                                INNER JOIN AthleteTags AS ta ON ta.Id = t.TagId 
                                INNER JOIN Athletes AS E on t.AthleteId = e.Id 
                                WHERE e.OrganizationId = @OrgId";
            var tagMappingDTOs = new List<AthleteWithTagsDTO>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var reader = sqlConn.ExecuteReader(tagMappings, new { OrgId = orgId });
                while (reader.Read())
                {
                    var targetDtoMapping = tagMappingDTOs.FirstOrDefault(x => x.AthleteId == reader.GetInt32(0));
                    var newTagDTO = new TagDTO() { Id = reader.GetInt32(1), Name = reader.GetString(2) };
                    if (targetDtoMapping == null)
                    {

                        tagMappingDTOs.Add(new AthleteWithTagsDTO() { AthleteId = reader.GetInt32(0), Tags = new List<TagDTO>() { newTagDTO } });
                    }
                    else
                    {
                        targetDtoMapping.Tags.Add(newTagDTO);
                    }
                }
            }
            return tagMappingDTOs;
        }
        public void MarkDayCompleted(int programDayId, int assignedProgramId, Models.Athlete.Athlete assignedAthlete)
        {
            var sql = $"INSERT INTO completedProgramDays (AthleteId,AssignedProgramId, ProgramDayId) VALUES ({assignedAthlete.Id},{assignedProgramId},{programDayId})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(sql);
            }
        }
        public void MarkWeekCompleted(int weekId, int assignedProgramId, Models.Athlete.Athlete assignedAthlete)
        {
            var sql = $"INSERT INTO CompletedProgramWeek (AthleteId,AssignedProgramId, ProgramWeek) VALUES ({assignedAthlete.Id},{assignedProgramId}, {weekId})";//this isnt a real id. we are counting weeks 1,2,3,4,5 so it is just the week number
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(sql);
            }
        }


    }
}
