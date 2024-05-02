CREATE TABLE [dbo].[IMS_LABELS] (
    [IMS_LABELS_ID]          INT            IDENTITY (1, 1) NOT NULL,
    [IMS_LABELS_TABLE_NAME]  NVARCHAR (50)  NOT NULL,
    [IMS_LABELS_NUMBER]      INT            NOT NULL,
    [IMS_LABELS_LANGUAGE_ID] INT            NOT NULL,
    [IMS_LABELS_VALUE]       NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([IMS_LABELS_ID] ASC),
    FOREIGN KEY ([IMS_LABELS_LANGUAGE_ID]) REFERENCES [dbo].[IMS_LANGUAGE] ([IMS_LANGUAGE_ID]),
    CONSTRAINT [unique_table_number_language] UNIQUE NONCLUSTERED ([IMS_LABELS_TABLE_NAME] ASC, [IMS_LABELS_NUMBER] ASC, [IMS_LABELS_LANGUAGE_ID] ASC)
);






GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Groupes Labels for one element over multiple languages. This is what FKs will reference.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IMS_LABELS', @level2type = N'COLUMN', @level2name = N'IMS_LABELS_NUMBER';

