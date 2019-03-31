How to re-deploy an update the app


1) Backup existing user Db
2) Backup existing app Db
3) run re-deploy.ps1
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
	