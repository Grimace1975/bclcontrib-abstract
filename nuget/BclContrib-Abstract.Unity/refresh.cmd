set SRC=..\..\Contoso.Abstract.Unity
xcopy %SRC%\bin\Release\Contoso.Abstract.Unity.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Unity.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Unity.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Unity.xml lib\NET40\ /Y/Q