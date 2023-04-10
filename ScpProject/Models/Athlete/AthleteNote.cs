using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Athlete
{
   public  class AthleteNote
    {

        public int Id { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUserId { get; set; }
        public int AthleteId { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("AthleteId")]
        public virtual Athlete NoteOwner { get; set; }

        [ForeignKey("CreatedUserId")]
        public virtual User.User CreatedUser { get; set; }
    }
}
