
CREATE PROCEDURE [dbo].[GetReport]
@PatientName AS VARCHAR (1000),
@AccessionNumber AS VARCHAR (1000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @Condition AS VARCHAR(2000)
	DECLARE @SQL VARCHAR(MAX)
	SET @Condition = ''
	SET @SQL = 
	'SELECT [ReportID],[ReportDateTime],[ImportedOn],[PatientID],[PatientName],
			[AccessionNumber],[ReportXML],[ReportText],[ActiveTF],[CreatedOn],[UID]
	 FROM [dbo].[Report] WHERE '

	
	IF @PatientName LIKE  '' AND  @AccessionNumber NOT LIKE '' 
	BEGIN
		SET @Condition = ' AccessionNumber LIKE ''' + @AccessionNumber + ''''
	END 		
	ELSE IF @AccessionNumber LIKE '' AND @PatientName NOT LIKE  '' 
	BEGIN
		SET @Condition = ' PatientName LIKE ''' + @PatientName + ''''
	END
	ELSE IF @PatientName NOT LIKE '' AND @AccessionNumber NOT LIKE '' 
	BEGIN
		SET @Condition = ' PatientName LIKE ''' + @PatientName + 
						''' AND AccessionNumber LIKE ''' + @AccessionNumber + ''''
	END 
	ELSE IF @PatientName = '' AND @AccessionNumber = '' 
	BEGIN
		SET @Condition = ' 2=1 '	
	END 

	EXECUTE (@SQL + @CONDITION)
END

