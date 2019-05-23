using System.Collections.Generic;

namespace Tutorials.AzureFunctions.DependencyInjection.Core
{
    public interface ITestLifetimeManagement
    {
        IEnumerable<string> Get { get; }

        void Add(string value);
    }

    public abstract class LifetimeManagementBase : ITestLifetimeManagement
    {
        private List<string> _values = new List<string>();

        public void Add(string value)
        {
            _values.Add(value);
        }

        public IEnumerable<string> Get => _values;
    }

    public interface ISingletonLifetimeManagement : ITestLifetimeManagement
    { }

    public class SingletonLifetimeManagement : LifetimeManagementBase, ISingletonLifetimeManagement
    { }


    public interface IScopedLifetimeManagement : ITestLifetimeManagement
    { }

    public class ScopedLifetimeManagement : LifetimeManagementBase, IScopedLifetimeManagement
    { }

    public interface ITransientLifetimeManagement : ITestLifetimeManagement
    { }

    public class TransientLifetimeManagement : LifetimeManagementBase, ITransientLifetimeManagement
    { }
}
