set SRC=..\..\Contoso.Abstract.SpringNet
xcopy %SRC%\bin\Release\Contoso.Abstract.SpringNet.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.SpringNet.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.SpringNet.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.SpringNet.xml lib\NET40\ /Y/Q