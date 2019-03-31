


 insert into [orion.web].[dbo].[EmployeeJob] (JobId, EmployeeId)
  SELECT j.JobId, e.EmployeeId
  FROM [orion.web].[dbo].[Employees] e
  cross apply [dbo].[Jobs] j
  where j.JobCode = '0000'
	or j.JobId = 87
	order by 2,1


 delete from [orion.web].[dbo].[EmployeeJob] 
 delete FROM [orion.web].[dbo].[TimeEntries]
 delete  FROM [orion.web].[dbo].[TimeSheetApprovals]

 USE [orion.web];
GO  

ALTER TABLE dbo.EmployeeJob   
ADD CONSTRAINT UC_EmployeeJob_NoDupe UNIQUE (EmployeeId, JobId);   
GO  


  