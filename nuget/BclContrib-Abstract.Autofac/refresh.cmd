set SRC=..\..\Contoso.Abstract.Autofac
xcopy %SRC%\bin\Release\Contoso.Abstract.Autofac.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Autofac.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Autofac.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Autofac.xml lib\NET40\ /Y/Q