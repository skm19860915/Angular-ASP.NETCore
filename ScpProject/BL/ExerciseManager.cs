using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Models.Exercise;
using b = BL.BusinessObjects;
using DAL.CustomerExceptions;
using BL.CustomExceptions;
using Models.Enums;

namespace BL
{
    public interface IExerciseManager
    {
        void AddTagsToExercise(List<ExerciseTag> tagIds, int exerciseId, Guid createdUserGuid);
        void Archive(int exerciseId, Guid createdUserGuid);
        int CreateNewExercise(string notes, string name, List<ExerciseTag> tagIds, Guid createdUserGuid, double? percent, int? PercentMetricCalculationId, string videoURL, Guid userToken);
        int Duplicate(int exerciseID, Guid createdUserToken);
        List<b.Exercise> GetAllExercises(Guid userToken);
        void HardDelete(int exerciseId, Guid userToken);
        void UnArchive(int exerciseId, Guid createdUserGuid);
        void UpdateExercise(int Id, string notes, string name, List<ExerciseTag> tagIds, Guid createdUserGuid, double? percent, int? percentMetricCalculationId, string videoURL, Guid userToken);
    }

    public class ExerciseManager : IExerciseManager
    {
        private IExerciseRepo _exerciseRepo { get; set; }
        private ITagRepo<ExerciseTag> _exerciseTagRepo { get; set; }
        private IUserRepo _userRepo { get; set; }
        private IOrganizationRepo _orgRepo { get; set; }
        private List<OrganizationRoleEnum> _userRoles { get; set; }
        public ExerciseManager(IExerciseRepo exerciseRepo, IUserRepo userRepo, IOrganizationRepo orgRepo, ITagRepo<ExerciseTag> exerciseTagRepo)
        {
            _exerciseRepo = exerciseRepo;
            _exerciseTagRepo = exerciseTagRepo;
            _userRepo = userRepo;
            _orgRepo = orgRepo;
            _exerciseTagRepo.InitializeTagRepo(TagEnum.Exercise);
        }
        private void GenerateUserRoles(Guid userToken)
        {
            _userRoles = _orgRepo.GetUserRoles(userToken);
        }

        public void HardDelete(int exerciseId, Guid userToken)
        {
            var tempUser = _userRepo.Get(userToken);
            var createdUser = _exerciseRepo.GetCreatedUser(exerciseId);
            GenerateUserRoles(userToken);
            if (tempUser.OrganizationId != createdUser.OrganizationId) throw new ApplicationException("User not in Organization");

            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveExercises) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To DELETE Exercises");
            }
            var targetExercise = _exerciseRepo.GetExercise(exerciseId, userToken);

            if (!targetExercise.CanModify) throw new ApplicationException("This Exercise Is In Use, And Cannot Be Deleted");

            _exerciseTagRepo.DeleteAssociatedTags(exerciseId);
            _exerciseRepo.HardDelete(exerciseId);
        }
        /// <summary>
        /// Creates new exercise
        /// </summary>
        /// <param name="notes">The notes attached to the exercise</param>
        /// <param name="name">Name of new Exercise</param>
        /// <param name="tagIds">Tags associated with the exercise</param>
        /// <param name="createdUserGuid">Created UserGuid</param>
        /// <exception cref="ItemAlreadyExistsException">Thrown when item already exists in db</exception>
        /// <returns></returns>
        public int CreateNewExercise(string notes, string name, List<ExerciseTag> tagIds, Guid createdUserGuid, double? percent, int? PercentMetricCalculationId, string videoURL, Guid userToken)
        {
            var tempVideo = string.Empty;
            GenerateUserRoles(userToken);
            if (videoURL.Contains("https://youtu.be/"))
            {
                tempVideo = videoURL.Replace("https://youtu.be/", "https://www.youtube.com/embed/");
            }
            else if (videoURL.Contains("https://vimeo.com/"))
            {
                tempVideo = videoURL.Replace("https://vimeo.com/", "https://player.vimeo.com/video/");
            }

            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateExercises) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Exercises");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ItemValidationError("Exercise Name Is Invalid. Please Change the name");
            }
            var newId = 0;
            try
            {
                if (!_userRepo.Get(createdUserGuid).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }

            try
            {
                newId = _exerciseRepo.CreateExericse(new Exercise() { VideoURL = tempVideo, Notes = notes, Name = name, Percent = percent, PercentMetricCalculationId = PercentMetricCalculationId }, createdUserGuid);
            }
            catch (DuplicateKeyException dup)
            {
                throw new ItemAlreadyExistsException("An Exercise With This Name Already Exists", dup);

            }
            AddTagsToExercise(tagIds, newId, createdUserGuid);
            return newId;
        }
        public void UpdateExercise(int Id, string notes, string name, List<ExerciseTag> tagIds, Guid createdUserGuid, double? percent, int? percentMetricCalculationId, string videoURL, Guid userToken)
        {
            var tempVideo = string.Empty;
            GenerateUserRoles(userToken);
            if (!string.IsNullOrEmpty(videoURL) && videoURL.Contains("https://youtu.be/"))
            {
                tempVideo = videoURL.Replace("https://youtu.be/", "https://www.youtube.com/embed/");
            }
            else if (!string.IsNullOrEmpty(videoURL) && videoURL.Contains("https://vimeo.com/"))
            {
                tempVideo = videoURL.Replace("https://vimeo.com/", "https://player.vimeo.com/video/");
            }
            if (!(_userRoles.Contains(OrganizationRoleEnum.ModifyExercises) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Update Exercises");
            }
            var targetExercise = _exerciseRepo.GetExercise(Id, createdUserGuid);
            if (targetExercise.CanModify)
            {
                _exerciseRepo.UpdateExercise(new Exercise() { VideoURL = tempVideo, Id = Id, IsDeleted = false, Notes = notes, Name = name, Percent = percent, PercentMetricCalculationId = percentMetricCalculationId }, createdUserGuid);
            }
            else
            {
                _exerciseRepo.UpdateExercise(new Exercise() { VideoURL = tempVideo, Id = Id, IsDeleted = false, Notes = notes, Name = targetExercise.Name, Percent = targetExercise.Percent, PercentMetricCalculationId = targetExercise.PercentMetricCalculationId }, createdUserGuid);
            }
            AddTagsToExercise(tagIds, Id, createdUserGuid);
        }
        public void UnArchive(int exerciseId, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveExercises) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To UnArchive Exercises");
            }
            _exerciseRepo.UnArchive(exerciseId, createdUserGuid);
        }
        public void Archive(int exerciseId, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveExercises) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Archive Exercises");
            }
            _exerciseRepo.Archive(exerciseId, createdUserGuid);
        }
        public int Duplicate(int exerciseID, Guid createdUserToken)
        {
            GenerateUserRoles(createdUserToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateExercises) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Duplicate Exercises");
            }
            return _exerciseRepo.DuplicateExercise(exerciseID, createdUserToken);
        }
        public void AddTagsToExercise(List<ExerciseTag> tagIds, int exerciseId, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateExercises) || _userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ModifyExercises)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Exercises");
            }

            var targetExercise = _exerciseRepo.GetExercise(exerciseId, createdUserGuid);
            if (targetExercise == null) return;

            _exerciseTagRepo.DeleteAssociatedTags(exerciseId);
            _exerciseTagRepo.AddAssociatedTags(tagIds, exerciseId);
        }
        public List<b.Exercise> GetAllExercises(Guid userToken)
        {

            var allMappings = _exerciseRepo.GetAllExerciseTagMappings(userToken);

            var allExercises = _exerciseRepo.GetAllExercises(userToken);

            var ret = new List<b.Exercise>();

            foreach (var e in allExercises)
            {
                var mapping = allMappings.FirstOrDefault(x => x.ExerciseId == e.Id);
                ret.Add(new b.Exercise()
                {
                    VideoURL = e.VideoURL,
                    Id = e.Id,
                    Percent = e.Percent,
                    PercentMetricCalculationId = e.PercentMetricCalculationId,
                    Name = e.Name,
                    CreatedUserId = e.CreatedUserId,
                    IsDeleted = e.IsDeleted,
                    Tags = mapping == null ? new List<b.Tag>() : mapping.Tags.Select(x => new b.Tag() { Id = x.Id, Name = x.Name }).ToList(),
                    CanModify = e.CanModify,
                    CalcMetricName = e.CalcMetricName
                });
            }
            return ret;
        }
    }
}
