using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Metric
{
    public class UnitOfMeasurement
    {
        public int Id { get; set; }
        [Index("IX_CreatedUser_Id_UnitType",IsUnique = true,Order = 2), StringLength(200)]
        public string UnitType { get; set; }
        [Index("IX_CreatedUser_Id_UnitType", IsUnique = true, Order = 1)]
        public int CreatedUserId { get; set; }
      public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization OwnerOrganization { get; set; }


    }
}
