using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DAL.DTOs;
using Dapper;
using Models.MultiMedia;
using DAL.CustomerExceptions;
using DAL.DTOs.MultiMedia;

namespace DAL.Repositories
{
    public interface IMultimediaRepo
    {
        void ArchiveMovie(int id, Guid token);
        int CreateMovie(string url, string title, Guid userToken);
        int CreateNote(string note, string title, Guid userToken);
        int CreatePicture(string url, string fileName, Guid userToken);
        void DeleteMovie(int movieId);
        List<Movie> GetAllMovies(Guid token);
        List<MovieWithTagsDTO> GetAllMovieTagMappings(Guid userToken);
        List<MultiMediaDTO> GetAllMultiMedia(Guid userToken);
        Movie GetMovie(int Id, Guid userToken);
        Note GetNote(int Id, Guid userToken);
        Picture GetPicture(int Id);
        Picture GetPicture(int Id, Guid userToken);
        void UnArchiveMovie(int id, Guid token);
        void UpdateMovie(string url, string title, int id, Guid token);
        void UpdateProfile(string profile, int pictureId);
        void UpdateThumbNailPicture(string thumbnail, int pictureId);
    }

    public class MultimediaRepo : IMultimediaRepo
    {
        private string ConnectionString;
        public MultimediaRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public Note GetNote(int Id, Guid userToken)
        {
            var getString = "SELECT id,title,NoteText FROM notes WHERE Id = @id, CreatedUserId = (" + ConstantSqlStrings.GetUserIdFromToken + ")";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Note>(getString, new { Id = Id, Token = userToken }).FirstOrDefault();
            }
        }
        public Picture GetPicture(int Id, Guid userToken)
        {
            var getString = "SELECT id,FileName, Thumbnail,Profile,Url FROM notes WHERE Id = @id, CreatedUserId = (" + ConstantSqlStrings.GetUserIdFromToken + ")";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Picture>(getString, new { Id = Id, Token = userToken }).FirstOrDefault();
            }
        }
        public Picture GetPicture(int Id)
        {
            var getString = "SELECT id,FileName, Thumbnail,Profile,Url FROM Pictures WHERE Id = @id";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Picture>(getString, new { Id = Id }).FirstOrDefault();
            }
        }
        public Movie GetMovie(int Id, Guid userToken)
        {
            var getString = $"SELECT * FROM movies WHERE Id = @id and organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Movie>(getString, new { Id = Id, Token = userToken }).FirstOrDefault();
            }
        }
        public void DeleteMovie(int movieId)
        {
            var delString = "DELETE FROM movies WHERE  id = @MovieId AND CanModify = 1";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(delString, new { MovieId = movieId });
            }

        }

        public int CreateNote(string note, string title, Guid userToken)
        {
            var insertString = "INSERT INTO [dbo].[Notes] "
                            + " ([NoteText] "
                            + " ,[Title] "
                            + " ,[CreatedUserId) "
                            + " VALUES "
                            + " (@NoteText,@Title,(" + ConstantSqlStrings.GetUserIdFromToken + "),0); SELECT SCOPE_IDENTITY() ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return int.Parse(sqlConn.ExecuteScalar(insertString, new { NoteText = note, Title = title, Token = userToken }).ToString());
            }
        }
        public int CreatePicture(string url, string fileName, Guid userToken)
        {
            var insertString = "INSERT INTO [dbo].[Pictures] "
                    + " ([URL] "
                    + " ,[FileName] "
                    + " ,[CreatedUserId]" +
                    ",[IsDeleted]) "
                    + " VALUES "
                    + " (@Url,@FileName,(" + ConstantSqlStrings.GetUserIdFromToken + "),0); SELECT SCOPE_IDENTITY() ";


            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return int.Parse(sqlConn.ExecuteScalar(insertString, new { URL = url, FIleName = fileName, Token = userToken }).ToString());
            }

        }
        public int CreateMovie(string url, string title, Guid userToken)
        {
            var insertString = $@"INSERT INTO [dbo].[Movies] 
                     ([URL] 
                     ,[Name] 
                     ,[CreatedUserId] 
                     ,[OrganizationId]
                     , [IsDeleted]
                     , [CanModify])
                     VALUES 
                     (@Url,@Title,({ ConstantSqlStrings.GetUserIdFromToken}),({ConstantSqlStrings.GetOrganizationIdByToken}), 0,1); SELECT SCOPE_IDENTITY() ";

            try
            {
                using (var sqlConn = new SqlConnection(ConnectionString))
                {
                    return int.Parse(sqlConn.ExecuteScalar(insertString, new { URL = url, Title = title, Token = userToken }).ToString());
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

        public void UpdateMovie(string url, string title, int id, Guid token)
        {

            var updateString = $"Update [dbo].[Movies] set [NAME] = @Name, [url] =@Url WHERE id = @Id and organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { @Name = title, @Id = id, @Url = url, Token = token });
            }
        }



        public List<MultiMediaDTO> GetAllMultiMedia(Guid userToken)
        {
            var sql = " SELECT ,n.Id AS 'MutliMediaId',n.Title AS 'Title',  (SELECT id FROM MediaTypes WHERE name = 'Note') AS 'MultiMediaType' "
                + $" from notes AS n  WHERE n.CreatedUserId = ({ConstantSqlStrings.GetUserIdFromToken})"
                + " UNION ALL "
                + " SELECT,n.Id AS 'MutliMediaId',n.Title AS 'Title',  (SELECT id FROM MediaTypes WHERE name = 'Image') AS 'MultiMediaType' "
                + $" from pictures AS n WHERE n.CreatedUserId = ({ConstantSqlStrings.GetUserIdFromToken})"
                + " UNION ALL "
                + " SELECT ,n.Id AS 'MutliMediaId',n.Title AS 'Title',  (SELECT id FROM MediaTypes WHERE name = 'Video') AS 'MultiMediaType' "
                + $" from movies AS n  WHERE n.CreatedUserId = ({ConstantSqlStrings.GetUserIdFromToken})";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<MultiMediaDTO>(sql, new { Token = userToken }).ToList();
            }
        }
        public void UpdateThumbNailPicture(string thumbnail, int pictureId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var text = " UPDATE PICTURES "
                          + $" set thumbnail = '{thumbnail}' "
                         + $" WHERE Id =  {pictureId}";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    var rowCount = cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateProfile(string profile, int pictureId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var text = " UPDATE PICTURES "
                         + $" set [profile] = '{profile}' "
                         + $" WHERE Id =  {pictureId}";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    var rowCount = cmd.ExecuteNonQuery();
                }
            }
        }

        public void ArchiveMovie(int id, Guid token)
        {
            var updateString = $"UPDATE [dbo].[Movies] set IsDeleted = 1 WHERE id = @Id and organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { @Id = id, Token = token });
            }
        }
        public void UnArchiveMovie(int id, Guid token)
        {
            var updateString = $"UPDATE [dbo].[Movies] set IsDeleted = 0 WHERE id = @Id and organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken}) ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { @Id = id, Token = token });
            }
        }
        public List<Movie> GetAllMovies(Guid token)
        {
            var getString = $"SELECT * FROM movies WHERE organizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Movie>(getString, new { Token = token }).ToList();
            }
        }

        public List<MovieWithTagsDTO> GetAllMovieTagMappings(Guid userToken)
        {
            var tagMappings = $"SELECT t.MovieId ,t.TagId, ta.Name FROM TagsToMovies AS t INNER JOIN MovieTags AS ta ON ta.Id = t.TagId INNER JOIN movies AS E on t.MovieId = e.Id WHERE e.OrganizationId = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            var tagMappingDTOs = new List<MovieWithTagsDTO>();
            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var reader = sqlConn.ExecuteReader(tagMappings, new { token = userToken });
                while (reader.Read())
                {
                    var targetDtoMapping = tagMappingDTOs.FirstOrDefault(x => x.MovieId == reader.GetInt32(0));
                    var newTagDTO = new TagDTO() { Id = reader.GetInt32(1), Name = reader.GetString(2) };
                    if (targetDtoMapping == null)
                    {

                        tagMappingDTOs.Add(new MovieWithTagsDTO() { MovieId = reader.GetInt32(0), Tags = new List<TagDTO>() { newTagDTO } });
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