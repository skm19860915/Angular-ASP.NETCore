using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Survey
{
    public class Survey
    {
        public int Id { get; set; }
        [Index("IX_CreatedUserId_Title_Surveys", IsUnique = true, Order = 2), StringLength(300)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Index("IX_CreatedUserId_Title_Surveys", IsUnique = true, Order = 1)]
        public int CreatedUserId { get; set; }
        public bool CanModify { get; set; }
        public bool IsDeleted { get; set; }
      public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization OwnerOrganization { get; set; }
        [ForeignKey("CreatedUserId")]
        public virtual User.User CreatedUser { get; set; }
    }
}
