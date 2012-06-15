# Service Locators
> abstracts a [service locator pattern](http://en.wikipedia.org/wiki/Service_locator_pattern "Service locator pattern") along the lines of [Inversion of Control](http://en.wikipedia.org/wiki/Inversion_of_control "Inversion_of_control") (IoC). this service locator abstraction is implemented by various providers.




# Defining the Provider

## Registering a Service Locator

as a singleton:

	ServiceLocatorManager.SetProvider(() => new UnityServiceLocator());

from an assembly type scan during creation:

	ServiceLocatorManager.SetProvider(() => new UnityServiceLocator())
	   .RegisterByNamingConvention();

place registration in a foreign service locator:

	var foreignLocator = { Foreign-Locator };
	ServiceLocatorManager.SetProvider(() => new UnityServiceLocator())
	   .RegisterByNamingConvention()
	   .RegisterWithServiceLocator(foreignLocator);

from an injected dependency:

	public MyClass(IServiceRegistrar registrar)
	{
	   // register as a type mapping
	   registrar.Register<IMyService, MyService>();
	   // register as a single instance
	   registrar.RegisterInstance<IMyService>(new MyService { Value = "Value" });
	   // register as a delegate
	   registrar.Register<IMyService>(locator => new MyService { Value = "Value" });
	}

## Consuming a Service Locator 

from a singleton:

	var myService = ServiceLocator.Resolve<IMyService>();

from an injected dependency:

	public MyClass(IServiceLocator locator)
	{
	   var myService = locator.Resolve<IMyService>();
	}




# Working with IServiceLocator