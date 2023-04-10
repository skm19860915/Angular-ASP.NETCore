using System;

namespace DAL.DTOs
{
    public class AthleteDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int CreatedUserId { get; set; }
        public Boolean IsDeleted { get; set; }
        public double? HeightPrimary { get; set; }
        public double? HeightSecondary { get; set; }
        public DateTime? Birthday { get; set; }
        public double? Weight { get; set; }
        public string OrganizationName { get; set; }
    }
}

