@echo off
setlocal enabledelayedexpansion

echo packing packages

set OUTPUTDIR=%~dp0.nupkg\
if not exist %OUTPUTDIR% mkdir %OUTPUTDIR%
set BUILD=-Build -Prop Configuration=Release

:: Services
for /r src\ %%x in (*.nuspec) do (
   set v=%%x
   set v=!v:.nuspec=.csproj!
   set v=!v:BclEx-Abstract.csproj=System.Abstract.csproj!
   set v=!v:BclEx-Abstract.=Contoso.Abstract.!
   set v=!v:BclEx-Practices.=Contoso.Practices.!
   ::echo.!v!
   tools\nuget pack "!v!" -NoDefaultExcludes -OutputDirectory %OUTPUTDIR% %BUILD% )
   ::pause )

echo pushing packages
pause
for %%x in (%OUTPUTDIR%*.nupkg) do tools\nuget push "%%x" -source http://nuget.degdarwin.com

endlocal
choice /N /D Y /T 5
