using System;
using System.Collections.Generic;
using b = BL.BusinessObjects;
using m = Models;

namespace BL.BusinessObjects
{
    public class Athlete
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int AthleteUserId { get; set; }//the user associated with an athlete
        public int? ProgramId { get; set; }//this overrides a team assigned program
        public double? HeightPrimary { get; set; }
        public double? HeightSecondary { get; set; }
        public DateTime? Birthday { get; set; }
        public double? Weight { get; set; }
        public int UserId { get; set; }

        public string Email { get; set; }
        public List<b.Tag> Tags { get; set; }
        public m.MultiMedia.Picture ProfilePicture { get; set; }
        public bool IsDeleted { get; set; }
        public bool ValidatedEmail { get; set; }

    }
}
