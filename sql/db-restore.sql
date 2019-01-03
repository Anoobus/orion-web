USE [master]
RESTORE DATABASE [orion.web.aspnet.identity] 
	FROM  DISK = N'/var/opt/mssql/data/backups/users.bak' WITH  FILE = 1,  
	MOVE N'orion.web.aspnet.identity' TO N'/var/opt/mssql/data/orion.web.aspnet.identity.mdf',  
	MOVE N'orion.web.aspnet.identity_log' TO N'/var/opt/mssql/data/orion.web.aspnet.identity_log.ldf',  
	NOUNLOAD,  STATS = 5
GO

USE [master]
RESTORE DATABASE [orion.web] 
	FROM  DISK = N'/var/opt/mssql/data/backups/web.bak' WITH  FILE = 1,  
	MOVE N'orion.web' TO N'/var/opt/mssql/data/orion.web.mdf',  
	MOVE N'orion.web_log' TO N'/var/opt/mssql/data/orion.web_log.ldf',  
	NOUNLOAD,  STATS = 5
GO


