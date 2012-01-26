using System.Abstract;
using Contoso.Abstract;
using Contoso.Abstract.RhinoServiceBus;
namespace Example
{
    public class Bootstrap : BootstrapRhinoServiceBusHost
    {
        public override void Initialize()
        {
            ServiceLocatorManager.SetProvider(() => new MicroServiceLocator());
        }
    }
}
