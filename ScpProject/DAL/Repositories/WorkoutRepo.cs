using Dapper;
using System.Data.SqlClient;
using Models.SetsAndReps;
using System;
using sr = DAL.DTOs.SetsAndReps;
using System.Collections.Generic;
using System.Linq;
using DAL.DTOs;
using DAL.CustomerExceptions;

namespace DAL.Repositories
{
    public interface IWorkoutRepo
    {
        void ArchiveWorkout(int workoutId, Guid userToken);
        int DuplicateWorkout(int workoutId, Guid createdUserToken);
        sr.WorkoutWithTagsDTO GetAllTagsForAWorkout(int workoutId, Guid userToken);
        List<sr.WorkoutWithTagsDTO> GetAllWorkoutTagMappings(Guid userToken);
        Workout GetProgramWorkout(int programDayItemExerciseId);
        sr.Workout GetWorkout(int workoutId, Guid userToken);
        Workout GetWorkoutDetails(int workoutId, Guid userToken);
        List<sr.Workout> GetWorkouts(Guid userToken);
        int SaveNewWorkout(Workout newWorkout, Guid userToken);
        void UnArchiveWorkout(int workoutId, Guid userToken);
        void UpdateWorkout(Workout newWorkout, Guid userToken);
    }

    public class WorkoutRepo : IWorkoutRepo
    {
        private string ConnectionString;
        public WorkoutRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public void UnArchiveWorkout(int workoutId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Workouts]
               SET IsDeleted = 0
              WHERE Id = @Id AND organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = workoutId });
            }
        }
        public void ArchiveWorkout(int workoutId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Workouts]
               SET IsDeleted = 1
              WHERE Id = @Id AND organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = workoutId });
            }
        }
        public int DuplicateWorkout(int workoutId, Guid createdUserToken)
        {
            var ran = new Random().Next(1000000);
            var dupeName = $"-copy {ran}";

            var dupString = $@"
                                DECLARE @OldWorkoutId	INT = {workoutId}
                                DECLARE @NewWorkoutId INT
                                DECLARE @NewWeeksTable TABLE (position INT,OldParentWorkoutId INT , NewParentWorkoutId INT , oldId INT, [newId]INT )

                                INSERT INTO workouts ([name],notes,createdDate,createduserId,Isdeleted,canModify,OrganizationId,rest)
                                SELECT [name]+'{dupeName}', notes,createdDate,createduserId,IsDeleted,1,OrganizationId,rest FROM workouts where id = @oldWorkoutId AND organizationId =  ({ConstantSqlStrings.GetOrganizationIdByToken});

                                SELECT @NewWorkoutId = scope_identity();

                                INSERT INTO weeks(position,parentWorkoutId)
                                OUTPUT inserted.id , inserted.Position, inserted.ParentWorkoutId , @OldWorkoutId INTO @NewWeeksTable( [newId], Position,newParentWorkoutId, OldParentWorkoutId )
                                SELECT position, @NewWorkoutId FROM weeks WHERE ParentWorkoutId = @oldWorkoutId

                                UPDATE nwt 
                                set oldId = w.id
                                FROM weeks AS w 
                                INNER JOIN @NewWeeksTable AS nwt ON nwt.OldParentWorkoutId = w.ParentWorkoutId AND nwt.position = w.Position

                                INSERT INTO dbo.[SETS]
                                SELECT s.Position,
                                s.[sets],
                                s.reps,
                                s.[percent],
                                s.[weight],
                                nwt.[newid],
                                s.[Minutes], 
                                s.[Seconds],
                                s.[Distance],
                                s.[RepsAchieved],
                                s.[Other]
                                FROM @NewWeeksTable AS nwt
                                INNER JOIN [sets] AS s on s.ParentWeekId = nwt.oldId ; SELECT @NewWOrkoutId  ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(dupString, new { Token = createdUserToken });
            }
        }
        public Workout GetProgramWorkout(int programDayItemExerciseId)
        {
            var weeksQuery = $"SELECT id, position FROM programweeks WHERE programDayitemExerciseId = @ProgramDayItemExerciseId";
            var setsRepQuery = $@"SELECT ID, [Position], 
                                [Sets],
                                [Reps],
                                [Percent],
                                [Weight],
                                ParentWeekId
                                [Minutes],
                                [Seconds],
                                [Distance],
                                [RepsAchieved],
                                [Other]  FROM programsets WHERE ParentProgramWeekId = @ParentProgramWeekId ";
            var workoutQuery = $"SELECT ID, Name, Notes, CanModify,IsDeleted,rest FROM Workouts WHERE Id =(select workoutId from ProgramDayItemExercises where id = {programDayItemExerciseId} ) ";
            var ret = new Workout()
            {
                Id = 0,
                Name = "Program Workouts Dont Have Names",
                Notes = " Program Workouts Dont Have Notes",
                TotalWorkout = new List<Week>()
            };
            var weeks = new List<Week>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var targetWorkOut = sqlConn.Query<Workout>(workoutQuery, new { ProgramDayItemExerciseId = programDayItemExerciseId });
                if (targetWorkOut.Any())
                {
                    ret.Id = targetWorkOut.First().Id;
                    ret.Name = targetWorkOut.First().Name;
                    ret.Notes = targetWorkOut.First().Notes;
                    ret.Rest = targetWorkOut.First().Rest;
                }
                weeks = sqlConn.Query<Week>(weeksQuery, new { ProgramDayItemExerciseId = programDayItemExerciseId }).ToList();

                foreach (var w in weeks)
                {
                    w.SetsAndReps = sqlConn.Query<Set>(setsRepQuery, new { ParentProgramWeekId = w.Id }).ToList();
                }
            }
            ret.TotalWorkout = weeks;
            return ret;
        }

        public Workout GetWorkoutDetails(int workoutId, Guid userToken)
        {
            var ret = new Workout();
            var weeks = new List<Week>();
            var workoutInfoQuery = $"SELECT id, name, notes, CanModify, IsDeleted, rest, showWeight FROM Workouts WHERE id = @Id  AND organizationId =  ({ConstantSqlStrings.GetOrganizationIdByToken}) ";
            var weeksQuery = $"SELECT id, position FROM weeks WHERE parentWorkoutId = @parentworkoutId";
            var setsRepQuery = $@"SELECT ID,[Position], 
                                [Sets],
                                [Reps],
                                [Percent],
                                [Weight],
                                ParentWeekId,
                                [Minutes],
                                [Seconds],
                                [Distance],
                                [RepsAchieved],
                                [Other]
                                 FROM sets WHERE parentWeekId = @parentWeekId ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                ret = sqlConn.Query<Workout>(workoutInfoQuery, new { Id = workoutId, Token = userToken }).FirstOrDefault();
                weeks = sqlConn.Query<Week>(weeksQuery, new { parentworkoutId = ret.Id }).ToList();

                foreach (var w in weeks)
                {
                    w.SetsAndReps = sqlConn.Query<Set>(setsRepQuery, new { ParentWeekId = w.Id }).ToList();
                }
            }
            ret.TotalWorkout = weeks;
            return ret;
        }

        public void UpdateWorkout(Workout newWorkout, Guid userToken)
        {
            var updateWorkoutDetails = $@" UPDATE dbo.workouts
                                         set [name]=@Name
                                         ,Notes = @Notes 
                                        ,Rest = @Rest
                                        ,ShowWeight = @ShowWeight
                                          WHERE id = @Id and  organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";
            //Instead of updating we are deleteing all sets and weeks because they can remove weeks/sets and reps and we would have to calculate what they removed
            //this way we just start fresh.
            var deleteSets = $@"DELETE S
                            FROM weeks AS w
                            INNER JOIN[Sets] AS s ON w.Id = s.ParentWeekId
                            WHERE w.ParentWorkoutId = {newWorkout.Id}";

            var deleteWeeks = @" DELETE FROM weeks WHERE ParentWorkoutId = @WorkoutId";


            var insertWeek = " INSERT INTO dbo.Weeks (Position,ParentWorkoutId) VALUES (@Position, @ParentWorkoutId);SELECT SCOPE_IDENTITY(); ";

            var insertSets = @" ;INSERT INTO dbo.Sets
                                ([Position], 
                                [Sets], 
                                [Reps], 
                                [Percent], 
                                [Weight], 
                                ParentWeekId,
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
                                @ParentWeekId,
                                @Minutes,
                                @Seconds,
                                @Distance,
                                @RepsAchieved,
                                @Other) ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateWorkoutDetails, new { Name = newWorkout.Name, Notes = newWorkout.Notes, CreatedDate = DateTime.Now, Token = userToken, Id = newWorkout.Id, Rest = newWorkout.Rest, ShowWeight = newWorkout.ShowWeight });
                sqlConn.Execute(deleteSets);
                sqlConn.Execute(deleteWeeks, new { WorkoutId = newWorkout.Id });
                foreach (var week in newWorkout.TotalWorkout)
                {
                    var weekId = int.Parse(sqlConn.ExecuteScalar(insertWeek, new { Position = week.Position, ParentWorkoutId = newWorkout.Id }).ToString());
                    foreach (var set in week.SetsAndReps)
                    {
                        if (
                    (!set.Percent.HasValue || set.Percent == 0) &&
                    (!set.Reps.HasValue || set.Reps == 0) &&
                    (!set.Sets.HasValue || set.Sets == 0) &&
                    (!set.Weight.HasValue || set.Weight == 0) &&
                    (!set.Minutes.HasValue || set.Minutes == 0) &&
                    (!set.Seconds.HasValue || set.Seconds == 0) &&
                    string.IsNullOrEmpty(set.Distance) &&
                    string.IsNullOrEmpty(set.Other))
                        {
                            continue;
                        }
                        sqlConn.Execute(insertSets,
                            new
                            {
                                Position = set.Position,
                                Sets = set.Sets,
                                Reps = set.Reps,
                                Percent = set.Percent,
                                Weight = set.Weight,
                                ParentWeekId = weekId,
                                Minutes = set.Minutes,
                                Seconds = set.Seconds,
                                Distance = string.IsNullOrEmpty(set.Distance) ? null : set.Distance,
                                RepsAchieved = set.RepsAchieved,
                                Other = set.Other
                            });
                    }
                }
            }
        }
        public int SaveNewWorkout(Workout newWorkout, Guid userToken)
        {
            var insertWorkoutDetails = $@"INSERT INTO dbo.workouts 
                 (Name, 
                 Notes, 
                 CreatedDate,
                 CreatedUserId,
                 isDeleted ,
                 canModify,
                 OrganizationId,
                 Rest,
                 ShowWeight) 
                 VALUES 
                 (@Name, 
                 @Notes, 
                 @CreatedDate,
                 ({ConstantSqlStrings.GetUserIdFromToken}),
                 0,
                 1,
                 ({ConstantSqlStrings.GetOrganizationIdByToken}),
                  @Rest,
                  @ShowWeight); SELECT SCOPE_IDENTITY();";

            var insertWeek = " INSERT INTO dbo.Weeks (Position,ParentWorkoutId) VALUES (@Position, @ParentWorkoutId);SELECT SCOPE_IDENTITY(); ";

            var insertSets = @" ;INSERT INTO dbo.Sets
                                ([Position], 
                                [Sets], 
                                [Reps], 
                                [Percent], 
                                [Weight], 
                                ParentWeekId,
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
                                @ParentWeekId,
                                @Minutes,
                                @Seconds,
                                @Distance,
                                @RepsAchieved,
                                @Other) ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var workoutId = 0;
                try
                {
                    workoutId = int.Parse(sqlConn.ExecuteScalar(insertWorkoutDetails,
                        new
                        {
                            Name = newWorkout.Name,
                            Notes = newWorkout.Notes,
                            CreatedDate = DateTime.Now,
                            Token = userToken,
                            Rest = newWorkout.Rest,
                            ShowWeight = newWorkout.ShowWeight
                        }).ToString());
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 2601) //duplicate key insert
                    {
                        throw new DuplicateKeyException();
                    }
                    throw;
                }
                foreach (var week in newWorkout.TotalWorkout)
                {
                    var weekId = int.Parse(sqlConn.ExecuteScalar(insertWeek, new { Position = week.Position, ParentWorkoutId = workoutId }).ToString());
                    foreach (var set in week.SetsAndReps)
                    {
                        if (
                            (!set.Percent.HasValue || set.Percent == 0) &&
                            (!set.Reps.HasValue || set.Reps == 0) &&
                            (!set.Sets.HasValue || set.Sets == 0) &&
                            (!set.Weight.HasValue || set.Weight == 0) &&
                            (!set.Minutes.HasValue || set.Minutes == 0) &&
                            (!set.Seconds.HasValue || set.Seconds == 0) &&
                             string.IsNullOrEmpty(set.Distance) &&
                           string.IsNullOrEmpty(set.Other))
                        {
                            continue;
                        }
                        sqlConn.Execute(insertSets,
                        new
                        {
                            Position = set.Position,
                            Sets = set.Sets,
                            Reps = set.Reps,
                            Percent = set.Percent,
                            Weight = set.Weight,
                            ParentWeekId = weekId,
                            Minutes = set.Minutes,
                            Seconds = set.Seconds,
                            Distance = string.IsNullOrEmpty(set.Distance) ? null : set.Distance,
                            RepsAchieved = set.RepsAchieved,
                            Other = set.Other
                        });
                    }
                }
                return workoutId;
            }
        }

        public List<sr.Workout> GetWorkouts(Guid userToken)
        {
            var getString = $"SELECT w.id,w.name,CanModify,isDeleted,Rest FROM workouts as w WHERE  organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<sr.Workout>(getString, new { Token = userToken }).ToList();
            }
        }

        public sr.Workout GetWorkout(int workoutId, Guid userToken)
        {
            var getString = $"SELECT w.id,w.name,CanModify,IsDeleted,Rest FROM workouts as w WHERE w.Id = @Id AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<sr.Workout>(getString, new { Id = workoutId, Token = userToken }).FirstOrDefault();
            }
        }

        public List<sr.WorkoutWithTagsDTO> GetAllWorkoutTagMappings(Guid userToken)
        {
            return GetAllWorkoutTagsMappingsOptional(userToken);
        }

        public sr.WorkoutWithTagsDTO GetAllTagsForAWorkout(int workoutId, Guid userToken)
        {
            return GetAllWorkoutTagsMappingsOptional(userToken, workoutId).FirstOrDefault(); ;
        }

        private List<sr.WorkoutWithTagsDTO> GetAllWorkoutTagsMappingsOptional(Guid userToken, int workoutId = 0)
        {
            var tagMappings = $" SELECT t.WorkoutId ,t.TagId, ta.Name FROM TagsToWorkouts AS t INNER JOIN WorkoutTags AS ta ON ta.Id = t.TagId INNER JOIN workouts AS E on t.WorkoutId = e.Id ";
            var tagMappwingsWhereClause = $" WHERE e.OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) " + (workoutId == 0 ? "" : $" AND e.Id = {workoutId} ");
            var tagMappingDTOs = new List<sr.WorkoutWithTagsDTO>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var reader = sqlConn.ExecuteReader(tagMappings + tagMappwingsWhereClause, new { token = userToken });
                while (reader.Read())
                {
                    var targetDtoMapping = tagMappingDTOs.FirstOrDefault(x => x.WorkoutId == reader.GetInt32(0));
                    var newTagDTO = new TagDTO() { Id = reader.GetInt32(1), Name = reader.GetString(2) };
                    if (targetDtoMapping == null)
                    {
                        tagMappingDTOs.Add(new sr.WorkoutWithTagsDTO() { WorkoutId = reader.GetInt32(0), Tags = new List<TagDTO>() { newTagDTO } });
                    }
                    else
                    {
                        targetDtoMapping.Tags.Add(newTagDTO);
                    }
                }
            }
            return tagMappingDTOs;

        }

    }
}
