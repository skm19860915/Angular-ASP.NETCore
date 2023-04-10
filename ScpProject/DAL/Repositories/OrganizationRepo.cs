using Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DAL.DTOs;
using Models.Payment;
using Models.Organization;
using Models.User;

namespace DAL.Repositories
{
    public interface IOrganizationRepo
    {
        void AddImage(int pictureId, Guid createdUserToken);
        void AddPrimaryColor(string primaryColorHex, Guid createdUserToken);
        void AddPrimaryFontColor(string primaryFontColorHex, Guid createdUserToken);
        void AddSecondaryColor(string secondaryColorHex, Guid createdUserToken);
        void AddSecondaryFontColor(string secondaryFontColorHex, Guid createdUserToken);
        void AddSubscriptionAudit(User approver, int previousPlanId, int newPlanId);
        void AssignRole(int targetCoach, OrganizationRoleEnum newRole, int organizationId, int assignedByUserId);
        void BadCreditCard(int orgId);
        void CancelSubscription(string stripeGuid);
        void CardExpiring(string stripeGuid);
        void ChargeFailed(string stripeGuid);
        int CreateOrganization(string name);
        void CreditCardExpiring(int orgId);
        void CreditCardUpdated(int orgId);
        void DeleteCoach(int userId, int headCoachId);
        void DeleteOrganization(string name);
        void DeleteOrganizationFromBadRegistration(int orgId);
        bool DoesOrganizationExist(string name);
        void FinishAssistantCoachRegistration(string userName, string password, int userId, string emailValidation);
        List<AssitantCoach> GetAllCoaches(Guid userToken, bool includeHeadCoach = false);
        List<Organization> GetAllOrgs();
        Organization GetOrg(int id);
        Organization GetOrg(string stripGuide);
        Tuple<int, int> GetOrganizationAthleteCountStatus(int orgId);
        List<Role> GetRoles();
        List<SubscriptionType> GetSubscriptionInfo();
        List<OrganizationRoleEnum> GetUserRoles(Guid userToken);
        void MarkOrgNoLongerCustomer(int orgId);
        void MarkOrgsWithoutCustomers(List<int> orgsIdsWithoutValidCustomer);
        void PlanUpgradeAuthLog(int userId, string firstName, string lastName, int oldPlanId, int newPlanId, int orgId);
        void RemoveRole(int targetCoach, OrganizationRoleEnum newRole, int organizationId);
        void ResetCardInfo(string stripeGuid, string paymentMethod);
        void ResetSubscription(string stripeGuid);
        void SubscriptionEnded(int orgId);
        void SubscriptionStarted(int orgId, int subscripritonPlanId);
        void UpdatedCreditCard(int orgId);
        void UpdateOrganizationName(string orgName, Guid createdUserToken);
        void UpdateOrganizationPlan(int orgId, int subscriptionId);
        void UpdateOrganizationWithSessionData(int OrganizationId, string SessionId, int planId);
        void UpdateOrganizationWithStripeData(string CustomerId, int OrgId, int subscriptionPlan);
        void UpdateOrganizationWithStripeData(string CustomerId, string SessionId);
    }

    public class OrganizationRepo : IOrganizationRepo
    {
        private string ConnectionString;
        public OrganizationRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void ChargeFailed(string stripeGuid)
        {
            var updateString = @"Update Organizations
                           Set StripeFailedToProcess = 1
                            where stripeGuid = @StripeGuid";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { StripeGuid = stripeGuid });
            }
        }
        public void CardExpiring(string stripeGuid)
        {
            var updateString = @"Update Organizations
                                 Set ExpiredCard = 1
                                 where stripeGuid = @StripeGuid";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { StripeGuid = stripeGuid });
            }
        }
        public void CancelSubscription(string stripeGuid)
        {
            var updateString = @"Update Organizations
                                 Set IsCustomer = 0
                                where stripeGuid = @StripeGuid";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { StripeGuid = stripeGuid });
            }
        }
        public void ResetSubscription(string stripeGuid)
        {
            var updateString = @"Update Organizations
                                 Set IsCustomer = 1
                                where stripeGuid = @StripeGuid";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { StripeGuid = stripeGuid });
            }
        }
        public void MarkOrgsWithoutCustomers(List<int> orgsIdsWithoutValidCustomer)
        {

            var updateString = "UPDATE Organizations SET IsCustomer = 0 0 WHERE id IN @Ids";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Ids = orgsIdsWithoutValidCustomer });
            }
        }
        public void UpdateOrganizationName(string orgName, Guid createdUserToken)
        {
            var updateString = $@" UPDATE Organizations
                       SET [Name] = @Name
                       WHERE Id = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { Name = orgName, Token = createdUserToken });
            }
        }
        public void AddImage(int pictureId, Guid createdUserToken)
        {
            var updateString = $@" UPDATE Organizations
                       SET ProfilePictureId = @pictureId
                       WHERE Id =( {ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { PictureId = pictureId, Token = createdUserToken });
            }
        }
        public void AddPrimaryColor(string primaryColorHex, Guid createdUserToken)
        {
            var updateString = $@" UPDATE Organizations
                       SET PrimaryColorHex = @PrimaryColorHex
                       WHERE Id = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { PrimaryColorHex = primaryColorHex, Token = createdUserToken });
            }
        }
        public void AddSecondaryColor(string secondaryColorHex, Guid createdUserToken)
        {
            var updateString = $@" UPDATE Organizations
                       SET SecondaryColorHex = @SecondaryColorHex
                       WHERE Id = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { SecondaryColorHex = secondaryColorHex, Token = createdUserToken });
            }
        }

        public void AddPrimaryFontColor(string primaryFontColorHex, Guid createdUserToken)
        {
            var updateString = $@" UPDATE Organizations
                       SET PrimaryFontColorHex = @PrimaryFontColorHex
                       WHERE Id = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { PrimaryFontColorHex = primaryFontColorHex, Token = createdUserToken });
            }
        }
        public void AddSecondaryFontColor(string secondaryFontColorHex, Guid createdUserToken)
        {
            var updateString = $@" UPDATE Organizations
                       SET SecondaryFontColorHex = @SecondaryFontColorHex
                       WHERE Id = ({ConstantSqlStrings.GetOrganizationIdByToken})";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { SecondaryFontColorHex = secondaryFontColorHex, Token = createdUserToken });
            }
        }
        public List<Organization> GetAllOrgs()
        {
            var getString = "SELECT * FROM Organizations ";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Organization>(getString).ToList();
            }
        }
        public void MarkOrgNoLongerCustomer(int orgId)
        {
            var updateString = "UPDATE Organizations SET IsCustomer = 0 WHERE id = @OrgId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { OrgId = orgId });
            }

        }
        public void AddSubscriptionAudit(User approver, int previousPlanId, int newPlanId)
        {
            var insertString = $@"INSERT INTO [dbo].[SubscriptionApprovalAudits]
                                           ([ApprovalFirstName]
                                           ,[ApprovalLastName]
                                           ,[PreviousPlanId]
                                           ,[NewPlanId]
                                           ,[OrganizationId]
                                           ,[ApprovalTime]
                                           ,[UserId])
                                     VALUES
                                           (@FirstName
                                           ,@LastName
                                           ,@PreviousPlanId
                                           ,@NewPlanID
                                           ,@OrgId
                                           ,@ApprovalTime
                                           ,@UseriD)
                                    ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insertString, new
                {
                    FirstName = approver.FirstName,
                    LastName = approver.LastName,
                    PreviousPlanId = previousPlanId,
                    NewPlanId = newPlanId,
                    ApprovalTime = DateTime.Now,
                    UserId = approver.Id,
                    OrgId = approver.OrganizationId,

                });
            }
        }

        public void UpdateOrganizationPlan(int orgId, int subscriptionId)
        {
            var updateString = "Update Organizations set CurrentSubscriptionPlanId = @PlanId WHERE Id = @Id";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.QueryFirstOrDefault<Organization>(updateString, new { PlanId = subscriptionId, Id = orgId });
            }
        }
        public Organization GetOrg(int id)
        {
            var getString = "SELECT * FROM Organizations where ID = @Id";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.QueryFirstOrDefault<Organization>(getString, new { Id = id });
            }
        }
        public void CreditCardExpiring(int orgId)
        {
            var updateString = "UPDATE Organizations SET CreditCardExpiring = 1 WHERE id = @OrgId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { OrgId = orgId });
            }
        }
        public void CreditCardUpdated(int orgId)
        {
            var updateString = "UPDATE Organizations SET CreditCardExpiring = 0 WHERE id = @OrgId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { OrgId = orgId });
            }
        }
        public void SubscriptionEnded(int orgId)
        {
            var updateString = "UPDATE Organizations SET SubscriptionEnded = 1 CurrentSubscriptionPlanId = null WHERE id = @OrgId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { OrgId = orgId });
            }
        }
        public void SubscriptionStarted(int orgId, int subscripritonPlanId)
        {
            var updateString = "UPDATE Organizations SET SubscriptionEnded = 1, CurrentSubscriptionPlanId = @NewSubPlan WHERE id = @OrgId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { OrgId = orgId, NewSubPlan = subscripritonPlanId });
            }
        }
        public Organization GetOrg(string stripGuide)
        {
            var getString = "SELECT * FROM Organizations where StripeGuid = @StripGuid";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.QueryFirstOrDefault<Organization>(getString, new { StripeGuid = stripGuide });
            }
        }

        public void BadCreditCard(int orgId)
        {
            var updateString = "UPDATE Organizations SET BadCreditCard = 1 WHERE id = @OrgId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { OrgId = orgId });
            }
        }
        public void UpdatedCreditCard(int orgId)
        {
            var updateString = "UPDATE Organizations SET BadCreditCard = 0  WHERE id = @OrgId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { OrgId = orgId });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns>Tuple(CurrentAthleteCount,MaxAthleteCount)</currentAthleteCount></returns>
        public Tuple<int, int> GetOrganizationAthleteCountStatus(int orgId)
        {


            var getString = $@"
                            SELECT count(A.ID) AS athleteCount,
                                (SELECT athleteCount
                                FROM SubscriptionTypes AS sub
                                INNER JOIN Organizations AS odub ON sub.id = odub.CurrentSubscriptionPlanId WHERE odub.id = @orgId) as maxAthletes
                            FROM organizations AS o
                            INNER JOIN athletes AS a ON A.OrganizationId = O.ID
                            WHERE o.id = @orgId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var reader = sqlConn.ExecuteReader(getString, new { orgId = orgId });
                reader.Read();
                var newTuple = new Tuple<int, int>(reader.GetInt32(0), reader.GetInt32(1));
                return newTuple;
            }
        }
        public List<SubscriptionType> GetSubscriptionInfo()
        {
            var getallSubscriptionsString = "SELECT * FROM SubscriptionTypes where recurring = 1 order by athleteCount asc";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<SubscriptionType>(getallSubscriptionsString).ToList();
            }
        }
        public bool DoesOrganizationExist(string name)
        {
            var getString = $@"select 1 from Organizations where [name] = @name";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var ret = sqlConn.ExecuteScalar<int>(getString, new { Name = name }) == 1;
                return ret;
            }
        }
        public void PlanUpgradeAuthLog(int userId, string firstName, string lastName, int oldPlanId, int newPlanId, int orgId)
        {
            var insert = $@"
                            INSERT INTO [dbo].[SubscriptionApprovalAudits]
                                       ([ApprovalFirstName]
                                       ,[ApprovalLastName]
                                       ,[PreviousPlanId]
                                       ,[NewPlanId]
                                       ,[OrganizationId]
                                       ,[ApprovalTime]
                                       ,[UserId])
                                 VALUES
                                       (@UserFirstName
                                       ,@UserLastName
                                       ,@PreviousPlanId
                                       ,@NewPlanId
                                       ,@OrgId
                                       ,getDate()
                                       ,@UserId
                            GO";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(insert, new { UserFirstName = firstName, UserLastName = lastName, NewPlanId = newPlanId, OrgId = orgId, UserId = userId });
            }

        }
        public int CreateOrganization(string name)
        {
            var createString = "INSERT INTO dbo.Organizations (Name, CreatedUserId,IsCustomer) Values (@Name, 0,0); SELECT SCOPE_IDENTITY()";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.ExecuteScalar<int>(createString, new { Name = name });
            }
        }

        public void UpdateOrganizationWithStripeData(string CustomerId, string SessionId)
        {
            var updateString = $@"UPDATE Organizations SET StripeGuid = @customerId,IsCustomer = 1,  HowMuchTheyOwe = 0  WHERE StripeGuid = @sessionId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new
                {
                    customerId = CustomerId,
                    sessionId = SessionId
                });
            }
        }
        public void UpdateOrganizationWithStripeData(string CustomerId, int OrgId, int subscriptionPlan)
        {
            var updateString = $@"UPDATE Organizations SET StripeGuid = @customerId,IsCustomer = 1, HasSubscription = 1, HowMuchTheyOwe = 0 , 
                                         CurrentSubscriptionPlanId = @SubscriptionPlan WHERE Id = @OrgId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new
                {
                    customerId = CustomerId,
                    OrgId = OrgId,
                    SubscriptionPlan = subscriptionPlan
                });
            }
        }
        public void ResetCardInfo(string stripeGuid, string paymentMethod)
        {
            var updateString = "Update Organizations set ExpiredCard 0, stripeFailedToProcess = 0,CreditCardExpiring = 0,BadCreditCard = 0, CurrentPaymentMethod = @PaymentMethod Organizations where StripeGuid = @StripGuid";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { StripeGuid = stripeGuid, PaymentMethod = paymentMethod });
            }
        }

        public void UpdateOrganizationWithSessionData(int OrganizationId, string SessionId, int planId)
        {
            var updateString = $@" UPDATE Organizations
                                    SET StripeGuid = @sessionId,
                                    CurrentSubscriptionPlanId = @PlanId
                                    WHERE Id = @organizationId";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new
                {
                    organizationId = OrganizationId,
                    sessionId = SessionId,
                    PlanId = planId
                });
            }
        }

        public void FinishAssistantCoachRegistration(string userName, string password, int userId, string emailValidation)
        {
            var updateString = $@" UPDATE u
                                    SET  u.UserName = @UserName, u.[Password] = @Password, IsEmailValidated = 1
                                    FROM users AS u
                                    WHERE u.id = @UserId and u.EmailValidationToken = @Token ";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new
                {
                    UserId = userId,
                    UserName = userName,
                    Password = password,
                    Token = emailValidation
                });
            }
        }

        /// <summary>
        /// THis will only delete new created/empty organizations. That is by design because we dont want to lose data
        /// </summary>
        /// <param name="Name"></param>
        public void DeleteOrganization(string name)
        {
            var deleteString = "DELETE FROM organizations WHERE name=@Name";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(deleteString, new { Name = name });
            }
        }
        /// <summary>
        ///this is ment to delete a newly created organization. If the delete fails then the organization isnt newly created and xszomeon is trying
        ///to fukc with this endpooint
        /// </summary>
        /// <param name="orgId"></param>
        public void DeleteOrganizationFromBadRegistration(int orgId)
        {
            //this is ment to delete a newly created organization. If the delete fails then the organization isnt newly created and xszomeon is trying
            //to fukc with this endpooint
            var deleteString = "DELETE FROM organizations WHERE id=@Id";
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(deleteString, new { Id = orgId });
            }
        }
        public void DeleteCoach(int userId, int headCoachId)
        {
            var updateString = $@"UPDATE Organizations set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE Programs set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE Athletes set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE AthleteInjuries set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE AthleteNotes set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE Pictures set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE Metrics set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE UnitOfMeasurements set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE Questions set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE Workouts set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE Exercises set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE Movies set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE Notes set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE Surveys set CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser;
                                UPDATE AddedMetrics SET EnteredbyUserId = @headCoachId where enteredByUserId = @deletedUser;
                                UPDATE Movies SET CreatedUserId = @headCoachId WHERE createdUserId = @deletedUser
                                DELETE FROM UserToOrganizationRoles WHERE userId = @deletedUser
                                DELETE FROM UserTokens where userId = @deletedUser
                                DELETE FROM users WHERE id = @deletedUser;";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(updateString, new { headCoachId = headCoachId, deletedUser = userId });
            }
        }

        public List<AssitantCoach> GetAllCoaches(Guid userToken, bool includeHeadCoach = false)
        {
            var ret = new List<AssitantCoach>();
            var roles = new List<RoleDTO>();
            var getCoachesString = $"SELECT id,firstName,lastName, IsDeleted FROM Users WHERE organizationId = (SELECT organizationId FROM users WHERE id = ({ConstantSqlStrings.GetUserIdFromToken})) AND iscoach = 1 ";
            if (!includeHeadCoach)
            {
                getCoachesString += " and isHeadCoach = 0";
            }
            var getCoachesRolesString = $@"SELECT uo.UserId,uo.OrganizationRoleId,orgRoles.Name FROM [dbo].[UserToOrganizationRoles] as UO 
                                            INNER JOIN users AS u  on u.id = uo.userId
                                            INNER JOIN [dbo].[OrganizationRoles] AS orgRoles ON orgRoles.Id = uo.OrganizationRoleId
                                            WHERE u.organizationId =  (SELECT organizationId FROM users WHERE id = ({ConstantSqlStrings.GetUserIdFromToken}))";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var coachReader = sqlConn.ExecuteReader(getCoachesString, new { Token = userToken });

                while (coachReader.Read())
                {
                    ret.Add(new AssitantCoach() { Roles = new List<Role>(), Id = coachReader.GetInt32(0), FirstName = coachReader.GetString(1), LastName = coachReader.GetString(2), IsDeleted = coachReader.GetBoolean(3) });
                }
            }
            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                var roleReader = sqlConn.ExecuteReader(getCoachesRolesString, new { token = userToken });
                while (roleReader.Read())
                {
                    roles.Add(new RoleDTO() { UserId = roleReader.GetInt32(0), RoleId = roleReader.GetInt32(1), Name = roleReader.GetString(2) });
                }
            }
            ret.ForEach(x =>
            {
                x.Roles = roles.Where(u => u.UserId == x.Id).Select(z => new Role() { Id = z.RoleId, Name = z.Name }).ToList();
            });
            return ret;
        }
        public List<OrganizationRoleEnum> GetUserRoles(Guid userToken)
        {
            var verifyRole = $"SELECT OrganizationRoleId FROM UserToOrganizationRoles WHERE UserId IN ({ConstantSqlStrings.GetUserIdFromToken})";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<OrganizationRoleEnum>(verifyRole, new { Token = userToken }).ToList();
            }
        }

        public List<Role> GetRoles()
        {
            var getRoles = $"SELECT Id as 'Id', Name as 'Name' FROM [OrganizationRoles]";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<Role>(getRoles).ToList();
            }
        }
        public void AssignRole(int targetCoach, OrganizationRoleEnum newRole, int organizationId, int assignedByUserId)
        {
            var indeleteRole = @"DELETE FROM userToOrganizationRoles WHERE userId = @UserId AND OrganizationRoleId = @roleId;
                                INSERT INTO userToOrganizationRoles (UserId,OrganizationRoleId,OrganizationId,AssignedByUserId)
                                VALUES (@UserId,@RoleId,@OrganizationId, @AssignedByUserId)";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(indeleteRole, new { UserId = targetCoach, RoleId = (int)newRole, OrganizationId = organizationId, AssignedByUserId = assignedByUserId });
            }
        }
        public void RemoveRole(int targetCoach, OrganizationRoleEnum newRole, int organizationId)
        {
            var deleteRole = @"DELETE FROM userToOrganizationRoles WHERE userId = @UserId AND OrganizationRoleId = @RoleId AND OrganizationId = @OrganizationId;";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Execute(deleteRole, new { UserId = targetCoach, RoleId = (int)newRole, OrganizationId = organizationId });
            }
        }
    }
}
