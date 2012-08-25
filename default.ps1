properties { 
  $base_dir = resolve-path .
  $packageinfo_dir = "$base_dir\packaging"
  $build_dir = "$build_dir\"
  $release_dir = "$base_dir\Release"
  $sln_file = "$base_dir\BCLEX-ABSTRACT.sln"
  $sln_test_file = "$base_dir\BCLEX-ABSTRACT.tests.sln"
  $tools_dir = "$base_dir\tools"
  $config = "Release"
  $run_tests = $true
}

$framework = '4.0'

#include .\psake_ext.ps1
	
task default -depends Release

task Clean {
	remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue
}

task Init -depends Clean {
	new-item $release_dir -itemType directory 
}

task Compile -depends Init {
	msbuild $sln_file /target:Rebuild /p:"Configuration=$config"
}

task Test -depends Compile -precondition { return $run_tests }{
	$old = pwd
	cd $build_dir
	& $tools_dir\xUnit\xunit.console.clr4.exe "$build_dir\3.5\BclEx.Tests.dll" /noshadow
	cd $old		
}

task Dependency {
	$package_files = @(Get-ChildItem src -include *packages.config -recurse)
	foreach ($package in $package_files)
	{
		& $tools_dir\NuGet.exe install $package.FullName -o packages
	}
}

task Release -depends Dependency, Compile, Test {
	cd $build_dir
	& $tools_dir\7za.exe a $release_dir\BclEx.zip `
		*\Castle.Core.dll `
    	..\license.txt
	if ($lastExitCode -ne 0) {
		throw "Error: Failed to execute ZIP command"
    }
}

task Package -depends Release {
	$spec_files = @(Get-ChildItem $packageinfo_dir)
	foreach ($spec in $spec_files)
	{
		& $tools_dir\NuGet.exe pack $spec.FullName -o $release_dir -Version $version -Symbols -BasePath $base_dir
	}
}
