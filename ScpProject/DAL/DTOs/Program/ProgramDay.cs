using System.Collections.Generic;

namespace DAL.DTOs.Program
{
    public class ProgramDay
    {

        public int Id { get; set; }
        public int Position { get; set; }
        public List<ProgramDayExercise> Exercises { get; set; }
        public List<ProgramDaySurvey> Surveys { get; set; }
        public List<ProgramDayNotes> Notes { get; set; }
        public List<ProgramDayMetrics> Metrics { get; set; }
        public List<ProgramDaySuperSet> SuperSets { get; set; }
        public List<ProgramDayMovie> Movies { get; set; }

        public int TotalDayItemCount
        {
            get
            {

                return (Exercises == null ? 0 : Exercises.Count)
                    + (Surveys == null ? 0 : Surveys.Count)
                    + (Notes == null ? 0 : Notes.Count)
                    + (Metrics == null ? 0 : Metrics.Count);
            }
        }
    }

    public class ProgramDayMovie
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int ProgramDayId { get; set; }
        public Models.MultiMedia.Movie Video { get; set; }
        public List<int> DisplayWeeks { get; set; }
    }
    public class ProgramDayExercise
    {
        public int Id { get; set; }
        public int ProgramDayId { get; set; }
        public int Position { get; set; }
        public Models.Exercise.Exercise Exercise { get; set; }
        public Models.SetsAndReps.Workout Workout { get; set; }
        public List<Models.Program.ProgramWeek> Weeks { get; set; }

    }

    public class ProgramDaySuperSet
    {
        public int Id { get; set; }
        public int ProgramDayId { get; set; }
        public int Position { get; set; }
        public int ProgramDaySuperSetId { get; set; }
        public string SuperSetDisplayTitle { get; set; }
        public List<ProgramDaySuperSet_Note> Notes { get; set; }
        public List<ProgramDaySuperSet_Exercise> Exercises { get; set; }
    }

    public class ProgramDaySuperSet_Note
    {
        public int Id { get; set; }
        public int ProgramDayItemSuperSetId { get; set; }
        public int Position { get; set; }
        public string Note { get; set; }
        public List<int> DisplayWeeks { get; set; }
    }

    public class ProgramDaySuperSet_Exercise
    {
        public int programDaySuperSet_ExerciseId { get; set; }
        public int ExerciseId { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public int Position { get; set; }
        public int ProgramDaySuperSetId { get; set; }
        public string Rest { get; set; }
        public bool ShowWeight { get; set; }
        public List<ProgramDaySuperSet_Weeks> Weeks { get; set; }
    }

    public class ProgramDaySuperSet_Weeks
    {
        public int ProgramDaySuperSet_WeeksId { get; set; }
        public int Position { get; set; }
        public int ProgramDaySuperSet_ExerciseId { get; set; }
        public List<ProgramDaySuperSet_Sets> SetsAndReps { get; set; }
    }

    public class ProgramDaySuperSet_Sets
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public double? Percent { get; set; }
        public double? Weight { get; set; }
        public int ProgramDayItemSuperSetWeekId { get; set; }
        public int? Minutes { get; set; }
        public int? Seconds { get; set; }
        public string Distance { get; set; }
        public bool? RepsAchieved { get; set; }
        public string Other { get; set; }
    }

    public class ProgramDaySurvey
    {
        public int Id { get; set; }
        public int ProgramDayId { get; set; }
        public int Position { get; set; }
        public int SurveyId { get; set; }
        public string SurveyName;
        public List<DTOs.QuestionDTO> Questions { get; set; }
        public List<Models.Survey.SurveyTag> Tags { get; set; }
        public List<int> DisplayWeeks { get; set; }
    }

    public class ProgramDayNotes
    {
        public int ProgramDayId { get; set; }
        public int Position { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public int Id { get; set; }
        public List<int> DisplayWeeks { get; set; }
    }

    public class ProgramDayMetrics
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int ProgramDayId { get; set; }
        public Models.Metric.Metric Metric { get; set; }
        public List<int> DisplayWeeks { get; set; }
    }
}
