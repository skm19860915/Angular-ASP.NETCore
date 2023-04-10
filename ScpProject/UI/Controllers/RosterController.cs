using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Controllers.ViewModels.Roster;
using b = BL.BusinessObjects;
using Controllers.ViewModels.Athlete;
using Models.Athlete;
using System.Threading.Tasks;

namespace Controllers.Controllers
{
    [RoutePrefix("api/Roster")]
    public class RosterController : ApiController
    {

        private IRosterManager _rosterManager;
        public RosterController(IRosterManager rosMan)
        {
            _rosterManager = rosMan;

        }

        [HttpPost, Route("FinishAssistantCoachRegistration/")]
        public void FinishAssistantCoachRegistration([FromBody] AssistantCoach_CompleteRegistration assistanCoachRegistration)
        {
            _rosterManager.FinishAssistantCoachRegistration(assistanCoachRegistration.UserName, assistanCoachRegistration.Password, assistanCoachRegistration.UserId, assistanCoachRegistration.EmailValidationToken);
        }


        public async Task AddAssistantCoach(AssistantCoachVM newCoach)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _rosterManager.GenerateUserRoles(userGuid);
            await _rosterManager.CreateAssistantCoach(newCoach.FirstName, newCoach.LastName, newCoach.Email, userGuid);
        }
        [HttpPost, Route("ResendCoachEmail/{coachId:int}")]
        public async Task ResendCoachEmail(int coachId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _rosterManager.GenerateUserRoles(userGuid);
           await _rosterManager.SendCoachRegistrationEmail(coachId);
        }
        [Route("AddAthlete"), HttpPost]
        public int AddAthlete(AthleteVM newAthlete)
        {
            var tagIds = !newAthlete.AthleteTags.Any() ? new List<AthleteTag>() : newAthlete.AthleteTags.Select(x => new AthleteTag() { Name = x.Name, Id = x.Id }).ToList();
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _rosterManager.GenerateUserRoles(userGuid);
            return _rosterManager.CreateAthlete(newAthlete.Athlete, tagIds, userGuid, newAthlete.Metrics, false);

        }
        [Route("ResendAthleteRegistartion/{id:int}"), HttpPost]
        public void ResendAthleteRegistartion(int id)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _rosterManager.GenerateUserRoles(userGuid);
            _rosterManager.SendAthleteRegistartion(id, userGuid);
        }
        [Route("GetAllAthletes"), HttpGet]
        public List<b.Athlete> GetAllAthletes()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _rosterManager.GenerateUserRoles(userGuid);
            return _rosterManager.GetAllAthletes(userGuid);
        }
        [Route("GetAllAthletesWithoutProgram/{pageCount:int}/{athleteCount:int}"), HttpGet]
        public DashboardAthleteWithoutProgram GetAllAthletesWithoutProgram(int pageCount, int athleteCount)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _rosterManager.GenerateUserRoles(userGuid);
            var tup = _rosterManager.GetAllAthletesWithoutProgram(userGuid, pageCount, athleteCount);
            return new DashboardAthleteWithoutProgram() { AthleteCount = tup.Item2, Athletes = tup.Item1 };
        }
        [Route("GetAthlete/{id:int}"), HttpGet]
        public b.Athlete GetAthlete(int id)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _rosterManager.GenerateUserRoles(userGuid);
            return _rosterManager.GetAthlete(userGuid, id);
        }
        [Route("CheckAssignedProgram"), HttpPost]
        public List<DAL.DTOs.Athlete.AssignedProgramAthleteDTO> CheckAssignedProgram(AthletesToCheck athleteIds)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _rosterManager.GenerateUserRoles(userGuid);
            return _rosterManager.CheckAthletesAssignedPrograms(athleteIds.AthleteIdsToCheck, userGuid);
        }

        [Route("AssignProgram"), HttpPost]
        public async Task AssignProgram(AssignProgramVM assignVM)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            //todo: move this to azure function cause it takes to long 
            //var userGuid = new Guid("D4EF913D-BD26-41AD-A0FD-6FA351A7E71B");
            _rosterManager.GenerateUserRoles(userGuid);

            await _rosterManager.AssignProgramToAthletes(assignVM.AthleteIds, assignVM.ProgramId, userGuid, DateTime.Now);// assignVM.StartDate);
        }
    }

    public class AthletesToCheck
    {
        public List<int> AthleteIdsToCheck { get; set; }
    }
}
