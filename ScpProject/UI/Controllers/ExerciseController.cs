using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL;
using vm = Controllers.ViewModels;
using b = BL.BusinessObjects;
using BL.CustomExceptions;
using Models.Exercise;

namespace Controllers.Controllers
{
    [RoutePrefix("api/Exercises")]
    public class ExerciseController : ApiController
    {
        private IExerciseManager _exerciseManager;


        public ExerciseController(IExerciseManager exerciseMan)
        {
            _exerciseManager = exerciseMan;
        }

        [HttpGet, Route("HardDelete/{id:int}")]
        public void HardDelete(int id)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _exerciseManager.HardDelete(id, userGuid);
        }
        [HttpPost, Route("CreateNewExercise")]
        public HttpResponseMessage CreateExercise([FromBody] vm.Exercise.ExerciseVM newEx)
        {
            var tagIds = !newEx.Tags.Any() ? new List<ExerciseTag>() : newEx.Tags.Select(x => new ExerciseTag { Name = x.Name, Id = x.Id }).ToList();
            try
            {
                var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
                return Request.CreateResponse(HttpStatusCode.OK, _exerciseManager.CreateNewExercise(newEx.Notes, newEx.Name, tagIds, userGuid, newEx.Percent
                    , newEx.PercentMetricCalculationId, newEx.VideoURL, userGuid));
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
        [HttpPost, Route("UpdateExercise")]
        public HttpStatusCode UpdateExercise([FromBody] vm.Exercise.ExerciseVM newEx)
        {
            var tagIds = !newEx.Tags.Any() ? new List<ExerciseTag>() : newEx.Tags.Select(x => new ExerciseTag() { Id = x.Id, Name = x.Name }).ToList();
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _exerciseManager.UpdateExercise(newEx.Id, newEx.Notes, newEx.Name, tagIds, userGuid, newEx.Percent, newEx.PercentMetricCalculationId, newEx.VideoURL, userGuid);
            return HttpStatusCode.OK;


        }
        [HttpGet, Route("GetAllExercises")]
        public List<b.Exercise> GetAllExercises()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _exerciseManager.GetAllExercises(userGuid);
        }

        [HttpPost, Route("DuplicateExercise/{exerciseId:int}")]
        public int DuplicateExercise(int exerciseId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _exerciseManager.Duplicate(exerciseId, userGuid);
        }
        [HttpPost, Route("ArchiveExercise/{exerciseId:int}")]
        public void ArchiveExercise(int exerciseId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _exerciseManager.Archive(exerciseId, userGuid);
        }
        [HttpPost, Route("UnArchiveExercise/{exerciseId:int}")]
        public void UnArchiveExercise(int exerciseId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _exerciseManager.UnArchive(exerciseId, userGuid);
        }
    }
}
