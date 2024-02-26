DECLARE @MeterId uniqueidentifier;

DECLARE meterCursor CURSOR FOR
	SELECT ID
	FROM EnergyMeters;

OPEN meterCursor;

FETCH NEXT FROM meterCursor INTO @MeterId;

WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC InsertDaily
        @EnergyMeterId = @MeterId

    FETCH NEXT FROM meterCursor INTO @MeterId;
END

CLOSE meterCursor;
DEALLOCATE meterCursor;
