using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctions
{
    public static class AssignProgramSnapShots
    {
        [FunctionName("AssignProgramSnapShots")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var snapshotInfo = JsonConvert.DeserializeObject<SnapshotInfo>(requestBody);

            var _programRepo = new DAL.Repositories.ProgramRepo(Config.SqlConn);
            var _athleteRepo = new DAL.Repositories.AthleteRepo(Config.SqlConn);


          var snapshotId =  await _programRepo.CreateProgramSnapShot(snapshotInfo.ProgramId, snapshotInfo.AthleteId, snapshotInfo.UserToken);

            await _athleteRepo.AssignProgramToAthlete(snapshotInfo.AthleteId, snapshotId.Item2);

            return new OkObjectResult("all good");
        }
    }

    public class SnapshotInfo
    {
        public int ProgramId { get; set; }
        public int AthleteId { get; set; }
        public Guid UserToken { get; set; }
    }
}
