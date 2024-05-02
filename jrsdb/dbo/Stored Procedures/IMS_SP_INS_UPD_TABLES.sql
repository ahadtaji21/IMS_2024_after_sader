
CREATE PROCEDURE dbo.IMS_SP_INS_UPD_TABLES
	@JSON_PAYLOAD NVARCHAR(MAX),
	@RET_JSON NVARCHAR(MAX) OUTPUT
AS
BEGIN
    /***********************************************************
    * Description: Create and update IMS_TABLES
    * Date:   2020-04-06
    * Author: n.migliore
    *
    * Changes
    * Date		Modified By			Comments
    ************************************************************
    *
    ************************************************************/
    DECLARE @ACTION_TYPE NVARCHAR(25)= JSON_VALUE(@JSON_PAYLOAD, '$.actionType');
    DECLARE @PAYLOAD NVARCHAR(MAX)= JSON_QUERY(@JSON_PAYLOAD, '$.jsonPayload');
	DECLARE @INPUT_DATASET TABLE (
		ID INT,
		TABLE_NAME VARCHAR(128),
		CREATE_SP_NAME VARCHAR(128),
		READ_SP_NAME VARCHAR(128),
		UPDATE_SP_NAME VARCHAR(128),
		DELETE_SP_NAME VARCHAR(128),
		TITLE_TRANSLATION_KEY VARCHAR(250)
	);


	------------------------------------------------------------------------------------
	--Recover payload data
	------------------------------------------------------------------------------------
    INSERT INTO @INPUT_DATASET(
        ID,
        TABLE_NAME,
        CREATE_SP_NAME,
        READ_SP_NAME,
        UPDATE_SP_NAME,
        DELETE_SP_NAME,
        TITLE_TRANSLATION_KEY
    )
    
	SELECT jd.ID,
        jd.TABLE_NAME,
        jd.CREATE_SP_NAME,
        jd.READ_SP_NAME,
        jd.UPDATE_SP_NAME,
        jd.DELETE_SP_NAME,
        jd.TITLE_TRANSLATION_KEY
	FROM OPENJSON(@PAYLOAD)
	WITH (
		ID INT,
		TABLE_NAME VARCHAR(128),
		CREATE_SP_NAME VARCHAR(128),
		READ_SP_NAME VARCHAR(128),
		UPDATE_SP_NAME VARCHAR(128),
		DELETE_SP_NAME VARCHAR(128),
		TITLE_TRANSLATION_KEY VARCHAR(250)
	)AS	jd;


    BEGIN TRANSACTION
	BEGIN TRY
		------------------------------------------------------------------------------------
		--Create or update data
		------------------------------------------------------------------------------------
		--Set ims_tables data
        MERGE dbo.IMS_TABLES AS TARGET
            USING @INPUT_DATASET AS SOURCE
            ON TARGET.ID = SOURCE.ID
                WHEN MATCHED
                THEN UPDATE SET
                TARGET.TABLE_NAME = SOURCE.TABLE_NAME,
                TARGET.CREATE_SP_NAME = SOURCE.CREATE_SP_NAME,
                TARGET.READ_SP_NAME = SOURCE.READ_SP_NAME,
                TARGET.UPDATE_SP_NAME = SOURCE.UPDATE_SP_NAME,
                TARGET.DELETE_SP_NAME = SOURCE.DELETE_SP_NAME,
                TARGET.TITLE_TRANSLATION_KEY = SOURCE.TITLE_TRANSLATION_KEY
            WHEN NOT MATCHED
            THEN
                INSERT(
                    TABLE_NAME,
                    CREATE_SP_NAME,
                    READ_SP_NAME,
                    UPDATE_SP_NAME,
                    DELETE_SP_NAME,
                    TITLE_TRANSLATION_KEY
                )
                VALUES(
                    SOURCE.TABLE_NAME,
                    SOURCE.CREATE_SP_NAME,
                    SOURCE.READ_SP_NAME,
                    SOURCE.UPDATE_SP_NAME,
                    SOURCE.DELETE_SP_NAME,
                    SOURCE.TITLE_TRANSLATION_KEY
                );
    

        ------------------------------------------------------------------------------------
		--Compile return object
		------------------------------------------------------------------------------------
		SET @RET_JSON = '{ "status":"success", "error_code":0, "message":"Successfully updated data." }';
		COMMIT;

    END TRY
	BEGIN CATCH
		------------------------------------------------------------------------------------
		--Compile return object
		------------------------------------------------------------------------------------
		SET @RET_JSON = '{ "status":"error", "error_code":"'+CAST(@@ERROR AS VARCHAR(10))+'", "message":"'+error_message()+'" }';
		rollback;

	END CATCH
END;