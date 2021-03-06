begin transaction
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [JobId]
      ,[JobCode]
      ,[JobName]
      ,[TaskCategoryId]
      ,[ClientId]
      ,[SiteId]
      ,[TargetHours]
  FROM [orion.web].[dbo].[Jobs]

  ;with datas as
  (
select '0221' as clientCode , '0601' as jobCode, 'GM M5 Project  (Arlington)' as JobName, 'GM Arlington - Arlington, TX' as Site union all
select '0221' as clientCode , '0602' as jobCode, 'GM M5 Project  (Flint)' as JobName, 'GM Flint - Flint, MI' as Site union all
select '0221' as clientCode , '0603' as jobCode, 'GM M5 Project  (Ft. Wayne)' as JobName, 'GM Ft. Wayne - Ft. Wayne, IN' as Site union all
select '0221' as clientCode , '0604' as jobCode, 'GM M5 Project  (Silao)' as JobName, 'GMM Silao - Silao, MX' as Site union all
select '0221' as clientCode , '0605' as jobCode, 'GM Silao M5 Expanded Scope' as JobName, 'GMM Silao - Silao, MX' as Site union all
select '0336' as clientCode , '0400' as jobCode, 'GM Wentzville 20 Bay Addition' as JobName, 'GM Wentzville - Wentzville, MO' as Site union all
select '0336' as clientCode , '0500' as jobCode, 'GM Wentzville Stamping Addition (Elect.)' as JobName, 'GM Wentzville - Wentzville, MO' as Site union all
select '0336' as clientCode , '0700' as jobCode, 'Parma Assembly Press Arc Flash' as JobName, 'GM Parma - Cleveland, OH' as Site union all
select '0336' as clientCode , '0900' as jobCode, 'GM Bowling Green Arc Flash Study' as JobName, 'GM Bowling Green - Bowling Green, KY' as Site union all
select '0338' as clientCode , '0200' as jobCode, 'GM Renaissance' as JobName, 'GM Renaissance - Detroit, MI' as Site union all
select '0417' as clientCode , '4000' as jobCode, 'Generator Design' as JobName, 'Detroit Diesel - Redford, MI' as Site union all
select '0508' as clientCode , '0100' as jobCode, 'GM Tonawanda' as JobName, 'GM Tonawanda - Buffalo, NY' as Site union all
select '0509' as clientCode , '0100' as jobCode, 'GM Ft. Wayne New Chiller' as JobName, 'GM Ft. Wayne - Ft. Wayne, IN' as Site union all
select '0611' as clientCode , '1400' as jobCode, 'Arc Flash Hazard Calculations' as JobName, 'GM Fairfax - Kansas City, KS' as Site union all
select '0611' as clientCode , '1500' as jobCode, 'Arc Flash Hazard Calculations' as JobName, 'GM Fairfax - Kansas City, KS' as Site union all
select '0611' as clientCode , '1800' as jobCode, '15kV Switching Revision Investigation' as JobName, 'GM Fairfax - Kansas City, KS' as Site union all
select '0712' as clientCode , '0600' as jobCode, 'GM Bowling Green' as JobName, 'GM Bowling Green - Bowling Green, KY' as Site union all
select '0712' as clientCode , '0700' as jobCode, 'GM Bowling Green Manifold Bldg& Swhse' as JobName, 'GM Bowling Green - Bowling Green, KY' as Site union all
select '0713' as clientCode , '1300' as jobCode, 'GM Silao Improvement Program' as JobName, 'GMM Silao - Silao, MX' as Site union all
select '0714' as clientCode , '0200' as jobCode, 'Short Circuit, Protective Device & Arc Flash Study' as JobName, 'GM CCA Grand Blanc - Grand Blanc, MI' as Site union all
select '0718' as clientCode , '0700' as jobCode, 'Short Circuit & Arc Flash & Load Study' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '0718' as clientCode , '0800' as jobCode, 'DEAC Expansion' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '0718' as clientCode , '0900' as jobCode, '480V Busway Bid Pkg & Installation Design' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '0718' as clientCode , '1000' as jobCode, 'Arc Flash & Short Circuit Studies' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '0718' as clientCode , '1100' as jobCode, 'DEAC Program' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '0718' as clientCode , '1200' as jobCode, 'Sub 15 Design & Sub 3 Removal' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '0718' as clientCode , '1201' as jobCode, 'Substation Pre-Purchase Package' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '0718' as clientCode , '1300' as jobCode, '480V Busway Pre-Purchase Pkg., Installation Design & Studies' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '0718' as clientCode , '1400' as jobCode, 'Upgrade Exit Egress Night Light Design Installation' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '0718' as clientCode , '1500' as jobCode, 'Emergency Generator Replacement Design' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '0718' as clientCode , '1600' as jobCode, 'Air Compressor Arc Flash Hazard & Short Circuit Study' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '0910' as clientCode , '0200' as jobCode, 'GM Bowling Green' as JobName, 'GM Bowling Green - Bowling Green, KY' as Site union all
select '1211' as clientCode , '1250' as jobCode, 'Chiller Design (Not Opened Yet)' as JobName, 'GM Lordstown - Lordstown, OH' as Site union all
select '1211' as clientCode , '1500' as jobCode, 'Sub & Protective Device Data Book Update' as JobName, 'GM Lordstown - Lordstown, OH' as Site union all
select '1212' as clientCode , '0300' as jobCode, 'ABIV Process Equipment Arc Flash Study' as JobName, 'Lake Erie Electric - OH' as Site union all
select '1218' as clientCode , '0101' as jobCode, 'Eckert, Units 4 & 5' as JobName, 'LBW&L - Lansing, MI' as Site union all
select '1218' as clientCode , '0102' as jobCode, 'Eckert, Units 1, 2, 3 & 6' as JobName, 'Lansing, MI - Lansing, MI' as Site union all
select '1218' as clientCode , '0103' as jobCode, 'Erickson' as JobName, 'Lansing, MI - Lansing, MI' as Site union all
select '1218' as clientCode , '0104' as jobCode, 'Field Work' as JobName, 'Lansing, MI - Lansing, MI' as Site union all
select '1303' as clientCode , '5100' as jobCode, 'Arc Flash Mitigation Project, Phase I (MDOC) ' as JobName, 'Various Locations MI - Various Locations MI' as Site union all
select '1303' as clientCode , '5150' as jobCode, 'Jackson State Bldg.  Weather Intrusion Remediation (DTMB) ' as JobName, 'Jackson, MI - Jackson, MI' as Site union all
select '1303' as clientCode , '5160' as jobCode, 'Jackson State Bldg. Weather Intrusion Remediation 500,600,700 (DTMB) ' as JobName, 'Jackson, MI - Jackson, MI' as Site union all
select '1303' as clientCode , '5200' as jobCode, 'Cotton Correctional Facility (MDOC) ' as JobName, 'Jackson, MI - Jackson, MI' as Site union all
select '1308' as clientCode , '0000' as jobCode, 'Dairy Research Pri Revs' as JobName, 'East Lansing, MI - East Lansing, MI' as Site union all
select '1308' as clientCode , '1911' as jobCode, 'Arc Flash Study' as JobName, 'East Lansing, MI - East Lansing, MI' as Site union all
select '1308' as clientCode , '1914' as jobCode, 'Arc Flash Study Repair' as JobName, 'East Lansing, MI - East Lansing, MI' as Site union all
select '1308' as clientCode , '1917' as jobCode, 'NSCLFRIB Primary Coord. for CL Fuses' as JobName, 'East Lansing, MI - East Lansing, MI' as Site union all
select '1308' as clientCode , '1950' as jobCode, 'IM Sports Circle New Electrical Vault' as JobName, 'East Lansing, MI - East Lansing, MI' as Site union all
select '1308' as clientCode , '1970' as jobCode, 'Arc Flash Calculations Phase III' as JobName, 'East Lansing, MI - East Lansing, MI' as Site union all
select '1308' as clientCode , '2000' as jobCode, 'Electrical Distribution  General Assistance' as JobName, 'East Lansing, MI - East Lansing, MI' as Site union all
select '1308' as clientCode , '2102' as jobCode, 'NSCL Arc Flash & Device Coordination' as JobName, 'East Lansing, MI - East Lansing, MI' as Site union all
select '1317' as clientCode , '1800' as jobCode, 'New Crane Power Distribution' as JobName, 'Marion, IN - Marion, IN' as Site union all
select '1324' as clientCode , '1500' as jobCode, 'Substation 4 Upgrade' as JobName, 'Flint, MI - Flint, MI' as Site union all
select '1331' as clientCode , '0900' as jobCode, 'Sub R to Sub P Incoming Line Switchgear' as JobName, 'Kalamazoo Research - Kalamazoo, MI' as Site union all
select '1331' as clientCode , '1300' as jobCode, 'Clinical Pathology Renovation' as JobName, '? - MI' as Site union all
select '1333' as clientCode , '0500' as jobCode, 'Design Center Parking Deck' as JobName, 'GM Warren Tech Center - Warren, MI' as Site union all
select '1333' as clientCode , '0600' as jobCode, 'GM Pontiac Arc Flash' as JobName, 'GM Pontiac - Pontiac, MI' as Site union all
select '1333' as clientCode , '0800' as jobCode, 'GM Warren Tech Center SDL' as JobName, 'GM Warren - Warren, MI' as Site union all
select '1334' as clientCode , '0100' as jobCode, 'SC&PDC and Arc Flash Hazard Study (Ferndale Elec) ' as JobName, 'GM CCA Memphis - Memphis, TN' as Site union all
select '1404' as clientCode , '8910' as jobCode, 'Chiller Project' as JobName, 'GM Ft. Wayne - Ft. Wayne, IN' as Site union all
select '1404' as clientCode , '8920' as jobCode, 'Electrical Modernization' as JobName, 'GM Defiance - Defiance, OH' as Site union all
select '1404' as clientCode , '8921' as jobCode, 'Electrical Modernization Distribution' as JobName, 'GM Defiance - Defiance, OH' as Site union all
select '1404' as clientCode , '8930' as jobCode, 'Lighting Design and Load Study' as JobName, 'GM Bay City - Bay City, MI' as Site union all
select '1404' as clientCode , '8940' as jobCode, 'Temporary Power System Metering' as JobName, 'GM Bay City - Bay City, MI' as Site union all
select '1404' as clientCode , '8950' as jobCode, 'Typ Swyd Swhse Exhibits' as JobName, 'N/A - Jackson, MI' as Site union all
select '1405' as clientCode , '0700' as jobCode, 'Short Circuit and Arc Flash Studies' as JobName, 'GM Oshawa Car Assembly Plant - Oshawa, ON' as Site union all
select '1405' as clientCode , '0701' as jobCode, 'Short Circuit and Arc Flash Studies' as JobName, 'GM Canadian Technical Centre - Markham, ON' as Site union all
select '1405' as clientCode , '0702' as jobCode, 'Short Circuit and Arc Flash Studies' as JobName, 'GM Canadian Headquarters Building - Oshawa, ON' as Site union all
select '1405' as clientCode , '0703' as jobCode, 'Short Circuit and Arc Flash Studies' as JobName, 'GM Cold Weather Test Development Centre - Kapuskasing, ON' as Site union all
select '1405' as clientCode , '0704' as jobCode, 'Short Circuit and Arc Flash Studies' as JobName, 'GM Montreal Parts Distribution Centre - Montreal, QC' as Site union all
select '1405' as clientCode , '0705' as jobCode, 'Short Circuit and Arc Flash Studies' as JobName, 'GM Woodstock Parts Distribution Centre - Woodstock, ON' as Site union all
select '1405' as clientCode , '0706' as jobCode, 'Short Circuit and Arc Flash Studies' as JobName, 'GM Edmonton Parts Distribution Centre - Edmonton, AB' as Site union all
select '1405' as clientCode , '0707' as jobCode, 'Short Circuit and Arc Flash Studies' as JobName, 'GM Vancouver Parts Distribution Centre - Langley Twp, British Columbia' as Site union all
select '1413' as clientCode , '0200' as jobCode, 'GM Components Holding Gamma Room AF' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '1413' as clientCode , '0300' as jobCode, 'Pfizer Bldg. 541 Break Settings & Arc Flash Calculations' as JobName, 'Pfizer Bldg. 541 - Portage, MI' as Site union all
select '1507' as clientCode , '0100' as jobCode, 'Installation & Removal of Power Meter (Master Electric) ' as JobName, 'Oaks Correctional Facility - Manistee, MI' as Site union all
select '1508' as clientCode , '0100' as jobCode, 'Pekin, IL Facility Arc Flash Study (Lakeshore Elec) ' as JobName, 'Ox Paperboard - Constantine, MI' as Site union all
select '1603' as clientCode , '2300' as jobCode, 'Short Circuit & Arc Flash Study' as JobName, 'GM Parma - Parma, OH' as Site union all
select '1603' as clientCode , '2600' as jobCode, 'Primary Protection Eval' as JobName, 'GM Parma - Parma, OH' as Site union all
select '1603' as clientCode , '2700' as jobCode, 'Siemens Main Motor Drive Panel Arc Flash & Coord Study (Schuler Group) ' as JobName, 'GM Parma - Parma, OH' as Site union all
select '1603' as clientCode , '2800' as jobCode, 'SC&C Update' as JobName, 'GM Parma - Parma, OH' as Site union all
select '1603' as clientCode , '2900' as jobCode, 'C90 Tool Room Arc Flash (Conti) ' as JobName, 'GM Parma - Parma, OH' as Site union all
select '1620' as clientCode , '0200' as jobCode, 'Test Wing 3 Addition' as JobName, 'GM Pontiac North - Pontiac, MI' as Site union all
select '1620' as clientCode , '0201' as jobCode, 'Building C, Substation 13T Firm Capacity Increase' as JobName, 'GM Pontiac North - Pontiac, MI' as Site union all
select '1620' as clientCode , '0501' as jobCode, 'Aluminum Bus Evaluation (TEMPEST) ' as JobName, 'GM Pontiac North - Pontiac, MI' as Site union all
select '1620' as clientCode , '0502' as jobCode, 'Ground Fault CT Evaluation (TEMPEST) ' as JobName, 'GM Pontiac North - Pontiac, MI' as Site union all
select '1620' as clientCode , '0503' as jobCode, 'VFD Evaluation and Approval (TEMPEST) ' as JobName, 'GM Pontiac North - Pontiac, MI' as Site union all
select '1620' as clientCode , '0504' as jobCode, 'Engineering Support Services (TEMPEST) ' as JobName, 'GM Pontiac North - Pontiac, MI' as Site union all
select '1620' as clientCode , '1400' as jobCode, 'Racing Cell Breaker Study' as JobName, 'GM Pontiac North - Pontiac, MI' as Site union all
select '1620' as clientCode , '1500' as jobCode, 'Temp Power Sys Metering & Reporting' as JobName, 'GM Pontiac North - Pontiac, MI' as Site union all
select '1620' as clientCode , '1600' as jobCode, 'Fuel Cell Lab Expansion (Superior) ' as JobName, 'GM Pontiac North - Pontiac, MI' as Site union all
select '1805' as clientCode , '0600' as jobCode, 'Primary Electrical Service Addition' as JobName, 'GMM Ramos - Ramos, MX' as Site union all
select '1805' as clientCode , '0700' as jobCode, 'New Ramos Paint Shop Electrical Engineering Services (GK)' as JobName, 'GMM Ramos - Ramos, MX' as Site union all
select '1914' as clientCode , '0700' as jobCode, 'Ground Fault Protection Setting Revisions' as JobName, 'GMM Silao - Silao, MX' as Site union all
select '1921' as clientCode , '0800' as jobCode, 'Elect. Feeders to Induction Heaters' as JobName, 'GM Flint - Flint, MI' as Site union all
select '1921' as clientCode , '1100' as jobCode, 'Silao GRx Studies' as JobName, 'GMM Silao - Silao, MX' as Site union all
select '1928' as clientCode , '0900' as jobCode, 'Switch House Design (Superior) ' as JobName, 'GMM San Luis Potosi - San Luis Potosi, MX' as Site union all
select '1930' as clientCode , '0300' as jobCode, 'Propulsion Plant Short Circuit and Arc Flash Studies' as JobName, 'GM St. Catharines - St. Catharines, ON' as Site union all
select '1932' as clientCode , '2501' as jobCode, 'GM Toledo  FW1 & ABIV Studies' as JobName, 'GM Toledo - Toledo, OH' as Site union all
select '1932' as clientCode , '2502' as jobCode, 'GM Toledo FW1 & ABIV Switchgear Pre-purchase Package' as JobName, 'GM Toledo - Toledo, OH' as Site union all
select '1932' as clientCode , '2503' as jobCode, 'GM Toledo FW1 & ABIV Prmary Electric Upgrade' as JobName, 'GM Toledo - Toledo, OH' as Site union all
select '1932' as clientCode , '2505' as jobCode, 'GM Toledo BP 2000 Arc Flash Study' as JobName, 'GM Toledo - Toledo, OH' as Site union all
select '1932' as clientCode , '2900' as jobCode, 'GM Bowling Green Arc Flash Study' as JobName, 'GM Bowling Green - Bowling Green, KY' as Site union all
select '1932' as clientCode , '3000' as jobCode, 'GM Bowling Green Arc Flash Study' as JobName, 'GM Bowling Green - Bowling Green, KY' as Site union all
select '1932' as clientCode , '3300' as jobCode, 'GMCH Sub 13A & Chillers' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '1932' as clientCode , '3400' as jobCode, 'San Luis Potosi Body Shop' as JobName, 'GMM San Luis Potosi - San Luis Potosi, MX' as Site union all
select '1932' as clientCode , '3500' as jobCode, 'Trim Shop Electrical Studies' as JobName, 'GM Flint - Flint, MI' as Site union all
select '1932' as clientCode , '3600' as jobCode, 'GMCH FAX Facility Drawings' as JobName, 'GMCH - Wyoming, MI' as Site union all
select '1938' as clientCode , '0100' as jobCode, 'GM Warren RSB Renovation' as JobName, 'GM Warren Tech Center - Warren, MI' as Site union all
select '1938' as clientCode , '0300' as jobCode, 'GM Warren Tech Center Short Circuit, Device Coord., & Arc Flash Studies' as JobName, 'GM Warren Tech Center - Warren, MI' as Site union all
select '2005' as clientCode , '1300' as jobCode, 'Ground Fault Protection Setting Revisions' as JobName, 'GM Ft. Wayne - Ft. Wayne, IN' as Site union all
select '2009' as clientCode , '1300' as jobCode, 'MCC Replacement Installation Design & Pre-Purchase Pkg' as JobName, 'GM Flint - Flint, MI' as Site union all
select '2011' as clientCode , '1500' as jobCode, '480V CIRCUIT DESIGN (EXOTIC) ' as JobName, 'GM Warren Tech Center - Warren, MI' as Site union all
select '2011' as clientCode , '1600' as jobCode, '7000 Building Consolidation (SMITH GROUP) ' as JobName, 'GM Warren Tech Center - Warren, MI' as Site union all
select '2013' as clientCode , '0300' as jobCode, 'GM Warren Arc Flash Study' as JobName, 'GM Warren Tech Center - Warren, MI' as Site union all
select '2015' as clientCode , '0400' as jobCode, 'Gen V HD Renovations (The State Group) ' as JobName, 'GM Tonawanda - Buffalo, NY' as Site union all
select '2102' as clientCode , '9804' as jobCode, 'Sub T Replacement' as JobName, 'Portage Road Site - Kalamazoo, MI' as Site union all
select '2102' as clientCode , '9805' as jobCode, 'Chilled Water Capacity, Bldgs 315 & 188' as JobName, 'Bldgs 315 & 318 - Kalamazoo, MI' as Site union all
select '2102' as clientCode , '9807' as jobCode, 'Sub AL-MV Secondary Swgr Replacement' as JobName, 'Portage Road Site - Kalamazoo, MI' as Site union all
select '2102' as clientCode , '9810' as jobCode, 'Sub AZ Feeder Breakers' as JobName, 'Portage Road Site - Kalamazoo, MI' as Site union all
select '2102' as clientCode , '9811' as jobCode, 'Substations A and AC' as JobName, 'Portage Road Site - Kalamazoo, MI' as Site union all
select '2102' as clientCode , '9812' as jobCode, 'Transient Voltage Excursions' as JobName, 'Portage Road Site - Kalamazoo, MI' as Site union all
select '2102' as clientCode , '9813' as jobCode, '2018 Electrical Engineering Support' as JobName, 'Portage Road Site - Kalamazoo, MI' as Site union all
select '2102' as clientCode , '9814' as jobCode, 'Substation N Capacity Expansion' as JobName, 'Portage Road Site - Kalamazoo, MI' as Site union all
select '2102' as clientCode , '9815' as jobCode, 'AOV Work Center Provide Settings' as JobName, 'Portage Road Site - Kalamazoo, MI' as Site union all
select '2102' as clientCode , '9816' as jobCode, 'Sub AUT1' as JobName, 'Portage Road Site - Kalamazoo, MI' as Site union all
select '2102' as clientCode , '9817' as jobCode, 'Sub V Breaker Settings' as JobName, 'Portage Road Site - Kalamazoo, MI' as Site union all
select '2102' as clientCode , '9818' as jobCode, 'SUB E LOAD SEGREGATION' as JobName, 'Portage Road Site - Kalamazoo, MI' as Site union all
select '2304' as clientCode , '2100' as jobCode, 'Press Arc Flash' as JobName, 'GM Wentzville - Wentzville, MO' as Site union all
select '2314' as clientCode , '0300' as jobCode, 'Bldg. 248 Short-Term Expansion' as JobName, 'Bldg 248 - Kalamazoo, MI' as Site union all
select '2314' as clientCode , '0400' as jobCode, 'Breaker Settings in B300 Generator Swgr' as JobName, 'Zoetis - Kalamazoo, MI' as Site union all
select '2314' as clientCode , '0500' as jobCode, 'B214 Substation Upgrade' as JobName, 'Bldg 214 - Kalamazoo, MI' as Site union all
select '2315' as clientCode , '0200' as jobCode, 'GM Arlington Paint Shop Expansion' as JobName, 'GM Arlington - Arlington, TX' as Site union all
select '2315' as clientCode , '0300' as jobCode, 'GM Ft. Wayne Paint Shop Expansion' as JobName, 'GM Ft. Wayne - Ft. Wayne, IN' as Site union all
select '2315' as clientCode , '0400' as jobCode, 'GM Silao Paint Shop Expansion' as JobName, 'GMM Silao - Silao, MX' as Site union all
select '2315' as clientCode , '0500' as jobCode, 'GM Bowling Green Paint Shop Expansion' as JobName, 'GM Bowling Green - Bowling Green, KY' as Site union all
select '2315' as clientCode , '0600' as jobCode, 'GM Ramos Arizpe -  CSS Engine Plant' as JobName, 'GMM Ramos - Ramos, MX' as Site union all
select '2315' as clientCode , '0700' as jobCode, 'GM SLP CMA -Arc Flash Study' as JobName, 'GMM San Luis Potosi - San Luis Potosi, MX' as Site union all
select '2318' as clientCode , '0100' as jobCode, 'GM Bowling Green (NOT OPEN YET)' as JobName, 'GM Bowling Green - Bowling Green, KY' as Site
),
addl as (
select distinct c.ClientId, jobCode, SiteID, datas.clientCode, SiteName from datas
inner join  Sites on
Sites.SiteName = datas.Site
inner join Clients  c
on c.ClientCode = datas.clientCode
)

insert into Jobs (JobCode, JobName, TaskCategoryId, ClientId, SiteId, TargetHours)
select d.jobCode, JobName, 1, addl.ClientId, addl.SiteID, 0 
from datas d
inner join addl
on d.jobCode = addl.jobCode
and d.Site = addl.SiteName
and d.clientCode = d.clientCode


/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [JobId]
      ,[JobCode]
      ,[JobName]
      ,[TaskCategoryId]
      ,[ClientId]
      ,[SiteId]
      ,[TargetHours]
  FROM [orion.web].[dbo].[Jobs]
  commit transaction
  --rollback transaction

  