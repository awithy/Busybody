$ErrorActionPreference = "Stop"

$rootDir = Resolve-Path .
$nugetDir = Join-Path $rootDir "nuget"
if ($rootDir.Path.EndsWith("scripts")) {
	$rootDir = Join-Path $rootDir "..\"
}

$psakeDir = ls $buildTools | where { $_.Name.StartsWith("psake") }

$scriptPath = join-path $rootDir "buildTools\psake.4.3.0.0\tools"

import-module (join-path $scriptPath psake.psm1)

if($args.Length -eq 0) {
  $args = "default"
}
 
#invoke-psake $buildFile $taskList $framework $docs $parameters $properties $initialization $nologo
invoke-psake $args

if($psake.build_success -eq $false) {
    exit 1
}
