using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;

namespace Models.Organization
{

    public class UserToOrganizationRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrganizationRoleId { get; set; }
      public int OrganizationId { get; set; }
        public int AssignedByUserId { get; set; }

        [ForeignKey("OrganizationRoleId")]
        public virtual OrganizationRole Role { get; set; }

        [ForeignKey("UserId")]
        public virtual User.User TargetUser { get; set; }

        [ForeignKey("AssignedByUserId")]
        public virtual User.User AssignedByUser { get; set; }
    }
}
