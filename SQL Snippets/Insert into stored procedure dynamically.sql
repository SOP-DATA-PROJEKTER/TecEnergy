DECLARE @RoomId uniqueidentifier;
DECLARE @CurrentDate datetime = CONVERT(datetime, CONVERT(date, GETDATE()));
DECLARE @PreviousDate datetime = CONVERT(datetime, CONVERT(date, @CurrentDate - 1));

DECLARE roomCursor CURSOR FOR
    SELECT Id
    FROM Rooms;

OPEN roomCursor;

FETCH NEXT FROM roomCursor INTO @RoomId;

WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC CalculateAndInsertDailyAccumulatedValue
        @RoomId = @RoomId,
        @StartDate = @PreviousDate,
        @EndDate = @CurrentDate;

    FETCH NEXT FROM roomCursor INTO @RoomId;
END

CLOSE roomCursor;
DEALLOCATE roomCursor;





--SELECT * FROM DailyAccumulated order by DateTime;
--delete from DailyAccumulated;




---- '4E10F56A-147E-4541-ADE4-08DBEF4BCA36' wrong data
---- 'DDADA893-0D4C-41E6-F1C7-08DBFA4515B7'

--select * from Rooms

--DECLARE @CurrentDate datetime = CONVERT(datetime, CONVERT(date, GETDATE() + 1));
--DECLARE @PreviousDate datetime = CONVERT(datetime, CONVERT(date, @CurrentDate - 1));

--SELECT TOP(1) ED_End.AccumulatedValue
--        FROM EnergyData ED_End
--        JOIN EnergyMeters EM_End ON ED_End.EnergyMeterID = EM_End.Id
--        JOIN Rooms R_End ON EM_End.RoomID = R_End.Id
--        WHERE R_End.Id = '4E10F56A-147E-4541-ADE4-08DBEF4BCA36'
--          AND ED_End.DateTime >= @PreviousDate
--          AND ED_End.DateTime < @CurrentDate
--        ORDER BY ED_End.AccumulatedValue DESC
---- should be highest
---- 90066


--SELECT TOP(1) ED_End.AccumulatedValue
--        FROM EnergyData ED_End
--        JOIN EnergyMeters EM_End ON ED_End.EnergyMeterID = EM_End.Id
--        JOIN Rooms R_End ON EM_End.RoomID = R_End.Id
--        WHERE R_End.Id = '4E10F56A-147E-4541-ADE4-08DBEF4BCA36'
--          AND ED_End.DateTime >= @PreviousDate
--          AND ED_End.DateTime < @CurrentDate
--        ORDER BY ED_End.AccumulatedValue ASC
---- should be lowest
---- 82406