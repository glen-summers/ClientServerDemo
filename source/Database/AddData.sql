CREATE PROCEDURE [demo].[AddData] @dataValue VARCHAR(256), @id int OUTPUT
AS
	INSERT INTO [demo].[Data] ([DataValue]) OUTPUT INSERTED.Id VALUES (@dataValue);
	SET @id = @@IDENTITY;
RETURN 0;
