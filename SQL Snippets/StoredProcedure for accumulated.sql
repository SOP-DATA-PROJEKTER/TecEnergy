
CREATE PROCEDURE InsertDaily
    @EnergyMeterId uniqueidentifier

AS
	BEGIN
		DECLARE @EndDate datetime = CONVERT(datetime, CONVERT(date, GETDATE()));
		DECLARE @StartDate datetime = CONVERT(datetime, CONVERT(date, GETDATE() - 1));
		DECLARE @DailyAccumulatedValue bigint;

		SELECT @DailyAccumulatedValue =
			(SELECT TOP(1) AccumulatedValue FROM EnergyData
				Where EnergyMeterId = @EnergyMeterId
				AND [DateTime] >= @StartDate
				AND [DateTime] < @StartDate
				Order By [DateTime] DESC)
				-
			(SELECT TOP(1) AccumulatedValue FROM EnergyData
				Where EnergyMeterId = @EnergyMeterId
				AND [DateTime] >= @StartDate
				AND [DateTime] < @StartDate
				Order By [DateTime] ASC);

		INSERT INTO DailyAccumulations VALUES(NEWID(), @EnergyMeterId, @StartDate, @DailyAccumulatedValue);
	END;

