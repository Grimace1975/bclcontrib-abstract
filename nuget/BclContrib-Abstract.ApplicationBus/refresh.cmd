set SRC=..\..\Contoso.Abstract.ApplicationBus
xcopy %SRC%\bin\Release\Contoso.Abstract.ApplicationBus.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.ApplicationBus.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.ApplicationBus.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.ApplicationBus.xml lib\NET40\ /Y/Q