set SRC=..\..\Contoso.Abstract.RabbitMQ
xcopy %SRC%\bin\Release\Contoso.Abstract.RabbitMQ.dll lib\NET35\ /Y/Q
xcopy %SRC%\bin\Release\Contoso.Abstract.RabbitMQ.xml lib\NET35\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RabbitMQ.dll lib\NET40\ /Y/Q
xcopy %SRC%.4\bin\Release\Contoso.Abstract.RabbitMQ.xml lib\NET40\ /Y/Q