$consul_version = '0.7.2'
    
choco install 7zip.commandline
$consul_zip = "consul_$($consul_version)_windows_386.zip"
$consul_url = "https://releases.hashicorp.com/consul/$($consul_version)/$consul_zip"
if (!(test-path "c:\consul_install")) {mkdir -Force c:\consul_install | Out-Null}
write-output "Downloading https://releases.hashicorp.com/consul/$consul_version/$consul_zip"
if (!(test-path "c:\consul_install\$consul_zip")) {Invoke-WebRequest $consul_url -OutFile "c:\consul_install\$consul_zip"}
7z x -y c:\consul_install\$consul_zip -oc:\consul\consul.test
