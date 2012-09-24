@echo off
setlocal enabledelayedexpansion

echo packing packages

set OUTPUTDIR=%~dp0.nupkg\
if not exist %OUTPUTDIR% mkdir %OUTPUTDIR%
set BUILD=-Build -Prop Configuration=Release

:: Services
tools\nuget pack "C:\_APPLICATION\BCLEX-ABSTRACT\src\ServiceBuses\Contoso.Abstract.RhinoServiceBus\Contoso.Abstract.RhinoServiceBus.csproj" -NoDefaultExcludes -OutputDirectory %OUTPUTDIR% %BUILD%

pause

endlocal
choice /N /D Y /T 5
