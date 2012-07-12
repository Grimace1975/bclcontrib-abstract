@echo off
for /r %%x in (*packages.config) do ( 
echo .
echo %%x:
tools\nuget install "%%x" -o packages )

:: MvcTurbine2.1
pushd packages
..\tools\nuget install MvcTurbine -version 2.1
popd
pause