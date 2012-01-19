using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.LoadBalancer;
namespace Contoso.Abstract.RhinoServiceBus
{
    public class ServiceLocatorLoadBalancerBootStrapper : ServiceLocatorBootStrapper
    {
        protected override AbstractRhinoServiceBusConfiguration CreateConfiguration()
        {
            return new LoadBalancerConfiguration();
        }
    }
}