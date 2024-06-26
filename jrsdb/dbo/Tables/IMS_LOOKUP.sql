﻿CREATE TABLE [dbo].[IMS_LOOKUP] (
    [IMS_LOOKUP_ID]             INT            IDENTITY (1, 1) NOT NULL,
    [IMS_LOOKUP_LOOKUP_TYPE_ID] INT            NOT NULL,
    [IMS_LOOKUP_CODE]           NVARCHAR (50)  NULL,
    [IMS_LOOKUP_VALUE]          NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([IMS_LOOKUP_ID] ASC),
    FOREIGN KEY ([IMS_LOOKUP_LOOKUP_TYPE_ID]) REFERENCES [dbo].[IMS_LOOKUP_TYPE] ([IMS_LOOKUP_TYPE_ID])
);

