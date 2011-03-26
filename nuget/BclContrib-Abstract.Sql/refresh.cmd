set SRC=..\..\Contoso.Abstract.Sql
xcopy %SRC%\bin\Release\Contoso.Abstract.Sql.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Sql.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Sql.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Sql.xml lib\NET40\ /Y/Q