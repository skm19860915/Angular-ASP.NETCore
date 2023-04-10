using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.User
{
    public class PasswordReset
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid PasswordResetToken { get; set; }
        public DateTime IssuedInUTC { get; set; }
        public DateTime ExpiresInUTC { get; set; }
        [ForeignKey("UserId")]
        public virtual User TargetUser { get; set; }
    }
}
