using System;
using System.Collections.Generic;
using System.Linq;
using BL.BusinessObjects.Metric;
using BL.CustomExceptions;
using DAL.CustomerExceptions;
using DAL.DTOs.AthleteAssignedPrograms;
using DAL.DTOs.Metrics;
using DAL.Repositories;
using Models.Enums;
using Models.Metric;
using blo = BL.BusinessObjects.Metric;

namespace BL
{
    public interface IMetricManager
    {
        int AddMeasurement(string unitType, Guid userToken);
        int AddMetric(string name, int? unitOfMeasurementId, List<MetricTag> tagIds, Guid userToken);
        void AddTagsToMetric(List<MetricTag> tagIds, int metricId, Guid createdUserGuid);
        void Archive(int metricId, Guid createdUserGuid);
        int DuplicateMetric(int metricId, Guid createdUserGuid);
        List<HistoricProgram> GetAllHistoricProgramsWithMetrics(int athleteId, Guid createdUserToken);
        AthleteCompletedMetricHomePage GetAllMeasuredMetrics(int athleteId, Guid userGuid);
        List<UnitOfMeasurement> GetAllMeasurements(Guid userToken);
        List<MetricDetails> GetMetrics(Guid userToken);
        void HardDelete(int metricId, Guid userToken);
        void UnArchive(int metricId, Guid createdUserGuid);
        void UpdateMetric(string name, int? unitOfMeasurementId, List<MetricTag> tagIds, Guid userToken, int id);
    }

    public class MetricManager : IMetricManager
    {
        private IMetricRepo _metricRepo { get; set; }
        private IUserRepo _userRepo { get; set; }
        private ITagRepo<MetricTag> _tagRepo { get; set; }
        private IOrganizationRepo _orgRepo { get; set; }
        private List<OrganizationRoleEnum> _userRoles { get; set; }

        public MetricManager(IMetricRepo metRep, IUserRepo userRepo, ITagRepo<MetricTag> tagRepo, IOrganizationRepo orgRepo)
        {
            _metricRepo = metRep;
            _userRepo = userRepo;
            _tagRepo = tagRepo;
            _orgRepo = orgRepo;
            _tagRepo.InitializeTagRepo(TagEnum.Metric);
        }

        private void GenerateUserRoles(Guid userToken)
        {
            _userRoles = _orgRepo.GetUserRoles(userToken);
        }

        public void HardDelete(int metricId, Guid userToken)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveMetrics) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To DELETE Metrics");
            }
            var targetMetric = _metricRepo.GetMetric(metricId, userToken);

            if (!targetMetric.CanModify) throw new ApplicationException("This Metric Is In Use And Cannot Be Deleted");
            _tagRepo.DeleteAssociatedTags(metricId);
            _metricRepo.HardDelete(metricId);
        }
        public List<HistoricProgram> GetAllHistoricProgramsWithMetrics(int athleteId, Guid createdUserToken)
        {
            return _metricRepo.GetAllHistoricProgramsWithMetrics(athleteId, createdUserToken);
        }
        public void UnArchive(int metricId, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveMetrics) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To UnArchive Metrics");
            }
            _metricRepo.UnArchive(metricId, createdUserGuid);
        }
        public void Archive(int metricId, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveMetrics) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Archive Metrics");
            }
            _metricRepo.Archive(metricId, createdUserGuid);
        }
        public int DuplicateMetric(int metricId, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateMetrics) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Duplicate Metricss");
            }
            return _metricRepo.DuplicateMetric(metricId, createdUserGuid);
        }
        public int AddMetric(string name, int? unitOfMeasurementId, List<MetricTag> tagIds, Guid userToken)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateMetrics) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Metrics");
            }
            try
            {
                if (!_userRepo.Get(userToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ItemValidationError("Metric Name Is Invalid. Please Change the name");
            }
            var newId = 0;
            try
            {
                newId = _metricRepo.CreateMetric(new Metric() { Name = name, UnitOfMeasurementId = unitOfMeasurementId }, userToken);
            }
            catch (DuplicateKeyException dup)
            {
                throw new ItemAlreadyExistsException("A Metric With This Name Already Exists", dup);

            }
            AddTagsToMetric(tagIds, newId, userToken);
            return newId;
        }

        public void UpdateMetric(string name, int? unitOfMeasurementId, List<MetricTag> tagIds, Guid userToken, int id)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ModifyMetrics) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Metrics");
            }
            try
            {
                if (!_userRepo.Get(userToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ItemValidationError("Metric Name Is Invalid. Please Change the name");
            }
            _metricRepo.UpdateMetric(new Metric() { Name = name, UnitOfMeasurementId = unitOfMeasurementId, Id = id }, userToken);
            AddTagsToMetric(tagIds, id, userToken);
        }
        public int AddMeasurement(string unitType, Guid userToken)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateMetrics) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Metric");
            }
            try
            {
                if (!_userRepo.Get(userToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            try
            {
                return _metricRepo.CreateUnitOfMeasurement(new UnitOfMeasurement() { UnitType = unitType }, userToken);
            }
            catch (DuplicateKeyException dup)
            {
                throw new ItemAlreadyExistsException("A Measurement With This Name Already Exists", dup);

            }
        }
        public List<blo.MetricDetails> GetMetrics(Guid userToken)
        {
            var allmetrics = _metricRepo.GetAllMetrics(userToken);
            var allUOM = _metricRepo.GetAllUnitOfMeasurements(userToken);

            var ret = new List<blo.MetricDetails>();

            ret = allmetrics.Select(x => new blo.MetricDetails()
            {
                Id = x.Id,
                Name = x.Name,
                UnitOfMeasurementId = x.UnitOfMeasurementId,
                UnitOfMeasurementType = (x.UnitOfMeasurementId != null) ? allUOM.FirstOrDefault(y => y.Id == x.UnitOfMeasurementId).UnitType : string.Empty,
                Tags = new List<BusinessObjects.Tag>(),
                CanModify = x.CanModify,
                IsDeleted = x.IsDeleted
            }).ToList();


            var allTags = _metricRepo.GetAllMetricsTagMappings(userToken);
            if (allTags == null) return ret;

            ret.ForEach(x =>
            {
                var allMetricTags = allTags.FirstOrDefault(y => y.MetricId == x.Id);
                if (allMetricTags != null && allMetricTags.Tags.Any())
                {
                    x.Tags = allMetricTags.Tags.Select(y => new BusinessObjects.Tag() { Id = y.Id, Name = y.Name }).ToList();
                }
            });
            return ret;
        }
        public List<UnitOfMeasurement> GetAllMeasurements(Guid userToken)
        {
            return _metricRepo.GetAllUnitOfMeasurements(userToken);
        }

        public void AddTagsToMetric(List<MetricTag> tagIds, int metricId, Guid createdUserGuid)
        {
            if (!(_userRoles.Contains(OrganizationRoleEnum.ModifyMetrics) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Metrics");
            }
            var targetMetric = _metricRepo.GetMetric(metricId, createdUserGuid);
            if (targetMetric == null) return;


            _tagRepo.DeleteAssociatedTags(metricId);
            _tagRepo.AddAssociatedTags(tagIds, metricId);
        }
        public AthleteCompletedMetricHomePage GetAllMeasuredMetrics(int athleteId, Guid userGuid)
        {
            var ret = _metricRepo.GetAllMeasuredMetrics(athleteId, userGuid);
            //  ret.ForEach(x => x.CompletedDateDisplay = x.CompletedDate.ToShortDateString());
            return ret;
        }
    }
}
