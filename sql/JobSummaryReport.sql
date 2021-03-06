use [orion.web]
go

declare @WeekEnd as Date = '1/4/2019'
declare @WeekStart as Date = '12/22/2018'

declare @vacationRowId as int
select @vacationRowId = JobTaskId from dbo.JobTasks where ShortName = '87 - Vacation'

declare @sickRowId as int
select @sickRowId = JobTaskId from dbo.JobTasks where ShortName = '85 - Sick Time'

declare @personalTime as int
select @personalTime = JobTaskId from dbo.JobTasks where ShortName = '83 - Personal Time'

declare @holidayTime as int
select @holidayTime = JobTaskId from dbo.JobTasks where ShortName = '88 - Holiday'

declare @excusedWithoutPay as int
select @excusedWithoutPay = JobTaskId from dbo.JobTasks where ShortName = '89 - Excused WITHOUT Pay'

Select 

 	min(Convert(varchar(10),Isnull(te.Date,@WeekStart), 101)) as  PeriodStart, 
	max(Convert(varchar(10),isnull( te.Date,@WeekEnd),101)) as PeriodEnd,    
	e.First + ', ' + e.Last  as  EmployeeName, 
	c.clientcode + '-' + j.JobCode  as  JobCode, 
	j.JobName as  JobName,
	c.ClientName as  ClientName, 
    s.SiteName as  SiteName, 
	jt.ShortName as TaskName, 
	tc.Name as TaskCategory, 
	isnull(sum(te.hours),0) as Regular, 
    isnull(sum(te.overtimehours),0) as Overtime, 
	isnull(sum(te.hours),0) + isnull(sum(te.overtimehours),0) as  Combined

from 
    dbo.Jobs j
    inner join dbo.Clients c
        on c.ClientId = j.ClientId
	left outer join [dbo].TimeEntries te
		on j.JobId = te.JobId
    left outer join [dbo].Employees e
		on e.EmployeeId = te.EmployeeId
    inner join dbo.[Sites] s
	    on j.SiteId = s.SiteID
	left outer join dbo.JobTasks jt
		on jt.JobTaskId = te.TaskId
    inner join dbo.TaskCategories tc
	    on tc.TaskCategoryId = jt.TaskCategoryId
--where 
	--(@JobId is null Or te.JobId = @JobId) and
	--te.Date >= @WeekStart and te.Date <= @WeekEnd
group by 
	tc.Name,s.SiteName, 
	c.clientcode + '-' + j.JobCode, 
	e.First + ', ' + e.Last , 
	c.clientcode + 
	j.JobCode  , 
	j.JobName, 
	c.ClientName , 
	jt.ShortName, 
	j.JobId
	