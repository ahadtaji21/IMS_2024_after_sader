CREATE TABLE [dbo].[LOG] (
    [Id]              BIGINT            IDENTITY (1, 1) NOT NULL,
    [RequestTime]     DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    [ResponseMillis]  BIGINT            NOT NULL,
    [Statuscode]      INT            NOT NULL,
    [Headers]         NVARCHAR (MAX) DEFAULT (NULL) NULL,
    [Method]          NVARCHAR (20)  NOT NULL,
    [Path]            NVARCHAR (100) NOT NULL,
    [QueryString]     NVARCHAR (256) DEFAULT (NULL) NULL,
    [Requestbody]     NVARCHAR (256) DEFAULT (NULL) NULL,
    [Responsebody]    NVARCHAR (256) DEFAULT (NULL) NULL,
    [Servervariables] NVARCHAR (MAX) DEFAULT (NULL) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

