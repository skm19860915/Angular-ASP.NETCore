using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Models.Tag;
using Models.Enums;
using Models.Athlete;
using System;
using Models.Exercise;
using Models.Program;
using Models.SetsAndReps;
using Models.MultiMedia;
using Models.Survey;
using Models.Metric;
using Models.Documents;

namespace DAL.Repositories
{
    public interface ITagRepo<T> where T : Tag
    {
        void AddAssociatedTags(List<T> tags, int objectId);
        int CreateTag(string name, string notes, int organizationId);
        void DeleteAssociatedTags(int objectId);
        List<T> GetAllTags(int organizationId);
        int GetTag(string tagName, int orgId);
        void InitializeTagRepo(TagEnum targetTagType);
        void InitializeTagRepo(System.Type targetTagType);
    }

    public class TagRepo<T> : ITagRepo<T> where T : Tag
    {
        private string _tagTable { get; set; }
        private string _tagToTable { get; set; }//linker table matching tag to object
        private string _typeOfTag { get; set; }
        private string ConnectionString;

        public TagRepo(string connectionString)
        {

            ConnectionString = connectionString;

        }
        public void InitializeTagRepo(System.Type targetTagType)
        {
            //cannot use a switch statement because, it requires constants
            if (typeof(AthleteTag).Equals(targetTagType))
            {
                this.InitializeTagRepo(TagEnum.Athlete);
            }
            else if (typeof(ExerciseTag).Equals(targetTagType))
            {
                this.InitializeTagRepo(TagEnum.Exercise);
            }
            else if (typeof(ProgramTag).Equals(targetTagType))
            {
                this.InitializeTagRepo(TagEnum.Program);
            }
            else if (typeof(WorkoutTag).Equals(targetTagType))
            {
                this.InitializeTagRepo(TagEnum.Workout);
            }
            else if (typeof(MovieTag).Equals(targetTagType))
            {
                this.InitializeTagRepo(TagEnum.Movie);
            }
            else if (typeof(SurveyTag).Equals(targetTagType))
            {
                this.InitializeTagRepo(TagEnum.Survey);
            }
            else if (typeof(MetricTag).Equals(targetTagType))
            {
                this.InitializeTagRepo(TagEnum.Athlete);
            }
            else if (typeof(DocumentTag).Equals(targetTagType))
            {
                this.InitializeTagRepo(TagEnum.Document);
            }
            else
            {
                throw new ApplicationException("Undeteremeind type of tag");
            }
        }
        public void InitializeTagRepo(TagEnum targetTagType)
        {
            switch (targetTagType)
            {
                case TagEnum.Exercise:
                    _tagTable = "ExerciseTags";
                    _tagToTable = "TagsToExercises";
                    _typeOfTag = "Exercise";
                    break;
                case TagEnum.Program:
                    _tagTable = "ProgramTags";
                    _tagToTable = "TagsToPrograms";
                    _typeOfTag = "Program";
                    break;
                case TagEnum.Workout:
                    _tagTable = "WorkoutTags";
                    _tagToTable = "TagsToWorkouts";
                    _typeOfTag = "Workout";
                    break;
                case TagEnum.Movie:
                    _tagTable = "MovieTags";
                    _tagToTable = "TagsToMovies";
                    _typeOfTag = "Movie";
                    break;
                case TagEnum.Athlete:
                    _tagTable = "AthleteTags";
                    _tagToTable = "TagsToAthletes";
                    _typeOfTag = "Athlete";
                    break;
                case TagEnum.Survey:
                    _tagTable = "SurveyTags";
                    _tagToTable = "TagsToSurveys";
                    _typeOfTag = "Survey";
                    break;
                case TagEnum.Metric:
                    _tagTable = "MetricTags";
                    _tagToTable = "TagsToMetrics";
                    _typeOfTag = "Metric";
                    break;
                case TagEnum.Document:
                    _tagTable = "DocumentTags";
                    _tagToTable = "TagsToDocument";
                    _typeOfTag = "Document";
                    break;
                default:
                    break;
            }
        }
        public void DeleteAssociatedTags(int objectId)
        {
            var tagDeleteString = $"DELETE FROM [dbo].[{_tagToTable}] WHERE [{_typeOfTag}Id] = {objectId}";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(tagDeleteString);
            }
        }
        /// <summary>
        /// You cannot use this method to insert tagsForMultimedia
        /// </summary>
        /// <param name="tagIds"></param>
        /// <param name="objectId"></param>
        public void AddAssociatedTags(List<T> tags, int objectId)
        {

            var tagInsertString = $@"INSERT INTO [dbo].[{_tagToTable}] 
                                 ([TagId] 
                                 ,[{_typeOfTag}Id]) 
                                 VALUES 
                                (@TagId,@ObjectId);";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                tags.ForEach(x =>
                {
                    sqlConn.Execute(tagInsertString, new { TagId = x.Id, ObjectId = objectId });
                });
            }
        }

        public int CreateTag(string name, string notes, int organizationId)
        {

            var insertString = $@"INSERT INTO [dbo].[{_tagTable}] 
                                ([Name] 
                               ,[OrganizationId] 
                               ,[Notes] 
                               ,[IsDeleted]) 
                                VALUES 
                                (@Name,@OrganizationId,@Notes,0); SELECT SCOPE_IDENTITY() ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return int.Parse(sqlConn.ExecuteScalar(insertString, new { Name = name, Notes = notes, OrganizationId = organizationId }).ToString());
            }
        }
        public List<T> GetAllTags(int organizationId)
        {
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var sqlQuery = $@"SELECT t.Name,t.Id,t.Notes,t.IsDeleted,organizationId
                                    FROM {_tagTable } AS t
                                    where OrganizationId = @OrganizationId";
                var ret = sqlConn.Query<T>(sqlQuery, new { OrganizationId = organizationId });
                return ret.ToList(); ;
            }
        }
        public int GetTag(string tagName, int orgId)
        {
            var getQuery = $@"Select id from {_tagTable} WHERE OrganizationId = @OrganizationId AND [Name] = @tagName";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return int.Parse(sqlConn.ExecuteScalar(getQuery, new { TagName = tagName, OrganizationId = orgId }).ToString());
            }
        }

    }
}
