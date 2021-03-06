
declare @payPeriodEnd as Date = '1/4/2019'
declare @payPeriodStart as Date = '12/22/2018'


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
	e.EmployeeId, 
	e.First + ', ' + e.Last as EmployeeName,
	IsNull(sum(regular.hours),0) as regular, 
	IsNull(sum(regular.overtimehours),0) as overtime, 
	IsNull(sum(vacation.hours) + sum(vacation.overtimehours),0)  as Vacation,
	IsNull(sum(sick.hours) + sum(sick.overtimehours),0) as Sick,
	IsNull(sum(personal.hours) + sum(personal.overtimehours),0) as Personal,
	IsNull(sum(holiday.hours) + sum(holiday.overtimehours),0) as Holiday,
	IsNull(sum(excusedNoPay.hours) + sum(excusedNoPay.overtimehours),0) as ExcusedNoPay,
	e.IsExempt,
	sum(te.hours) + sum(te.overtimehours)  as combined
from 
[dbo].Employees e
left outer join [dbo].TimeEntries te
	on e.EmployeeId = te.EmployeeId
left outer join dbo.TimeEntries regular
	on te.TimeEntryId = regular.TimeEntryId
	and te.TaskId not in (@vacationRowId, @sickRowId, @personalTime, @holidayTime, @excusedWithoutPay)
left outer join dbo.TimeEntries vacation
	on te.TimeEntryId = vacation.TimeEntryId
	and vacation.TaskId = @vacationRowId
left outer join dbo.TimeEntries sick
	on te.TimeEntryId = sick.TimeEntryId
	and sick.TaskId = @sickRowId
left outer join dbo.TimeEntries personal
	on te.TimeEntryId = personal.TimeEntryId
	and personal.TaskId = @personalTime
left outer join dbo.TimeEntries holiday
	on te.TimeEntryId = holiday.TimeEntryId
	and holiday.TaskId = @holidayTime
left outer join dbo.TimeEntries excusedNoPay
	on te.TimeEntryId = excusedNoPay.TimeEntryId
	and excusedNoPay.TaskId = @excusedWithoutPay
	

where 
	 te.Date >= @payPeriodStart
	and te.Date <= @payPeriodEnd
group by e.EmployeeId, 
	e.First + ', ' + e.Last,
	e.IsExempt
	