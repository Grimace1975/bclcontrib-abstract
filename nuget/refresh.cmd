@echo off
echo refreshing packages:

::
echo Abstract
pushd BclContrib-Abstract
set SRC=..\..\System.Abstract
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\System.Abstract.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\System.Abstract.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\System.Abstract.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\System.Abstract.xml lib\NET40\ /Y/Q
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
echo Abstract.SpringNet
pushd BclContrib-Abstract.SpringNet
set SRC=..\..\Contoso.Abstract.SpringNet
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.SpringNet.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.SpringNet.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.SpringNet.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.SpringNet.xml lib\NET40\ /Y/Q
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
echo Abstract.TypeSerializer
pushd BclContrib-Abstract.TypeSerializer
set SRC=..\..\Contoso.Abstract.TypeSerializer
xcopy %SRC%\Changelog.txt . /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.TypeSerializer.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.TypeSerializer.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.TypeSerializer.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.TypeSerializer.xml lib\NET40\ /Y/Q
popd

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

pause