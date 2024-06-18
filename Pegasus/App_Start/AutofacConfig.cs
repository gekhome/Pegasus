using Autofac;
using Autofac.Integration.Mvc;
using Pegasus.DAL;
using Pegasus.Services;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Pegasus
{
    public class AutofacConfig
    {
        public static void RegisterComponents()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterSource(new ViewRegistrationSource());

            builder.RegisterType<PegasusDBEntities>().AsSelf();

            //builder.RegisterType<UserAdminService>().As<IUserAdminService>().WithParameter("entities", new PegasusDBEntities());

            //builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            //    .Where(t => t.Name.EndsWith("Service") && t.Namespace.Contains("Services"))
            //    .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Service") && t.Namespace.Contains("Services"))
                .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name));
                
                // This line breaks the services
                //.WithParameter("entities", new PegasusDBEntities());

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}