using DAL.Repositories;
using System;
using System.Collections.Generic;
using Models.Program;
using b = BL.BusinessObjects;
using System.Linq;
using DAL.DTOs.Program;
using System.Net.Http;
using Newtonsoft.Json;
using AzureFunctions;
using DAL.CustomerExceptions;
using BL.CustomExceptions;
using Models.Enums;
using DAL.DTOs.AthleteAssignedPrograms;
using System.Threading.Tasks;

namespace BL
{
    public interface IProgramManager
    {
        void AddTagsToProgram(List<ProgramTag> tagIds, int programId, Guid createdUserId);
        void Archive(int programId, Guid createdUserGuid);
        int CreateProgram(b.Program.Program targetProgram, Guid createdUserGuid, List<ProgramTag> tagIds);
        void Duplicate(int programId, Guid createdUserToken);
        List<AthleteHomePagePastProgram> GetAllPastPrograms(int athleteId);
        List<b.Program.Program> GetAllPrograms(Guid createdUserGuid);
        List<AssignedMetric> GetAssignedMetrics(int assignedProgramId, int athleteId, Guid createdAthleteGuid, int programDayId = 0);
        DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAssignedProgramDetails(Guid createduserGuid, int assignedProgramId = 0);
        List<AthleteAssignedQuestions> GetAssignedQuestions(int assignedProgramId, int athleteId, Guid createdAthleteGuid, int programDayId = 0);
        DAL.DTOs.Program.Program GetProgramDetails(int programId, Guid createduserGuid);
        DAL.DTOs.Program.Program GetSnapShotProgramDetails(int athleteId, Guid requestingUserGuid);
        void HardDelete(int programId, Guid userToken);
        Task MarkDayAsComplete(Guid userToken, int programDayId, int weekNumber, int athleteId);
        void PrintAllAssignedAthletePrograms(int programId, Guid createdUserToken, List<int> athleteIds, bool printMasterPdf, bool printSelectedAthletes, bool printUsingAdvancedOptions);
        void UnArchive(int programId, Guid createdUserGuid);
        Task UpdateAssignedSnapShot(b.Program.Program newProgram, int athleteId, Guid userToken);
        void UpdateProgram(b.Program.Program targetProgram, Guid createdUserGuid, List<ProgramTag> tagIds);
    }

    public class ProgramManager : IProgramManager
    {
        private ITagRepo<ProgramTag> _programTagRepo { get; set; }
        private IProgramRepo _programRepo { get; set; }
        private IAthleteRepo _athleteRepo { get; set; }
        private IExerciseRepo _exRepo { get; set; }
        private IOrganizationRepo _orgRepo { get; set; }
        private List<OrganizationRoleEnum> _userRoles { get; set; }
        private IUserRepo _userRepo { get; set; }
        private ILogRepo _logRepo { get; set; }
        private IPlayerSnapShotRepository _playerSnapShotRepository { get; set; }
        public ProgramManager(ITagRepo<ProgramTag> _programTag, IProgramRepo programRepo, IAthleteRepo athleteRepo, IExerciseRepo exerciseRepo, IOrganizationRepo orgRepo, IUserRepo userRepo, ILogRepo logRepo, IPlayerSnapShotRepository playerSnapShotRepo)
        {
            _programRepo = programRepo;
            _programTagRepo = _programTag;
            _programTagRepo.InitializeTagRepo(TagEnum.Program);
            _athleteRepo = athleteRepo;
            _exRepo = exerciseRepo;
            _orgRepo = orgRepo;
            _userRepo = userRepo;
            _logRepo = logRepo;
            _playerSnapShotRepository = playerSnapShotRepo;
        }

        private void GenerateUserRoles(Guid userToken)
        {
            _userRoles = _orgRepo.GetUserRoles(userToken);
        }

        public void HardDelete(int programId, Guid userToken)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ArchivePrograms)))
            {
                throw new ApplicationException("User Does Not Have Rights To Archive Programs");
            }
            var targetProgram = _programRepo.GetProgram(programId, userToken);
            if (!targetProgram.CanModify) throw new ApplicationException("This Program Is In Use And Cannot Be Deleted");

            _programTagRepo.DeleteAssociatedTags(programId);
            _programRepo.DontFuckingDoThis_Delete_All_Information_About_A_Program(programId);
        }
        public void UnArchive(int programId, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ArchivePrograms)))
            {
                throw new ApplicationException("User Does Not Have Rights To UnArchive Programs");
            }
            _programRepo.UnArchive(programId, createdUserGuid);
        }
        public void Archive(int programId, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ArchivePrograms)))
            {
                throw new ApplicationException("User Does Not Have Rights To Archive Programs");
            }
            _programRepo.Archive(programId, createdUserGuid);
        }
        public List<AthleteHomePagePastProgram> GetAllPastPrograms(int athleteId)
        {
            return _programRepo.GetAllPastPrograms(athleteId);
        }
        public List<DAL.DTOs.AthleteAssignedPrograms.AssignedMetric> GetAssignedMetrics(int assignedProgramId, int athleteId, Guid createdAthleteGuid, int programDayId = 0)
        {
            var targetAthlete = _athleteRepo.GetAthlete(athleteId, createdAthleteGuid);
            var loggedInAthlete = _athleteRepo.GetAthlete(createdAthleteGuid);
            //if it is coach targetAthlete would not be null, if it is an athlete then loggedInAthlete.Id would be the same as athleteId
            if (targetAthlete == null && (loggedInAthlete == null || loggedInAthlete.Id != athleteId)) throw new ApplicationException("Cannot get athlete");
            return _programRepo.GetAssignedMetrics(assignedProgramId, athleteId, programDayId);
        }
        public List<DAL.DTOs.AthleteAssignedPrograms.AthleteAssignedQuestions> GetAssignedQuestions(int assignedProgramId, int athleteId, Guid createdAthleteGuid, int programDayId = 0)
        {
            var targetAthlete = _athleteRepo.GetAthlete(athleteId, createdAthleteGuid);
            var loggedInAthlete = _athleteRepo.GetAthlete(createdAthleteGuid);
            //if it is coach targetAthlete would not be null, if it is an athlete then loggedInAthlete.Id would be the same as athleteId
            if (targetAthlete == null && (loggedInAthlete == null || loggedInAthlete.Id != athleteId)) throw new ApplicationException("Cannot get athlete");

            return _programRepo.GetAssignedQuestions(assignedProgramId, athleteId, programDayId);
        }
        public DAL.DTOs.Program.Program GetProgramDetails(int programId, Guid createduserGuid)
        {
            return _programRepo.GetProgram(programId, createduserGuid);
        }
        public DAL.DTOs.Program.Program GetSnapShotProgramDetails(int athleteId, Guid requestingUserGuid)
        {
            var targetAthlete = _athleteRepo.GetAthlete(athleteId);
            var requestingUser = _userRepo.Get(requestingUserGuid);
            if (targetAthlete.OrganizationId != requestingUser.OrganizationId) throw new ApplicationException("You are not in same organization As Athlete");


            return _programRepo.GetSnapShotProgram(targetAthlete.AssignedProgram_AssignedProgramId.Value, requestingUserGuid);
        }
        public DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAssignedProgramDetails(Guid createduserGuid, int assignedProgramId = 0)
        {
            var targetAthlete = _athleteRepo.GetAthlete(createduserGuid);
            return _programRepo.GetAssignedProgram(targetAthlete, assignedProgramId);
        }
        public async Task MarkDayAsComplete(Guid userToken, int programDayId, int weekNumber, int athleteId)
        {
            var targetAthlete = _athleteRepo.GetAthlete(athleteId);
            var user = _userRepo.Get(userToken);

            if ((targetAthlete.AthleteUserId != user.Id && !user.IsCoach) && (targetAthlete.OrganizationId != user.OrganizationId))
            {
                throw new ApplicationException("User Does Not Have Rights To Mark Day As Complete");
            }
            if (!targetAthlete.AssignedProgram_AssignedProgramId.HasValue)
            {
                return;
            }
            //fuck em, we are only doing snapshots, we are not retrofitting v1 of programs to handle this shit. as time goes on there will no longer be v1 of programs anyway
                await _playerSnapShotRepository.MarkDayAsComplete(programDayId, weekNumber);
        }
        public void UpdateProgram(b.Program.Program targetProgram, Guid createdUserGuid, List<ProgramTag> tagIds)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ModifyPrograms)))
            {
                throw new ApplicationException("User Does Not Have Rights To Update Programs");
            }
            AddTagsToProgram(tagIds, targetProgram.Id, createdUserGuid);//they can always update tags on a program regardless of it being in use/used
            if (!_programRepo.GetProgram(targetProgram.Id, createdUserGuid).CanModify) return;
            _programRepo.DontFuckingDoThis_Delete_All_Information_About_A_Program(targetProgram.Id);
            CreateProgram(targetProgram, createdUserGuid, tagIds);
        }
        public async Task UpdateAssignedSnapShot(b.Program.Program newProgram, int athleteId, Guid userToken)
        {
            var targetAthlete = _athleteRepo.GetAthlete(athleteId);
            var oldSnapShot = _programRepo.GetAssignedProgramSnapShot(targetAthlete.AssignedProgram_AssignedProgramId.Value, userToken, targetAthlete.Id);
            await _playerSnapShotRepository.UpdateProgramWeekDayCount(newProgram.WeekCount, newProgram.Days.Count, targetAthlete.AssignedProgram_AssignedProgramId.Value);
            await _playerSnapShotRepository.UpdateProgramInfo(newProgram.Name, targetAthlete.AssignedProgram_AssignedProgramId.Value);

            //delete days that dont exist on the new program
            var oldDaysThatNeedTobeDeleted = oldSnapShot.Days.Select(x => x.Position).Except(newProgram.Days.Select(x => x.Position)).ToArray();

            for (int i = 0; i < oldDaysThatNeedTobeDeleted.Count(); i++)
            {
                var targetDay = oldSnapShot.Days.FirstOrDefault(x => x.Position == oldDaysThatNeedTobeDeleted[i]);

                for (int y = 0; y < targetDay.AssignedSurveys.Count(); y++)
                {
                    await _playerSnapShotRepository.DeleteSurvey(targetDay.AssignedSurveys[y].ProgramDayItemSurveyId);
                }
                for (int y = 0; y < targetDay.AssignedNotes.Count(); y++)
                {
                    await _playerSnapShotRepository.DeleteNote(targetDay.AssignedNotes[y].ProgramDayItemNoteId);
                }
                for (int y = 0; y < targetDay.AssignedMetrics.Count(); y++)
                {
                    await _playerSnapShotRepository.DeleteMetric(targetDay.AssignedMetrics[y].ProgramDayItemMetricId);
                }
                for (int y = 0; y < targetDay.AssignedSuperSets.Count(); y++)
                {
                    await _playerSnapShotRepository.DeleteProgramDayItemSuperSet(targetDay.AssignedSuperSets[y].SuperSetId);
                }
                for (int y = 0; y < targetDay.AssignedVideos.Count(); y++)
                {
                    await _playerSnapShotRepository.DeleteMovie(targetDay.AssignedVideos[y].ProgramDayItemMovieId);
                }
                await _playerSnapShotRepository.JustDeleteAssignedProgramDay(targetDay.Id);
            }


            //I fucked up at the beganing. I always assumed the position didnt matter as it would increment up and it would just owrk out
            //thats not the case, now i need to gaurentee it starts at 0 and increments up. If i dont the athlete snapshot workout displays blank data

            //makesure all programDay Positions start at 0 and increment up
            var orderByAsc = newProgram.Days.OrderBy(x => x.Position).ToList();
            for (int i = 0; i < newProgram.Days.Count; i++)
            {
                orderByAsc[i].Position = i;
            }
            //makesure all old program info has positions starting at 0 and increment up

            var oldDaysOrderByAsc = oldSnapShot.Days.OrderBy(x => x.Position).ToList();
            for (int i = 0; i < oldSnapShot.Days.Count; i++)
            {
                oldDaysOrderByAsc[i].Position = i;
            }
            // at this point only new program days and old program days that match up on position exist
            for (int i = 0; i < newProgram.Days.Count; i++)
            {
                var newDay = newProgram.Days.FirstOrDefault(x => x.Position == newProgram.Days[i].Position);
                var oldDay = oldSnapShot.Days.FirstOrDefault(x => x.Position == newProgram.Days[i].Position);
                var programDayId = 0;
                //then we have to insert the day
                if (oldDay == null)
                {
                    programDayId = await _playerSnapShotRepository.AddNewProgramDay(newDay.Position, targetAthlete.AssignedProgram_AssignedProgramId.Value);
                }
                else
                {
                    programDayId = oldDay.Id;
                }

                //clear out all the shit on the old day that we can. before adding/modifying
                if (oldDay != null)
                {
                    //delete all metrics that exist inthe old one that doent exist in the new one.
                    var oldMetricsWhichDontMatchUpToNewMetrics = oldDay.AssignedMetrics.Select(x => x.Position).Except(newDay.Metrics.Select(x => x.Position)).ToList();
                    for (var oldMetricPosition = 0; oldMetricPosition < oldMetricsWhichDontMatchUpToNewMetrics.Count(); oldMetricPosition++)
                    {
                        var programDayItemMetricId = oldDay.AssignedMetrics.Where(x => x.Position == oldMetricsWhichDontMatchUpToNewMetrics[oldMetricPosition]).FirstOrDefault().ProgramDayItemMetricId;
                        await _playerSnapShotRepository.DeleteMetric(programDayItemMetricId);
                    }

                    //delete all surveys that exist inthe old one that doent exist inthe new one
                    var oldSurveysWhichDontMatchUpTonewSurveys = oldDay.AssignedSurveys.Select(x => x.Position).Except(newDay.Surveys.Select(x => x.Position)).ToList();
                    for (var oldSurveyPosition = 0; oldSurveyPosition < oldSurveysWhichDontMatchUpTonewSurveys.Count(); oldSurveyPosition++)
                    {
                        var programDayItemSurveyId = oldDay.AssignedSurveys.Where(x => x.Position == oldSurveysWhichDontMatchUpTonewSurveys[oldSurveyPosition]).FirstOrDefault().ProgramDayItemSurveyId;
                        await _playerSnapShotRepository.DeleteSurvey(programDayItemSurveyId);
                    }
                    //since movies dont have any thing tied to them, its easier just to delete all the old movies and add all the new ones
                    var oldDayMovies = oldDay.AssignedVideos;
                    for (var movieIndex = 0; movieIndex < oldDayMovies.Count; movieIndex++)
                    {
                        await _playerSnapShotRepository.DeleteMovie(oldDayMovies[movieIndex].ProgramDayItemMovieId);
                    }
                    //since notes dont have any thing tied to them, its easier just to delete all the old notes and add all the new ones
                    var oldDayNotes = oldDay.AssignedNotes;
                    for (var noteIndex = 0; noteIndex < oldDayNotes.Count; noteIndex++)
                    {
                        await _playerSnapShotRepository.DeleteNote(oldDayNotes[noteIndex].ProgramDayItemNoteId);
                    }

                    //delete all supersets that dont match a position with the new supersets
                    var superSetsToDelete = oldDay.AssignedSuperSets.Select(x => x.PositionInProgramDay).Except(newDay.SuperSets.Select(x => x.Position)).ToList();

                    for (var indx = 0; indx < superSetsToDelete.Count(); indx++)
                    {
                        var superSetId = oldDay.AssignedSuperSets.FirstOrDefault(x => x.PositionInProgramDay == superSetsToDelete[indx]).SuperSetId;
                        await _playerSnapShotRepository.DeleteProgramDayItemSuperSet(superSetId);
                    }
                }
                for (int m = 0; m < newDay.Surveys.Count; m++)
                {
                    var targetNewSurvey = newDay.Surveys.OrderBy(x => x.Position).Skip(m).Take(1).FirstOrDefault();
                    var oldDaySurvey = oldDay == null ? null : oldDay.AssignedSurveys.Where(x => x.Position == targetNewSurvey.Position).FirstOrDefault();

                    if (oldDaySurvey == null)
                    {
                        await _playerSnapShotRepository.AddSurvey(targetNewSurvey.SurveyId, targetNewSurvey.DisplayWeeks, targetNewSurvey.Position, programDayId);

                    }
                    else if (targetNewSurvey.SurveyId != oldDaySurvey.SurveyId)
                    {
                        await _playerSnapShotRepository.DeleteSurvey(oldDaySurvey.ProgramDayItemSurveyId);
                        await _playerSnapShotRepository.AddSurvey(targetNewSurvey.SurveyId, targetNewSurvey.DisplayWeeks, targetNewSurvey.Position, programDayId);

                    }
                    else
                    {
                        var newDisplayWeeks = targetNewSurvey.DisplayWeeks.Except(oldDaySurvey.DisplayWeeks).ToList();
                        var weeksToDelete = oldDaySurvey.DisplayWeeks.Except(targetNewSurvey.DisplayWeeks).ToList();
                        await _playerSnapShotRepository.UpdateSurveyDisplayWeeks(newDisplayWeeks, oldDaySurvey.ProgramDayItemSurveyId, weeksToDelete);
                    }
                }
                for (int m = 0; m < newDay.Metrics.Count; m++)
                {
                    var targetNewMetric = newDay.Metrics.OrderBy(x => x.Position).Skip(m).Take(1).FirstOrDefault();
                    //We are doing to list because, when we get an assignedMetric from the DB it comes back as a list of items. THe only difference in each item is the display week. 
                    //Instead of doing a list of display weeks like we are in other places we are doing a list of items with different displayweeks
                    var oldMetric = oldDay == null ? null : oldDay.AssignedMetrics.Where(x => x.Position == targetNewMetric.Position).ToList();

                    //Old metric doesnt exsit for a corresponding new metric, determined by the fact the the new metric position exists and the old doesnt
                    if (oldMetric == null || oldMetric.Count == 0)
                    {
                        await _playerSnapShotRepository.AddMetric(targetNewMetric.MetricId, programDayId, targetNewMetric.DisplayWeeks, targetNewMetric.Position);

                    }

                    //same position different metrics, its the same position because we are ordering by position and taking the first
                    else if (targetNewMetric.MetricId != oldMetric.FirstOrDefault().MetricId)
                    {
                        await _playerSnapShotRepository.DeleteMetric(oldMetric.FirstOrDefault().MetricId);
                        await _playerSnapShotRepository.AddMetric(targetNewMetric.MetricId, programDayId, targetNewMetric.DisplayWeeks, targetNewMetric.Position);

                    }
                    //same metric need to check display weeks
                    else
                    {
                        var newDisplayWeeks = targetNewMetric.DisplayWeeks.Except(oldMetric.Select(x => x.DisplayWeekId)).ToList();
                        if (newDisplayWeeks.Count > 0)
                        {
                            await _playerSnapShotRepository.UpdateMetricDisplayWeeks(newDisplayWeeks, oldMetric.FirstOrDefault().ProgramDayItemMetricId);
                        }
                    }
                }
                for (int m = 0; m < newDay.Videos.Count; m++)
                {
                    var targetMovie = newDay.Videos[m];
                    await _playerSnapShotRepository.AddMovie(programDayId, targetMovie.MovieId, targetMovie.DisplayWeeks, targetMovie.Position);
                }
                for (int n = 0; n < newDay.Notes.Count; n++)
                {
                    var targetNote = newDay.Notes[n];
                    await _playerSnapShotRepository.AddNote(targetNote.Note, targetNote.Name, targetNote.DisplayWeeks, targetNote.Position, programDayId);
                }
                for (int s = 0; s < newDay.SuperSets.Count; s++)
                {
                    var newDaySuperSets = newDay.SuperSets.OrderBy(x => x.Position).Skip(s).Take(1).FirstOrDefault();
                    //haxing. if the old day is null then there arent any supersets and lets start fucking adding
                    //else there is a superset and lets look that shit up
                    var oldDaySuperSets = oldDay == null ? null : oldDay.AssignedSuperSets.Where(x => x.PositionInProgramDay == newDaySuperSets.Position).FirstOrDefault();

                    // there isnt a matchin superset in the position so create an entirly new one
                    if (oldDaySuperSets == null)
                    {
                        var newSuperSetNotes = newDaySuperSets.Notes.Select(x => new ProgramDaySuperSet_Note()
                        {
                            Position = x.Position,
                            Note = x.Note,
                            DisplayWeeks = x.DisplayWeeks
                        }).ToList();

                        var newExercises = newDaySuperSets.Exercises.Select(x => new ProgramDaySuperSet_Exercise()
                        {
                            ExerciseId = x.ExerciseId,
                            Name = x.ExerciseName,
                            Position = x.Position,
                            Rest = x.Rest,
                            ShowWeight = x.ShowWeight,
                            Weeks = x.Weeks.Select(y => new ProgramDaySuperSet_Weeks()
                            {
                                Position = y.Position,
                                SetsAndReps = y.SetsAndReps.Select(z => new ProgramDaySuperSet_Sets()
                                {
                                    Position = z.Position,
                                    Sets = z.Sets,
                                    Reps = z.Reps,
                                    Percent = z.Percent,
                                    Weight = z.Weight,
                                    Minutes = z.Minutes,
                                    Seconds = z.Seconds,
                                    Distance = z.Distance,
                                    RepsAchieved = z.RepsAchieved,
                                    Other = z.Other
                                }).ToList()
                            }).ToList()
                        }).ToList();
                        var wrecked = new DAL.DTOs.Program.ProgramDaySuperSet()
                        {
                            ProgramDayId = programDayId,
                            Position = newDaySuperSets.Position,
                            SuperSetDisplayTitle = newDaySuperSets.SuperSetDisplayTitle,
                            Notes = newSuperSetNotes,
                            Exercises = newExercises
                        };
                        await _programRepo.CreateSuperSetsforSnapshots(wrecked, programDayId, athleteId);
                    }
                    // there is a superset in the position so no need to create one, now we are on modifying
                    else if (newDaySuperSets.Position == oldDaySuperSets.PositionInProgramDay)
                    {
                        //hopefully it is impossible to have a superset that is empty of exercises. so in theory the first item in the exercises is guarenteed to be populated
                        var assignedProgram_ProgrmaDayItemSuperSetId = oldDaySuperSets.Exercises.First().ProgramDayItemSuperSetId;

                        //delete all exercsies that exist in the oldProgram but not the new program
                        var allOldPositionsToDelete = oldDaySuperSets.Exercises.Select(x => x.PositionInSuperSet).Except(newDaySuperSets.Exercises.Select(y => y.Position)).ToList();

                        for (int oldToDeleteIndex = 0; oldToDeleteIndex < allOldPositionsToDelete.Count(); oldToDeleteIndex++)
                        {
                            await _playerSnapShotRepository.DeleteSuperSetExercise(oldDaySuperSets.Exercises.FirstOrDefault(x => x.PositionInSuperSet == allOldPositionsToDelete[i]).SuperSetExerciseId);
                        }

                        for (int ss = 0; ss < newDaySuperSets.Exercises.Count; ss++)
                        {
                            var targetNewExercise = newDaySuperSets.Exercises.OrderBy(x => x.Position).Skip(ss).Take(1).FirstOrDefault();
                            var targetOldExercise = oldDaySuperSets.Exercises.Where(x => x.PositionInSuperSet == targetNewExercise.Position).FirstOrDefault();

                            //insert the exercise with the position
                            if (targetOldExercise == null)
                            {

                                var newExercise = new ProgramDaySuperSet_Exercise()
                                {
                                    ExerciseId = targetNewExercise.ExerciseId,
                                    Name = targetNewExercise.ExerciseName,
                                    Position = targetNewExercise.Position,
                                    Rest = targetNewExercise.Rest,
                                    ShowWeight = targetNewExercise.ShowWeight,
                                    Weeks = targetNewExercise.Weeks.Select(y => new ProgramDaySuperSet_Weeks()
                                    {
                                        Position = y.Position,
                                        SetsAndReps = y.SetsAndReps.Select(z => new ProgramDaySuperSet_Sets()
                                        {
                                            Position = z.Position,
                                            Sets = z.Sets,
                                            Reps = z.Reps,
                                            Percent = z.Percent,
                                            Weight = z.Weight,
                                            Minutes = z.Minutes,
                                            Seconds = z.Seconds,
                                            Distance = z.Distance,
                                            RepsAchieved = z.RepsAchieved,
                                            Other = z.Other
                                        }).ToList()
                                    }).ToList()
                                };
                                await _programRepo.AddSuperSetExerciseForSnapShot(newExercise, assignedProgram_ProgrmaDayItemSuperSetId,athleteId);
                            }

                            //completely new exercise better create and insert
                            else if (targetNewExercise.ExerciseId != targetOldExercise.SuperSet_ExerciseId)
                            {
                                //delete the original
                                await _playerSnapShotRepository.DeleteSuperSetExercise(targetOldExercise.SuperSetExerciseId);

                                // add the new
                                var newExercises = newDaySuperSets.Exercises.Where(x => x.ExerciseId == targetNewExercise.ExerciseId).Select(x => new ProgramDaySuperSet_Exercise()
                                {
                                    ExerciseId = x.ExerciseId,
                                    Name = x.ExerciseName,
                                    Position = x.Position,
                                    Rest = x.Rest,
                                    ShowWeight = x.ShowWeight,
                                    Weeks = x.Weeks.Select(y => new ProgramDaySuperSet_Weeks()
                                    {
                                        Position = y.Position,
                                        SetsAndReps = y.SetsAndReps.Select(z => new ProgramDaySuperSet_Sets()
                                        {
                                            Position = z.Position,
                                            Sets = z.Sets,
                                            Reps = z.Reps,
                                            Percent = z.Percent,
                                            Weight = z.Weight,
                                            Minutes = z.Minutes,
                                            Seconds = z.Seconds,
                                            Distance = z.Distance,
                                            RepsAchieved = z.RepsAchieved,
                                            Other = z.Other
                                        }).ToList()
                                    }).ToList()
                                }).FirstOrDefault();
                                await _programRepo.AddSuperSetExerciseForSnapShot(newExercises, assignedProgram_ProgrmaDayItemSuperSetId, athleteId);
                            }
                            else
                            {
                                //update the rest, its easier toupdate then check if its modified
                                await _playerSnapShotRepository.UpdateRest(targetNewExercise.Rest, targetOldExercise.SuperSetExerciseId);
                                var deleteOldWeeksWhichDoentExistInNewWorkout = targetOldExercise.SetsAndReps.Select(x => x.WeekPosition).Except(targetNewExercise.Weeks.Select(x => x.Position)).ToList();

                                for (int index = 0; i < deleteOldWeeksWhichDoentExistInNewWorkout.Count(); i++)
                                {
                                    var weeksTodelete = targetOldExercise.SetsAndReps.Where(w => w.WeekPosition == deleteOldWeeksWhichDoentExistInNewWorkout[index]).ToList();

                                    for (int w = 0; w < weeksTodelete.Count(); w++)
                                    {
                                        await _playerSnapShotRepository.DeleteSuperSetInWeek( weeksTodelete[w].AssignedProgram_ProgramDayItemSuperSetWeekId, weeksTodelete[w].WeekPosition);
                                    }
                                }

                                targetOldExercise.SetsAndReps.RemoveAll(x => deleteOldWeeksWhichDoentExistInNewWorkout.Contains(x.WeekPosition));

                                //add all new weeks to program that do not exist
                                var insertNewWeeksWhichDoentExistInOldWorkout = targetNewExercise.Weeks.Select(x => x.Position).Except(targetOldExercise.SetsAndReps.Select(x => x.WeekPosition)).ToList();

                                for (int index = 0; index < insertNewWeeksWhichDoentExistInOldWorkout.Count(); index++)
                                {
                                    var weekToadd = targetNewExercise.Weeks.Where(week => week.Position == insertNewWeeksWhichDoentExistInOldWorkout[index]).FirstOrDefault();
                                    var SuperSetWeekId = await _playerSnapShotRepository.AddSuperSetWeek(weekToadd.Position, targetOldExercise.SuperSetExerciseId);
                                    for (int setsRepsIndex = 0; setsRepsIndex < weekToadd.SetsAndReps.Count(); setsRepsIndex++)
                                    {
                                        var t = weekToadd.SetsAndReps[setsRepsIndex];
                                        if (t.Sets != null ||
                                            t.Reps != null ||
                                            t.Percent != null ||
                                            t.Minutes != null ||
                                            t.Seconds != null ||
                                            t.Distance != null ||
                                            t.Weight != null
                                            )
                                        {
                                            await _playerSnapShotRepository.AddSuperSetInWeek(t.Other, SuperSetWeekId, t.Position, t.Sets, t.Reps, 
                                                t.Percent, t.Weight, t.Minutes, t.Seconds, t.Distance, t.RepsAchieved, athleteId, targetNewExercise.ExerciseId);
                                        }
                                    }
                                }


                                //remove the new weeks, just leaving the old weeks which we will have to individual compare to update shit.
                                //the old weeks positions should match up 1 to 1 with the newweek positions at this time
                                targetNewExercise.Weeks.RemoveAll(x => insertNewWeeksWhichDoentExistInOldWorkout.Contains(x.Position));
                                //update all weeks which arent new
                                for (int w = 0; w < targetNewExercise.Weeks.Count; w++)
                                {
                                    var targetWeek = targetNewExercise.Weeks.OrderBy(x => x.Position).Skip(w).Take(1).FirstOrDefault();

                                    var oldWeek = targetOldExercise.SetsAndReps.Where(x => x.WeekPosition == targetWeek.Position).ToList();

                                    var newSetsAndReps = targetWeek.SetsAndReps.OrderBy(x => x.Position).ToList();

                                    //delete old sets and reps that dont exist in the new one
                                    var targetSetsRepsToDelete = oldWeek.Select(x => x.PositionInSet).Except(newSetsAndReps.Select(x => x.Position)).ToList();
                                    for (var ll = 0; ll < targetSetsRepsToDelete.Count(); ll++)
                                    {
                                        await _playerSnapShotRepository.DeleteSuperSetInWeek(oldWeek.First().AssignedProgram_ProgramDayItemSuperSetWeekId, targetSetsRepsToDelete[ll]) ;
                                    }
                                    foreach (var setsAndRep in newSetsAndReps)
                                    {
                                        
                                        var oldSetAndrep = oldWeek.Where(x => x.PositionInSet == setsAndRep.Position).FirstOrDefault();
                                        setsAndRep.RepsAchieved = setsAndRep.RepsAchieved == null ? false : setsAndRep.RepsAchieved.Value;
                                        if (oldSetAndrep == null)
                                        {
                                            var t = setsAndRep;
                                            //at this point, they should have atleast 1 set/rep scheme in old week. If not something is fucked and it (it being inserting the set/rep ) shouldn've never made it this fair
                                            await _playerSnapShotRepository.AddSuperSetInWeek(t.Other, oldWeek.First().AssignedProgram_ProgramDayItemSuperSetWeekId, t.Position, t.Sets, 
                                                t.Reps, t.Percent, t.Weight, t.Minutes, t.Seconds, t.Distance, t.RepsAchieved, athleteId, targetNewExercise.ExerciseId);
                                            continue;
                                        }
                                        if (setsAndRep.Distance == null
                                            && setsAndRep.Reps == null
                                            && setsAndRep.Seconds == null
                                            && setsAndRep.Minutes == null
                                            && setsAndRep.Sets == null
                                            && setsAndRep.Weight == null)
                                        {
                                            await _playerSnapShotRepository.DeleteSuperSetInWeek(oldSetAndrep.AssignedProgram_ProgramDayItemSuperSetWeekId, oldSetAndrep.PositionInSet);
                                            continue;
                                        }
                                        if (oldSetAndrep.AssignedOther != setsAndRep.Other)
                                        {
                                            await _playerSnapShotRepository.UpdateSuperSetOtherInWeek(setsAndRep.Other, oldSetAndrep.OriginalSuperSet_SetId, oldSetAndrep.AssignedProgram_ProgramDayItemSuperSetWeekId, oldSetAndrep.PositionInSet);
                                            //update other
                                        }
                                        if (oldSetAndrep.AssignedWorkoutDistance != setsAndRep.Distance
                                            || oldSetAndrep.AssignedWorkoutReps != setsAndRep.Reps
                                            || oldSetAndrep.AssignedWorkoutSeconds != setsAndRep.Seconds
                                            || oldSetAndrep.AssignedWorkoutMinutes != setsAndRep.Minutes
                                            || oldSetAndrep.AssignedWorkoutSets != setsAndRep.Sets
                                            || oldSetAndrep.RepsAchieved != setsAndRep.RepsAchieved
                                            || oldSetAndrep.AssignedWorkoutPercent != setsAndRep.Percent
                                            || oldSetAndrep.AssignedWorkoutWeight != setsAndRep.Weight)
                                        {

                                            await _playerSnapShotRepository.UpdateSuperSetInWeek(setsAndRep.Other, oldSetAndrep.OriginalSuperSet_SetId, oldSetAndrep.AssignedProgram_ProgramDayItemSuperSetWeekId, setsAndRep.Position,
                                                setsAndRep.Sets, setsAndRep.Reps, setsAndRep.Percent, setsAndRep.Weight, setsAndRep.Minutes, setsAndRep.Seconds, setsAndRep.Distance, setsAndRep.RepsAchieved, athleteId, targetNewExercise.ExerciseId);
                                        }


                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var newSuperSetNotes = newDaySuperSets.Notes.Select(x => new ProgramDaySuperSet_Note()
                        {
                            Position = x.Position,
                            Note = x.Note,
                            DisplayWeeks = x.DisplayWeeks
                        }).ToList();

                        var newExercises = newDaySuperSets.Exercises.Select(x => new ProgramDaySuperSet_Exercise()
                        {
                            ExerciseId = x.ExerciseId,
                            Name = x.ExerciseName,
                            Position = x.Position,
                            Rest = x.Rest,
                            ShowWeight = x.ShowWeight,
                            Weeks = x.Weeks.Select(y => new ProgramDaySuperSet_Weeks()
                            {
                                Position = y.Position,
                                SetsAndReps = y.SetsAndReps.Select(z => new ProgramDaySuperSet_Sets()
                                {
                                    Position = z.Position,
                                    Sets = z.Sets,
                                    Reps = z.Reps,
                                    Percent = z.Percent,
                                    Weight = z.Weight,
                                    Minutes = z.Minutes,
                                    Seconds = z.Seconds,
                                    Distance = z.Distance,
                                    RepsAchieved = z.RepsAchieved,
                                    Other = z.Other
                                }).ToList()
                            }).ToList()
                        }).ToList();
                        var wrecked = new DAL.DTOs.Program.ProgramDaySuperSet()
                        {
                            ProgramDayId = oldDaySuperSets.ProgramDayId,
                            Position = newDaySuperSets.Position,
                            SuperSetDisplayTitle = newDaySuperSets.SuperSetDisplayTitle,
                            Notes = newSuperSetNotes,
                            Exercises = newExercises
                        };
                        await _programRepo.CreateSuperSetsforSnapshots(wrecked, programDayId, athleteId);
                    }//position is different so we insert new thing

                }
            }
        }

        public int CreateProgram(b.Program.Program targetProgram, Guid createdUserGuid, List<ProgramTag> tagIds)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.CreatePrograms)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Programs");
            }
            try
            {
                if (!_userRepo.Get(createdUserGuid).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }

            var programId = 0;
            try
            {
                programId = _programRepo.CreateProgram(new Models.Program.Program() { Name = targetProgram.Name, WeekCount = targetProgram.WeekCount }, createdUserGuid);
            }
            catch (DuplicateKeyException dup)
            {
                throw new ItemAlreadyExistsException("A Program With This Name Already Exists", dup);

            }

            try
            {
                //makesure all programDay Positions start at 0 and increment up
                var orderByAsc = targetProgram.Days.OrderBy(x => x.Position).ToList();
                for (int i = 0; i < targetProgram.Days.Count; i++)
                {
                    orderByAsc[i].Position = i;
                }
                targetProgram.Days.ForEach(d =>
                {
                    var dayId = _programRepo.CreateProgramDay(programId, d.Position);

                    if (d.Videos != null)
                    {
                        d.Videos.ForEach(x =>
                        {
                            var dayItemId = _programRepo.CreateProgramDayItem(x.Position, Models.Enums.ProgramDayItemEnum.Video, dayId);
                            _programRepo.AddMovieToProgram(x.MovieId, dayItemId, x.DisplayWeeks);
                        });
                    }
                    if (d.Exercises != null)
                    {
                        d.Exercises.ForEach(x =>
                        {
                            var dayItemId = _programRepo.CreateProgramDayItem(x.Position, Models.Enums.ProgramDayItemEnum.Workout, dayId);
                            _programRepo.AddWorkoutToProgram(x.Weeks, dayItemId, x.ExerciseId, x.WorkoutId);
                        });
                    }
                    if (d.Metrics != null)
                    {
                        d.Metrics.ForEach(x =>
                        {
                            var dayItemId = _programRepo.CreateProgramDayItem(x.Position, Models.Enums.ProgramDayItemEnum.Metric, dayId);
                            _programRepo.AddMetricToProgram(x.MetricId, dayItemId, x.DisplayWeeks);
                        });
                    }
                    if (d.Surveys != null)
                    {
                        d.Surveys.ForEach(x =>
                        {
                            var dayItemId = _programRepo.CreateProgramDayItem(x.Position, Models.Enums.ProgramDayItemEnum.Survey, dayId);
                            _programRepo.AddSurveyToProgram(x.SurveyId, dayItemId, x.DisplayWeeks);
                        });
                    }
                    if (d.Notes != null)
                    {
                        d.Notes.ForEach(x =>
                        {
                            var dayItemId = _programRepo.CreateProgramDayItem(x.Position, Models.Enums.ProgramDayItemEnum.Note, dayId);
                            _programRepo.AddNoteToProgram(x.Note, x.Name, dayItemId, x.DisplayWeeks);
                        });
                    }
                    if (d.SuperSets != null)
                    {
                        d.SuperSets.ForEach(x =>
                        {
                            //I understand that I am breaking the standard. When I come back through I will put all this into the DAL where it belongs
                            //well its been about a year and this shit is still here so fuck me i guess
                            var dayItemId = _programRepo.CreateProgramDayItem(x.Position, Models.Enums.ProgramDayItemEnum.SuperSet, dayId);
                            var superSetId = _programRepo.InsertSuperSet(dayItemId, x.SuperSetDisplayTitle);
                            x.Notes.ForEach(n =>
                            {
                                _programRepo.InsertSuperSetNotes(n.Note, n.DisplayWeeks, n.Position, superSetId);
                            });
                            x.Exercises.ForEach(e =>
                            {
                                var superSetExerciseID = _programRepo.InsertSuperSetExercise(new SuperSetExercise()
                                {
                                    ExerciseId = e.ExerciseId,
                                    Position = e.Position,
                                    ProgramDayItemSuperSetId = superSetId,
                                    Rest = e.Rest,
                                    ShowWeight = e.ShowWeight

                                });
                                e.Weeks.ForEach(w =>
                                {

                                    var superSetWeekId = _programRepo.InsertSuperSetWeek(new ProgramDayItemSuperSetWeek()
                                    {
                                        Position = w.Position,
                                        SuperSetExerciseId = superSetExerciseID
                                    });

                                    var convertedSuperSet_Sets = w.SetsAndReps.Where(l =>
                                    {
                                        //ok welcome to crazytown. The user and howthis is written allows the user to save jsut a reps achieved week. With how the weeks auto insert a new week whenever you fill out an existing week
                                        // the UI will send us all the weeks the user filled out plus 1 extra week (because of the before mentioned autofill). The plus 1 week could potentionally be
                                        // just a reps achieved week. So we are making sure if reps achieved is turned on there has to be another field turned on too. Or we arent saving the week.

                                        return l.Percent != null ||
                                        l.Sets != null ||
                                        l.Reps != null ||
                                        l.Weight != null ||
                                        l.Minutes != null ||
                                        l.Seconds != null ||
                                        !String.IsNullOrEmpty(l.Distance) ||
                                        l.Other != null;
                                    }
                                    ).Select(sr => new ProgramDayItemSuperSet_Set()
                                    {
                                        Percent = sr.Percent,
                                        Position = sr.Position,
                                        Reps = sr.Reps,
                                        Sets = sr.Sets,
                                        Weight = sr.Weight,
                                        ProgramDayItemSuperSetWeekId = superSetWeekId,
                                        Distance = sr.Distance,
                                        Minutes = sr.Minutes,
                                        Seconds = sr.Seconds,
                                        Other = sr.Other,
                                        RepsAchieved = sr.RepsAchieved

                                    }).ToList();
                                    //since i remoeved items above, we need to remap position so there arent these scenares position : 1, position:5, position:6
                                    if (convertedSuperSet_Sets != null && convertedSuperSet_Sets.Any())
                                    {
                                        for (int i = 0; i < convertedSuperSet_Sets.Count; i++)
                                        {
                                            convertedSuperSet_Sets[i].Position = i;
                                        }
                                        _programRepo.InsertSuperSet_sets(convertedSuperSet_Sets, superSetWeekId);
                                    }
                                });
                            });

                        });
                    }
                });


                AddTagsToProgram(tagIds, programId, createdUserGuid);
                return programId;
            }
            catch (Exception e)
            {
                _programRepo.DontFuckingDoThis_Delete_All_Information_About_A_Program(targetProgram.Id);
                throw e;
            }

        }
        public void AddTagsToProgram(List<ProgramTag> tagIds, int programId, Guid createdUserId)
        {
            GenerateUserRoles(createdUserId);
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.CreatePrograms) || _userRoles.Contains(OrganizationRoleEnum.ModifyPrograms)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Programs");
            }
            var targetProgram = _programRepo.GetProgram(programId, createdUserId);
            if (targetProgram == null) return;

            _programTagRepo.DeleteAssociatedTags(programId);
            _programTagRepo.AddAssociatedTags(tagIds, programId);
        }
        public List<b.Program.Program> GetAllPrograms(Guid createdUserGuid)
        {
            var allMappings = _programRepo.GetAllProgramTagMappings(createdUserGuid);
            var allPrograms = _programRepo.GetAllPrograms(createdUserGuid);
            var allProgramsWithAdvancedFeaturesToggled = _programRepo.GetAllProgramIdsWithAdvancedFeaturesTurnedOn(createdUserGuid);
            var ret = new List<b.Program.Program>();

            foreach (var e in allPrograms)
            {
                var mapping = allMappings.FirstOrDefault(x => x.ProgramId == e.Id);
                ret.Add(new b.Program.Program()
                {
                    Id = e.Id,
                    Name = e.Name,
                    CanModify = e.CanModify,
                    IsDeleted = e.IsDeleted,
                    WeekCount = e.WeekCount,
                    DayCount = e.DayCount,
                    HasAdvancedOptions = allProgramsWithAdvancedFeaturesToggled.Contains(e.Id),
                    Tags = mapping == null ? new List<b.Tag>() : mapping.Tags.Select(x => new b.Tag() { Id = x.Id, Name = x.Name }).ToList()
                });
            }
            return ret;
        }
        public void Duplicate(int programId, Guid createdUserToken)
        {
            GenerateUserRoles(createdUserToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.CreatePrograms)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Programs");
            }
            _programRepo.DuplicateProgram(programId, createdUserToken);
        }
        public void PrintAllAssignedAthletePrograms(int programId, Guid createdUserToken, List<int> athleteIds, bool printMasterPdf, bool printSelectedAthletes, bool printUsingAdvancedOptions)
        {
            GenerateUserRoles(createdUserToken);
            if (Config.logEverything)
            {
                var user = _userRepo.Get(createdUserToken);
                _logRepo.LogShit(new Models.LogMessage()
                {
                    Message = "entering PrintAllAssignedAthletePrograms",
                    LoggedDate = DateTime.Now,
                    UserId = user.Id,
                    StackTrace = new System.Diagnostics.StackTrace().ToString()
                }); ;
            }
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.PrintPrograms)))
            {
                throw new ApplicationException("User Does Not Have Rights To Print Programs");
            }


            if (printUsingAdvancedOptions)
            {
                if (Config.logEverything)
                {
                    var user = _userRepo.Get(createdUserToken);
                    _logRepo.LogShit(new Models.LogMessage()
                    {
                        Message = $"calling pdf {Config.AzureFunctionsBaseUrl}/AthleteNewPdf?code={ Config.GenerateNewPdfCode }",
                        LoggedDate = DateTime.Now,
                        UserId = user.Id,
                        StackTrace = new System.Diagnostics.StackTrace().ToString()
                    }); ;
                }
                //todo: george we will need to change the code
                var newPdf = new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/AthleteNewPdf?code={Config.GenerateNewPdfCode}",
                        new StringContent(JsonConvert.SerializeObject(new PrintPDFOptions()
                        {
                            CreatedUserToken = createdUserToken,
                            ProgramId = programId,
                            PrintMasterPdf = printMasterPdf,
                            PrintOnlyTheseAthletes = athleteIds,
                            PrintSelectedAthletes = printSelectedAthletes,
                            UseNewPdf = printUsingAdvancedOptions
                        })));
            }
            else
            {
                if (Config.logEverything)
                {
                    var user = _userRepo.Get(createdUserToken);
                    _logRepo.LogShit(new Models.LogMessage()
                    {
                        Message = $"calling pdf Azure Function {Config.AzureFunctionsBaseUrl}/GeneratePDF?code={Config.GeneratePDFCode}",
                        LoggedDate = DateTime.Now,
                        UserId = user.Id,
                        StackTrace = new System.Diagnostics.StackTrace().ToString()
                    }); ;
                }
                var oldPdf = new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/GeneratePDF?code={Config.GeneratePDFCode}",
                         new StringContent(JsonConvert.SerializeObject(new PrintPDFOptions()
                         {
                             CreatedUserToken = createdUserToken,
                             ProgramId = programId,
                             PrintMasterPdf = printMasterPdf,
                             PrintOnlyTheseAthletes = athleteIds,
                             PrintSelectedAthletes = printSelectedAthletes,
                             UseNewPdf = printUsingAdvancedOptions
                         })));
            }
        }
    }
}