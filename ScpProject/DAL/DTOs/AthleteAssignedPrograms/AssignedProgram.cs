using DAL.DTOs.Program;
using Models.Enums;
using System.Collections.Generic;

namespace DAL.DTOs.AthleteAssignedPrograms
{
    public class AssignedProgram
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreatedUserId { get; set; }
        public List<AssignedProgramDays> Days { get; set; }
        public List<CompletedAssignedProgramDay> CompletedDays { get; set; }
        public int WeekCount { get; set; }
        public int AthleteId { get; set; }
        public bool IsSnapShot { get; set; }
    }

    public class CompletedAssignedProgramDay
    {
        public int ProgramDayId { get; set; }
        public int WeekNumber { get; set; }
    }
    public class AssignedProgramDays
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public List<AssignedProgramDaySurvey> AssignedSurveys { get; set; }
        public List<AssignedMetric> AssignedMetrics { get; set; }
        public List<AssignedExercise> AssignedExercises { get; set; }
        public List<AssignedNote> AssignedNotes { get; set; }
        public List<AssignedSuperSet> AssignedSuperSets { get; set; }
        public List<AssignedVideo> AssignedVideos { get; set; }
    }
    public class AssignedVideo
    {
        public int ProgramDayItemMovieId { get; set; }
        public int MovieId { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int DisplayWeekId { get; set; }
        public int AssignedProgramId { get; set; }
        public int ProgramId { get; set; }
        public int Position { get; set; }
        public int ProgramDayId { get; set; }
    }
    public class AssignedNote
    {
        public int AssignedProgramId { get; set; }
        public int ProgramDayItemNoteId { get; set; }
        public int ProgramDayId { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public string NoteText { get; set; }

        public int DisplayWeekId { get; set; }
    }
    public class AssignedSuperSet
    {
        public int SuperSetId { get; set; }
        public int ProgramDayId { get; set; }
        public int PositionInProgramDay { get; set; }
        public string SuperSetDisplayTitle { get; set; }
        public List<AssignedSuperSetExercise> Exercises { get; set; }
        public List<AssignedSuperSetNote> Notes { get; set; }
    }
    public class AssignedSuperSetNote
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public List<int> DisplayWeeks { get; set; }
        public int Position { get; set; }
        public int ProgramDayItemSuperSetId { get; set; }
    }
    public class AssignedSuperSetExercise
    {
        public int PositionInSuperSet { get; set; }
        /// <summary>
        /// The Id of the exercise. Just the id of the exercise that the superItem is
        /// </summary>
        public int SuperSet_ExerciseId { get; set; }
        public int ProgramDayItemSuperSetId { get; set; }
        public string ExerciseName { get; set; }
        /// <summary>
        /// the Id of the SuperSetExercise Table. A SuperSetExercise Table mapes up supersets to exercises and their position inside the superset
        /// </summary>
        public int SuperSetExerciseId { get; set; }
        public string VideoURL { get; set; }
        public int VideoProvider { get; set; }
        public string Rest { get; set; }
        public bool ShowWeight { get; set; }
        public List<AssignedSuperSetSetRep> SetsAndReps { get; set; }
    }
    public class AssignedExercise
    {
        public List<AssignedSetRep> AssignedSetsReps { get; set; }
        public int Position { get; set; }
    }
    public class AssignedProgramDaySurvey
    {
        public ProgramDayItemEnum ItemType { get { return ProgramDayItemEnum.Survey; } }
        public int ProgramDayItemSurveyId { get; set; }
        public int ProgramDayId { get; set; }
        public int Position { get; set; }
        public int SurveyId { get; set; }
        public string SurveyName;
        public List<AthleteAssignedQuestions> Questions { get; set; }
        public List<int> DisplayWeeks { get; set; }
    }

}
