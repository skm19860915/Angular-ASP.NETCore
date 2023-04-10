using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.User
{
    public class UserToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid Token { get; set; }
        [ForeignKey("UserId")]
        public virtual User TokenOwner { get; set; }
    }
}
