@echo off
echo refreshing packages:

::
echo Abstract
pushd BclContrib-Abstract
set SRC=..\..\System.Abstract
set SRC2=..\..\System.Abstract.Configuration
xcopy %SRC%\bin\Release\System.Abstract.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\System.Abstract.xml lib\net35\ /Y/Q
xcopy %SRC2%\bin\Release\System.Abstract.Configuration.dll lib\net35\ /Y/Q
xcopy %SRC2%\bin\Release\System.Abstract.Configuration.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\System.Abstract.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\System.Abstract.xml lib\net40\ /Y/Q
xcopy %SRC2%.4\bin\Release\System.Abstract.Configuration.dll lib\net40\ /Y/Q
xcopy %SRC2%.4\bin\Release\System.Abstract.Configuration.xml lib\net40\ /Y/Q
popd

::
echo Abstract.Autofac
pushd BclContrib-Abstract.Autofac
set SRC=..\..\Contoso.Abstract.Autofac
xcopy %SRC%\bin\Release\Contoso.Abstract.Autofac.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Autofac.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Autofac.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Autofac.xml lib\net40\ /Y/Q
popd

::
echo Abstract.CastleWindsor
pushd BclContrib-Abstract.CastleWindsor
set SRC=..\..\Contoso.Abstract.CastleWindsor
xcopy %SRC%\bin\Release\Contoso.Abstract.CastleWindsor.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.CastleWindsor.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.CastleWindsor.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.CastleWindsor.xml lib\net40\ /Y/Q
popd

::
echo Abstract.DurableBus
pushd BclContrib-Abstract.DurableBus
set SRC=..\..\Contoso.Practices.DurableBus
xcopy %SRC%\bin\Release\Contoso.Practices.DurableBus.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Practices.DurableBus.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Practices.DurableBus.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Practices.DurableBus.xml lib\net40\ /Y/Q
popd

::
echo Abstract.Cqrs
pushd BclContrib-Abstract.Cqrs
set SRC=..\..\Contoso.Practices.Cqrs
xcopy %SRC%\bin\Release\Contoso.Practices.Cqrs.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Practices.Cqrs.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Practices.Cqrs.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Practices.Cqrs.xml lib\net40\ /Y/Q
popd

::
echo Abstract.Hiro
pushd BclContrib-Abstract.Hiro
set SRC=..\..\Contoso.Abstract.Hiro
xcopy %SRC%\bin\Release\Contoso.Abstract.Hiro.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Hiro.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Hiro.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Hiro.xml lib\net40\ /Y/Q
popd

::
echo Abstract.Log4Net
pushd BclContrib-Abstract.Log4Net
set SRC=..\..\Contoso.Abstract.Log4Net
xcopy %SRC%\bin\Release\Contoso.Abstract.Log4Net.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Log4Net.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Log4Net.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Log4Net.xml lib\net40\ /Y/Q
popd

::
echo Abstract.Memcached
pushd BclContrib-Abstract.Memcached
set SRC=..\..\Contoso.Abstract.Memcached
xcopy %SRC%\bin\Release\Contoso.Abstract.Memcached.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Memcached.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Memcached.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Memcached.xml lib\net40\ /Y/Q
popd

::
echo Abstract.MongoDB
pushd BclContrib-Abstract.MongoDB
set SRC=..\..\Contoso.Abstract.MongoDB
xcopy %SRC%\bin\Release\Contoso.Abstract.MongoDB.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.MongoDB.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MongoDB.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MongoDB.xml lib\net40\ /Y/Q
popd

::
echo Abstract.MSMQ
pushd BclContrib-Abstract.MSMQ
set SRC=..\..\Contoso.Abstract.MSMQ
xcopy %SRC%\bin\Release\Contoso.Abstract.MSMQ.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.MSMQ.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MSMQ.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MSMQ.xml lib\net40\ /Y/Q
popd

::
echo Abstract.MSSql
pushd BclContrib-Abstract.MSSql
set SRC=..\..\Contoso.Abstract.MSSql
xcopy %SRC%\bin\Release\Contoso.Abstract.MSSql.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.MSSql.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MSSql.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MSSql.xml lib\net40\ /Y/Q
popd

::
echo Abstract.MvcTurbine
pushd BclContrib-Abstract.MvcTurbine
set SRC=..\..\Contoso.Abstract.MvcTurbine
xcopy %SRC%\bin\Release\Contoso.Abstract.MvcTurbine.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.MvcTurbine.xml lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\MvcTurbine.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\MvcTurbine.Web.dll lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MvcTurbine.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MvcTurbine.xml lib\net40\ /Y/Q
popd

::
echo Abstract.Ninject
pushd BclContrib-Abstract.Ninject
set SRC=..\..\Contoso.Abstract.Ninject
xcopy %SRC%\bin\Release\Contoso.Abstract.Ninject.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Ninject.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Ninject.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Ninject.xml lib\net40\ /Y/Q
popd

::
echo Abstract.NLog
pushd BclContrib-Abstract.NLog
set SRC=..\..\Contoso.Abstract.NLog
xcopy %SRC%\bin\Release\Contoso.Abstract.NLog.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.NLog.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NLog.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NLog.xml lib\net40\ /Y/Q
popd

::
echo Abstract.NServiceBus
pushd BclContrib-Abstract.NServiceBus
set SRC=..\..\Contoso.Abstract.NServiceBus
xcopy %SRC%\bin\Release\Contoso.Abstract.NServiceBus.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.NServiceBus.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NServiceBus.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NServiceBus.xml lib\net40\ /Y/Q
popd

::
echo Abstract.RabbitMQ
pushd BclContrib-Abstract.RabbitMQ
set SRC=..\..\Contoso.Abstract.RabbitMQ
xcopy %SRC%\bin\Release\Contoso.Abstract.RabbitMQ.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.RabbitMQ.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RabbitMQ.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RabbitMQ.xml lib\net40\ /Y/Q
popd

::
echo Abstract.RhinoServiceBus
pushd BclContrib-Abstract.RhinoServiceBus
set SRC=..\..\Contoso.Abstract.RhinoServiceBus
set LIB=..\..\lib
xcopy %SRC%\bin\Release\Contoso.Abstract.RhinoServiceBus.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.RhinoServiceBus.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RhinoServiceBus.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RhinoServiceBus.xml lib\net40\ /Y/Q
::
xcopy %SRC%\bin\Release\Rhino.* lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Rhino.* lib\net40\ /Y/Q
xcopy %LIB%\Rhino.ServiceBus\net40\* lib\net40\ /Y/Q/S
popd

::
echo Abstract.ServerAppFabric
pushd BclContrib-Abstract.ServerAppFabric
set SRC=..\..\Contoso.Abstract.ServerAppFabric
xcopy %SRC%\bin\Release\Contoso.Abstract.ServerAppFabric.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.ServerAppFabric.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.ServerAppFabric.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.ServerAppFabric.xml lib\net40\ /Y/Q
popd

::
echo Abstract.Spring
pushd BclContrib-Abstract.Spring
set SRC=..\..\Contoso.Abstract.Spring
xcopy %SRC%\bin\Release\Contoso.Abstract.Spring.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Spring.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Spring.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Spring.xml lib\net40\ /Y/Q
popd

::
echo Abstract.StructureMap
pushd BclContrib-Abstract.StructureMap
set SRC=..\..\Contoso.Abstract.StructureMap
xcopy %SRC%\bin\Release\Contoso.Abstract.StructureMap.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.StructureMap.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.StructureMap.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.StructureMap.xml lib\net40\ /Y/Q
popd

::
echo Abstract.Unity
pushd BclContrib-Abstract.Unity
set SRC=..\..\Contoso.Abstract.Unity
xcopy %SRC%\bin\Release\Contoso.Abstract.Unity.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Unity.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Unity.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Unity.xml lib\net40\ /Y/Q
popd

::
echo Abstract.Web
pushd BclContrib-Abstract.Web
set SRC=..\..\Contoso.Abstract.Web
xcopy %SRC%\bin\Release\Contoso.Abstract.Web.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Web.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Web.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Web.xml lib\net40\ /Y/Q
popd

::
echo Abstract.Web.Mvc
pushd BclContrib-Abstract.Web.Mvc
set SRC=..\..\Contoso.Abstract.Web.Mvc
xcopy %SRC%\bin\Release\Contoso.Abstract.Web.Mvc.dll lib\net35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Web.Mvc.xml lib\net35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Web.Mvc.dll lib\net40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Web.Mvc.xml lib\net40\ /Y/Q
popd

::
echo Hiro
pushd Hiro
set SRC=..\..\lib\Hiro
xcopy %SRC%\* lib\ /Y/Q
popd

::
echo ServerAppFabric.Client
pushd ServerAppFabric.Client
set SRC=..\..\lib\ServerAppFabric.Client
xcopy %SRC%\* lib\ /Y/Q
popd

::pause