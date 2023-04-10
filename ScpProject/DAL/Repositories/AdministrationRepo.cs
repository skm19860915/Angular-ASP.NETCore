using System;
using System.Data.SqlClient;
using Dapper;

namespace DAL.Repositories
{
    public interface IAdministrationRepo
    {
        void AddFeedBack(int userId, string feedback);
    }

    public class AdministrationRepo : IAdministrationRepo
    {
        private string ConnectionString;
        public AdministrationRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void AddFeedBack(int userId, string feedback)
        {
            var insertString = $@"
                                INSERT INTO[dbo].[FeedBacks]
                                ([feedBack]
                                    ,[UserId]
                                    ,[Sent])
                                VALUES
                                (@FeedBack
                                ,@UserId
                                ,@Time)";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insertString, new { FeedBack = feedback, UserId = userId, Time = DateTime.Now });
            }
        }
    }
}
