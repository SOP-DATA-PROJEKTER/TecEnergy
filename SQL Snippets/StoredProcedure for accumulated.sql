
CREATE PROCEDURE CalculateAndInsertDailyAccumulatedValue
    @RoomId uniqueidentifier,
    @StartDate datetime2,
    @EndDate datetime2
AS
BEGIN
    DECLARE @DailyAccumulatedValue bigint;
    DECLARE @NewId uniqueidentifier = NEWID();

    SELECT @DailyAccumulatedValue = 
        (SELECT TOP(1) ED_End.AccumulatedValue
        FROM EnergyData ED_End
        JOIN EnergyMeters EM_End ON ED_End.EnergyMeterID = EM_End.Id
        JOIN Rooms R_End ON EM_End.RoomID = R_End.Id
        WHERE R_End.Id = @RoomId
          AND ED_End.DateTime >= @StartDate
          AND ED_End.DateTime < @EndDate
        ORDER BY ED_End.AccumulatedValue DESC) -
        (SELECT TOP(1) ED_Start.AccumulatedValue
        FROM EnergyData ED_Start
        JOIN EnergyMeters EM_Start ON ED_Start.EnergyMeterID = EM_Start.Id
        JOIN Rooms R_Start ON EM_Start.RoomID = R_Start.Id
        WHERE R_Start.Id = @RoomId
          AND ED_Start.DateTime >= @StartDate
          AND ED_Start.DateTime < @EndDate
        ORDER BY ED_Start.AccumulatedValue ASC);

    INSERT INTO DailyAccumulated (Id, RoomId, DateTime, DailyAccumulatedValue)
    VALUES (@NewId, @RoomId, @StartDate, @DailyAccumulatedValue);
END;

