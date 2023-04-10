using Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Enums
{
    public class OrganizationRole
    {
        public OrganizationRole() { }
        private OrganizationRole(OrganizationRoleEnum @enum)
        {

            Id = (int)@enum;
            Name = @enum.ToString();
            Description = @enum.GetEnumDescription();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public static implicit operator OrganizationRole(OrganizationRoleEnum @enum) => new OrganizationRole(@enum);

        public static implicit operator OrganizationRoleEnum(OrganizationRole organizationRole) => (OrganizationRoleEnum)organizationRole.Id;
    }
    public enum OrganizationRoleEnum
    {
        Admin = 1,
        CreateExercises = 2,
        ModifyExercises = 3,
        ArchiveExercises = 4,
        CreateMetrics = 5,
        ModifyMetrics = 6,
        ArchiveMetrics = 7,
        CreateWorkouts = 8,
        ModifyWorkouts = 9,
        ArchiveWorkouts = 10,
        CreateSurveys = 11,
        ModifySurveys = 12,
        ArchiveSurveys = 13,
        CreatePrograms = 14,
        ModifyPrograms = 15,
        ArchivePrograms = 16,
        CreateAthletes = 17,
        ModifyAthletes = 18,
        ArchiveAthletes = 19,
        AssignPrograms = 20,
        PrintPrograms = 21,
        ViewAthletes = 22,
        WeightRoomView = 23,
        CreateMovies = 24,
        ModifyMovies = 25,
        ArchiveMovies = 26

    }
}
