using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Athlete
{
    public class Athlete
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int CreatedUserId { get; set; }
        public int AthleteUserId { get; set; }//the user associated with an athlete
        public int? AssignedProgramId { get; set; }//v1 of programs, this is before we implemented programSnapShots
        public DateTime? ProgramStartDate { get; set; }//once this shit is implememnted it will probably just be for programSnapShots only
        public DateTime? ProgramEndDate { get; set; }//once this shit is implememnted it will probably just be for programSnapShots only
        public bool IsDeleted { get; set; }
        public string EmailValidationToken { get; set; }
        public DateTime? TokenIssued { get; set; }
        public bool ValidatedEmail { get; set; }
        public int? ProfilePictureId { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? AssignedProgram_AssignedTime { get; set; }
        public int? AssignedProgram_AssignedProgramId {get;set;} //this is the snapshot program
        //they cannot have assignedProgramId and assignedProgram_AssignedProgram both be non-null

        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization OwnerOrganization { get; set; }
        [ForeignKey("ProfilePictureId")]
        public virtual MultiMedia.Picture ProfilePicture { get; set; }
        [ForeignKey("AthleteUserId")]
        public virtual User.User AthleteUser { get; set; }
        [ForeignKey("CreatedUserId")]
        public virtual User.User CreatedUser { get; set; }
        public virtual List<AthleteNote> Notes { get; set; }
        public virtual List<AthleteInjury> Injuries { get; set; }
        [ForeignKey("AssignedProgramId")]
        public Program.AssignedProgram AssignedProgram { get; set; }

    }
}
