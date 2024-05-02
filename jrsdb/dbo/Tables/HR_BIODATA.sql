CREATE TABLE [dbo].[HR_BIODATA] (
    [HR_BIODATA_ID]                       INT              IDENTITY (1, 1) NOT NULL,
    [HR_BIODATA_USER_UID]                 UNIQUEIDENTIFIER NULL,
    [HR_BIODATA_PERSONAL_TITLE_ID]        INT              NULL,
    [HR_BIODATA_SURNAME]                  NVARCHAR (150)   NOT NULL,
    [HR_BIODATA_MIDDLE_NAME]              NVARCHAR (150)   NULL,
    [HR_BIODATA_NAME]                     NVARCHAR (150)   NOT NULL,
    [HR_BIODATA_GENDER_ID]                INT              NOT NULL,
    [HR_BIODATA_BIRTH_DATE]               DATE             NOT NULL,
    [HR_BIODATA_BIRTH_PLACE]              VARCHAR (150)    NOT NULL,
    [HR_BIODATA_NATIONALITY]              NVARCHAR (MAX)   NULL,
    [HR_BIODATA_RELIGIOUS]                BIT              DEFAULT ((0)) NOT NULL,
    [HR_BIODATA_PERMANENT_LOCATION_ID]    INT              NULL,
    [HR_BIODATA_IDENTIFICATION_DOCUMENTS] NVARCHAR (MAX)   NULL,
    [HR_BIODATA_REGNUMBER]                VARCHAR (100)    NULL,
    [HR_BIODATA_PHOTO]                    VARBINARY (MAX)  NULL,
    [HR_BIODATA_GENEDER_LOOKUP_ID]        INT              NULL,
    [HR_BIODATA_PERSONAL_TITLE_LOOKUP_ID] INT              NULL,
    PRIMARY KEY CLUSTERED ([HR_BIODATA_ID] ASC),
    FOREIGN KEY ([HR_BIODATA_GENDER_ID]) REFERENCES [dbo].[HR_GENDER] ([HR_GENDER_ID]),
    FOREIGN KEY ([HR_BIODATA_GENEDER_LOOKUP_ID]) REFERENCES [dbo].[IMS_LOOKUP] ([IMS_LOOKUP_ID]),
    FOREIGN KEY ([HR_BIODATA_PERSONAL_TITLE_ID]) REFERENCES [dbo].[HR_PERSONAL_TITLE] ([HR_PERSONAL_TITLE_ID]),
    FOREIGN KEY ([HR_BIODATA_PERSONAL_TITLE_LOOKUP_ID]) REFERENCES [dbo].[IMS_LOOKUP] ([IMS_LOOKUP_ID]),
    FOREIGN KEY ([HR_BIODATA_USER_UID]) REFERENCES [dbo].[IMS_USER] ([IMS_USER_UID])
);














GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'JSON list of objects with identification document information : doc. type, doc. number, doc. expiry date', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'HR_BIODATA', @level2type = N'COLUMN', @level2name = N'HR_BIODATA_IDENTIFICATION_DOCUMENTS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'JSON list of objects with nationality information', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'HR_BIODATA', @level2type = N'COLUMN', @level2name = N'HR_BIODATA_NATIONALITY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Internal JRS registration code', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'HR_BIODATA', @level2type = N'COLUMN', @level2name = N'HR_BIODATA_REGNUMBER';

