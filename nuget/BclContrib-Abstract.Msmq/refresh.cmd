set SRC=..\..\Contoso.Abstract.Msmq
xcopy %SRC%\bin\Release\Contoso.Abstract.Msmq.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.Msmq.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Msmq.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.Msmq.xml lib\NET40\ /Y/Q