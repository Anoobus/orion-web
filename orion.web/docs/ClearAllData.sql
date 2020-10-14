--Clear All Data

truncate table [orion.web].[dbo].TimeSheetApprovals
truncate table [orion.web].[dbo].timeentries
truncate table [orion.web].[dbo].employeejobs
truncate table [orion.web].[dbo].expenses

delete from [orion.web].[dbo].jobs
DBCC CHECKIDENT ('[orion.web].[dbo].jobs', RESEED, 0);  


delete from  [orion.web].[dbo].sites
DBCC CHECKIDENT ('[orion.web].[dbo].sites', RESEED, 0);  

delete from  [orion.web].[dbo].clients
DBCC CHECKIDENT ('[orion.web].[dbo].clients', RESEED, 0); 

INSERT into [orion.web].dbo.clients (ClientName)
values ('Orion-Internal','0000')
insert into [orion.web].dbo.sites (sitename) values ('Jackson HQ')
insert into [orion.web].dbo.[Jobs] ( JobCode, JobName, ClientId, SiteId, TargetHours, JobStatusId, EmployeeId)
values ('0000','Orion Internal',1,1,0,1,2)