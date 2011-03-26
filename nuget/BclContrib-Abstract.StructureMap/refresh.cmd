set SRC=..\..\Contoso.Abstract.StructureMap
xcopy %SRC%\bin\Release\Contoso.Abstract.StructureMap.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.StructureMap.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.StructureMap.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.StructureMap.xml lib\NET40\ /Y/Q