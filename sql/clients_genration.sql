/****** Script for SelectTopNRows command from SSMS  ******/
begin transaction
SELECT TOP (1000) [ClientId]
      ,[ClientName]
      ,[ClientCode]
  FROM [orion.web].[dbo].[Clients]

  ;with datas as 
  (

  select '0221' as code, 'BARTON MALOW ' as client union all
select '0221' as code, 'BARTON MALOW ' as client union all
select '0221' as code, 'BARTON MALOW ' as client union all
select '0221' as code, 'BARTON MALOW ' as client union all
select '0221' as code, 'BARTON MALOW ' as client union all
select '0336' as code, 'CONTI ELECTRIC ' as client union all
select '0336' as code, 'CONTI ELECTRIC ' as client union all
select '0336' as code, 'CONTI ELECTRIC ' as client union all
select '0336' as code, 'CONTI ELECTRIC ' as client union all
select '0338' as code, 'CENTER LINE ELECTRIC ' as client union all
select '0417' as code, 'DETROIT DIESEL (DAIMLER)' as client union all
select '0508' as code, 'THE E&L CONSTRUCTION GROUP ' as client union all
select '0509' as code, 'ELECTRIC PLUS ' as client union all
select '0611' as code, 'GM FAIRFAX ' as client union all
select '0611' as code, 'GM FAIRFAX ' as client union all
select '0611' as code, 'GM FAIRFAX ' as client union all
select '0712' as code, 'GALLAGHER KAISER ' as client union all
select '0712' as code, 'GALLAGHER KAISER ' as client union all
select '0713' as code, 'GHAFARI ' as client union all
select '0714' as code, 'GM GRAND BLANC CCA' as client union all
select '0718' as code, 'GMCH ' as client union all
select '0718' as code, 'GMCH ' as client union all
select '0718' as code, 'GMCH ' as client union all
select '0718' as code, 'GMCH ' as client union all
select '0718' as code, 'GMCH ' as client union all
select '0718' as code, 'GMCH ' as client union all
select '0718' as code, 'GMCH ' as client union all
select '0718' as code, 'GMCH ' as client union all
select '0718' as code, 'GMCH ' as client union all
select '0718' as code, 'GMCH ' as client union all
select '0718' as code, 'GMCH ' as client union all
select '0910' as code, 'INT. INDUSTRIAL CONTRACTING CORP ' as client union all
select '1211' as code, 'GM LORDSTOWN ' as client union all
select '1211' as code, 'GM LORDSTOWN ' as client union all
select '1212' as code, 'LAKE ERIE ELECTRIC OF TOLEDO ' as client union all
select '1218' as code, 'LANSING BOARD OF WATER & LIGHT ' as client union all
select '1218' as code, 'LANSING BOARD OF WATER & LIGHT ' as client union all
select '1218' as code, 'LANSING BOARD OF WATER & LIGHT ' as client union all
select '1218' as code, 'LANSING BOARD OF WATER & LIGHT ' as client union all
select '1303' as code, 'STATE OF MI' as client union all
select '1303' as code, 'STATE OF MI' as client union all
select '1303' as code, 'STATE OF MI' as client union all
select '1303' as code, 'STATE OF MI' as client union all
select '1308' as code, 'MSU ' as client union all
select '1308' as code, 'MSU ' as client union all
select '1308' as code, 'MSU ' as client union all
select '1308' as code, 'MSU ' as client union all
select '1308' as code, 'MSU ' as client union all
select '1308' as code, 'MSU ' as client union all
select '1308' as code, 'MSU ' as client union all
select '1308' as code, 'MSU ' as client union all
select '1317' as code, 'GM MARION ' as client union all
select '1324' as code, 'GM FLINT METAL CENTER ' as client union all
select '1331' as code, 'MPI RESEARCH' as client union all
select '1331' as code, 'MPI RESEARCH ' as client union all
select '1333' as code, 'MOTOR CITY ELECTRIC ' as client union all
select '1333' as code, 'MOTOR CITY ELECTRIC ' as client union all
select '1333' as code, 'MOTOR CITY ELECTRIC ' as client union all
select '1334' as code, 'GM CCA Memphis' as client union all
select '1404' as code, 'GM PROJECTS (LEGACY)' as client union all
select '1404' as code, 'GM PROJECTS (LEGACY)' as client union all
select '1404' as code, 'GM PROJECTS (LEGACY)' as client union all
select '1404' as code, 'GM PROJECTS (LEGACY)' as client union all
select '1404' as code, 'GM PROJECTS (LEGACY)' as client union all
select '1404' as code, 'GM PROJECTS (LEGACY)' as client union all
select '1405' as code, 'GM CANADA' as client union all
select '1405' as code, 'GM CANADA' as client union all
select '1405' as code, 'GM CANADA' as client union all
select '1405' as code, 'GM CANADA' as client union all
select '1405' as code, 'GM CANADA' as client union all
select '1405' as code, 'GM CANADA' as client union all
select '1405' as code, 'GM CANADA' as client union all
select '1405' as code, 'GM CANADA' as client union all
select '1413' as code, 'NEWKIRK ELECTRIC ' as client union all
select '1413' as code, 'NEWKIRK ELECTRIC ' as client union all
select '1507' as code, 'OAKS CORRECTIONAL FACILITY' as client union all
select '1508' as code, 'OX PAPERBOARD' as client union all
select '1603' as code, 'GM PARMA ' as client union all
select '1603' as code, 'GM PARMA' as client union all
select '1603' as code, 'GM PARMA ' as client union all
select '1603' as code, 'GM PARMA ' as client union all
select '1603' as code, 'GM PARMA ' as client union all
select '1620' as code, 'GM PONTIAC NORTH - POWERTRAIN' as client union all
select '1620' as code, 'GM PONTIAC NORTH - POWERTRAIN' as client union all
select '1620' as code, 'GM PONTIAC NORTH - POWERTRAIN' as client union all
select '1620' as code, 'GM PONTIAC NORTH - POWERTRAIN' as client union all
select '1620' as code, 'GM PONTIAC NORTH - POWERTRAIN' as client union all
select '1620' as code, 'GM PONTIAC NORTH - POWERTRAIN' as client union all
select '1620' as code, 'GM PONTIAC NORTH - POWERTRAIN' as client union all
select '1620' as code, 'GM PONTIAC NORTH - POWERTRAIN' as client union all
select '1620' as code, 'GM PONTIAC NORTH - POWERTRAIN' as client union all
select '1805' as code, 'GM RAMOS ' as client union all
select '1805' as code, 'GM RAMOS ' as client union all
select '1914' as code, 'GM SILAO ' as client union all
select '1921' as code, 'SSOE ' as client union all
select '1921' as code, 'SSOE ' as client union all
select '1928' as code, 'SLP' as client union all
select '1930' as code, 'GM ST. CATHARINES' as client union all
select '1932' as code, 'SUPERIOR ELECTRIC ' as client union all
select '1932' as code, 'SUPERIOR ELECTRIC ' as client union all
select '1932' as code, 'SUPERIOR ELECTRIC ' as client union all
select '1932' as code, 'SUPERIOR ELECTRIC ' as client union all
select '1932' as code, 'SUPERIOR ELECTRIC ' as client union all
select '1932' as code, 'SUPERIOR ELECTRIC ' as client union all
select '1932' as code, 'SUPERIOR ELECTRIC ' as client union all
select '1932' as code, 'SUPERIOR ELECTRIC ' as client union all
select '1932' as code, 'SUPERIOR ELECTRIC ' as client union all
select '1932' as code, 'SUPERIOR ELECTRIC ' as client union all
select '1938' as code, 'SHAW ELECTRIC ' as client union all
select '1938' as code, 'SHAW ELECTRIC ' as client union all
select '2005' as code, 'GM FT. WAYNE ' as client union all
select '2009' as code, 'GM FLINT' as client union all
select '2011' as code, 'GM WARREN TECH CENTER' as client union all
select '2011' as code, 'GM WARREN TECH CENTER' as client union all
select '2013' as code, 'TRIANGLE ELECTRIC ' as client union all
select '2015' as code, 'GM TONAWANDA' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2102' as code, 'PFIZER ' as client union all
select '2304' as code, 'GM WENTZVILLE ' as client union all
select '2314' as code, 'ZOETIS ' as client union all
select '2314' as code, 'ZOETIS ' as client union all
select '2314' as code, 'ZOETIS ' as client union all
select '2315' as code, 'WALBRIDGE ' as client union all
select '2315' as code, 'WALBRIDGE ' as client union all
select '2315' as code, 'WALBRIDGE ' as client union all
select '2315' as code, 'WALBRIDGE ' as client union all
select '2315' as code, 'WALBRIDGE ' as client union all
select '2315' as code, 'WALBRIDGE ' as client union all
select '2318' as code, 'WAGNER INDUSTRIAL CONTRACTING ' as client 
)
insert into Clients (ClientName, ClientCode)
select  distinct client, code  from datas

SELECT TOP (1000) [ClientId]
      ,[ClientName]
      ,[ClientCode]
  FROM [orion.web].[dbo].[Clients]
  
  --commit transaction