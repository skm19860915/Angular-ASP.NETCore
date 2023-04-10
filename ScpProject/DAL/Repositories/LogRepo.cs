using System;
using System.Data.SqlClient;
using Models;
using Dapper;

namespace DAL.Repositories
{
    public interface ILogRepo
    {
        void LogShit(LogMessage message);
    }

    public class LogRepo : ILogRepo
    {
        private string ConnectionString;
        public LogRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void LogShit(LogMessage message)
        {
            var logSql = @"INSERT INTO LogMessages (Message,StackTrace,UserId,LoggedDate) values (@message,@stackTrace,@userId,@loggedDate)";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(logSql, new { message = message.Message, stackTrace = message.StackTrace, userId = message.UserId, loggedDate = DateTime.Now });
            }
        }
    }
}
