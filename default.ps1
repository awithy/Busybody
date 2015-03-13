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
}

task ? -description "Helper to display task info" {
    Write-Documentation
}

task Clean {
#TBC
}

task default -depends UnitTest

task UnitTest -depends Compile {
#TBC
}

task AllTests -depends Compile {
#TBC
}

task Compile -depends Clean {
	exec { msbuild /v:m $solutionPath /p:"Configuration=$buildConfiguration;Platform=Any CPU;TrackFileAccess=false" }
}
