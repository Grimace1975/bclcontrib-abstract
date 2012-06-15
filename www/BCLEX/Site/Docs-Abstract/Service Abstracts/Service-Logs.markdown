# Service Logs
> abstracts a [server log](http://en.wikipedia.org/wiki/Server_log "Server log"). this server log abstraction is implemented by various providers.




# Defining the Provider

## Registering a Service Log

as a singleton:

	ServiceLogManager.SetProvider(() => new Log4NetServiceLog());

## Consuming a Service Log

from a singleton:

	TBD

from an injected dependency:

	public MyClass(IServiceLog log)
	{
	   TBD
	}




# Working with IServiceLog