using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Exercise
{
    public class Exercise
    {
        
        public int Id { get; set; }
        [Index("IX_CreatedUserId_Name_Exercises",IsUnique = true, Order = 2), StringLength(200) ]
        public string Name { get; set; }
        public string Notes { get; set; }
        [Index("IX_CreatedUserId_Name_Exercises", IsUnique = true, Order = 1)]
        public int CreatedUserId { get; set; }
        public bool IsDeleted { get; set; }
        public bool CanModify { get; set; }
        public double? Percent { get; set; }
        public int? PercentMetricCalculationId { get; set; }
        public string VideoURL { get; set; }
      public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization OwnerOrganization { get; set; }

        [ForeignKey("PercentMetricCalculationId")]
        public virtual Metric.Metric PercentCalculation { get; set; }


        [ForeignKey("CreatedUserId")]
        public virtual User.User CreatedUser { get; set; }
    }
}
