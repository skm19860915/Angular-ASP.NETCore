using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Athlete
{
    [Table("TagsToAthletes")]
    public class TagToAthlete
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int AthleteId { get; set; }

        [ForeignKey("TagId")]
        public virtual AthleteTag Tag { get; set; }

        [ForeignKey("AthleteId")]
        public virtual Athlete TaggedAthlete { get; set; }
    }
}
