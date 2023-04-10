using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Survey
{
    public class Question
    {
        public int Id { get; set; }
        [Index("IX_CreatedUserId_QuestionDisplayText_Questions", IsUnique = true, Order = 2), StringLength(300)]
        public string QuestionDisplayText { get; set; }
        public int QuestionTypeId { get; set; }
        [Index("IX_CreatedUserId_QuestionDisplayText_Questions", IsUnique = true, Order = 1)]
        public int CreatedUserId { get; set; }
        public bool IsDeleted { get; set; }
      public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization OwnerOrganization { get; set; }
        public bool CanModify { get; set; }
        [ForeignKey("QuestionTypeId")]
        public virtual QuestionType Type { get; set; }

        public virtual User.User CreatedUser { get; set; }


    }
}
