
$Env:ASPNETCORE_URLS = "https://+;http://+"
$Env:ASPNETCORE_Kestrel__Certificates__Default__Path = (Get-ChildItem "self-cert.pfx").FullName
$Env:ASPNETCORE_ENVIRONMENT = "VM"
$ErrorActionPreference = "Continue"

Start-Process dotnet orion.web.dll -NoNewWindow -RedirectStandardError "log.err.txt" -RedirectStandardOutput "log.out.txt"