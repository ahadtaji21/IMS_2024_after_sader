
CREATE PROCEDURE dbo.IMS_SP_INS_UPD_DEL_MENU
	@JSON_PAYLOAD NVARCHAR(MAX),
	@RET_JSON NVARCHAR(MAX) OUTPUT
AS
    /***********************************************************
    * Description: Create, update and delete MENU.
    * Date:   2020-04-20
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
		ID int,
        DESCR NVARCHAR(100),
        URL NVARCHAR(100),
        PARENT_MENU_ID INT,
        PARENT_DESCR NVARCHAR(100),
        LABEL_KEY VARCHAR(100),
        ICON_CODE VARCHAR(50)
    );


    ------------------------------------------------------------------------------------
	--Recover payload data
	------------------------------------------------------------------------------------
    INSERT INTO @INPUT_DATASET(
        ID,
        DESCR,
        URL,
        PARENT_MENU_ID,
        PARENT_DESCR,
        LABEL_KEY,
        ICON_CODE
    )
    SELECT ID,
        DESCR,
        URL,
        PARENT_MENU_ID,
        PARENT_DESCR,
        LABEL_KEY,
        ICON_CODE
    FROM OPENJSON(@PAYLOAD)
	WITH (
        ID int ,
        DESCR NVARCHAR(100),
        URL NVARCHAR(100),
        PARENT_MENU_ID INT,
        PARENT_DESCR NVARCHAR(100),
        LABEL_KEY VARCHAR(100),
        ICON_CODE VARCHAR(50)
    )AS jd;


    ------------------------------------------------------------------------------------
	--Check conditions
	------------------------------------------------------------------------------------
    if @ACTION_TYPE = 'Destroy'
        AND EXISTS(
            SELECT 1
            FROM @INPUT_DATASET AS DATASET
                INNER JOIN MENU AS CHILD ON CHILD.PARENT_MENU_ID = DATASET.ID
        )
    BEGIN
        raiserror('Error: MENU may not be deleted. There is one or more chuild MENU depending on it.', 16, 1);
    END;


    BEGIN TRANSACTION
    BEGIN TRY
        ------------------------------------------------------------------------------------
        --Destroy data
        ------------------------------------------------------------------------------------
        if @ACTION_TYPE = 'Destroy'
        BEGIN
            DELETE 
            FROM MENU
            WHERE ID IN (
                SELECT ID
                FROM @INPUT_DATASET );
        END;


        ------------------------------------------------------------------------------------
        --Create or update data
        ------------------------------------------------------------------------------------
        IF @ACTION_TYPE IN ( 'Create', 'Update')
        BEGIN
            MERGE dbo.MENU AS TARGET
                USING @INPUT_DATASET AS source
                ON TARGET.ID = SOURCE.ID
                    WHEN MATCHED
                    THEN UPDATE SET
                        TARGET.DESCR = SOURCE.DESCR,
                        TARGET.URL = SOURCE.URL,
                        TARGET.PARENT_MENU_ID = SOURCE.PARENT_MENU_ID,
                        TARGET.LABEL_KEY = SOURCE.LABEL_KEY,
                        TARGET.ICON_CODE = SOURCE.ICON_CODE
                    WHEN NOT MATCHED
                    THEN INSERT (
                        DESCR,
                        URL,
                        PARENT_MENU_ID,
                        LABEL_KEY,
                        ICON_CODE
                    )
                    VALUES(
                        SOURCE.DESCR,
                        SOURCE.URL,
                        SOURCE.PARENT_MENU_ID,
                        SOURCE.LABEL_KEY,
                        SOURCE.ICON_CODE
                    );
        END;


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

    END CATCH;