@echo off
for /r %%x in (*packages.config) do nuget install "%%x" -o Packages
:: MvcTurbine2.1
pushd Packages
..\nuget install MvcTurbine -version 2.1
popd

pause