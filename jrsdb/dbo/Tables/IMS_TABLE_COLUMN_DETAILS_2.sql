CREATE TABLE [dbo].[IMS_TABLE_COLUMN_DETAILS] (
    [ID]                                INT            IDENTITY (1, 1) NOT NULL,
    [TABLE_NAME]                        VARCHAR (500)  NOT NULL,
    [COLUMN_NAME]                       VARCHAR (500)  NOT NULL,
    [TRANSLATION_KEY]                   VARCHAR (500)  NULL,
    [HIDE_IN_LIST]                      BIT            DEFAULT ((0)) NULL,
    [IS_PK]                             BIT            NULL,
    [REQUIRED]                          BIT            NULL,
    [HINT]                              NVARCHAR (500) NULL,
    [HINT_TRANSLATION_KEY]              NVARCHAR (500) NULL,
    [SELECT_ITEMS_DATASOURCE]           NVARCHAR (MAX) NULL,
    [SELECT_ITEM_KEY]                   VARCHAR (250)  NULL,
    [SELECT_ITEM_TEXT]                  VARCHAR (250)  NULL,
    [SELECT_IS_MULTIPLE]                BIT            NULL,
    [HIDE_IN_FORM]                      BIT            NULL,
    [SELECT_ITEMS_DATASOURCE_CONDITION] NVARCHAR (MAX) NULL,
    [SELECT_ITEMS_LOOKUP_TYPE_CODE]     NVARCHAR (25)  NULL,
    [READONLY]                          BIT            DEFAULT ((0)) NOT NULL,
    [TABLE_ID]                          INT            NULL,
    [SELECT_ITEMS_ORDER_CONDITION]      NVARCHAR (MAX) NULL,
    [TAB_CODE]                          VARCHAR (10)   NULL,
    [TAB_TRANSL_KEY]                    VARCHAR (250)  NULL,
    [FORM_FIELD_TYPE]                   VARCHAR (50)   NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    FOREIGN KEY ([FORM_FIELD_TYPE]) REFERENCES [dbo].[IMS_FORM_FIELD_TYPE] ([CODE]),
    FOREIGN KEY ([TABLE_ID]) REFERENCES [dbo].[IMS_TABLES] ([ID])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'LOOKUP_TYPE_CODE for retrieving "select items" from lookups client-side', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IMS_TABLE_COLUMN_DETAILS', @level2type = N'COLUMN', @level2name = N'SELECT_ITEMS_LOOKUP_TYPE_CODE';

