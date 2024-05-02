

-- Create the stored procedure in the specified schema
CREATE   PROCEDURE [dbo].[IMS_SP_INS_UPD_NARRATIVE_SECTION]
	@JSON_PAYLOAD NVARCHAR(MAX),
	@RET_JSON NVARCHAR(MAX) OUTPUT
AS
    /***********************************************************
    * Description: Create and update narrative sections.
    * Date:   2020-04-14
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
		ID int ,
        REFERENCE_ID int,
        CODE varchar(25),
        SECTION_TITLE varchar(250),
        SECTION_TEXT NVARCHAR(MAX),
        NARRATIVE_SECTION_TYPE_ID INT,
        REMOVED BIT
    );


    ------------------------------------------------------------------------------------
	--Recover payload data
	------------------------------------------------------------------------------------
    INSERT INTO @INPUT_DATASET(
        ID,
        REFERENCE_ID,
        CODE,
        SECTION_TITLE,
        SECTION_TEXT,
        REMOVED
    )
    SELECT jd.ID,
        jd.REFERENCE_ID,
        jd.CODE,
        jd.SECTION_TITLE,
        jd.SECTION_TEXT,
        isnull(jd.REMOVED, cast(0 as bit))
    FROM OPENJSON(@PAYLOAD)
	WITH (
        ID int ,
        REFERENCE_ID int,
        CODE varchar(25),
        SECTION_TITLE varchar(250),
        SECTION_TEXT NVARCHAR(MAX),
        REMOVED BIT
    )AS jd;


    ------------------------------------------------------------------------------------
    --Recover missing data
    ------------------------------------------------------------------------------------
    UPDATE DATASET
    SET NARRATIVE_SECTION_TYPE_ID = T.ID
    FROM @INPUT_DATASET AS DATASET
        INNER JOIN dbo.IMS_NARRATIVE_SECTION_TYPE AS T ON T.CODE = DATASET.CODE;


    BEGIN TRANSACTION
	BEGIN TRY
        ------------------------------------------------------------------------------------
		--Delete custom narrative sections
		------------------------------------------------------------------------------------
        DELETE
		FROM dbo.IMS_NARRATIVE_SECTION
        WHERE EXISTS(
            SELECT 1
            FROM @INPUT_DATASET AS I
            WHERE I.ID = dbo.IMS_NARRATIVE_SECTION.ID
                AND I.REMOVED = 1 
        );

        DELETE FROM @INPUT_DATASET
        WHERE REMOVED = 1;


		------------------------------------------------------------------------------------
		--Create or update data
		------------------------------------------------------------------------------------
		--Set project data
        MERGE dbo.IMS_NARRATIVE_SECTION AS TARGET
            USING @INPUT_DATASET AS SOURCE
            ON TARGET.ID = SOURCE.ID
                WHEN MATCHED
                THEN UPDATE SET
                    TARGET.NARRATIVE_SECTION_TYPE_ID = SOURCE.NARRATIVE_SECTION_TYPE_ID,
                    TARGET.REFERENCE_ID = SOURCE.REFERENCE_ID,
                    TARGET.SECTION_TITLE = SOURCE.SECTION_TITLE,
                    TARGET.SECTION_TEXT = SOURCE.SECTION_TEXT
                WHEN NOT MATCHED
                THEN INSERT(
                    NARRATIVE_SECTION_TYPE_ID,
                    REFERENCE_ID,
                    SECTION_TITLE,
                    SECTION_TEXT
                )
                VALUES(
                    SOURCE.NARRATIVE_SECTION_TYPE_ID,
                    SOURCE.REFERENCE_ID,
                    SOURCE.SECTION_TITLE,
                    SOURCE.SECTION_TEXT
                );


        ------------------------------------------------------------------------------------
		--Compile return object
		------------------------------------------------------------------------------------
		SET @RET_JSON = '{ "status":"success", "error_code":0, "message":"Successfully updated Narrative section." }';
		COMMIT;

	END TRY
	BEGIN CATCH
		------------------------------------------------------------------------------------
		--Compile return object
		------------------------------------------------------------------------------------
		SET @RET_JSON = '{ "status":"error", "error_code":"'+CAST(@@ERROR AS VARCHAR(10))+'", "message":"'+error_message()+'" }';
		rollback;

	END CATCH