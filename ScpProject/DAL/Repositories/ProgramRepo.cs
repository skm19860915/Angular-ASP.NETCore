using System;
using m = Models;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using DAL.DTOs;
using System.Linq;
using Models.Metric;
using DAL.DTOs.Program;
using System.Data;
using System.Text;
using DAL.DTOs.AthleteAssignedPrograms;
using Models.Enums;
using DAL.CustomerExceptions;
using Models.Program;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IProgramRepo
    {
        DTOs.Program.Program GetSnapShotProgram(int assignedProgramId, Guid createdUserToken, int programDayId = 0);
        void AddMetricToProgram(int metricId, int programDayItemId, List<int> weeksToDisplay);
        void AddMovieToProgram(int movieId, int programDayItemId, List<int> weeksToDisplay);
        void AddNoteToProgram(string note, string name, int programDayItemId, List<int> weeksToDisplay);
        void AddSurveyToProgram(int SurveyId, int programDayItemId, List<int> weeksToDisplay);
        void AddWorkoutToProgram(List<m.Program.ProgramWeek> weeksToAdd, int programDayItemId, int programExerciseId, int workoutId);
        void Archive(int programId, Guid userToken);
        void CompleteAllAssignedWeightWorkouts(int athleteId, int programDayId, int assignedProgramId, int weekId);
        int CreateProgram(m.Program.Program targetProgram, Guid userToken);
        int CreateProgramDay(int programId, int positionId);
        int CreateProgramDayItem(int positionId, ProgramDayItemEnum programType, int programDayId);
        int DoesProgramExist(string programName, Guid createdUserToken);
        //welp this shit doesnt work with snapshots
        void DontFuckingDoThis_Delete_All_Information_About_A_Program(int programId);
        void DuplicateProgram(int programId, Guid createdUserToken);
        List<int> GetAllAthleteIdsForAssignedProgram(int assignedProgramId, Guid createdUserGuid);
        List<AthleteHomePagePastProgram> GetAllPastPrograms(int athleteId);
        List<int> GetAllProgramIdsWithAdvancedFeaturesTurnedOn(Guid createdUserToken);
        List<m.Program.Program> GetAllPrograms(Guid createdUserToken);
        List<ProgramWithTagsDTO> GetAllProgramTagMappings(Guid userToken);
        List<AssignedMetric> GetAssignedMetrics(int assignedProgramId, int athleteId, int programDayId = 0);
        List<AssignedNote> GetAssignedNotes(int assignedProgramid, int programDayId);
        DTOs.AthleteAssignedPrograms.AssignedProgram GetAssignedProgram(m.Athlete.Athlete targetAthlete, int assignedProgramId = 0, int programDayId = 0);
        int GetAssignedProgramId(Guid athleteToken);
        List<AthleteAssignedQuestions> GetAssignedQuestions(int assignedProgramId, int athleteId, int surveyId, int programDayItemSurveyId = 0, int programDayId = 0);
        List<AssignedSetRep> GetAssignedSets(int assignedProgramId, int athleteId, int programDayId = 0);
        List<AssignedSuperSet> GetAssignedSuperSets(int programDayId, int assignedProgramId);
        List<AssignedSuperSetExercise> GetAssignedSuperSetsExericses(int programDayId, int assignedProgramId);
        List<AssignedSuperSetSetRep> GetAssignedSuperSet_SetsAndReps(int assignedProgramId, int athleteId, int programDayId = 0);
        List<AssignedProgramDaySurvey> GetAssignedSurveys(int assignedProgramId, int athleteId, int programDayId = 0);
        List<AssignedVideo> GetAssignedVideos(int assignedProgramId, int athleteId, int programDayId = 0);
        void GetDisplayWeeksForAssignedProgramSurvey(List<AssignedProgramDaySurvey> targetSurveys);
        void GetDisplayWeeksForAssignedSuperSetNote(List<AssignedSuperSetNote> targetSuperSetNotes);
        void GetDisplayWeeksForProgramMetrics(List<ProgramDayMetrics> targetMetrics);
        void GetDisplayWeeksForProgramMovies(List<ProgramDayMovie> targetMovies);
        void GetDisplayWeeksForProgramNotes(List<ProgramDayNotes> targetNotes);
        void GetDisplayWeeksForProgramSurvey(List<ProgramDaySurvey> targetSurveys);
        void GetDisplayWeeksForSuperSetNotes(List<ProgramDaySuperSet> targetSuperSets);
        int GetLatestAssignedProgramID(int programId);
        DTOs.Program.Program GetProgram(int programId, Guid createdUserToken, int programDayId = 0);
        List<ProgramDaySuperSet> GetProgramDaySuperSetFromDataReader(IDataReader reader);
        int InsertSuperSet(int programDayItemId, string title);
        int InsertSuperSetExercise(m.Program.SuperSetExercise ssExercises);
        int InsertSuperSetNotes(string note, List<int> displayWeeks, int position, int programDayItemSuperSetId);
        int InsertSuperSetWeek(m.Program.ProgramDayItemSuperSetWeek ssWeek);
        void InsertSuperSet_sets(List<m.Program.ProgramDayItemSuperSet_Set> sets, int ProgramDayItemSuperSetWeekId);
        void MarkDayAsCompleted(int athleteId, int programDayId, int assignedProgramId, int weekNumber);
        List<int> ProgramDays(int programId);
        List<AssignedSuperSetNote> SuperSetNotes(int assignedProgramId, int athleteId, int programDayId = 0);
        void UnArchive(int programId, Guid userToken);
        /// <summary>
        /// Returns tuple(int,int) item1 = athleteId, item2 is snaphostId
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="athleteId"></param>
        /// <param name="createdUserToken"></param>
        /// <returns></returns>
        Task<Tuple<int, int>> CreateProgramSnapShot(int programId, int athleteId, Guid createdUserToken);
        //i fucked up, I dont think we need the athleteId. We will go back through and clear out this requirement.
        DTOs.AthleteAssignedPrograms.AssignedProgram GetAssignedProgramSnapShot(int assignedProgramId, Guid createdUserToken, int athleteId, int programDayId = 0);
        Task AddSuperSet_SetsToAssignedProgram(List<Models.Program.ProgramDayItemSuperSet_Set> sets, int ProgramDayItemSuperSetWeekId, int exerciseId, int athleteId);
        Task CreateSuperSetsforSnapshots(ProgramDaySuperSet target, int assignedProgram_ProgramDayId, int athleteId);
        Task AddSuperSetExerciseForSnapShot(ProgramDaySuperSet_Exercise e, int supserSetId, int athleteId);
    }

    public class ProgramRepo : IProgramRepo
    {
        private PlayerSnapShotRepository _snapShotRepo { get; set; }
        private SurveyRepo _surveyRepo { get; set; }
        private ExerciseRepo _exerciseRepo { get; set; }
        private WorkoutRepo _workoutRepo { get; set; }
        private string ConnectionString { get; set; }
        public ProgramRepo(string connectionString)
        {
            ConnectionString = connectionString;
            _surveyRepo = new SurveyRepo(connectionString);
            _exerciseRepo = new ExerciseRepo(connectionString);
            _workoutRepo = new WorkoutRepo(connectionString);
            _snapShotRepo = new PlayerSnapShotRepository(connectionString);
        }
        public void DuplicateProgram(int programId, Guid createdUserToken)
        {
            var ran = new Random().Next(1000000);
            var dupeName = $"-copy {ran}";
            var dupString = $@"DECLARE @OriginalProgramID INT = {programId}
                DECLARE @NewProgramID INT 

                DECLARE @NewProgramDays Table (position int, insertedId int, oldId int)
                DECLARE @NewProgramDayItems TABLE (oldId int, [newId] int,newProgramDayId int,oldProgramDayId int, position int,itemenum int)
                DECLARE @NewProgramDayItemExercises TABLE (oldId INT, [newId] INT, exerciseId INT, workoutId INT, oldProgramDayItemId INT, newProgramDayItemId INT)
                DECLARE @NewProgramDayExercisesWeeks TABLE (oldId INT, [newID] INT , position INT, oldProgramDayItemExerciseId INT, newProgramDayItemExerciseId INT)
                DECLARE @NewProgramDayItemSuperSets TABLE (oldId INT, [newID] INT, oldProgramDayItemId INT, newProgramDayItemId INT)
                DECLARE @NewProgramDayItemSuperSetExercise TABLE(oldId INT, [newID] INT, oldProgramDayItemSuperSetId INT, newProgramDayItemSuperSetId INT, position INT, exerciseId INT, Rest NVARCHAR(MAX), showWeight bit)
                DECLARE @NewProgramItemSuperSetWeeks TABLE (oldId INT, [newId] INT, position INT, oldSuperSetExerciseID INT, newSuperSetExerciseID INT)
                DECLARE @NewProgramItemSuperSetNotes TABLE (oldId INT, [newID] INT, position INT, note varchar(max))

                --dupe programs
                INSERT INTO PROGRAMS ([name],CreatedUserId,WeekCount,IsDeleted,CanModify,OrganizationId)
                SELECT [name] + '{dupeName}',CreatedUserId, WeekCount,0, 1,OrganizationId
                FROM Programs where Id = @OriginalProgramID

                --save new program id
                SELECT @NewProgramID = SCOPE_IDENTITY()

                -- dupe program days
                INSERT INTO programDays  (position,programId)
                OUTPUT  INSERTED.[Position],INSERTED.[Id] INTO  @newProgramDays(position,InsertedId)
                SELECT position, @newProgramId FROM programDays where programid = @OriginalProgramID

                -- update the new programdays to match the old ProgramId to NewProgramId
                UPDATE npd
                SET oldId = pd.Id 
                FROM @NewProgramDays as npd
                INNER JOIN ProgramDays AS pd ON pd.Position = npd.position
                WHERE pd.ProgramId = @originalProgramId

                --dupe program dayitems
                INSERT INTO ProgramDayItems (ProgramDayId, Position,ItemEnum)
                OUTPUT INSERTED.Id ,INSERTED.Position, INSERTED.ProgramDayId, INSERTED.ItemEnum INTO @NewProgramDayItems([newId],position,newProgramDayId, itemenum)
                SELECT npd.InsertedId, pdi.position,pdi.itemenum
                FROM @NewProgramDays AS npd 
                INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = npd.oldId

                --update dupped programdayItems tables with old data to have match it up
                UPDATE npdI
                SET NPDI.oldId  = pdi.Id , npdi.oldProgramDayId = pdi.ProgramDayId 
                FROM @NewProgramDayItems as npdi
                INNER JOIN ProgramDayItems AS pdI ON pdi.Position = npdi.position AND pdi.ItemEnum = npdi.ItemEnum
                INNER JOIN @NewProgramDays AS npd ON PDI.ProgramDayId = NPD.oldId AND npdi.newProgramDayId = npd.insertedId

                --dupe metrics
                INSERT INTO programDayItemMetrics (metricId,ProgramDayItemId)
                SELECT pdim.MetricId , npdi.[newId]
                FROM programDayItemMetrics AS pdim
                INNER JOIN @NewProgramDayItems AS npdi ON npdi.oldId = pdim.ProgramDayItemId AND NPDI.itemenum = {(int)ProgramDayItemEnum.Metric}

                --dupe metric displayweeks
                INSERT INTO metricDisplayWeeks (displayWeek,ProgramDayItemMetricId)
                SELECT mdw.DisplayWeek ,pdim2.Id
                FROM MetricDisplayWeeks AS mdw 
                INNER JOIN ProgramDayItemMetrics AS pdim ON pdim.id = mdw.ProgramDayItemMetricId
                INNER JOIN ProgramDayItems AS pdi ON pdi.Id = pdim.ProgramDayItemId 
                INNER JOIN  @NewProgramDayItems AS npdi ON pdim.ProgramDayItemId  = npdi.oldId AND npdi.itemenum = {(int)ProgramDayItemEnum.Metric}
                INNER JOIN programdayItemMEtrics AS pdim2 on pdim2.ProgramDayItemId = npdi.[newid]

                ---dupe notes
                INSERT INTO ProgramDayItemNotes ([name],[note],programdayItemId)
                SELECT [name],[note], npdi.[newId] bob 
                FROM ProgramDayItemNotes as pdin
                INNER JOIN @NewProgramDayItems AS npdi ON npdi.oldId = pdin.ProgramDayItemId AND NPDI.itemenum = {(int)ProgramDayItemEnum.Note} 
               
                --dupe note displayweeks
                INSERT INTO NoteDisplayWeeks (displayWeek,ProgramDayItemNoteId)
                SELECT mdw.DisplayWeek ,pdim2.Id
                FROM NoteDisplayWeeks AS mdw 
                INNER JOIN ProgramDayItemNotes AS pdim ON pdim.id = mdw.ProgramDayItemNoteId
                INNER JOIN ProgramDayItems AS pdi ON pdi.Id = pdim.ProgramDayItemId 
                INNER JOIN  @NewProgramDayItems AS npdi ON pdim.ProgramDayItemId  = npdi.oldId AND npdi.itemenum = {(int)ProgramDayItemEnum.Note}
                INNER JOIN programdayItemNotes AS pdim2 on pdim2.ProgramDayItemId = npdi.[newid]

                --dupe videos 
                INSERT INTO programdayitemmovies (MovieId,ProgramDayItemId)
                SELECT pdim.MovieId , npdi.[newId]
                FROM programdayitemmovies AS pdim
                INNER JOIN @NewProgramDayItems AS npdi ON npdi.oldId = pdim.ProgramDayItemId AND NPDI.itemenum = {(int)ProgramDayItemEnum.Video}

                --dupe video displayWeeks
                INSERT INTO moviedisplayWeeks (displayWeek,ProgramDayItemMovieId)
                SELECT mdw.DisplayWeek ,pdim2.Id
                FROM moviedisplayWeeks AS mdw 
                INNER JOIN programdayitemmovies AS pdim ON pdim.id = mdw.ProgramDayItemMovieId
                INNER JOIN ProgramDayItems AS pdi ON pdi.Id = pdim.ProgramDayItemId 
                INNER JOIN  @NewProgramDayItems AS npdi ON pdim.ProgramDayItemId  = npdi.oldId AND npdi.itemenum = {(int)ProgramDayItemEnum.Video}
                INNER JOIN programdayitemmovies AS pdim2 on pdim2.ProgramDayItemId = npdi.[newid]

                --dupExerciseItems
                INSERT INTO ProgramDayItemExercises (exerciseId,workoutId,programDayItemId)
                OUTPUT INSERTED.Id , inserted.ExerciseId, inserted.WorkoutId,inserted.ProgramDayItemId INTO @NewProgramDayItemExercises ([newId],exerciseId,workoutId, newProgramDayItemId)
                SELECT pdie.ExerciseId,pdie.WorkoutId,npdi.[newId]
                FROM ProgramDayItemExercises AS pdie
                INNER JOIN @NewProgramDayItems AS npdi ON pdie.ProgramDayItemId = npdi.oldId AND npdi.itemenum = {(int)ProgramDayItemEnum.Workout}

                --UPDATE dupped programDayItemExercises
                UPDATE npdie
                SET npdie.oldId = pdie.Id, npdie.oldProgramDayItemId = pdie.ProgramDayItemId
                FROM @NewProgramDayItemExercises AS npdie
                INNER JOIN @NewProgramDayItems AS npdi ON npdi.[newId] = npdie.newProgramDayItemId
                INNER JOIN ProgramDayItemExercises AS pdie ON pdie.ProgramDayItemId = npdi.oldId 

                -- dupeProgramWeeks
                INSERT INTO programWeeks (position, ProgramDayItemExerciseId)
                OUTPUT inserted.Id, inserted.Position,inserted.ProgramDayItemExerciseId INTO @NewProgramDayExercisesWeeks([newID],position,newProgramDayItemExerciseId)
                SELECT pw.position,npdie.[newId]
                FROM ProgramWeeks AS pw
                INNER JOIN @NewProgramDayItemExercises AS npdie ON pw.ProgramDayItemExerciseId = npdie.oldId

                --update dupped weeks
                UPDATE npdiew
                SET npdiew.oldId = pw.Id, npdiew.oldProgramDayItemExerciseId = pw.ProgramDayItemExerciseId
                FROM @NewProgramDayExercisesWeeks AS npdiew
                INNER JOIN @NewProgramDayItemExercises AS npdie ON npdie.[newId]  = npdiew.newProgramDayItemExerciseId 
                INNER JOIN @NewProgramDayItems AS npdi ON npdi.oldId = npdie.oldProgramDayItemId
                INNER JOIN ProgramWeeks AS pw ON pw.ProgramDayItemExerciseId = npdie.oldId  AND npdiew.position = pw.Position

                --dupe sets
                --dont delete this, need it for backwards compatablity to people who used sets before supersets
                INSERT INTO programSets(position,[sets],[reps],[percent],[weight],parentprogramweekID)
                SELECT ps.position,ps.[sets],ps.[reps],ps.[percent],ps.[weight], npdew.[newID]
                FROM @NewProgramDayExercisesWeeks AS npdew
                INNER JOIN ProgramSets AS ps ON ps.ParentProgramWeekId = npdew.oldId
            
                --dupe SUPERSETS
                INSERT INTO ProgramDayItemSuperSets (programDayItemId,SuperSetDisplayTitle)
                OUTPUT inserted.Id, inserted.ProgramDayItemId INTO @NewProgramDayItemSuperSets ([newId],[newProgramDayItemId])
                SELECT npdi.[newId], pdiss.SuperSetDisplayTitle 
                FROM @NewProgramDayItems as npdi
                INNER JOIN ProgramDayItemSuperSets AS pdiss ON pdiss.ProgramDayItemId = npdi.oldId 
                
                --update duped SUPERSETS
                UPDATE @NewProgramDayItemSuperSets
                SET  oldId = pdiss.Id, oldProgramDayItemId = pdiss.ProgramDayItemId
                FROM  ProgramDayItemSuperSets AS pdiss 
                INNER JOIN @NewProgramDayItems AS npdi ON pdiss.ProgramDayItemId = npdi.oldId
                INNER JOIN @NewProgramDayItemSuperSets AS npdiss ON npdiss.newProgramDayItemId = npdi.[newId]

                -- dup SUPERSET NOTES
                INSERT INTO supersetnotes ( Note,ProgramDayItemSuperSetId, position)
                OUTPUT inserted.Id, inserted.position, inserted.Note INTO @NewProgramItemSuperSetNotes (newId, position,note)
                SELECT ssn.note,npdiss.[newId],ssn.position
                FROM @NewProgramDayItemSuperSets AS npdiss
                INNER JOIN superSetNotes AS ssn ON npdiss.oldid = ssn.ProgramDayItemSuperSetId


                --update SUPERSET NOTES DISPLAYWEEK
                UPDATE @NewProgramItemSuperSetNotes 
                SET oldId = ssn.id
                FROM supersetnotes AS ssn
                INNER JOIN @NewProgramItemSuperSetNotes AS npissn ON npissn.note = ssn.note AND npissn.position = ssn.position
                INNER JOIN @NewProgramDayItemSuperSets AS npdiss ON npdiss.oldid = ssn.programdayitemsupersetid

                --dup SUPERSET NOTE DISPLAYWEEKS
                INSERT INTO SuperSetNoteDisplayWeeks (SuperSetNoteId, displayWeek)
                SELECT [newId],displayWeek
                FROM superSetNoteDisplayWeeks AS ssndw 
                INNER JOIN @NewProgramItemSuperSetNotes AS npdissn ON ssndw.supersetnoteid = npdissn.oldid

                --dupSuperSetExercises
                INSERT INTO SuperSetExercises(ProgramDayItemSuperSetId,Position,ExerciseId,Rest, ShowWeight)
                OUTPUT inserted.Id , inserted.ExerciseId, inserted.Position, inserted.ProgramDayItemSuperSetId, inserted.Rest, inserted.ShowWeight INTO @NewProgramDayItemSuperSetExercise([newId], exerciseId,position,newProgramDayItemSuperSetId,Rest,ShowWeight)
                SELECT npdiss.[newID], sse.Position,sse.ExerciseId,sse.Rest, sse.ShowWeight
                FROM @NewProgramDayItemSuperSets AS npdiss
                INNER JOIN SuperSetExercises AS sse ON npdiss.oldid = sse.ProgramDayItemSuperSetId

                --update superSetExercises
                UPDATE @NewProgramDayItemSuperSetExercise
                SET oldId =sse.id, oldProgramDayItemSuperSetId = sse.ProgramDayItemSuperSetId
                FROM  SuperSetExercises AS sse 
                INNER JOIN @NewProgramDayItemSuperSets AS npdiss ON npdiss.oldid = sse.ProgramDayItemSuperSetId	
                INNER JOIN @NewProgramDayItemSuperSetExercise AS npdisse ON npdisse.position = sse.Position AND npdisse.exerciseId = sse.ExerciseId AND npdisse.newProgramDayItemSuperSetId = npdiss.[newID]

                INSERT INTO ProgramDayItemSuperSetWeeks (position,SuperSetExerciseId)
                OUTPUT  inserted.id , inserted.Position
n, inserted.SuperSetExerciseId  INTO @NewProgramItemSuperSetWeeks([newId],position,newSuperSetExerciseId)
                SELECT pdissw.position, npdisse.[newID]
                FROM @NewProgramDayItemSuperSetExercise AS npdisse
                INNER JOIN ProgramDayItemSuperSetWeeks AS pdissw ON npdisse.oldId = pdissw.SuperSetExerciseId

                UPDATE NPISSW
                SET oldId =pdissw.id, oldSuperSetExerciseID  = pdissw.SuperSetExerciseId 
                FROM  ProgramDayItemSuperSetWeeks AS pdissw
                INNER JOIN @NewProgramDayItemSuperSetExercise AS npdisse ON npdisse.oldId = pdissw.SuperSetExerciseId
                INNER JOIN @NewProgramItemSuperSetWeeks AS npissw ON npissw.newSuperSetExerciseID = npdisse.[newid] AND npissw.position = pdissw.position

                INSERT INTO ProgramDayItemSuperSet_Set( position,[sets],[reps],[percent],[weight], ProgramDayItemSuperSetWeekId, [Minutes],[Seconds],[Distance],[Other])
                SELECT pdiss_s.position,pdiss_s.[sets],pdiss_s.[reps],pdiss_s.[percent],pdiss_s.[weight], npissw.[newId],pdiss_s.[Minutes],pdiss_s.[Seconds],pdiss_s.[Distance],pdiss_s.[Other]
                FROM ProgramDayItemSuperSet_Set AS pdiss_s
                INNER JOIN @NewProgramItemSuperSetWeeks AS npissw ON pdiss_s.ProgramDayItemSuperSetWeekId = npissw.oldId

                INSERT INTO ProgramDayItemSurveys (SurveyId, ProgramDayItemId)
                SELECT pdis.SurveyId,npdi.[newId]
                FROM ProgramDayItemSurveys AS pdis
                INNER JOIN @NewProgramDayItems AS npdi ON npdi.oldId = pdis.ProgramDayItemId

                INSERT INTO SurveyDisplayWeeks (DisplayWeek, ProgramDayItemSurveyId)
                SELECT svd.DisplayWeek , pdis2.Id
                FROM ProgramDayItemSurveys AS pdis
                INNER JOIN SurveyDisplayWeeks AS svd ON svd.ProgramDayItemSurveyId = pdis.id
                INNER JOIN @NewProgramDayItems AS npdi ON npdi.oldId = pdis.ProgramDayItemId
                INNER JOIN ProgramDayItemSurveys AS pdis2 ON pdis2.ProgramDayItemId = npdi.[newid]";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(dupString, new { Token = createdUserToken, Id = programId });
            }
        }
        public void UnArchive(int programId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Programs]
               SET IsDeleted = 0
              WHERE Id = @Id AND organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = programId });
            }
        }
        public void Archive(int programId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Programs]
               SET IsDeleted = 1
              WHERE Id = @Id AND organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = programId });
            }
        }

        public int CreateProgram(m.Program.Program targetProgram, Guid userToken)
        {
            var createString = $@" INSERT INTO dbo.Programs 
                                 ([Name],[CreatedUserId], [WeekCount], IsDeleted, CanModify,OrganizationId)
                                 VALUES 
                                  (@ProgramName,({ConstantSqlStrings.GetUserIdFromToken}), @weekCount,0,1,({ConstantSqlStrings.GetOrganizationIdByToken})); SELECT SCOPE_IDENTITY();  ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                try
                {
                    return int.Parse(sqlConn.ExecuteScalar(createString, new { Token = userToken, ProgramName = targetProgram.Name, WeekCount = targetProgram.WeekCount }).ToString());
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 2601) //duplicate key insert
                    {
                        throw new DuplicateKeyException();
                    }
                    throw;
                }
            }
        }
        public int CreateProgramDayItem(int positionId, Models.Enums.ProgramDayItemEnum programType, int programDayId)
        {
            var createString = @" INSERT INTO dbo.ProgramDayItems 
                                 ([Position],[ProgramDayId],[ItemEnum])
                                 VALUES 
                                  (@position, @ProgramDayId,@ItemType); SELECT SCOPE_IDENTITY();  ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return int.Parse(sqlConn.ExecuteScalar(createString, new { Position = positionId, ItemType = ((int)programType), ProgramDayId = programDayId }).ToString());
            }
        }

        public int CreateProgramDay(int programId, int positionId)
        {
            var createString = @" INSERT INTO dbo.ProgramDays 
                                 ([ProgramId],[Position])
                                 VALUES 
                                  (@ProgramId,@position); SELECT SCOPE_IDENTITY();  ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return int.Parse(sqlConn.ExecuteScalar(createString, new { ProgramId = programId, Position = positionId }).ToString());
            }
        }

        public void AddMetricToProgram(int metricId, int programDayItemId, List<int> weeksToDisplay)
        {
            var createString = @" INSERT INTO dbo.ProgramDayItemMetrics 
                                 ([MetricId],[ProgramDayItemId])
                                 VALUES 
                                  (@MetricId,@ProgramDayItemId); SELECT SCOPE_IDENTITY();";

            var insertDisplayWeeks = new StringBuilder(@" INSERT INTO[dbo].[MetricDisplayWeeks] 
                                        ([ProgramDayItemMetricId]
                                        ,[DisplayWeek]) 
                                     VALUES ");



            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var programDayItemMetricId = sqlConn.ExecuteScalar<int>(createString, new { MetricId = metricId, ProgramDayItemId = programDayItemId });
                weeksToDisplay.ForEach(x => insertDisplayWeeks.Append($"({programDayItemMetricId},{x}),"));
                sqlConn.Execute(insertDisplayWeeks.ToString().TrimEnd(','));
            }
        }
        public void AddMovieToProgram(int movieId, int programDayItemId, List<int> weeksToDisplay)
        {
            var createString = @" INSERT INTO dbo.ProgramDayItemMovies
                                 ([MovieId],[ProgramDayItemId])
                                 VALUES 
                                  (@MovieId,@ProgramDayItemId); SELECT SCOPE_IDENTITY();";

            var insertDisplayWeeks = new StringBuilder(@" INSERT INTO[dbo].[MovieDisplayWeeks] 
                                        ([ProgramDayItemMovieId]
                                        ,[DisplayWeek]) 
                                     VALUES ");



            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var programDayItemMovieId = sqlConn.ExecuteScalar<int>(createString, new { MovieId = movieId, ProgramDayItemId = programDayItemId });
                weeksToDisplay.ForEach(x => insertDisplayWeeks.Append($"({programDayItemMovieId},{x}),"));
                sqlConn.Execute(insertDisplayWeeks.ToString().TrimEnd(','));
            }
        }
        public void AddSurveyToProgram(int SurveyId, int programDayItemId, List<int> weeksToDisplay)
        {
            var createString = @" INSERT INTO dbo.ProgramDayItemSurveys
                                 ([SurveyId],[ProgramDayItemId])
                                 VALUES 
                                  (@SurveyId,@ProgramDayItemId); SELECT SCOPE_IDENTITY();";

            var insertDisplayWeeks = new StringBuilder(@" INSERT INTO[dbo].[SurveyDisplayWeeks] 
                                        ([ProgramDayItemSurveyId] 
                                        ,[DisplayWeek]) 
                                     VALUES ");


            var markQuestionsAsCannotModify = $@"UPDATE q
                                                set CanModify = 1
                                                FROM SurveysToQuestions AS sq
                                                INNER JOIN questions AS q on sq.QuestionId = q.Id
                                                where SurveyId = @surveyId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var programDayItemSurveyId = sqlConn.ExecuteScalar<int>(createString, new { SurveyId = SurveyId, ProgramDayItemId = programDayItemId });
                sqlConn.Execute(markQuestionsAsCannotModify, new { SurveyId = SurveyId });
                weeksToDisplay.ForEach(x => insertDisplayWeeks.Append($" ({programDayItemSurveyId},{x}),"));
                sqlConn.Execute(insertDisplayWeeks.ToString().TrimEnd(','));
            }
        }
        public void AddNoteToProgram(string note, string name, int programDayItemId, List<int> weeksToDisplay)
        {
            var createString = @" INSERT INTO dbo.ProgramDayItemNotes
                                 ([Name],[Note],[ProgramDayItemId])
                                 VALUES 
                                  (@Name,@Note, @ProgramDayItemId); SELECT SCOPE_IDENTITY();  ";

            var insertDisplayWeeks = @" INSERT INTO[dbo].[NoteDisplayWeeks] 
                                        ([ProgramDayItemNoteId],[DisplayWeek]) 
                                         VALUES (@ProgramDayItemNoteId, @WeekToDisplay)";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var ProgramDayItemNoteId = sqlConn.ExecuteScalar<int>(createString, new { Name = name, Note = note, ProgramDayItemId = programDayItemId });
                weeksToDisplay.ForEach(x => sqlConn.Execute(insertDisplayWeeks, new { ProgramDayItemNoteId = ProgramDayItemNoteId, WeekToDisplay = x }));
            }
        }
        public void AddWorkoutToProgram(List<m.Program.ProgramWeek> weeksToAdd, int programDayItemId, int programExerciseId, int workoutId)
        {
            var insertProgramDayItemExercise = "INSERT INTO programDayItemExercises (exerciseID,ProgramDayItemId, WorkoutId) VALUES (@ExerciseId, @ProgramDayItemId, @WorkoutId); SELECT SCOPE_IDENTITY()";
            var insertWeek = " INSERT INTO dbo.ProgramWeeks (Position,ProgramDayItemExerciseId) VALUES (@Position, @ItemExerciseId);SELECT SCOPE_IDENTITY(); ";

            var insertSets = " ;INSERT INTO dbo.ProgramSets "
             + " ([Position], "
             + " [Sets], "
             + " [Reps], "
             + " [Percent], "
             + " [Weight], "
             + " ParentProgramWeekId) "
             + " VALUES "
             + " (@Position, "
             + " @Sets, "
             + " @Reps,"
             + " @Percent, "
             + " @Weight, "
             + " @ParentProgramWeekId) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var pdieId = int.Parse(sqlConn.ExecuteScalar(insertProgramDayItemExercise, new { ExerciseId = programExerciseId, ProgramDayItemId = programDayItemId, WorkoutId = workoutId }).ToString());
                foreach (var week in weeksToAdd)
                {

                    var ParentProgramWeekId = int.Parse(sqlConn.ExecuteScalar(insertWeek, new { Position = week.Position, ItemExerciseId = pdieId }).ToString());
                    foreach (var set in week.SetsAndReps)
                    {
                        if (set.Percent == 0 && set.Reps == 0 && set.Sets == 0 && set.Weight == 0) continue;//do not save empty sets/reps
                        sqlConn.Execute(insertSets, new { Position = set.Position, Sets = set.Sets, Reps = set.Reps, Percent = set.Percent, Weight = set.Weight, ParentProgramWeekId = ParentProgramWeekId });
                    }
                }
            }
        }
        private List<Models.Program.ProgramWeek> GetAllProgramWeeksForAProgarmDayItemExercise(int programDayItemExerciseId)
        {
            var programSetsQuery = " SELECT ps.Position,[Sets],[Reps],[Percent],[Weight],ParentProgramWeekId, pw.ProgramDayItemExerciseId "
                                   + " from ProgramSets as ps "
                                   + " inner join ProgramWeeks as pw ON  ps.ParentProgramWeekId = pw.id "
                                   + " WHERE ProgramDayItemExerciseId = @ProgramDayItemExerciseId";

            var programWeeksQuery = "select Id, Position, ProgramDayItemExerciseId  from ProgramWeeks WHERE ProgramDayItemExerciseId = @ProgramDayItemExerciseId";
            var programSets = new List<Models.Program.ProgramSet>();
            var programWeeks = new List<Models.Program.ProgramWeek>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                programSets = sqlConn.Query<Models.Program.ProgramSet>(programSetsQuery, new { ProgramDayItemExerciseId = programDayItemExerciseId }).ToList();
                programWeeks = sqlConn.Query<Models.Program.ProgramWeek>(programWeeksQuery, new { ProgramDayItemExerciseId = programDayItemExerciseId }).ToList();
            }

            programWeeks.ForEach(x =>
            {
                x.SetsAndReps = programSets.Where(y => y.ParentProgramWeekId == x.Id).ToList();
            });
            return programWeeks;
        }
        public List<int> ProgramDays(int programId)
        {
            var daysQuery = $" SELECT Id FROM ProgramDays WHERE programId = {programId} ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<int>(daysQuery).ToList();
            }
        }

        public int GetAssignedProgramId(Guid athleteToken)
        {
            var programQuery = " SELECT p.Id FROM dbo.Programs  AS P "
                            + " INNER JOIN assignedPrograms AS ap ON ap.programId = p.Id "
                            + " INNER JOIN athletes AS a ON a.AssignedProgramId = ap.Id"
                            + $" WHERE a.AthleteUserId = ({ConstantSqlStrings.GetUserIdFromToken}) ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(programQuery, new { Token = athleteToken });
            }
        }

        public DTOs.AthleteAssignedPrograms.AssignedProgram GetAssignedProgram(Models.Athlete.Athlete targetAthlete, int assignedProgramId = 0, int programDayId = 0)
        {
            var targetAssignedProgramId = 0;
            if (assignedProgramId == 0)
            {
                targetAssignedProgramId = targetAthlete.AssignedProgramId.HasValue ? targetAthlete.AssignedProgramId.Value : 0;
            }
            else
            {
                targetAssignedProgramId = assignedProgramId;
            }
            if (targetAssignedProgramId == 0)
            {
                return new DTOs.AthleteAssignedPrograms.AssignedProgram();

            }
            var getProgramIdFromAssignedProgramId = 0;
            var programQuery = " SELECT Id, Name, WeekCount FROM dbo.Programs WHERE Id = @ProgramId ";
            var programDayQuery = " SELECT Id, Position FROM dbo.ProgramDays WHERE ProgramId = @ProgramId";

            var CompletedDaysWeek = @" select  ProgramDayId, WeekNumber from CompletedAssignedProgramDays
                                            WHERE AssignedProgramId = @AssignedProgramid AND athleteId = @AthleteId";

            var assignedProgram = new DTOs.AthleteAssignedPrograms.AssignedProgram();
            var completedDays = new List<CompletedProgramDay>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                completedDays = sqlConn.Query<CompletedProgramDay>(CompletedDaysWeek, new { AssignedProgramId = targetAssignedProgramId, AthleteId = targetAthlete.Id }).ToList();
                getProgramIdFromAssignedProgramId = sqlConn.ExecuteScalar<int>($" SELECT ProgramId FROM AssignedPrograms WHERE id = {targetAssignedProgramId} ");
                assignedProgram = sqlConn.QueryFirst<DTOs.AthleteAssignedPrograms.AssignedProgram>(programQuery, new { ProgramId = getProgramIdFromAssignedProgramId });
                assignedProgram.Days = sqlConn.Query<DTOs.AthleteAssignedPrograms.AssignedProgramDays>(programDayQuery, new { ProgramId = getProgramIdFromAssignedProgramId }).ToList();
            }
            if (completedDays.Any())
            {
                assignedProgram.CompletedDays = completedDays.Select(x => new DTOs.AthleteAssignedPrograms.CompletedAssignedProgramDay() { ProgramDayId = x.ProgramDayId, WeekNumber = x.WeekNumber }).ToList();
            }
            else
            {
                assignedProgram.CompletedDays = new List<DTOs.AthleteAssignedPrograms.CompletedAssignedProgramDay>() {
                        new DTOs.AthleteAssignedPrograms.CompletedAssignedProgramDay() { ProgramDayId = assignedProgram.Days.Select(x => x.Id).OrderBy(x => x).ToList().First(), WeekNumber = 1 }
                        };
            }

            if (assignedProgram.Days != null)
            {
                var programSurveys = GetAssignedSurveys(targetAssignedProgramId, targetAthlete.Id, programDayId);
                var programMetrics = GetAssignedMetrics(targetAssignedProgramId, targetAthlete.Id, programDayId);
                var programWorkout = GetAssignedSets(targetAssignedProgramId, targetAthlete.Id, programDayId);//not getting the new advanced options
                var programSuperSet_setsAndReps = GetAssignedSuperSet_SetsAndReps(targetAssignedProgramId, targetAthlete.Id, programDayId);
                var programSuperSets = GetAssignedSuperSets(programDayId, targetAssignedProgramId);
                var programSuperSetExercise = GetAssignedSuperSetsExericses(programDayId, targetAssignedProgramId);
                var programNotes = GetAssignedNotes(targetAssignedProgramId, programDayId);
                var superSetNotes = SuperSetNotes(targetAssignedProgramId, targetAthlete.Id, programDayId);
                var programVideos = GetAssignedVideos(targetAssignedProgramId, targetAthlete.Id, programDayId);
                programSuperSetExercise.ForEach(x =>
                {
                    x.SetsAndReps = new List<AssignedSuperSetSetRep>();
                    x.SetsAndReps = programSuperSet_setsAndReps.Where(y => y.SuperSetExerciseId == x.SuperSetExerciseId).ToList();
                });
                programSuperSets.ForEach(x =>
                {
                    x.Exercises = new List<AssignedSuperSetExercise>();
                    x.Exercises = programSuperSetExercise.Where(y => y.ProgramDayItemSuperSetId == x.SuperSetId).ToList();
                });
                //programSuperSets.ForEach(x => x.Exercises = pro)
                assignedProgram.Days.ForEach(x =>
                {
                    x.AssignedExercises = new List<DTOs.AthleteAssignedPrograms.AssignedExercise>();
                    x.AssignedMetrics = programMetrics.Where(y => y.ProgramDayId == x.Id).ToList();
                    x.AssignedSurveys = programSurveys.Where(y => y.ProgramDayId == x.Id).ToList();
                    x.AssignedNotes = programNotes.Where(y => y.ProgramDayId == x.Id).ToList();
                    x.AssignedVideos = programVideos.Where(y => y.ProgramDayId == x.Id).ToList();

                    programWorkout.Where(y => y.ProgramDayId == x.Id).GroupBy(
                           g => g.ProgramDayItemExerciseId,
                           g => g,
                           (key, setsAndReps) => new { key = key, SetsAndReps = setsAndReps }).ToList().ForEach(z =>
                           {
                               var tempPosition = 0;
                               if (z.SetsAndReps.Any())
                               {
                                   tempPosition = z.SetsAndReps.FirstOrDefault().ProgramDayItemPosition;
                               }
                               x.AssignedExercises.Add(new AssignedExercise() { AssignedSetsReps = z.SetsAndReps.ToList(), Position = tempPosition });
                           });

                    x.AssignedSuperSets = programSuperSets.Where(y => y.ProgramDayId == x.Id).ToList();
                    x.AssignedSuperSets.ForEach(z => { z.Notes = superSetNotes.Where(y => y.ProgramDayItemSuperSetId == z.SuperSetId).ToList(); });

                });
            }
            assignedProgram.AthleteId = targetAthlete.Id;
            return assignedProgram;
        }

        public List<AssignedSuperSetExercise> GetAssignedSuperSetsExericses(int programDayId, int assignedProgramId)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var getSSEString = $@" SELECT se.ShowWeight as 'ShowWeight', se.Rest AS 'Rest', ss.Id AS 'ProgramDayItemSuperSetId', se.Position AS 'PositionInSuperSet', se.ExerciseId AS 'SuperSet_ExerciseId', e.[Name] AS 'ExerciseName',se.id AS 'SuperSetExerciseId', e.videoURL AS 'VideoURL', se.Rest
                                FROM ProgramDayItemSuperSets AS ss
                                INNER JOIN SuperSetExercises AS se ON SS.Id = se.ProgramDayItemSuperSetId
                                INNER JOIN ProgramDayItems AS pdi ON pdi.id = ss.ProgramDayItemId
                                INNER JOIN ProgramDays AS pd ON pd.Id = pdi.ProgramDayId
                                INNER JOIN programs  AS p ON p.Id = pd.ProgramId
                                INNER JOIN exercises AS e ON e.id = se.ExerciseId
                                INNER JOIN  AssignedPrograms AS ap ON ap.ProgramId  = P.Id
                                    WHERE ap.id = @AssignedProgramId {programDayInsert}";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var ret = sqlConn.Query<AssignedSuperSetExercise>(getSSEString, new { AssignedProgramId = assignedProgramId, ProgramDayId = programDayId }).ToList();

                ret.ForEach(x =>
                {
                    //yup fucked up. the json object at the angular service is straight mapping the return to another object, instead of going through my converting function
                    if (string.IsNullOrEmpty(x.VideoURL))
                    {
                        x.VideoProvider = 0;
                    }
                    else
                    {
                        x.VideoProvider = x.VideoURL.IndexOf("youtu") > 0 ? 1 : 2;
                    }

                });
                return ret;
            }
        }

        public List<AssignedSuperSet> GetAssignedSuperSets(int programDayId, int assignedProgramId)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var insertString = $@" SELECT  ss.id AS 'SuperSetId', pdi.Position AS 'PositionInProgramDay',pd.Id AS 'ProgramDayId' , ss.SuperSetDisplayTitle as 'SuperSetDisplayTitle'
                                    FROM ProgramDayItemSuperSets AS ss
                                    INNER JOIN ProgramDayItems AS pdi ON pdi.id = ss.ProgramDayItemId
                                    INNER JOIN ProgramDays AS pd ON pd.Id = pdi.ProgramDayId
                                    INNER JOIN programs AS p ON p.Id = pd.ProgramId
                                    INNER JOIN  AssignedPrograms AS ap ON ap.ProgramId = P.Id
                                    WHERE ap.id = @AssignedProgramId {programDayInsert}";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<AssignedSuperSet>(insertString, new { AssignedProgramId = assignedProgramId, ProgramDayId = programDayId }).ToList();
            }
        }
        public void MarkDayAsCompleted(int athleteId, int programDayId, int assignedProgramId, int weekNumber)
        {
            var insertString = " INSERT INTO CompletedAssignedProgramDays(AssignedProgramId, ProgramDayId, AthleteId, WeekNumber) VALUES (@AssignedProgramId, @ProgramDayId, @AthleteId, @WeekNumber) ";
            using (var sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Execute(insertString, new { AssignedProgramId = assignedProgramId, ProgramDayId = programDayId, AthleteId = athleteId, WeekNumber = weekNumber });
            }
        }
        public void CompleteAllAssignedWeightWorkouts(int athleteId, int programDayId, int assignedProgramId, int weekId)
        {
            //i dont think we need this
            //var updateString = @"INSERT INTO  CompletedSets(Position,[Sets],Reps,[Percent],[Weight],OriginalSetId,AthleteId,AssignedProgramId,CompletedDate) 
            //                    SELECT ps.Position 
            //                    ,ps.[Sets] 
            //                    ,ps.Reps
            //                    ,ps.[Percent]
            //                    , CASE 
            //                      WHEN ps.[Percent] IS NOT NULL THEN
            //                       (SELECT TOP 1 [value] 
            //                       FROM completedmetrics AS cm
            //                       INNER JOIN metrics AS m ON cm.metricid = m.id
            //                       WHERE m.id = e.PercentMetricCalculationId
            //                       ORDER BY cm.id DESC) * e.[percent] * ps.[Percent]
            //                       WHEN ps.[Percent] IS NULL THEN
            //                       (SELECT top 1 [value] 
            //                       FROM completedmetrics AS cm
            //                       INNER JOIN metrics AS m ON cm.metricid = m.id
            //                       WHERE m.id = e.PercentMetricCalculationId
            //                       ORDER BY cm.id DESC) * e.[percent]
            //                      END
            //                      ,ps.Id 
            //                      ,@AthleteId
            //                      ,@AssignedProgramId
            //                      ,getdate()
            //                    FROM ProgramDayItemExercises AS pdiE   
            //                    INNER JOIN  ProgramDayItems AS pdi ON pdi.id = pdie.ProgramDayItemId   
            //                    INNER JOIN Exercises AS e ON PDIE.ExerciseId = e.Id   
            //                    INNER JOIN ProgramDays AS pd on pd.Id = pdi.ProgramDayId   
            //                    INNER JOIN AssignedPrograms AS ap ON ap.ProgramId = pd.programId
            //                    INNER JOIN ProgramWeeks AS pw ON pw.ProgramDayItemExerciseId = pdie.Id
            //                    INNER JOIN programSets AS ps ON ps.ParentProgramWeekId = pw.Id
            //                    WHERE ap.id = @AssignedProgramId 
            //                    and programDayId = @ProgramDayId
            //                    and e.PercentMetricCalculationId is not null
            //                    AND pw.position =  @WeekId
            //                    ";
            //using (var sqlCon = new SqlConnection(ConnectionString))
            //{
            //    sqlCon.Execute(updateString, new { AssignedProgramId = assignedProgramId, ProgramDayId = programDayId, AthleteId = athleteId, WeekId = weekId });
            //}
        }



        public void GetDisplayWeeksForSnapShotProgramMovies(List<ProgramDayMovie> targetMovies)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM AssignedProgram_moviedisplayweek WHERE AssignedProgram_ProgramDayItemMovieId = @PDIMetricId";

            foreach (var item in targetMovies)
            {
                using (var sqlCon = new SqlConnection(ConnectionString))
                {
                    item.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { PDIMetricID = item.Id }).ToList();
                }
            }
        }

        public void GetDisplayWeeksForSnapShotProgramMetrics(List<ProgramDayMetrics> targetMetrics)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM AssignedProgram_metricsDisplayWeek WHERE AssignedProgram_ProgramDayItemMetricId = @PDIMetricId";

            foreach (var item in targetMetrics)
            {
                using (var sqlCon = new SqlConnection(ConnectionString))
                {
                    item.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { PDIMetricID = item.Id }).ToList();
                }
            }
        }
        public void GetDisplayWeeksForSnapShotProgramSurvey(List<ProgramDaySurvey> targetSurveys)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM AssignedProgram_SurveyDisplayWeeks WHERE AssignedProgram_ProgramDayItemSurveyId = @PDISurveyId";

            foreach (var item in targetSurveys)
            {
                using (var sqlCon = new SqlConnection(ConnectionString))
                {
                    item.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { PDISurveyId = item.Id }).ToList();
                }
            }
        }


        public void GetDisplayWeeksForSnapShotSuperSetNote(List<AssignedSuperSetNote> targetSuperSetNotes)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM AssignedProgram_SuperSetNoteDisplayWeek WHERE AssignedProgram_SuperSetNoteId = @SSNID";

            foreach (var item in targetSuperSetNotes)
            {
                using (var sqlCon = new SqlConnection(ConnectionString))
                {
                    item.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { SSNID = item.Id }).ToList();
                }
            }
        }

        public void GetDisplayWeeksForProgramMovies(List<ProgramDayMovie> targetMovies)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM moviedisplayweeks WHERE ProgramDayItemMovieId = @PDIMetricId";

            foreach (var item in targetMovies)
            {
                using (var sqlCon = new SqlConnection(ConnectionString))
                {
                    item.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { PDIMetricID = item.Id }).ToList();
                }
            }
        }

        public void GetDisplayWeeksForProgramMetrics(List<ProgramDayMetrics> targetMetrics)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM metricDisplayWeeks WHERE ProgramDayItemMetricId = @PDIMetricId";

            foreach (var item in targetMetrics)
            {
                using (var sqlCon = new SqlConnection(ConnectionString))
                {
                    item.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { PDIMetricID = item.Id }).ToList();
                }
            }
        }
        public void GetDisplayWeeksForProgramSurvey(List<ProgramDaySurvey> targetSurveys)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM SurveyDisplayWeeks WHERE ProgramDayItemSurveyId = @PDISurveyId";

            foreach (var item in targetSurveys)
            {
                using (var sqlCon = new SqlConnection(ConnectionString))
                {
                    item.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { PDISurveyId = item.Id }).ToList();
                }
            }
        }
        public void GetDisplayWeeksForAssignedProgramSurvey(List<DTOs.AthleteAssignedPrograms.AssignedProgramDaySurvey> targetSurveys)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM AssignedProgram_SurveyDisplayWeeks WHERE AssignedProgram_ProgramDayItemSurveyId = @PDISurveyId";

            foreach (var item in targetSurveys)
            {
                using (var sqlCon = new SqlConnection(ConnectionString))
                {
                    item.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { PDISurveyId = item.ProgramDayItemSurveyId }).ToList();
                }
            }
        }

        public void GetDisplayWeeksForAssignedSuperSetNote(List<AssignedSuperSetNote> targetSuperSetNotes)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM SuperSetNoteDisplayWeeks WHERE SuperSetNoteId = @SSNID";

            foreach (var item in targetSuperSetNotes)
            {
                using (var sqlCon = new SqlConnection(ConnectionString))
                {
                    item.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { SSNID = item.Id }).ToList();
                }
            }
        }

        public List<DTOs.AthleteAssignedPrograms.AssignedSetRep> GetAssignedSets(int assignedProgramId, int athleteId, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";

            var getString = $@"SELECT pdie.Id as 'ProgramDayItemExerciseId' ,pd.Id as 'ProgramDayId',pw.Position as 'SetWeekId',pdi.Position as 'ProgramDayItemPosition'
                            ,ps.[Percent] as 'AssignedWorkoutPercent', ps.[Sets] as 'AssignedWorkoutSets', ps.Reps as 'AssignedWorkoutReps', ps.[weight] as 'AssignedWorkoutweight', ps.position as 'PositionInSet', ps.Id as 'OriginalSetId'  
                            ,cs.[Percent] as 'CompletedSetPercent', cs.[Sets] as 'CompletedSetSets', cs.Reps as 'CompletedSetReps', cs.[weight] as 'CompletedSetWeight'
                            ,e.[Name] as 'ExerciseName',e.[Percent] , e.PercentMetricCalculationId, ap.Id AS 'AssignedProgramId', @AthleteId as 'AthleteId'
                            ,(SELECT TOP 1 [VALUE]
                                FROM (
                                SELECT [value], CompletedDate
                                FROM addedMetrics AS am
                                where  am.metricId = e.PercentMetricCalculationId AND am.athleteId = @AthleteId
                                UNION ALL
                                SELECT [value], CompletedDate
                                FROM CompletedMetrics AS CM where  CM.metricId = e.PercentMetricCalculationId AND cm.athleteId = @AthleteId) AS allMetrics
                                ORDER BY completedDate desc) * (e.[percent] * .01) AS PercentMaxCalc 
                            ,(SELECT TOP 1 [value] 
                                FROM (
                                SELECT [value], CompletedDate
                                FROM addedMetrics AS am
                                where  am.metricId = e.PercentMetricCalculationId AND am.athleteId = @AthleteId
                                UNION ALL
                                SELECT [value], CompletedDate
                                FROM CompletedMetrics AS CM where  CM.metricId = e.PercentMetricCalculationId AND cm.athleteId = @AthleteId) AS allMetrics
                                ORDER BY completedDate desc) * (e.[percent] * .01) * (ps.[Percent] * .01) AS PercentMaxCalcSubPercent   
                            FROM ProgramDayItemExercises AS pdiE   
                            INNER JOIN  ProgramDayItems AS pdi ON pdi.id = pdie.ProgramDayItemId   
                            INNER JOIN Exercises AS e ON PDIE.ExerciseId = e.Id   
                            INNER JOIN ProgramDays AS pd on pd.Id = pdi.ProgramDayId   
                            INNER JOIN AssignedPrograms AS ap ON ap.ProgramId = pd.programId
                            INNER JOIN ProgramWeeks AS pw ON pw.ProgramDayItemExerciseId = pdie.Id
                            INNER JOIN programSets AS ps ON ps.ParentProgramWeekId = pw.Id
                            LEFT JOIN completedSets AS cs ON cs.OriginalSetId = ps.Id and cs.AssignedProgramId = ap.Id and cs.AthleteId = @AthleteId AND cs.AssignedProgramId = ap.Id
                            WHERE ap.id = @AssignedProgramId {programDayInsert}";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.AthleteAssignedPrograms.AssignedSetRep>(getString, new { AssignedProgramId = assignedProgramId, AthleteId = athleteId }).ToList();
            }
        }
        public List<DTOs.AthleteAssignedPrograms.AssignedSuperSetSetRep> GetAssignedSuperSet_SetsAndReps(int assignedProgramId, int athleteId, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";

            var getString = $@"SELECT  ssw.Id AS 'SuperSetWeekId'
                                ,ssw.Position AS 'WeekPosition'
                                ,ss_s.[Percent]  AS 'AssignedWorkoutPercent'
                                ,ss_s.[Sets] AS 'AssignedWorkoutSets'
                                ,ss_s.Reps AS 'AssignedWorkoutReps'
                                ,ss_s.[Weight] AS 'AssignedWorkoutWeight'
                                ,ss_s.id AS 'OriginalSuperSet_SetId'
                                ,ss_s.[Minutes] AS 'AssignedWorkoutMinutes'
                                ,ss_s.[Seconds] AS 'AssignedWorkoutSeconds'
                                ,ss_s.[Distance] AS 'AssignedWorkoutDistance'
                                ,ss_s.[RepsAchieved] AS 'RepsAchieved'
                                ,ss_s.[Other] AS 'AssignedOther' 
                                ,css_s.[Percent] AS 'CompletedSetPercent'
                                ,css_s.[Sets] AS 'CompletedSetSets'
                                ,css_s.[Weight] AS 'CompletedSetWeight'
                                ,css_s.[CompletedRepsAchieved] AS 'CompletedRepsAchieved'
                                ,FLOOR(ROUND((SELECT TOP 1 [VALUE]
                                    FROM (
                                    SELECT [value], CompletedDate
                                    FROM addedMetrics AS am
                                    where  am.metricId = e.PercentMetricCalculationId AND am.athleteId = @AthleteId
                                    UNION ALL
                                    SELECT [value], CompletedDate
                                    FROM CompletedMetrics AS CM where  CM.metricId = e.PercentMetricCalculationId AND cm.athleteId = @AthleteId) AS allMetrics
                                    ORDER BY completedDate desc)  * (e.[percent] * .01),0)) AS PercentMaxCalc 
                                ,FLOOR(ROUND((SELECT TOP 1 [VALUE]
                                        FROM (
                                        SELECT [value], CompletedDate
                                        FROM addedMetrics AS am
                                        where  am.metricId = e.PercentMetricCalculationId AND am.athleteId = @AthleteId
                                        UNION ALL
                                        SELECT [value], CompletedDate
                                        FROM CompletedMetrics AS CM where  CM.metricId = e.PercentMetricCalculationId AND cm.athleteId = @AthleteId) AS allMetrics
                                        ORDER BY completedDate desc)  * (e.[percent] * .01) * (ss_s.[Percent] * .01),0)) AS PercentMaxCalcSubPercent 
                                ,e.[Name] AS 'ExerciseName'
                                ,@AssignedProgramId AS 'AssignedProgramId'
                                ,pd.Id AS 'ProgramDayId'
                                ,ss_s.Position AS 'PositionInSet'
                                ,ss.Id AS 'ProgramDayItemSuperSetId'
                                ,@athleteId AS 'AthleteId'
                                ,se.Id AS 'SuperSetExerciseId'
                                ,se.ExerciseId  AS 'SuperSet_ExerciseId'
                                ,ss_s.Id AS 'OriginalSuperSet_SetId' 
                                ,se.Rest AS 'SuperSetExerciseRest'
                                ,se.ShowWeight as 'ShowWeight'
                                FROM ProgramDayItemSuperSets AS ss
                                INNER JOIN SuperSetExercises AS se ON se.ProgramDayItemSuperSetId = ss.id
                                INNER JOIN ProgramDayItemSuperSetWeeks AS ssw ON ssw.SuperSetExerciseId = se.Id
                                INNER JOIN ProgramDayItemSuperSet_Set AS ss_s ON ss_s.ProgramDayItemSuperSetWeekId = ssw.Id
                                LEFT JOIN CompletedSuperSet_Set AS css_s ON css_s.OriginalSuperSet_SetId = ss_s.Id  AND css_s.AthleteId = @athleteId
                                INNER JOIN ProgramDayItems AS pdi ON pdi.id = ss.ProgramDayItemId
                                INNER JOIN ProgramDays AS pd ON pd.Id = pdi.ProgramDayId
                                INNER JOIN programs  AS p ON p.Id = pd.ProgramId
                                INNER JOIN AssignedPrograms AS ap ON ap.ProgramId = p.Id
                                INNER JOIN Exercises AS e ON e.Id = se.ExerciseId 
                                WHERE ap.id = @AssignedProgramId  {programDayInsert}";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.AthleteAssignedPrograms.AssignedSuperSetSetRep>(getString, new { AssignedProgramId = assignedProgramId, AthleteId = athleteId }).ToList();
            }
        }

        public List<DTOs.AthleteAssignedPrograms.AthleteAssignedQuestions> GetAssignedQuestions(int assignedProgramId, int athleteId, int surveyId, int programDayItemSurveyId = 0, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var seperateByDays = programDayItemSurveyId == 0 ? "" : $" AND pdis.id = {programDayItemSurveyId}";
            var getString = $@" SELECT  s.Id as 'SurveyId', s.Title as 'SurveyName', sdw.DisplayWeek as 'DisplayWeekId', ap.Id as 'AssignedProgramId', ap.ProgramId as 'ProgramId', pdis.Id as 'ProgramDaySurveyItemId' ,q.Id as 'QuestionId', q.QuestionTypeId as 'QuestionTypeId', q.questionDisplayText as 'QuestionDisplayText'
                                        , CASE
                                                WHEN q.QuestionTypeId = 1 THEN CAST(cqyn.YesNoValue AS VARCHAR )
                                                WHEN q.QuestionTypeId = 2 THEN CAST(cqs.ScaleValue AS VARCHAR)
                                                WHEN q.QuestionTypeId = 3 THEN cqoe.Response
                                       END AS 'Answer'
                            FROM AssignedPrograms AS ap
                            INNER JOIN Programs AS p on AP.ProgramId = p.Id
                            INNER JOIN ProgramDays AS pd ON pd.ProgramId = p.Id
                            INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                            INNER JOIN ProgramDayItemSurveys AS pdis ON pdis.ProgramDayItemId = pdi.id
                            INNER JOIN Surveys AS s ON s.Id = pdis.SurveyId
                            INNER JOIN SurveysToQuestions AS stq ON stq.SurveyId = s.Id
                            INNER JOIN Questions AS q ON stq.QuestionId = q.Id
                            INNER JOIN SurveyDisplayWeeks AS sdw ON sdw.ProgramDayItemSurveyId = pdis.Id
                            LEFT JOIN CompletedQuestionOpenEndeds AS cqoe ON cqoe.QuestionId = q.id AND cqoe.athleteId = @athleteId AND cqoe.weekId = sdw.DisplayWeek  AND cqoe.AssignedProgramId = ap.Id 
                            LEFT JOIN CompletedQuestionScales AS cqs ON cqs.QuestionId = q.Id AND cqs.athleteId = @athleteId AND cqs.weekId = sdw.DisplayWeek AND cqs.AssignedProgramId = ap.Id 
                            LEFT JOIN CompletedQuestionYesNoes AS cqyn ON cqyn.questionId = q.id AND cqyn.athleteId = @athleteId AND cqyn.weekId = sdw.DisplayWeek AND cqyn.AssignedProgramId = ap.Id 
                            WHERE ap.id = @AssignedProgramId AND s.id = @SurveyId {programDayInsert} {seperateByDays}"; //god am i sorry. We need to fucking re-work surveys

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.AthleteAssignedPrograms.AthleteAssignedQuestions>(getString, new { AssignedProgramId = assignedProgramId, AthleteId = athleteId, SurveyId = surveyId, @ProgramDayItemSurveyId = programDayItemSurveyId }).ToList();
            }
        }

        public List<DTOs.AthleteAssignedPrograms.AthleteAssignedQuestions> GetSnapShotQuestions(int assignedProgramId, int athleteId, int surveyId, int programDayItemSurveyId = 0, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var seperateByDays = programDayItemSurveyId == 0 ? "" : $" AND pdis.id = {programDayItemSurveyId}";
            var getString = $@" 
SELECT  s.Id as 'SurveyId', s.Title as 'SurveyName', 
sdw.DisplayWeek as 'DisplayWeekId',
 ap.Id as 'AssignedProgramId', ap.Id as 'ProgramId', pdis.Id as 'ProgramDaySurveyItemId' ,q.Id as 'QuestionId', q.QuestionTypeId as 'QuestionTypeId', q.questionDisplayText as 'QuestionDisplayText'
            , CASE
                    WHEN q.QuestionTypeId = 1 THEN CAST(cqyn.YesNoValue AS VARCHAR )
                    WHEN q.QuestionTypeId = 2 THEN CAST(cqs.ScaleValue AS VARCHAR)
                    WHEN q.QuestionTypeId = 3 THEN cqoe.Response
            END AS 'Answer'
FROM AssignedProgram_Program AS ap
INNER JOIN AssignedProgram_ProgramDay AS pd ON pd.AssignedProgram_ProgramId = ap.Id
INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.AssignedProgram_ProgramDayId = pd.Id
INNER JOIN AssignedProgram_ProgramDayItemSurvey AS pdis ON pdis.AssignedProgram_ProgramDayItemId = pdi.id
INNER JOIN Surveys AS s ON s.Id = pdis.SurveyId
INNER JOIN SurveysToQuestions AS stq ON stq.SurveyId = s.Id
INNER JOIN Questions AS q ON stq.QuestionId = q.Id
INNER JOIN AssignedProgram_SurveyDisplayWeeks AS sdw ON sdw.AssignedProgram_ProgramDayItemSurveyId = pdis.Id
LEFT JOIN AssignedProgram_CompletedQuestionOpenEnded AS cqoe ON cqoe.QuestionId = q.id AND cqoe.weekId = sdw.DisplayWeek  AND cqoe.AssignedProgram_ProgramId = ap.Id AND cqoe.AssignedProgram_ProgramDayItemSurveyId = pdis.Id
LEFT JOIN AssignedProgram_CompletedQuestionScale AS cqs ON cqs.QuestionId = q.Id  AND cqs.weekId = sdw.DisplayWeek AND cqs.AssignedProgram_ProgramId = ap.Id AND cqs.AssignedProgram_ProgramDayItemSurveyId = pdis.Id
LEFT JOIN AssignedProgram_CompletedQuestionYesNo AS cqyn ON cqyn.questionId = q.id  AND cqyn.weekId = sdw.DisplayWeek AND cqyn.AssignedProgram_ProgramId = ap.Id AND cqyn.AssignedProgram_ProgramDayItemSurveyId = pdis.Id
WHERE ap.id = @AssignedProgramId AND s.id = @SurveyId {programDayInsert} {seperateByDays}"; //god am i sorry. We need to fucking re-work surveys

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.AthleteAssignedPrograms.AthleteAssignedQuestions>(getString, new { AssignedProgramId = assignedProgramId, AthleteId = athleteId, SurveyId = surveyId, @ProgramDayItemSurveyId = programDayItemSurveyId }).ToList();
            }
        }
        public List<AssignedNote> GetAssignedNotes(int assignedProgramid, int programDayId)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var noteQuery = $@"
                         SELECT ap.Id AS 'AssignedProgramId', pdin.Id AS 'ProgramDayItemNoteId', pd.Id AS 'ProgramDayId', pdi.Position AS 'Position', pdin.Name AS 'Name' , pdin.Note AS 'NoteText', ndw.DisplayWeek AS 'DisplayWeekId'
                        FROM programs AS p
                        INNER JOIN AssignedPrograms AS ap ON ap.programId = p.id
                        INNER JOIN ProgramDays AS pd ON p.Id = pd.ProgramId
                        INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                        INNER JOIN ProgramDayItemNotes AS pdin ON pdi.Id = pdin.ProgramDayItemId
                        INNER JOIN NoteDisplayWeeks AS ndw ON ndw.ProgramDayItemNoteId = pdin.id
                            WHERE ap.Id = {assignedProgramid}  {programDayInsert}";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<AssignedNote>(noteQuery, new
                {
                    AssignedProgramId = assignedProgramid
                }).ToList();
            }
        }
        public List<DTOs.AthleteAssignedPrograms.AssignedProgramDaySurvey> GetAssignedSurveys(int assignedProgramId, int athleteId, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var surveyQuery = $@" SELECT pdiSurveys.Id  as 'ProgramDayItemSurveyId', pdiSurveys.SurveyId 'SurveyId',pdi.position as 'Position', pdi.ProgramDayId AS  'ProgramDayId' 
                                    FROM ProgramDayItemSurveys AS pdiSurveys 
                                    INNER JOIN programDayItems AS pdi ON pdi.Id = pdiSurveys.ProgramDayItemId 
                                    INNER JOIN ProgramDays AS pd on pd.Id = pdi.ProgramDayId 
                                    INNER JOIN AssignedPrograms AS ap ON ap.ProgramId = pd.ProgramId
                                    Where ap.Id = @AssignedProgramId {programDayInsert}";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var programSurveysReader = sqlConn.ExecuteReader(surveyQuery, new { AssignedProgramId = assignedProgramId });
                var programSurveys = GetAssignedProgramDaySurveysFromDataReader(programSurveysReader, athleteId, assignedProgramId);
                GetDisplayWeeksForAssignedProgramSurvey(programSurveys);
                programSurveysReader.Close();
                return programSurveys;
            }
        }
        public List<AssignedSuperSetNote> GetSnapShotSuperSetNotes(int assignedProgramId, int athleteId, int programDayId = 0)
        {
            var noteQuery = $@"
                                SELECT ssn.note,ssn.Position,pdiss.id AS 'ProgramDayItemSuperSetId' , ssn.Id AS id
                                FROM AssignedProgram_ProgramDay AS ap
                                INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.AssignedProgram_ProgramDayId = ap.Id
                                INNER JOIN AssignedProgram_ProgramDayItemSuperSet AS pdiss ON pdiss.AssignedProgram_ProgramDayItemId = pdi.Id
                                INNER JOIN AssignedProgram_SuperSetNote AS ssn ON ssn.AssignedProgram_ProgramDayItemSuperSetId = pdiss.Id
                                INNER JOIN [AssignedProgram_SuperSetNoteDisplayWeek] AS ssndw ON ssndw.AssignedProgram_SuperSetNoteId = ssn.Id
                                WHERE ap.AssignedProgram_ProgramId = @assignedProgramId                        
                                ";

            var displayWeekQuery = $@"select DisplayWeek from [AssignedProgram_SuperSetNoteDisplayWeek] as a where a.AssignedProgram_SuperSetNoteId = @id";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var superSetNotes = sqlConn.Query<AssignedSuperSetNote>(noteQuery, new { assignedProgramId }).ToList();
                foreach (var note in superSetNotes)
                {
                    note.DisplayWeeks = sqlConn.Query<int>(displayWeekQuery, new { id = note.Id }).ToList();
                }
                return superSetNotes;
            }
        }


        public List<AssignedSuperSetNote> SuperSetNotes(int assignedProgramId, int athleteId, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var surveyQuery = $@" SELECT  ssn.id,ssn.note,ssn.Position, ssn.programDayItemSuperSetId
                                    FROM ProgramDayItemSuperSets AS ss
                                    INNER JOIN ProgramDayItems AS pdi ON pdi.id = ss.ProgramDayItemId
                                    INNER JOIN ProgramDays AS pd ON pd.Id = pdi.ProgramDayId
                                    INNER JOIN programs AS p ON p.Id = pd.ProgramId
                                    INNER JOIN  AssignedPrograms AS ap ON ap.ProgramId = P.Id
                                    INNER JOIN  SuperSetNotes AS ssn ON ssn.ProgramDayItemSuperSetId = ss.id
                                    WHERE ap.id = @AssignedProgramId {programDayInsert}";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var superSetNoteReader = sqlConn.ExecuteReader(surveyQuery, new { AssignedProgramId = assignedProgramId });
                var superSetNotes = GetAssignedProgramDaySuperSetNotes(superSetNoteReader, athleteId, assignedProgramId);
                GetDisplayWeeksForAssignedSuperSetNote(superSetNotes);
                superSetNoteReader.Close();
                return superSetNotes;
            }
        }

        public List<DTOs.AthleteAssignedPrograms.AssignedMetric> GetAssignedMetrics(int assignedProgramId, int athleteId, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var getString = $@"SELECT ap.Id as 'AssignedProgramId', ap.ProgramId as 'ProgramId',  pdiMetric.Id  as 'ProgramDayItemMetricId', m.Id as 'MetricId', m.[name] AS 'MetricName', pdi.position as 'Position', pdi.ProgramDayId AS  'ProgramDayId', mdw.DisplayWeek as 'DisplayWeekId',cm.[Value] AS 'CompletedWeight'
                            FROM AssignedPrograms AS ap
                            INNER JOIN Programs AS p on AP.ProgramId = p.Id
                            INNER JOIN ProgramDays AS pd ON pd.ProgramId = p.Id
                            INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                            INNER JOIN ProgramDayItemMetrics AS pdiMetric ON pdiMetric.ProgramDayItemId = pdi.Id
                            INNER JOIN Metrics AS m ON m.Id = pdiMetric.MetricId
                            INNER JOIN MetricDisplayWeeks AS mdw ON mdw.ProgramDayItemMetricId = pdiMetric.id
                            LEFT JOIN CompletedMetrics AS cm ON cm.ProgramDayItemMetricId = pdiMetric.id and mdw.DisplayWeek = WeekId AND cm.AthleteId = @athleteId AND ap.id = @AssignedProgramId
                            WHERE ap.id = @AssignedProgramId  {programDayInsert}";
            //todo: fix the fuck out of this shitty query
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.AthleteAssignedPrograms.AssignedMetric>(getString, new { AssignedProgramId = assignedProgramId, AthleteId = athleteId }).ToList();
            }

        }
        public List<AssignedVideo> GetAssignedVideos(int assignedProgramId, int athleteId, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var getString = $@"SELECT ap.Id as 'AssignedProgramId', ap.ProgramId as 'ProgramId',  pdiMovie.Id  as 'ProgramDayItemMovieId', m.Id as 'MovieId', m.[name] AS 'Name', m.[url] as 'URL' , pdi.position as 'Position', pdi.ProgramDayId AS  'ProgramDayId', mdw.DisplayWeek as 'DisplayWeekId'
                            FROM AssignedPrograms AS ap
                            INNER JOIN Programs AS p on AP.ProgramId = p.Id
                            INNER JOIN ProgramDays AS pd ON pd.ProgramId = p.Id
                            INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                            INNER JOIN ProgramDayItemMovies AS pdiMovie ON pdiMovie.ProgramDayItemId = pdi.Id
                            INNER JOIN Movies AS m ON m.Id = pdiMovie.MovieId
                            INNER JOIN MovieDisplayWeeks AS mdw ON mdw.ProgramDayItemMovieId = pdiMovie.id
                            WHERE ap.id = @AssignedProgramId  {programDayInsert}";
            //todo: fix the fuck out of this shitty query
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.AthleteAssignedPrograms.AssignedVideo>(getString, new { AssignedProgramId = assignedProgramId, AthleteId = athleteId }).ToList();
            }

        }
        public void GetDisplayWeeksForSuperSetNotes(List<ProgramDaySuperSet> targetSuperSets)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM SuperSetNoteDisplayWeeks WHERE SuperSetNoteId = @SuperSetNoteId";

            foreach (var item in targetSuperSets)
            {
                foreach (var note in item.Notes)
                {
                    using (var sqlCon = new SqlConnection(ConnectionString))
                    {
                        note.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { SuperSetNoteId = note.Id }).ToList();
                    }
                }
            }

        }
        public void GetDisplayWeeksForProgramNotes(List<ProgramDayNotes> targetNotes)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM NoteDisplayWeeks WHERE ProgramDayItemNoteId = @PDINoteId";

            foreach (var item in targetNotes)
            {
                using (var sqlCon = new SqlConnection(ConnectionString))
                {
                    item.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { PDINoteId = item.Id }).ToList();
                }
            }
        }
        public void GetDisplayWeeksForSnapShotSuperSetNotes(List<ProgramDaySuperSet> targetSuperSets)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM AssignedProgram_SuperSetNoteDisplayWeek WHERE AssignedProgram_SuperSetNoteId = @SuperSetNoteId";

            foreach (var item in targetSuperSets)
            {
                foreach (var note in item.Notes)
                {
                    using (var sqlCon = new SqlConnection(ConnectionString))
                    {
                        note.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { SuperSetNoteId = note.Id }).ToList();
                    }
                }
            }

        }
        public void GetDisplayWeeksForSnapShotProgramNotes(List<ProgramDayNotes> targetNotes)
        {
            var metricsWeekDisplay = "SELECT DisplayWeek FROM AssignedProgram_NoteDisplayWeek WHERE AssignedProgram_ProgramDayItemNoteId = @PDINoteId";

            foreach (var item in targetNotes)
            {
                using (var sqlCon = new SqlConnection(ConnectionString))
                {
                    item.DisplayWeeks = sqlCon.Query<int>(metricsWeekDisplay, new { PDINoteId = item.Id }).ToList();
                }
            }
        }
        public List<DTOs.AthleteAssignedPrograms.AssignedSuperSetSetRep> GetSnapShotSuperSet_SetsAndReps(int assignedProgramId, int athleteId, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";

            var getString = $@"CREATE TABLE #fuckingSnapShot_333  (ssw_id INT NOT NULL,ssw_position INT NOT NULL, pd_id INT NOT NULL, ss_Id INT NOT NULL, se_id int not null,SE_REST nvarchar(max), se_ShowWeight BIT NULL, se_ExerciseId int not null)

                                        INSERT INTO  #fuckingSnapShot_333 (ssw_id, ssw_position,pd_id,ss_id,se_id,se_rest,se_showweight,se_ExerciseId )
                                        SELECT 
                                        ssw.Id ,
                                        ssw.Position , 
                                        pd.Id ,
                                        ss.Id ,
                                        se.Id  , 
                                        se.Rest , 
                                        se.ShowWeight,
                                        se.ExerciseId 
                                        FROM assignedProgram_ProgramDay AS pD
                                        INNER JOIN assignedProgram_ProgramDayItem AS pdi ON pd.Id = PDI.AssignedProgram_ProgramDayId
                                        INNER JOIN assignedProgram_ProgramDayItemSuperSet AS ss ON ss.AssignedProgram_ProgramDayItemId = pdi.Id
                                        INNER JOIN assignedProgram_SuperSetExercise AS se ON se.assignedProgram_ProgramDayItemSuperSetId = ss.id 
                                        INNER JOIN assignedProgram_ProgramDayItemSuperSetWeek AS ssw ON ssw.assignedProgram_SuperSetExerciseId = se.Id
                                        WHERE PD.AssignedProgram_ProgramId  =@AssignedProgramId  {programDayInsert}


                                        SELECT 
                                        ssw.ssw_id AS 'SuperSetWeekId', 
                                        ssw.ssw_position AS 'WeekPosition', 
                                        ss_s.[Percent] AS 'AssignedWorkoutPercent', 
                                        ss_s.[Sets] AS 'AssignedWorkoutSets', 
                                        ss_s.Reps AS 'AssignedWorkoutReps', 
                                        ss_s.[Weight] AS 'AssignedWorkoutWeight', 
                                        ss_s.id AS 'OriginalSuperSet_SetId', 
                                        ss_s.[Minutes] AS 'AssignedWorkoutMinutes', 
                                        ss_s.[Seconds] AS 'AssignedWorkoutSeconds', 
                                        ss_s.[Distance] AS 'AssignedWorkoutDistance', 
                                        ss_s.[RepsAchieved] AS 'RepsAchieved', 
                                        ss_s.[Other] AS 'AssignedOther', 
                                        ss_s.[Percent] AS 'CompletedSetPercent', 
                                        ss_s.[completed_Sets] AS 'CompletedSetSets', 
                                        ss_s.[completed_Weight] AS 'CompletedSetWeight', 
                                        ss_s.[Completed_RepsAchieved] AS 'CompletedRepsAchieved', 
                                        ss_s.PercentMaxCalc AS PercentMaxCalc, 
                                        ss_s.PercentMaxCalcSubPercent AS PercentMaxCalcSubPercent, 
                                        e.[Name] AS 'ExerciseName', 
                                        ssw.pd_Id AS 'ProgramDayId', 
                                        ss_s.Position AS 'PositionInSet', 
                                        ssw.ss_Id AS 'ProgramDayItemSuperSetId', 
                                        ssw.se_Id AS 'SuperSetExerciseId', 
                                        ssw.se_ExerciseId AS 'SuperSet_ExerciseId', 
                                        ss_s.Id AS 'OriginalSuperSet_SetId', 
                                        ssw.se_Rest AS 'SuperSetExerciseRest', 
                                        ssw.se_ShowWeight as 'ShowWeight', 
                                        ssw.ssw_id AS 'AssignedProgram_ProgramDayItemSuperSetWeekId' 
                                        FROM #fuckingSnapShot_333 AS ssw
                                        INNER JOIN assignedProgram_ProgramDayItemSuperSet_Set AS ss_s ON ss_s.assignedProgram_ProgramDayItemSuperSetWeekId = ssw.ssw_id
                                        INNER JOIN Exercises AS e ON e.Id = ssw.se_ExerciseId

                                      
                                        drop table #fuckingSnapShot_333 
                                         ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.AthleteAssignedPrograms.AssignedSuperSetSetRep>(getString, new { AssignedProgramId = assignedProgramId, AthleteId = athleteId }).ToList();
            }
        }
        public List<AssignedSuperSetExercise> GetSnapShot_SuperSetExercies(int programDayId, int assignedProgramId)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var getSSEString = $@" SELECT se.ShowWeight as 'ShowWeight', se.Rest AS 'Rest', ss.Id AS 'ProgramDayItemSuperSetId', se.Position AS 'PositionInSuperSet', se.ExerciseId AS 'SuperSet_ExerciseId', e.[Name] AS 'ExerciseName',se.id AS 'SuperSetExerciseId', e.videoURL AS 'VideoURL', se.Rest
                                FROM assignedProgram_ProgramDayItemSuperSet AS ss
                                INNER JOIN assignedProgram_SuperSetExercise AS se ON SS.Id = se.assignedProgram_ProgramDayItemSuperSetId
                                INNER JOIN assignedProgram_ProgramDayItem AS pdi ON pdi.id = ss.assignedProgram_ProgramDayItemId
                                INNER JOIN assignedProgram_ProgramDay AS pd ON pd.Id = pdi.assignedProgram_ProgramDayId
                                INNER JOIN assignedProgram_program  AS p ON p.Id = pd.assignedProgram_ProgramId
                                INNER JOIN exercises AS e ON e.id = se.ExerciseId
                                WHERE p.id = @AssignedProgramId {programDayInsert}";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var ret = sqlConn.Query<AssignedSuperSetExercise>(getSSEString, new { AssignedProgramId = assignedProgramId, ProgramDayId = programDayId }).ToList();

                ret.ForEach(x =>
                {
                    //yup fucked up. the json object at the angular service is straight mapping the return to another object, instead of going through my converting function
                    if (string.IsNullOrEmpty(x.VideoURL))
                    {
                        x.VideoProvider = 0;
                    }
                    else
                    {
                        x.VideoProvider = x.VideoURL.IndexOf("youtu") > 0 ? 1 : 2;
                    }

                });
                return ret;
            }
        }
        public List<AssignedSuperSet> GetSnapShotSuperSets(int programDayId, int assignedProgramId)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var getString = $@"SELECT  ss.id AS 'SuperSetId', pdi.Position AS 'PositionInProgramDay',pd.Id AS 'ProgramDayId' , ss.SuperSetDisplayTitle as 'SuperSetDisplayTitle'
                                    FROM AssignedProgram_ProgramDayItemSuperSet AS ss
                                    INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.id = ss.AssignedProgram_ProgramDayItemId
                                    INNER JOIN AssignedProgram_ProgramDay AS pd ON pd.Id = pdi.AssignedProgram_ProgramDayId
                                    INNER JOIN AssignedProgram_program AS p ON p.Id = pd.AssignedProgram_ProgramId
                                    WHERE p.id = @AssignedProgramId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<AssignedSuperSet>(getString, new { AssignedProgramId = assignedProgramId, ProgramDayId = programDayId }).ToList();
            }
        }
        public List<AssignedVideo> GetSnapShotVideos(int assignedProgramId, int athleteId, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var getString = $@"SELECT ap.Id as 'AssignedProgramId', ap.Id as 'ProgramId',  pdiMovie.Id  as 'ProgramDayItemMovieId', m.Id as 'MovieId', m.[name] AS 'Name', m.[url] as 'URL' , pdi.position as 'Position', pdi.AssignedProgram_ProgramDayId AS  'ProgramDayId', mdw.DisplayWeek as 'DisplayWeekId'
                            FROM AssignedProgram_Program AS ap
                            INNER JOIN AssignedProgram_ProgramDay AS pd ON pd.AssignedProgram_ProgramId = ap.Id
                            INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.AssignedProgram_ProgramDayId = pd.Id
                            INNER JOIN AssignedProgram_ProgramDayItemMovie AS pdiMovie ON pdiMovie.AssignedProgram_ProgramDayItemId = pdi.Id
                            INNER JOIN Movies AS m ON m.Id = pdiMovie.MovieId
                            INNER JOIN AssignedProgram_MovieDisplayWeek AS mdw ON mdw.AssignedProgram_ProgramDayItemMovieId = pdiMovie.id
                            WHERE ap.id = @AssignedProgramId  {programDayInsert}";
            //todo: fix the fuck out of this shitty query
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.AthleteAssignedPrograms.AssignedVideo>(getString, new { AssignedProgramId = assignedProgramId, AthleteId = athleteId }).ToList();
            }
        }
        public List<DTOs.AthleteAssignedPrograms.AssignedMetric> GetSnapShotMetrics(int assignedProgramId, int athleteId, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var getString = $@"SELECT  ap.Id as 'AssignedProgramId', ap.Id as 'ProgramId',  pdiMetric.Id  as 'ProgramDayItemMetricId', m.Id as 'MetricId', m.[name] AS 'MetricName', pdi.position as 'Position', pdi.AssignedProgram_ProgramDayId AS  'ProgramDayId', mdw.DisplayWeek as 'DisplayWeekId',mdw.[Value] AS 'CompletedWeight'
                                FROM AssignedProgram_program AS ap
                                INNER JOIN AssignedProgram_ProgramDay AS pd ON pd.AssignedProgram_ProgramId = ap.Id
                                INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.AssignedProgram_ProgramDayId = pd.Id
                                INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdiMetric ON pdiMetric.AssignedProgram_ProgramDayItemId = pdi.Id
                                INNER JOIN Metrics AS m ON m.Id = pdiMetric.MetricId
                                INNER JOIN AssignedProgram_MetricsDisplayWeek AS mdw ON mdw. AssignedProgram_ProgramDayItemMetricId = pdiMetric.id
                                WHERE ap.id = @AssignedProgramId {programDayInsert}";
            //todo: fix the fuck out of this shitty query
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.AthleteAssignedPrograms.AssignedMetric>(getString, new { AssignedProgramId = assignedProgramId, AthleteId = athleteId }).ToList();
            }

        }
        public List<AssignedNote> GetSnapshotAssignedNotes(int assignedProgramid, int programDayId)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var noteQuery = $@"SELECT p.Id AS 'AssignedProgramId', pdin.Id AS 'ProgramDayItemNoteId', pd.Id AS 'ProgramDayId', pdi.Position AS 'Position', pdin.Name AS 'Name' , pdin.Note AS 'NoteText', ndw.DisplayWeek AS 'DisplayWeekId'
                            FROM assignedProgram_program AS p
                            INNER JOIN assignedProgram_ProgramDay AS pd ON p.Id = pd.assignedProgram_ProgramId
                            INNER JOIN assignedProgram_ProgramDayItem AS pdi ON pdi.assignedProgram_ProgramDayId = pd.Id
                            INNER JOIN assignedProgram_ProgramDayItemNote AS pdin ON pdi.Id = pdin.assignedProgram_ProgramDayItemId
                            INNER JOIN assignedProgram_NoteDisplayWeek AS ndw ON ndw.assignedProgram_ProgramDayItemNoteId = pdin.id
                            WHERE  p.Id = @assignedProgramId  {programDayInsert}";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<AssignedNote>(noteQuery, new
                {
                    AssignedProgramId = assignedProgramid
                }).ToList();
            }
        }
        public List<AssignedProgramDaySurvey> GetSnapShotAssignedSurvey(int assignedProgramId, int programDayId, int athleteId)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var surveyQuery = $@"SELECT pdiSurveys.Id as 'pdiSurveyId', pdiSurveys.SurveyId 'surveyId',pdi.position as 'position', pdi.AssignedProgram_ProgramDayId AS  'dayId'
                             FROM AssignedProgram_ProgramDayItemSurvey AS pdiSurveys
                             INNER JOIN AssignedProgram_programDayItem AS pdi ON pdi.Id = pdiSurveys.AssignedProgram_ProgramDayItemId
                             INNER JOIN AssignedProgram_ProgramDay AS pd on pd.Id = pdi.AssignedProgram_ProgramDayId
                             Where pd.AssignedProgram_ProgramId = @AssignedProgramId { programDayInsert}";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var programSurveysReader = sqlConn.ExecuteReader(surveyQuery, new { AssignedProgramId = assignedProgramId });
                var programSurveys = GetAssignedProgramDaySurveysFromDataReader(programSurveysReader, athleteId, assignedProgramId);
                GetDisplayWeeksForAssignedProgramSurvey(programSurveys);
                programSurveysReader.Close();
                return programSurveys;
            }



        }
        /// <summary>
        /// this grabs an assignedProgram snapshot, to be used and displayed on an athletes workout. Not to be confused with GetSnapShotProgram. GetSnapshotProgram will get a the same persons snapshot program but its to be displayed and
        /// modified on a program builder.
        /// </summary>
        /// <param name="assignedProgramId"></param>
        /// <param name="createdUserToken"></param>
        /// <param name="athleteId"></param>
        /// <param name="programDayId"></param>
        /// <returns></returns>
        public DTOs.AthleteAssignedPrograms.AssignedProgram GetAssignedProgramSnapShot(int assignedProgramId, Guid createdUserToken, int athleteId, int programDayId = 0)
        {
            var assignedProgram = new DTOs.AthleteAssignedPrograms.AssignedProgram();
            var programQuery = " SELECT Id, Name, WeekCount FROM dbo.assignedprogram_program WHERE Id = @ProgramId ";
            var programDayQuery = " SELECT Id, Position FROM dbo.AssignedProgram_ProgramDay WHERE AssignedProgram_ProgramId = @ProgramId";
            var completedDays = _snapShotRepo.GetCompletedDays(assignedProgramId);
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                assignedProgram = sqlConn.QueryFirst<DTOs.AthleteAssignedPrograms.AssignedProgram>(programQuery, new { ProgramId = assignedProgramId });
                assignedProgram.Days = sqlConn.Query<DTOs.AthleteAssignedPrograms.AssignedProgramDays>(programDayQuery, new { ProgramId = assignedProgramId }).ToList();
            }

            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";


            if (assignedProgram.Days != null)
            {
                var programSuperSet_setsAndReps = GetSnapShotSuperSet_SetsAndReps(assignedProgramId, athleteId, programDayId);//not getting the new advanced options
                var programSuperSetExercise = GetSnapShot_SuperSetExercies(programDayId, assignedProgramId);
                var programVideos = GetSnapShotVideos(assignedProgramId, athleteId, programDayId);
                var programSuperSets = GetSnapShotSuperSets(programDayId, assignedProgramId);
                var superSetNotes = GetSnapShotSuperSetNotes(assignedProgramId, athleteId, programDayId);
                var programMetrics = GetSnapShotMetrics(assignedProgramId, athleteId, programDayId);
                var programNotes = GetSnapshotAssignedNotes(assignedProgramId, programDayId);
                var programSurveys = GetSnapShotAssignedSurvey(assignedProgramId, programDayId, athleteId);



                programSuperSetExercise.ForEach(x =>
                {
                    x.SetsAndReps = new List<AssignedSuperSetSetRep>();
                    x.SetsAndReps = programSuperSet_setsAndReps.Where(y => y.SuperSetExerciseId == x.SuperSetExerciseId).ToList();
                });
                programSuperSets.ForEach(x =>
                {
                    x.Exercises = new List<AssignedSuperSetExercise>();
                    x.Exercises = programSuperSetExercise.Where(y => y.ProgramDayItemSuperSetId == x.SuperSetId).ToList();
                });
                assignedProgram.Days.ForEach(x =>
                {
                    x.AssignedExercises = new List<DTOs.AthleteAssignedPrograms.AssignedExercise>();
                    x.AssignedVideos = programVideos.Where(y => y.ProgramDayId == x.Id).ToList();
                    x.AssignedSuperSets = programSuperSets.Where(y => y.ProgramDayId == x.Id).ToList();
                    x.AssignedSuperSets.ForEach(z => { z.Notes = superSetNotes.Where(y => y.ProgramDayItemSuperSetId == z.SuperSetId).ToList(); });
                    x.AssignedMetrics = programMetrics.Where(y => y.ProgramDayId == x.Id).ToList();
                    x.AssignedNotes = programNotes.Where(n => n.ProgramDayId == x.Id).ToList();
                    x.AssignedSurveys = programSurveys.Where(s => s.ProgramDayId == x.Id).ToList();
                });

            }

            if (completedDays == null)
            {
                assignedProgram.CompletedDays = new List<DTOs.AthleteAssignedPrograms.CompletedAssignedProgramDay>()
                    {
                        new DTOs.AthleteAssignedPrograms.CompletedAssignedProgramDay() {  ProgramDayId = 1, WeekNumber =1}
                    };
            }
            else
            {
                assignedProgram.CompletedDays = completedDays.Select(x => new DTOs.AthleteAssignedPrograms.CompletedAssignedProgramDay() { ProgramDayId = x.AssignedProgram_ProgramDayId, WeekNumber = x.WeekId }).ToList();
            }


            assignedProgram.AthleteId = athleteId;
            return assignedProgram;
        }
        /// <summary>
        /// put security at the manager level
        /// 
        /// This grabs a persons assignedProgram to be displayed only on the program build, thus they can change it. This will not return the correct object to dipslay on  a workout. If you want to display it on a work out you will
        /// need to grab GetAssignedProgramSnapShot
        /// </summary>
        /// <param name="assignedProgramId"></param>
        /// <param name="createdUserToken"></param>
        /// <param name="programDayId"></param>
        /// <returns></returns>
        public DTOs.Program.Program GetSnapShotProgram(int assignedProgramId, Guid createdUserToken, int programDayId = 0)
        {
            var programQuery = " SELECT Id, Name,  WeekCount, 1 AS 'CanModify', 0 AS 'isDeleted' FROM dbo.assignedProgram_program WHERE Id = @assignedProgramId";
            var programDayQuery = " SELECT Id, Position FROM dbo.AssignedProgram_ProgramDay WHERE AssignedProgram_programId = @assignedProgramId";

            var metricsQuery = $@" SELECT pdiMet.Id as 'pdiMetId', met.Id as 'MetricId', met.[name] AS 'name', pdi.position as 'position', pdi. AssignedProgram_ProgramDayId AS  'dayId' 
                                    FROM assignedProgram_ProgramDayItemMetric AS pdiMet 
                                    INNER JOIN  metrics AS met ON met.Id = pdiMet.MetricId   
                                    INNER JOIN assignedProgram_programDayItem AS pdi ON pdi.Id = pdiMet. AssignedProgram_ProgramDayItemId 
                                    INNER JOIN assignedProgram_ProgramDay AS pd on pd.Id = pdi. AssignedProgram_ProgramDayId 
                                    WHERE pd. AssignedProgram_ProgramId = @assignedProgramId";


            var videoQuery = $@"SELECT pdim.Id, m.Id,m.[url], m.[name],  pdi.Position,pd.id
                                FROM AssignedProgram_ProgramDayItemMovie AS pdim
                                INNER JOIN Movies AS m ON  m.Id = pdim.MovieId
                                INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.Id = pdim.AssignedProgram_ProgramDayItemId
                                INNER JOIN AssignedProgram_ProgramDay AS pd ON  pd.Id = pdi.AssignedProgram_ProgramDayId
                                where pd.AssignedProgram_programId = @assignedProgramID ";

            var notesQuery = $@" SELECT pdiNotes.Id as 'pdiNoteId', pdiNotes.[name] as 'name', pdiNotes.Note as 'notes', pdi.position as 'position', pdi. AssignedProgram_ProgramDayId AS  'dayId' 
                                FROM  AssignedProgram_ProgramDayItemNote AS pdiNotes 
                                INNER JOIN  AssignedProgram_programDayItem AS pdi ON pdi.Id = pdiNotes. AssignedProgram_ProgramDayItemId 
                                INNER JOIN  AssignedProgram_ProgramDay AS pd on pd.Id = pdi. AssignedProgram_ProgramDayId 
                                Where pd. AssignedProgram_ProgramId = @assignedProgramID ";

            var surveyQuery = $@" SELECT pdiSurveys.Id  as 'pdiSurveyId', pdiSurveys.SurveyId 'surveyId',pdi.position as 'position', pdi.AssignedProgram_ProgramDayId AS  'dayId' 
                                FROM AssignedProgram_ProgramDayItemSurvey AS pdiSurveys 
                                INNER JOIN AssignedProgram_programDayItem AS pdi ON pdi.Id = pdiSurveys.AssignedProgram_ProgramDayItemId 
                                INNER JOIN AssignedProgram_ProgramDay AS pd on pd.Id = pdi.AssignedProgram_ProgramDayId 
                                Where pd.AssignedProgram_ProgramId = @assignedProgramID ";
            //woot fucking exercises can never be on snapshots, because they were deprecated in 2020 and snapshots were created in 2021

            var SuperSetQuery = $@"  SELECT pdiSS.Id 'ProgramDaySuperSetId',pd.Id as 'ProgramDayId', pdi.position as 'Position', SuperSetDisplayTitle as 'SuperSetDisplayTitle'
                                    FROM AssignedProgram_ProgramDayItemSuperSet as pdiSS
                                    INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.id = pdiSS.AssignedProgram_ProgramDayItemId
                                    INNER JOIN AssignedProgram_ProgramDay AS pd ON pd.id = pdi.AssignedProgram_ProgramDayId
                                    INNER JOIN AssignedProgram_program AS p on p.id = pd.AssignedProgram_ProgramId 
                                    Where p.Id = @assignedProgramID";

            var SuperSetExercisesQuery = $@"SELECT sse.ExerciseId,sse.Position, e.[name],e.notes,pdiss.id as 'ProgramDaySuperSetId', sse.Id as 'programDaySuperSet_ExerciseId', sse.Rest, sse.ShowWeight as 'ShowWeight'
                                            FROM AssignedProgram_ProgramDayItemSuperSet as pdiSS
                                            INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.id = pdiSS.AssignedProgram_ProgramDayItemId
                                            INNER JOIN AssignedProgram_SuperSetExercise  AS sse ON sse.AssignedProgram_ProgramDayItemSuperSetId = pdiSS.Id
                                            INNER JOIN Exercises AS e ON e.Id = sse.ExerciseID
                                            INNER JOIN AssignedProgram_ProgramDay AS pd ON pd.id = pdi.AssignedProgram_ProgramDayId
                                            INNER JOIN AssignedProgram_program AS p on p.id = pd.AssignedProgram_ProgramId 
                                            Where p.Id = @assignedProgramID  ";

            var SuperSetWeeksQuery = $@"SELECT pdiSSW.id as 'ProgramDaySuperSet_WeeksId' , pdiSSw.Position as 'Position',pdiSSW.AssignedProgram_SuperSetExerciseId as 'ProgramDaySuperSet_ExerciseId'
                                        FROM AssignedProgram_ProgramDayItemSuperSet as pdiSS
                                        INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.id = pdiSS.AssignedProgram_ProgramDayItemId
                                        INNER JOIN AssignedProgram_SuperSetExercise  AS sse ON sse.AssignedProgram_ProgramDayItemSuperSetId = pdiSS.Id
                                        INNER JOIN Exercises AS e ON e.Id = sse.ExerciseID
                                        INNER JOIN AssignedProgram_ProgramDayItemSuperSetWeek AS pdiSSW on sse.Id = pdiSSW.AssignedProgram_SuperSetExerciseId
                                        INNER JOIN AssignedProgram_ProgramDay AS pd ON pd.id = pdi.AssignedProgram_ProgramDayId
                                        INNER JOIN AssignedProgram_program AS p on p.id = pd.AssignedProgram_ProgramId 
                                        Where p.Id = @assignedProgramID";


            var superSet_SetsQuery = $@"CREATE TABLE #SpeedUpSearchingProgramDayItemSuperSetWeek
                                        (
                                        pdiSSWId INT
                                        )
                                        INSERT INTO #SpeedUpSearchingProgramDayItemSuperSetWeek
                                        SELECT  pdissw.Id 
                                        from AssignedProgram_ProgramDay AS pd
                                        INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.AssignedProgram_ProgramDayId = pd.Id
                                        INNER JOIN AssignedProgram_ProgramDayItemSuperSet AS pdiss  ON  pdiss.AssignedProgram_ProgramDayItemId = pdi.Id
                                        INNER JOIN AssignedProgram_SuperSetExercise  AS sse ON sse.AssignedProgram_ProgramDayItemSuperSetId = pdiSS.Id
                                        INNER JOIN Exercises AS e ON e.Id = sse.ExerciseID
                                        INNER JOIN AssignedProgram_ProgramDayItemSuperSetWeek AS pdiSSW on sse.Id = pdiSSW.AssignedProgram_SuperSetExerciseId
                                        --INNER JOIN AssignedProgram_ProgramDayItemSuperSet_Set  AS pdiss_s ON pdiss_s.AssignedProgram_ProgramDayItemSuperSetWeekId = pdiSSW.Id
                                        WHERE pd.AssignedProgram_ProgramId = @assignedProgramID


                                        SELECT pdiss_s.Id, pdiss_s.Position,pdiss_s.Sets,pdiss_s.Reps, [pdiss_s].[Percent], [pdiss_s].[Weight], pdiss_s.AssignedProgram_ProgramDayItemSuperSetWeekId AS 'ProgramDayItemSuperSetWeekId', [pdiss_s].[Minutes], pdiss_s.Seconds, pdiss_s.Distance,pdiss_s.RepsAchieved,pdiss_s.Other
                                        from AssignedProgram_ProgramDayItemSuperSet_Set AS pdiss_s
                                        inner join #SpeedUpSearchingProgramDayItemSuperSetWeek AS b ON b.pdiSSWId = pdiss_s.AssignedProgram_ProgramDayItemSuperSetWeekId

                                        DROP TABLE #SpeedUpSearchingProgramDayItemSuperSetWeek";

            var superSet_NotesQuery = $@"SELECT AssignedProgram_ProgramDayItemSuperSetId as 'ProgramDayItemSuperSetId' , pdssn.position,pdssn.note,pdssn.id 
                                        FROM AssignedProgram_SuperSetNote  AS pdssn
                                        INNER JOIN  AssignedProgram_ProgramDayItemSuperSet as pdiSS ON pdiSS.Id = pdssn.AssignedProgram_ProgramDayItemSuperSetId
                                        INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.id = pdiSS.AssignedProgram_ProgramDayItemId
                                        INNER JOIN AssignedProgram_ProgramDay AS pd ON pd.id = pdi.AssignedProgram_ProgramDayId
                                        INNER JOIN AssignedProgram_program AS p on p.id = pd.AssignedProgram_ProgramId
                                        Where p.Id = @assignedProgramID ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var thePrograms = sqlConn.Query<DTOs.Program.Program>(programQuery, new { Token = createdUserToken, assignedProgramID = assignedProgramId });
                if (!thePrograms.Any())
                {
                    return new DTOs.Program.Program();
                }
                var targetProgram = thePrograms.First();
                var programDays = sqlConn.Query<DTOs.Program.ProgramDay>(programDayQuery, new { assignedProgramId = assignedProgramId }).ToList();

                targetProgram.Days = programDays;

                var superSet = sqlConn.Query<ProgramDaySuperSet>(SuperSetQuery, new { assignedProgramID = assignedProgramId }).ToList();
                var superSetExercises = sqlConn.Query<ProgramDaySuperSet_Exercise>(SuperSetExercisesQuery, new { assignedProgramID = assignedProgramId }).ToList();
                var SuperSetWeeks = sqlConn.Query<ProgramDaySuperSet_Weeks>(SuperSetWeeksQuery, new { assignedProgramID = assignedProgramId }).ToList();
                var superSet_Sets = sqlConn.Query<ProgramDaySuperSet_Sets>(superSet_SetsQuery, new { assignedProgramID = assignedProgramId }).ToList();
                var superSetNotes = sqlConn.Query<ProgramDaySuperSet_Note>(superSet_NotesQuery, new { assignedProgramID = assignedProgramId }).ToList();
                if (superSet.Any())
                {
                    SuperSetWeeks.ForEach(x =>
                    {
                        x.SetsAndReps = superSet_Sets.Where(y => y.ProgramDayItemSuperSetWeekId == x.ProgramDaySuperSet_WeeksId).ToList();
                    });
                    superSetExercises.ForEach(x =>
                    {
                        x.Weeks = SuperSetWeeks.Where(y => y.ProgramDaySuperSet_ExerciseId == x.programDaySuperSet_ExerciseId).ToList();
                    });
                    superSet.ForEach(x =>
                    {
                        x.Exercises = superSetExercises.Where(y => y.ProgramDaySuperSetId == x.ProgramDaySuperSetId).ToList();
                        x.Notes = superSetNotes.Where(y => y.ProgramDayItemSuperSetId == x.ProgramDaySuperSetId).ToList();
                    });
                }
                GetDisplayWeeksForSnapShotSuperSetNotes(superSet);

                var programVideoReader = sqlConn.ExecuteReader(videoQuery, new { assignedProgramID = assignedProgramId });
                var programVideos = GetProgramDayMovieDataReader(programVideoReader);
                GetDisplayWeeksForSnapShotProgramMovies(programVideos);
                programVideoReader.Close();

                var programMetricsReader = sqlConn.ExecuteReader(metricsQuery, new { assignedProgramID = assignedProgramId });
                var programMetircs = GetProgramDayMetricsFromDataReader(programMetricsReader);
                GetDisplayWeeksForSnapShotProgramMetrics(programMetircs);
                programMetricsReader.Close();


                var programNotesReader = sqlConn.ExecuteReader(notesQuery, new { assignedProgramID = assignedProgramId });
                var programNotes = GetProgramDayNotesFromDataReader(programNotesReader);
                GetDisplayWeeksForSnapShotProgramNotes(programNotes);
                programNotesReader.Close();


                var programSurveysReader = sqlConn.ExecuteReader(surveyQuery, new { assignedProgramID = assignedProgramId });
                var programSurveys = GetProgramDaySurveysFromDataReader(programSurveysReader, createdUserToken);
                GetDisplayWeeksForSnapShotProgramSurvey(programSurveys);
                programSurveysReader.Close();


                if (targetProgram.Days != null)
                {
                    targetProgram.Days.ForEach(x =>
                    {
                        x.Metrics = programMetircs.Where(y => y.ProgramDayId == x.Id).ToList();
                        x.Notes = programNotes.Where(y => y.ProgramDayId == x.Id).ToList();
                        x.Surveys = programSurveys.Where(y => y.ProgramDayId == x.Id).ToList();
                        x.SuperSets = superSet.Where(y => y.ProgramDayId == x.Id).ToList();
                        x.Movies = programVideos.Where(y => y.ProgramDayId == x.Id).ToList();
                        x.Exercises = new List<ProgramDayExercise>();//doing this to make it fucking backwards compatable. the javascript errors out because its expecting this. I am 
                        //not going to re-write the import processif all i have to do is add an empty fucking list.
                    });
                }
                return targetProgram;
            }
        }

        public DTOs.Program.Program GetProgram(int programId, Guid createdUserToken, int programDayId = 0)
        {
            var programDayInsert = programDayId == 0 ? "" : $" AND pd.Id = {programDayId} ";
            var programQuery = " SELECT Id, Name,  WeekCount, CanModify, isDeleted FROM dbo.Programs WHERE Id = @ProgramId " + (createdUserToken == null ? " " : "  AND OrganizationId = (" + ConstantSqlStrings.GetOrganizationIdByToken + ")");
            var programDayQuery = " SELECT Id, Position FROM dbo.ProgramDays WHERE ProgramId = @ProgramId";


            var metricsQuery = $@" SELECT pdiMet.Id as 'pdiMetId', met.Id as 'MetricId', met.[name] AS 'name', pdi.position as 'position', pdi.ProgramDayId AS  'dayId' 
                             FROM ProgramDayItemMetrics AS pdiMet 
                            INNER JOIN  metrics AS met ON met.Id = pdiMet.MetricId   
                             INNER JOIN programDayItems AS pdi ON pdi.Id = pdiMet.ProgramDayItemId 
                             INNER JOIN ProgramDays AS pd on pd.Id = pdi.ProgramDayId 
                             Where pd.ProgramId = @ProgramId {programDayInsert}";

            var videoQuery = $@"SELECT pdim.Id, m.Id,m.[url], m.[name],  pdi.Position,pd.id
                                FROM ProgramDayItemMovies AS pdim
                                INNER JOIN Movies AS m ON  m.Id = pdim.MovieId
                                INNER JOIN ProgramDayItems AS pdi ON pdi.Id = pdim.ProgramDayItemId
                                INNER JOIN ProgramDays AS pd ON  pd.Id = pdi.ProgramDayId
                                where pd.programId = @programId   {programDayInsert}";

            var notesQuery = $@" SELECT pdiNotes.Id as 'pdiNoteId', pdiNotes.[name] as 'name', pdiNotes.Note as 'notes', pdi.position as 'position', pdi.ProgramDayId AS  'dayId' 
                             FROM ProgramDayItemNotes AS pdiNotes 
                             INNER JOIN programDayItems AS pdi ON pdi.Id = pdiNotes.ProgramDayItemId 
                             INNER JOIN ProgramDays AS pd on pd.Id = pdi.ProgramDayId 
                             Where pd.ProgramId = @ProgramId {programDayInsert}";

            var surveyQuery = $@" SELECT pdiSurveys.Id  as 'pdiSurveyId', pdiSurveys.SurveyId 'surveyId',pdi.position as 'position', pdi.ProgramDayId AS  'dayId' 
                             FROM ProgramDayItemSurveys AS pdiSurveys 
                             INNER JOIN programDayItems AS pdi ON pdi.Id = pdiSurveys.ProgramDayItemId 
                             INNER JOIN ProgramDays AS pd on pd.Id = pdi.ProgramDayId 
                             Where pd.ProgramId = @ProgramId {programDayInsert}";

            var exerciseQuery = $@" SELECT pdiE.Id 'pdiEId' ,programDayId as 'programDayId', pdiE.ExerciseId as 'exerciseId',pdi.Position as 'position',pdiE.workoutId as 'workoutID' 
                                 FROM ProgramDayItemExercises AS pdiE 
                                 INNER JOIN  ProgramDayItems AS pdi ON pdi.id = pdie.ProgramDayItemId 
                                 INNER JOIN Exercises AS e ON PDIE.ExerciseId = e.Id 
                                 INNER JOIN ProgramDays AS pd on pd.Id = pdi.ProgramDayId 
                                 Where pd.ProgramId = @ProgramId {programDayInsert}";

            var SuperSetQuery = $@" SELECT pdiSS.Id 'ProgramDaySuperSetId',pd.Id as 'ProgramDayId', pdi.position as 'Position', SuperSetDisplayTitle as 'SuperSetDisplayTitle'
                                    FROM ProgramDayItemSuperSets as pdiSS
                                    INNER JOIN ProgramDayItems AS pdi ON pdi.id = pdiSS.ProgramDayItemId
                                    INNER JOIN ProgramDays AS pd ON pd.id = pdi.ProgramDayId
                                    INNER JOIN programs AS p on p.id = pd.ProgramId 
                                    Where p.Id = @ProgramId {programDayInsert}";

            var SuperSetExercisesQuery = $@"SELECT sse.ExerciseId,sse.Position, e.[name],e.notes,pdiss.id as 'ProgramDaySuperSetId', sse.Id as 'programDaySuperSet_ExerciseId', sse.Rest, sse.ShowWeight as 'ShowWeight'
                                        FROM ProgramDayItemSuperSets as pdiSS
                                        INNER JOIN ProgramDayItems AS pdi ON pdi.id = pdiSS.ProgramDayItemId
                                        INNER JOIN SuperSetExercises  AS sse ON sse.ProgramDayItemSuperSetId = pdiSS.Id
                                        INNER JOIN Exercises AS e ON e.Id = sse.ExerciseID
                                        INNER JOIN ProgramDays AS pd ON pd.id = pdi.ProgramDayId
                                        INNER JOIN programs AS p on p.id = pd.ProgramId 
                                         Where p.Id = @ProgramId {programDayInsert}";

            var SuperSetWeeksQuery = $@"SELECT pdiSSW.id as 'ProgramDaySuperSet_WeeksId' , pdiSSw.Position as 'Position',pdiSSW.SuperSetExerciseId as 'ProgramDaySuperSet_ExerciseId'
                                        FROM ProgramDayItemSuperSets as pdiSS
                                        INNER JOIN ProgramDayItems AS pdi ON pdi.id = pdiSS.ProgramDayItemId
                                        INNER JOIN SuperSetExercises  AS sse ON sse.ProgramDayItemSuperSetId = pdiSS.Id
                                        INNER JOIN Exercises AS e ON e.Id = sse.ExerciseID
                                        INNER JOIN ProgramDayItemSuperSetWeeks AS pdiSSW on sse.Id = pdiSSW.SuperSetExerciseId
                                        INNER JOIN ProgramDays AS pd ON pd.id = pdi.ProgramDayId
                                        INNER JOIN programs AS p on p.id = pd.ProgramId 
                                         Where p.Id = @ProgramId {programDayInsert}";

            var superSet_SetsQuery = $@"SELECT  pdiss_s.*
                                        FROM ProgramDayItemSuperSets as pdiSS
                                        INNER JOIN ProgramDayItems AS pdi ON pdi.id = pdiSS.ProgramDayItemId
                                        INNER JOIN SuperSetExercises  AS sse ON sse.ProgramDayItemSuperSetId = pdiSS.Id
                                        INNER JOIN Exercises AS e ON e.Id = sse.ExerciseID
                                        INNER JOIN ProgramDayItemSuperSetWeeks AS pdiSSW on sse.Id = pdiSSW.SuperSetExerciseId
                                        INNER JOIN ProgramDayItemSuperSet_Set  AS pdiss_s ON pdiss_s.ProgramDayItemSuperSetWeekId = pdiSSW.Id
                                        INNER JOIN ProgramDays AS pd ON pd.id = pdi.ProgramDayId
                                        INNER JOIN programs AS p on p.id = pd.ProgramId 
                                        Where p.Id = @ProgramId {programDayInsert}";

            var superSet_NotesQuery = $@"SELECT ProgramDayItemSuperSetId , pdssn.position,pdssn.note,pdssn.id 
                                        FROM SuperSetNotes  AS pdssn
                                        INNER JOIN  ProgramDayItemSuperSets as pdiSS ON pdiSS.Id = pdssn.ProgramDayItemSuperSetId
                                        INNER JOIN ProgramDayItems AS pdi ON pdi.id = pdiSS.ProgramDayItemId
                                        INNER JOIN ProgramDays AS pd ON pd.id = pdi.ProgramDayId
                                        INNER JOIN programs AS p on p.id = pd.ProgramId
                                        Where p.Id = @ProgramId { programDayInsert} ";




            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var thePrograms = sqlConn.Query<DTOs.Program.Program>(programQuery, new { Token = createdUserToken, ProgramId = programId });
                if (!thePrograms.Any())
                {
                    return new DTOs.Program.Program();
                }
                var targetProgram = thePrograms.First();
                var programDays = sqlConn.Query<DTOs.Program.ProgramDay>(programDayQuery, new { ProgramId = targetProgram.Id }).ToList();

                targetProgram.Days = programDays;

                var superSet = sqlConn.Query<ProgramDaySuperSet>(SuperSetQuery, new { ProgramId = programId }).ToList();
                var superSetExercises = sqlConn.Query<ProgramDaySuperSet_Exercise>(SuperSetExercisesQuery, new { ProgramId = programId }).ToList();
                var SuperSetWeeks = sqlConn.Query<ProgramDaySuperSet_Weeks>(SuperSetWeeksQuery, new { ProgramId = programId }).ToList();
                var superSet_Sets = sqlConn.Query<ProgramDaySuperSet_Sets>(superSet_SetsQuery, new { ProgramId = programId }).ToList();
                var superSetNotes = sqlConn.Query<ProgramDaySuperSet_Note>(superSet_NotesQuery, new { ProgramId = programId }).ToList();
                if (superSet.Any())
                {
                    SuperSetWeeks.ForEach(x =>
                    {
                        x.SetsAndReps = superSet_Sets.Where(y => y.ProgramDayItemSuperSetWeekId == x.ProgramDaySuperSet_WeeksId).ToList();
                    });
                    superSetExercises.ForEach(x =>
                    {
                        x.Weeks = SuperSetWeeks.Where(y => y.ProgramDaySuperSet_ExerciseId == x.programDaySuperSet_ExerciseId).ToList();
                    });
                    superSet.ForEach(x =>
                    {
                        x.Exercises = superSetExercises.Where(y => y.ProgramDaySuperSetId == x.ProgramDaySuperSetId).ToList();
                        x.Notes = superSetNotes.Where(y => y.ProgramDayItemSuperSetId == x.ProgramDaySuperSetId).ToList();
                    });
                }
                GetDisplayWeeksForSuperSetNotes(superSet);

                var programVideoReader = sqlConn.ExecuteReader(videoQuery, new { ProgramId = targetProgram.Id });
                var programVideos = GetProgramDayMovieDataReader(programVideoReader);
                GetDisplayWeeksForProgramMovies(programVideos);
                programVideoReader.Close();

                var programMetricsReader = sqlConn.ExecuteReader(metricsQuery, new { ProgramId = targetProgram.Id });
                var programMetircs = GetProgramDayMetricsFromDataReader(programMetricsReader);
                GetDisplayWeeksForProgramMetrics(programMetircs);
                programMetricsReader.Close();


                var programNotesReader = sqlConn.ExecuteReader(notesQuery, new { ProgramId = targetProgram.Id });
                var programNotes = GetProgramDayNotesFromDataReader(programNotesReader);
                GetDisplayWeeksForProgramNotes(programNotes);
                programNotesReader.Close();


                var programSurveysReader = sqlConn.ExecuteReader(surveyQuery, new { ProgramId = targetProgram.Id });
                var programSurveys = GetProgramDaySurveysFromDataReader(programSurveysReader, createdUserToken);
                GetDisplayWeeksForProgramSurvey(programSurveys);
                programSurveysReader.Close();


                var programExerciseReader = sqlConn.ExecuteReader(exerciseQuery, new { ProgramId = targetProgram.Id });
                var programExercise = GetProgramDayExercises(programExerciseReader, createdUserToken);
                programExerciseReader.Close();

                if (targetProgram.Days != null)
                {
                    targetProgram.Days.ForEach(x =>
                    {
                        x.Metrics = programMetircs.Where(y => y.ProgramDayId == x.Id).ToList();
                        x.Notes = programNotes.Where(y => y.ProgramDayId == x.Id).ToList();
                        x.Surveys = programSurveys.Where(y => y.ProgramDayId == x.Id).ToList();
                        x.Exercises = programExercise.Where(y => y.ProgramDayId == x.Id).ToList();
                        x.SuperSets = superSet.Where(y => y.ProgramDayId == x.Id).ToList();
                        x.Movies = programVideos.Where(y => y.ProgramDayId == x.Id).ToList();
                    });
                }
                return targetProgram;
            }
        }

        public List<ProgramDaySuperSet> GetProgramDaySuperSetFromDataReader(IDataReader reader)
        {
            var ret = new List<ProgramDaySuperSet>();
            while (reader.Read())
            {
                ret.Add(new ProgramDaySuperSet()
                {
                    Id = reader.GetInt32(0),
                    ProgramDayId = reader.GetInt32(1),
                    Position = reader.GetInt32(2)
                });
            }
            return ret;
        }
        public int InsertSuperSetNotes(string note, List<int> displayWeeks, int position, int programDayItemSuperSetId)
        {
            var insertStatement = @"
                                    INSERT INTO [dbo].[SuperSetNotes]
                                    ([Note],[ProgramDayItemSuperSetId],[Position])
                                    VALUES
                                    (@Note, @ProgramDayItemSuperSetId, @position); SELECT SCOPE_IDENTITY();";

            var instertDisplayWeeks = @"INSERT INTO SuperSetNoteDisplayWeeks
                                       ([SuperSetNoteId], [DisplayWeek])
                                        VALUES 
                                       (@SuperSetNoteId, @DisplayWeek)";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var noteId = sqlConn.ExecuteScalar<int>(insertStatement, new { Note = note, ProgramDayItemSuperSetId = programDayItemSuperSetId, Position = position });
                displayWeeks.ForEach(x =>
                {
                    sqlConn.Execute(instertDisplayWeeks, new { SuperSetNoteId = noteId, DisplayWeek = x });
                });
                return noteId;
            }
        }

        public int InsertSuperSet(int programDayItemId, string title)
        {
            var insertStatement = @"
                                    INSERT INTO [dbo].[ProgramDayItemSuperSets]
                                    ([ProgramDayItemId],SuperSetDisplayTitle)
                                    VALUES
                                    (@ProgramDayItemId, @Title); SELECT SCOPE_IDENTITY();";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(insertStatement, new { ProgramDayItemId = programDayItemId, Title = title });
            }
        }

        public int InsertSuperSetExercise(Models.Program.SuperSetExercise ssExercises)
        {
            var insertStatement = @"INSERT INTO[dbo].[SuperSetExercises]
                                   ([ProgramDayItemSuperSetId]
                                   ,[Position]
                                   ,[ExerciseId]
                                    ,[Rest]
                                    ,[ShowWeight])
                                    VALUES
                                   (@ProgramDayItemSuperSetId
                                   ,@Position
                                   ,@ExerciseId
                                    ,@Rest
                                    ,@ShowWeight);  SELECT SCOPE_IDENTITY();";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(insertStatement, new
                {
                    ProgramDayItemSuperSetId = ssExercises.ProgramDayItemSuperSetId,
                    Position = ssExercises.Position,
                    ExerciseId = ssExercises.ExerciseId,
                    Rest = ssExercises.Rest,
                    ShowWeight = ssExercises.ShowWeight
                });
            }
        }

        public int InsertSuperSetWeek(Models.Program.ProgramDayItemSuperSetWeek ssWeek)
        {
            var insertStatement = @"INSERT INTO[dbo].[ProgramDayItemSuperSetWeeks]
                                   ([Position]
                                   ,[SuperSetExerciseId])
                                    VALUES
                                   (@Position
                                   ,@ExerciseId);  SELECT SCOPE_IDENTITY();";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(insertStatement, new { Position = ssWeek.Position, ExerciseId = ssWeek.SuperSetExerciseId });
            }
        }

        public void InsertSuperSet_sets(List<Models.Program.ProgramDayItemSuperSet_Set> sets, int ProgramDayItemSuperSetWeekId)
        {
            var insertSets = @" ;INSERT INTO dbo.ProgramDayItemSuperSet_set 
                          ([Position], 
                          [Sets], 
                          [Reps], 
                          [Percent], 
                          [Weight], 
                          ProgramDayItemSuperSetWeekId,
                          [Minutes],
                          [Seconds],
                          [Distance],
                          [RepsAchieved],
                          [Other]) 
                          VALUES 
                          (@Position, 
                          @Sets, 
                          @Reps,
                          @Percent, 
                          @Weight, 
                          @ProgramDayItemSuperSetWeekId,
                          @Minutes,
                          @Seconds,
                          @Distance,
                          @RepsAchieved,
                          @Other) ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                foreach (var set in sets)
                {
                    if (set.Percent == 0 && set.Reps == 0 && set.Sets == 0 && set.Weight == 0 && set.Minutes == 0 && set.Seconds == 0 && String.IsNullOrEmpty(set.Distance) && string.IsNullOrEmpty(set.Other))
                    {
                        continue;//do not save empty sets/reps}
                    }
                    sqlConn.Execute(insertSets,
                        new
                        {
                            Position = set.Position,
                            Sets = set.Sets,
                            Reps = set.Reps,
                            Percent = set.Percent,
                            Weight = set.Weight,
                            ProgramDayItemSuperSetWeekId = ProgramDayItemSuperSetWeekId,
                            Minutes = set.Minutes,
                            Seconds = set.Seconds,
                            Distance = set.Distance,
                            RepsAchieved = set.RepsAchieved,
                            Other = set.Other
                        });
                }
            }
        }

        private List<ProgramDayExercise> GetProgramDayExercises(IDataReader reader, Guid userToken)
        {
            var ret = new List<ProgramDayExercise>();


            while (reader.Read())
            {
                ret.Add(new ProgramDayExercise()
                {
                    Id = reader.GetInt32(0),
                    ProgramDayId = reader.GetInt32(1),
                    Exercise = _exerciseRepo.GetExercise(reader.GetInt32(2), userToken),
                    Position = reader.GetInt32(3),
                    Workout = _workoutRepo.GetProgramWorkout(reader.GetInt32(0)),
                    Weeks = GetAllProgramWeeksForAProgarmDayItemExercise(reader.GetInt32(0))
                });
            }
            return ret;
        }

        private List<DTOs.AthleteAssignedPrograms.AssignedProgramDaySurvey> GetAssignedProgramDaySurveysFromDataReader(IDataReader reader, int athleteId, int assignedProgramId)
        {
            var ret = new List<DTOs.AthleteAssignedPrograms.AssignedProgramDaySurvey>();

            while (reader.Read())
            {
                ret.Add(new DTOs.AthleteAssignedPrograms.AssignedProgramDaySurvey()
                {
                    ProgramDayItemSurveyId = reader.GetInt32(0),
                    SurveyId = reader.GetInt32(1),
                    Position = reader.GetInt32(2),
                    Questions = GetSnapShotQuestions(assignedProgramId, athleteId, reader.GetInt32(1), reader.GetInt32(0)),
                    ProgramDayId = reader.GetInt32(3)
                });
            }
            return ret;
        }
        private List<DTOs.AthleteAssignedPrograms.AssignedSuperSetNote> GetAssignedProgramDaySuperSetNotes(IDataReader reader, int athleteId, int assignedProgramId)
        {
            var ret = new List<DTOs.AthleteAssignedPrograms.AssignedSuperSetNote>();

            while (reader.Read())
            {
                ret.Add(new DTOs.AthleteAssignedPrograms.AssignedSuperSetNote()
                {
                    Id = reader.GetInt32(0),
                    Note = reader.GetString(1),
                    Position = reader.GetInt32(2),
                    ProgramDayItemSuperSetId = reader.GetInt32(3)

                });
            }
            return ret;
        }

        private List<ProgramDaySurvey> GetProgramDaySurveysFromDataReader(IDataReader reader, Guid userToken)
        {
            var ret = new List<ProgramDaySurvey>();

            while (reader.Read())
            {
                ret.Add(new ProgramDaySurvey()
                {
                    Id = reader.GetInt32(0),
                    SurveyId = reader.GetInt32(1),
                    Position = reader.GetInt32(2),
                    Questions = _surveyRepo.GetAllSurveyQuestions(reader.GetInt32(1), userToken),
                    ProgramDayId = reader.GetInt32(3)
                });
            }
            return ret;
        }

        private List<ProgramDayNotes> GetProgramDayNotesFromDataReader(IDataReader reader)
        {
            var ret = new List<ProgramDayNotes>();

            while (reader.Read())
            {
                ret.Add(new ProgramDayNotes()
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Note = reader.GetString(2),
                    Position = reader.GetInt32(3),
                    ProgramDayId = reader.GetInt32(4)
                });
            }
            return ret;
        }

        private List<ProgramDayMovie> GetProgramDayMovieDataReader(IDataReader reader)
        {
            var ret = new List<ProgramDayMovie>();
            while (reader.Read())
            {
                ret.Add(new ProgramDayMovie()
                {
                    Id = reader.GetInt32(0),
                    Position = reader.GetInt32(4),
                    ProgramDayId = reader.GetInt32(5),
                    Video = new m.MultiMedia.Movie()
                    {
                        Id = reader.GetInt32(1),
                        URL = reader.GetString(2),
                        Name = reader.GetString(3)
                    }
                });
            }
            return ret;
        }
        private List<ProgramDayMetrics> GetProgramDayMetricsFromDataReader(IDataReader reader)
        {
            var ret = new List<ProgramDayMetrics>();
            while (reader.Read())
            {
                ret.Add(new ProgramDayMetrics()
                {
                    Id = reader.GetInt32(0),
                    Metric = new Metric()
                    {
                        Id = reader.GetInt32(1),
                        Name = reader.GetString(2)
                    },
                    Position = reader.GetInt32(3),
                    ProgramDayId = reader.GetInt32(4)
                });
            }
            return ret;
        }
        public List<int> GetAllProgramIdsWithAdvancedFeaturesTurnedOn(Guid createdUserToken)
        {
            //SO WHAT IS WITH THIS BULLSHIT HERE!!!??/
            //apparntly the query fucking bombed when I had 4 OR statements. Commenting out one OR
            //and putting all the data into a temp table made it run fast. I dont know why 4 OR statements
            //would crush sql server but it did. If you are smart enough maybe you can research and figure out.
            var getString = $@"
                                SELECT DISTINCT  p.id
                                                 ,pdiss_s.[Minutes]
                                                 ,pdiss_s.[seconds] 
                                                 ,pdiss_s.Distance 
                                                 ,pdiss_s.RepsAchieved 
						        INTO #frank 
                                FROM Programs AS p
                                INNER JOIN ProgramDays AS pd ON pd.ProgramId = p.Id
                                INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                                INNER JOIN ProgramDayItemSuperSets AS pdiss ON pdiss.ProgramDayItemId = pdi.Id
                                INNER JOIN SuperSetExercises AS sse ON sse.ProgramDayItemSuperSetId = pdiss.Id
                                INNER JOIN ProgramDayItemSuperSetWeeks AS pdissw ON pdissw.SuperSetExerciseId = sse.Id
                                INNER JOIN ProgramDayItemSuperSet_Set AS pdiss_s ON pdiss_s.ProgramDayItemSuperSetWeekId = pdissw.Id
                                WHERE 
								 pdiss_s.[Minutes] IS NOT NULL
                                OR pdiss_s.[seconds] IS NOT NULL
                                OR pdiss_s.Distance IS NOT NULL
                                --OR pdiss_s.RepsAchieved IS NOT NULL
                                AND  p.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})

                                SELECT id FROM #FRANK AS f
                                WHERE f.Minutes is not null AND F.Seconds is not null AND f.Distance IS NULL AND f.RepsAchieved IS NOT NULL

                                DROP TABLE #FRANK
                        ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<int>(getString, new { Token = createdUserToken }).ToList();
            }
        }
        public List<m.Program.Program> GetAllPrograms(Guid createdUserToken)
        {

            var getString = $@" SELECT Id, Name, WeekCount, CanModify, isDeleted, (select count(id) from ProgramDays where programId = p.id) as DayCount from Programs as p 
                                WHERE  p.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<m.Program.Program>(getString, new { Token = createdUserToken }).ToList();
            }
        }
        public List<ProgramWithTagsDTO> GetAllProgramTagMappings(Guid userToken)
        {
            var tagMappings = $"SELECT t.ProgramId ,t.TagId, ta.Name FROM TagsToPrograms AS t INNER JOIN ProgramTags AS ta ON ta.Id = t.TagId INNER JOIN Programs AS E on t.ProgramId = e.Id WHERE e.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            var tagMappingDTOs = new List<ProgramWithTagsDTO>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var reader = sqlConn.ExecuteReader(tagMappings, new { Token = userToken });
                while (reader.Read())
                {
                    var targetDtoMapping = tagMappingDTOs.FirstOrDefault(x => x.ProgramId == reader.GetInt32(0));
                    var newTagDTO = new TagDTO() { Id = reader.GetInt32(1), Name = reader.GetString(2) };
                    if (targetDtoMapping == null)
                    {

                        tagMappingDTOs.Add(new ProgramWithTagsDTO() { ProgramId = reader.GetInt32(0), Tags = new List<TagDTO>() { newTagDTO } });
                    }
                    else
                    {
                        targetDtoMapping.Tags.Add(newTagDTO);
                    }
                }
            }
            return tagMappingDTOs;
        }

        public List<AthleteHomePagePastProgram> GetAllPastPrograms(int athleteId)
        {
            var getString = @"SELECT * FROM (
                        select p.[Name] as 'ProgramName', p.Id as 'ProgramId' , aph.AssignedProgramId as 'AssignedProgramId', aph.StartDate as 'ProgramStartDate' , aph.EndDate as 'ProgramEndDate',0 as IsSnapShot
                        from AthleteProgramHistories as aph
                        INNER JOIN AssignedPrograms AS ap ON ap.Id = aph.AssignedProgramId
                        INNER JOIN programs as p on p.id = ap.ProgramId 
                        where aph.athleteId = @AthleteId 
                        UNION
                        select p.[Name] as 'ProgramName', p.Id as 'ProgramId' , aph.AssignedProgram_ProgramId as 'AssignedProgramId', aph.AssignedDate as 'ProgramStartDate' , aph.CompletedDate as 'ProgramEndDate',1 as IsSnapShot
                        from AssignedProgram_AssignedProgramHistory as aph
                        INNER JOIN AssignedProgram_Program as p on p.id = aph.AssignedProgram_ProgramId 
                        where aph.athleteId = @AthleteId ) AS a
                        ORDER BY a.AssignedProgramId desc
                        ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<AthleteHomePagePastProgram>(getString, new { AthleteId = athleteId }).ToList();
            }
        }

        public int DoesProgramExist(string programName, Guid createdUserToken)
        {
            var getString = $@"SELECT isnull(Id,0) FROM programs WHERE name = @Name AND organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.QueryFirstOrDefault<int>(getString, new { Name = programName, Token = createdUserToken });
            }
        }
        public void DontFuckingDoThis_Delete_All_Information_About_A_Program(int programId)
        {
            //this will not delete a program that has been assigned and a matter of fact this will not run if the program has been assigned
            var I_Said_No = $@" IF NOT EXISTS (SELECT  1  
                                                FROM AssignedPrograms AS ap
                                                INNER JOIN programs AS p on p.Id = ap.ProgramId
                                                WHERE p.Id = @programId
                                                )
    BEGIN

                       DELETE FROM SuperSetNoteDisplayWeeks
                                WHERE SuperSetNoteId 
                                IN (
                                SELECT ssn.Id   FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDayItemSuperSets AS pdiss ON pdiss.ProgramDayItemId = pdi.Id
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                INNER JOIN SuperSetNotes AS ssn ON ssn.ProgramDayItemSuperSetId = pdiss.Id
                                WHERE p.Id = @programId)

                              DELETE FROM SuperSetNotes where ProgramDayItemSuperSetId
                            IN (
                            SELECT pdiss.Id  FROM ProgramDayItems AS pdi
                            INNER JOIN ProgramDayItemSuperSets AS pdiss ON pdiss.ProgramDayItemId = pdi.Id
                            INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                            INNER JOIN Programs AS p ON p.id = pd.ProgramId
                            WHERE p.Id = @programId)

                                DELETE FROM ProgramDayItemSuperSet_Set
                                WHERE ProgramDayItemSuperSetWeekId  IN (
                                SELECT pdissw.id  FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDayItemSuperSets AS pdiss ON pdiss.ProgramDayItemId = pdi.Id
                                INNER JOIN SuperSetExercises  AS sse ON sse.ProgramDayItemSuperSetId = pdiss.Id
                                INNER JOIN  ProgramDayItemSuperSetWeeks AS pdissw ON pdissw.SuperSetExerciseId = sse.Id
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE  p.Id = @programId)


                                DELETE FROM ProgramDayItemSuperSetWeeks
                                WHERE SuperSetExerciseId  IN (
                                SELECT sse.Id  FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDayItemSuperSets AS pdiss ON pdiss.ProgramDayItemId = pdi.Id
                                INNER JOIN SuperSetExercises  AS sse ON sse.ProgramDayItemSuperSetId = pdiss.Id
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE p.Id = @programId)


                                DELETE FROM SuperSetExercises
                                WHERE ProgramDayItemSuperSetId  IN (
                                SELECT pdiss.Id  FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDayItemSuperSets AS pdiss ON pdiss.ProgramDayItemId = pdi.Id
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE p.Id = @programId)

                                DELETE FROM ProgramDayItemSuperSets
                                WHERE ProgramDayItemId  IN (
                                SELECT pdi.Id  FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE  p.Id = @programId)

                                DELETE FROM ProgramSets
                                WHERE ParentProgramWeekId  IN(
                                SELECT pw.Id
                                FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDayItemExercises AS pdie ON pdie.ProgramDayItemId = pdi.Id
                                INNER JOIN programweeks AS pw on pw.ProgramDayItemExerciseId = pdie.Id
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE  p.Id = @programId)

                                DELETE FROM ProgramWeeks
                                WHERE ProgramDayItemExerciseId IN(
                                SELECT pdie.Id
                                FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDayItemExercises AS pdie ON pdie.ProgramDayItemId = pdi.Id
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE p.Id = @programId)

                                DELETE FROM ProgramDayItemExercises 
                                WHERE ProgramDayItemId IN (
                                SELECT pdi.Id
                                FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE p.Id = @programId)

                                DELETE FROM SurveyDisplayWeeks
                                WHERE ProgramDayItemSurveyId IN (
                                SELECT id FROM  ProgramDayItemSurveys 
                                WHERE ProgramDayItemId IN (
                                SELECT pdi.Id
                                FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE  p.Id = @programId))


                                DELETE FROM ProgramDayItemSurveys 
                                WHERE ProgramDayItemId IN (
                                SELECT pdi.Id
                                FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE  p.Id = @programId)

                                DELETE FROM  NoteDisplayWeeks 
                                WHERE ProgramDayItemNoteId IN
                                (SELECT id from  ProgramDayItemNotes
                                WHERE ProgramDayItemId IN (
                                SELECT pdi.Id
                                FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE  p.Id = @programId))

                                DELETE FROM ProgramDayItemNotes
                                WHERE ProgramDayItemId IN (
                                SELECT pdi.Id
                                FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE  p.Id = @programId)

                                DELETE FROM MetricDisplayWeeks
                                WHERE ProgramDayItemMetricId IN (
                                SELECT id FROM  ProgramDayItemMetrics
                                WHERE ProgramDayItemId IN (
                                SELECT pdi.Id
                                FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE  p.Id = @programId))

                                DELETE FROM ProgramDayItemMetrics
                                WHERE ProgramDayItemId IN (
                                SELECT pdi.Id
                                FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE p.Id = @programId)

                                DELETE FROM MovieDisplayWeeks
                                WHERE ProgramDayItemMovieId IN (
                                SELECT id FROM  programdayitemMovies
                                WHERE ProgramDayItemId IN (
                                SELECT pdi.Id
                                FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE  p.Id = @programId))

                                DELETE FROM ProgramDayItemMovies
                                WHERE ProgramDayItemId IN (
                                SELECT pdi.Id
                                FROM ProgramDayItems AS pdi
                                INNER JOIN ProgramDays AS pd ON pdi.ProgramDayId = PD.ID
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE p.Id = @programId)


                                DELETE FROM ProgramDayItems
                                WHERE ProgramDayId  IN (
                                SELECT pd.Id  FROM  ProgramDays AS pd 
                                INNER JOIN Programs AS p ON p.id = pd.ProgramId
                                WHERE  p.Id = @programId)

                                DELETE FROM ProgramDays
                                WHERE ProgramId  = @ProgramId

                                DELETE FROM TagsToPrograms 
                                WHERE ProgramId = @ProgramId

                                DELETE FROM Programs
                                WHERE Id  = @ProgramId

END";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(I_Said_No, new { ProgramId = programId });
            }

        }
        public List<int> GetAllAthleteIdsForAssignedProgram(int assignedProgramId, Guid createdUserGuid)
        {
            var query = $@"SELECT Id FROM Athletes WHERE assignedProgramId = {assignedProgramId} AND organizationId = ({ ConstantSqlStrings.GetOrganizationIdByToken}) AND isDeleted = 0 ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<int>(query, new { Token = createdUserGuid }).ToList();
            }
        }
        public int GetLatestAssignedProgramID(int programId)
        {
            //gets the last assigned program. They cannot print out historically assigned programs fuck em
            var query = $@"SELECT TOP 1 ISNULL(id,0) FROM AssignedPrograms WHERE ProgramId = @ProgramId ORDER BY assignedDate desc ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(query, new { ProgramId = programId });
            }
        }

        public void MarkAllDaysAsCompleteWithin24Hours(DateTime utc) { }

        public async Task<Tuple<int, int>> CreateProgramSnapShot(int programId, int athleteId, Guid createdUserToken)
        {
            var targetProgram = this.GetProgram(programId, createdUserToken);

            var createSnapShotProgramString = $@" INSERT INTO dbo.AssignedProgram_Program
                                 ([Name],[CreatedUserId], [WeekCount], OrganizationId, DayCount)
                                 VALUES 
                                  (@ProgramName,({ConstantSqlStrings.GetUserIdFromToken}), @weekCount,({ConstantSqlStrings.GetOrganizationIdByToken}), @DayCount); SELECT SCOPE_IDENTITY();  ";

            int assignedProgram_ProgramId = 0;

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                assignedProgram_ProgramId = await sqlConn.ExecuteScalarAsync<int>
                    (createSnapShotProgramString, new { Token = createdUserToken, ProgramName = targetProgram.Name, WeekCount = targetProgram.WeekCount, @DayCount = targetProgram.Days.Count });
            }

            //make sure all positions start at 0 and increment up
            var programAsc = targetProgram.Days.OrderBy(x => x.Position).ToList();

            for (int i = 0; i < programAsc.Count; i++)
            {
                programAsc[i].Position = i;
            }
            foreach (var x in targetProgram.Days)
            {
                var createString = @" INSERT INTO dbo.AssignedProgram_ProgramDay
                                 ([AssignedProgram_ProgramId],[Position])
                                 VALUES 
                                  (@ProgramId,@position); SELECT SCOPE_IDENTITY();  ";
                int assignedProgram_ProgramDayId = 0;
                using (var sqlConn = new SqlConnection(ConnectionString))
                {
                    assignedProgram_ProgramDayId = await sqlConn.ExecuteScalarAsync<int>(createString, new { ProgramId = assignedProgram_ProgramId, Position = x.Position });
                }
                if (x.Notes != null)
                {
                    var notesTask = x.Notes.Select(n => this.AddNoteToAssignedProgram(n.Note, n.Title, n.DisplayWeeks, n.Position, assignedProgram_ProgramDayId));
                    await Task.WhenAll(notesTask);
                }
                if (x.Movies != null)
                {
                    var movieTasks = x.Movies.Select(m => this.AddMovieToAssignedProgram(m.Video.Id, m.DisplayWeeks, m.Position, assignedProgram_ProgramDayId));
                    await Task.WhenAll(movieTasks);
                }
                if (x.Metrics != null)
                {
                    var metricTasks = x.Metrics.Select(m => this.AddMetricToAssignedProgram(m.Metric.Id, m.DisplayWeeks, m.Position, assignedProgram_ProgramDayId));
                    await Task.WhenAll(metricTasks);
                }
                if (x.Surveys != null)
                {
                    var surveyTasks = x.Surveys.Select(s => AddSurveyToAssignedProgram(s.SurveyId, s.DisplayWeeks, s.Position, assignedProgram_ProgramDayId));
                    await Task.WhenAll(surveyTasks);
                }
                if (x.SuperSets != null)
                {
                    var superSetTask = x.SuperSets.Select(s => CreateSuperSetsforSnapshots(s, assignedProgram_ProgramDayId, athleteId));
                    await Task.WhenAll(superSetTask);
                }
            }
            return new Tuple<int, int>(athleteId, assignedProgram_ProgramId);
        }
        public async Task CreateSuperSetsforSnapshots(ProgramDaySuperSet target, int assignedProgram_ProgramDayId, int athleteId)
        {
            var dayItemId = await CreateAssignedProgram_ProgramDayItem(target.Position, ProgramDayItemEnum.SuperSet, assignedProgram_ProgramDayId);
            var supserSetId = await AddSuperSetToAssignedProgram(dayItemId, target.SuperSetDisplayTitle);
            var superSetNotesTasks = target.Notes.Select(async n => await AddSuperSetNotesToAssignedProgram(n.Note, n.DisplayWeeks, n.Position, supserSetId));
            await Task.WhenAll(superSetNotesTasks);
            var exercisesTasks = target.Exercises.Select(x => AddSuperSetExerciseForSnapShot(x, supserSetId, athleteId));
            await Task.WhenAll(exercisesTasks);
        }
        public async Task AddSuperSetExerciseForSnapShot(ProgramDaySuperSet_Exercise e, int supserSetId, int athleteId)
        {

            var superSetExerciseId = await AddSuperSetExerciseToAssignedProgram(new SuperSetExercise()
            {
                ExerciseId = e.ExerciseId,
                Position = e.Position,
                ProgramDayItemSuperSetId = supserSetId,
                Rest = e.Rest,
                ShowWeight = e.ShowWeight
            });
            foreach (var w in e.Weeks)
            {
                var superSetWeekId = await AddSuperSetWeekToAssignedProgram(new ProgramDayItemSuperSetWeek() { Position = w.Position, SuperSetExerciseId = superSetExerciseId });
                var convertedSuperSet_Sets = w.SetsAndReps.Where(l =>
                l.Percent != null ||
                l.Sets != null ||
                l.Reps != null ||
                l.Weight != null ||
                l.Minutes != null ||
                l.Seconds != null ||
                !String.IsNullOrEmpty(l.Distance) ||
                l.RepsAchieved != null ||
                l.Other != null).Select(sr => new ProgramDayItemSuperSet_Set()
                {
                    Percent = sr.Percent,
                    Position = sr.Position,
                    Reps = sr.Reps,
                    Sets = sr.Sets,
                    Weight = sr.Weight,
                    ProgramDayItemSuperSetWeekId = superSetWeekId,
                    Distance = sr.Distance,
                    Minutes = sr.Minutes,
                    Seconds = sr.Seconds,
                    Other = sr.Other,
                    RepsAchieved = sr.RepsAchieved
                }).ToList();

                if (convertedSuperSet_Sets != null && convertedSuperSet_Sets.Any())
                {
                    for (int i = 0; i < convertedSuperSet_Sets.Count(); i++)
                    {
                        convertedSuperSet_Sets[i].Position = i;
                    }
                    try
                    {
                        await AddSuperSet_SetsToAssignedProgram(convertedSuperSet_Sets, superSetWeekId, e.ExerciseId, athleteId);
                    }
                    catch (Exception rex)
                    {
                        var r = rex;
                        throw;
                    }

                }
            }
        }
        public async Task<int> CreateAssignedProgram_ProgramDayItem(int positionId, Models.Enums.ProgramDayItemEnum programType, int AssignedProgram_ProgramDayId)
        {
            var createString = @" INSERT INTO dbo.AssignedProgram_ProgramDayItem
                                 ([Position],[AssignedProgram_ProgramDayId],[ItemEnum])
                                 VALUES 
                                  (@position, @AssignedProgram_ProgramDayId,@ItemType); SELECT SCOPE_IDENTITY();  ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return await sqlConn.ExecuteScalarAsync<int>(createString, new { Position = positionId, ItemType = ((int)programType), AssignedProgram_ProgramDayId = AssignedProgram_ProgramDayId });
            }
        }
        public async Task AddMovieToAssignedProgram(int movieId, List<int> weeksToDisplay, int position, int assignedProgramDayId)
        {
            var AssignedProgram_ProgramDayItemId = await CreateAssignedProgram_ProgramDayItem(position, Models.Enums.ProgramDayItemEnum.Video, assignedProgramDayId);

            var createString = @" INSERT INTO dbo.AssignedProgram_ProgramDayItemMovie
                                 ([MovieId],[AssignedProgram_ProgramDayItemId])
                                 VALUES 
                                  (@MovieId,@AssignedProgram_ProgramDayItemId); SELECT SCOPE_IDENTITY();";

            var insertDisplayWeeks = new StringBuilder(@" INSERT INTO[dbo].[AssignedProgram_MovieDisplayWeek] 
                                        ([AssignedProgram_ProgramDayItemMovieId]
                                        ,[DisplayWeek]) 
                                     VALUES ");



            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var programDayItemMovieId = sqlConn.ExecuteScalar<int>(createString, new { MovieId = movieId, AssignedProgram_ProgramDayItemId = AssignedProgram_ProgramDayItemId });
                weeksToDisplay.ForEach(x => insertDisplayWeeks.Append($"({programDayItemMovieId},{x}),"));
                await sqlConn.ExecuteAsync(insertDisplayWeeks.ToString().TrimEnd(','));
            }
            return;
        }

        public async Task AddMetricToAssignedProgram(int metricId, List<int> weeksToDisplay, int position, int assignedProgram_ProgramDayId)
        {
            var assignedProgram_ProgramDayItemId = await CreateAssignedProgram_ProgramDayItem(position, ProgramDayItemEnum.Metric, assignedProgram_ProgramDayId);

            var createString = @" INSERT INTO dbo.AssignedProgram_ProgramDayItemMetric
                                 ([MetricId],[AssignedProgram_ProgramDayItemId])
                                 VALUES 
                                  (@MetricId,@AssignedProgram_ProgramDayItemId); SELECT SCOPE_IDENTITY();";

            var insertDisplayWeeks = new StringBuilder(@" INSERT INTO[dbo].[AssignedProgram_MetricsDisplayWeek] 
                                        ([AssignedProgram_ProgramDayItemMetricId]
                                        ,[DisplayWeek]) 
                                     VALUES ");



            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var programDayItemMetricId = await sqlConn.ExecuteScalarAsync<int>(createString, new { MetricId = metricId, AssignedProgram_ProgramDayItemId = assignedProgram_ProgramDayItemId });
                weeksToDisplay.ForEach(x => insertDisplayWeeks.Append($"({programDayItemMetricId},{x}),"));
                await sqlConn.ExecuteAsync(insertDisplayWeeks.ToString().TrimEnd(','));
            }
        }
        public async Task AddSurveyToAssignedProgram(int SurveyId, List<int> weeksToDisplay, int position, int assignedProgram_ProgramDayId)
        {
            var AssignedProgram_ProgramDayItemId = await CreateAssignedProgram_ProgramDayItem(position, ProgramDayItemEnum.Survey, assignedProgram_ProgramDayId);
            var createString = @" INSERT INTO dbo.AssignedProgram_ProgramDayItemSurvey
                                 ([SurveyId],[AssignedProgram_ProgramDayItemId])
                                 VALUES 
                                  (@SurveyId,@AssignedProgram_ProgramDayItemId); SELECT SCOPE_IDENTITY();";

            var insertDisplayWeeks = new StringBuilder(@" INSERT INTO[dbo].[AssignedProgram_SurveyDisplayWeeks] 
                                        ([AssignedProgram_ProgramDayItemSurveyId] 
                                        ,[DisplayWeek]) 
                                     VALUES ");

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var programDayItemSurveyId = sqlConn.ExecuteScalar<int>(createString, new { SurveyId = SurveyId, AssignedProgram_ProgramDayItemId = AssignedProgram_ProgramDayItemId });
                weeksToDisplay.ForEach(x => insertDisplayWeeks.Append($" ({programDayItemSurveyId},{x}),"));
                await sqlConn.ExecuteAsync(insertDisplayWeeks.ToString().TrimEnd(','));
            }
        }
        public async Task<int> AddSuperSetToAssignedProgram(int programDayItemId, string title)
        {
            var insertStatement = @"
                                    INSERT INTO [dbo].[AssignedProgram_ProgramDayItemSuperSet]
                                    ([AssignedProgram_ProgramDayItemId],SuperSetDisplayTitle)
                                    VALUES
                                    (@ProgramDayItemId, @Title); SELECT SCOPE_IDENTITY();";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return await sqlConn.ExecuteScalarAsync<int>(insertStatement, new { ProgramDayItemId = programDayItemId, Title = title });
            }
        }
        public async Task<int> AddSuperSetNotesToAssignedProgram(string note, List<int> displayWeeks, int position, int programDayItemSuperSetId)
        {
            var insertStatement = @"
                                    INSERT INTO [dbo].[AssignedProgram_SuperSetNote]
                                    ([Note],[AssignedProgram_ProgramDayItemSuperSetId],[Position])
                                    VALUES
                                    (@Note, @ProgramDayItemSuperSetId, @position); SELECT SCOPE_IDENTITY();";

            var instertDisplayWeeks = @"INSERT INTO AssignedProgram_SuperSetNoteDisplayWeek
                                       ([AssignedProgram_SuperSetNoteId], [DisplayWeek])
                                        VALUES 
                                       (@SuperSetNoteId, @DisplayWeek)";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var noteId = await sqlConn.ExecuteScalarAsync<int>(insertStatement, new { Note = note, ProgramDayItemSuperSetId = programDayItemSuperSetId, Position = position });
                for (int i = 0; i < displayWeeks.Count(); i++)
                {
                    await sqlConn.ExecuteAsync(instertDisplayWeeks, new { SuperSetNoteId = noteId, DisplayWeek = displayWeeks[i] });
                }
                return noteId;
            }
        }
        public async Task<int> AddSuperSetExerciseToAssignedProgram(Models.Program.SuperSetExercise ssExercises)
        {
            var insertStatement = @"INSERT INTO[dbo].[AssignedProgram_SuperSetExercise]
                                   ([AssignedProgram_ProgramDayItemSuperSetId]
                                   ,[Position]
                                   ,[ExerciseId]
                                    ,[Rest]
                                    ,[ShowWeight])
                                    VALUES
                                   (@ProgramDayItemSuperSetId
                                   ,@Position
                                   ,@ExerciseId
                                    ,@Rest
                                    ,@ShowWeight);  SELECT SCOPE_IDENTITY();";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return await sqlConn.ExecuteScalarAsync<int>(insertStatement, new
                {
                    ProgramDayItemSuperSetId = ssExercises.ProgramDayItemSuperSetId,
                    Position = ssExercises.Position,
                    ExerciseId = ssExercises.ExerciseId,
                    Rest = ssExercises.Rest,
                    ShowWeight = ssExercises.ShowWeight
                });
            }
        }
        public async Task AddSuperSet_SetsToAssignedProgram(List<Models.Program.ProgramDayItemSuperSet_Set> sets, int ProgramDayItemSuperSetWeekId, int exerciseId, int athleteId)
        {
            var insertSets = @"
                        
                        DECLARE @metricToEnter INT
                        ;WITH assignedProgramIdCTE(assignedProgramID) AS (
                        SELECT AssignedProgram_AssignedProgramId from Athletes where id = @athleteId
                        UNION ALL 
                        select AssignedProgram_ProgramId from [dbo].[AssignedProgram_AssignedProgramHistory] where athleteid = @athleteId
                        )
                        ,latestSnapShotMetricCTE ( lastValue, lastUpdatedValue) as
                        (
                        select TOP 1 mdw.Value, mdw.completedDate as lastUpdatedValue
						from assignedProgramIdCTE AS apiCTE
						INNER JOIN assignedProgram_ProgramDay AS d ON d.assignedProgram_programId = apiCTE.assignedProgramId
						INNER JOIN assignedProgram_ProgramDayItem AS pdi ON pdi.AssignedProgram_ProgramDayId = d.id
						INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdim ON pdim.assignedProgram_ProgramDayItemId = pdi.Id
						INNER JOIN AssignedProgram_MetricsDisplayWeek AS mdw ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.id
						INNER JOIN exercises AS e ON e.PercentMetricCalculationId = pdim.metricId
						WHERE mdw.value is not null AND e.id = @exerciseId
						 ORDER BY MDW.CompletedDate DESC)
                        ,latestpreSnapShotMetricCTE (lastValue, lastUpdatedValue) AS
                        (
                        SELECT TOP 1 [VALUE], CompletedDate  as lastUpdatedValue
                        FROM (
                        SELECT [value], CompletedDate
                        FROM addedMetrics AS am
                        INNER JOIN Exercises AS e ON E.PercentMetricCalculationId = AM.MetricId
                        where  am.metricId = e.PercentMetricCalculationId AND am.athleteId = @athleteId
                        AND e.id =  @exerciseId       
                        UNION ALL
                        SELECT [value], CompletedDate
                        FROM CompletedMetrics AS cm
                        INNER JOIN Exercises AS e ON E.PercentMetricCalculationId = CM.MetricId AND cm.athleteId = @athleteId
                        WHERE e.id =  @exerciseId) AS allMetrics
                        ORDER BY completedDate DESC
                        )

						SELECT top 1 @metricToEnter = isnull(lastvalue,0)
						FROM 
                        (SELECT lastValue , lastUpdatedValue
                        FROM latestSnapShotMetricCTE
                        UNION
                        SELECT lastValue , lastUpdatedValue
                        FROM latestpreSnapShotMetricCTE) AS a
                        WHERE a.lastValue is not null
                        ORDER BY a.lastUpdatedValue desc

                ;INSERT INTO dbo.AssignedProgram_ProgramDayItemSuperSet_set 
                          ([Position], 
                          [Sets], 
                          [Reps], 
                          [Percent], 
                          [Weight], 
                          AssignedProgram_ProgramDayItemSuperSetWeekId,
                          [Minutes],
                          [Seconds],
                          [Distance],
                          [RepsAchieved],
                          [Other],
                          [PercentMaxCalc],
                          [PercentMaxCalcSubPercent]) 
                          VALUES 
                          (@Position, 
                          @Sets, 
                          @Reps,
                          @Percent, 
                          @Weight, 
                          @ProgramDayItemSuperSetWeekId,
                          @Minutes,
                          @Seconds,
                          @Distance,
                          @RepsAchieved,
                          @Other,
                          FLOOR(ROUND(@metricToEnter * ((SELECT [percent] FROM exercises WHERE id = @exerciseId) * .01) , 0)),
                          FLOOR(ROUND(@metricToEnter * ((SELECT [percent] FROM exercises WHERE id = @exerciseId) * .01) * (@percent * .01) , 0)))
                                     ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                foreach (var set in sets)
                {
                    if (set.Percent == 0 && set.Reps == 0 && set.Sets == 0 && set.Weight == 0 && set.Minutes == 0 && set.Seconds == 0 && String.IsNullOrEmpty(set.Distance) && string.IsNullOrEmpty(set.Other))
                    {
                        continue;//do not save empty sets/reps}
                    }

                    try
                    {

                        await sqlConn.ExecuteAsync(insertSets,
                             new
                             {
                                 Position = set.Position,
                                 Sets = set.Sets,
                                 Reps = set.Reps,
                                 Percent = set.Percent,
                                 Weight = set.Weight,
                                 ProgramDayItemSuperSetWeekId = ProgramDayItemSuperSetWeekId,
                                 Minutes = set.Minutes,
                                 Seconds = set.Seconds,
                                 Distance = set.Distance,
                                 RepsAchieved = set.RepsAchieved,
                                 Other = set.Other,
                                 AthleteId = athleteId,
                                 ExerciseId = exerciseId
                             });

                    }
                    catch (Exception ex)
                    {
                        var y = ex;
                    }
                }
            }
        }
        public async Task<int> AddSuperSetWeekToAssignedProgram(Models.Program.ProgramDayItemSuperSetWeek ssWeek)
        {
            var insertStatement = @"INSERT INTO [dbo].[AssignedProgram_ProgramDayItemSuperSetWeek]
                                   ([Position]
                                   ,[AssignedProgram_SuperSetExerciseId])
                                    VALUES
                                   (@Position
                                   ,@ExerciseId);  SELECT SCOPE_IDENTITY();";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return await sqlConn.ExecuteScalarAsync<int>(insertStatement, new { Position = ssWeek.Position, ExerciseId = ssWeek.SuperSetExerciseId });
            }
        }

        public async Task AddNoteToAssignedProgram(string note, string name, List<int> weeksToDisplay, int position, int assignedProgram_ProgramDayId)
        {
            var AssignedProgram_ProgramDayItemId = await CreateAssignedProgram_ProgramDayItem(position, ProgramDayItemEnum.Note, assignedProgram_ProgramDayId);

            var createString = @" INSERT INTO dbo.AssignedProgram_ProgramDayItemNote
                                 ([Name],[Note],[AssignedProgram_ProgramDayItemId])
                                 VALUES 
                                  (@Name,@Note, @ProgramDayItemId); SELECT SCOPE_IDENTITY();  ";

            var insertDisplayWeeks = @" INSERT INTO[dbo].[AssignedProgram_NoteDisplayWeek] 
                                        ([AssignedProgram_ProgramDayItemNoteId],[DisplayWeek]) 
                                         VALUES (@ProgramDayItemNoteId, @WeekToDisplay)";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var ProgramDayItemNoteId = await sqlConn.ExecuteScalarAsync<int>(createString, new { Name = name, Note = note, ProgramDayItemId = AssignedProgram_ProgramDayItemId });

                foreach (var week in weeksToDisplay)
                {
                    await sqlConn.ExecuteAsync(insertDisplayWeeks, new { ProgramDayItemNoteId = ProgramDayItemNoteId, WeekToDisplay = week });
                }
            }
        }

    }
}