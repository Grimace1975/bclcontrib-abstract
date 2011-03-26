set SRC=..\..\Contoso.Abstract.MongoDB
xcopy %SRC%\bin\Release\Contoso.Abstract.MongoDB.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.MongoDB.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MongoDB.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.MongoDB.xml lib\NET40\ /Y/Q