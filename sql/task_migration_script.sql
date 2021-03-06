/****** Script for SelectTopNRows command from SSMS  ******/
begin transaction
SELECT TOP (20) [JobTaskId]
      ,SUBSTRING ( [Name] ,1 ,2 ) 
	  ,[Name]
      ,[Description]
      ,[TaskCategoryId]
      ,[LegacyCode]
      ,[UsageStatusId]
  FROM [orion.web].[dbo].[JobTasks]
  

  update [orion.web].[dbo].[JobTasks] 
	set LegacyCode = SUBSTRING ( [Name] ,1 ,2 ),
	UsageStatusId = 1,
	[name] = [Description],
	[Description] = ''


	SELECT TOP (20) [JobTaskId]
      ,SUBSTRING ( [Name] ,1 ,2 ) 
	  ,[Name]
      ,[Description]
      ,[TaskCategoryId]
      ,[LegacyCode]
      ,[UsageStatusId]
  FROM [orion.web].[dbo].[JobTasks]
  commit transaction