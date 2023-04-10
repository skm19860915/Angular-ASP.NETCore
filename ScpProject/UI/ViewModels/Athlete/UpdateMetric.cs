using System;

namespace Controllers.ViewModels.Athlete
{
    public class UpdateMetric
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public DateTime CompletedDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}