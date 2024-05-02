﻿CREATE TABLE [dbo].[HR_OFFICE] (
    [HR_OFFICE_ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [HR_OFFICE_CODE]                  VARCHAR (25)   NOT NULL,
    [HR_OFFICE_LEGAL_NAME]            NVARCHAR (250) NOT NULL,
    [HR_OFFICE_LEGAL_NAME_NATIVE]     NVARCHAR (250) NULL,
    [HR_OFFICE_LEGAL_LANGUAGE_ID]     INT            NULL,
    [HR_OFFICE_NATIVE_LANGUAGE_ID]    INT            NULL,
    [HR_OFFICE_LEGAL_REP_NAME]        NVARCHAR (500) NULL,
    [HR_OFFICE_SIGNATORY_NAME]        NVARCHAR (500) NULL,
    [HR_OFFICE_LEGAL_ADDRESS_ID]      INT            NULL,
    [HR_OFFICE_COUNTRY_ADMIN_AREA_ID] INT            NULL,
    PRIMARY KEY CLUSTERED ([HR_OFFICE_ID] ASC),
    FOREIGN KEY ([HR_OFFICE_COUNTRY_ADMIN_AREA_ID]) REFERENCES [dbo].[IMS_ADMIN_AREA] ([IMS_ADMIN_AREA_ID]),
    FOREIGN KEY ([HR_OFFICE_LEGAL_ADDRESS_ID]) REFERENCES [dbo].[IMS_LOCATION] ([IMS_LOCATION_ID]),
    FOREIGN KEY ([HR_OFFICE_LEGAL_LANGUAGE_ID]) REFERENCES [dbo].[IMS_LANGUAGE] ([IMS_LANGUAGE_ID]),
    FOREIGN KEY ([HR_OFFICE_NATIVE_LANGUAGE_ID]) REFERENCES [dbo].[IMS_LANGUAGE] ([IMS_LANGUAGE_ID])
);

