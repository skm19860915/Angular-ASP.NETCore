using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL;
using b = BL.BusinessObjects;
using System.Web.Configuration;
using System.Threading.Tasks;
using System.Threading;
using Models.MultiMedia;
using BL.CustomExceptions;

namespace Controllers.Controllers
{
    public class ImageInformation
    {
        public string ContainerName { get; set; }
        public string FileName { get; set; }
        public int PictureId { get; set; }
    }

    [RoutePrefix("api/MultiMedia")]
    public class MultiMediaController : ApiController
    {
        private string AzureMediaBaseUrl { get; set; }
        public IMultiMediaManager _mmManager { get; set; }
        public IMovieManager _movieMan { get; set; }
        public IOrganizationManager _organizationManager;
        public IRosterManager _rosterManager; 
        public MultiMediaController(IMultiMediaManager mmMan, IMovieManager movieMan, IOrganizationManager orgMan, IRosterManager rosterMan)
        {
            var tempUrl = WebConfigurationManager.AppSettings.Get("azureMediaBaseUrl");
            AzureMediaBaseUrl = tempUrl.EndsWith("/") ? tempUrl : tempUrl + "/";
            _mmManager = mmMan;
            _movieMan = movieMan;
            _organizationManager = orgMan;
            _rosterManager = rosterMan;
        }

        [Route("CreateOrganizationImage"),HttpPost]
        public async void CreateOrganizationImage(HttpRequestMessage request)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);

            IEnumerable<HttpContent> parts = null;
            try
            {
                var re = request.Content.ReadAsByteArrayAsync().Result;

                Task.Factory
                    .StartNew(() => parts = Request.Content.ReadAsMultipartAsync().Result.Contents,
                        CancellationToken.None,
                        TaskCreationOptions.LongRunning, // guarantees separate thread
                        TaskScheduler.Default)
                    .Wait();
                var item = parts.FirstOrDefault();
                var fileName = item.Headers.ContentDisposition.FileName.ToString().Replace("\"", string.Empty);
                var x = await item.ReadAsByteArrayAsync();
                var picId = _mmManager.CreatePicture(x, fileName.ToLower(), userGuid, AzureMediaBaseUrl);
                _organizationManager.AddImage(picId, userGuid);
            }
            catch (Exception ex)
            {
                var ox = ex;
                throw;
            }
        }

        [Route("CreateProfilePicture/{id:int}"), HttpPost]
        public async void CreatePictureAsync([FromUri]int id, HttpRequestMessage request)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);

            IEnumerable<HttpContent> parts = null;
            try
            {
                var re = request.Content.ReadAsByteArrayAsync().Result;

                Task.Factory
                    .StartNew(() => parts = Request.Content.ReadAsMultipartAsync().Result.Contents,
                        CancellationToken.None,
                        TaskCreationOptions.LongRunning, // guarantees separate thread
                        TaskScheduler.Default)
                    .Wait();
                var item = parts.FirstOrDefault();
                var fileName = item.Headers.ContentDisposition.FileName.ToString().Replace("\"", string.Empty);
                var x = await item.ReadAsByteArrayAsync();
                var picId = _mmManager.CreatePicture(x, fileName.ToLower(), userGuid, AzureMediaBaseUrl);
                _rosterManager.AddProfilePictureToAthlete(id, picId, userGuid);

            }
            catch (Exception ex)
            {
                var ox = ex;
                throw;
            }
        }
        [Route("CreateMovie")]
        public HttpResponseMessage CreateMovie([FromBody] ViewModels.MultiMedia.MovieVM newMovie)
        {
            var tags = !newMovie.Tags.Any() ? new List<MovieTag>() : newMovie.Tags.Select(x => new MovieTag { Name = x.Name, Id = x.Id }).ToList();
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _movieMan.CreateNewMovie(newMovie.URL, newMovie.Name, userGuid, tags));
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
        [HttpGet, Route("HardDelete/{id:int}")]
        public void HardDelete(int id)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _movieMan.HardDelete(id, userGuid);
        }
        [Route("UpdateMovie")]
        public HttpStatusCode UpdateMovie([FromBody] ViewModels.MultiMedia.MovieVM newMovie)
        {
            var tags = !newMovie.Tags.Any() ? new List<MovieTag>() : newMovie.Tags.Select(x => new MovieTag { Name = x.Name, Id = x.Id }).ToList();
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _movieMan.UpdateMovie(newMovie.Id, newMovie.URL, newMovie.Name, userGuid, tags);
            return HttpStatusCode.OK;
        }
        [HttpGet, Route("GetAllMovies")]
        public List<b.MultiMedia.Movie> GetAllMovies()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _movieMan.GetAllMovies(userGuid);
        }
        [HttpPost, Route("ArchiveMovie/{movieId:int}")]
        public void ArchiveExercise(int movieId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _movieMan.Archive(movieId, userGuid);
        }
        [HttpPost, Route("UnArchiveMovie/{movieId:int}")]
        public void UnArchiveExercise(int movieId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _movieMan.UnArchive(movieId, userGuid);
        }
    }
}
