set SRC=..\..\Contoso.Abstract.Ninject
xcopy %SRC%\bin\Release\Contoso.Abstract.Ninject.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Ninject.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Ninject.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Ninject.xml lib\NET40\ /Y/Q