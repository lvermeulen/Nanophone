$consulJob = Start-Job {C:\consul\Consul.Test\consul.exe agent -dev
$running = $false
do {
    Receive-Job -Job $consulJob
    if ($running -and $consulJob.State -ne "Running") {break}
    if ($consulJob.State -eq "Running") {
    $running = $true
    try {$result = convertfrom-json (invoke-webrequest -UseBasicParsing -TimeoutSec 1 -Uri http://localhost:8500/v1/status/leader).Content; sleep 1} catch {write-output "Waiting for Consul to come up..."}
    }
} until (![string]::IsNullOrEmpty($result))
Receive-Job $consulJob
