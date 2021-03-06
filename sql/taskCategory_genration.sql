begin transaction
/****** Script for SelectTopNRows command from SSMS  ******/

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
 --commit transaction 
 --rollback transaction