cd "C:\temp\Jan-21-2019\orion-web-master"
#stop services
$services=$(docker ps -q)
if ($services) { docker stop $(docker ps -q) }
#set tls level so we can download the zip
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest -Uri "https://github.com/Anoobus/orion-web/archive/master.zip" -OutFile "..\..\orion-web-master.zip"
#get date identifier
$dateId=Get-Date -Format "MMM-dd-yyyy"
#unzip the file
Expand-Archive -LiteralPath "..\..\orion-web-master.zip" -DestinationPath "..\..\$dateId"
#copy over the .env file
Copy-Item ".\.env" -Destination "..\..\$dateId\orion-web-master"
#delete old zip folder
Remove-Item "..\..\orion-web-master.zip"
#set working dir to "new folder"
cd "..\..\$dateId\orion-web-master"
docker-compose up -d