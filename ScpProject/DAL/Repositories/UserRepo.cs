using Dapper;
using Models.Organization;
using Models.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IUserRepo
    {
        int Create(User newUser);
        void DupeDemoDataToUser(User targetUser);
        bool EmailInUse(string email);
        bool FinishUserRegistration(string emailToken);
        User Get(Guid userToken);
        User Get(int Id);
        List<User> Get(List<int> Ids);
        User Get(string userName);
        User GetByEmail(string email);
        string GetStripeGuIdForOrganization(int OrganizationId);
        string GetStripeGuIdForUser(int UserId);
        UserVisitedStatus GetVisitedStatusForUser(int id);
        void InsertPasswordResetInfo(PasswordReset info);
        void OneTimeRegisterHeadCoach(string organizationName, string password, string userName, Guid emailValToken, int userId);
        PasswordReset PasswordTokenInfo(string passwordToken);
        void UpdateEmail(string email, int id);
        void UpdatePassword(string password, int Id);
        void UpdateStripeDetailsForOrganisation(int OrganizationId, StripeCustomerData stripeCustomerData, int planId, string stripeGuid);
        void UpdateUserInfo(string firstName, string lastName, string email, int userId);
        void UpdateUserPassword(string newPassword, int userId);
        void UpdateVisitedUserCount(string FieldName, int id);
        bool UserNameInUse(string userName);
        Task UpdateUserEmailValidationToken(Guid newEmailValidationToken, int userId);
    }

    public class UserRepo : IUserRepo
    {
        private string ConnectionString;
        public UserRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool UserNameInUse(string userName)
        {
            var getString = "SELECT 1 FROM users where userName = @UserName";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(getString, new { UserName = userName }) == 1;
            }
        }
        public bool EmailInUse(string email)
        {
            var getString = "SELECT TOP 1 1 FROM users where email = @Email";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(getString, new { Email = email }) == 1;
            }
        }

        public PasswordReset PasswordTokenInfo(string passwordToken)
        {
            var getString = "SELECT * FROM passwordResets where PasswordResetToken = @ResetToken";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.QueryFirstOrDefault<PasswordReset>(getString, new { ResetToken = passwordToken });
            }

        }

        public void UpdateVisitedUserCount(string FieldName, int id)
        {
            var updateString = "UPDATE Users SET " + FieldName + " = 1 " + " WHERE Id = " + id;
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString);
            }
        }

        public UserVisitedStatus GetVisitedStatusForUser(int id)
        {
            UserVisitedStatus userVisitedStatus = null;
            var selectString = "SELECT VisitedExercise, VisitedPrograms, VistedRosters, VisitedSurveys, VisitedSetsReps, VisitedCoachRoster, VisitedMetrics, VisitedProgramBuilder FROM Users where Id=" + id;
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var roleReader = sqlConn.ExecuteReader(selectString);
                while (roleReader.Read())
                {
                    userVisitedStatus = new UserVisitedStatus(roleReader.GetBoolean(0), roleReader.GetBoolean(1), roleReader.GetBoolean(2), roleReader.GetBoolean(4), roleReader.GetBoolean(3), roleReader.GetBoolean(6), roleReader.GetBoolean(5), roleReader.GetBoolean(7));
                }
            }
            return userVisitedStatus;
        }

        public void UpdateUserInfo(string firstName, string lastName, string email, int userId)
        {
            var updateString = $@"UPDATE [Users]
                                 SET FirstName = @Firstname, LastName = @LastName, Email=@Email
                                WHERE id = @Id";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { FirstName = firstName, LastName = lastName, Email = email, Id = userId });
            }
        }
        public void UpdateUserPassword(string newPassword, int userId)
        {
            var updateString = @"
                                UPDATE [dbo].[Users]
                                SET [Password] = @NewPassword
                                WHERE id = @TargetId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { NewPassword = newPassword, TargetId = userId });
            }
        }
        public void InsertPasswordResetInfo(PasswordReset info)
        {
            var insertResetinfo = @"INSERT INTO passwordResets (UserId, PasswordResetToken, IssuedInUTC, ExpiresInUTC)
                                    VALUES (@UserId, @PasswordResetToken, @IssuedInUTC, @ExpiresInUTC)";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.ExecuteScalar(insertResetinfo, new { UserId = info.UserId, PasswordResetToken = info.PasswordResetToken, IssuedInUTC = info.IssuedInUTC, ExpiresInUTC = info.ExpiresInUTC });
            }
        }
        public void OneTimeRegisterHeadCoach(string organizationName, string password, string userName, Guid emailValToken, int userId)
        {
            var updateShit = @"UPDATE users SET userName = @UserName, password = @Password WHERE emailValidationToken = @EmailToken;
                                Update Organizations SET name = @OrgName
                                WHERE createdUserId =  @UserId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.ExecuteScalar(updateShit, new { UserId = userId, UserName = userName, Password = password, EmailToken = emailValToken.ToString(), OrgName = organizationName });
            }
        }
        public async Task UpdateUserEmailValidationToken(Guid newEmailValidationToken, int userId)
        {
            var updateShit = @"UPDATE users SET  emailValidationToken = @EmailToken ,IsEmailValidated = 0  WHERE id =  @UserId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                await sqlConn.ExecuteScalarAsync(updateShit, new { UserId = userId, EmailToken = newEmailValidationToken.ToString() });
            }
        }
        /// <summary>
        /// Returns found user, if no user found returns new user();
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public User Get(string userName)
        {
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var ret = sqlConn.QueryFirstOrDefault<User>($"SELECT * FROM Users WHERE userName = @UserName", new { UserName = userName });
                return ret ?? new User();
            }
        }
        public User Get(int Id)
        {
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var ret = sqlConn.QueryFirst<User>($"SELECT * FROM Users WHERE Id = @Id", new { Id = Id });
                return ret ?? new User();
            }
        }
        public List<User> Get(List<int> Ids)
        {
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var ret = sqlConn.Query<User>($"SELECT * FROM Users WHERE Id in @Ids", new { Ids = Ids }).ToList();
                return ret ?? new List<User>();
            }
        }
        public void UpdateEmail(string email, int id)
        {
            var updateString = @"
                                UPDATE [dbo].[Users]
                                SET [Email] = @Email
                                WHERE id = @TargetId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Email = email, TargetId = id });
            }
        }
        public void UpdatePassword(string password, int Id)
        {
            var updateString = @"
                                UPDATE [dbo].[Users]
                                SET [Password] = @Password
                                WHERE id = @TargetId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Password = password, TargetId = Id });
            }
        }
        public User Get(Guid userToken)
        {

            using (var sqlConn = new SqlConnection(ConnectionString))
            {

                var ret = sqlConn.QuerySingleOrDefault<User>($"SELECT * FROM Users WHERE Id =({ConstantSqlStrings.GetUserIdFromToken}) ", new { Token = userToken });
                // var verifyOrgIsSignedUP = sqlConn.ExecuteScalar<int>($"select 1 from Organizations  where StripeGuid is not null and id = @id ", new { id = ret.OrganizationId });


                // var verifyOrgIsSignedUP = sqlConn.ExecuteScalar<int>($"select 1 from Organizations  where StripeGuid is not null and id = @id ", new { id = ret.OrganizationId });


                return ret ?? new User();
            }
        }
        public User GetByEmail(string email)
        {
            string getString = @"SELECT top 1 u.* FROM  users AS u WHERE u.email = @Email";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.QuerySingleOrDefault<User>(getString, new { Email = email });
            }
        }
        public bool FinishUserRegistration(string emailToken)
        {
            var getUserByTokenQuery = "SELECT id FROM users WHERE emailValidationToken = @Token and IsEmailValidated <> 0";
            var userUpdateString = "Update isEmailValidated = 1 WHERE Id = @userId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var user = sqlConn.QuerySingleOrDefault<User>(getUserByTokenQuery, new { Token = emailToken });
                if (user != null)
                {
                    sqlConn.Execute(userUpdateString, new { userId = user.Id });
                }
                return user == null;
            }

        }
        public int Create(User newUser)
        {
            var insertString = @"INSERT INTO [dbo].[Users] 
                       ([UserName]
                       ,[Password]
                       ,[IsCoach]
                       ,[Email]
                       ,[FailedEntryAttempts]
                       ,[LockedOut]
                       ,[ImageContainerName]
                       ,[IsDeleted]
                       ,FirstName
                       ,LastName
                       ,EmailValidationToken
                       ,IsEmailValidated
                       ,IsHeadCoach
                        ,OrganizationId
                        ,SignalRGroupID)
                       VALUES
                       (@userName,@password,@IsCoach,@Email,0,0,@ImageContainerName,0, @FirstName, @LastName, @EmailValidationToken, 0, @IsHeadCoach, @OrgId,newid()); SELECT SCOPE_IDENTITY()  ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return int.Parse(sqlConn.ExecuteScalar(insertString, new { OrgId = newUser.OrganizationId, IsHeadCoach = newUser.IsHeadCoach, EmailValidationToken = newUser.EmailValidationToken, ImageContainerName = newUser.ImageContainerName, UserName = newUser.UserName, Password = newUser.Password, IsCoach = newUser.IsCoach ? 1 : 0, Email = newUser.Email, FirstName = newUser.FirstName, LastName = newUser.LastName }).ToString());
            }
        }

        public string GetStripeGuIdForUser(int UserId)
        {
            String StripeGuid = "";
            var queryString = "Select StripeGuid from Organizations where Id in (SELECT OrganizationId FROM Users where Id=" + UserId + ")";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var roleReader = sqlConn.ExecuteReader(queryString);
                while (roleReader.Read())
                {
                    StripeGuid = roleReader.GetString(0);
                }
            }
            return StripeGuid;
        }


        public string GetStripeGuIdForOrganization(int OrganizationId)
        {
            String StripeGuid = "";
            var queryString = "Select StripeGuid from Organizations where Id =" + OrganizationId;
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var roleReader = sqlConn.ExecuteReader(queryString);
                while (roleReader.Read())
                {
                    StripeGuid = roleReader.GetString(0);
                }
            }
            return StripeGuid;
        }

        public void UpdateStripeDetailsForOrganisation(int OrganizationId, StripeCustomerData stripeCustomerData, int planId, string stripeGuid)
        {
            var updateStripe = @"UPDATE Organizations SET CurrentSubscriptionPlanId=@CurrentSubscriptionPlanId,StripeGuid=@StripeGuid, FirstName=@FirstName,LastName=@LastName, Phone=@Phone,Email=@Email,Address1=@Address1, Address2=@Address2, City=@City, State=@State, Country=@Country, Zip=@Zip where Id=" + OrganizationId;
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.ExecuteScalar(updateStripe, new { CurrentSubscriptionPlanId = planId, StripeGuid = stripeGuid, FirstName = stripeCustomerData.FirstName, LastName = stripeCustomerData.LastName, Phone = stripeCustomerData.Phone, Email = stripeCustomerData.Email, Address1 = stripeCustomerData.AddressLine1, Address2 = stripeCustomerData.AddressLine2, City = stripeCustomerData.City, State = stripeCustomerData.State, Country = stripeCustomerData.Country, Zip = stripeCustomerData.Zip });
            }
        }

        public void DupeDemoDataToUser(User targetUser)
        {

            var text = @"DupeDemoDataToUser";


            new Task(() =>
            {
                using (var sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();



                    var cmd = new SqlCommand("DupeDemoDataToUser", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param1 = new SqlParameter("@UserId", targetUser.Id);
                    SqlParameter param2 = new SqlParameter("@orgId", targetUser.OrganizationId);
                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.CommandTimeout = 300;

                    cmd.ExecuteNonQuery();
                }
            }).Start();
            //sqlConn.ExecuteAsync(text,
            //          new
            //          {
            //              UserId = targetUser.Id,
            //              OrgId = targetUser.OrganizationId

            //          }, commandType: CommandType.StoredProcedure, commandTimeout: 600);

        }
    }
}
