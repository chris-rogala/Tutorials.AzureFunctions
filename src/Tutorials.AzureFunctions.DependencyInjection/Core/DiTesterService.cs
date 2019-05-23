using Microsoft.Extensions.Configuration;

namespace Tutorials.AzureFunctions.DependencyInjection.Core
{
    public interface IDiTesterService
    {
        dynamic Run(string name, string value);
    }

    public class DiTesterService : IDiTesterService
    {
        protected ISingletonLifetimeManagement SingletonLifetimeManagement { get; }
        protected ITransientLifetimeManagement TransientLifetimeManagement { get; }
        protected IScopedLifetimeManagement ScopedLifetimeManagement { get; }
        protected IConfiguration Configuration { get; }

        public DiTesterService(IConfiguration configuration,
            ISingletonLifetimeManagement singletonLifetimeManagement,
            IScopedLifetimeManagement scopedLifetimeManagement,
            ITransientLifetimeManagement transientLifetimeManagement)
        {
            Configuration = configuration;
            SingletonLifetimeManagement = singletonLifetimeManagement;
            ScopedLifetimeManagement = scopedLifetimeManagement;
            TransientLifetimeManagement = transientLifetimeManagement;
        }

        public dynamic Run(string name, string value)
        {
            ScopedLifetimeManagement.Add(value);
            SingletonLifetimeManagement.Add(value);
            TransientLifetimeManagement.Add(value);

            return new
            {
                config = Configuration[name],
                ScopedLifetimeManagement = ScopedLifetimeManagement.Get,
                SingletonLifetimeManagement = SingletonLifetimeManagement.Get,
                TransientLifetimeManagement = TransientLifetimeManagement.Get
            };
        }
    }
}
