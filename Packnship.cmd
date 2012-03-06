@echo off
setlocal enabledelayedexpansion

echo packing packages

set OUTPUTDIR=%~dp0dagent\
if not exist %OUTPUTDIR% mkdir %OUTPUTDIR%
set BUILD=-Prop Configuration=Release

:: Services
for /r src %%x in (*.nuspec) do (
   set v=%%x
   set v=!v:.nuspec=.csproj!
   set v=!v:BclEx-Abstract.csproj=System.Abstract.csproj!
   set v=!v:BclEx-Abstract.=Contoso.Abstract.!
   set v=!v:BclEx-Practices.=Contoso.Practices.!
   ::echo.!v!
   nuget pack "!v!" -OutputDirectory %OUTPUTDIR% %BUILD% )
   ::pause )


echo pushing packages
::for %%x in (%OUTPUTDIR%*.nupkg) do nuget push "%%x" -source http://nuget.degdarwin.com

endlocal
choice /N /D Y /T 5
