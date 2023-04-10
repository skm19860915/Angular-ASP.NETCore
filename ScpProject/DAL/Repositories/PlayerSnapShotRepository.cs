using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Models.Enums;
using Models.Program.AssignedProgramSnapShots;

namespace DAL.Repositories
{
    public interface IPlayerSnapShotRepository
    {
        Task AddMetric(int metricId, int assignedProgram_ProgramDayId, List<int> displayWeeks, int position);
        Task AddMovie(int assignedProgram_ProgramDayId, int movieId, List<int> displayWeeks, int position);
        Task AddNote(string note, string name, List<int> displayWeeks, int position, int assignedProgram_ProgramDayId);
        Task AddSurvey(int surveyId, List<int> displayWeeks, int position, int assignedProgram_programDayId);
        Task DeleteMetric(int assignedProgram_ProgramDayItemMetricId);
        Task DeleteMovie(int assignedProgram_ProgramDayItemMovieId);
        Task DeleteNote(int assignedProgram_ProgramDayItemNoteId);
        Task DeleteSuperSetInWeek(int AssignedProgram_ProgramDayItemSuperSetWeekId, int position);
        Task DeleteSurvey(int assignedProgram_programDayItemSurveyId);
        Task AddSuperSetInWeek(string other, int AssignedProgram_ProgramDayItemSuperSetWeekId, int position, int? sets, int? reps, double? percent, double? weight, int? minutes, int? seconds, string distance, bool? repsAchieved, int athleteId, int exerciseId);
        Task UpdateMetricDisplayWeeks(List<int> newDisplayWeeks, int AssignedProgram_ProgramDayItemMetricId);
        Task UpdateSuperSetInWeek(string other, int superset_setId, int AssignedProgram_ProgramDayItemSuperSetWeekId, int position, int? sets, int? reps, double? percent, double? weight,
            int? minutes, int? seconds, string distance, bool? repsAchieved, int athleteId, int exerciseId);
        Task UpdateSuperSetOtherInWeek(string other, int superset_setId, int AssignedProgram_ProgramDayItemSuperSetWeekId, int position);
        Task UpdateSurveyDisplayWeeks(List<int> newSurveyDisplayWeeks, int assignedProgram_programdayItemSurveyId, List<int> displayWeeksToDelete);
        Task DeleteSuperSetExercise(int superSetExerciseId);
        Task UpdateProgramWeekDayCount(int weekCount, int dayCount, int programId);
        Task UpdatePositionsToIncludedAddedOne(int position, int assignedProgram_programDayId);
        Task UpdatePositionsToIncludedRemovedOne(int position, int assignedProgram_programDayId);
        Task DeleteProgramDayItemSuperSet(int assignedProgram_programdayItemSuperSetId);
        Task<int> AddNewProgramDay(int position, int programId);
        Task JustDeleteAssignedProgramDay(int programDayId);
        Task UpdateProgramInfo(string programTitle, int programId);
        Task UpdateRest(string rest, int assignedProgram_programDayItemSuperSetId);
        Task<int> AddSuperSetWeek(int position, int assignedProgram_SuperSetExerciseId);
        Task MarkDayAsComplete(int assignedProgram_ProgramDayId, int weekId);
        List<AssignedProgram_CompletedDay> GetCompletedDays(int programId);
    }

    //todo remember to go throughthe final snapshot and re-arrange all the positions to be consecutive
    //when you remove something there will be gaps between the positions
    public class PlayerSnapShotRepository : IPlayerSnapShotRepository
    {
        private string ConnectionString;
        public PlayerSnapShotRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public async Task MarkDayAsComplete(int assignedProgram_ProgramDayId, int weekId)
        {
            var insertSql = @"
                                IF NOT EXISTS ( SELECT 1 FROM assignedProgram_completedDay WHERE assignedProgram_programDayId =@assignedProgram_ProgramDayId AND  weekId = @weekId )
                                BEGIN
                                    INSERT INTO assignedProgram_completedDay (assignedProgram_programDayId , weekId) VALUES(@assignedProgram_ProgramDayId, @weekId)
                                END";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                await sqlConn.ExecuteAsync(insertSql, new { weekId = weekId, assignedProgram_ProgramDayId = assignedProgram_ProgramDayId });
            }
        }
        public  List<AssignedProgram_CompletedDay> GetCompletedDays(int programId)
        {

            var getString = @"
                                SELECT  a.* 
                                FROM  assignedProgram_completedDay  AS a
                                INNER JOIN assignedProgram_programDay AS pd ON pd.id = a.assignedProgram_programDayid
                                INNER JOIN assignedProgram_Program AS ap ON ap.id = pd.assignedProgram_ProgramId
                                WHERE ap.id = @programId
                                ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return  sqlConn.Query<AssignedProgram_CompletedDay>(getString, new { programId = programId }).ToList();
            }

        }
        public async Task UpdateProgramWeekDayCount(int weekCount, int dayCount, int programId)
        {
            var updateSQL = @"UPDATE [dbo].[AssignedProgram_Program]
                          SET WeekCount = @WeekCount, DayCount = @DayCount
                          WHERE id = @id";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                await sqlConn.ExecuteAsync(updateSQL, new { weekCount = weekCount, dayCount = dayCount, id = programId });
            }
        }
        public async Task UpdateProgramInfo(string programTitle, int programId)
        {
            var updateProgram = " UPDATE dbo.assignedprogram_program SET Name = @name WHERE id = @programId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                await sqlConn.ExecuteAsync(updateProgram, new { name = programTitle, programId = programId });
            }
        }
        public async Task AddNote(string note, string name, List<int> displayWeeks, int position, int assignedProgram_ProgramDayId)
        {

            var insertSql = @"
                                DECLARE @programDayItemId INT
      
                              
                                INSERT INTO [AssignedProgram_ProgramDayItem] (AssignedProgram_ProgramDayId,  Position,ItemEnum)
                                VALUES (@assignedProgram_programDayid,@position,@itemType)


                                SELECT @programdayItemId =SCOPE_IDENTITY()

                                INSERT INTO [AssignedProgram_ProgramDayItemNote] (name,note,assignedProgram_ProgramDayItemId)
                                VALUES  (@name,@note,@programdayItemId)

                                SELECT SCOPE_IDENTITY()
                            ";
            var dipslayWeeksInsert = @"
                                        INSERT INTO [AssignedProgram_NoteDisplayWeek] (AssignedProgram_ProgramDayItemNoteId,DisplayWeek)           
                                        VALUES(@programDayItemNoteId, @DisplayWeek)
                                    ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var insertedProgramDayItemNoteId = await sqlConn.ExecuteScalarAsync<int>(insertSql, new { position, itemType = (int)ProgramDayItemEnum.Note, name = name, note = note, assignedProgram_programDayid = assignedProgram_ProgramDayId });

                for (int i = 0; i < displayWeeks.Count; i++)
                {
                    await sqlConn.ExecuteScalarAsync<int>(dipslayWeeksInsert, new { programDayItemNoteId = insertedProgramDayItemNoteId, DisplayWeek = displayWeeks[i] });
                }
            }

        }
        public async Task<int> AddSuperSetWeek(int position, int assignedProgram_SuperSetExerciseId)
        {
            var insertSql = @"      INSERT INTO AssignedProgram_ProgramDayItemSuperSetWeek (position, AssignedProgram_SuperSetExerciseId)
                                VALUES(@sswPosition, @AssignedProgram_SuperSetExerciseId)

                                SELECT SCOPE_IDENTITY(); ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                return await sqlConn.ExecuteScalarAsync<int>(insertSql, new { sswPosition = position, AssignedProgram_SuperSetExerciseId = assignedProgram_SuperSetExerciseId });
            }
        }
        public async Task DeleteNote(int assignedProgram_ProgramDayItemNoteId)
        {

            var deleteString = @"
                                        DECLARE @programDayItemId AS INT
		
	                                    SELECT @programDayItemId = pdi.Id
                                        FROM AssignedProgram_ProgramDayItem AS pdi
                                        LEFT JOIN [AssignedProgram_ProgramDayItemNote] AS pdim ON pdi.Id = pdim.AssignedProgram_ProgramDayItemId
                                        LEFT JOIN [AssignedProgram_NoteDisplayWeek] AS mdw ON pdim.Id = mdw.DisplayWeek
                                        WHERE pdim.Id = @assignedProgram_ProgramDayItemNoteId
								
	                                    DELETE mdw
                                        FROM  [AssignedProgram_NoteDisplayWeek] AS mdw 
                                        WHERE MDW.AssignedProgram_ProgramDayItemNoteId = @assignedProgram_ProgramDayItemNoteId

	                                    DELETE pdim
                                        from [AssignedProgram_ProgramDayItemNote] AS pdim 
                                        WHERE pdim.Id  = @assignedProgram_ProgramDayItemNoteId



                                        DELETE  PDI 
                                        FROM AssignedProgram_ProgramDayItem AS pdi
                                        WHERE Id = @programDayItemId

";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                await sqlConn.ExecuteAsync(deleteString, new { assignedProgram_ProgramDayItemNoteId = assignedProgram_ProgramDayItemNoteId });
            }
        }
        public async Task DeleteMovie(int assignedProgram_ProgramDayItemMovieId)
        {

            var deleteString = @"
                                    DECLARE @programDayId AS INT


                                    SELECT @programDayId = pdi.Id
                                    FROM AssignedProgram_ProgramDayItem AS pdi
                                    INNER JOIN [AssignedProgram_ProgramDayItemMovie] AS pdim ON pdi.Id = pdim.AssignedProgram_ProgramDayItemId
                                    INNER JOIN [AssignedProgram_MovieDisplayWeek] AS mdw ON pdim.Id = mdw.AssignedProgram_ProgramDayItemMovieId
                                    WHERE pdim.Id = @assignedProgram_ProgramDayItemMovieId

                                    DELETE mdw
                                    FROM AssignedProgram_ProgramDayItem AS pdi
                                    INNER JOIN [AssignedProgram_ProgramDayItemMovie] AS pdim ON pdi.Id = pdim.AssignedProgram_ProgramDayItemId
                                    INNER JOIN [AssignedProgram_MovieDisplayWeek] AS mdw ON pdim.Id = mdw.AssignedProgram_ProgramDayItemMovieId
                                    WHERE pdim.Id = @assignedProgram_ProgramDayItemMovieId

                                    DELETE pdim
                                    FROM AssignedProgram_ProgramDayItem AS pdi
                                    INNER JOIN [AssignedProgram_ProgramDayItemMovie] AS pdim ON pdi.Id = pdim.AssignedProgram_ProgramDayItemId
                                    WHERE pdim.Id = @assignedProgram_ProgramDayItemMovieId


                                    DELETE  PDI 
                                    FROM AssignedProgram_ProgramDayItem AS pdi
                                    WHERE Id = @programDayId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                await sqlConn.ExecuteAsync(deleteString, new { assignedProgram_ProgramDayItemMovieId = assignedProgram_ProgramDayItemMovieId });
            }
        }
        public async Task AddMovie(int assignedProgram_ProgramDayId, int movieId, List<int> displayWeeks, int position)
        {

            var insertSql = @"
                                DECLARE @programDayItemId INT
      
                                INSERT INTO [AssignedProgram_ProgramDayItem] (AssignedProgram_ProgramDayId,  Position,ItemEnum)
                                VALUES (@assignedProgram_programDayid,@position,@itemType)

                                SELECT @programdayItemId =SCOPE_IDENTITY()

                                INSERT INTO [AssignedProgram_ProgramDayItemMovie] (MovieId,AssignedProgram_ProgramDayItemId)
                                VALUES (@movieId , @programdayItemId)

                                SELECT SCOPE_IDENTITY()
                            ";
            var dipslayWeeksInsert = @"
                                        INSERT INTO [AssignedProgram_MovieDisplayWeek] (AssignedProgram_ProgramDayItemMovieId,DisplayWeek)           
                                        VALUES(@programDayItemMovieId, @DisplayWeek)
                                    ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var insertedProgramDayItemMovieId = await sqlConn.ExecuteScalarAsync<int>(insertSql, new { position, itemType = (int)ProgramDayItemEnum.Video, movieId, assignedProgram_programDayid = assignedProgram_ProgramDayId });

                for (int i = 0; i < displayWeeks.Count; i++)
                {
                    await sqlConn.ExecuteScalarAsync<int>(dipslayWeeksInsert, new { programDayItemMovieId = insertedProgramDayItemMovieId, DisplayWeek = displayWeeks[i] });
                }
            }

        }
        public async Task DeleteMetric(int assignedProgram_ProgramDayItemMetricId)
        {
            var deleteString = @"
    
                                DECLARE @programDayId AS INT
    
                                SELECT @programDayId = pdi.Id
                                FROM AssignedProgram_ProgramDayItem AS pdi
                                INNER JOIN assignedProgram_programdayitemmetric AS pdim ON pdi.id = pdim.AssignedProgram_ProgramDayItemId
                                INNER JOIN AssignedProgram_MetricsDisplayWeek AS mdw ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.Id
                                WHERE pdim.Id = @assignedProgram_ProgramDayItemMetricId

                                DELETE mdw
                                FROM AssignedProgram_ProgramDayItem AS pdi
                                INNER JOIN assignedProgram_programdayitemmetric AS pdim ON pdi.id = pdim.AssignedProgram_ProgramDayItemId
                                INNER JOIN AssignedProgram_MetricsDisplayWeek AS mdw ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.Id
                                WHERE pdim.Id = @assignedProgram_ProgramDayItemMetricId

                                DELETE pdim
                                FROM AssignedProgram_ProgramDayItem AS pdi
                                INNER JOIN assignedProgram_programdayitemmetric AS pdim ON pdi.id = pdim.AssignedProgram_ProgramDayItemId
                                WHERE pdim.Id = @assignedProgram_ProgramDayItemMetricId

                                DELETE  PDI 
                                FROM AssignedProgram_ProgramDayItem AS pdi
                                WHERE Id = @programDayId
                                ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                await sqlConn.ExecuteAsync(deleteString, new { assignedProgram_ProgramDayItemMetricId = assignedProgram_ProgramDayItemMetricId });
            }
        }
        public async Task UpdateMetricDisplayWeeks(List<int> newDisplayWeeks, int AssignedProgram_ProgramDayItemMetricId)
        {

            var dipslayWeeksInsert = @"
                                        DECLARE @pdimId AS INT ;


                                        INSERT INTO [AssignedProgram_MetricsDisplayWeek] (AssignedProgram_ProgramDayItemMetricId,DisplayWeek)           
                                        VALUES(@AssignedProgram_ProgramDayItemMetricId, @DisplayWeek)
                                    ";


            var deleteScript = @"
                                DELETE  MDW
                                FROM AssignedProgram_ProgramDayItem AS pdi
                                INNER JOIN assignedProgram_programdayitemmetric AS pdim ON pdi.id = pdim.AssignedProgram_ProgramDayItemId
                                INNER JOIN AssignedProgram_MetricsDisplayWeek AS mdw ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.Id
                                WHERE pdim.Id = @AssignedProgram_ProgramDayItemMetricId
                                and mdw.DisplayWeek not in (@newDisplayWeeks)
                                 ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                await sqlConn.ExecuteAsync(deleteScript, new { AssignedProgram_ProgramDayItemMetricId = AssignedProgram_ProgramDayItemMetricId, newDisplayWeeks = newDisplayWeeks });

                for (int i = 0; i < newDisplayWeeks.Count; i++)
                {
                    await sqlConn.ExecuteScalarAsync<int>(dipslayWeeksInsert, new { AssignedProgram_ProgramDayItemMetricId = AssignedProgram_ProgramDayItemMetricId, DisplayWeek = newDisplayWeeks[i] });
                }
            }
        }
        public async Task AddMetric(int metricId, int assignedProgram_ProgramDayId, List<int> displayWeeks, int position)
        {

            var insertSql = @"
                                DECLARE @programDayItemId INT
                       
                                INSERT INTO [AssignedProgram_ProgramDayItem] (AssignedProgram_ProgramDayId,  Position,ItemEnum)
                                VALUES (@assignedProgram_programDayid,@position,@itemType)

                                SELECT @programdayItemId =SCOPE_IDENTITY()

                                INSERT INTO [AssignedProgram_ProgramDayItemMetric] (MetricId,AssignedProgram_ProgramDayItemId)
                                VALUES (@metricId , @programdayItemId)

                               SELECT SCOPE_IDENTITY()

                            ";
            var dipslayWeeksInsert = @"
                                        INSERT INTO [AssignedProgram_MetricsDisplayWeek] (AssignedProgram_ProgramDayItemMetricId,DisplayWeek)           
                                        VALUES(@programDayItemMetric, @DisplayWeek)
                                    ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var insertedProgramDayItemMetricId = await sqlConn.ExecuteScalarAsync<int>(insertSql, new { position, itemType = (int)ProgramDayItemEnum.Metric, metricId, assignedProgram_programDayid = assignedProgram_ProgramDayId });

                for (int i = 0; i < displayWeeks.Count; i++)
                {
                    await sqlConn.ExecuteScalarAsync<int>(dipslayWeeksInsert, new { programDayItemMetric = insertedProgramDayItemMetricId, DisplayWeek = displayWeeks[i] });
                }
            }

        }
        public async Task DeleteSurvey(int assignedProgram_programDayItemSurveyId)
        {
            var deleteString = @"
                            DECLARE @assignedProgram_programdayItemId AS INT 

                            DELETE cqs
                            FROM [AssignedProgram_CompletedQuestionScale] AS cqs
                            INNER JOIN [dbo].[AssignedProgram_ProgramDayItemSurvey] AS pdis ON  pdis.Id = cqs.AssignedProgram_ProgramDayItemSurveyId
                            WHERE pdis.id =  @assignedProgram_programdayItemSurvey

                            DELETE cqs
                            FROM [AssignedProgram_CompletedQuestionYesNo] AS cqs
                            INNER JOIN [dbo].[AssignedProgram_ProgramDayItemSurvey] AS pdis ON  pdis.Id = cqs.AssignedProgram_ProgramDayItemSurveyId
                            WHERE pdis.id =  @assignedProgram_programdayItemSurvey

                            DELETE cqs
                            FROM .[AssignedProgram_CompletedQuestionOpenEnded] AS cqs
                            INNER JOIN [dbo].[AssignedProgram_ProgramDayItemSurvey] AS pdis ON  pdis.Id = cqs.AssignedProgram_ProgramDayItemSurveyId
                            WHERE pdis.id =  @assignedProgram_programdayItemSurvey

                            SELECT @assignedProgram_programDayItemId = pdis.AssignedProgram_ProgramDayItemId
                            FROM [AssignedProgram_ProgramDayItemSurvey] as pdis

                            
                            DELETE FROM [dbo].[AssignedProgram_SurveyDisplayWeeks]
                            WHERE assignedProgram_ProgramDayItemSurveyId =  @assignedProgram_programdayItemSurvey 

                            SELECT @assignedProgram_programDayItemId = assignedProgram_programdayitemId
                            FROM [AssignedProgram_ProgramDayItemSurvey] 
                            WHERE id = @assignedProgram_programdayItemSurvey

                            DELETE FROM [AssignedProgram_ProgramDayItemSurvey] 
                            WHERE id = @assignedProgram_programdayItemSurvey 

                            DELETE FROM [dbo].[AssignedProgram_ProgramDayItem]
                            WHERE id = @assignedProgram_programDayItemId
                            ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                await sqlConn.ExecuteAsync(deleteString, new { assignedProgram_programdayItemSurvey = assignedProgram_programDayItemSurveyId });
            }

        }
        public async Task AddSurvey(int surveyId, List<int> displayWeeks, int position, int assignedProgram_programDayId)
        {
            var insertSql = @"
                                DECLARE @programDayItemId INT
      
                                INSERT INTO [AssignedProgram_ProgramDayItem] (Position,ItemEnum,assignedProgram_programDayId)
                                VALUES(@position,@itemType,@assignedProgram_programDayId)

                                SELECT @programdayItemId =SCOPE_IDENTITY()

                                INSERT INTO [AssignedProgram_ProgramDayItemSurvey] (SurveyId,AssignedProgram_ProgramDayItemId)
                                VALUES (@surveyId , @programdayItemId)

                               SELECT SCOPE_IDENTITY()

                            ";
            var dipslayWeeksInsert = @"
                                        INSERT INTO [AssignedProgram_SurveyDisplayWeeks] (AssignedProgram_ProgramDayItemSurveyId,DisplayWeek)           
                                        VALUES(@programDayItemSurvey, @DisplayWeek)
                                    ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var programDayItemSurvey = await sqlConn.ExecuteScalarAsync<int>(insertSql, new { assignedProgram_programDayId = assignedProgram_programDayId, position, itemType = (int)ProgramDayItemEnum.Survey, surveyId = surveyId });

                for (int i = 0; i < displayWeeks.Count; i++)
                {
                    await sqlConn.ExecuteScalarAsync<int>(dipslayWeeksInsert, new { programDayItemSurvey = programDayItemSurvey, DisplayWeek = displayWeeks[i] });
                }
            }
        }
        public async Task UpdateSurveyDisplayWeeks(List<int> newSurveyDisplayWeeks, int assignedProgram_programdayItemSurveyId, List<int> displayWeeksToDelete)
        {
            var deleteString = @"

                            DELETE cqs
                            FROM [AssignedProgram_CompletedQuestionScale] AS cqs
                            INNER JOIN [dbo].[AssignedProgram_ProgramDayItemSurvey] AS pdis ON  pdis.Id = cqs.AssignedProgram_ProgramDayItemSurveyId
                            WHERE pdis.id =  @assignedProgram_programdayItemSurvey
                            AND cqs.weekId IN (@weeksToDelete)

                            DELETE cqs
                            FROM [AssignedProgram_CompletedQuestionYesNo] AS cqs
                            INNER JOIN [dbo].[AssignedProgram_ProgramDayItemSurvey] AS pdis ON  pdis.Id = cqs.AssignedProgram_ProgramDayItemSurveyId
                            WHERE pdis.id =  @assignedProgram_programdayItemSurvey
                            AND cqs.weekId IN (@weeksToDelete)

                            DELETE cqs
                            FROM .[AssignedProgram_CompletedQuestionOpenEnded] AS cqs
                            INNER JOIN [dbo].[AssignedProgram_ProgramDayItemSurvey] AS pdis ON  pdis.Id = cqs.AssignedProgram_ProgramDayItemSurveyId
                            WHERE pdis.id =  @assignedProgram_programdayItemSurvey
                            AND cqs.weekId IN (@weeksToDelete)
                            
                            DELETE FROM [dbo].[AssignedProgram_SurveyDisplayWeeks]
                            WHERE assignedProgram_ProgramDayItemSurveyId =  @assignedProgram_programdayItemSurvey
                            AND displayWeek IN (@weeksToDelete)
                            ";



            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                await sqlConn.ExecuteAsync(deleteString, new { assignedProgram_programdayItemSurvey = assignedProgram_programdayItemSurveyId, weeksToDelete = displayWeeksToDelete });
            }
            var dipslayWeeksInsert = @"
                                        INSERT INTO [AssignedProgram_SurveyDisplayWeeks] (AssignedProgram_ProgramDayItemSurveyId,DisplayWeek)           
                                        VALUES(@assignedProgram_programdayItemSurveyId, @DisplayWeek)
                                    ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                for (int i = 0; i < newSurveyDisplayWeeks.Count; i++)
                {
                    await sqlConn.ExecuteScalarAsync<int>(dipslayWeeksInsert, new { assignedProgram_programdayItemSurveyId = assignedProgram_programdayItemSurveyId, DisplayWeek = newSurveyDisplayWeeks[i] });
                }
            }

        }
        public async Task UpdateSuperSetInWeek(string other, int superset_setId, int AssignedProgram_ProgramDayItemSuperSetWeekId, int position, int? sets,
            int? reps, double? percent, double? weight, int? minutes, int? seconds, string distance, bool? repsAchieved, int athleteId, int exerciseId)
        {
            var updateString = @"
                                     DECLARE @snapShotValue INT
                        DECLARE @nonSnapShotValue INT 
                        DECLARE @metricToEnter INT
                        ;WITH assignedProgramIdCTE(assignedProgramID) AS (
                        SELECT AssignedProgram_AssignedProgramId from Athletes where id = @athleteId
                        UNION ALL 
                        select AssignedProgram_ProgramId from [dbo].[AssignedProgram_AssignedProgramHistory] where athleteid = @athleteId
                        )
                        ,latestSnapShotMetricCTE ( lastValue) as
                        (
                        select TOP 1 mdw.Value
                        from assignedProgramIdCTE AS apiCTE
                        INNER JOIN AssignedProgram_ProgramDay AS pd ON apiCTE.assignedProgramID = pd.AssignedProgram_ProgramId
                        INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.AssignedProgram_ProgramDayId = pd.Id
                        INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdim ON pdim.AssignedProgram_ProgramDayItemId = pdi.Id
                        INNER JOIN [AssignedProgram_MetricsDisplayWeek] AS mdw ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.Id
                        INNER JOIN metrics AS m ON m.id = pdim.MetricId
                        INNER JOIN exercises AS e ON e.PercentMetricCalculationId = m.Id
                        WHERE e.id = @exerciseId AND mdw.[value] is not null
                        ORDER BY MDW.CompletedDate DESC)
                        ,latestpreSnapShotMetricCTE (lastValue) AS
                        (
                        SELECT TOP 1 [VALUE]
                        FROM (
                        SELECT [value], CompletedDate
                        FROM addedMetrics AS am
                        INNER JOIN Exercises AS e ON E.PercentMetricCalculationId = AM.MetricId
                        where  am.metricId = e.PercentMetricCalculationId AND am.athleteId = @athleteId
                        UNION ALL
                        SELECT [value], CompletedDate
                        FROM CompletedMetrics AS cm
                        INNER JOIN Exercises AS e ON E.PercentMetricCalculationId = CM.MetricId AND cm.athleteId = 1) AS allMetrics
                        ORDER BY completedDate DESC
                        )
                        select @snapShotValue = (SELECT lastValue FROM  latestSnapShotMetricCTE), @nonSnapShotValue =(SELECT lastValue from latestpreSnapShotMetricCTE)

                        IF (@snapShotValue IS NOT NULL)
	                        select @metricToEnter = @snapShotValue
                        ELSE
	                        select @metricToEnter = @nonSnapShotValue

                                UPDATE [dbo].[AssignedProgram_ProgramDayItemSuperSet_Set]
                                   SET [Sets] = @sets
                                      ,[Reps] = @reps
                                      ,[Percent] = @percent
                                      ,[Weight] = @weight
                                      ,[Minutes] = @minutes
                                      ,[Seconds] = @seconds
                                      ,[Distance] = @distance
                                      ,[RepsAchieved] = @repsAchieved
                                      ,[Other] = @other
                                      ,[Completed_Sets] = NULL
                                      ,[Completed_Reps] =  NULL
                                      ,[Completed_Weight] = NULL
                                      ,[Completed_RepsAchieved] = null
                                      ,[LastCompletedUpdateTime] = NULL
                                      ,[PercentMaxCalc] = FLOOR(ROUND(@metricToEnter * ((SELECT [percent] FROM exercises WHERE id = @exerciseId) * .01) , 0)) 
                                      ,[PercentMaxCalcSubPercent] = FLOOR(ROUND(@metricToEnter * ((SELECT [percent] FROM exercises WHERE id = @exerciseId) * .01) * (@percent * .01) , 0))
                                 WHERE id = @superset_setId AND position = @position AND  [AssignedProgram_ProgramDayItemSuperSetWeekId] = @AssignedProgram_ProgramDayItemSuperSetWeekId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                await sqlConn.ExecuteAsync(updateString,
                    new
                    {
                        superset_setId = superset_setId,
                        position = position,
                        AssignedProgram_ProgramDayItemSuperSetWeekId = AssignedProgram_ProgramDayItemSuperSetWeekId,
                        sets = sets,
                        reps = reps,
                        percent = percent,
                        weight = weight,
                        minutes = minutes,
                        seconds = seconds,
                        distance = distance,
                        repsAchieved = repsAchieved,
                        other = other,
                        exerciseId = exerciseId,
                        AthleteId = athleteId
                    });
            }

        }
        public async Task AddSuperSetInWeek(string other, int AssignedProgram_ProgramDayItemSuperSetWeekId, int position, int? sets, int? reps, double? percent, double? weight, int? minutes, int? seconds, string distance, bool? repsAchieved, int athleteId, int exerciseId)
        {
            var updateString = @"
         
                        DECLARE @snapShotValue INT
                        DECLARE @nonSnapShotValue INT 
                        DECLARE @metricToEnter INT
                        ;WITH assignedProgramIdCTE(assignedProgramID) AS (
                        SELECT AssignedProgram_AssignedProgramId from Athletes where id = @athleteId
                        UNION ALL 
                        select AssignedProgram_ProgramId from [dbo].[AssignedProgram_AssignedProgramHistory] where athleteid = @athleteId
                        )
                        ,latestSnapShotMetricCTE ( lastValue) as
                        (
                        select TOP 1 mdw.Value
                        from assignedProgramIdCTE AS apiCTE
                        INNER JOIN AssignedProgram_ProgramDay AS pd ON apiCTE.assignedProgramID = pd.AssignedProgram_ProgramId
                        INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.AssignedProgram_ProgramDayId = pd.Id
                        INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdim ON pdim.AssignedProgram_ProgramDayItemId = pdi.Id
                        INNER JOIN [AssignedProgram_MetricsDisplayWeek] AS mdw ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.Id
                        INNER JOIN metrics AS m ON m.id = pdim.MetricId
                        INNER JOIN exercises AS e ON e.PercentMetricCalculationId = m.Id
                        WHERE e.id = @exerciseId AND mdw.[value] is not null
                        ORDER BY MDW.CompletedDate DESC)
                        ,latestpreSnapShotMetricCTE (lastValue) AS
                        (
                        SELECT TOP 1 [VALUE]
                        FROM (
                        SELECT [value], CompletedDate
                        FROM addedMetrics AS am
                        INNER JOIN Exercises AS e ON E.PercentMetricCalculationId = AM.MetricId
                        where  am.metricId = e.PercentMetricCalculationId AND am.athleteId = @athleteId
                        UNION ALL
                        SELECT [value], CompletedDate
                        FROM CompletedMetrics AS cm
                        INNER JOIN Exercises AS e ON E.PercentMetricCalculationId = CM.MetricId AND cm.athleteId = 1) AS allMetrics
                        ORDER BY completedDate DESC
                        )
                        select @snapShotValue = (SELECT lastValue FROM  latestSnapShotMetricCTE), @nonSnapShotValue =(SELECT lastValue from latestpreSnapShotMetricCTE)

                        IF (@snapShotValue IS NOT NULL)
	                        select @metricToEnter = @snapShotValue
                        ELSE
	                        select @metricToEnter = @nonSnapShotValue


                               INSERT INTO [dbo].[AssignedProgram_ProgramDayItemSuperSet_Set]
                               ([Position]
                               ,[Sets]
                               ,[Reps]
                               ,[Percent]
                               ,[Weight]
                               ,[Minutes]
                               ,[Seconds]
                               ,[Distance]
                               ,[RepsAchieved]
                               ,[Other]
                               ,[AssignedProgram_ProgramDayItemSuperSetWeekId]
                               ,[PercentMaxCalc]
                               ,[PercentMaxCalcSubPercent])
                         VALUES
                               (@position
                               ,@sets
                               ,@reps
                               ,@percent
                               ,@weight
                               ,@minutes
                               ,@seconds
                               ,@distance
                               ,@repsAchieved
                               ,@other
                               ,@AssignedProgram_ProgramDayItemSuperSetWeekId
                                ,FLOOR(ROUND(@metricToEnter * ((SELECT [percent] FROM exercises WHERE id = @exerciseId) * .01) , 0))
                                ,FLOOR(ROUND(@metricToEnter * ((SELECT [percent] FROM exercises WHERE id = @exerciseId) * .01) * (@percent * .01) , 0))
                                                            ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                await sqlConn.ExecuteScalarAsync<int>(updateString,
                    new
                    {
                        position = position,
                        AssignedProgram_ProgramDayItemSuperSetWeekId = AssignedProgram_ProgramDayItemSuperSetWeekId,
                        sets = sets,
                        reps = reps,
                        percent = percent,
                        weight = weight,
                        minutes = minutes,
                        seconds = seconds,
                        distance = distance,
                        repsAchieved = repsAchieved,
                        other = other,
                        athleteId = athleteId,
                        exerciseId = exerciseId
                    });
            }

        }
        public async Task UpdateSuperSetOtherInWeek(string other, int superset_setId, int AssignedProgram_ProgramDayItemSuperSetWeekId, int position)
        {
            var updateString = @"
                                UPDATE [dbo].[AssignedProgram_ProgramDayItemSuperSet_Set]
                                   SET
                                      [Other] = @other
                                 WHERE id = @superset_setId AND position = @position AND  AssignedProgram_ProgramDayItemSuperSetWeekId = @AssignedProgram_ProgramDayItemSuperSetWeekId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                await sqlConn.ExecuteScalarAsync<int>(updateString,
                    new
                    {
                        superset_setId = superset_setId,
                        position = position,
                        AssignedProgram_ProgramDayItemSuperSetWeekId = AssignedProgram_ProgramDayItemSuperSetWeekId,

                        other = other
                    });
            }
        }
        public async Task DeleteSuperSetInWeek(int AssignedProgram_ProgramDayItemSuperSetWeekId, int position)
        {
            var updateString = @"
                                DELETE [dbo].[AssignedProgram_ProgramDayItemSuperSet_Set]
                                WHERE position = @position AND  AssignedProgram_ProgramDayItemSuperSetWeekId = @AssignedProgram_ProgramDayItemSuperSetWeekId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                await sqlConn.ExecuteScalarAsync<int>(updateString,
                    new
                    {
                        position = position,
                        AssignedProgram_ProgramDayItemSuperSetWeekId = AssignedProgram_ProgramDayItemSuperSetWeekId,
                    });
            }

        }
        public async Task UpdateRest(string rest, int assignedProgram_programDayItemSuperSetExerciseId)
        {
            var updateScript = @"UPDATE AssignedProgram_SuperSetExercise SET rest = @Rest WHERE id = @id";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                await sqlConn.ExecuteAsync(updateScript, new { Rest = rest, id = assignedProgram_programDayItemSuperSetExerciseId });
            }

        }
        public async Task DeleteSuperSetExercise(int superSetExerciseId)
        {

            var deleteScript = @"
                                DELETE s_s
                                FROM [dbo].[AssignedProgram_SuperSetExercise] AS sse 
                                INNER JOIN [dbo].[AssignedProgram_ProgramDayItemSuperSetWeek] AS w ON w.AssignedProgram_SuperSetExerciseId = sse.Id
                                INNER JOIN [dbo].[AssignedProgram_ProgramDayItemSuperSet_Set] AS s_s ON s_s.AssignedProgram_ProgramDayItemSuperSetWeekId = w.Id
                                WHERE sse.id =@sseId

                                DELETE w
                                FROM [dbo].[AssignedProgram_SuperSetExercise] AS sse 
                                INNER JOIN [dbo].[AssignedProgram_ProgramDayItemSuperSetWeek] AS w ON w.AssignedProgram_SuperSetExerciseId = sse.Id
                                WHERE sse.id =@sseId

                                DELETE sse
                                FROM [dbo].[AssignedProgram_SuperSetExercise] AS sse 
                                WHERE sse.id =@sseId

                                ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                await sqlConn.ExecuteScalarAsync<int>(deleteScript, new { sseId = superSetExerciseId });
            }
        }
        public async Task UpdatePositionsToIncludedAddedOne(int position, int assignedProgram_programDayId)
        {
            var updateSQL = @"UPDATE [dbo].[AssignedProgram_ProgramDayItem]
                            SET Position = q.position+1
                            FROM [dbo].[AssignedProgram_ProgramDayItem] AS q
                            WHERE q.Position >= @position AND q.AssignedProgram_ProgramDayId = @programDayId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                await sqlConn.ExecuteAsync(updateSQL, new { position = position, programDayId = assignedProgram_programDayId });
            }
        }
        public async Task UpdatePositionsToIncludedRemovedOne(int position, int assignedProgram_programDayId)
        {
            var updateSQL = @"UPDATE [dbo].[AssignedProgram_ProgramDayItem]
                            SET Position = q.position-1
                            FROM [dbo].[AssignedProgram_ProgramDayItem] AS q
                            WHERE q.Position >= @position AND q.AssignedProgram_ProgramDayId = @programDayId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                await sqlConn.ExecuteAsync(updateSQL, new { position = position, programDayId = assignedProgram_programDayId });
            }
        }
        public async Task<int> AddNewProgramDay(int position, int programId)
        {
            var insertScript = @" INSERT INTO assignedProgram_ProgramDay(Position ,assignedProgram_programId)
                                VALUES (@position,@programId);  SELECT SCOPE_IDENTITY();";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return await sqlConn.ExecuteScalarAsync<int>(insertScript, new { position = position, programId = programId });
            }

        }
        public async Task JustDeleteAssignedProgramDay(int programDayId)
        {
            var delete = @"DELETE FROM AssignedProgram_ProgramDay WHERE id = @id";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                await sqlConn.ExecuteAsync(delete, new { id = programDayId });
            }
        }
        public async Task DeleteProgramDayItemSuperSet(int assignedProgram_programdayItemSuperSetId)
        {

            var deleteScript = @"
       
                                DECLARE @assignedProgram_programDayItemId AS INT 

                                SELECT @assignedProgram_programDayItemId = assignedProgram_ProgramDayItemId
                                FROM assignedProgram_programDayItemSuperSet AS sse
                                WHERE id = @assignedProgram_programdayItemSuperSetId

                                DELETE FROM [dbo].[AssignedProgram_SuperSetNoteDisplayWeek]
                                WHERE AssignedProgram_SuperSetNoteId 
                                IN (
                                SELECT ssn.Id   FROM AssignedProgram_ProgramDayItem AS pdi
                                INNER JOIN AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.AssignedProgram_ProgramDayItemId = pdi.Id
                                INNER JOIN AssignedProgram_SuperSetNote AS ssn ON ssn.AssignedProgram_ProgramDayItemSuperSetId = pdiss.Id
                                WHERE pdi.id = @assignedProgram_programDayItemId)

                                DELETE FROM AssignedProgram_SuperSetNote
                                WHERE assignedProgram_ProgramDayItemSuperSetId
                                IN (
                                SELECT pdiss.Id  FROM AssignedProgram_ProgramDayItem AS pdi
                                INNER JOIN AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.assignedProgram_ProgramDayItemId = pdi.Id
                                WHERE pdi.id = @assignedProgram_programDayItemId)

                                DELETE FROM AssignedProgram_ProgramDayItemSuperSet_Set
                                WHERE assignedProgram_ProgramDayItemSuperSetWeekId  IN (
                                SELECT pdissw.id  
                                FROM AssignedProgram_ProgramDayItem AS pdi
                                INNER JOIN AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.AssignedProgram_ProgramDayItemId = pdi.Id
                                INNER JOIN AssignedProgram_SuperSetExercise  AS sse ON sse.AssignedProgram_ProgramDayItemSuperSetId = pdiss.Id
                                INNER JOIN AssignedProgram_ProgramDayItemSuperSetWeek AS pdissw ON pdissw.AssignedProgram_SuperSetExerciseId = sse.Id
                                WHERE  pdi.id = @assignedProgram_programDayItemId)


                                DELETE FROM AssignedProgram_ProgramDayItemSuperSetWeek
                                WHERE assignedProgram_SuperSetExerciseId  IN (
                                SELECT sse.Id  
                                FROM AssignedProgram_ProgramDayItem AS pdi
                                INNER JOIN AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.AssignedProgram_ProgramDayItemId = pdi.Id
                                INNER JOIN AssignedProgram_SuperSetExercise  AS sse ON sse.AssignedProgram_ProgramDayItemSuperSetId = pdiss.Id
                                WHERE pdi.id = @assignedProgram_programDayItemId)


                                DELETE FROM AssignedProgram_SuperSetExercise
                                WHERE assignedProgram_ProgramDayItemSuperSetId  IN (
                                SELECT pdiss.Id  
                                FROM AssignedProgram_ProgramDayItem AS pdi
                                INNER JOIN AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.AssignedProgram_ProgramDayItemId = pdi.Id
                                WHERE pdi.id = @assignedProgram_programDayItemId)

								 DELETE FROM AssignedProgram_ProgramDayItemSuperSet
								 WHERE id = @assignedProgram_programdayItemSuperSetId

                                DELETE FROM  AssignedProgram_ProgramDayItem 
                                WHERE id = @assignedProgram_programDayItemId
                                ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                await sqlConn.ExecuteScalarAsync<int>(deleteScript, new { assignedProgram_programdayItemSuperSetId = assignedProgram_programdayItemSuperSetId });
            }

        }
    }
}
