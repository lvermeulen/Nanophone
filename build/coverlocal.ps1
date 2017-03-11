# run:
#	from src: ..\build\coverlocal.ps1
#	reportgenerator -reports:C:\gitrepo\Nanophone\codecoverage\coverage\coverage.xml -targetdir:.
#	start index.htm

$CoverFilter = '+[Nano*]* -[*]*.Logging.* -[*Tests]*'
Write-Output "Starting code coverage with filter: $CoverFilter"

$alwaysFilter = "-[xunit*]* -[Microsoft*]* -[dotnet*]* -[NuGet*]* -[Newtonsoft*]* -[Consul*]* -[Nancy*]* -[AngleSharp]* -[csc]* -[Anonymously*]*"
$filter = "$CoverFilter $alwaysFilter"

$packagesPath = $env:USERPROFILE + "\.nuget\packages"
$opencoverPath = $packagesPath + "\OpenCover\4.6.519\tools\OpenCover.Console.exe"
$tempPath = "..\codecoverage"
$tempCoveragePath = $tempPath + "\coverage\"
$tempCoverageFileName = $tempCoveragePath + "coverage.xml"

Write-Host "opencoverPath: $opencoverPath"
Write-Host "tempPath: $tempPath"
Write-Host "tempCoveragePath: $tempCoveragePath"
Write-Host "tempCoverageFileName: $tempCoverageFileName"
Write-Host "PSScriptRoot: $PSScriptRoot"

# create temp path
if (-not (test-path $opencoverPath) ) {
	Write-Host "$opencoverPath doesn't exist"
}

# run opencover
Get-ChildItem -Path $PSScriptRoot\..\test -Filter project.json -Recurse | ForEach-Object {
    $path = "$tempPath\$($_.Directory.BaseName)"
    if (-not (test-path $path) ) {
        mkdir $path | Out-Null
    }

    $tempBinPath = $path + "\bin\"
    $targetArgs = "`"test -o $tempBinPath $($_.FullName)`""

    if (-not (test-path $tempBinPath) ) {
        mkdir $tempBinPath | Out-Null
    }

    if (-not (test-path $tempCoveragePath) ) {
        mkdir $tempCoveragePath | Out-Null
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

