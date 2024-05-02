

CREATE PROCEDURE [dbo].[UTIL_SP_GENERIC_QUERY_DATA]
	@VIEW_NAME NVARCHAR(250),
	@COLUMN_LIST NVARCHAR(MAX) = NULL,
	@WHERE_CONDITION NVARCHAR(MAX) = NULL,
	@ORDER_STATEMENT NVARCHAR(MAX) = NULL,
	@RET_JSON NVARCHAR(MAX) OUTPUT
AS
	/***********************************************************
	* Function description:
	* Date:   03/17/2020  
	* Author: n.migliore
	*
	* Changes
	* Date		Modified By			Comments
	************************************************************
	*
	************************************************************/
BEGIN
	DECLARE @SQL_STMT NVARCHAR(MAX),
		@PARAM_DEFINITION NVARCHAR(MAX),
		@HEADERS_JSON NVARCHAR(MAX),
		@DATA_JSON NVARCHAR(MAX);

	
	------------------------------------------------------------------------------------
	--Get Headers
	------------------------------------------------------------------------------------
	SELECT @HEADERS_JSON = DBO.UTIL_UF_GET_VIEW_FIELD_DEFINITION(@VIEW_NAME);


	------------------------------------------------------------------------------------
	--Get Data
	------------------------------------------------------------------------------------
	--Set dynamic call
	SET  @SQL_STMT = N'SELECT @DATA = ('
					+ 'SELECT ' + ISNULL(@COLUMN_LIST, '*') + CHAR(10)
					+ 'FROM ' + @VIEW_NAME + CHAR(10)
					+ ISNULL('WHERE ' + @WHERE_CONDITION + CHAR(10), '')
					+ ISNULL(@ORDER_STATEMENT + CHAR(10), '')
					+ 'FOR JSON path'
					+ ')';

	--Set params fro dynamic call
	SET @PARAM_DEFINITION = '@DATA NVARCHAR(MAX) OUT';
	
	--PRINT @SQL_STMT
	EXEC sys.sp_executesql @SQL_STMT, @PARAM_DEFINITION, @DATA_JSON OUT;


	------------------------------------------------------------------------------------
	--Compile return object
	------------------------------------------------------------------------------------
	SET @RET_JSON = '{ "columns":"", "table_data":"" }';
	SET @RET_JSON = JSON_MODIFY(@RET_JSON, '$.columns', JSON_QUERY(@HEADERS_JSON, '$.table_definition'));
	SET @RET_JSON = JSON_MODIFY(@RET_JSON, '$.table_data', JSON_QUERY(@DATA_JSON));
	

	--select @RET_JSON AS retJson;
END