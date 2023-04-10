using Dapper;
using Models.Organization;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace DAL.Repositories
{
    public interface IWeightRoomRepo
    {
        int CreateAccount(int organizationId, string name, string token);
        WeightRoomAccount Get(Guid token);
        WeightRoomAccount Get(int orgId);
    }

    public class WeightRoomRepo : IWeightRoomRepo
    {
        private string ConnectionString;
        public WeightRoomRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public int CreateAccount(int organizationId, string name, string token)
        {

            var insertString = @" INSERT INTO[dbo].[WeightRoomAccounts]
                                    ([Name]
                                    ,[Token]
                                    ,[OrganizationId])
                                    VALUES
                                    (@Name
                                    ,@Token
                                    ,@OrgId);  SELECT SCOPE_IDENTITY(); ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(insertString, new { Name = name, Token = token, OrgId = organizationId });
            }
        }
        public WeightRoomAccount Get(int orgId)
        {
            var getString = @"SELECT name,token,organizationId FROM WeightRoomAccounts where OrganizationId = @OrgId ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<WeightRoomAccount>(getString, new { OrgId = orgId }).FirstOrDefault();
            }
        }
        public WeightRoomAccount Get(Guid token)
        {
            var getString = @"SELECT name,token,organizationId FROM WeightRoomAccounts where Token = @Token ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<WeightRoomAccount>(getString, new { Token = @token }).FirstOrDefault();
            }
        }
    }
}
