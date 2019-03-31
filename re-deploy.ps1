#manual start location for testing
#cd "C:\temp\build-Jan-22-2019"

#stop services
$services=$(docker ps -q)
if ($services) { docker stop $(docker ps -q) }

#get date identifier
$dateId=Get-Date -Format "MMM-dd-yyyy"
$dateId="build-$dateId"

#back up db folder
$backupDbPath=Get-Location
Copy-Item "..\sql-mount" -Destination "$backupDbPath-sql-mount-backup" -recurse

#set tls level so we can download the zip
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest -Uri "https://github.com/Anoobus/orion-web/archive/master.zip" -OutFile "..\orion-web-master.zip"

#unzip the file
Expand-Archive -LiteralPath "..\orion-web-master.zip" -DestinationPath "..\temp"
Move-Item -Path "..\temp\orion-web-master" -Destination "..\$dateId"

#copy over the .env file
Copy-Item ".\.env" -Destination "..\$dateId"

#delete old zip folder
Remove-Item "..\orion-web-master.zip"
Remove-Item "..\temp" -Recurse

#set working dir to "new folder"
cd "..\$dateId"

#start app and sql
docker-compose up -d