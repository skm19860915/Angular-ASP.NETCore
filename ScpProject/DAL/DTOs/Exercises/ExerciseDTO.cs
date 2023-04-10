namespace DAL.DTOs.Exercises
{
    public class ExerciseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public int CreatedUserId { get; set; }
        public bool IsDeleted { get; set; }
        public bool CanModify { get; set; }
        public double? Percent { get; set; }
        public int? PercentMetricCalculationId { get; set; }
        public string VideoURL { get; set; }
        public int OrganizationId { get; set; }
        public string CalcMetricName { get; set; }
    }
}
