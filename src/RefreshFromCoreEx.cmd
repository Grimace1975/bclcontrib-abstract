@echo off
echo refreshing packages:

::
echo FromCoreEx
pushd +FromCoreEx
set SRC=..\..\..\BCLEX-EXTEND\src\System.CoreEx
xcopy %SRC%\+Kludge\Collections\HashHelpers.cs +Kludge\Collections\ /Y/Q
xcopy %SRC%\+Kludge\Runtime\CompilerServices\ConditionalWeakTable.cs +Kludge\Runtime\CompilerServices\ /Y/Q
xcopy %SRC%\+Kludge\Runtime\CompilerServices\DependentHandle.cs +Kludge\Runtime\CompilerServices\ /Y/Q
xcopy %SRC%\Collections\IIndexer.cs Collections\ /Y/Q
xcopy %SRC%\IO\WrapTextReader.cs IO\ /Y/Q
xcopy %SRC%\Linq\Expressions\ExpressionEx.cs Linq\Expressions\ /Y/Q
xcopy %SRC%\Reflection\AssemblyExtensions.cs Reflection\ /Y/Q
xcopy %SRC%\CoreExtensions+Lazy.cs .\ /Y/Q
xcopy %SRC%\EnvironmentEx2.cs .\ /Y/Q
xcopy %SRC%\ExceptionEx.cs .\ /Y/Q

popd

pause