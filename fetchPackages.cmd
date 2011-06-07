@echo off
for /r %%x in (*packages.config) do nuget\nuget install "%%x" -o Packages
nuget\nuget install MvcTurbine -version 2.1


