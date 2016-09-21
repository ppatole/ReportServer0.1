CREATE PROCEDURE [dbo].[SaveReport]
	@XML XML
AS
BEGIN

	UPDATE Report SET ActiveTF = 0
	WHERE AccessionNumber IN
	(	SELECT xc.value('(AccessionNumber)[1]', 'VARCHAR(100)')  AS AccessionNumber		
		FROM @XML.nodes('/Report') AS xt(xc)
	)	

	INSERT INTO [dbo].[Report]
        ([ReportDateTime]
        ,[ImportedOn]
        ,[ReportText]
		,	PatientID 
		,	PatientName 
		,	AccessionNumber 
		,	[ReportXML] 
        ,[ActiveTF]        ,[CreatedOn]        ,[UID])
	SELECT
			xc.value('(ReportDateTime)[1]', 'DateTime2') AS ReportdateTime,
			xc.value('(ImportedOn)[1]', 'DateTime2') AS ImportedOn,
			xc.value('(ReportText)[1]', 'VARCHAR(MAX)')  AS ReportText,
			xc.value('(PatientID)[1]', 'VARCHAR(1000)')  AS PatientID,
			xc.value('(Patientname)[1]', 'VARCHAR(2000)')  AS PatientName,
			xc.value('(AccessionNumber)[1]', 'VARCHAR(100)')  AS AccessionNumber,			
			@XML,
			1,getDate() CreatedOn ,NEWID() [UID]
	FROM 
		@XML.nodes('/Report') AS xt(xc)

DECLARE @reportID BIGINT
SELECT @reportID = @@IDENTITY
  

  END
