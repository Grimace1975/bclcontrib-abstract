using System.Abstract;
using Contoso.Abstract;
using Contoso.Abstract.RhinoServiceBus;
namespace Example
{
    public class Runtime : ServiceLocatorBootStrapper
    {
        public override void InitializeContainer()
        {
            ServiceLocatorManager.SetProvider(() => new MicroServiceLocator());
            base.InitializeContainer();
        }
    }
}
