using System;
using System.Data.Entity;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using FluentValidation;
using FluentValidation.Mvc;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Web.Common;
using Ninject.Web.Mvc.FilterBindingSyntax;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Infrastructure.Ioc.Ninject.Ninject.Web.Mvc.FluentValidation;
using Sfw.Sabp.Mca.Infrastructure.Web.Attributes;
using Sfw.Sabp.Mca.Web;
using Sfw.Sabp.Mca.Web.Attributes;
using Sfw.Sabp.Mca.Web.Attributes.MetaData;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]

namespace Sfw.Sabp.Mca.Web
{
    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));

            Bootstrapper.Initialize(CreateKernel);          
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {   
            Bootstrapper.ShutDown();
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

                var ninjectValidatorFactory = new NinjectValidatorFactory(kernel);
                FluentValidationModelValidatorProvider.Configure(x => x.ValidatorFactory = ninjectValidatorFactory);
                DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

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
            kernel.Bind(scanner => scanner.FromAssembliesMatching("Sfw.Sabp.Mca.*")
                .SelectAllClasses().NotInNamespaces(new[] { "Sfw.Sabp.Mca.Web.ViewModels.Validation"})
                .BindAllInterfaces());

            kernel.Bind<DbContext>().To<DataAccess.Ef.Mca>().InRequestScope();

            kernel.Rebind<IUnitOfWork>().To<UnitOfWork>()
                .InRequestScope()
                .WithConstructorArgument(new DataAccess.Ef.Mca());

            kernel.BindFilter<AuthorizeMcaUsersAttribute>(FilterScope.Global, 0);
            kernel.BindFilter<ErrorLoggerFilter>(FilterScope.Global, 0);

            kernel.BindFilter<AuditFilterAttribute>(FilterScope.Action, 0).WhenActionMethodHas<AuditAttribute>()
                .WithConstructorArgumentFromActionAttribute<AuditAttribute>("auditProperties", x => x.AuditProperties);

            kernel.BindFilter<AssessmentInProgressActionFilter>(FilterScope.Action, 0)
                .WhenActionMethodHas<AssessmentInProgressAttribute>()
                .WithConstructorArgumentFromActionAttribute<AssessmentInProgressAttribute>("actionParameterId", x => x.ActionParameterId);

            kernel.BindFilter<AssessmentCompleteActionFilter>(FilterScope.Action, 0)
                .WhenActionMethodHas<AssessmentCompleteAttribute>()
                .WithConstructorArgumentFromActionAttribute<AssessmentCompleteAttribute>("actionParameterId", x => x.ActionParameterId);

            kernel.BindFilter<AgreedToDisclaimerAuthorizeAttribute>(FilterScope.Controller, 0).WhenControllerHas<AgreedToDisclaimerAuthorizeAttributeNinject>();

            kernel.BindFilter<AuthorizeAdministratorAttribute>(FilterScope.Action, 0).WhenActionMethodHas<AuthorizeAdministratorAttributeNinject>();

            kernel.Rebind<ModelMetadataProvider>().To<CustomModelMetadataProvider>().InSingletonScope();

            AssemblyScanner.FindValidatorsInAssembly(Assembly.GetExecutingAssembly())
                .ForEach(match => kernel.Bind(match.InterfaceType).To(match.ValidatorType));    
        }        
    }
}
