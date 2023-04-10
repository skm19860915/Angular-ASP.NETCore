using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using b = BL.BusinessObjects;
using Models.SetsAndReps;
using blDTO = BL.BusinessObjects.SetsAndReps;
using DAL.CustomerExceptions;
using BL.CustomExceptions;
using Models.Enums;

namespace BL
{
    public interface IWorkoutManager
    {
        void AddTagsToWorkout(List<WorkoutTag> tagIds, int workoutId, Guid createdUserGuid);
        void ArchiveWorkout(int workoutId, Guid createdUserToken);
        int CreateNewWorkout(Workout newWorkout, Guid userToken, List<WorkoutTag> tagIds);
        int DuplicateWorkout(int workoutId, Guid createdUserToken);
        List<blDTO.Workout> GetWorkout(Guid userToken);
        blDTO.WorkoutDetails GetWorkoutDetails(int workoutId, Guid userToken);
        void UnArchiveWorkout(int workoutId, Guid createdUserToken);
        void UpdateWorkout(Workout newWorkout, Guid userToken, List<WorkoutTag> tagIds);
    }

    public class WorkoutManager : IWorkoutManager
    {
        private IWorkoutRepo _workoutRepo { get; set; }
        private ITagRepo<WorkoutTag> _workoutTagRepo { get; set; }
        private List<OrganizationRoleEnum> _userRoles { get; set; }
        private IUserRepo _userRepo { get; set; }
        private IOrganizationRepo _orgRepo { get; set; }
        public WorkoutManager(IWorkoutRepo workoutRepo, ITagRepo<WorkoutTag> workoutTagRepo, IUserRepo userRepo, IOrganizationRepo orgRepo)
        {
            _workoutRepo = workoutRepo;
            _workoutTagRepo = workoutTagRepo;
            _workoutTagRepo.InitializeTagRepo(TagEnum.Workout);
            _userRepo = userRepo;
            _orgRepo = orgRepo;
        }
        private void GenerateUserRoles(Guid userToken)
        {
            _userRoles = _orgRepo.GetUserRoles(userToken);
        }

        public void UpdateWorkout(Models.SetsAndReps.Workout newWorkout, Guid userToken, List<WorkoutTag> tagIds)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ModifyWorkouts) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Workouts");
            }
            try
            {
                if (!_userRepo.Get(userToken).IsCoach) throw new ApplicationException("Only Coaches Can Modify Workouts");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            _workoutRepo.UpdateWorkout(newWorkout, userToken);
            AddTagsToWorkout(tagIds, newWorkout.Id, userToken);
        }


        public int DuplicateWorkout(int workoutId, Guid createdUserToken)
        {
            GenerateUserRoles(createdUserToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateWorkouts) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Workouts");
            }
            return _workoutRepo.DuplicateWorkout(workoutId, createdUserToken);
        }
        public void UnArchiveWorkout(int workoutId, Guid createdUserToken)
        {
            GenerateUserRoles(createdUserToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveWorkouts) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To UnArchive Workout");
            }
            _workoutRepo.UnArchiveWorkout(workoutId, createdUserToken);
        }
        public void ArchiveWorkout(int workoutId, Guid createdUserToken)
        {
            GenerateUserRoles(createdUserToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveWorkouts) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Archive Workout");
            }
            _workoutRepo.ArchiveWorkout(workoutId, createdUserToken);
        }
        public blDTO.WorkoutDetails GetWorkoutDetails(int workoutId, Guid userToken)
        {
            var workout = _workoutRepo.GetWorkoutDetails(workoutId, userToken);

            if (workout == null) return new blDTO.WorkoutDetails();

            var allTags = _workoutRepo.GetAllTagsForAWorkout(workoutId, userToken);
            var tagsToAdd = new List<b.Tag>();

            if (allTags != null && allTags.Tags != null)
            {
                tagsToAdd = allTags.Tags.Select(x => new b.Tag() { Id = x.Id, Name = x.Name }).ToList();
            }

            var weeks = new List<blDTO.Week>();

            if (workout.TotalWorkout != null && workout.TotalWorkout != null)
            {
                workout.TotalWorkout.ForEach(x =>
                {
                    var newWeek = new blDTO.Week()
                    {
                        Id = x.Id,
                        ParentWorkoutId = x.ParentWorkoutId,
                        Position = x.Position,
                        SetsAndReps = new List<blDTO.Set>()
                    };


                    if (x.SetsAndReps != null)
                    {
                        x.SetsAndReps.ForEach(y =>
                        {
                            newWeek.SetsAndReps.Add(new blDTO.Set()
                            {
                                Id = y.Id,
                                ParentWeekId = y.ParentWeekId,
                                Percent = y.Percent,
                                Position = y.Position,
                                Reps = y.Reps,
                                Sets = y.Sets,
                                Weight = y.Weight,
                                Distance = y.Distance,
                                Minutes = y.Minutes,
                                Other = y.Other,
                                RepsAchieved = y.RepsAchieved,
                                Seconds = y.Seconds
                            });
                        });
                    }
                    weeks.Add(newWeek);
                });
            }

            var ret = new blDTO.WorkoutDetails()
            {
                CreatedUserId = workout.CreatedUserId,
                Id = workout.Id,
                Name = workout.Name,
                Notes = workout.Notes,
                Tags = tagsToAdd,
                CanModify = workout.CanModify,
                IsDeleted = workout.IsDeleted,
                TotalWorkout = weeks,
                Rest = workout.Rest,
                ShowWeight = workout.ShowWeight
            };

            return ret;
        }

        public int CreateNewWorkout(Models.SetsAndReps.Workout newWorkout, Guid userToken, List<WorkoutTag> tagIds)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateWorkouts) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Workouts");
            }
            try
            {
                if (!_userRepo.Get(userToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Workouts");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            var newId = 0;
            try
            {
                newId = _workoutRepo.SaveNewWorkout(newWorkout, userToken);
            }
            catch (DuplicateKeyException dup)
            {
                throw new ItemAlreadyExistsException("An Workout With This Name Already Exists", dup);

            }
            AddTagsToWorkout(tagIds, newId, userToken);
            return newId;
        }
        public List<blDTO.Workout> GetWorkout(Guid userToken)
        {
            var allMappings = _workoutRepo.GetAllWorkoutTagMappings(userToken);
            var allWorkouts = _workoutRepo.GetWorkouts(userToken);

            var ret = new List<blDTO.Workout>();

            foreach (var e in allWorkouts)
            {
                var mapping = allMappings.FirstOrDefault(x => x.WorkoutId == e.Id);
                ret.Add(new blDTO.Workout()
                {
                    Id = e.Id,
                    Name = e.Name,
                    CanModify = e.CanModify,
                    IsDeleted = e.IsDeleted,
                    Tags = mapping == null ? new List<b.Tag>() : mapping.Tags.Select(x => new b.Tag() { Id = x.Id, Name = x.Name }).ToList()
                });
            }
            return ret;
        }
        public void AddTagsToWorkout(List<WorkoutTag> tagIds, int workoutId, Guid createdUserGuid)
        {
            if (!(_userRoles.Contains(OrganizationRoleEnum.ModifyWorkouts) || _userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ModifyWorkouts)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Workouts");
            }
            var targetExercise = _workoutRepo.GetWorkout(workoutId, createdUserGuid);
            if (targetExercise == null) return;

            _workoutTagRepo.DeleteAssociatedTags(workoutId);
            _workoutTagRepo.AddAssociatedTags(tagIds, workoutId);
        }
    }
}
