using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Team
{
    class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }//todo: rename this createUserId
        public string Notes { get; set; }
        public int? ProgramId { get; set; }
        public DateTime? StartDate { get; set; }//todo:rename this to ProgramStartDate
        public DateTime? EndDate { get; set; }//todo:rename this to ProgramEndDate

        [ForeignKey("UserId")]
        public virtual User.User CreateUser { get; set; }


        //[ForeignKey("ProgramId")]
        //public virtual Program CurrentProgram { get; set; }
    }
}
