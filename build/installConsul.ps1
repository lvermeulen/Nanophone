$consul_version = '0.7.0'
    
choco install 7zip.commandline
$consul_zip = "consul_$($consul_version)_windows_386.zip"
if (!(test-path "c:\consul_install")) {mkdir c:\consul_install}
write-output "Downloading https://releases.hashicorp.com/consul/$consul_version/$consul_zip"
if (!(test-path "c:\consul_install\$consul_zip")) {(new-object net.webclient).downloadfile("https://releases.hashicorp.com/consul/$consul_version/$consul_zip", "c:\consul_install\$consul_zip")}
7z x -y c:\consul_install\$consul_zip -oc:\consul\consul.test
