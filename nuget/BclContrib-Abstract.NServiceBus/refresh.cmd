set SRC=..\..\Contoso.Abstract.NServiceBus
xcopy %SRC%\bin\Release\Contoso.Abstract.NServiceBus.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.NServiceBus.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NServiceBus.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.NServiceBus.xml lib\NET40\ /Y/Q