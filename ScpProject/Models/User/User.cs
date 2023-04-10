
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.User
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsCoach { get; set; }
        public bool IsHeadCoach { get; set; }
        public string Email { get; set; }
        public int FailedEntryAttempts { get; set; }
        public bool LockedOut { get; set; }
        public int? ProfilePictureId { get; set; }
        public bool IsDeleted { get; set; }
        public string ImageContainerName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailValidationToken { get; set; }
        public bool IsEmailValidated { get; set; }
        public int OrganizationId { get; set; }
        public bool VisitedExercise { get; set; }
        public bool VisitedPrograms { get; set; }
        public bool VistedRosters { get; set; }
        public bool VisitedSurveys { get; set; }
        public bool VisitedSetsReps { get; set; }
        public bool VisitedCoachRoster { get; set; }
        public bool VisitedMetrics { get; set; }
        public bool VisitedProgramBuilder { get; set; }
        
        public Guid SignalRGroupID { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization OwnerOrganization { get; set; }

    }
}
