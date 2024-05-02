CREATE TABLE [dbo].[IMS_NARRATIVE_SECTION] (
    [ID]                        INT            IDENTITY (1, 1) NOT NULL,
    [NARRATIVE_SECTION_TYPE_ID] INT            NULL,
    [REFERENCE_ID]              INT            NOT NULL,
    [SECTION_TITLE]             VARCHAR (250)  NULL,
    [SECTION_TEXT]              NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

