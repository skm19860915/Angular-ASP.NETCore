using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models.Program
{
    public class Program
    {
        public int Id { get; set; }
        [Index("IX_CreatedUserId_Name_Programs", IsUnique = true, Order = 2), StringLength(400)]
        public string Name { get; set; }
        [Index("IX_CreatedUserId_Name_Programs", IsUnique = true, Order = 1)]
        public int CreatedUserId { get; set; }
        public bool IsDeleted { get; set; }
        public bool CanModify { get; set; }
        public int WeekCount { get; set; }
        public int DayCount { get; set; }
        public virtual User.User CreatedUser { get; set; }
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization OwnerOrganization { get; set; }
    }
}
