namespace Models.Migrations
{
    using System.Data.Entity.Migrations;
    using Extensions;
    using Models.Enums;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.StrenContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            CommandTimeout = 10000; // migration timeout
        }

        protected override void Seed(Models.StrenContext context)
        {
            context.QuestionTypes.SeedEnumValues<QuestionType, QuestionTypeEnum>(@enum => @enum);
            context.MediaTypes.SeedEnumValues<MediaType, MediaTypeEnum>(@enum => @enum);
            context.ProgramDayItemTypes.SeedEnumValues<ProgramDayItemType, ProgramDayItemEnum>(@enum => @enum);
            context.OrganizationRoles.SeedEnumValues<OrganizationRole, OrganizationRoleEnum>(@enum => @enum);
            context.NotificationTypes.SeedEnumValues<NotificationType, NotificationTypeEnum>(@enum => @enum);
            context.QuestionThresholds.SeedEnumValues<QuestionThreshold, QuestionThresholdEnum>(@enum => @enum);
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
