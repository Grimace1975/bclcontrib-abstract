properties { 
  $base_dir = resolve-path .
  $build_dir = "$base_dir\build"
  $packageinfo_dir = "$base_dir\packaging"
  $40_build_dir = "$build_dir\4.0\"
  $35_build_dir = "$build_dir\3.5\"
  $lib_dir = "$base_dir\SharedLibs"
  $35_lib_dir = "$base_dir\SharedLibs\3.5\"
  $release_dir = "$base_dir\Release"
  $sln_file = "$base_dir\BCLEX-ABSTRACT.sln"
  $version = Get-Version-From-Git-Tag
  $tools_dir = "$base_dir\tools"
  $config = "Release"
  $run_tests = $true
}

$framework = '4.0'

include .\psake_ext.ps1
	
task default -depends Package

task Clean {
  remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue
  remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue
}

task Init -depends Clean {
	#$infos = (
	#	"$base_dir\Rhino.ServiceBus\Properties\AssemblyInfo.cs",
	#	"$base_dir\Rhino.ServiceBus.Tests\Properties\AssemblyInfo.cs",
	#	"$base_dir\Rhino.ServiceBus.Host\Properties\AssemblyInfo.cs",
	#	"$base_dir\Rhino.ServiceBus.Castle\Properties\AssemblyInfo.cs",
	#	"$base_dir\Rhino.ServiceBus.StructureMap\Properties\AssemblyInfo.cs",
	#	"$base_dir\Rhino.ServiceBus.Autofac\Properties\AssemblyInfo.cs",
	#	"$base_dir\Rhino.ServiceBus.Unity\Properties\AssemblyInfo.cs",
	#	"$base_dir\Rhino.ServiceBus.Spring\Properties\AssemblyInfo.cs"
	#);
	#
	#$infos | foreach { Generate-Assembly-Info `
	#	-file $_ `
	#	-title "Rhino Service Bus $version" `
	#	-description "Developer friendly service bus for .NET" `
	#	-company "Hibernating Rhinos" `
	#	-product "Rhino Service Bus $version" `
	#	-version $version `
	#	-copyright "Hibernating Rhinos & Ayende Rahien 2004 - 2009" `
	#}

	new-item $release_dir -itemType directory 
	new-item $build_dir -itemType directory 
}

task Compile -depends Init {
  msbuild $sln_file /p:"OutDir=$35_build_dir;Configuration=$config;TargetFrameworkVersion=V3.5;LibDir=$35_lib_dir"
  msbuild $sln_file /target:Rebuild /p:"OutDir=$40_build_dir;Configuration=$config;TargetFrameworkVersion=4.0"
}

task Test -depends Compile -precondition { return $run_tests }{
  $old = pwd
  cd $build_dir
  & $tools_dir\xUnit\xunit.console.clr4.exe "$build_dir\3.5\BclEx.Tests.dll" /noshadow
  cd $old		
}

task Release -depends Compile, Test {

  cd $build_dir
	& $tools_dir\7za.exe a $release_dir\BclEx.zip `
    	*\Castle.Core.dll `
    	..\license.txt
	if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute ZIP command"
    }
}

task Package -depends Release {
  #$spec_files = @(Get-ChildItem $packageinfo_dir)
  #foreach ($spec in $spec_files)
  #{
  #  & $tools_dir\NuGet.exe pack $spec.FullName -o $release_dir -Version $version -Symbols -BasePath $base_dir
  #}
}
