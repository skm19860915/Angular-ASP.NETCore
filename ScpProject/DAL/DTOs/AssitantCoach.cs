using System.Collections.Generic;

namespace DAL.DTOs
{
    public class AssitantCoach
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Role> Roles { get; set; }
        public bool IsDeleted { get; set; }

    }
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class RoleDTO
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
    }
}


