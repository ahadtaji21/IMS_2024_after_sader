CREATE PROCEDURE [dbo].[IMS_SP_INS_UPD_TABLE_COLUMN_DETAILS]
	@JSON_PAYLOAD NVARCHAR(MAX),
	@RET_JSON NVARCHAR(MAX) OUTPUT
AS
BEGIN
    /***********************************************************
    * Description: Create and Update Jrs tabel column defintion.
    * Date:   2020-04-06
    * Author: n.migliore
    *
    * Changes
    * Date		Modified By			Comments
    ************************************************************
    *   2020-04-09      n.migliore      Managed column FORM_FIELD_TYPE
    ************************************************************/
	DECLARE @ACTION_TYPE NVARCHAR(25)= JSON_VALUE(@JSON_PAYLOAD, '$.actionType');
    DECLARE @PAYLOAD NVARCHAR(MAX)= JSON_QUERY(@JSON_PAYLOAD, '$.jsonPayload');
	DECLARE @INPUT_DATASET TABLE (
		ID int , 
        TABLE_NAME VARCHAR(128), 
        COLUMN_NAME VARCHAR(128), 
        TRANSLATION_KEY VARCHAR(250), 
        HIDE_IN_LIST BIT, 
        IS_PK BIT, 
        REQUIRED BIT, 
        HINT VARCHAR(500), 
        HINT_TRANSLATION_KEY VARCHAR(500), 
        SELECT_ITEMS_DATASOURCE NVARCHAR(MAX), 
        SELECT_ITEM_KEY VARCHAR(250), 
        SELECT_ITEM_TEXT VARCHAR(250), 
        SELECT_IS_MULTIPLE BIT, 
        HIDE_IN_FORM BIT, 
        SELECT_ITEMS_DATASOURCE_CONDITION NVARCHAR(MAX), 
        SELECT_ITEMS_LOOKUP_TYPE_CODE VARCHAR(25), 
        READONLY BIT, 
        TABLE_ID INT, 
        SELECT_ITEMS_ORDER_CONDITION NVARCHAR(MAX),
        TAB_CODE VARCHAR(10),
        TAB_TRANSL_KEY VARCHAR(250),
        FORM_FIELD_TYPE varchar(20)
	);


    ------------------------------------------------------------------------------------
	--Recover payload data
	------------------------------------------------------------------------------------
	INSERT INTO @INPUT_DATASET
	(
	    ID, 
        TABLE_NAME, 
        COLUMN_NAME, 
        TRANSLATION_KEY, 
        HIDE_IN_LIST, 
        IS_PK, 
        REQUIRED, 
        HINT, 
        HINT_TRANSLATION_KEY, 
        SELECT_ITEMS_DATASOURCE, 
        SELECT_ITEM_KEY, 
        SELECT_ITEM_TEXT, 
        SELECT_IS_MULTIPLE, 
        HIDE_IN_FORM, 
        SELECT_ITEMS_DATASOURCE_CONDITION, 
        SELECT_ITEMS_LOOKUP_TYPE_CODE, 
        READONLY, 
        TABLE_ID, 
        SELECT_ITEMS_ORDER_CONDITION,
        TAB_CODE,
        TAB_TRANSL_KEY,
        FORM_FIELD_TYPE
	)
	SELECT jd.ID, 
        jd.TABLE_NAME, 
        jd.COLUMN_NAME, 
        jd.TRANSLATION_KEY, 
        jd.HIDE_IN_LIST, 
        jd.IS_PK, 
        jd.REQUIRED, 
        jd.HINT, 
        jd.HINT_TRANSLATION_KEY, 
        jd.SELECT_ITEMS_DATASOURCE, 
        jd.SELECT_ITEM_KEY, 
        jd.SELECT_ITEM_TEXT, 
        jd.SELECT_IS_MULTIPLE, 
        jd.HIDE_IN_FORM, 
        jd.SELECT_ITEMS_DATASOURCE_CONDITION, 
        jd.SELECT_ITEMS_LOOKUP_TYPE_CODE, 
        isnull(jd.READONLY, 0), 
        jd.TABLE_ID, 
        jd.SELECT_ITEMS_ORDER_CONDITION,
        NULLIF(jd.TAB_CODE, ''),
        NULLIF(jd.TAB_TRANSL_KEY, ''),
        jd.FORM_FIELD_TYPE
	FROM OPENJSON(@PAYLOAD)
	WITH (
		ID int , 
        TABLE_NAME VARCHAR(128), 
        COLUMN_NAME VARCHAR(128), 
        TRANSLATION_KEY VARCHAR(250), 
        HIDE_IN_LIST BIT, 
        IS_PK BIT, 
        REQUIRED BIT, 
        HINT VARCHAR(500), 
        HINT_TRANSLATION_KEY VARCHAR(500), 
        SELECT_ITEMS_DATASOURCE NVARCHAR(MAX), 
        SELECT_ITEM_KEY VARCHAR(250), 
        SELECT_ITEM_TEXT VARCHAR(250), 
        SELECT_IS_MULTIPLE BIT, 
        HIDE_IN_FORM BIT, 
        SELECT_ITEMS_DATASOURCE_CONDITION NVARCHAR(MAX), 
        SELECT_ITEMS_LOOKUP_TYPE_CODE VARCHAR(25), 
        READONLY BIT, 
        TABLE_ID INT, 
        SELECT_ITEMS_ORDER_CONDITION NVARCHAR(MAX),
        TAB_CODE VARCHAR(10),
        TAB_TRANSL_KEY VARCHAR(250),
        FORM_FIELD_TYPE VARCHAR(20)
	)AS	jd;
    

    ------------------------------------------------------------------------------------
    --Recover missing data
    ------------------------------------------------------------------------------------
    UPDATE DATASET
    SET TABLE_ID = ISNULL(DATASET.TABLE_ID, T.ID)
    FROM @INPUT_DATASET AS DATASET
        INNER JOIN dbo.IMS_TABLES AS T ON T.TABLE_NAME = DATASET.TABLE_NAME;


	BEGIN TRANSACTION
	BEGIN TRY
		------------------------------------------------------------------------------------
		--Create or update data
		------------------------------------------------------------------------------------
		--Set project data
        MERGE dbo.IMS_TABLE_COLUMN_DETAILS AS TARGET
            USING @INPUT_DATASET AS SOURCE
            ON TARGET.ID = SOURCE.ID
                WHEN MATCHED
                THEN UPDATE SET
                    TARGET.TABLE_NAME = SOURCE.TABLE_NAME, 
                    TARGET.COLUMN_NAME = SOURCE.COLUMN_NAME, 
                    TARGET.TRANSLATION_KEY = SOURCE.TRANSLATION_KEY, 
                    TARGET.HIDE_IN_LIST = SOURCE.HIDE_IN_LIST, 
                    TARGET.IS_PK = SOURCE.IS_PK, 
                    TARGET.REQUIRED = SOURCE.REQUIRED, 
                    TARGET.HINT = SOURCE.HINT, 
                    TARGET.HINT_TRANSLATION_KEY = SOURCE.HINT_TRANSLATION_KEY, 
                    TARGET.SELECT_ITEMS_DATASOURCE = SOURCE.SELECT_ITEMS_DATASOURCE, 
                    TARGET.SELECT_ITEM_KEY = SOURCE.SELECT_ITEM_KEY, 
                    TARGET.SELECT_ITEM_TEXT = SOURCE.SELECT_ITEM_TEXT,
                    TARGET.SELECT_IS_MULTIPLE = SOURCE.SELECT_IS_MULTIPLE,
                    TARGET.HIDE_IN_FORM = SOURCE.HIDE_IN_FORM,
                    TARGET.SELECT_ITEMS_DATASOURCE_CONDITION = SOURCE.SELECT_ITEMS_DATASOURCE_CONDITION, 
                    TARGET.SELECT_ITEMS_LOOKUP_TYPE_CODE = SOURCE.SELECT_ITEMS_LOOKUP_TYPE_CODE,
                    TARGET.READONLY = SOURCE.READONLY, 
                    TARGET.TABLE_ID = SOURCE.TABLE_ID,
                    TARGET.SELECT_ITEMS_ORDER_CONDITION = SOURCE.SELECT_ITEMS_ORDER_CONDITION,
                    TARGET.TAB_CODE = SOURCE.TAB_CODE,
                    TARGET.TAB_TRANSL_KEY = SOURCE.TAB_TRANSL_KEY,
                    TARGET.FORM_FIELD_TYPE = SOURCE.FORM_FIELD_TYPE
                    
                WHEN NOT MATCHED
                THEN
                    INSERT(
                        TABLE_NAME, 
                        COLUMN_NAME, 
                        TRANSLATION_KEY, 
                        HIDE_IN_LIST, 
                        IS_PK, 
                        REQUIRED, 
                        HINT, 
                        HINT_TRANSLATION_KEY, 
                        SELECT_ITEMS_DATASOURCE, 
                        SELECT_ITEM_KEY, 
                        SELECT_ITEM_TEXT, 
                        SELECT_IS_MULTIPLE, 
                        HIDE_IN_FORM, 
                        SELECT_ITEMS_DATASOURCE_CONDITION, 
                        SELECT_ITEMS_LOOKUP_TYPE_CODE, 
                        READONLY, 
                        TABLE_ID, 
                        SELECT_ITEMS_ORDER_CONDITION,
                        TAB_CODE,
                        TAB_TRANSL_KEY,
                        FORM_FIELD_TYPE
                    )
                    VALUES(
                        SOURCE.TABLE_NAME, 
                        SOURCE.COLUMN_NAME, 
                        SOURCE.TRANSLATION_KEY, 
                        SOURCE.HIDE_IN_LIST, 
                        SOURCE.IS_PK, 
                        SOURCE.REQUIRED, 
                        SOURCE.HINT, 
                        SOURCE.HINT_TRANSLATION_KEY, 
                        SOURCE.SELECT_ITEMS_DATASOURCE, 
                        SOURCE.SELECT_ITEM_KEY, 
                        SOURCE.SELECT_ITEM_TEXT, 
                        SOURCE.SELECT_IS_MULTIPLE, 
                        SOURCE.HIDE_IN_FORM, 
                        SOURCE.SELECT_ITEMS_DATASOURCE_CONDITION, 
                        SOURCE.SELECT_ITEMS_LOOKUP_TYPE_CODE, 
                        SOURCE.READONLY, 
                        SOURCE.TABLE_ID, 
                        SOURCE.SELECT_ITEMS_ORDER_CONDITION,
                        SOURCE.TAB_CODE,
                        SOURCE.TAB_TRANSL_KEY,
                        SOURCE.FORM_FIELD_TYPE
                    );

        ------------------------------------------------------------------------------------
		--Compile return object
		------------------------------------------------------------------------------------
		SET @RET_JSON = '{ "status":"success", "error_code":0, "message":"Successfully updated Column Definiton." }';
		COMMIT;

	END TRY
	BEGIN CATCH
		------------------------------------------------------------------------------------
		--Compile return object
		------------------------------------------------------------------------------------
		SET @RET_JSON = '{ "status":"error", "error_code":"'+CAST(@@ERROR AS VARCHAR(10))+'", "message":"'+error_message()+'" }';
		rollback;

	END CATCH
END