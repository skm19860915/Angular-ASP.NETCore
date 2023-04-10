[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Controllers.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Controllers.App_Start.NinjectWebCommon), "Stop")]

namespace Controllers.App_Start
{
    using System;
    using System.Web;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using DAL.Repositories;
    using System.Web.Configuration;
    using BL;
    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application.
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var connString = WebConfigurationManager.ConnectionStrings["scp"].ConnectionString;
            kernel.Bind<IExerciseRepo>().To<ExerciseRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IUserRepo>().To<UserRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IOrganizationRepo>().To<OrganizationRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind(typeof(ITagRepo<>)).To(typeof(TagRepo<>)).InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IExerciseManager>().To<ExerciseManager>().InSingletonScope();
            kernel.Bind(typeof(ITagManager<>)).To(typeof(TagManager<>)).InSingletonScope();
            kernel.Bind<IWeightRoomRepo>().To<WeightRoomRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IMessageRepo>().To<MessageRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IMessageManager>().To<MessageManager>();
            kernel.Bind<IMetricRepo>().To<MetricRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IMetricManager>().To<MetricManager>();
            kernel.Bind<IProgramRepo>().To<ProgramRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString); 
            kernel.Bind<IAthleteRepo>().To<AthleteRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IProgramManager>().To<ProgramManager>();
            kernel.Bind<IMultimediaRepo>().To<MultimediaRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IMultiMediaManager>().To<MultiMediaManager>().InSingletonScope().WithConstructorArgument("azureConnectionEndpoint", WebConfigurationManager.AppSettings.Get("storage:ConnectionEndpoint"));
            kernel.Bind<INotificationRepo>().To<NotificationRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<ISubscriptionRepo>().To<SubscriptionRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<INotificationManager>().To<NotificationManager>();
            kernel.Bind<IOrganizationManager>().To<OrganizationManager>();
            kernel.Bind<IRosterManager>().To<RosterManager>();
            kernel.Bind<IAdministrationRepo>().To<AdministrationRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IUserTokenRepo>().To<UserTokenRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IAthleteManager>().To<AthleteManager>();
            kernel.Bind<IUserManager>().To<UserManager>();
            kernel.Bind<ILogRepo>().To<LogRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<ISurveyRepo>().To<SurveyRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IWorkoutRepo>().To<WorkoutRepo>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<ISurveyManager>().To<SurveyManager>();
            kernel.Bind<IWorkoutManager>().To<WorkoutManager>();
            kernel.Bind<IMovieManager>().To<MovieManager>();
            kernel.Bind<IPlayerSnapShotRepository>().To<PlayerSnapShotRepository>().InSingletonScope().WithConstructorArgument("connectionString", connString);
            kernel.Bind<IDocumentRepository>().To<DocumentRepository>().InSingletonScope().WithConstructorArgument("connectionString", connString);


        }
    }
}