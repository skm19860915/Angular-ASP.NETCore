using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace Models.Notifications
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public Enums.NotificationType Type { get; set; }
        public int GeneratedAthleteId { get; set; }
        public bool HasBeenViewed { get; set; }
        public int DestinationUserId { get; set; }
        public DateTime SentDate { get; set; }

        [ForeignKey("GeneratedAthleteId")]
        public virtual Athlete.Athlete GeneratedAthlete { get; set; }
        [ForeignKey("DestinationUserId")]
        public virtual User.User DestinationUser { get; set; }
    }
}
