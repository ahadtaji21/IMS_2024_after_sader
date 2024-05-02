
CREATE PROCEDURE dbo.UTIL_SP_GET_VIEW_DEFINITION
	@VIEW_NAME VARCHAR(250),
	@COLUMN_LIST NVARCHAR(MAX) = NULL,
	@WHERE_CONDITION NVARCHAR(MAX) = NULL,
	@ORDER_STATEMENT NVARCHAR(MAX) = NULL,
	@RET_JSON NVARCHAR(MAX) OUTPUT
AS
/***********************************************************
* Function description:Returns a JSON with the information
*	of the columns AND CRUD info for a given view.
* Date:   03/24/2020  
* Author: n.migliore
*
* Changes
* Date		Modified By			Comments
************************************************************
*
************************************************************/
BEGIN
	DECLARE @CRUD_JSON NVARCHAR(MAX),
		@HEADERS_JSON NVARCHAR(MAX);


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
	--Compile return object
	------------------------------------------------------------------------------------
	SET @RET_JSON = '{ "crud_info":"", "columns":"" }';
	SET @RET_JSON = JSON_MODIFY(@RET_JSON, '$.crud_info', JSON_QUERY(@CRUD_JSON));
	SET @RET_JSON = JSON_MODIFY(@RET_JSON, '$.columns', JSON_QUERY(@HEADERS_JSON, '$.table_definition'));
	
END