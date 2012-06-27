@echo off
echo pushing packages
for /r %%x in (*.nupkg) do ..\nuget push -source http://nuget.org/ "%%x"
pause