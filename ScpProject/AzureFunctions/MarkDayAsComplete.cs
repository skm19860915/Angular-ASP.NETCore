using System;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public static class MarkDayAsComplete
    {
        [FunctionName("MarkDayAsComplete")]
        public static void Run([TimerTrigger("0 */4 * * *")]TimerInfo myTimer, ILogger log)
        {
            new DAL.Repositories.ProgramRepo(Config.SqlConn).MarkAllDaysAsCompleteWithin24Hours(DateTime.UtcNow);
        }
    }
}
