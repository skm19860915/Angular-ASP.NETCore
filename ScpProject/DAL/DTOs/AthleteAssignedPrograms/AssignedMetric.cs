namespace DAL.DTOs.AthleteAssignedPrograms
{
    public  class AssignedMetric
    {
        public int ProgramDayItemMetricId { get; set; }
        public int MetricId { get; set; }
        public string MetricName { get; set; }
        public int Position { get; set; }
        public int ProgramDayId { get; set; }
        public int DisplayWeekId { get; set; }
        public int AssignedProgramId { get; set; }
        public int ProgramId { get; set; }

        public double? CompletedWeight { get; set; }
    }
}
