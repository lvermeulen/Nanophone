param (
    [string]$CoverallsRepoToken=$(throw "-CoverallsRepoToken is required."),
    [string]$CoverFilter=$(throw "-CoverFilter is required.")
)

Write-Output "Code coverage started"
Write-Output "Code coverage filter: $($CoverFilter)" 

# run restore on all project.json files in the src folder including 2>1 to redirect stderr to stdout for badly behaved tools
Get-ChildItem -Path $PSScriptRoot\..\test -Filter project.json -Recurse | ForEach-Object { & dotnet restore $_.FullName 2>&1 }

$alwaysFilter = "-[xunit*]* -[Microsoft*]* -[dotnet*]* -[NuGet*]* -[Newtonsoft*]* -[Consul*]* -[Nancy*]* -[csc]* -[Anonymously*]*"
$filter = "$CoverFilter $alwaysFilter"

$packagesPath = $env:USERPROFILE + "\.nuget\packages"
$opencoverPath = $packagesPath + "\OpenCover\4.6.519\tools\OpenCover.Console.exe"
$coverallsPath = $packagesPath + "\coveralls.io\1.3.4\tools\coveralls.net.exe"
$tempPath = "c:\codecoverage"
$tempCoveragePath = $tempPath + "\coverage\"
$tempCoverageFileName = $tempCoveragePath + "coverage.xml"

# check existence of packages
if (test-path $packagesPath) {
    Write-Output "Packages path found: $packagesPath"
}
else {
    Write-Output "Packages path not found: $packagesPath"
}

# check existence of tools
if (test-path $opencoverPath) {
    Write-Output "OpenCover is present"
}
else {
    Write-Output "OpenCover is not present: $opencoverPath"
}

if (test-path $coverallsPath) {
    Write-Output "coveralls is present"
}
else {
    Write-Output "coveralls is not present: $coverallsPath"
}

# create temp path
Write-Output "Creating $tempPath"
if (-not (test-path $tempPath) ) {
    mkdir $tempPath # | Out-Null
}
if (-not (test-path $tempPath) ) {
    Write-Output "$tempPath doesn't exist after mkdir"
}

# remove temp subfolders
#Get-ChildItem -Path $tempPath -Directory | ForEach-Object { Remove-Item -Recurse $_.FullName }

# run opencover
Get-ChildItem -Path ..\test -Filter project.json -Recurse | ForEach-Object {
    Write-Output "Running OpenCover on every test project"
    $path = "$tempPath\$($_.Directory.BaseName)"
    if (-not (test-path $path) ) {
        mkdir $path # | Out-Null
    }
    if (-not (test-path $path) ) {
        Write-Output "$path doesn't exist after mkdir"
    }

    $tempBinPath = $path + "\bin\"
    $targetArgs = "`"test -o $tempBinPath $($_.FullName)`""

    if (-not (test-path $tempBinPath) ) {
        mkdir $tempBinPath # | Out-Null
    }
    if (-not (test-path $tempBinPath) ) {
        Write-Output "$tempBinPath doesn't exist after mkdir"
    }

    if (-not (test-path $tempCoveragePath) ) {
        mkdir $tempCoveragePath # | Out-Null
    }
    if (-not (test-path $tempCoveragePath) ) {
        Write-Output "$tempCoveragePath doesn't exist after mkdir"
    }

    Write-Output "Running OpenCover on "
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

#Get-ChildItem -Path $tempCoveragePath -Filter coverage.xml | ForEach-Object { Write-Output $_.FullName }

<#

# upload to coveralls.io
Write-Output "Sending code coverage results to coveralls.io"

& $coverallsPath `
    --opencover $tempCoverageFileName `
    --full-sources `
    --repo-token $CoverallsRepoToken

#>

Write-Output "Code coverage ended"
