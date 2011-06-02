@echo off
echo refreshing packages:

::
echo FromCoreEx
pushd +FromCoreEx
set SRC=..\..\BCLCONTRIB-EXTEND\System.CoreEx
xcopy %SRC%\+Kludge\Collections\HashHelpers.cs +Kludge\Collections\ /Y/Q
xcopy %SRC%\+Kludge\Runtime\CompilerServices\ConditionalWeakTable.cs +Kludge\Runtime\CompilerServices\ /Y/Q
xcopy %SRC%\+Kludge\Runtime\CompilerServices\DependentHandle.cs +Kludge\Runtime\CompilerServices\ /Y/Q
xcopy %SRC%\Collections\IIndexer.cs Collections\ /Y/Q
xcopy %SRC%\Configuration\ConfigurationElementEx.cs Configuration\ /Y/Q
xcopy %SRC%\Configuration\ConfigurationElementsEx.cs Configuration\ /Y/Q
xcopy %SRC%\Configuration\ConfigurationSectionEx.cs Configuration\ /Y/Q
xcopy %SRC%\Linq\Expressions\ExpressionEx.cs Linq\Expressions\ /Y/Q
xcopy %SRC%\Reflection\AssemblyExtensions.cs Reflection\ /Y/Q
xcopy %SRC%\ExceptionEx.cs .\ /Y/Q
xcopy %SRC%\EnvironmentEx.cs .\ /Y/Q
xcopy %SRC%\LazyExtensions.cs .\ /Y/Q
popd


pause