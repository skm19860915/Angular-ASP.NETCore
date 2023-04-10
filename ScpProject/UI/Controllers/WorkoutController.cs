using System;
using System.Web.Http;
using BL;
using BL.CustomExceptions;
using System.Net.Http;
using System.Linq;
using vm = Controllers.ViewModels.SetAndRep;
using System.Collections.Generic;
using m = Models.SetsAndReps;
using blm = BL.BusinessObjects.SetsAndReps;
using System.Net;
using System.Web.Configuration;
using Models.SetsAndReps;

namespace Controllers.Controllers
{
    [RoutePrefix("api/Workout")]
    public class WorkoutController : ApiController
    {
        private IWorkoutManager wManager;
        public WorkoutController(IWorkoutManager workMan)
        {
            wManager = workMan;
        }
        [HttpPost, Route("ArchiveWorkout/{workoutId:int}")]
        public void ArchiveWorkout(int workoutId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            wManager.ArchiveWorkout(workoutId, userGuid);
        }
        [HttpPost, Route("DuplicateWorkout/{workoutId:int}")]
        public int DuplicateWorkout(int workoutId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return wManager.DuplicateWorkout(workoutId, userGuid);
        }
        [HttpPost, Route("UnArchiveWorkout/{workoutId:int}")]
        public void UnArchiveWorkout(int workoutId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            wManager.UnArchiveWorkout(workoutId, userGuid);
        }
        [HttpGet, Route("GetWorkoutDetails/{workoutId:int}")]
        public blm.WorkoutDetails GetWorkoutDetails(int workoutId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return wManager.GetWorkoutDetails(workoutId, userGuid);
        }
        [HttpGet, Route("GetWorkouts")]
        public List<blm.Workout> GetWorkouts()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return wManager.GetWorkout(userGuid);
        }
        [Route("CreateNewWorkout"), HttpPost]
        public HttpResponseMessage CreateNewWorkout(vm.WorkoutDetails newWorkout)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            var tagIds = !newWorkout.Tags.Any() ? new List<WorkoutTag>() : newWorkout.Tags.Select(x => new WorkoutTag() { Name = x.Name, Id = x.Id }).ToList();
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, wManager.CreateNewWorkout(ViewModelToModelWorkoutDTO(newWorkout), userGuid, tagIds));
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
        [Route("UpdateWorkout"), HttpPost]
        public void UpdateWorkout(vm.WorkoutDetails targetWorkout)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            var tagIds = !targetWorkout.Tags.Any() ? new List<WorkoutTag>() : targetWorkout.Tags.Select(x => new WorkoutTag() { Name = x.Name, Id = x.Id }).ToList();
            wManager.UpdateWorkout(ViewModelToModelWorkoutDTO(targetWorkout), userGuid, tagIds);
        }
        public m.Workout ViewModelToModelWorkoutDTO(vm.WorkoutDetails targetToConvert)
        {
            var ret = new m.Workout
            {
                Id = targetToConvert.Id,
                Name = targetToConvert.Name,
                Notes = targetToConvert.Notes,
                CreatedDate = targetToConvert.CreatedDate,
                TotalWorkout = new List<m.Week>(),
                Rest = targetToConvert.Rest,
                ShowWeight = targetToConvert.ShowWeight
            };
            {
                foreach (var x in targetToConvert.TotalWorkout)
                {
                    var newWeek = new m.Week()
                    {
                        Position = x.Position,
                        ParentWorkoutId = x.ParentWorkoutId,
                        SetsAndReps = x.SetsAndReps == null ? new List<m.Set>() : x.SetsAndReps.Select(s => new m.Set()
                        {
                            Id = s.Id,
                            ParentWeekId = s.ParentWeekId,
                            Percent = s.Percent,
                            Position = s.Position,
                            Reps = s.Reps,
                            Sets = s.Sets,
                            Weight = s.Weight,
                            Distance = s.Distance,
                            Minutes = s.Minutes,
                            Seconds = s.Seconds,
                            Other = s.Other,
                            RepsAchieved = s.RepsAchieved
                        }).ToList()
                    };

                    ret.TotalWorkout.Add(newWeek);
                }
                return ret;
            }
        }
    }
}
