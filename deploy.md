How to re-deploy an update the app


1) Backup existing user Db
2) Backup existing app Db
3) stop all services
	#In powershell
	docker stop $(docker ps -q)
4) Rename folder from orion.web.master to orion.web.master.$(todays date)
5) set powershell tls level to pull the code
	 [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
5) download the latest source code https://github.com/Anoobus/orion-web/archive/master.zip 
	Invoke-WebRequest -Uri "https://github.com/Anoobus/orion-web/archive/master.zip" -OutFile orion-web-master.zip
6) extract the zip file
	rigth click extract all
	for the destination; orion-web-master since it will unzip to a folder anyway
7) 

Backing Up a database

1) In ssms, right click db -> tasks -> backup
	Db = target Db
	Backup Type = Full
	Backup Component = Database
	Backup To = Disk
	Remove any existing entries and then click [Add...] 
	FileName destination = some item under /var/opt/mssql/data/
2) copy the backups to the host machine
	* with bind mount
		A) Navigate to the sql data folder
	* without bind mount
		A) in powershell
			#get the id and set it to a variable
			$id=$(docker ps -f NAME=orion-sql -q)
			#copy all the data in sql !!update the c:\temp with the proper sql data folder!!
			docker cp $id/:"/var/opt/mssql/data" "C:\temp"
		B) Navigate to the sql data folder
	