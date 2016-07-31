param (
    [string]$CoverallsRepoToken=$(throw "-CoverallsRepoToken is required."),
    [string]$CoverFilter=$(throw "-CoverFilter is required.")
)

Add-AppveyorCompilationMessage -Message "Code coverage started"
Add-AppveyorCompilationMessage -Message "Code coverage filter: $($CoverFilter)" 

# run restore on all project.json files in the src folder including 2>1 to redirect stderr to stdout for badly behaved tools
#Get-ChildItem -Path $PSScriptRoot\..\test -Filter project.json -Recurse | ForEach-Object { & dotnet restore $_.FullName 2>&1 }

$alwaysFilter = "-[xunit*]* -[Microsoft*]* -[dotnet*]* -[NuGet*]* -[Newtonsoft*]* -[Consul*]* -[Nancy*]* -[csc]* -[Anonymously*]*"
$filter = "$CoverFilter $alwaysFilter"

#$packagesPath = "C:\Users\Luk\.nuget\packages"
$packagesPath = "\packages"
$opencoverPath = $packagesPath + "\OpenCover\4.6.519\tools\OpenCover.Console.exe"
$coverallsPath = $packagesPath + "\coveralls.io\1.3.4\tools\coveralls.net.exe"
$tempPath = "\temp"
$tempCoveragePath = $tempPath + "\coverage\"
$tempCoverageFileName = $tempCoveragePath + "coverage.xml"

# create temp path
if (-not (test-path $tempPath) ) {
    md $tempPath | Out-Null
}

# remove temp subfolders
Get-ChildItem -Path $tempPath -Directory | ForEach-Object { Remove-Item -Recurse $_.FullName }

# run opencover
Get-ChildItem -Path ..\test -Filter project.json -Recurse | ForEach-Object {

    $path = "$tempPath\$($_.Directory.BaseName)"
    md $path | Out-Null

    $tempBinPath = $path + "\bin\"
    $targetArgs = "`"test -o $tempBinPath $($_.FullName)`""

    md $tempBinPath | Out-Null

    if (-not (test-path $tempCoveragePath) ) {
        md $tempCoveragePath | Out-Null
    }

    & $opencoverPath `
        -register:user `
        -target:"dotnet.exe" `
        -targetargs:$targetArgs `
        -searchdirs:$tempBinPath `
        -output:$tempCoverageFileName `
        -mergebyhash `
        -mergeoutput `
        -skipautoprops `
        -returntargetcode `
        -filter:$filter `
        -hideskipped:Filter `
        -oldstyle 

}

Get-ChildItem -Path $tempCoveragePath -Filter coverage.xml | ForEach-Object { Add-AppveyorCompilationMessage -Message $_.FullName }

<#

# upload to coveralls.io
Add-AppveyorCompilationMessage -Message "Sending code coverage results to coveralls.io"

& $coverallsPath `
    --opencover $tempCoverageFileName `
    --full-sources `
    --repo-token $CoverallsRepoToken

#>

Add-AppveyorCompilationMessage -Message "Code coverage ended"
