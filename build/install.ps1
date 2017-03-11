$consul_version = "0.7.2"

cinst 7zip.commandline
mkdir -Force c:\consul_install | Out-Null
$consul_zip = "consul_$($consul_version)_windows_386.zip"
$consul_url = "https://releases.hashicorp.com/consul/$($consul_version)/$consul_zip"
if (!(test-path "c:\consul_install\$consul_zip")) {
    write-output "Downloading $consul_url"
    [Net.ServicePointManager]::SecurityProtocol = 'Tls12'
    Invoke-WebRequest $consul_url -OutFile "c:\consul_install\$consul_zip"
}
7z x -y c:\consul_install\$consul_zip -oc:\consul\consul.test
