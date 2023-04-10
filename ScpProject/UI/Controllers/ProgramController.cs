using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vm = Controllers.ViewModels;
using BL;
using b = BL.BusinessObjects;
using ddp = DAL.DTOs.Program;
using DAL.DTOs.Program;
using BL.CustomExceptions;
using System.Web.Configuration;
using Models.Program;
using DAL.Repositories;
using System.Threading.Tasks;

namespace Controllers.Controllers
{
    [RoutePrefix("api/Program")]
    public class ProgramController : ApiController
    {
        public bool LogEverything { get; set; }
        public IProgramManager _proMan;
        public IUserRepo _userRepo;

        public ProgramController(IProgramManager programManager, IUserRepo userRepo)
        {
            LogEverything = WebConfigurationManager.AppSettings["turnOnLogging"].ToLower() == "true";
            _proMan = programManager;
            _userRepo = userRepo;
        }


        [HttpGet, Route("HardDelete/{id:int}")]
        public void HardDelete(int id)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _proMan.HardDelete(id, userGuid);
        }


        [HttpPost, Route("CreateNewProgram")]
        public HttpResponseMessage CreateProgram([FromBody] vm.Program.ProgramDetailsVM newProgram)
        {
            var tagIds = !newProgram.Tags.Any() ? new List<ProgramTag>() : newProgram.Tags.Select(x => new ProgramTag() { Name = x.Name, Id = x.Id }).ToList();

            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _proMan.CreateProgram(VMProgramToModelsProgram(newProgram), userGuid, tagIds));
            }
            catch (ItemAlreadyExistsException iex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, iex.Message);
            }
            catch (ItemValidationError ex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, ex.Message);
            }
        }
        [HttpPost, Route("UpdateProgram")]
        public void UpdateProgram([FromBody] vm.Program.ProgramDetailsVM targetProgram)
        {
            var tagIds = !targetProgram.Tags.Any() ? new List<ProgramTag>() : targetProgram.Tags.Select(x => new ProgramTag() { Name = x.Name, Id = x.Id }).ToList();
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _proMan.UpdateProgram(VMProgramToModelsProgram(targetProgram), userGuid, tagIds);
        }

        [HttpPost, Route("UpdateSnapShotProgram/{athleteId:int}")]
        public async Task UpdateSnapShotProgram([FromBody] vm.Program.ProgramDetailsVM targetProgram, [FromUri] int athleteId)
        {
            var tagIds = !targetProgram.Tags.Any() ? new List<ProgramTag>() : targetProgram.Tags.Select(x => new ProgramTag() { Name = x.Name, Id = x.Id }).ToList();
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);

             await _proMan.UpdateAssignedSnapShot(VMProgramToModelsProgram(targetProgram), athleteId, userGuid);
        }

        [HttpPost, Route("MarkDayAsCompleted/{programDayId:int}/{weekNumber:int}/{athleteId:int}")]
        public async Task MarkDayAsCompleted(int programDayId, int weekNumber, int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            await _proMan.MarkDayAsComplete(userGuid, programDayId, weekNumber, athleteId);
        }
        [HttpGet, Route("GetAllPrograms")]
        public List<b.Program.Program> GetAllPrograms()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _proMan.GetAllPrograms(userGuid);
        }
        [HttpGet, Route("GetProgram/{id:int}")]
        public ddp.Program GetProgram(int id)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _proMan.GetProgramDetails(id, userGuid);
        }
        [HttpGet, Route("GetAllPastPrograms/{athleteId:int}")]
        public List<AthleteHomePagePastProgram> GetAllPastPrograms(int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _proMan.GetAllPastPrograms(athleteId);
        }
        public b.Program.Program VMProgramToModelsProgram(vm.Program.ProgramDetailsVM targetProgram)
        {

            return new b.Program.Program()
            {
                Id = targetProgram.Id,
                Name = targetProgram.Name,
                Days = targetProgram.Days,
                WeekCount = targetProgram.WeekCount
            };
        }
        [HttpPost, Route("DuplicateProgram/{programId:int}")]
        public void DuplicateProgram(int programId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _proMan.Duplicate(programId, userGuid);
        }
        [HttpPost, Route("ArchiveProgram/{programId:int}")]
        public void ArchiveProgram(int programId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _proMan.Archive(programId, userGuid);
        }
        [HttpPost, Route("UnArchiveProgram/{programId:int}")]
        public void UnArchiveProgram(int programId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _proMan.UnArchive(programId, userGuid);
        }
        [HttpPost, Route("PrintPDFProgram")]
        public void PrintProgram([FromBody] PdfPrintOptions printOptions)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            var user = _userRepo.Get(userGuid);
            if (LogEverything)
            {

                new LogRepo(WebConfigurationManager.ConnectionStrings["scp"].ConnectionString).LogShit(new Models.LogMessage()
                {
                    Message = "entering PrintProgram",
                    LoggedDate = DateTime.Now,
                    UserId = user.Id,
                    StackTrace = new System.Diagnostics.StackTrace().ToString()
                }); ;
            }

            _proMan.PrintAllAssignedAthletePrograms(printOptions.ProgramId,
                userGuid,
                printOptions.AthleteIdsToPrint,
                printOptions.PrintMasterPdf,
                printOptions.PrintSelectedAthletes,
                printOptions.PrintUsingAdvancedOptions);
            if (LogEverything)
            {
                new LogRepo(WebConfigurationManager.ConnectionStrings["scp"].ConnectionString).LogShit(new Models.LogMessage()
                {
                    Message = "exit PrintProgram",
                    LoggedDate = DateTime.Now,
                    UserId = user.Id,
                    StackTrace = new System.Diagnostics.StackTrace().ToString()
                }); ;
            }
        }
    }

    public class PdfPrintOptions
    {
        public int ProgramId { get; set; }
        public Boolean PrintMasterPdf { get; set; }
        public List<int> AthleteIdsToPrint { get; set; }
        public Boolean PrintSelectedAthletes { get; set; }
        public bool PrintUsingAdvancedOptions { get; set; }
    }
}