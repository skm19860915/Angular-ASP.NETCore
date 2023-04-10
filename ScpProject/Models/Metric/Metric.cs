using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Metric
{
    public class Metric
    {

        public int Id { get; set; }
        [Index("IX_CreatedUserId_Name_Metrics", IsUnique = true, Order = 2), StringLength(200)]
        public string Name { get; set; }
        [Index("IX_CreatedUserId_Name_Metrics", IsUnique = true, Order = 1)]
        public int CreatedUserId { get; set; }
        public int? UnitOfMeasurementId { get; set; }

        public string Note { get; set; }
        public bool CanModify { get; set; }

        public bool IsDeleted { get; set; }
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization OwnerOrganization { get; set; }

        [ForeignKey("UnitOfMeasurementId")]
        public virtual UnitOfMeasurement AssociatedUnitOfMeasurement { get; set; }
    }
}
