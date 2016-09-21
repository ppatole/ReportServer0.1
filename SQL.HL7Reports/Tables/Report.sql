CREATE TABLE [dbo].[Report]
(
	[ReportID] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [ReportDateTime] DATETIME2 NULL, 
    [ImportedOn] DATETIME2 NULL, 
	PatientID VARCHAR(1000),
	PatientName VARCHAR(2000),
	AccessionNumber VARCHAR(100),
    [ReportXML] XML,
    [ReportText] NVARCHAR(MAX) NULL, 
	[ActiveTF] BIT NULL, 
    [CreatedOn] DATETIME NULL DEFAULT GetDate(), 
    [UID] UNIQUEIDENTIFIER NULL DEFAULT NEWID() 
)
