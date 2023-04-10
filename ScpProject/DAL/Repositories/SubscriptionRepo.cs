using Models.Payment;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;



namespace DAL.Repositories
{
    public interface ISubscriptionRepo
    {
        List<SubscriptionType> GetAllSubscriptions();
        SubscriptionType GetSubscription(int subscriptionId);
    }

    public class SubscriptionRepo : ISubscriptionRepo
    {
        private string ConnectionString;
        public SubscriptionRepo(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public SubscriptionType GetSubscription(int subscriptionId)
        {
            var getSql = $@"Select id,name,athletecount,recurring,stripesubscriptionGuid,tiered from subscriptionTypes where id = @subscriptionId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<SubscriptionType>(getSql, new { SubscriptionId = subscriptionId }).First();
            }
        }
        public List<SubscriptionType> GetAllSubscriptions()
        {
            var getSql = $@"Select id,name,athletecount,recurring,stripesubscriptionGuid,tiered from subscriptionTypes where id = @subscriptionId";

            using (var sqlConn = new SqlConnection(ConnectionString))
            {
                return sqlConn.Query<SubscriptionType>(getSql).ToList();
            }
        }
    }
}
