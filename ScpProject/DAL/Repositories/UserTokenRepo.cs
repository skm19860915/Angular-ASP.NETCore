using System;
using Dapper;
using System.Data.SqlClient;

namespace DAL.Repositories
{
    public interface IUserTokenRepo
    {
        void DeleteOldTokens(Guid token);
        void DeleteOldTokens(int userId);
        Guid GenerateNewToken(int userId);
        Guid GetExistingTokenOrGenerateNewOne(int userId);
    }

    public class UserTokenRepo : IUserTokenRepo
    {
        private string ConnectionString;
        public UserTokenRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public Guid GenerateNewToken(int userId)
        {
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var newGuid = Guid.NewGuid();
                sqlConn.Execute($"INSERT INTO UserTokens (UserId, Token) VALUES ( @UserId,@Token)", new { UserId = userId, Token = newGuid.ToString() });
                return newGuid;
            }
        }
        public Guid GetExistingTokenOrGenerateNewOne(int userId)
        {
            var newGuid = Guid.NewGuid();
            var query = @" IF EXISTS(SELECT Token FROM userTokens WHERE userId = @UserId)
                          BEGIN
                            SELECT Token FROM userTokens WHERE userId = @UserId
                          END
                          ELSE
                          BEGIN
                            INSERT INTO UserTokens (UserId, Token) VALUES ( @UserId,@Token)
                            SELECT @token
                           END
                           ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var s = sqlConn.ExecuteScalar(query, new { UserId = userId, Token = newGuid.ToString() }); ;
                return (Guid)sqlConn.ExecuteScalar(query, new { UserId = userId, Token = newGuid.ToString() });
            }
        }

        public void DeleteOldTokens(int userId)
        {
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute($"DELETE FROM UserTokens WHERE UserId = @UserId", new { UserId = userId });
            }
        }
        public void DeleteOldTokens(Guid token)
        {
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute($"DELETE FROM UserTokens WHERE Token = @Token", new { Token = token });
            }
        }
    }
}
