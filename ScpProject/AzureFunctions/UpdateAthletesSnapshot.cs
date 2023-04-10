using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using a = azureFunctions_program;
using b = DAL.DTOs.Program;

namespace AzureFunctions
{
    public static class UpdateAthletesSnapshot
    {
        [FunctionName("UpdateAthletesSnapshot")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var athleteData = JsonConvert.DeserializeObject<AthleteData>(requestBody);

            await UpdateAssignedSnapShot(athleteData.NewProgram, athleteData.AthleteId, athleteData.guid);
            return new OkResult();
        }

        public static async Task UpdateAssignedSnapShot(a.Program newProgram, int athleteId, Guid userToken)
        {
            var _programRepo = new DAL.Repositories.ProgramRepo(Config.SqlConn);
            var _athleteRepo = new DAL.Repositories.AthleteRepo(Config.SqlConn);
            var _playerSnapShotRepository = new DAL.Repositories.PlayerSnapShotRepository(Config.SqlConn);
            var targetAthlete = _athleteRepo.GetAthlete(athleteId);
            var oldSnapShot = _programRepo.GetAssignedProgramSnapShot(targetAthlete.AssignedProgram_AssignedProgramId.Value, userToken, targetAthlete.Id);


            for (int i = 0; i < newProgram.Days.Count; i++)
            {
                var newDay = newProgram.Days.FirstOrDefault(x => x.Position == i);
                var oldDay = oldSnapShot.Days.FirstOrDefault(x => x.Position == i);

                for (int m = 0; m < newDay.Metrics.Count; m++)
                {
                    var targetNewMetric = newDay.Metrics.OrderBy(x => x.Position).Skip(m).Take(1).FirstOrDefault();
                    //We are doing to list because, when we get an assignedMetric from the DB it comes back as a list of items. THe only difference in each item is the display week. 
                    //Instead of doing a list of display weeks like we are in other places we are doing a list of items with different displayweeks
                    var oldMetric = oldDay.AssignedMetrics.Where(x => x.Position == targetNewMetric.Position).ToList();

                    var oldMetricsWhichDontMatchUpToNewMetrics = oldDay.AssignedMetrics.Select(x => x.Position).Except(newDay.Metrics.Select(x => x.Position)).ToList();

                    //Old metric doesnt exsit for a corresponding new metric, determined by the fact the the new metric position exists and the old doesnt
                    if (oldMetric == null)
                    {
                        await _playerSnapShotRepository.AddMetric(targetNewMetric.MetricId, oldDay.Id, targetNewMetric.DisplayWeeks, targetNewMetric.Position);
                    }
                    //delete all metrics that exist inthe old one that doent exist in the new one.
                    for (var oldMetricPosition = 0; oldMetricPosition < oldMetricsWhichDontMatchUpToNewMetrics.Count(); oldMetricPosition++)
                    {
                        var programDayItemMetricId = oldDay.AssignedMetrics.Where(x => x.Position == oldMetricsWhichDontMatchUpToNewMetrics[oldMetricPosition]).FirstOrDefault().ProgramDayItemMetricId;
                        await _playerSnapShotRepository.DeleteMetric(programDayItemMetricId);
                    }

                    //same position different metrics, its the same position because we are ordering by position and taking the first
                    if (targetNewMetric.MetricId != oldMetric.FirstOrDefault().MetricId)
                    {
                        await _playerSnapShotRepository.DeleteMetric(oldMetric.FirstOrDefault().MetricId);
                    }
                    //same metric need to check display weeks
                    else
                    {
                        var newDisplayWeeks = targetNewMetric.DisplayWeeks.Except(oldMetric.Select(x => x.DisplayWeekId)).ToList();
                        await _playerSnapShotRepository.UpdateMetricDisplayWeeks(newDisplayWeeks, oldMetric.FirstOrDefault().ProgramDayItemMetricId);
                    }
                }
                for (int m = 0; m < newDay.Videos.Count; m++)
                {
                    //since movies dont have any thing tied to them, its easier just to delete all the old movies and add all the new ones
                    var oldDayMovies = oldDay.AssignedVideos;

                    var newMovies = newDay.Videos;
                    for (var movieIndex = 0; movieIndex < oldDayMovies.Count; movieIndex++)
                    {
                        await _playerSnapShotRepository.DeleteMovie(oldDayMovies[movieIndex].ProgramDayItemMovieId);
                    }
                    for (var newMovieIndex = 0; newMovieIndex < newMovies.Count(); newMovieIndex++)
                    {
                        var targetMovie = newMovies[newMovieIndex];
                        await _playerSnapShotRepository.AddMovie(oldDay.Id,targetMovie.MovieId, targetMovie.DisplayWeeks, targetMovie.Position);
                    }
                }
                for (int n = 0; n < newDay.Notes.Count; n++)
                {
                    //since notes dont have any thing tied to them, its easier just to delete all the old movies and add all the new ones

                    var oldDayNotes = oldDay.AssignedNotes;
                    var newNotes = newDay.Notes;

                    for (var noteIndex = 0; noteIndex < oldDayNotes.Count; noteIndex++)
                    {
                        await _playerSnapShotRepository.DeleteNote(oldDayNotes[noteIndex].ProgramDayItemNoteId);
                    }
                    for (var newNoteIndex = 0; newNoteIndex < newNotes.Count(); newNoteIndex++)
                    {
                        var targetNote = newNotes[newNoteIndex];
                        await _playerSnapShotRepository.AddNote(targetNote.Note, targetNote.Name, targetNote.DisplayWeeks, targetNote.Position, oldDay.Id);
                    }
                }
                for (int s = 0; s < newDay.SuperSets.Count; s++)
                {
                    var newDaySuperSets = newDay.SuperSets.OrderBy(x => x.Position).Skip(s).Take(1).FirstOrDefault();
                    var oldDaySuperSets = oldDay.AssignedSuperSets.Where(x => x.PositionInProgramDay == newDaySuperSets.Position).FirstOrDefault();

                    // there isnt a matchin superset in the position so create an entirly new one
                    if (oldDaySuperSets == null)
                    {
                        var newNotes = newDaySuperSets.Notes.Select(x => new b.ProgramDaySuperSet_Note()
                        {
                            Position = x.Position,
                            Note = x.Note,
                            DisplayWeeks = x.DisplayWeeks
                        }).ToList();

                        var newExercises = newDaySuperSets.Exercises.Select(x => new b.ProgramDaySuperSet_Exercise()
                        {
                            ExerciseId = x.ExerciseId,
                            Name = x.ExerciseName,
                            Position = x.Position,
                            Rest = x.Rest,
                            ShowWeight = x.ShowWeight,
                            Weeks = x.Weeks.Select(y => new b.ProgramDaySuperSet_Weeks()
                            {
                                Position = y.Position,
                                SetsAndReps = y.SetsAndReps.Select(z => new b.ProgramDaySuperSet_Sets()
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
                            Notes = newNotes,
                            Exercises = newExercises
                        };
                        await _programRepo.CreateSuperSetsforSnapshots(wrecked, oldDay.Id, athleteId);
                    }
                    // there is a superset in the position so no need to create one, now we are on modifying
                    else if (newDaySuperSets.Position == oldDaySuperSets.PositionInProgramDay)
                    {
                        //hopefully it is impossible to have a superset that is empty of exercises. so in theory the first item in the exercises is guarenteed to be populated
                        var assignedProgram_ProgrmaDayItemSuperSetId = oldDaySuperSets.Exercises.First().ProgramDayItemSuperSetId;
                        for (int ss = 0; ss < newDaySuperSets.Exercises.Count; ss++)
                        {
                            var targetNewExercise = newDaySuperSets.Exercises.OrderBy(x => x.Position).Skip(ss).Take(1).FirstOrDefault();
                            var targetOldExercise = oldDaySuperSets.Exercises.Where(x => x.PositionInSuperSet == targetNewExercise.Position).FirstOrDefault();

                            //insert the exercise with the position
                            if (targetOldExercise == null)
                            {

                                var newExercises = newDaySuperSets.Exercises.Select(x => new b.ProgramDaySuperSet_Exercise()
                                {
                                    ExerciseId = x.ExerciseId,
                                    Name = x.ExerciseName,
                                    Position = x.Position,
                                    Rest = x.Rest,
                                    ShowWeight = x.ShowWeight,
                                    Weeks = x.Weeks.Select(y => new b.ProgramDaySuperSet_Weeks()
                                    {
                                        Position = y.Position,
                                        SetsAndReps = y.SetsAndReps.Select(z => new b.ProgramDaySuperSet_Sets()
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
                                var addExerciseTask = newExercises.Select(p => _programRepo.AddSuperSetExerciseForSnapShot(p, assignedProgram_ProgrmaDayItemSuperSetId, athleteId));
                                await Task.WhenAll(addExerciseTask);
                            }

                            //completely new exercise better create and insert
                            else if (targetNewExercise.ExerciseId != targetOldExercise.SuperSet_ExerciseId)
                            {
                                //delete the original
                                await _playerSnapShotRepository.DeleteSuperSetExercise(targetOldExercise.SuperSetExerciseId);

                                // add the new
                                var newExercises = newDaySuperSets.Exercises.Where(x => x.ExerciseId == targetNewExercise.ExerciseId).Select(x => new b.ProgramDaySuperSet_Exercise()
                                {
                                    ExerciseId = x.ExerciseId,
                                    Name = x.ExerciseName,
                                    Position = x.Position,
                                    Rest = x.Rest,
                                    ShowWeight = x.ShowWeight,
                                    Weeks = x.Weeks.Select(y => new b.ProgramDaySuperSet_Weeks()
                                    {
                                        Position = y.Position,
                                        SetsAndReps = y.SetsAndReps.Select(z => new b.ProgramDaySuperSet_Sets()
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

                                var insertNewWeeksWhichDoentExistInOldWorkout = targetNewExercise.Weeks.Select(x => x.Position).Except(targetOldExercise.SetsAndReps.Select(x => x.WeekPosition)).ToList();

                                for (int index = 0; index < insertNewWeeksWhichDoentExistInOldWorkout.Count(); index++)
                                {
                                    var weeksToadd = targetNewExercise.Weeks.Where(week => week.Position == insertNewWeeksWhichDoentExistInOldWorkout[index]).FirstOrDefault();

                                    for (int setsRepsIndex = 0; setsRepsIndex < weeksToadd.SetsAndReps.Count(); setsRepsIndex++)
                                    {
                                        var t = weeksToadd.SetsAndReps[setsRepsIndex];
                                        await _playerSnapShotRepository.AddSuperSetInWeek(t.Other, targetOldExercise.SetsAndReps.First().AssignedProgram_ProgramDayItemSuperSetWeekId, 
                                            t.Position, t.Sets, t.Reps, t.Percent, t.Weight, t.Minutes, t.Seconds, t.Distance, t.RepsAchieved, athleteId, targetNewExercise.ExerciseId);
                                    }
                                }


                                targetNewExercise.Weeks.RemoveAll(x => insertNewWeeksWhichDoentExistInOldWorkout.Contains(x.Position));

                                for (int w = 0; w < targetNewExercise.Weeks.Count; w++)
                                {
                                    var targetWeek = targetNewExercise.Weeks.OrderBy(x => x.Position).Skip(w).Take(1).FirstOrDefault();
                                    if (targetWeek == null)
                                    {
                                        continue;
                                    }
                                    var oldWeek = targetOldExercise.SetsAndReps.Where(x => x.WeekPosition == targetWeek.Position).ToList();

                                    if (oldWeek == null)
                                    {
                                        for (int setsRepsIndex = 0; setsRepsIndex < targetWeek.SetsAndReps.Count(); setsRepsIndex++)
                                        {
                                            var t = targetWeek.SetsAndReps[setsRepsIndex];
                                            await _playerSnapShotRepository.AddSuperSetInWeek(t.Other, targetOldExercise.SetsAndReps.First().AssignedProgram_ProgramDayItemSuperSetWeekId,
                                                t.Position, t.Sets, t.Reps, t.Percent, t.Weight, t.Minutes, t.Seconds, t.Distance, t.RepsAchieved, athleteId, targetNewExercise.ExerciseId);
                                        }
                                        continue;
                                    }


                                    var newSetsAndReps = targetWeek.SetsAndReps.OrderBy(x => x.Position).ToList();

                                    //delete all positions larger than last

                                    foreach (var setsAndRep in newSetsAndReps)
                                    {

                                        var oldSetAndrep = oldWeek.Where(x => x.PositionInSet == setsAndRep.Position).FirstOrDefault();
                                        if (oldSetAndrep == null)
                                        {
                                            var t = setsAndRep;
                                            await _playerSnapShotRepository.AddSuperSetInWeek(t.Other, targetOldExercise.SetsAndReps.First().AssignedProgram_ProgramDayItemSuperSetWeekId, t.Position, t.Sets,
                                                t.Reps, t.Percent, t.Weight, t.Minutes, t.Seconds, t.Distance, t.RepsAchieved, athleteId,targetNewExercise.ExerciseId);
                                            continue;
                                        }
                                        if (!oldSetAndrep.AssignedOther.Equals(setsAndRep.Other))
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
                                            || oldSetAndrep.AssignedWorkoutPercent != setsAndRep.Percent)
                                        {

                                            await _playerSnapShotRepository.UpdateSuperSetInWeek(setsAndRep.Other, oldSetAndrep.OriginalSuperSet_SetId, oldSetAndrep.AssignedProgram_ProgramDayItemSuperSetWeekId, setsAndRep.Position,
                                                setsAndRep.Sets, setsAndRep.Reps, setsAndRep.Percent, setsAndRep.Weight, setsAndRep.Minutes, setsAndRep.Seconds, setsAndRep.Distance, setsAndRep.RepsAchieved,athleteId, targetNewExercise.ExerciseId);
                                        }


                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var newNotes = newDaySuperSets.Notes.Select(x => new b.ProgramDaySuperSet_Note()
                        {
                            Position = x.Position,
                            Note = x.Note,
                            DisplayWeeks = x.DisplayWeeks
                        }).ToList();

                        var newExercises = newDaySuperSets.Exercises.Select(x => new b.ProgramDaySuperSet_Exercise()
                        {
                            ExerciseId = x.ExerciseId,
                            Name = x.ExerciseName,
                            Position = x.Position,
                            Rest = x.Rest,
                            ShowWeight = x.ShowWeight,
                            Weeks = x.Weeks.Select(y => new b.ProgramDaySuperSet_Weeks()
                            {
                                Position = y.Position,
                                SetsAndReps = y.SetsAndReps.Select(z => new b.ProgramDaySuperSet_Sets()
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
                            Notes = newNotes,
                            Exercises = newExercises
                        };
                        await _programRepo.CreateSuperSetsforSnapshots(wrecked, oldDay.Id, athleteId);
                    }//position is different so we insert new thing

                }
                //delete all old supersets that do not have the same position as new ones

                var shitToDelete = oldDay.AssignedSuperSets.Select(x => x.PositionInProgramDay).Except(newDay.SuperSets.Select(x => x.Position));

            }
        }
    }
    public class AthleteData
    {
        public a.Program NewProgram { get; set; }
        public int AthleteId { get; set; }
        public Guid guid { get; set; }
        //b.Program.Program newProgram, int athleteId, Guid userToken
    }
}
namespace azureFunctions_program
{
    /// <summary>
    /// whelp i suck at programmingg. once we get to a resting point/interns to help fix theis fucking mess.
    /// we are going to have a models folder with all the fucking models and that will be shared so we dont have 
    /// fucking duplications. what duplication you ask check out Program.cs
    /// </summary>
    public class Program
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProgramDay> Days { get; set; }
        public int WeekCount { get; set; }
        public bool CanModify { get; set; }
        public bool IsDeleted { get; set; }
        public int DayCount { get; set; }
        public bool HasAdvancedOptions { get; set; }
    }
    public class ProgramDay
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public List<ProgramDayItemExercise> Exercises { get; set; }
        public List<ProgramDayItemMetric> Metrics { get; set; }
        public List<ProgramDayItemSurvey> Surveys { get; set; }
        public List<ProgramDayItemNote> Notes { get; set; }
        public List<ProgramDayItemSuperSet> SuperSets { get; set; }
        public List<ProgramDayItemVideo> Videos { get; set; }

    }
    public class ProgramDayItemSuperSet
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public string SuperSetDisplayTitle { get; set; }
        public List<SuperSet_Exercise> Exercises { get; set; }
        public List<SuperSetNote> Notes { get; set; }
    }
    public class SuperSetNote
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public List<int> DisplayWeeks { get; set; }
        public string Note { get; set; }
    }
    public class SuperSet_Exercise
    {
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int Position { get; set; }
        public string Rest { get; set; }
        //Weight and Load are the same thing
        public bool ShowWeight { get; set; }

        public List<SuperSet_Week> Weeks { get; set; }

    }
    public class SuperSet_Week
    {
        public int Position { get; set; }
        public List<SuperSet_SetRep> SetsAndReps { get; set; }

    }
    public class SuperSet_SetRep
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public double? Percent { get; set; }
        public double? Weight { get; set; }
        public int? Minutes { get; set; }
        public int? Seconds { get; set; }
        public string Distance { get; set; }
        public bool? RepsAchieved { get; set; }
        public string Other { get; set; }
        public int ParentSuperSetWeekId { get; set; }
    }

    public class ProgramDayItemExercise
    {
        public int ExerciseId { get; set; }
        public int WorkoutId { get; set; }
        public int Position { get; set; }
        public List<Models.Program.ProgramWeek> Weeks { get; set; }
    }
    public class ProgramDayItemMetric
    {
        public int MetricId { get; set; }
        public int Position { get; set; }
        public List<int> DisplayWeeks { get; set; }

    }
    public class ProgramDayItemSurvey
    {
        public int SurveyId { get; set; }
        public int Position { get; set; }
        public List<int> DisplayWeeks { get; set; }
    }
    public class ProgramDayItemNote
    {
        public string Name { get; set; }
        public string Note { get; set; }
        public int Position { get; set; }
        public List<int> DisplayWeeks { get; set; }
    }
    public class ProgramDayItemVideo
    {
        public int MovieId { get; set; }
        public int Position { get; set; }
        public List<int> DisplayWeeks { get; set; }
    }
}
