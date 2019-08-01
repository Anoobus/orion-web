$ErrorActionPreference = "Stop"

#get date identifier
$dateId=Get-Date -Format "MMM-dd-yyyy"
$dateId="build-$dateId"

#create sql data dir for this build
mkdir "..\$dateId\sql-data"

Copy-Item -Path "upload-data" -Destination  "..\$dateId\upload-data" -Recurse

#backup our current DB into our build folder
$backupCmd=[string]::Format("BACKUP DATABASE [orion.web] TO DISK='{0}\{1}\sql-data\orion.web.bak'", ((Get-Item (Get-Location).Path).Parent).FullName, $dateId);
SqlCmd -E -S "(localdb)\mssqllocaldb" -Q "$backupCmd"
$backupCmd=[string]::Format("BACKUP DATABASE [orion.web.aspnet.identity] TO DISK='{0}\{1}\sql-data\orion.web.aspnet.identity.bak'", ((Get-Item (Get-Location).Path).Parent).FullName, $dateId);
SqlCmd -E -S "(localdb)\mssqllocaldb" -Q "$backupCmd"


#get latest source code
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
Move-Item -Path "app\run.ps1" -Destination "run.ps1"
Move-Item -Path "app\deploy-latest.ps1" -Destination "deploy-latest.ps1"
Remove-Item -LiteralPath "orion-web-master" -Force -Recurse
cd app