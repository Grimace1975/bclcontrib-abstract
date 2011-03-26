@echo off
echo pushing packages
for /r %%x in (*.nupkg) do nuget push -source http://packages.nuget.org/v1/ "%%x"
pause