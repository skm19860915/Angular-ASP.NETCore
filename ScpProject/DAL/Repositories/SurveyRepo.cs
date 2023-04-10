using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DAL.DTOs;
using Dapper;
using m = Models;
using Models.Enums;
using DAL.CustomerExceptions;
using DAL.DTOs.AthleteAssignedPrograms;
using DAL.DTOs.Survey;
using Models.Athlete;
using Models.Survey;

namespace DAL.Repositories
{
    public interface ISurveyRepo
    {
        void AddCoachEmailToScaleThresholdNotification(int scaleThresholdId, List<int> coachIds);
        void AddCoachEmailToYesNoThresholdNotification(int yesNoThresholdId, List<int> coachIds);
        int AddScaleThreshold(int questionId, QuestionThresholdEnum comparer, int value);
        int AddYesNoThreshold(int questionId, bool threshold);
        void ArchiveQuestion(int questionId, Guid userToken);
        void ArchiveSurvey(int surveyId, Guid userToken);
        int AssociateQuestionWithSurvey(int questionId, int surveyId);
        void CompleteOpenEndedQuestion(CompletedQuestionOpenEnded answeredQuestion);
        void CompleteScaleQuestion(CompletedQuestionScale answeredQuestion);
        void CompleteYesNoQuestion(CompletedQuestionYesNo answeredQuestion);
        int CreateQuestion(string questionText, QuestionTypeEnum questionTypeEnum, Guid userToken);
        int CreateSurvey(Survey newSurvey, Guid userToken);
        void DuplicateSurvey(int surveyId, Guid userToken);
        List<HistoricProgram> GetAllHistoricProgramsWithSurveys(int athleteId, Guid createdUserToken);
        List<QuestionDTO> GetAllQuestions(Guid userToken);
        List<QuestionDTO> GetAllSurveyQuestions(int surveyId, Guid userToken);
        List<Survey> GetAllSurveys(Guid userToken);
        List<SurveyWithTagsDTO> GetAllSurveyTagMappings(Guid userToken);
        List<AthleteHomePageSurvey> GetHomePageSurveysByAssignedProgramId(int assignedProgramId, Guid createdUserGuid);
        List<PastSurveyItem> GetPastSurveyList(int athleteId);
        Question GetQuestion(int questionId, Guid userToken);
        List<ScaleQuestionThreshold> GetScaleThresholdsForQuestion(int questionId);
        Survey GetSurvey(int surveyId, Guid userToken);
        int GetSurveyIdOfMappedQuestion(int SurveysToQuestionsId);
        List<int> GetThresholdNotificationsForCoachesScaleThreshold(int thresholdId);
        List<int> GetThresholdNotificationsForCoachesYesNoThreshold(int thresholdId);
        List<YesNoQuestionThreshold> GetYesNoThresholdsForQuestion(int questionId);
        void HardDelete(int surveyId);
        int MapQuestionToSurvey(int surveyId, int questionId);
        void RemoveCoachesFromScaleThresholdNotification(int questionid);
        void RemoveCoachesFromYesNoThresholdNotification(int questionId);
        void RemoveQuestionFromSurvey(int SurveysToQuestionsId);
        void RemoveScaleThreshold(int questionId);
        void RemoveYesNoThreshold(int questionId);
        void SendOutNotificationsForScaleQuestion(int questionId, int athleteId, int answer);
        void SendOutNotificationsForYesNow(int questionId, int athleteId, bool thresholdValue);
        void UnArchiveQuestion(int questionId, Guid userToken);
        void UnArchiveSurvey(int surveyId, Guid userToken);
        void UpdateQuestion(string questionText, QuestionTypeEnum questionTypeEnum, int questionId, Guid userToken);
        void UpdateScaleThreshold(int id, int value, QuestionThresholdEnum comparer);
        void UpdateSurvey(Survey newSurvey, Guid userToken);
        void AssignedProgram_CompleteOpenEndedQuestion(m.Athlete.CompletedQuestionOpenEnded answeredQuestion);
        void AssignedProgram_CompleteScaleQuestion(m.Athlete.CompletedQuestionScale answeredQuestion);
        void AssignedProgram_CompleteYesNoQuestion(m.Athlete.CompletedQuestionYesNo answeredQuestion);
    }

    public class SurveyRepo : ISurveyRepo
    {
        private string ConnectionString;
        public SurveyRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void HardDelete(int surveyId)
        {
            var deleteQuestionAssociation = " DELETE FROM SurveysToQuestions WHERE surveyId = @SurveyId  ";
            var delString = "DELETE FROM Surveys WHERE id = @SurveyId AND CanModify = 1";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(deleteQuestionAssociation, new { SurveyId = surveyId });
                sqlConn.Execute(delString, new { SurveyId = surveyId });
            }
        }


        public List<PastSurveyItem> GetPastSurveyList(int athleteId)
        {
            //first select statements gets all past programs surveys
            //second select statement gets current program surveys
            var getString = $@";with maxPastQuestionsCTE(completedDate, assignedProgramId) as (
                                            SELECT MAX(CompletedDate), maxedQuestions.AssignedProgramId
                                            FROM(
                                            SELECT MAX(completedDate) as 'CompletedDate', AssignedProgramId  
                                            FROM CompletedQuestionScales 
                                            group by AssignedProgramId
                                            UNION
                                            SELECT MAX(completedDate) as 'CompletedDate', AssignedProgramId  
                                            FROM CompletedQuestionOpenEndeds 
                                            group by AssignedProgramId
                                            UNION
                                            SELECT MAX(completedDate) as 'CompletedDate', AssignedProgramId  
                                            FROM CompletedQuestionYesNoes 
                                            group by AssignedProgramId) AS maxedQuestions
                                            INNER JOIN AthleteProgramHistories AS aph ON aph.AssignedProgramId = maxedQuestions.AssignedProgramId 
                                            WHERE aph.AthleteId = @athleteId
                                            group by maxedQuestions.AssignedProgramId
                                            )

                                            SELECT  P.[name] AS ProgramName ,s.Title AS SurveyName, ap.id AS assignedProgramId,mpqCTE.completedDate as 'SurveyCompleted' ,mpqCTE.assignedProgramId
                                            FROM Programs AS p
                                            INNER JOIN ProgramDays AS pd ON pd.ProgramId = p.Id
                                            INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                                            INNER JOIN ProgramDayItemSurveys AS pdis ON pdi.Id = pdis.ProgramDayItemId
                                            INNER JOIN Surveys AS s ON s.Id = pdis.SurveyId
                                            INNER JOIN AssignedPrograms AS ap ON ap.ProgramId = p.Id
                                            INNER JOIN AthleteProgramHistories AS aph ON aph.AssignedProgramId = ap.Id
                                            LEFT JOIN maxPastQuestionsCTE AS mpqCTE ON mpqCTE.assignedProgramId = aph.AssignedProgramId
                                            WHERE aph.AthleteId = @athleteId

                                            ;with maxCurrentQuestionsCTE(completedDate, assignedProgramId) as (
                                            SELECT MAX(CompletedDate), maxedQuestions.AssignedProgramId
                                            FROM(
                                            SELECT MAX(completedDate) as 'CompletedDate', AssignedProgramId  
                                            FROM CompletedQuestionScales 
                                            group by AssignedProgramId
                                            UNION
                                            SELECT MAX(completedDate) as 'CompletedDate', AssignedProgramId  
                                            FROM CompletedQuestionOpenEndeds 
                                            group by AssignedProgramId
                                            UNION
                                            SELECT MAX(completedDate) as 'CompletedDate', AssignedProgramId  
                                            FROM CompletedQuestionYesNoes 
                                            group by AssignedProgramId) AS maxedQuestions
                                            INNER JOIN Athletes AS a ON a.AssignedProgramId = maxedQuestions.AssignedProgramId 
                                            WHERE a.Id = @athleteId
                                            group by maxedQuestions.AssignedProgramId
                                            )

                                            SELECT  P.[name] AS ProgramName,s.Title AS SurveyName, ap.Id  AS assignedProgramId, mpqCTE.completedDate as 'SurveyCompleted' ,mpqCTE.assignedProgramId
                                            FROM Programs AS p
                                            INNER JOIN ProgramDays AS pd ON pd.ProgramId = p.Id
                                            INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = pd.Id
                                            INNER JOIN ProgramDayItemSurveys AS pdis ON pdi.Id =pdis.ProgramDayItemId
                                            INNER JOIN Surveys AS s ON s.Id = pdis.SurveyId
                                            INNER JOIN AssignedPrograms AS ap ON ap.ProgramId = p.Id
                                            INNER JOIN Athletes AS a ON a.AssignedProgramId = ap.Id
                                            LEFT JOIN maxCurrentQuestionsCTE AS mpqCTE ON mpqCTE.assignedProgramId = a.AssignedProgramId
                                            WHERE a.id = @athleteId
                                            ORDER BY ap.id desc";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<PastSurveyItem>(getString, new { athleteId = athleteId }).ToList();

            }
        }
        public void DuplicateSurvey(int surveyId, Guid userToken)
        {

            var ran = new Random().Next(1000000);
            var dupeName = $"-copy {ran}";
            var dupSurvey = $@" INSERT INTO surveys (title,description,createdUserId,isDeleted,CanModify,OrganizationId) 
                              SELECT title + '{dupeName}',description,createdUserId,isdeleted,1, OrganizationId
                              FROM surveys AS s WHERE s.Id = @OldSurveyId AND  organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}); select scope_identity()";
            var dupSurveyTagsAndQuestions = @"INSERT INTO surveysToQuestions (surveyId, questionId) 
                                             SELECT @NewSurveyId, questionId FROM surveysToQuestions WHERE surveyId = @OldSurveyId;
                                              INSERT INTO tagsToSurveys(tagId, SurveyId)
                                              SELECT tagId, @NewSurveyId FROM TagsToSurveys WHERE surveyID = @OldSurveyId;";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var newSurveyId = sqlConn.ExecuteScalar<int>(dupSurvey, new { Token = userToken, OldSurveyId = surveyId });
                sqlConn.Execute(dupSurveyTagsAndQuestions, new { OldSurveyId = surveyId, NewSurveyId = newSurveyId });
            }
        }
        public void UnArchiveSurvey(int surveyId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Surveys]
               SET IsDeleted = 0
              WHERE Id = @Id AND organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = surveyId });
            }
        }
        public void ArchiveSurvey(int surveyId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Surveys]
               SET IsDeleted = 1
              WHERE Id = @Id AND organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = surveyId });
            }
        }
        public void UnArchiveQuestion(int questionId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Questions]
               SET IsDeleted = 0
              WHERE Id = @Id AND organizationId = ({ConstantSqlStrings.GetUserIdFromToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = questionId });
            }
        }
        public void ArchiveQuestion(int questionId, Guid userToken)
        {
            var updateString = $@"UPDATE [dbo].[Questions]
               SET IsDeleted = 1
              WHERE Id = @Id AND organizationId = ({ConstantSqlStrings.GetUserIdFromToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Token = userToken, Id = questionId });
            }
        }
        public void UpdateQuestion(string questionText, QuestionTypeEnum questionTypeEnum, int questionId, Guid userToken)
        {
            var updateString = " UPDATE[dbo].[Questions] "
                                + " SET[QuestionDisplayText] = @QuestionDisplayText "
                                + " ,[QuestionTypeId] = @QuestionTypeId "
                                + $"  WHERE Id = @Id AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { QuestionDisplayText = questionText, QuestionTypeId = questionTypeEnum, Token = userToken, Id = questionId });
            }

        }
        public void UpdateSurvey(m.Survey.Survey newSurvey, Guid userToken)
        {
            var updateString = $@" UPDATE [dbo].[Surveys] 
                           SET[Title] = @Title
                            ,[Description] = @Description
                            WHERE Id = @Id AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.ExecuteScalar(updateString, new { Title = newSurvey.Title, Description = newSurvey.Description, Token = userToken, Id = newSurvey.Id });
            }
        }


        public int MapQuestionToSurvey(int surveyId, int questionId)
        {
            var mapQuestionToSurvey = $"INSERT INTO [dbo].[SurveysToQuestions] (SurveyId, QuestionId) VALUES (@SurveyId,@QuestionId); SELECT SCOPE_IDENTITY()";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return int.Parse(sqlConn.ExecuteScalar(mapQuestionToSurvey, new { SurveyId = surveyId, QuestionId = questionId }).ToString());
            }
        }

        public int CreateQuestion(string questionText, QuestionTypeEnum questionTypeEnum, Guid userToken)
        {
            var insertString = $"INSERT INTO [dbo].[Questions] (QuestionDisplayText, QuestionTypeId, CreatedUserId, OrganizationId) VALUES (@Title,@QuestionTypeId,({ConstantSqlStrings.GetUserIdFromToken}),({ConstantSqlStrings.GetOrganizationIdByToken})); SELECT SCOPE_IDENTITY()";

            int questionId = 0;

            try
            {
                using (var sqlConn = new SqlConnection(ConnectionString))
                {
                    return questionId = int.Parse(sqlConn.ExecuteScalar(insertString, new { Title = questionText, QuestionTypeId = questionTypeEnum, Token = userToken }).ToString());
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
        /// <summary>
        /// Creates a new Survey
        /// </summary>
        /// <param name="newSurvey">newSurvey to create </param>
        /// <param name="userToken"> created User token</param>
        /// <exception cref="DuplicateKeyException">Thrown when The survey already exsists. AN existing Survey is one with the same name and same inserted userId</exception> 
        /// <returns></returns>
        public int CreateSurvey(m.Survey.Survey newSurvey, Guid userToken)
        {
            var insertString = $"INSERT INTO [dbo].[Surveys] (Title, Description, CreatedUserId, canModify, OrganizationId) VALUES (@Title,@Description,({ConstantSqlStrings.GetUserIdFromToken}),1,({ConstantSqlStrings.GetOrganizationIdByToken})); SELECT SCOPE_IDENTITY()";
            try
            {
                using (var sqlConn = new SqlConnection(ConnectionString))
                {
                    return int.Parse(sqlConn.ExecuteScalar(insertString, new { Title = newSurvey.Title, Description = newSurvey.Description, Token = userToken }).ToString());
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

        public m.Survey.Question GetQuestion(int questionId, Guid userToken)
        {
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var sqlQuery = $@"SELECT *
                                  FROM questions AS q 
                                  WHERE q.Id = @QuestionId AND OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
                return sqlConn.QueryFirstOrDefault<m.Survey.Question>(sqlQuery, new { Token = userToken, QuestionId = questionId });
            }
        }
        public m.Survey.Survey GetSurvey(int surveyId, Guid userToken)
        {
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var sqlQuery = $@"SELECT *
                                FROM Surveys AS s 
                                WHERE OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) AND s.Id = @SurveyId";
                return sqlConn.QueryFirstOrDefault<m.Survey.Survey>(sqlQuery, new { Token = userToken, SurveyId = surveyId });
            }
        }
        public void RemoveQuestionFromSurvey(int SurveysToQuestionsId)
        {
            var deleteString = $"DELETE FROM SurveysToQuestions where Id = @SurveyToQuestionId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.ExecuteScalar(deleteString, new { SurveyToQuestionId = SurveysToQuestionsId });
            }
        }
        public int GetSurveyIdOfMappedQuestion(int SurveysToQuestionsId)
        {
            var deleteString = $"SELECT SurveyId FROM SurveysToQuestions where Id = @SurveyToQuestionId ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(deleteString, new { SurveyToQuestionId = SurveysToQuestionsId });
            }
        }
        public int AssociateQuestionWithSurvey(int questionId, int surveyId)
        {
            var insertString = $"INSERT INTO SurveysToQuestions (SurveyId,QuestionId) VALUES (@SurveyId,@QuestionId)";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Execute(insertString, new { SurveyId = surveyId, QuestionId = questionId });
            }
        }
        public List<SurveyWithTagsDTO> GetAllSurveyTagMappings(Guid userToken)
        {
            var tagMappings = $"SELECT t.TagId,t.SurveyId, st.Name FROM TagsToSurveys AS t INNER JOIN SurveyTags AS st ON st.Id = t.tagId INNER JOIN Surveys AS s on t.SurveyId = s.Id WHERE s.OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            var tagMappingDTOs = new List<SurveyWithTagsDTO>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var reader = sqlConn.ExecuteReader(tagMappings, new { token = userToken });
                while (reader.Read())
                {
                    var targetDtoMapping = tagMappingDTOs.FirstOrDefault(x => x.SurveyId == reader.GetInt32(1));
                    var newTagDTO = new TagDTO() { Id = reader.GetInt32(0), Name = reader.GetString(2) };
                    if (targetDtoMapping == null)
                    {
                        tagMappingDTOs.Add(new SurveyWithTagsDTO() { SurveyId = reader.GetInt32(1), Tags = new List<TagDTO>() { newTagDTO } });
                    }
                    else
                    {
                        targetDtoMapping.Tags.Add(newTagDTO);
                    }
                }
            }
            return tagMappingDTOs;
        }

        public List<DTOs.QuestionDTO> GetAllSurveyQuestions(int surveyId, Guid userToken)
        {
            var sql = $@" SELECT q.QuestionDisplayText, q.QuestionTypeId, q.Id,sq.SurveyId,sq.Id as 'SurveysToQuestionsId' , q.CanModify
                 FROM SurveysToQuestions AS sq 
                 INNER JOIN Questions AS q ON  sq.QuestionId = q.Id 
                 INNER JOIN surveys AS  s ON s.Id = sq.SurveyId 
                 WHERE sq.SurveyId = @surveyId 
                 AND s.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var ret = sqlConn.Query<DTOs.QuestionDTO>(sql, new { SurveyId = surveyId, Token = userToken });
                return ret.ToList();
            }
        }

        public List<DTOs.QuestionDTO> GetAllQuestions(Guid userToken)
        {
            var sql = " SELECT q.QuestionDisplayText, q.QuestionTypeId, q.Id, q.CanModify "
                + " FROM Questions AS q "
                + $" WHERE q.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var ret = sqlConn.Query<DTOs.QuestionDTO>(sql, new { Token = userToken });
                return ret.ToList();
            }
        }
        public List<Models.Survey.YesNoQuestionThreshold> GetYesNoThresholdsForQuestion(int questionId)
        {
            var ret = new List<Models.Survey.YesNoQuestionThreshold>();

            var getSQL = $@"SELECT * FROM YesNoQuestionThresholds AS t WHERE questionId = @QuestionId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Models.Survey.YesNoQuestionThreshold>(getSQL, new { QuestionId = questionId }).ToList();

            }
        }
        public List<Models.Survey.ScaleQuestionThreshold> GetScaleThresholdsForQuestion(int questionId)
        {

            var getSQL = $@"SELECT * FROM [ScaleQuestionThresholds] AS t WHERE questionId = @QuestionId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Models.Survey.ScaleQuestionThreshold>(getSQL, new { QuestionId = questionId }).ToList();

            }
        }

        public List<int> GetThresholdNotificationsForCoachesYesNoThreshold(int thresholdId)
        {
            var getSQL = $@"SELECT UserId FROM [YesNoQuestionThresholdToCoaches] WHERE YesNoThresholdId = @ThresholdId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<int>(getSQL, new { ThresholdId = thresholdId }).ToList();
            }
        }
        public List<int> GetThresholdNotificationsForCoachesScaleThreshold(int thresholdId)
        {
            var getSQL = $@"SELECT UserId FROM [ScaleThresholdToCoaches] WHERE ScaleThresholdId = @ThresholdId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<int>(getSQL, new { ThresholdId = thresholdId }).ToList();
            }
        }

        public List<m.Survey.Survey> GetAllSurveys(Guid userToken)
        {
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var sqlQuery = $@"SELECT s.Title ,s.Description,s.Id, s.CanModify, isDeleted
                                FROM Surveys AS s  WHERE Organizationid = ({ConstantSqlStrings.GetOrganizationIdByToken})";


                var ret = sqlConn.Query<m.Survey.Survey>(sqlQuery, new { Token = userToken });
                return ret.ToList(); ;
            }
        }
        public void AssignedProgram_CompleteYesNoQuestion(m.Athlete.CompletedQuestionYesNo answeredQuestion)
        {


            var insertStatement = @"
            IF EXISTS(SELECT 1 FROM [AssignedProgram_CompletedQuestionYesNo] WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgram_ProgramId = @AssignedProgramId AND AssignedProgram_ProgramDayItemSurveyId = @ProgramDayItemSurveyId  AND WeekId = @WeekId)
            BEGIN
                UPDATE [AssignedProgram_CompletedQuestionYesNo]
                SET YesNoValue = @YesNoValue, CompletedDate = @CompletedDate
                WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgram_ProgramId = @AssignedProgramId AND AssignedProgram_ProgramDayItemSurveyId = @ProgramDayItemSurveyId  AND WeekId = @WeekId
            END
            ELSE
            BEGIN
                INSERT INTO [dbo].[AssignedProgram_CompletedQuestionYesNo]
                           ([YesNoValue]
                           ,[AthleteId]
                           ,[QuestionId]
                           ,[CompletedDate]
                           ,[AssignedProgram_ProgramId]
                           ,[AssignedProgram_ProgramDayItemSurveyId]
                           ,[WeekId])
                     VALUES
                           (@YesNoValue
                           ,@AthleteId
                           ,@QuestionId
                           ,@CompletedDate
                           ,@AssignedProgramId
                           ,@ProgramDayItemSurveyId
                            ,@WeekId)
            END";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insertStatement,
                    new
                    {
                        YesNoValue = answeredQuestion.YesNoValue,
                        AthleteId = answeredQuestion.AthleteId,
                        QuestionId = answeredQuestion.QuestionId,
                        CompletedDate = answeredQuestion.CompletedDate,
                        AssignedProgramId = answeredQuestion.AssignedProgramId,
                        ProgramDayItemSurveyId = answeredQuestion.ProgramDayItemSurveyId,
                        WeekId = answeredQuestion.WeekId
                    });
            }
        }
        public void CompleteYesNoQuestion(m.Athlete.CompletedQuestionYesNo answeredQuestion)
        {


            var insertStatement = @"
            IF EXISTS(SELECT 1 FROM CompletedQuestionYesNoes WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgramId = @AssignedProgramId AND ProgramDayItemSurveyId = @ProgramDayItemSurveyId  AND WeekId = @WeekId)
            BEGIN
                UPDATE CompletedQuestionYesNoes
                SET YesNoValue = @YesNoValue, CompletedDate = @CompletedDate
                WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgramId = @AssignedProgramId AND ProgramDayItemSurveyId = @ProgramDayItemSurveyId  AND WeekId = @WeekId
            END
            ELSE
            BEGIN
                INSERT INTO [dbo].[CompletedQuestionYesNoes]
                           ([YesNoValue]
                           ,[AthleteId]
                           ,[QuestionId]
                           ,[CompletedDate]
                           ,[AssignedProgramId]
                           ,[ProgramDayItemSurveyId]
                           ,[WeekId])
                     VALUES
                           (@YesNoValue
                           ,@AthleteId
                           ,@QuestionId
                           ,@CompletedDate
                           ,@AssignedProgramId
                           ,@ProgramDayItemSurveyId
                            ,@WeekId)
            END";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insertStatement,
                    new
                    {
                        YesNoValue = answeredQuestion.YesNoValue,
                        AthleteId = answeredQuestion.AthleteId,
                        QuestionId = answeredQuestion.QuestionId,
                        CompletedDate = answeredQuestion.CompletedDate,
                        AssignedProgramId = answeredQuestion.AssignedProgramId,
                        ProgramDayItemSurveyId = answeredQuestion.ProgramDayItemSurveyId,
                        WeekId = answeredQuestion.WeekId
                    });
            }
        }
        public void AssignedProgram_CompleteScaleQuestion(m.Athlete.CompletedQuestionScale answeredQuestion)
        {
            var insertString = @"
            IF EXISTS(SELECT 1 FROM [AssignedProgram_CompletedQuestionScale] WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgram_ProgramId = @AssignedProgramId AND AssignedProgram_ProgramDayItemSurveyId = @ProgramDayItemSurveyId AND WeekId = @WeekId)
            BEGIN
                UPDATE [AssignedProgram_CompletedQuestionScale]
                SET ScaleValue = @ScaleValue, CompletedDate = @CompletedDate
                WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgram_ProgramId = @AssignedProgramId AND AssignedProgram_ProgramDayItemSurveyId = @ProgramDayItemSurveyId  AND WeekId = @WeekId
            END
            ELSE
            BEGIN
                                INSERT INTO[dbo].[AssignedProgram_CompletedQuestionScale]
                                        ([ScaleValue]
                                          ,[AthleteId]
                                          ,[QuestionId]
                                          ,[CompletedDate]
                                          ,[AssignedProgram_ProgramId]
                                          ,[AssignedProgram_ProgramDayItemSurveyId]
                                          ,[WeekId])
                                    VALUES
                                        (@ScaleValue
                                        ,@AthleteId
                                        ,@QuestionId
                                        ,@CompletedDate
                                        ,@AssignedProgramId
                                        ,@ProgramDayItemSurveyId
                                        ,@WeekId)
        END";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insertString,
                    new
                    {
                        ScaleValue = answeredQuestion.ScaleValue,
                        AthleteId = answeredQuestion.AthleteId,
                        QuestionId = answeredQuestion.QuestionId,
                        CompletedDate = answeredQuestion.CompletedDate,
                        AssignedProgramId = answeredQuestion.AssignedProgramId,
                        ProgramDayItemSurveyId = answeredQuestion.ProgramDayItemSurveyId,
                        WeekId = answeredQuestion.WeekId
                    });
            }
        }
        public void CompleteScaleQuestion(m.Athlete.CompletedQuestionScale answeredQuestion)
        {
            var insertString = @"
            IF EXISTS(SELECT 1 FROM CompletedQuestionScales WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgramId = @AssignedProgramId AND ProgramDayItemSurveyId = @ProgramDayItemSurveyId AND WeekId = @WeekId)
            BEGIN
                UPDATE CompletedQuestionScales
                SET ScaleValue = @ScaleValue, CompletedDate = @CompletedDate
                WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgramId = @AssignedProgramId AND ProgramDayItemSurveyId = @ProgramDayItemSurveyId  AND WeekId = @WeekId
            END
            ELSE
            BEGIN
                                INSERT INTO[dbo].[CompletedQuestionScales]
                                        ([ScaleValue]
                                          ,[AthleteId]
                                          ,[QuestionId]
                                          ,[CompletedDate]
                                          ,[AssignedProgramId]
                                          ,[ProgramDayItemSurveyId]
                                          ,[WeekId])
                                    VALUES
                                        (@ScaleValue
                                        ,@AthleteId
                                        ,@QuestionId
                                        ,@CompletedDate
                                        ,@AssignedProgramId
                                        ,@ProgramDayItemSurveyId
                                        ,@WeekId)
        END";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insertString,
                    new
                    {
                        ScaleValue = answeredQuestion.ScaleValue,
                        AthleteId = answeredQuestion.AthleteId,
                        QuestionId = answeredQuestion.QuestionId,
                        CompletedDate = answeredQuestion.CompletedDate,
                        AssignedProgramId = answeredQuestion.AssignedProgramId,
                        ProgramDayItemSurveyId = answeredQuestion.ProgramDayItemSurveyId,
                        WeekId = answeredQuestion.WeekId
                    });
            }
        }
        public void CompleteOpenEndedQuestion(m.Athlete.CompletedQuestionOpenEnded answeredQuestion)
        {
            var insertString = @"
            IF EXISTS(SELECT 1 FROM CompletedQuestionOpenEndeds WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgramId = @AssignedProgramId AND ProgramDayItemSurveyId = @ProgramDayItemSurveyId  AND WeekId = @WeekId)
            BEGIN
                UPDATE CompletedQuestionOpenEndeds
                SET Response = @Response, CompletedDate = @CompletedDate
                WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgramId = @AssignedProgramId AND ProgramDayItemSurveyId = @ProgramDayItemSurveyId  AND WeekId = @WeekId
            END
            ELSE
            BEGIN
                                INSERT INTO [dbo].[CompletedQuestionOpenEndeds]
                                ([Response]
                                ,[AthleteId]
                                ,[QuestionId]
                                ,[CompletedDate]
                                ,[AssignedProgramId]
                                ,[ProgramDayItemSurveyId]
                                ,[WeekId])
                                VALUES
                                (@Response
                                ,@AthleteId
                                ,@QuestionId
                                ,@CompletedDate
                                ,@AssignedProgramId
                                ,@ProgramDayItemSurveyId
                                ,@WeekId)
            END";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insertString,
                    new
                    {
                        Response = answeredQuestion.Response,
                        AthleteId = answeredQuestion.AthleteId,
                        QuestionId = answeredQuestion.QuestionId,
                        CompletedDate = answeredQuestion.CompletedDate,
                        AssignedProgramId = answeredQuestion.AssignedProgramId,
                        ProgramDayItemSurveyId = answeredQuestion.ProgramDayItemSurveyId,
                        WeekId = answeredQuestion.WeekId
                    });
            }
        }
        public void AssignedProgram_CompleteOpenEndedQuestion(m.Athlete.CompletedQuestionOpenEnded answeredQuestion)
        {
            var insertString = @"
            IF EXISTS(SELECT 1 FROM [dbo].[AssignedProgram_CompletedQuestionOpenEnded] WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgram_ProgramId = @AssignedProgramId AND AssignedProgram_ProgramDayItemSurveyId = @ProgramDayItemSurveyId  AND WeekId = @WeekId)
            BEGIN
                UPDATE AssignedProgram_CompletedQuestionOpenEnded
                SET Response = @Response, CompletedDate = @CompletedDate
                WHERE AthleteId = @AthleteId AND QuestionId = @QuestionId AND AssignedProgram_ProgramId = @AssignedProgramId AND AssignedProgram_ProgramDayItemSurveyId = @ProgramDayItemSurveyId  AND WeekId = @WeekId
            END
            ELSE
            BEGIN
                                INSERT INTO [dbo].[AssignedProgram_CompletedQuestionOpenEnded]
                                ([Response]
                                ,[AthleteId]
                                ,[QuestionId]
                                ,[CompletedDate]
                                ,[AssignedProgram_ProgramId]
                                ,[AssignedProgram_ProgramDayItemSurveyId]
                                ,[WeekId])
                                VALUES
                                (@Response
                                ,@AthleteId
                                ,@QuestionId
                                ,@CompletedDate
                                ,@AssignedProgramId
                                ,@ProgramDayItemSurveyId
                                ,@WeekId)
            END";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insertString,
                    new
                    {
                        Response = answeredQuestion.Response,
                        AthleteId = answeredQuestion.AthleteId,
                        QuestionId = answeredQuestion.QuestionId,
                        CompletedDate = answeredQuestion.CompletedDate,
                        AssignedProgramId = answeredQuestion.AssignedProgramId,
                        ProgramDayItemSurveyId = answeredQuestion.ProgramDayItemSurveyId,
                        WeekId = answeredQuestion.WeekId
                    });
            }
        }
        public List<DTOs.Survey.AthleteHomePageSurvey> GetHomePageSurveysByAssignedProgramId(int assignedProgramId, Guid createdUserGuid)
        {
            var getString = $@"select s.Title 'SurveyTitle',s.Id as 'SurveyId',ap.Id as 'AssignedProgramId'
                                from ProgramDayItemSurveys as pdis 
                                INNER JOIN Surveys AS s ON s.id = pdis.SurveyId 
                                INNER JOIN ProgramDayItems as pdi on pdis.ProgramDayItemId = pdi.id and pdi.ItemEnum = 2
                                INNER JOIN ProgramDays AS pd ON PDI.ProgramDayId = pd.Id
                                INNER JOIN programs as p on p.Id = pd.ProgramId 
                                INNER JOIN assignedPrograms AS ap ON AP.ProgramId = p.Id
                                INNER JOIN AthleteProgramHistories AS aph ON aph.AssignedProgramId = ap.Id
                                INNER JOIN Athletes AS a ON a.id = APH.AthleteId
                                where ap.id = @AssignedProgramId";

            //todo: for now we are not going to put any kindve security on this. In the future make sure only athletes of that coach or that caoch can view.
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<DTOs.Survey.AthleteHomePageSurvey>(getString, new { @AssignedProgramId = assignedProgramId, Token = createdUserGuid }).ToList();
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="athleteId"></param>
        /// <param name="createdUserToken">Can be either a coach id or a athlete Id</param>
        /// <returns></returns>
        public List<HistoricProgram> GetAllHistoricProgramsWithSurveys(int athleteId, Guid createdUserToken)
        {
            var getString = $@"  select p.id AS 'ProgramId',p.[name],aph.StartDate as 'StartDate', aph.AssignedProgramId
                                from ProgramDayItemSurveys as pdis 
                                INNER JOIN ProgramDayItems as pdi on pdis.ProgramDayItemId = pdi.id and pdi.ItemEnum = 2
                                INNER JOIN ProgramDays AS pd ON PDI.ProgramDayId = pd.Id
                                INNER JOIN programs as p on p.Id = pd.ProgramId 
                                INNER JOIN assignedPrograms AS ap ON AP.ProgramId = p.Id
                                INNER JOIN AthleteProgramHistories AS aph ON aph.AssignedProgramId = ap.Id
                                INNER JOIN Athletes AS a ON a.id = APH.AthleteId
                                where aph.AthleteId = @AthleteID AND (a.organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) OR a.AthleteUserID = ({ConstantSqlStrings.GetUserIdFromToken}))
                                select p.id AS 'ProgramId',p.[name],a.ProgramStartDate as 'StartDate', a.AssignedProgramId
                                from Athletes AS a 
                                INNER JOIN assignedPrograms AS ap ON AP.ProgramId = a.AssignedProgramId 
                                INNER JOIN programs as p on p.Id = ap.ProgramId
                                INNER JOIN ProgramDays AS pd ON PD.ProgramId = pd.Id
                                INNER JOIN ProgramDayItems as pdi on pdi.ProgramDayId = pd.id and pdi.ItemEnum = 2
                                INNER JOIN  ProgramDayItemSurveys as pdis ON pdis.ProgramDayItemId = pdi.Id 
                                where a.Id = @AthleteID";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<HistoricProgram>(getString, new { AthleteId = athleteId, Token = createdUserToken }).ToList();
            }
        }
        public void RemoveCoachesFromScaleThresholdNotification(int questionid)
        {
            var deleteString = $@"DELETE FROM [dbo].[ScaleThresholdToCoaches] WHERE ScaleThresholdId IN (SELECT ID from [ScaleQuestionThresholds] WHERE Questionid = @QuestionId )";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(deleteString, new { QuestionId = questionid });
            }
        }
        public void RemoveCoachesFromYesNoThresholdNotification(int questionId)
        {
            var deleteString = $@"DELETE FROM [dbo].[YesNoQuestionThresholdToCoaches] WHERE YesNoThresholdId IN (SELECT ID from [YesNoQuestionThresholds]  WHERE Questionid = @QuestionId )";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(deleteString, new { QuestionId = questionId });
            }
        }
        public void AddCoachEmailToScaleThresholdNotification(int scaleThresholdId, List<int> coachIds)
        {
            var insertString = $@"INSERT INTO [dbo].[ScaleThresholdToCoaches]
                                       ([UserId]
                                       ,[ScaleThresholdId])
                                 VALUES
                                       (@CoachId
                                       ,@ScaleThresholdId)";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                coachIds.ForEach(x =>
                {
                    sqlConn.Execute(insertString, new { CoachId = x, ScaleThresholdId = scaleThresholdId });
                });
            }
        }
        public void AddCoachEmailToYesNoThresholdNotification(int yesNoThresholdId, List<int> coachIds)
        {
            var insertString = $@"   INSERT INTO [dbo].[YesNoQuestionThresholdToCoaches]
                               ([UserId]
                               ,[YesNoThresholdId])
                         VALUES
                               (@CoachId
                               ,@YesNoThresholdId); ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                coachIds.ForEach(x =>
                {
                    sqlConn.Execute(insertString, new { CoachId = x, YesNoThresholdId = yesNoThresholdId });
                });
            }
        }
        public int AddScaleThreshold(int questionId, Models.Enums.QuestionThresholdEnum comparer, int value)
        {
            var insertString = $@"INSERT INTO [dbo].[ScaleQuestionThresholds]
                                               ([QuestionId]
                                               ,[Comparer]
                                               ,[ThresholdValue])
                                         VALUES
                                               (@QuestionId
                                               ,@Comparer
                                               ,@Value);

                                SELECT SCOPE_IDENTITY();";


            var updateQuestionModify = $@"UPDATE questions SET CanModify = 0 WHERE Id = @QuestionId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                sqlConn.Execute(updateQuestionModify, new { QuestionId = questionId });
                return sqlConn.ExecuteScalar<int>(insertString, new { QuestionId = questionId, Comparer = (int)comparer, Value = value });
            }
        }
        public void RemoveScaleThreshold(int questionId)
        {
            var deleteString = $@"DELETE FROM [dbo].[ScaleQuestionThresholds] WHERE QuestionId = @QuestionId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(deleteString, new { QuestionId = questionId });
            }
        }
        public void RemoveYesNoThreshold(int questionId)
        {
            var deleteString = $@"DELETE FROM [dbo].[YesNoQuestionThresholds]WHERE QuestionId = @QuestionId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(deleteString, new { QuestionId = questionId });
            }
        }
        public void UpdateScaleThreshold(int id, int value, QuestionThresholdEnum comparer)
        {
            var updateString = $@"UPDATE [dbo].[ScaleQuestionThresholds]
                                       SET [Comparer] = @Comparer
                                          ,[ThresholdValue] = @Value
                                     WHERE Id = @Id";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Comparer = comparer, Value = value, Id = id });
            }
        }
        public int AddYesNoThreshold(int questionId, bool threshold)
        {
            var insertString = $@"INSERT INTO [dbo].[YesNoQuestionThresholds]
                                           ([QuestionId]
                                           ,[Threshold])
                                     VALUES
                                           (@QuestionId
                                           ,@Threshold);  select scope_identity()";

            var updateQuestionModify = $@"UPDATE questions SET CanModify = 0 WHERE Id = @QuestionId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateQuestionModify, new { QuestionId = questionId });
                return sqlConn.ExecuteScalar<int>(insertString, new { QuestionId = questionId, Threshold = threshold });
            }

        }
        public void SendOutNotificationsForYesNow(int questionId, int athleteId, bool thresholdValue)
        {
            var updateSql = $@"DECLARE @AthleteName AS VARCHAR(MAX)
                               DECLARE @question AS VARCHAR(max)
                               SELECT @AthleteName = a.FirstName + ' ' + a.LastName FROM athletes AS a WHERE Id = @Athleteid
                               select @question = QuestionDisplayText from Questions where Id = @questionId

                            INSERT INTO Notifications(DestinationUserId,Description,Title,GeneratedAthleteId,[Type_Id],HasBeenViewed,SentDate)
                            SELECT UserId,@athleteName + ' responded to ""' + @question +'"" on ' + CONVERT(varchar,GetDate(),107) + ' with an answer of ' + @StringVersionThreshold, 'Survey Threshold Reachead for ' +  @athleteName ,@Athleteid,2,0,GetDate()
                            FROM [YesNoQuestionThresholds] as T
                            INNER JOIN [YesNoQuestionThresholdToCoaches] AS c ON T.ID = C.YesNoThresholdId
                            WHERE t.QuestionId = @QuestionId AND t.Threshold = @ThresholdValue";

            var stringAnswer = thresholdValue ? "YES" : "NO";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateSql, new { Athleteid = athleteId, QuestionId = questionId, StringVersionThreshold = stringAnswer, ThresholdValue = thresholdValue });
            }
        }
        public void SendOutNotificationsForScaleQuestion(int questionId, int athleteId, int answer)
        {
            var updateSQL = $@"
DECLARE @AthleteName AS VARCHAR(MAX)
DECLARE @question AS VARCHAR(max)
SELECT @AthleteName = a.FirstName + ' ' + a.LastName FROM athletes AS a WHERE Id = @Athleteid
select @question = QuestionDisplayText from Questions where Id = @questionId

INSERT INTO Notifications(DestinationUserId,Title,[Description],GeneratedAthleteId,[Type_Id],HasBeenViewed,SentDate)
SELECT UserId,'Survey Threshold Reachead for ' +  @athleteName ,@athleteName + ' responded to ""' + @question +'"" on ' + CONVERT(varchar,GetDate(),107) + ' with an answer of ' + CONVERT(varchar(10),@Answer),@Athleteid,2,0,GetDate()
FROM ScaleQuestionThresholds AS st
INNER JOIN ScaleThresholdToCoaches AS c ON st.Id = c.ScaleThresholdId
WHERE st.QuestionId = @QuestionId AND comparer = 1 and ThresholdValue = @Answer;

            INSERT INTO Notifications(DestinationUserId, Title,[Description], GeneratedAthleteId,[Type_Id], HasBeenViewed, SentDate)
SELECT UserId,'Survey Threshold Reachead for ' + @athleteName ,@athleteName + ' responded to ""' + @question + '"" on ' + CONVERT(varchar, GetDate(), 107) + ' with an answer of ' + CONVERT(varchar(10), @Answer),@Athleteid,2,0,GetDate()
FROM ScaleQuestionThresholds AS st
INNER JOIN ScaleThresholdToCoaches AS c ON st.Id = c.ScaleThresholdId
WHERE st.QuestionId = @QuestionId AND comparer = 2 AND ThresholdValue<@Answer;

            INSERT INTO Notifications(DestinationUserId, Title,[Description], GeneratedAthleteId,[Type_Id], HasBeenViewed, SentDate)
SELECT UserId,'Survey Threshold Reachead for ' + @athleteName ,@athleteName + ' responded to ""' + @question + '"" on ' + CONVERT(varchar, GetDate(), 107) + ' with an answer of ' + CONVERT(varchar(10), @Answer),@Athleteid,2,0,GetDate()
FROM ScaleQuestionThresholds AS st
INNER JOIN ScaleThresholdToCoaches AS c ON st.Id = c.ScaleThresholdId
WHERE st.QuestionId = @QuestionId AND comparer = 3 AND ThresholdValue > @Answer;

            INSERT INTO Notifications(DestinationUserId, Title,[Description], GeneratedAthleteId,[Type_Id], HasBeenViewed, SentDate)
SELECT UserId,'Survey Threshold Reachead for ' + @athleteName ,@athleteName + ' responded to ""' + @question + '"" on ' + CONVERT(varchar, GetDate(), 107) + ' with an answer of ' + CONVERT(varchar(10), @Answer),@Athleteid,2,0,GetDate()
FROM ScaleQuestionThresholds AS st
INNER JOIN ScaleThresholdToCoaches AS c ON st.Id = c.ScaleThresholdId
WHERE st.QuestionId = @QuestionId AND comparer = 4 AND ThresholdValue <= @Answer;

            INSERT INTO Notifications(DestinationUserId, Title,[Description], GeneratedAthleteId,[Type_Id], HasBeenViewed, SentDate)
SELECT UserId,'Survey Threshold Reachead for ' + @athleteName ,@athleteName + ' responded to ""' + @question + '"" on ' + CONVERT(varchar, GetDate(), 107) + ' with an answer of ' + CONVERT(varchar(10), @Answer),@Athleteid,2,0,GetDate()
FROM ScaleQuestionThresholds AS st
INNER JOIN ScaleThresholdToCoaches AS c ON st.Id = c.ScaleThresholdId
WHERE st.QuestionId = @QuestionId AND comparer = 5 AND ThresholdValue >= @Answer;



            ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateSQL, new { Athleteid = athleteId, QuestionId = questionId, Answer = answer });
            }
        }
    }
}
