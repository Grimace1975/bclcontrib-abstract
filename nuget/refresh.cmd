@echo off
echo refreshing packages:

::
echo Abstract
pushd BclContrib-Abstract
set SRC=..\..\System.Abstract
set SRC2=..\..\System.Abstract.Configuration
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\System.Abstract.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\System.Abstract.xml lib\NET35\ /Y/Q
xcopy %SRC2%\bin\Release\System.Abstract.Configuration.dll lib\NET35\ /Y/Q
xcopy %SRC2%\bin\Release\System.Abstract.Configuration.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\System.Abstract.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\System.Abstract.xml lib\NET40\ /Y/Q
xcopy %SRC2%.4\bin\Release\System.Abstract.Configuration.dll lib\NET40\ /Y/Q
xcopy %SRC2%.4\bin\Release\System.Abstract.Configuration.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.ApplicationBus
pushd BclContrib-Abstract.ApplicationBus
set SRC=..\..\Contoso.Abstract.ApplicationBus
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.ApplicationBus.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.ApplicationBus.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.ApplicationBus.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.ApplicationBus.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.Autofac
pushd BclContrib-Abstract.Autofac
set SRC=..\..\Contoso.Abstract.Autofac
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Autofac.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Autofac.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Autofac.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Autofac.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.CastleWindsor
pushd BclContrib-Abstract.CastleWindsor
set SRC=..\..\Contoso.Abstract.CastleWindsor
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.CastleWindsor.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.CastleWindsor.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.CastleWindsor.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.CastleWindsor.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.DurableBus
pushd BclContrib-Abstract.DurableBus
set SRC=..\..\Contoso.Practices.DurableBus
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Practices.DurableBus.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Practices.DurableBus.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Practices.DurableBus.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Practices.DurableBus.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.Cqrs
pushd BclContrib-Abstract.Cqrs
set SRC=..\..\Contoso.Practices.Cqrs
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Practices.Cqrs.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Practices.Cqrs.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Practices.Cqrs.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Practices.Cqrs.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.Hiro
pushd BclContrib-Abstract.Hiro
set SRC=..\..\Contoso.Abstract.Hiro
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Hiro.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Hiro.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Hiro.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Hiro.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.Log4Net
pushd BclContrib-Abstract.Log4Net
set SRC=..\..\Contoso.Abstract.Log4Net
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Log4Net.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Log4Net.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Log4Net.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Log4Net.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.Memcached
pushd BclContrib-Abstract.Memcached
set SRC=..\..\Contoso.Abstract.Memcached
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Memcached.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Memcached.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Memcached.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Memcached.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.MongoDB
pushd BclContrib-Abstract.MongoDB
set SRC=..\..\Contoso.Abstract.MongoDB
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.MongoDB.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.MongoDB.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MongoDB.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MongoDB.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.Msmq
pushd BclContrib-Abstract.Msmq
set SRC=..\..\Contoso.Abstract.Msmq
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Msmq.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Msmq.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Msmq.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Msmq.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.MvcTurbine
pushd BclContrib-Abstract.MvcTurbine
set SRC=..\..\Contoso.Abstract.MvcTurbine
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.MvcTurbine.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.MvcTurbine.xml lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\MvcTurbine.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\MvcTurbine.Web.dll lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MvcTurbine.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MvcTurbine.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.Ninject
pushd BclContrib-Abstract.Ninject
set SRC=..\..\Contoso.Abstract.Ninject
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Ninject.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Ninject.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Ninject.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Ninject.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.NLog
pushd BclContrib-Abstract.NLog
set SRC=..\..\Contoso.Abstract.NLog
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.NLog.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.NLog.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NLog.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NLog.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.NServiceBus
pushd BclContrib-Abstract.NServiceBus
set SRC=..\..\Contoso.Abstract.NServiceBus
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.NServiceBus.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.NServiceBus.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NServiceBus.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NServiceBus.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.RabbitMQ
pushd BclContrib-Abstract.RabbitMQ
set SRC=..\..\Contoso.Abstract.RabbitMQ
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.RabbitMQ.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.RabbitMQ.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RabbitMQ.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RabbitMQ.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.RhinoServiceBus
pushd BclContrib-Abstract.RhinoServiceBus
set SRC=..\..\Contoso.Abstract.RhinoServiceBus
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.RhinoServiceBus.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.RhinoServiceBus.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RhinoServiceBus.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RhinoServiceBus.xml lib\NET40\ /Y/Q
::
xcopy %SRC%\bin\Release\Rhino.* lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Rhino.* lib\NET40\ /Y/Q
popd

::
echo Abstract.ServerAppFabric
pushd BclContrib-Abstract.ServerAppFabric
set SRC=..\..\Contoso.Abstract.ServerAppFabric
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.ServerAppFabric.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.ServerAppFabric.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.ServerAppFabric.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.ServerAppFabric.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.Spring
pushd BclContrib-Abstract.Spring
set SRC=..\..\Contoso.Abstract.Spring
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Spring.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Spring.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Spring.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Spring.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.Sql
pushd BclContrib-Abstract.Sql
set SRC=..\..\Contoso.Abstract.Sql
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Sql.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Sql.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Sql.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Sql.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.StructureMap
pushd BclContrib-Abstract.StructureMap
set SRC=..\..\Contoso.Abstract.StructureMap
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.StructureMap.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.StructureMap.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.StructureMap.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.StructureMap.xml lib\NET40\ /Y/Q
popd

::
::echo Abstract.TypeSerializer
::pushd BclContrib-Abstract.TypeSerializer
::set SRC=..\..\Contoso.Abstract.TypeSerializer
::xcopy %SRC%\Changelog.txt . /Y/Q
::xcopy %SRC%\bin\Release\Contoso.Abstract.TypeSerializer.dll lib\NET35\ /Y/Q
::xcopy %SRC%\bin\Release\Contoso.Abstract.TypeSerializer.xml lib\NET35\ /Y/Q
::xcopy %SRC%.4\bin\Release\Contoso.Abstract.TypeSerializer.dll lib\NET40\ /Y/Q
::xcopy %SRC%.4\bin\Release\Contoso.Abstract.TypeSerializer.xml lib\NET40\ /Y/Q
::popd

::
echo Abstract.Unity
pushd BclContrib-Abstract.Unity
set SRC=..\..\Contoso.Abstract.Unity
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Unity.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Unity.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Unity.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Unity.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.Web
pushd BclContrib-Abstract.Web
set SRC=..\..\Contoso.Abstract.Web
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Web.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Web.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Web.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Web.xml lib\NET40\ /Y/Q
popd

::
echo Abstract.Web.Mvc
pushd BclContrib-Abstract.Web.Mvc
set SRC=..\..\Contoso.Abstract.Web.Mvc
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Web.Mvc.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Web.Mvc.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Web.Mvc.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Web.Mvc.xml lib\NET40\ /Y/Q
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