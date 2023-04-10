using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DAL.DTOs;
using Dapper;
using Models.Exercise;
using DAL.CustomerExceptions;
using DAL.DTOs.Exercises;
using Models.User;

namespace DAL.Repositories
{
    public interface IExerciseRepo
    {
        void Archive(int exerciseId, Guid userToken);
        int CreateExericse(Exercise newEx, Guid userToken);
        int DuplicateExercise(int exerciseId, Guid createdUserToken);
        List<ExerciseDTO> GetAllExercises(Guid userToken);
        List<ExerciseWithTagsDTO> GetAllExerciseTagMappings(Guid userToken);
        Exercise GetExercise(int exerciseID, Guid userToken);
        void HardDelete(int exerciseId);
        User GetCreatedUser(int exerciseId);
        void UnArchive(int exerciseId, Guid userToken);
        void UpdateExercise(Exercise targetEx, Guid userToken);
    }

    public class ExerciseRepo : IExerciseRepo
    {
        private string ConnectionString;
        public ExerciseRepo(string connectionString) {
            ConnectionString = connectionString;
        }

        public void HardDelete(int exerciseId)
        {
            var delString = "DELETE FROM Exercises WHERE id = @ExerciseId AND CanModify = 1";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(delString, new { ExerciseId = exerciseId });
            }
        }
        public User GetCreatedUser(int exerciseId)
        {
            var getQuery = @"SELECT u.*
                            FROM exercises AS e
                            INNER JOIN users as u ON e.createdUserId = u.Id
                            WHERE e.Id = @exerciseId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<User>(getQuery, new { ExerciseId = exerciseId }).FirstOrDefault();
            }
        }

        public Exercise GetExercise(int exerciseID, Guid userToken)
        {
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var sqlQuery = $@"SELECT  e.Name ,e.IsDeleted, e.CreatedUserId, e.Notes, e.Id, e.[Percent] as 'Percent', e.PercentMetricCalculationId, canModify, videoURL
                                  FROM exercises AS e 
                                  INNER JOIN users AS u ON u.organizationId = e.organizationId
                                  INNER JOIN userTokens AS ut ON u.Id = ut.UserId 
                                  WHERE e.Id = @ExerciseId AND ut.token = @Token ";
                var ret = sqlConn.Query<Exercise>(sqlQuery, new { Token = userToken, ExerciseId = exerciseID });
                return ret.Any() ? ret.First() : new Exercise();
            }
        }

        public int DuplicateExercise(int exerciseId, Guid createdUserToken)
        {
            var ran = new Random().Next(1000000);
            var dupeName = $"-copy {ran}";

            var dupString = $@"INSERT INTO [dbo].[Exercises]   ([Name] 
                            ,[CreatedUserId] 
                            ,[Notes]
                            ,[IsDeleted]
                            ,[Percent]
                            ,[PercentMetricCalculationId]
                            ,[CanModify]
                            ,[OrganizationId]
                            ,[VideoURL]
                            ) 
                            SELECT name + '{dupeName}',createdUserId, notes,0,[percent],[PercentMetricCalculationId],1, organizationId,videoURL FROM exercises WHERE id = @Id AND  organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}); SELECT SCOPE_IDENTITY();   ";

            var dupTagsToExercise = $@"INSERT INTO TagsToExercises (tagId, ExerciseId)
                                        SELECT tagId, @NewExerciseId
                                        FROM TagsToExercises WHERE ExerciseId = @OldExerciseId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var newExerciseId = sqlConn.ExecuteScalar<int>(dupString, new { Token = createdUserToken, Id = exerciseId });
                if (exerciseId > 0)
                {
                    sqlConn.Execute(dupTagsToExercise, new { NewExerciseId = newExerciseId, OldExerciseId = exerciseId });
                }
                return newExerciseId;
            }

        }
        /// <summary>
        /// Insertes new Exercise
        /// </summary>
        /// <param name="newEx">Exercise to insert</param>
        /// <param name="userToken">user who created Exercise</param>
        /// <exception cref="DuplicateKeyException">Thrown when The exercise already exsists. AN existing exercise is one with the same name and same inserted userId</exception>
        /// <returns>Inserted Exercise Id</returns>
        public int CreateExericse(Exercise newEx, Guid userToken)
        {
            var insertString = $@"INSERT INTO [dbo].[Exercises] 
                            ([Name] 
                            ,[CreatedUserId] 
                            ,[IsDeleted]
                            ,[Percent]
                            ,[PercentMetricCalculationId]
                            ,[CanModify]
                            ,[OrganizationId]
                            ,[VideoURL]
                            ) 
                            VALUES 
                            (@Name,({ConstantSqlStrings.GetUserIdFromToken}),0, @Percent, @PercentMetricCalculationId,1,({ConstantSqlStrings.GetOrganizationIdByToken}), @video); SELECT SCOPE_IDENTITY() ";

            try
            {
                using (var sqlConn = new SqlConnection(ConnectionString))
                {
                    return int.Parse(sqlConn.ExecuteScalar(insertString, new { Video = newEx.VideoURL, Name = newEx.Name, Token = userToken, Percent = newEx.Percent, PercentMetricCalculationId = newEx.PercentMetricCalculationId }).ToString());
                }
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

        public void UpdateExercise(Exercise targetEx, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Exercises]
               SET [Name] = @Name 
              ,[Notes] = @Notes
              ,[Percent] = @Percent
              ,[PercentMetricCalculationID] = @PMCI
              ,[VideoURL] = @Video
              WHERE Id = @Id AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Video = targetEx.VideoURL, Name = targetEx.Name, Token = userToken, Notes = targetEx.Notes, IsDeleted = targetEx.IsDeleted, Id = targetEx.Id, Percent = targetEx.Percent, PMCI = targetEx.PercentMetricCalculationId });
            }
        }

        public void UnArchive(int exerciseId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Exercises]
               SET IsDeleted = 0
              WHERE Id = @Id AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = exerciseId });
            }
        }
        public void Archive(int exerciseId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Exercises]
               SET IsDeleted = 1
              WHERE Id = @Id AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = exerciseId });
            }
        }

        public List<ExerciseWithTagsDTO> GetAllExerciseTagMappings(Guid userToken)
        {
            var tagMappings = $"SELECT t.ExerciseId ,t.TagId, ta.Name FROM TagsToExercises AS t INNER JOIN ExerciseTags AS ta ON ta.Id = t.TagId INNER JOIN exercises AS E on t.ExerciseId = e.Id WHERE e.OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            var tagMappingDTOs = new List<ExerciseWithTagsDTO>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var reader = sqlConn.ExecuteReader(tagMappings, new { token = userToken });
                while (reader.Read())
                {
                    var targetDtoMapping = tagMappingDTOs.FirstOrDefault(x => x.ExerciseId == reader.GetInt32(0));
                    var newTagDTO = new TagDTO() { Id = reader.GetInt32(1), Name = reader.GetString(2) };
                    if (targetDtoMapping == null)
                    {

                        tagMappingDTOs.Add(new ExerciseWithTagsDTO() { ExerciseId = reader.GetInt32(0), Tags = new List<TagDTO>() { newTagDTO } });
                    }
                    else
                    {
                        targetDtoMapping.Tags.Add(newTagDTO);
                    }
                }
            }
            return tagMappingDTOs;
        }


        public List<ExerciseDTO> GetAllExercises(Guid userToken)
        {

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var sqlQuery = $@"SELECT e.Name ,e.IsDeleted, e.CreatedUserId, e.Notes, e.Id, e.[Percent] as 'Percent', e.PercentMetricCalculationId, isnull(m.Name,'') as 'CalcMetricName', e.canModify, e.videoURL
                                     FROM exercises AS e 
                                    LEFT JOIN metrics AS m on e.PercentMetricCalculationId = m.id
                                Where e.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
                var ret = sqlConn.Query<ExerciseDTO>(sqlQuery, new { Token = userToken });
                return ret.ToList(); ;
            }
        }
    }
}
