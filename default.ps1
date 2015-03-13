$ErrorActionPreference = "Stop"

properties {
    $rootDir = Resolve-Path .
    $buildDir = Join-Path $rootDir "Build"
    $srcDir = Join-Path $rootDir "src"
    $solutionPath = Join-Path $rootDir "Busybody.sln"
    $buildConfiguration = "Release"
    $packagesDir = Join-Path $rootDir "Packages"
    $majorVersion = "0"
    $minorVersion = "1"
    $buildNumber = "0"
	$buildTools = Join-Path $rootDir "buildTools"
	$nunitTestsNUnitFile = Join-Path $rootDir "NUnitTests.nunit"
	$nunit = Join-Path $buildTools "NUnit.Runners.2.6.2\tools\nunit-console.exe"
	$nuget = Join-Path $buildTools "nuget\nuget.exe"
}

task ? -description "Helper to display task info" {
    Write-Documentation
}

task Clean {
}

task default -depends UnitTests

task UnitTests -depends NUnitUnitTests, Compile {
}

task AllTests -depends UnitTest, Compile {
}

task Compile -depends Restore, Clean {
	exec { msbuild /v:m $solutionPath /p:"Configuration=$buildConfiguration;Platform=Any CPU;TrackFileAccess=false" }
}

task Restore -depends Clean {
	exec { & $nuget restore $solutionPath }
}

task NUnitUnitTests -depends Compile -description "NUnit unit tests" {
 exec{ & $nunit $nunitTestsNUnitFile /nologo /config:$buildConfiguration /noshadow "/exclude=LongRunning" }
}
