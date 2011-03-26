@echo off
echo refreshing packages:
for /d %%x in ("BclContrib-*") do (
   pushd %%x
   echo %%x
   call refresh.cmd
   popd)
pause