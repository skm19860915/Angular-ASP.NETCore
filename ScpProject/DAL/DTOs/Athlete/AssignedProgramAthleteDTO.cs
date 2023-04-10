using System;

namespace DAL.DTOs.Athlete
{
    public class AssignedProgramAthleteDTO
    {
        public int AthleteId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProgramName { get; set; }
        public string Thumbnail { get; set; }
        public string PictureBaseURL { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime AssignedDate { get; set; }

    }
}
