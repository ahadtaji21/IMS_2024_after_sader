﻿CREATE TABLE [dbo].[PMS_TYPE_OF_SERVICE] (
    [PMS_TOS_ID]          INT           IDENTITY (1, 1) NOT NULL,
    [PMS_TOS_CODE]        VARCHAR (50)  NULL,
    [PMS_TOS_DESCRIPTION] VARCHAR (MAX) NULL,
    [PMS_TOS_ENABLED]     BIT           CONSTRAINT [DF_PMS_TYPE_OF_SERVICE_PMS_TOS_ENABLED] DEFAULT ((1)) NOT NULL,
    [PMS_TOS_DELETED]     BIT           CONSTRAINT [DF_PMS_TYPE_OF_SERVICE_PMS_TOS_DELETED] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PMS_TYPE_OF_SERVICE] PRIMARY KEY CLUSTERED ([PMS_TOS_ID] ASC)
);

