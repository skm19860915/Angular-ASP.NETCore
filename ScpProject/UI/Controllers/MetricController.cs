using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL;
using BL.CustomExceptions;
using DAL.DTOs.AthleteAssignedPrograms;
using DAL.DTOs.Metrics;
using Models.Metric;
using blo = BL.BusinessObjects.Metric;

namespace Controllers.Controllers
{
    [RoutePrefix("api/Metric")]
    public class MetricController : ApiController
    {
        public IMetricManager _metricManager;
        public IProgramManager _programManager;
        public MetricController(IMetricManager metMan, IProgramManager programManager)
        {
            _metricManager = metMan;
            _programManager = programManager;
        }

        [HttpGet, Route("HardDelete/{id:int}")]
        public void HardDelete(int id)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _metricManager.HardDelete(id, userGuid);
        }

        [Route("AddMetric"), HttpPost]
        public HttpResponseMessage AddMetric(blo.MetricDetails newMet)
        {

            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            var tagIds = !newMet.Tags.Any() ? new List<MetricTag>() : newMet.Tags.Select(x => new MetricTag() { Name = x.Name, Id = x.Id}).ToList();
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _metricManager.AddMetric(newMet.Name, newMet.UnitOfMeasurementId, tagIds, userGuid));
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
        [Route("UpdateMetric"), HttpPost]
        public HttpResponseMessage UpdateMetric(blo.MetricDetails newMet)
        {

            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            var tagIds = !newMet.Tags.Any() ? new List<MetricTag>() : newMet.Tags.Select(x => new MetricTag() { Name = x.Name, Id = x.Id }).ToList();
            try
            {
                _metricManager.UpdateMetric(newMet.Name, newMet.UnitOfMeasurementId, tagIds, userGuid, newMet.Id);
                return Request.CreateResponse(HttpStatusCode.OK);
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
        [Route("AddMeasurement"), HttpPost]
        public HttpResponseMessage AddMeasurement([FromBody]UnitOfMeasurement unit)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _metricManager.AddMeasurement(unit.UnitType, userGuid));
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
        [Route("GetMetrics"), HttpGet]
        public List<blo.MetricDetails> GetMetrics()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _metricManager.GetMetrics(userGuid);
        }
        [Route("GetAllMeasurements"), HttpGet]
        public List<UnitOfMeasurement> GetAllMeasurements()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _metricManager.GetAllMeasurements(userGuid);
        }
        [HttpGet, Route("GetHistoricProgramsWithMetrics/{athleteId:int}")]
        public List<HistoricProgram> GetAllHistoricProgramsWithMetrics(int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _metricManager.GetAllHistoricProgramsWithMetrics(athleteId, userGuid);
        }
        [HttpGet, Route("GetAssignedProgramMetric/{athleteId:int}/{assignedProgramId:int}")]
        public List<DAL.DTOs.AthleteAssignedPrograms.AssignedMetric> GetAssignedMetrics(int athleteId, int assignedProgramId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _programManager.GetAssignedMetrics(assignedProgramId, athleteId, userGuid);
        }
        [HttpGet, Route("GetAllMeasuredMetrics/{athleteId:int}")]
        public AthleteCompletedMetricHomePage GetAllMeasuredMetrics(int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _metricManager.GetAllMeasuredMetrics(athleteId, userGuid);
        }
        [HttpPost, Route("DuplicateMetric/{metricId:int}")]
        public int DuplicateMetric(int metricId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _metricManager.DuplicateMetric(metricId, userGuid);
        }
        [HttpPost, Route("ArchiveMetric/{metricId:int}")]
        public void ArchiveMetric(int metricId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _metricManager.Archive(metricId, userGuid);
        }
        [HttpPost, Route("UnArchiveMetric/{metricId:int}")]
        public void UnArchiveMetric(int metricId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _metricManager.UnArchive(metricId, userGuid);
        }
    }
}

