
using System.ComponentModel.DataAnnotations.Schema;
namespace Models.Survey
{
    [Table("TagsToSurveys")]
  public  class TagToSurvey
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int SurveyId { get; set; }

        [ForeignKey("TagId")]
        public virtual SurveyTag Tag { get; set; }

        [ForeignKey("SurveyId")]
        public virtual Survey TaggedProgram { get; set; }
    }
}
