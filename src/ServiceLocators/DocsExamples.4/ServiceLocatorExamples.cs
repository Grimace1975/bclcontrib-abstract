using System.Abstract;
using Contoso.Abstract;
namespace Example
{
    public interface IMyService { }
    public class MyService : IMyService
    {
        public string Value { get; set; }
    }

    public class ServiceLocatorExamples
    {
        private void Main()
        {
            ServiceLocatorManager.SetProvider(() => new MicroServiceLocator())
                .RegisterByIServiceRegistration()
                .RegisterByNamingConvention();
        }

        public void MyClass(IServiceLocator locator)
        {
            var myService = locator.Resolve<IMyService>();
        }

        public void MyClass(IServiceRegistrar registrar)
        {
            // register as a type mapping
            registrar.Register<IMyService, MyService>();
            // register as a single instance
            registrar.RegisterInstance<IMyService>(new MyService { Value = "Value" });
            // register as a delegate
            registrar.Register<IMyService>(locator => new MyService { Value = "Value" });
        }
    }
}
