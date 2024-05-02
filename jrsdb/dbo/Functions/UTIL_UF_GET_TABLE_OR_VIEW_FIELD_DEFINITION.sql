

CREATE FUNCTION [dbo].[UTIL_UF_GET_TABLE_OR_VIEW_FIELD_DEFINITION]
/***********************************************************
* Function description: Returns a JSON with the information
*	of the columns for a given table or view.
*	Additional information are recovered from table "dbo.IMS_TABLE_COLUMN_DETAILS".
* Date:   03/30/2020  
* Author: n.migliore
*
* Changes
* Date		Modified By			Comments
************************************************************
*
************************************************************/
(
	@OBJECT_NAME VARCHAR(250)
) 
RETURNS NVARCHAR(MAX)
BEGIN
	DECLARE @ret NVARCHAR(MAX);

	DECLARE @DATATYPE_DICTIONARY TABLE (
		SQL_TYPE VARCHAR(20),
		JS_TYPE VARCHAR(20),
		FORM_FIELD_TYPE VARCHAR(20)
	);

	--Set datatypes
	INSERT INTO @DATATYPE_DICTIONARY(SQL_TYPE, JS_TYPE, FORM_FIELD_TYPE)
	VALUES ('uniqueidentifier', 'String', 'text'),
		('nvarchar', 'String', 'text'),
		('varchar', 'String', 'text'),
		('int', 'Numeric', 'text'),
		('decimal', 'Numeric', 'text'),
		('date', 'Date', 'date'),
		('datetime', 'Date', 'date'),
		('bit', 'Boolean', 'checkbox');

	--Get table definition
	IF EXISTS (SELECT 1 FROM sys.tables WHERE Name=@OBJECT_NAME) 
	BEGIN
		SELECT @ret = (
			SELECT V.TABLE_NAME AS [table_name],
				C.COLUMN_NAME AS [column_name],
				DICT.JS_TYPE AS [js_type],
				DICT.FORM_FIELD_TYPE AS [type],
				CASE C.IS_NULLABLE WHEN 'YES' THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [required],
				DETAILS.TRANSLATION_KEY AS [translationtKey],
				DETAILS.IS_PK AS [is_pk],
				DETAILS.HIDE_IN_LIST AS [is_hidden],
				DETAILS.REQUIRED AS [is_required],
				DETAILS.HINT AS [hint],
				DETAILS.HINT_TRANSLATION_KEY as [hintTranslationKey],
				DETAILS.SELECT_ITEMS_DATASOURCE as [select_items_datasource],
				DETAILS.SELECT_ITEM_KEY as [itemKey],
				DETAILS.SELECT_ITEM_TEXT as [itemText],
				DETAILS.SELECT_IS_MULTIPLE as [multiple],
				DETAILS.HIDE_IN_FORM AS [hide_in_form],
				DETAILS.SELECT_ITEMS_LOOKUP_TYPE_CODE AS [select_lookup_code]
			FROM INFORMATION_SCHEMA.TABLES AS v
				INNER JOIN INFORMATION_SCHEMA.COLUMNS AS C ON C.TABLE_NAME = V.TABLE_NAME
				LEFT JOIN @DATATYPE_DICTIONARY AS DICT ON DICT.SQL_TYPE	= C.DATA_TYPE
				LEFT JOIN dbo.IMS_TABLE_COLUMN_DETAILS AS DETAILS ON DETAILS.TABLE_NAME = V.TABLE_NAME
																	AND DETAILS.COLUMN_NAME = C.COLUMN_NAME
			WHERE V.[TABLE_NAME]= @OBJECT_NAME
			ORDER BY C.ORDINAL_POSITION
			FOR JSON path, root('table_definition')
		);
	END

	--Get view definition
	IF EXISTS (SELECT 1 FROM sys.views WHERE Name=@OBJECT_NAME)
	BEGIN
		SELECT @ret = (
			SELECT V.TABLE_NAME AS [table_name],
				C.COLUMN_NAME AS [column_name],
				DICT.JS_TYPE AS [js_type],
				DICT.FORM_FIELD_TYPE AS [type],
				CASE C.IS_NULLABLE WHEN 'YES' THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [required],
				DETAILS.TRANSLATION_KEY AS [translationtKey],
				DETAILS.IS_PK AS [is_pk],
				DETAILS.HIDE_IN_LIST AS [is_hidden],
				DETAILS.REQUIRED AS [is_required],
				DETAILS.HINT AS [hint],
				DETAILS.HINT_TRANSLATION_KEY as [hintTranslationKey],
				DETAILS.SELECT_ITEMS_DATASOURCE as [select_items_datasource],
				DETAILS.SELECT_ITEM_KEY as [itemKey],
				DETAILS.SELECT_ITEM_TEXT as [itemText],
				DETAILS.SELECT_IS_MULTIPLE as [multiple],
				DETAILS.HIDE_IN_FORM AS [hide_in_form],
				DETAILS.SELECT_ITEMS_LOOKUP_TYPE_CODE AS [select_lookup_code]
			FROM INFORMATION_SCHEMA.VIEWS AS v
				INNER JOIN INFORMATION_SCHEMA.COLUMNS AS C ON C.TABLE_NAME = V.TABLE_NAME
				LEFT JOIN @DATATYPE_DICTIONARY AS DICT ON DICT.SQL_TYPE	= C.DATA_TYPE
				LEFT JOIN dbo.IMS_TABLE_COLUMN_DETAILS AS DETAILS ON DETAILS.TABLE_NAME = V.TABLE_NAME
																	AND DETAILS.COLUMN_NAME = C.COLUMN_NAME
			WHERE V.[TABLE_NAME]= @OBJECT_NAME
			ORDER BY C.ORDINAL_POSITION
			FOR JSON path, root('table_definition')
		);
	END
	
	RETURN @ret
END