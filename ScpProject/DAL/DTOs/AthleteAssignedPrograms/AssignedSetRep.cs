namespace DAL.DTOs.AthleteAssignedPrograms
{
    public class AssignedSetRep
    {
        public int SetWeekId { get; set; }
        public int ProgramDayItemPosition { get; set; }
        public int AssignedWorkoutPercent { get; set; }
        public int AssignedWorkoutSets { get; set; }
        public int AssignedWorkoutReps { get; set; }
        public int AssignedWorkoutWeight { get; set; }
        public int OriginalSetId { get; set; }
        public int? CompletedSetPercent { get; set; }
        public int? CompletedSetSets { get; set; }
        public int? CompletedSetWeight { get; set; }
        public double? PercentMaxCalc { get; set; }
        public double? PercentMaxCalcSubPercent { get; set; }
        public string ExerciseName { get; set; }
        public int AssignedProgramId { get; set; }
        public int ProgramDayId { get; set; }
        public int PositionInSet { get; set; }
        public int ProgramDayItemExerciseId { get; set; }
        public int AthleteId { get; set; }
    }
    public class AssignedSuperSetSetRep
    {
        public int  OriginalSuperSet_SetId { get; set; }
        public int SuperSetWeekId { get; set; }
        public int? AssignedWorkoutPercent { get; set; }
        public int? AssignedWorkoutSets { get; set; }
        public int? AssignedWorkoutReps { get; set; }
        public int? AssignedWorkoutWeight { get; set; }
        public int? AssignedWorkoutMinutes { get; set; }
        public int? AssignedWorkoutSeconds { get; set; }
        public string AssignedWorkoutDistance { get; set; }
        public bool RepsAchieved { get; set; } //this isnt prefixed with Assigned because it is a yes/no. if it is yes the athletes has the option to input howmuch reps they did do. if it is no then they do not have that option
        public int AssignedRest { get; set; }
        public bool ShowWeight { get; set; }
        public string AssignedOther { get; set; }
        public int OriginalSetId { get; set; }
        public int? CompletedSetPercent { get; set; }
        public int? CompletedSetSets { get; set; }
        public int? CompletedSetWeight { get; set; }
        public int? CompletedRepsAchieved { get; set; }
        public double? PercentMaxCalc { get; set; }
        public double? PercentMaxCalcSubPercent { get; set; }
        public string ExerciseName { get; set; }
        public int AssignedProgramId { get; set; }
        public int ProgramDayId { get; set; }
        public int PositionInSet { get; set; }
        public int ProgramDayItemSuperSetId { get; set; }
        public int AthleteId { get; set; }
        public int SuperSetExerciseId { get; set; }
        public int SuperSet_ExerciseId { get; set; }
        public int WeekPosition { get; set; }
        public int AssignedProgram_ProgramDayItemSuperSetWeekId { get; set; }
    }
}
