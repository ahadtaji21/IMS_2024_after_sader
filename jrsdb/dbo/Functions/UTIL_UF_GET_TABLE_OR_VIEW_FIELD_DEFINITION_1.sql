


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
*	2020-04-09		n.migliore		Implemented use of IMS_FORM_FIELD_TYPE
*									for field type override.
*	2020-04-10		n.migliore		Implemented use of @COUMNS to limit result set.
************************************************************/
(
	@OBJECT_NAME VARCHAR(250),
	@COLUMNS varchar(max) = NULL
) 
RETURNS NVARCHAR(MAX)
BEGIN
	DECLARE @ret NVARCHAR(MAX);

	DECLARE @DATATYPE_DICTIONARY TABLE (
		SQL_TYPE VARCHAR(20),
		JS_TYPE VARCHAR(20),
		FORM_FIELD_TYPE VARCHAR(20)
	);

	DECLARE @COLUMN_LIST TABLE (
		TABLE_NAME VARCHAR(128),
		COLUMN_NAME VARCHAR(128),
		DATA_TYPE NVARCHAR(120),
		ORDINAL_POSITION INT,
		IS_NULLABLE VARCHAR(5)
	)

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

	--Set column list
	IF @COLUMNS IS NULL
	BEGIN
		INSERT INTO @COLUMN_LIST (
			TABLE_NAME,
			COLUMN_NAME,
			DATA_TYPE,
			IS_NULLABLE
		)
		SELECT TABLE_NAME,
			COLUMN_NAME,
			DATA_TYPE,
			IS_NULLABLE
		FROM INFORMATION_SCHEMA.COLUMNS AS C 
		WHERE C.TABLE_NAME = @OBJECT_NAME;
	END
	ELSE
	BEGIN
		INSERT INTO @COLUMN_LIST (
			TABLE_NAME,
			COLUMN_NAME,
			DATA_TYPE,
			ORDINAL_POSITION
		)
		SELECT TABLE_NAME,
			COLUMN_NAME,
			DATA_TYPE,
			ORDINAL_POSITION
		FROM INFORMATION_SCHEMA.COLUMNS AS C 
		WHERE C.TABLE_NAME = @OBJECT_NAME
			AND COLUMN_NAME IN ( 	SELECT *
									FROM string_split(@COLUMNS, ','));
	END;

	--Get columns definition
	SELECT @ret = (
		SELECT C.TABLE_NAME AS [table_name],
			C.COLUMN_NAME AS [column_name],
			DICT.JS_TYPE AS [js_type],
			ISNULL(LOWER(DETAILS.FORM_FIELD_TYPE), DICT.FORM_FIELD_TYPE) AS [type],
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
			DETAILS.SELECT_ITEMS_DATASOURCE_CONDITION AS [dataSourceCondition],
			DETAILS.SELECT_ITEMS_ORDER_CONDITION AS [dataSourceOrder],
			DETAILS.SELECT_IS_MULTIPLE as [multiple],
			DETAILS.HIDE_IN_FORM AS [hide_in_form],
			DETAILS.SELECT_ITEMS_LOOKUP_TYPE_CODE AS [select_lookup_code],
			DETAILS.[READONLY] AS [readonly],
			DETAILS.TAB_CODE AS [tabCode],
			DETAILS.TAB_TRANSL_KEY AS [tabTranslationKey]
		FROM @COLUMN_LIST AS C
				LEFT JOIN @DATATYPE_DICTIONARY AS DICT ON DICT.SQL_TYPE	= C.DATA_TYPE
				LEFT JOIN dbo.IMS_TABLE_COLUMN_DETAILS AS DETAILS ON DETAILS.TABLE_NAME = C.TABLE_NAME
																AND DETAILS.COLUMN_NAME = C.COLUMN_NAME
		ORDER BY C.ORDINAL_POSITION
		FOR JSON path, root('table_definition')
	);
	
	RETURN @ret
END