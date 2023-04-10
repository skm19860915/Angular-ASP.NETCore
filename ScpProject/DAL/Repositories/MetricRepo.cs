using System.Data.SqlClient;
using Dapper;
using Models.Metric;
using System;
using System.Collections.Generic;
using System.Linq;
using DAL.DTOs.Metrics;
using DAL.DTOs;
using DAL.DTOs.AthleteAssignedPrograms;
using DAL.CustomerExceptions;

namespace DAL.Repositories
{
    public interface IMetricRepo
    {
        void Archive(int metricId, Guid userToken);
        void AssociateMetricWithUnitOfMeasurement(int metricId, int unitOfMeasurementId, Guid userToken);
        int CreateMetric(Metric newMet, Guid userToken);
        int CreateUnitOfMeasurement(UnitOfMeasurement newUOM, Guid userToken);
        int DuplicateMetric(int metricId, Guid createdUserToken);
        List<HistoricProgram> GetAllHistoricProgramsWithMetrics(int athleteId, Guid UserToken);
        AthleteCompletedMetricHomePage GetAllMeasuredMetrics(int athleteId, Guid userGuid);
        List<Metric> GetAllMetrics(Guid userToken);
        List<MetricWithTagsDTO> GetAllMetricsTagMappings(Guid userToken);
        List<UnitOfMeasurement> GetAllUnitOfMeasurements(Guid userToken);
        Metric GetMetric(int metricId, Guid userToken);
        UnitOfMeasurement GetUnitOfMeasurement(int unitId, Guid userToken);
        void HardDelete(int metricId);
        void UnArchive(int metricId, Guid userToken);
        void UpdateMetric(Metric newMet, Guid userToken);
    }

    public class MetricRepo : IMetricRepo
    {
        private string ConnectionString;
        public MetricRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }


        public void HardDelete(int metricId)
        {
            var delString = "DELETE FROM Metrics WHERE id = @MetricId AND CanModify = 1";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(delString, new { MetricId = metricId });
            }
        }

        public int DuplicateMetric(int metricId, Guid createdUserToken)
        {
            var ran = new Random().Next(1000000);
            var dupeName = $"-copy {ran}";

            var dupString = $@"INSERT INTO [dbo].[metrics]   ([Name] 
                            ,[CreatedUserId] 
                            ,[UnitOfMeasurementId]
                            ,[IsDeleted]
                            ,[CanModify]
                            ,OrganizationId
                            ) 
                            SELECT name + '{dupeName}',createdUserId,[UnitOfMeasurementId],0,1,OrganizationId FROM metrics WHERE id = @Id AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}); SELECT SCOPE_IDENTITY();   ";


            var dupTagsToMetrics = $@"INSERT INTO TagsToMetrics (tagId, MetricId)
                                        SELECT tagId, @NewMetricId
                                        FROM TagsToMetrics WHERE MetricId = @OldMetricId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var newMetricId = sqlConn.ExecuteScalar<int>(dupString, new { Token = createdUserToken, Id = metricId });
                if (newMetricId > 0)
                {
                    sqlConn.Execute(dupTagsToMetrics, new { NewMetricId = newMetricId, OldMetricId = metricId });
                }
                return newMetricId;
            }
        }
        /// <summary>
        /// Returns all historic programs
        /// </summary>
        /// <param name="athleteId"></param>
        /// <param name="UserToken">Can be a coach userToken or an athlete UserToken</param>
        /// <returns></returns>

        public List<HistoricProgram> GetAllHistoricProgramsWithMetrics(int athleteId, Guid UserToken)
        {
            var getString = $@" select p.id AS 'ProgramId',p.[name],aph.StartDate as 'StartDate', aph.AssignedProgramId, 
                                from ProgramDayItemMetrics AS pdim
                                INNER JOIN ProgramDays AS pd ON pdim.ProgramDayItemId = pd.Id
                                INNER JOIN programs as p on p.Id = pd.ProgramId 
                                INNER JOIN AssignedPrograms AS ap ON ap.ProgramId = p.id
                                INNER JOIN AthleteProgramHistories AS aph ON aph.AssignedProgramId = ap.Id
                                INNER JOIN Athletes AS a ON a.id = APH.AthleteId
                                where aph.AthleteId = @AthleteID AND (a.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) OR a.AthleteUserID = ({ConstantSqlStrings.GetUserIdFromToken}))";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<HistoricProgram>(getString, new { AthleteId = athleteId, Token = UserToken }).ToList();
            }
        }
        public void UnArchive(int metricId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Metrics]
               SET IsDeleted = 0
              WHERE Id = @Id AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = metricId });
            }
        }
        public void Archive(int metricId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Metrics]
               SET IsDeleted = 1
              WHERE Id = @Id AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = metricId });
            }
        }
        public void UpdateMetric(Metric newMet, Guid userToken)
        {
            var updateString = $@" UPDATE Metrics
                                  SET [Name] = @Name,
                                  UnitOfMeasurementId = @UOMId
                                  WHERE id = @Id AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.ExecuteScalar(updateString, new { Name = newMet.Name, Token = userToken, uomId = newMet.UnitOfMeasurementId, Id = newMet.Id });
            }
        }
        public int CreateMetric(Metric newMet, Guid userToken)
        {
            var insertString = "INSERT INTO [dbo].[Metrics] "
                             + " ([Name] "
                             + " ,[CreatedUserId]"
                             + (newMet.UnitOfMeasurementId > 0 ? " , [UnitOfMeasurementId] " : string.Empty)
                             + " ,[CanModify]"
                             + " ,[IsDeleted]"
                             + ", OrganizationId  )"
                             + " VALUES "
                             + $" (@Name,({ConstantSqlStrings.GetUserIdFromToken})"
                             + (newMet.UnitOfMeasurementId > 0 ? " ,@uomId " : "")
                             + ",1"
                             + ",0"
                             + ",(" + ConstantSqlStrings.GetOrganizationIdByToken + "))"
                             + "; SELECT SCOPE_IDENTITY() ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                try
                {
                    return int.Parse(sqlConn.ExecuteScalar(insertString, new { Name = newMet.Name, Token = userToken, uomId = newMet.UnitOfMeasurementId }).ToString());
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
        public int CreateUnitOfMeasurement(UnitOfMeasurement newUOM, Guid userToken)
        {
            var insertString = $@"INSERT INTO [dbo].[UnitOfMeasurements] 
                                ([UnitType] 
                                ,[CreatedUserId] 
                                , OrganizationId)
                             VALUES 
                             (@Name,({ConstantSqlStrings.GetUserIdFromToken}),({ConstantSqlStrings.GetOrganizationIdByToken})); SELECT SCOPE_IDENTITY() ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                try
                {
                    return int.Parse(sqlConn.ExecuteScalar(insertString, new { Name = newUOM.UnitType, Token = userToken }).ToString());
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
        public void AssociateMetricWithUnitOfMeasurement(int metricId, int unitOfMeasurementId, Guid userToken)
        {
            var updateString = $"UPDATE metrics SET UnitOfMeasurementId = @uomId WHERE Id = @metricId AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { uomId = unitOfMeasurementId, metricID = metricId, Token = userToken });
            }
        }
        public List<Metric> GetAllMetrics(Guid userToken)
        {
            var getAllQuery = $"SELECT Id,name,CreatedUserId, UnitOfMeasurementId, CanModify, IsDeleted FROM metrics WHERE OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Metric>(getAllQuery, new { Token = userToken }).ToList();
            }
        }

        public UnitOfMeasurement GetUnitOfMeasurement(int unitId, Guid userToken)
        {
            var getAllQuery = $"SELECT UnitType, Id FROM UnitOfMeasurements WHERE oragnizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) AND Id = @id";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<UnitOfMeasurement>(getAllQuery, new { Token = userToken, Id = unitId }).FirstOrDefault();
            }
        }
        public List<UnitOfMeasurement> GetAllUnitOfMeasurements(Guid userToken)
        {
            var getAllQuery = $"SELECT UnitType, Id FROM UnitOfMeasurements WHERE organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<UnitOfMeasurement>(getAllQuery, new { Token = userToken }).ToList();
            }
        }
        public List<MetricWithTagsDTO> GetAllMetricsTagMappings(Guid userToken)
        {
            var tagMappings = $"SELECT t.MetricId ,t.TagId, ta.Name FROM TagsToMetrics AS t INNER JOIN MetricTags AS ta ON ta.Id = t.TagId INNER JOIN Metrics AS E on t.MetricId = e.Id WHERE e.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            var tagMappingDTOs = new List<MetricWithTagsDTO>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var reader = sqlConn.ExecuteReader(tagMappings, new { token = userToken });
                while (reader.Read())
                {
                    var targetDtoMapping = tagMappingDTOs.FirstOrDefault(x => x.MetricId == reader.GetInt32(0));
                    var newTagDTO = new TagDTO() { Id = reader.GetInt32(1), Name = reader.GetString(2) };
                    if (targetDtoMapping == null)
                    {

                        tagMappingDTOs.Add(new MetricWithTagsDTO() { MetricId = reader.GetInt32(0), Tags = new List<TagDTO>() { newTagDTO } });
                    }
                    else
                    {
                        targetDtoMapping.Tags.Add(newTagDTO);
                    }
                }
            }
            return tagMappingDTOs;
        }
        public Metric GetMetric(int metricId, Guid userToken)
        {
            var getAllQuery = $"SELECT * FROM metrics WHERE organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})  AND id = @Id";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Metric>(getAllQuery, new { Token = userToken, Id = metricId }).FirstOrDefault();
            }
        }
        /// <summary>
        /// Returns all meeasuerMetreics. A messuaredMetirc is a metric that has been completed. So in a sense MeasuredMetric == CompletedMetric
        /// </summary>
        /// <param name="athleteId"></param>
        /// <param name="userGuid">Can be the coach Id or the athlete Id</param>
        /// <returns></returns>
        public AthleteCompletedMetricHomePage GetAllMeasuredMetrics(int athleteId, Guid userGuid)
        {
            var getString = $@"select cm.[value] as 'MetricValue',  cm.CompletedDate,m.id, m.[name] as 'MetricName', uom.id 'UnitOfMeasurementId', uom.UnitType as 'UnitOfMeasurementName' , m.Id AS 'MetricId'
                                from CompletedMetrics AS CM
                                inner join Metrics AS m on cm.MetricId = m.id
                                INNER JOIN athletes AS a ON A.ID = CM.AthleteId
                                left join UnitOfMeasurements as uom on uom.id = m.UnitOfMeasurementId
                                where athleteId = @AthleteId and (a.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})   OR a.AthleteUserId =({ConstantSqlStrings.GetUserIdFromToken}))

                                UNION 

                                select cm.[value] as 'MetricValue',  cm.CompletedDate,m.id, m.[name] as 'MetricName', uom.id 'UnitOfMeasurementId', uom.UnitType as 'UnitOfMeasurementName' ,  m.Id AS 'MetricId'
                                from  addedMetrics AS CM
                                inner join Metrics AS m on cm.MetricId = m.id
                                INNER JOIN athletes AS a ON A.ID = CM.AthleteId
                                left join UnitOfMeasurements as uom on uom.id = m.UnitOfMeasurementId
                                where athleteId = @AthleteId and (a.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})   OR a.AthleteUserId =({ConstantSqlStrings.GetUserIdFromToken}))

                                UNION 

                                SELECT mdw.Value as 'MetricValue', mdw.CompletedDate,m.id, m.[name] as 'MetricName', uom.id 'UnitOfMeasurementId', uom.UnitType as 'UnitOfMeasurementName' ,  m.Id AS 'MetricId'
                                FROM [dbo].[AssignedProgram_MetricsDisplayWeek] AS mdw
                                INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdim ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.Id
                                INNER JOIN  AssignedProgram_ProgramDayItem AS pdi ON pdi.Id = pdim.AssignedProgram_ProgramDayItemId
                                INNER JOIN assignedProgram_programday AS pd ON pd.Id = pdi.AssignedProgram_ProgramDayId
                                INNER JOIN AssignedProgram_Program AS p ON pd.AssignedProgram_ProgramId = p.Id
                                INNER JOIN Athletes AS a ON a.AssignedProgram_AssignedProgramId =p.Id
                                INNER JOIN Metrics AS m ON m.id = pdim.MetricId
                                left join UnitOfMeasurements as uom on uom.id = m.UnitOfMeasurementId
                                WHERE a.Id = @athleteId

                       			UNION

			                    SELECT mdw.Value as 'MetricValue', mdw.CompletedDate,m.id, m.[name] as 'MetricName', uom.id 'UnitOfMeasurementId', uom.UnitType as 'UnitOfMeasurementName' ,  m.Id AS 'MetricId'
			                    FROM [dbo].[AssignedProgram_MetricsDisplayWeek] AS mdw
			                    INNER JOIN AssignedProgram_ProgramDayItemMetric AS pdim ON mdw.AssignedProgram_ProgramDayItemMetricId = pdim.Id
			                    INNER JOIN AssignedProgram_ProgramDayItem AS pdi ON pdi.Id = pdim.AssignedProgram_ProgramDayItemId
			                    INNER JOIN assignedProgram_programday AS pd ON pd.Id = pdi.AssignedProgram_ProgramDayId
			                    INNER JOIN AssignedProgram_Program AS p ON pd.AssignedProgram_ProgramId = p.Id
			                    INNER JOIN AssignedProgram_AssignedProgramHistory AS aph ON aph.AssignedProgram_ProgramId =p.Id
			                    INNER JOIN Metrics AS m ON m.id = pdim.MetricId
			                    left join UnitOfMeasurements as uom on uom.id = m.UnitOfMeasurementId
			                    WHERE aph.AthleteId = @athleteId AND MDW.[value] IS NOT NULL

";

            var s = new List<AthleteCompletedMetric>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                s = sqlConn.Query<AthleteCompletedMetric>(getString, new { Token = userGuid, AthleteId = athleteId }).ToList();
                //return sqlConn.Query<List<AthleteCompletedMetrics>>(getString, new { Token = userGuid, AthleteId = athleteId }).ToList();
            }
            var ret = new AthleteCompletedMetricHomePage() { CompletedMetrics = new List<AthleteCompletedMeasurementById>() };
            s.ForEach(x =>
            {
                var targetMetricGroup = ret.CompletedMetrics.Where(y => y.UnitOfMeasurementId == x.UnitOfMeasurementId).FirstOrDefault();
                if (targetMetricGroup == null)
                {
                    targetMetricGroup = new AthleteCompletedMeasurementById() { UnitOfMeasurementId = x.UnitOfMeasurementId, UnitOfMeasurementName = x.UnitOfMeasurementName, Metrics = new List<AthleteCompletedMetric>() };
                    ret.CompletedMetrics.Add(targetMetricGroup);
                }
                targetMetricGroup.Metrics.Add(x);
            });

            return ret;
        }
    }
}
