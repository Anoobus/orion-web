
/****** Script for SelectTopNRows command from SSMS  ******/
begin transaction


set identity_insert [TaskCategories] on
SELECT TOP (1000) [TaskCategoryId]
      ,[Name]
  FROM [orion.web].[dbo].[TaskCategories]
  
  ;with sourceData as
  ( 
	select 1 as Id,	'Civil/Structural' as CatName union all
	select  2 as Id,	'Electrical'as CatName union all
	select  3 as Id,	'Mechanical'as CatName union all
	select  4 as Id,	'Administrative'as CatName 
)

 MERGE [TaskCategories] AS target  
    USING sourceData AS source  
    ON (target.TaskCategoryId = source.Id)  
    WHEN MATCHED THEN   
        UPDATE SET Name = source.CatName  
WHEN NOT MATCHED THEN  
    INSERT (TaskCategoryId, Name)  
    VALUES (Id,source.CatName)  ;
	go

	set identity_insert [TaskCategories] off
	SELECT TOP (1000) [TaskCategoryId]
      ,[Name]
  FROM [orion.web].[dbo].[TaskCategories]

SELECT TOP (1000) [JobTaskId]
      ,[ShortName]
      ,[Description]
      ,[TaskCategoryId]
  FROM [orion.web].[dbo].[JobTasks]

  ;with tasks as
  ( select 'Drawings' as name, '10' as code union all
 select 'Review Mfrs. Drawings' as name, '11' as code union all
 select 'Bid Evaluation' as name, '12' as code union all
 select 'As-Builts' as name, '13' as code union all
 select 'Calculations' as name, '14' as code union all
 select 'Specifications' as name, '15' as code union all
 select 'Construction Liaison/Inspection' as name, '16' as code union all
 select 'Studies' as name, '17' as code union all
 select 'Field Trips for Data' as name, '18' as code union all
 select 'Client Meetings' as name, '19' as code union all
 select 'Drawings' as name, '30' as code union all
 select 'Review Mfrs. Drawings' as name, '31' as code union all
 select 'Bid Evaluation' as name, '32' as code union all
 select 'As-Builts' as name, '33' as code union all
 select 'Calculations' as name, '34' as code union all
 select 'Specifications' as name, '35' as code union all
 select 'Construction Liaison/Inspection' as name, '36' as code union all
 select 'Studies' as name, '37' as code union all
 select 'Field Trips for Data' as name, '38' as code union all
 select 'Client Meetings' as name, '39' as code union all
 select 'Drawings' as name, '40' as code union all
 select 'Review Mfrs. Drawings' as name, '41' as code union all
 select 'Bid Evaluation' as name, '42' as code union all
 select 'As-Builts' as name, '43' as code union all
 select 'Calculations' as name, '44' as code union all
 select 'Specifications' as name, '45' as code union all
 select 'Construction Liaison/Inspection' as name, '46' as code union all
 select 'Studies' as name, '47' as code union all
 select 'Field Trips for Data' as name, '48' as code union all
 select 'Client Meetings' as name, '49' as code union all
 select 'Computer Time (Engineering)' as name, '74' as code union all
 select 'Administration & Management' as name, '75' as code union all
 select 'Recruiting' as name, '76' as code union all
 select 'Clerical' as name, '77' as code union all
 select 'Word Processing' as name, '78' as code union all
 select 'CADD (Machine Time)' as name, '79' as code union all
 select 'Service & Computer Development' as name, '80' as code union all
 select 'Professional Development/Training' as name, '81' as code union all
 select 'Product/Service Development' as name, '82' as code union all
 select 'Personal Time' as name, '83' as code union all
 select 'Unassigned' as name, '84' as code union all
 select 'Sick Time' as name, '85' as code union all
 select 'Excused WITH Pay' as name, '86' as code union all
 select 'Vacation' as name, '87' as code union all
 select 'Holiday' as name, '88' as code union all
 select 'Excused WITHOUT Pay' as name, '89' as code union all
 select 'New Client Development' as name, '90' as code union all
 select 'Old Client Development' as name, '91' as code union all
 select 'Proposal/Contract Activity' as name, '92' as code 
)



insert into [orion.web].[dbo].[JobTasks] ([ShortName]
      ,[Description]
      ,[TaskCategoryId])

select [code] + ' - ' + [name] as shortName, [name] as [Description], 2 as [TaskCategoryId] from tasks



  --commit transaction