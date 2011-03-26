set SRC=..\..\Contoso.Abstract.TypeSerializer
xcopy %SRC%\bin\Release\Contoso.Abstract.TypeSerializer.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.TypeSerializer.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.TypeSerializer.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.TypeSerializer.xml lib\NET40\ /Y/Q