CREATE TABLE [dbo].[IMS_LOGERRORS] (
    [id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [Source]     NCHAR (255)    NULL,
    [StackTrace] TEXT           NULL,
    [InsertDate] DATETIME       NULL,
    [ServerName] NVARCHAR (255) NULL,
    [IP]         NCHAR (15)     NULL,
    [UserId]     CHAR (16)      NULL,
    [Parameters] VARCHAR (MAX)  NULL,
    [Class]      NCHAR (50)     NULL,
    [Procedure]  NCHAR (50)     NULL,
    CONSTRAINT [PK_JRS_LOGERRORI] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90)
);

