using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL;
using Controllers.ViewModels.Athlete;
using Controllers.ViewModels.Roster;
using DAL.DTOs.Metrics;
using DAL.DTOs.Program;
using Models.Athlete;
using m = Models;

namespace Controllers.Controllers
{
    [RoutePrefix("api/Athletes")]
    public class AthleteController : ApiController
    {
        private IAthleteManager _athleteManager;
        private IProgramManager _programManager;
        public AthleteController(IAthleteManager athleteManager, IProgramManager progMan)
        {

            _athleteManager = athleteManager;
            _programManager = progMan;
        }
        [Route("GetSnapShotForModifying/{athleteId:int}")]
        public Program GetSnapShotForModifying(int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _programManager.GetSnapShotProgramDetails(athleteId, userGuid);
        }
        [Route("UpdateMetric"), HttpPost]
        public void UpdateMetric(ViewModels.Athlete.UpdateMetric newMetric)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _athleteManager.UpdateMetric(newMetric.Id, newMetric.Value, newMetric.CompletedDate, newMetric.IsCompleted,userGuid);
        }
        [Route("GetAthleteProgramHistory/{athleteId:int}"), HttpGet]
        public List<ProgramHistory> GetAthleteProgramHistory(int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _athleteManager.GetAthleteProgramHistory(athleteId,userGuid);
        }
        [Route("GetAthleteListOfCompletedMetrics/{athleteId:int}"), HttpGet]
        public List<CompletedMetricDisplay> GetAthleteListOfCompletedMetrics(int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _athleteManager.GetAthleteListOfCompletedMetrics(athleteId,userGuid);
        }
        [Route("GetMetricHistory/{metricId:int}/{athleteId:int}"), HttpGet]
        public List<CompletedMetricHistory> GetMetricHistory(int metricId, int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _athleteManager.GetMetricHistory(metricId, athleteId,userGuid);
        }
        [Route("PrintAthleteWorkout/{programId:int}/{athleteId:int}")]
        public void PrintAthletePDF(int programId, int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _athleteManager.PrintWorkout(userGuid, programId, athleteId);
        }

        [Route("UpdateAthlete"), HttpPost]
        public void UpdateAthlete(AthleteVM newAthleteInfo)
        {
            var tagIds = !newAthleteInfo.AthleteTags.Any() ? new List<AthleteTag>() : newAthleteInfo.AthleteTags.Select(x => new AthleteTag() { Name = x.Name, Id = x.Id }).ToList();
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _athleteManager.UpdateAthlete(newAthleteInfo.Athlete, tagIds, newAthleteInfo.Metrics, userGuid);
        }
        [HttpPost, Route("ArchiveAthlete/{athleteId:int}")]
        public void ArchiveAthlete(int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _athleteManager.ArchiveAthlete(athleteId, userGuid);
        }
        [HttpPost, Route("FinishAthleteRegistration/")]
        public void FinishAthleteRegistration([FromBody]CompleteRegistration athleteRegistration)
        {
            _athleteManager.FinishAthleteRegistration(athleteRegistration.UserName, athleteRegistration.Password, athleteRegistration.AthleteId, athleteRegistration.EmailValidationToken);
        }

        [HttpGet, Route("GetAssignedProgram/{assignedProgramId:int?}")]
        public DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAssignedProgram(int assignedProgramId = 0)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _athleteManager.GetAssignedProgram(userGuid, assignedProgramId);
        }
        [HttpGet, Route("GetAssignedProgramWeightRoomAccount/{athleteId:int}")]
        public DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAssignedProgramWeightRoomAccount(int athleteId )
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _athleteManager.GetAnAthletesAssignedProgram(userGuid, athleteId);
        }
        [HttpGet, Route("GetAnAthletesAssignedProgramByProgramId/{assignedProgramId:int}/{isSnapShot:int}/{athleteId:int}")]
        public DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAssignedProgram(int assignedProgramId,int isSnapShot, int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _athleteManager.GetAnAthletesAssignedProgram(userGuid, athleteId, assignedProgramId, isSnapShot == 1);
        }


        [HttpGet, Route("GetAnAthletesAssignedProgram/{athleteId:int}")]
        public DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAnAthletesAssignedProgram(int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _athleteManager.GetAnAthletesAssignedProgram(userGuid, athleteId);
        }

        [HttpPost, Route("AddCompletedSet")]
        public HttpResponseMessage AddCompletedSet([FromBody]m.Athlete.CompletedSet newSet)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _athleteManager.AddCompletedSet(newSet, userGuid);
            return Request.CreateResponse(HttpStatusCode.OK);

        }

        [HttpPost, Route("AddCompletedSuperSet")]
        public HttpResponseMessage AddCompletedSuperSet([FromBody]m.Athlete.CompletedSuperSet_Set newSet)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _athleteManager.AddCompletedSuperSet(newSet, userGuid);
            return Request.CreateResponse(HttpStatusCode.OK);

        }
        [HttpPost, Route("AddCompletedMetric")]
        public HttpResponseMessage AddCompletedMetric([FromBody]m.Athlete.CompletedMetric newMetric)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return Request.CreateResponse(HttpStatusCode.OK, _athleteManager.AddCompleteMetric(newMetric, userGuid));
        }

        [HttpPost, Route("markDayCompleted/{assignedProgramId:int}/{programDayId:int}")]
        public HttpResponseMessage MarkDayCompleted(int assignedProgramId, int programDayId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _athleteManager.MarkDayCompleted(assignedProgramId, programDayId, userGuid);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet, Route("FixDuplicateAccounts")]
        public HttpResponseMessage RemoveDuplicateAccounts()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _athleteManager.fixDuplicateAccounts();
            return Request.CreateResponse(HttpStatusCode.OK, "Success");
        }
    }
}
