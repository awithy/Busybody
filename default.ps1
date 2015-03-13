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
	$nunitTestsNUnitFile = Join-Path $rootDir "NUnitTests.nunit"
	$nunit = Join-Path $rootDir "buildTools\NUnit.Runners.2.6.2\tools\nunit-console.exe"
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

task Compile -depends Clean {
	exec { msbuild /v:m $solutionPath /p:"Configuration=$buildConfiguration;Platform=Any CPU;TrackFileAccess=false" }
}

task NUnitUnitTests -depends Compile -description "NUnit unit tests" {
 exec{ & $nunit $nunitTestsNUnitFile /nologo /config:$buildConfiguration /noshadow "/exclude=LongRunning" }
}
