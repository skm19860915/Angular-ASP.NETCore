namespace DAL.Repositories
{
    public static class ConstantSqlStrings
    {
        public const string TokenJoin = " INNER JOIN userTokens AS ut ON ut.UserId = ";
        /// <summary>
        /// Gets UserId need to pass in @Token which is the targetUser access token. Returns 'UserId'
        /// </summary>
        public const string GetUserIdFromToken = "SELECT userId FROM UserTokens WHERE token = @token";
        /// <summary>
        /// Gets the OrgId need to pass in @token is the userTOekn is the target user Id, returns 'OrganizationId'
        /// </summary>
        public const string GetOrganizationIdByToken = @" SELECT OrganizationId FROM users AS u
                                                            INNER JOIN UserTokens as UT on u.Id = ut.UserId
                                                            WHERE token = @token";
    }
}
