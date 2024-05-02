
CREATE FUNCTION dbo.UTIL_UF_GENERIC_QUERY_DATA
/***********************************************************
* Function description:
* Date:   03/17/2020  
* Author: nicol
*
* Changes
* Date		Modified By			Comments
************************************************************
*
************************************************************/
(
	@VIEW_NAME NVARCHAR(250),
	@COLUMN_LIST NVARCHAR(MAX) = NULL,
	@WHERE_CONDITION NVARCHAR(MAX) = NULL,
	@ORDER_STATEMENT NVARCHAR(MAX) = NULL
) 
RETURNS NVARCHAR(MAX)
BEGIN
	DECLARE @SQL_STMT NVARCHAR(MAX),
		@PARAM_DEFINITION NVARCHAR(MAX),
		@HEADERS_JSON NVARCHAR(MAX),
		@DATA_JSON NVARCHAR(MAX),
		@RET_JSON NVARCHAR(MAX) = '{ "headers":"", "table_data":"" }';

	
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
	SET @RET_JSON = JSON_MODIFY(@RET_JSON, '$.headers', JSON_QUERY(@HEADERS_JSON, '$.table_definition'));
	SET @RET_JSON = JSON_MODIFY(@RET_JSON, '$.table_data', JSON_QUERY(@DATA_JSON));
	

	RETURN @RET_JSON;
END