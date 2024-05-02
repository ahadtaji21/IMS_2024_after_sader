
--/****** Object:  StoredProcedure [dbo].[UTIL_SP_GENERIC_QUERY_DATA]    Script Date: 3/24/2020 10:49:45 AM ******/
--SET ANSI_NULLS ON
--GO

--SET QUOTED_IDENTIFIER ON
--GO


CREATE PROCEDURE [dbo].[UTIL_SP_GENERIC_QUERY_DATA]
	@VIEW_NAME NVARCHAR(250),
	@COLUMN_LIST NVARCHAR(MAX) = NULL,
	@WHERE_CONDITION NVARCHAR(MAX) = NULL,
	@ORDER_STATEMENT NVARCHAR(MAX) = NULL,
	@RET_JSON NVARCHAR(MAX) OUTPUT
AS
	/***********************************************************
	* Description: Recover information of a table defintion as well as it's data.
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
		@CRUD_JSON NVARCHAR(MAX),
		@HEADERS_JSON NVARCHAR(MAX),
		@DATA_JSON NVARCHAR(MAX);

	------------------------------------------------------------------------------------
	--Get CRUD info
	------------------------------------------------------------------------------------
	SELECT @CRUD_JSON = (
		SELECT
			CRUDS.TABLE_NAME AS [table_name],
			CRUDS.CREATE_SP_NAME AS [create_sp],
			CRUDS.READ_SP_NAME AS [read_sp],
			CRUDS.UPDATE_SP_NAME AS [update_sp],
			CRUDS.DELETE_SP_NAME AS [delete_sp]
		FROM IMS_TABLE_CRUD_DETAILS CRUDS
		WHERE CRUDS.TABLE_NAME = @VIEW_NAME
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
	);


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
	SET @RET_JSON = '{ "crud_info":"", "columns":"", "table_data":"" }';
	SET @RET_JSON = JSON_MODIFY(@RET_JSON, '$.crud_info', JSON_QUERY(@CRUD_JSON));
	SET @RET_JSON = JSON_MODIFY(@RET_JSON, '$.columns', JSON_QUERY(@HEADERS_JSON, '$.table_definition'));
	SET @RET_JSON = JSON_MODIFY(@RET_JSON, '$.table_data', JSON_QUERY(@DATA_JSON));
	

END