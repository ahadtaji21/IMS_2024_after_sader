﻿CREATE TABLE [dbo].[HR_GENDER] (
    [HR_GENDER_ID]    INT          IDENTITY (1, 1) NOT NULL,
    [HR_GENDER_CODE]  VARCHAR (3)  NOT NULL,
    [HR_GENDER_DESCR] VARCHAR (25) NOT NULL,
    PRIMARY KEY CLUSTERED ([HR_GENDER_ID] ASC)
);
