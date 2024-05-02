﻿CREATE TABLE [dbo].[MENU] (
    [ID]             INT            IDENTITY (1, 1) NOT NULL,
    [DESCR]          NVARCHAR (100) NOT NULL,
    [URL]            NVARCHAR (100) NULL,
    [PARENT_MENU_ID] INT            CONSTRAINT [DF_MENU_PARENT_MENU_ID] DEFAULT (NULL) NULL,
    [LABEL_KEY]      VARCHAR (100)  NOT NULL,
    [ICON_CODE]      VARCHAR (50)   NULL,
    CONSTRAINT [PK_MENU] PRIMARY KEY CLUSTERED ([ID] ASC)
);











