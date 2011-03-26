@echo off
echo refreshing packages:

::
pushd BclContrib-Abstract
set SRC=..\..\System.Abstract
xcopy %SRC%\bin\Release\System.Abstract.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\System.Abstract.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\System.Abstract.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\System.Abstract.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.ApplicationBus
set SRC=..\..\Contoso.Abstract.ApplicationBus
xcopy %SRC%\bin\Release\Contoso.Abstract.ApplicationBus.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.ApplicationBus.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.ApplicationBus.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.ApplicationBus.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.Autofac
set SRC=..\..\Contoso.Abstract.Autofac
xcopy %SRC%\bin\Release\Contoso.Abstract.Autofac.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Autofac.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Autofac.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Autofac.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.MongoDB
set SRC=..\..\Contoso.Abstract.MongoDB
xcopy %SRC%\bin\Release\Contoso.Abstract.MongoDB.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.MongoDB.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MongoDB.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MongoDB.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.Msmq
set SRC=..\..\Contoso.Abstract.Msmq
xcopy %SRC%\bin\Release\Contoso.Abstract.Msmq.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Msmq.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Msmq.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Msmq.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.Niject
set SRC=..\..\Contoso.Abstract.Ninject
xcopy %SRC%\bin\Release\Contoso.Abstract.Ninject.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Ninject.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Ninject.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Ninject.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.NServiceBus
set SRC=..\..\Contoso.Abstract.NServiceBus
xcopy %SRC%\bin\Release\Contoso.Abstract.NServiceBus.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.NServiceBus.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NServiceBus.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NServiceBus.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.RabbitMQ
set SRC=..\..\Contoso.Abstract.RabbitMQ
xcopy %SRC%\bin\Release\Contoso.Abstract.RabbitMQ.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.RabbitMQ.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RabbitMQ.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RabbitMQ.xml lib\NET40\ /Y/Q
popd

;:
pushd BclContrib-Abstract.SpringNet
set SRC=..\..\Contoso.Abstract.SpringNet
xcopy %SRC%\bin\Release\Contoso.Abstract.SpringNet.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.SpringNet.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.SpringNet.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.SpringNet.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.Sql
set SRC=..\..\Contoso.Abstract.Sql
xcopy %SRC%\bin\Release\Contoso.Abstract.Sql.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Sql.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Sql.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Sql.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.StructureMap
set SRC=..\..\Contoso.Abstract.StructureMap
xcopy %SRC%\bin\Release\Contoso.Abstract.StructureMap.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.StructureMap.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.StructureMap.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.StructureMap.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.TypeSerializer
set SRC=..\..\Contoso.Abstract.TypeSerializer
xcopy %SRC%\bin\Release\Contoso.Abstract.TypeSerializer.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.TypeSerializer.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.TypeSerializer.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.TypeSerializer.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.Unity
set SRC=..\..\Contoso.Abstract.Unity
xcopy %SRC%\bin\Release\Contoso.Abstract.Unity.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Unity.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Unity.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Unity.xml lib\NET40\ /Y/Q
popd

::
pushd BclContrib-Abstract.Windsor
set SRC=..\..\Contoso.Abstract.Windsor
xcopy %SRC%\bin\Release\Contoso.Abstract.Windsor.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Windsor.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Windsor.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Windsor.xml lib\NET40\ /Y/Q
popd

pause