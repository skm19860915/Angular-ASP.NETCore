using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models.SetsAndReps
{
    public class Workout
    {
        public int Id { get; set; }
        [Index("IX_CreatedUserId_Name_Workouts", IsUnique = true, Order = 2), StringLength(200)]
        public string Name { get; set; }
        public String Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        [Index("IX_CreatedUserId_Name_Workouts", IsUnique = true, Order = 1)]
        public int CreatedUserId { get; set; }
        public virtual List<Week> TotalWorkout { get; set; }
        public string Rest { get; set; }
        public bool ShowWeight { get; set; }
        public bool CanModify { get; set; }
        public bool IsDeleted { get; set; }
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization OwnerOrganization { get; set; }
        [ForeignKey("CreatedUserId")]
        public virtual User.User CreatedUser { get; set; }
    }
}
