using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Athlete;

namespace Models.Program.AssignedProgramSnapShots
{
   public class AssignedProgram_Program
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreatedUserId { get; set; }
        public int WeekCount { get; set; }
        public int DayCount { get; set; }
        public virtual User.User CreatedUser { get; set; }
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization OwnerOrganization { get; set; }

    }
}
