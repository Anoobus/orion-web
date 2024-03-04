$ErrorActionPreference = "Stop"

Write-Host get date identifier
$dateId=Get-Date -Format "yyyy-MM-dd-\rHHmm"
$dateId="build-$dateId"

Write-Host create sql data dir for this build
mkdir "..\$dateId\sql-data-bu"

Copy-Item -Path "upload-data" -Destination  "..\$dateId\upload-data" -Recurse

Write-Host backup our current DB into our build folder
$backupCmd=[string]::Format("BACKUP DATABASE [orion.web] TO DISK='{0}\{1}\sql-data-bu\orion.web.{1}.bak'", ((Get-Item (Get-Location).Path).Parent).FullName, $dateId);
SqlCmd -E -S "localhost\sql2017" -Q "$backupCmd"
$backupCmd=[string]::Format("BACKUP DATABASE [orion.web.aspnet.identity] TO DISK='{0}\{1}\sql-data-bu\orion.web.aspnet.identity.{1}.bak'", ((Get-Item (Get-Location).Path).Parent).FullName, $dateId);
SqlCmd -E -S "localhost\sql2017" -Q "$backupCmd"


Write-Host get latest source code
#set tls level so we can download the zip
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest -Uri "https://github.com/Anoobus/orion-web/archive/master.zip" -OutFile "..\orion-web-master.zip"

Write-Host unzip the file
Expand-Archive -LiteralPath "..\orion-web-master.zip" -DestinationPath "..\$dateId"
del  "..\orion-web-master.zip"

Write-Host copy Prod AppSettings
Copy-Item -Path "app\appsettings.Production.json" -Destination "..\$dateId"

Write-Host set working dir to "new folder"
cd "..\$dateId"

Write-Host restore nugets for build
dotnet restore "orion-web-master\orion.web\orion.web.csproj"
Write-Host build in release con
dotnet build "orion-web-master\orion.web\orion.web.csproj" -c Release
Write-Host publish built project
dotnet publish "orion-web-master\orion.web\orion.web.csproj" -c Release -o "app"

#Write-Host move published results to final app folder
#Move-Item -Path "orion-web-master\app" -Destination "app"
Write-Host move deploy files to final build folder
Move-Item -Path "app\deploy-latest.ps1" -Destination "deploy-latest.ps1"
Write-Host remove sources folder
Remove-Item -LiteralPath "orion-web-master" -Force -Recurse

Write-Host move prod AppSettings to app folder
Move-Item -Path "appsettings.Production.json" -Destination "app/appsettings.Production.json"