BACKUP DATABASE [orion.web.aspnet.identity] 
	TO  DISK = N'/var/opt/mssql/data/backups/users.bak' WITH NOFORMAT,
	NOINIT,  
	NAME = N'users backup', 
	SKIP, 
	NOREWIND, 
	NOUNLOAD,  STATS = 10
GO

BACKUP DATABASE [orion.web] 
	TO  DISK = N'/var/opt/mssql/data/backups/web.bak' WITH NOFORMAT,
	NOINIT,  
	NAME = N'web backup', 
	SKIP, 
	NOREWIND, 
	NOUNLOAD,  STATS = 10
GO
