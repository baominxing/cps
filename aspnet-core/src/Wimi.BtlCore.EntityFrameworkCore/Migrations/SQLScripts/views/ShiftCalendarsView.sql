CREATE VIEW [dbo].[ShiftCalendarsView]
AS
SELECT t1.Code MachineCode,
       t1.Name MachineName,
       t1.SortSeq MachineSortSeq,
       t2.Id AS ShiftCalendarId,
       t2.ShiftSolutionId,
       t2.ShiftItemId,
       t2.ShiftItemSeq,
       t2.MachineShiftDetailId,
       t2.MachineId,
       t2.StartTime,
       t2.EndTime,
       t2.Duration,
       t2.ShiftDay,
       t2.ShiftDayName,
       t2.ShiftWeek,
       t2.ShiftWeekName,
       t2.ShiftMonth,
       t2.ShiftMonthName,
       t2.ShiftYear,
       t2.ShiftYearName,
       t3.Name AS ShiftSolutionName,
       t4.Name AS ShiftItemName,
       CONVERT(NVARCHAR(10),t2.ShiftDay,23) + ' ' + t4.Name AS MachineShiftDetailName
FROM dbo.Machines AS t1
    INNER JOIN dbo.ShiftCalendars AS t2
        ON t1.Id = t2.MachineId
    INNER JOIN dbo.ShiftSolutions AS t3
        ON t2.ShiftSolutionId = t3.Id
    INNER JOIN dbo.ShiftSolutionItems AS t4
        ON t2.ShiftItemId = t4.Id
WHERE (t1.IsDeleted = 0);