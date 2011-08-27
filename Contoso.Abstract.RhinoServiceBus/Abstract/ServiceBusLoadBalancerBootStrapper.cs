using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.LoadBalancer;
namespace Contoso.Abstract
{
    public sealed class ServiceBusLoadBalancerBootStrapper : ServiceBusBootStrapper
    {
        protected override AbstractRhinoServiceBusConfiguration CreateConfiguration()
        {
            return new LoadBalancerConfiguration();
        }
    }
}