using Contoso.Abstract;
using System.Abstract;
namespace Example
{
    public class BootStrapper : ServiceBusBootStrapper
    {
        public override void InitializeContainer()
        {
            ServiceLocatorManager.SetProvider(() => new MicroServiceLocator());
            base.InitializeContainer();
        }
    }
}
