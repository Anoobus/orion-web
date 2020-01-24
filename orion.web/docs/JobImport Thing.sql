IF OBJECT_ID('tempdb.dbo.#JobData', 'U') IS NOT NULL
  DROP TABLE #JobData; 


CREATE TABLE #JobData
(
	clientCode varchar(20),
	JobCode  varchar(10),
	ClientName VARCHAR (100),
	SiteName varchar(100),
	JobName varchar(100),
	LocationName varchar(100),
)
Insert into #JobData (ClientCode,JobCode,ClientName,SiteName,JobName, LocationName)
values
('0213','0700.1','Superior Electric Greak Lakes','GM Bowling Green','Arc Flash Study','Bowling Green, KY'),
('0227','0100.1','The State Group','Bell Canada Detroit','480V ATS Replacement','Detroit, MI'),
('0302','4300','ICC','GM Arlington','T1xx Body Shop Project Opportunity Rapid Chargers','Arlington, TX'),
('0419','0800','GM','GM DHAM','NXT Modernization Concept Design','Detroit, MI'),
('0419','0810','GM','GM DHAM','Utility Study for Steam and Natural Gas','Detroit, MI'),
('0423','0110','GM','GM Dayton','Low Voltage Shop Drawing Review','Dayton, OH'),
('0423','0120','GM','GM Dayton','Medium Voltage Shop Drawing Review','Dayton, OH'),
('0423','0200.1','GM','GM Dayton','Project Nora Design and Studies','Dayton, OH'),
('0424','0100.2','GM','GM Defiance','Substation B Replacement','Defiance, OH'),
('0424','0200.2','GM','GM Defiance','Lineup A 15KV Switchgear Equipment Pre-purchase Pkg.','Defiance, OH'),
('0625','0501','GM','GM Flint','Site Wide SC&C (Continued) ','Flint, MI'),
('0718','1904','GMCH','GMCH','Substation 7 Equipment Replacement','Wyoming, MI'),
('0718','1905','GMCH','GMCH','Substation 7 Busway and Cable Tray Installation Design','Wyoming, MI'),
('0718','2500','GMCH','GMCH','Substation 10 – New 2400V Air Compressor Studies','Wyoming, MI'),
('0718','2860','GMCH','GMCH','Sub 5 & 15 - Extra Engineering Services','Wyoming, MI'),
('0718','3000','GMCH','GMCH','Furnace Loads AF and LS Update','Wyoming, MI'),
('0718','3100','GMCH','GMCH','Arc Flash and Load Study Update for 1 Drive Line Robot','Wyoming, MI'),
('0818','0100.3','Ghafari','Mazda Toyota Huntsville','15KV Isolation Switch Design','Huntsville, AL'),
('1318','0100.4','DTMB','Macomb Correctional Facility','Arc Flash Calculations for Two (2) locations ','Lenox, MI'),
('1404','8951','GM','Office','GM Sustainable Workplaces - Electrical Specification 16230 Updates','Jackson, MI'),
('1404','8960','GM','-','GM Emergency Technical Support Engineering Services','-'),
('1405','0700.2','GM Canada','GM Oshawa Car Assembly Plant','GM Oshawa Car Assembly Plant','Oshawa, ON'),
('1405','0701','GM Canada','GM Canadian Technical Centre','GM Canadian Technical Centre','Markham, ON'),
('1405','0702','GM Canada','GM Canadian Headquarters Building','GM Canadian Headquarters Building','Oshawa, ON'),
('1405','0703','GM Canada','GM Cold Weather Test Development Centre','GM Cold Weather Test Development Centre','Kapuskasing, ON'),
('1405','0704','GM Canada','GM Montreal Parts Distribution Centre','GM Montreal Parts Distribution Centre','Montreal, QC'),
('1405','0705','GM Canada','GM Woodstock Parts Distribution Centre','GM Woodstock Parts Distribution Centre','Woodstock, ON'),
('1405','0706','GM Canada','GM Edmonton Parts Distribution Centre','GM Edmonton Parts Distribution Centre','Edmonton, AB'),
('1405','0707','GM Canada','GM Vancouver Parts Distribution Centre','GM Vancouver Parts Distribution Centre','Langley Twp, British Columbia'),
('1408','3400','GM','GM Milford','Building Substation 25 Feasibility Study','Milford, MI'),
('1408','3410','GM','GM Milford','Building 32 Substation Pre-purchase Package','Milford, MI'),
('1408','3420','GM','GM Milford','Building 70 Substation Pre-purchase Package','Milford, MI'),
('1504','1700.1','GM','GM Lake Orion','Co-Generation Reverse Power Modifications','Lake Orion, MI'),
('1504','1800','GM','GM Lake Orion','Welder Utilities Study','Lake Orion, MI'),
('1504','2000.1','GM','GM Lake Orion','Arc Flash Study','Lake Orion, MI'),
('1603','2600','GM','GM Parma','Primary Protection Evaluation','Parma, OH'),
('1603-schuler','2700','Schuler Group','GM Parma','Siemens Main Motor Drive Panel Arc Flash & Coord Study','Parma, OH'),
('1614','0600','GM','GM Toledo','ET1B Electrical Study','Toledo, OH'),
('1620-mce','1700.2','Motor City Electric','GM Pontiac GPS','Arc Flash Study','Pontiac, MI'),
('1620','1801','GM','GM Pontiac GPS','CEP Substation Replacement Design','Pontiac, MI'),
('1620','1802','GM','GM Pontiac GPS','CEP Substation Replacement Studies','Pontiac, MI'),
('1620','1900','GM','GM Pontiac GPS','Tempest Switchgear Replacement','Pontiac, MI'),
('1620','2000.2','GM','GM Pontiac GPS','New Bay 3 Crane Power Distribution','Pontiac, MI'),
('1620','2100','GM','GM Pontiac GPS','Substation D1-E Pre-Purchase Package','Pontiac, MI'),
('1620','2300','GM','GM Pontiac GPS','Substation 13D Pre-Purchase Package and Concept Design','Pontiac, MI'),
('1805','0700.3','Gallagher-Kaiser','GMM Ramos','New Paint Shop Electrical Engineering Services','Ramos, MX'),
('2004','1100','GM','GM Pontiac GPS','Test Stand Ungrounded Power System-Ground Fault Indication','Pontiac, MI'),
('2009','1500','Conti','GM Flint','K2 to LOC Conversion Studies','Flint, MI'),
('2102','9815','PFIZER','Portage Road Site','AOV Work Center Provide Settings','Kalamazoo, MI'),
('2102','9820','PFIZER','Portage Road Site','Electrical Engineering Consultation - Sub F and Sub H Ground Return Scheme','Kalamazoo, MI'),
('2102','9821','PFIZER','-','2020 Electrical Engineering Support','-'),
('2304','2400','GM','GM Wentzville','31XX2 Welder Utilities Planning Study','Wentzville, MS'),
('2314','0300','ZOETIS','Bldg 248','Bldg. 248 Short-Term Expansion','Kalamazoo, MI'),
('2314','0700.4','ZOETIS','B214','Substation X1 Replacement - B214','Kalamazoo, MI')

--select 'primeing the pump'
--while @@ROWCOUNT > 0
--begin
--delete from  [orion.web].[dbo].[Sites] where [SiteID] in (SELECT Max( [SiteID])
--  FROM [orion.web].[dbo].[Sites]
--  group by  [SiteName]
--	  having count(*) > 1)
--END

--select 'primeing the pump'
--while @@ROWCOUNT > 0
--begin
--delete from  [orion.web].[dbo].[Clients] where clientid in (SELECT Max( [ClientId])
--  FROM [orion.web].[dbo].[Clients]
--  group by  [ClientName]
--      ,[ClientCode]
--	  having count(*) > 1)
--END




Insert into  [orion.web].dbo.Clients (ClientName, ClientCode)
select distinct  source.clientName , source.clientCode 
from #JobData as  source
left outer join [orion.web].dbo.Clients c
on c.ClientCode = source.clientcode
where c.clientId is null

Insert into  [orion.web].dbo.sites (sitename)
select distinct source.SiteName + ' (' +  source.locationName +')'
from #JobData source
left outer join [orion.web].dbo.Sites s
on s.Sitename =  source.SiteName + ' (' +  source.locationName +')'
where s.siteId is  null

insert into [orion.web].dbo.[Jobs] ( JobCode, JobName, ClientId, SiteId, TargetHours, JobStatusId, EmployeeId)
select 
source.JobCode,
source.JobName, 
c.ClientId,
s.SiteId,
0 as targethours, 1 as JobStatusId , 2 as ExmployeeId
from #JobData source
inner join [orion.web].dbo.Clients c
on c.ClientCode = source.clientcode
inner join [orion.web].dbo.Sites s
on s.Sitename = source.SiteName + ' (' +  source.locationName +')'
left outer join [orion.web].dbo.Jobs j
on j.JobCode = source.jobcode
where j.JobId is null
