CREATE PROCEDURE [demo].[Query]	@count int
AS
	SELECT TOP(@count) [Id],[DataValue] FROM [demo].[Data] ORDER BY [Id] DESC;
RETURN 0
