begin transaction
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [SiteID]
      ,[SiteName]
  FROM [orion.web].[dbo].[Sites]

  ;with datas as 
  (
  select 'GM Arlington - Arlington, TX'  as site union all
select 'GM Flint - Flint, MI'  as site union all
select 'GM Ft. Wayne - Ft. Wayne, IN'  as site union all
select 'GMM Silao - Silao, MX'  as site union all
select 'GMM Silao - Silao, MX'  as site union all
select 'GM Wentzville - Wentzville, MO'  as site union all
select 'GM Wentzville - Wentzville, MO'  as site union all
select 'GM Parma - Cleveland, OH'  as site union all
select 'GM Bowling Green - Bowling Green, KY'  as site union all
select 'GM Renaissance - Detroit, MI'  as site union all
select 'Detroit Diesel - Redford, MI'  as site union all
select 'GM Tonawanda - Buffalo, NY'  as site union all
select 'GM Ft. Wayne - Ft. Wayne, IN'  as site union all
select 'GM Fairfax - Kansas City, KS'  as site union all
select 'GM Fairfax - Kansas City, KS'  as site union all
select 'GM Fairfax - Kansas City, KS'  as site union all
select 'GM Bowling Green - Bowling Green, KY'  as site union all
select 'GM Bowling Green - Bowling Green, KY'  as site union all
select 'GMM Silao - Silao, MX'  as site union all
select 'GM CCA Grand Blanc - Grand Blanc, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GM Bowling Green - Bowling Green, KY'  as site union all
select 'GM Lordstown - Lordstown, OH'  as site union all
select 'GM Lordstown - Lordstown, OH'  as site union all
select 'Lake Erie Electric - OH'  as site union all
select 'LBW&L - Lansing, MI'  as site union all
select 'Lansing, MI - Lansing, MI'  as site union all
select 'Lansing, MI - Lansing, MI'  as site union all
select 'Lansing, MI - Lansing, MI'  as site union all
select 'Various Locations MI - Various Locations MI'  as site union all
select 'Jackson, MI - Jackson, MI'  as site union all
select 'Jackson, MI - Jackson, MI'  as site union all
select 'Jackson, MI - Jackson, MI'  as site union all
select 'East Lansing, MI - East Lansing, MI'  as site union all
select 'East Lansing, MI - East Lansing, MI'  as site union all
select 'East Lansing, MI - East Lansing, MI'  as site union all
select 'East Lansing, MI - East Lansing, MI'  as site union all
select 'East Lansing, MI - East Lansing, MI'  as site union all
select 'East Lansing, MI - East Lansing, MI'  as site union all
select 'East Lansing, MI - East Lansing, MI'  as site union all
select 'East Lansing, MI - East Lansing, MI'  as site union all
select 'Marion, IN - Marion, IN'  as site union all
select 'Flint, MI - Flint, MI'  as site union all
select 'Kalamazoo Research - Kalamazoo, MI'  as site union all
select '? - MI'  as site union all
select 'GM Warren Tech Center - Warren, MI'  as site union all
select 'GM Pontiac - Pontiac, MI'  as site union all
select 'GM Warren - Warren, MI'  as site union all
select 'GM CCA Memphis - Memphis, TN'  as site union all
select 'GM Ft. Wayne - Ft. Wayne, IN'  as site union all
select 'GM Defiance - Defiance, OH'  as site union all
select 'GM Defiance - Defiance, OH'  as site union all
select 'GM Bay City - Bay City, MI'  as site union all
select 'GM Bay City - Bay City, MI'  as site union all
select 'N/A - Jackson, MI'  as site union all
select 'GM Oshawa Car Assembly Plant - Oshawa, ON'  as site union all
select 'GM Canadian Technical Centre - Markham, ON'  as site union all
select 'GM Canadian Headquarters Building - Oshawa, ON'  as site union all
select 'GM Cold Weather Test Development Centre - Kapuskasing, ON'  as site union all
select 'GM Montreal Parts Distribution Centre - Montreal, QC'  as site union all
select 'GM Woodstock Parts Distribution Centre - Woodstock, ON'  as site union all
select 'GM Edmonton Parts Distribution Centre - Edmonton, AB'  as site union all
select 'GM Vancouver Parts Distribution Centre - Langley Twp, British Columbia'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'Pfizer Bldg. 541 - Portage, MI'  as site union all
select 'Oaks Correctional Facility - Manistee, MI'  as site union all
select 'Ox Paperboard - Constantine, MI'  as site union all
select 'GM Parma - Parma, OH'  as site union all
select 'GM Parma - Parma, OH'  as site union all
select 'GM Parma - Parma, OH'  as site union all
select 'GM Parma - Parma, OH'  as site union all
select 'GM Parma - Parma, OH'  as site union all
select 'GM Pontiac North - Pontiac, MI'  as site union all
select 'GM Pontiac North - Pontiac, MI'  as site union all
select 'GM Pontiac North - Pontiac, MI'  as site union all
select 'GM Pontiac North - Pontiac, MI'  as site union all
select 'GM Pontiac North - Pontiac, MI'  as site union all
select 'GM Pontiac North - Pontiac, MI'  as site union all
select 'GM Pontiac North - Pontiac, MI'  as site union all
select 'GM Pontiac North - Pontiac, MI'  as site union all
select 'GM Pontiac North - Pontiac, MI'  as site union all
select 'GMM Ramos - Ramos, MX'  as site union all
select 'GMM Ramos - Ramos, MX'  as site union all
select 'GMM Silao - Silao, MX'  as site union all
select 'GM Flint - Flint, MI'  as site union all
select 'GMM Silao - Silao, MX'  as site union all
select 'GMM San Luis Potosi - San Luis Potosi, MX'  as site union all
select 'GM St. Catharines - St. Catharines, ON'  as site union all
select 'GM Toledo - Toledo, OH'  as site union all
select 'GM Toledo - Toledo, OH'  as site union all
select 'GM Toledo - Toledo, OH'  as site union all
select 'GM Toledo - Toledo, OH'  as site union all
select 'GM Bowling Green - Bowling Green, KY'  as site union all
select 'GM Bowling Green - Bowling Green, KY'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GMM San Luis Potosi - San Luis Potosi, MX'  as site union all
select 'GM Flint - Flint, MI'  as site union all
select 'GMCH - Wyoming, MI'  as site union all
select 'GM Warren Tech Center - Warren, MI'  as site union all
select 'GM Warren Tech Center - Warren, MI'  as site union all
select 'GM Ft. Wayne - Ft. Wayne, IN'  as site union all
select 'GM Flint - Flint, MI'  as site union all
select 'GM Warren Tech Center - Warren, MI'  as site union all
select 'GM Warren Tech Center - Warren, MI'  as site union all
select 'GM Warren Tech Center - Warren, MI'  as site union all
select 'GM Tonawanda - Buffalo, NY'  as site union all
select 'Portage Road Site - Kalamazoo, MI'  as site union all
select 'Bldgs 315 & 318 - Kalamazoo, MI'  as site union all
select 'Portage Road Site - Kalamazoo, MI'  as site union all
select 'Portage Road Site - Kalamazoo, MI'  as site union all
select 'Portage Road Site - Kalamazoo, MI'  as site union all
select 'Portage Road Site - Kalamazoo, MI'  as site union all
select 'Portage Road Site - Kalamazoo, MI'  as site union all
select 'Portage Road Site - Kalamazoo, MI'  as site union all
select 'Portage Road Site - Kalamazoo, MI'  as site union all
select 'Portage Road Site - Kalamazoo, MI'  as site union all
select 'Portage Road Site - Kalamazoo, MI'  as site union all
select 'Portage Road Site - Kalamazoo, MI'  as site union all
select 'GM Wentzville - Wentzville, MO'  as site union all
select 'Bldg 248 - Kalamazoo, MI'  as site union all
select 'Zoetis - Kalamazoo, MI'  as site union all
select 'Bldg 214 - Kalamazoo, MI'  as site union all
select 'GM Arlington - Arlington, TX'  as site union all
select 'GM Ft. Wayne - Ft. Wayne, IN'  as site union all
select 'GMM Silao - Silao, MX'  as site union all
select 'GM Bowling Green - Bowling Green, KY'  as site union all
select 'GMM Ramos - Ramos, MX'  as site union all
select 'GMM San Luis Potosi - San Luis Potosi, MX'  as site union all
select 'GM Bowling Green - Bowling Green, KY'  as site 
)

insert into Sites (SiteName)
select distinct [site] from datas

SELECT TOP (1000) [SiteID]
      ,[SiteName]
  FROM [orion.web].[dbo].[Sites]


  commit transaction
