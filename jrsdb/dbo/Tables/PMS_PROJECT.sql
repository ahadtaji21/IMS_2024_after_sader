CREATE TABLE [dbo].[PMS_PROJECT] (
    [ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [CODE]                  VARCHAR (50)   NOT NULL,
    [DESCR]                 NVARCHAR (250) NOT NULL,
    [LOCATION_DESCR]        NVARCHAR (250) NOT NULL,
    [COUNTRY_ADMIN_AREA_ID] INT            NOT NULL,
    [STATUS_ID]             INT            NOT NULL,
    [START_YEAR]            INT            NOT NULL,
    [OFFICE_ID]             INT            NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    FOREIGN KEY ([COUNTRY_ADMIN_AREA_ID]) REFERENCES [dbo].[IMS_ADMIN_AREA] ([IMS_ADMIN_AREA_ID]),
    CONSTRAINT [FK_PMS_PROJECT_HR_OFFICE] FOREIGN KEY ([OFFICE_ID]) REFERENCES [dbo].[HR_OFFICE] ([HR_OFFICE_ID])
);

