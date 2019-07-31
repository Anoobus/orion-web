$ErrorActionPreference = "Stop"

#get date identifier
$dateId=Get-Date -Format "MMM-dd-yyyy"
$dateId="build-$dateId"

#back up db folder
#$backupDbPath=Get-Location
#Copy-Item "..\sql-mount" -Destination "$backupDbPath-sql-mount-backup" -recurse

#set tls level so we can download the zip
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest -Uri "https://github.com/Anoobus/orion-web/archive/master.zip" -OutFile "..\orion-web-master.zip"

#unzip the file
Expand-Archive -LiteralPath "..\orion-web-master.zip" -DestinationPath "..\$dateId"

del  "..\orion-web-master.zip"

#set working dir to "new folder"
cd "..\$dateId"

dotnet restore "orion-web-master\orion.web\orion.web.csproj"
dotnet build "orion-web-master\orion.web\orion.web.csproj" -c Release -o "..\app"
dotnet publish "orion-web-master\orion.web\orion.web.csproj" -c Release -o "..\app"



Move-Item -Path "orion-web-master\app" -Destination "app"
Remove-Item -LiteralPath "orion-web-master" -Force -Recurse
cd app

$Env:ASPNETCORE_URLS += "https://+;http://+"
$Env:ASPNETCORE_Kestrel__Certificates__Default__Path += "/app/self-cert.pfx"
$Env:ASPNETCORE_ENVIRONMENT += "Development"
dotnet orion.web.dll

#start /b dotnet orion.web.dll