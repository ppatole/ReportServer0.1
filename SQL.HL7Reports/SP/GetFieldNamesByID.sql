CREATE PROCEDURE [dbo].[GetFieldNameByID]
@FieldNameID AS VARCHAR(30)	
AS
	SELECT * FROM FieldName 
	WHERE FieldNameID LIKE @FieldNameID

