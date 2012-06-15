# Ex-Abstract Overview

BclEx-Abstract is a class library that makes it easy to work with common services typicality re-implemented in various
projects/assemblies. BclEx-Abstract strives to provide standard interfaces for these services. Utilizing a standards based
type allows services to flow between otherwise disconnected assemblies.

Coherence is achieved by a reduction of a service's members to only those base enough to be common across multiple
service implementations. Implementation specific extensions to the base interfaces enable access to the uncommon methods.

A simple example of this would be the COM implementation of the IUnknown interface, containing the AddRef, QueryInterface
and Release methods. IUnknown provides a common type that all COM systems are aware of, allowing COM services to flow
between various COM aware systems. A COM system would then be queried using the QueryInterface method to get the specific
implementation requested. Of course with .Net, simple casting is all that is necessary to move from a common IServiceLocator
to a Unity specific IUnityServiceLocator.

For greater adaptation and ease of use, implementations for some of the more common abstracted systems are included as
multiple assemblies to optionally chose from. 

The library can be thought of as logically divided into the following four segments, and provided as [NuGet Packages](/site/docs-abstract/start-here/nuget-packages).

## System Type Kludges and Extensions
more information about [Kludges and Extensions](/site/docs-abstract/start-here/system-type-kludges-and-extensions).

Kludges provide backwards compatibility of depend .net types, and Extensions add types to the base class library. These are primarily used to support
the library.

## System Type Mocks
more information about [Mocks](/site/docs-abstract/start-here/system-type-mocks).

Some system types are implemented as static properties like DateTime.Now. It make unit testing difficult when such properties are dependencies because
of its time-sensitive behavior. BclEx-Abstract provides mocks for some of these types alowing better unit testing of there behaviors.

## Nparams Keyword
more information about [Nparams](/site/docs-abstract/start-here/nparams-keyword).

Nparams is a set of types and its manager to extend the behavior of the params keyword. It an optional key/value collection initialized by anonmous
object syntax and can be used for optional structured object parameter passing. Its also designed to not require null checking for most common uses.

## Service Managers
more information about [Service Managers](/site/docs-abstract/start-here/service-managers).

Service managers provide the bulk of the library. They are standard interfaces to each type of service, along with implementation specific interfaces for non-standard operations. They are lazily loaded and support both singleton and instanced uses.