set SRC=..\..\Contoso.Abstract.Windsor
xcopy %SRC%\bin\Release\Contoso.Abstract.Windsor.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Windsor.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Windsor.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Windsor.xml lib\NET40\ /Y/Q