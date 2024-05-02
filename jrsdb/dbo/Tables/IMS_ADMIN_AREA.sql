CREATE TABLE [dbo].[IMS_ADMIN_AREA] (
    [IMS_ADMIN_AREA_NODE]                   [sys].[hierarchyid] NOT NULL,
    [IMS_ADMIN_AREA_LEVEL]                  AS                  ([IMS_ADMIN_AREA_NODE].[GetLevel]()),
    [IMS_ADMIN_AREA_ID]                     INT                 IDENTITY (1, 1) NOT NULL,
    [IMS_ADMIN_AREA_NAME]                   VARCHAR (25)        NOT NULL,
    [IMS_ADMIN_AREA_NATIVE_NAME]            VARCHAR (25)        NULL,
    [IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID]     INT                 NULL,
    [IMS_ADMIN_AREA_MEMBERSHIP_TERM]        VARCHAR (50)        NULL,
    [IMS_ADMIN_AREA_NATIVE_MEMBERSHIP_TERM] VARCHAR (50)        NULL,
    [IMS_ADMIN_AREA_ALIAS]                  NVARCHAR (1000)     NULL,
    [IMS_ADMIN_AREA_HISTORIC_NAMES]         NVARCHAR (1000)     NULL,
    PRIMARY KEY CLUSTERED ([IMS_ADMIN_AREA_NODE] ASC),
    UNIQUE NONCLUSTERED ([IMS_ADMIN_AREA_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'JSON list of objects containing historic names and date interval of administrative area', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IMS_ADMIN_AREA', @level2type = N'COLUMN', @level2name = N'IMS_ADMIN_AREA_HISTORIC_NAMES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'JSON list of objects containing aliases of administrative area', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IMS_ADMIN_AREA', @level2type = N'COLUMN', @level2name = N'IMS_ADMIN_AREA_ALIAS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Term of membership to the administrative area (e.g. citizenship) in native language', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IMS_ADMIN_AREA', @level2type = N'COLUMN', @level2name = N'IMS_ADMIN_AREA_NATIVE_MEMBERSHIP_TERM';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Term of membership to the administrative area (e.g. citizenship)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IMS_ADMIN_AREA', @level2type = N'COLUMN', @level2name = N'IMS_ADMIN_AREA_MEMBERSHIP_TERM';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Name of the administrative area in native language', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IMS_ADMIN_AREA', @level2type = N'COLUMN', @level2name = N'IMS_ADMIN_AREA_NATIVE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Name of the administrative area', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IMS_ADMIN_AREA', @level2type = N'COLUMN', @level2name = N'IMS_ADMIN_AREA_NAME';

