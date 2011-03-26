set SRC=..\..\System.Abstract
xcopy %SRC%\bin\Release\System.Abstract.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\System.Abstract.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\System.Abstract.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\System.Abstract.xml lib\NET40\ /Y/Q