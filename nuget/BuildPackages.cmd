@echo off
echo building packages:
mkdir packages
for /r %%x in (*.nuspec) do ..\tools\nuget pack "%%x" -o packages\
pause