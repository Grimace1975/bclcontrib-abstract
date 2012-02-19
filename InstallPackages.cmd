@echo off
for /r %%x in (*packages.config) do ( 
echo .
echo %%x:
nuget install "%%x" -o packages )

:: MvcTurbine2.1
pushd packages
..\nuget install MvcTurbine -o packages -version 2.1
popd
pause