using System.Collections.Generic;
using b = BL.BusinessObjects;

namespace BL.BusinessObjects.Program
{
    /// <summary>
    /// whelp i suck at programmingg. once we get to a resting point/interns to help fix theis fucking mess.
    /// we are going to have a models folder with all the fucking models and that will be shared so we dont have 
    /// fucking duplications. what duplication you ask check out UpdateAthletesSnapshot.cs
    /// </summary>
    public class Program
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProgramDay> Days { get; set; }
        public List<b.Tag> Tags { get; set; }
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
