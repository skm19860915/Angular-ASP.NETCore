using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using vm = Controllers.ViewModels;
using BL;
using m = Models;
using System.Web.Configuration;
using Ninject;
using Models.SetsAndReps;
using DAL;
using DAL.Repositories;
using Models.Exercise;
using Models.Enums;

namespace Controllers.Controllers
{
    [RoutePrefix("api/Tag")]
    public class TagController : ApiController
    {
        /// <summary>
        /// In Implementation and Practice the Tag class is used to make the coding easier. ALl the tags will always share the same data, if they dont
        /// then you are doing something wrong. 1 Tag should not have any data that another tag wouldnt
        /// </summary>

        public TagController() { }

        [HttpPost, Route("CreateTag")]
        public int CreateTag([FromBody] vm.Tag.Tag newTag)
        {
            var conString = WebConfigurationManager.ConnectionStrings["scp"].ConnectionString;
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);

            var userRepo = new UserRepo(conString);
            var weightRoom = new WeightRoomRepo(conString);
            var tagRepo = new TagRepo<Models.Tag.Tag>(conString);
            return new TagManager<Models.Tag.Tag>(userRepo, tagRepo, weightRoom).CreateTag((TagEnum) newTag.Type, newTag.Name,string.Empty, userGuid);
        }
        [HttpGet, Route("GetAllTags/{tagType:int}")]
        public List<vm.Tag.Tag> GetTags(m.Enums.TagEnum tagType)
        {
            var conString = WebConfigurationManager.ConnectionStrings["scp"].ConnectionString;
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);

            var userRepo = new UserRepo(conString);
            var weightRoom = new WeightRoomRepo(conString);
            switch (tagType)
            {
                case m.Enums.TagEnum.Exercise:
                    var tagRepo = new TagRepo<ExerciseTag>(conString);
                    return new TagManager<m.Exercise.ExerciseTag>(userRepo, tagRepo, weightRoom).GetAllTags(TagEnum.Exercise, userGuid).Select( GenerateList).ToList();
                case m.Enums.TagEnum.Program:
                    var tagRepo2 = new TagRepo<m.Program.ProgramTag>(conString);
                    return new TagManager<m.Program.ProgramTag>(userRepo, tagRepo2, weightRoom).GetAllTags(TagEnum.Program, userGuid).Select(GenerateList).ToList();
                case m.Enums.TagEnum.Workout:
                    var tagRepo3 = new TagRepo<WorkoutTag>(conString);
                    return new TagManager<WorkoutTag>(userRepo, tagRepo3, weightRoom).GetAllTags(TagEnum.Workout, userGuid).Select(GenerateList).ToList();
                case m.Enums.TagEnum.Movie:
                    var tagRepo4 = new TagRepo<m.MultiMedia.MovieTag>(conString);
                    return new TagManager<m.MultiMedia.MovieTag>(userRepo, tagRepo4, weightRoom).GetAllTags(TagEnum.Movie, userGuid).Select(GenerateList).ToList();
                case m.Enums.TagEnum.Athlete:
                    var tagRepo5 = new TagRepo<m.Athlete.AthleteTag>(conString);
                    return new TagManager<m.Athlete.AthleteTag>(userRepo, tagRepo5, weightRoom).GetAllTags(TagEnum.Athlete,userGuid).Select(GenerateList).ToList();
                case m.Enums.TagEnum.Survey:
                    var tagRepo6 = new TagRepo<m.Survey.SurveyTag>(conString);
                    return new TagManager<m.Survey.SurveyTag>(userRepo, tagRepo6, weightRoom).GetAllTags(TagEnum.Survey,  userGuid).Select(GenerateList).ToList();
                case m.Enums.TagEnum.Metric:
                    var tagRepo7 = new TagRepo<m.Metric.MetricTag>(conString);
                    return new TagManager<m.Metric.MetricTag>(userRepo, tagRepo7, weightRoom).GetAllTags(TagEnum.Metric, userGuid).Select(GenerateList).ToList();
                default:
                    return new List<vm.Tag.Tag>();
            }
        }
        private vm.Tag.Tag GenerateList(m.Tag.Tag dbTag)
        {
            return new vm.Tag.Tag()
            {
                Name = dbTag.Name,
                IsDeleted = dbTag.IsDeleted,
                Id = dbTag.Id
            };
        }
    }
}
