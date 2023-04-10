using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Athlete
{
    public  class AthleteHeightWeight
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public double? HeightPrimary { get; set; }
        public double? HeightSecondary { get; set; }
        public double? Weight { get; set; }
        public DateTime AddedDate { get; set; }
        [ForeignKey("AthleteId")]
        public virtual Athlete athlete { get; set; }
    }
}
