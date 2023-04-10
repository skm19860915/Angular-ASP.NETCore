using System.ComponentModel.DataAnnotations.Schema;
using Extensions;

namespace Models.Enums
{
    public class NotificationType
    {
        public NotificationType() { }
        private NotificationType(NotificationTypeEnum @enum)
        {

            Id = (int)@enum;
            Name = @enum.ToString();
            Description = @enum.GetEnumDescription();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }


        public static implicit operator NotificationType(NotificationTypeEnum @enum) => new NotificationType(@enum);

        public static implicit operator NotificationTypeEnum(NotificationType notificationType) => (NotificationTypeEnum)notificationType.Id;
    }

    public enum NotificationTypeEnum
    {
        NewProgram = 1,
        SurveyThreshold = 2
    }
}
