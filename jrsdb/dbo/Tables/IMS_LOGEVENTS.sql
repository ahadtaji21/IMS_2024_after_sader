CREATE TABLE [dbo].[IMS_LOGEVENTS] (
    [id]         BIGINT        IDENTITY (897399717, 1) NOT NULL,
    [Message]    TEXT          NULL,
    [InsertDate] DATETIME      NULL,
    [UserId]     CHAR (50)     NULL,
    [IP]         NCHAR (15)    NULL,
    [Class]      NCHAR (50)    NULL,
    [Procedure]  NCHAR (50)    NULL,
    [Parameters] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_JRS_LOGEVENTI] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90)
);

