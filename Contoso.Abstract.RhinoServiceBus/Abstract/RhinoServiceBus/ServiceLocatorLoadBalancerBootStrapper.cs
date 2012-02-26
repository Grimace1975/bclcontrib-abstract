using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.LoadBalancer;
namespace Contoso.Abstract.RhinoServiceBus
{
    public class ServiceLocatorLoadBalancerBootStrapper : ServiceLocatorBootStrapper
    {
        protected ServiceLocatorLoadBalancerBootStrapper() { }
        protected ServiceLocatorLoadBalancerBootStrapper(System.Abstract.IServiceLocator locator)
            : base(locator) { }

        protected override AbstractRhinoServiceBusConfiguration CreateConfiguration()
        {
            return new LoadBalancerConfiguration();
        }
    }
}