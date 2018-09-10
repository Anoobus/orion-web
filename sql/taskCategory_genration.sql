/****** Script for SelectTopNRows command from SSMS  ******/
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
    INSERT ( Name)  
    VALUES (source.CatName)  
	go
 