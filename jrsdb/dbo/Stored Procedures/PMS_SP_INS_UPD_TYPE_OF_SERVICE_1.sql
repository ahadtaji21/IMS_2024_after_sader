
CREATE PROCEDURE [dbo].[PMS_SP_INS_UPD_TYPE_OF_SERVICE]
	@JSON_PAYLOAD NVARCHAR(MAX), 
	@RET_JSON     NVARCHAR(MAX) = NULL OUTPUT
AS
BEGIN
	/***********************************************************
	* Description: Insert and Update Type of services.
	* Date:   03/27/2020
	* Author: n.migliore
	*
	* Changes
	* Date		Modified By			Comments
	************************************************************
	*	2020-03-30		n.migliore		Added management of "dbo.PMS_COI_TOS_REL"e
	************************************************************/
	DECLARE @ACTION_TYPE NVARCHAR(25)= JSON_VALUE(@JSON_PAYLOAD, '$.actionType');
    DECLARE @PAYLOAD NVARCHAR(MAX)= JSON_QUERY(@JSON_PAYLOAD, '$.jsonPayload');
    DECLARE @INPUT_DATASET TABLE (
		PMS_TOS_ID INT, 
        PMS_TOS_CODE varchar(50), 
        PMS_TOS_DESCRIPTION varchar(max), 
        PMS_TOS_ENABLED bit,
		selectHolder_coi_relations_placeholder nvarchar(max)
    );

	DECLARE @COI_RELATIONS TABLE(
		PMS_TOS_ID INT,
		PMS_COI_ID INT
	);

	------------------------------------------------------------------------------------
	--Recover payload data
	------------------------------------------------------------------------------------
    INSERT INTO @INPUT_DATASET
    (
        PMS_TOS_ID,
        PMS_TOS_CODE,
        PMS_TOS_DESCRIPTION,
        PMS_TOS_ENABLED,
		selectHolder_coi_relations_placeholder
    )
	SELECT jd.PMS_TOS_ID,
		jd.PMS_TOS_CODE,
		jd.PMS_TOS_DESCRIPTION,
		jd.PMS_TOS_ENABLED,
		jd.selectHolder_coi_relations_placeholder
	FROM OPENJSON(@PAYLOAD)
	WITH (
		PMS_TOS_ID int,
		PMS_TOS_CODE varchar(50),
		PMS_TOS_DESCRIPTION varchar(max),
		PMS_TOS_ENABLED bit,
		selectHolder_coi_relations_placeholder nvarchar(max) AS JSON
	) as jd

	INSERT INTO @COI_RELATIONS
	(
	    PMS_TOS_ID,
	    PMS_COI_ID
	)
	SELECT dataset.PMS_TOS_ID,
		relation.PMS_COI_ID
	FROM @INPUT_DATASET dataset
	CROSS APPLY OPENJSON(dataset.selectHolder_coi_relations_placeholder)
	WITH ( PMS_COI_ID int ) AS relation;


	------------------------------------------------------------------------------------
	--Check data validity
	------------------------------------------------------------------------------------
	if @ACTION_TYPE = 'Create'
		AND exists(		SELECT 1
						FROM @INPUT_DATASET AS jd
						WHERE jd.PMS_TOS_ID IS NOT null)
	BEGIN
		raiserror('Error in input data.', 16, 1);
	END;

	if @ACTION_TYPE = 'Update'
		AND exists(		SELECT 1
						FROM @INPUT_DATASET AS jd
						WHERE jd.PMS_TOS_ID IS null)
	BEGIN
		raiserror('Error in input data.', 16, 1);
	END;


	BEGIN TRANSACTION
	BEGIN TRY
		------------------------------------------------------------------------------------
		--Create or update data
		------------------------------------------------------------------------------------
		--Set Type of service data
		MERGE dbo.PMS_TYPE_OF_SERVICE AS target
			USING @INPUT_DATASET AS source
			ON target.PMS_TOS_ID = source.PMS_TOS_ID
				WHEN MATCHED
				THEN UPDATE SET 
								target.PMS_TOS_CODE = source.PMS_TOS_CODE,
								target.PMS_TOS_DESCRIPTION = source.PMS_TOS_DESCRIPTION,
								target.PMS_TOS_ENABLED = source.PMS_TOS_ENABLED
				WHEN NOT MATCHED
				THEN
					INSERT(
						PMS_TOS_CODE,
						PMS_TOS_DESCRIPTION,
						PMS_TOS_ENABLED
					)
					VALUES(
						source.PMS_TOS_CODE,
						source.PMS_TOS_DESCRIPTION,
						source.PMS_TOS_ENABLED
					);

		--Set Category of intervention relation
		MERGE dbo.PMS_COI_TOS_REL AS target
			using @COI_RELATIONS AS source
			ON target.PMS_TOS_ID = source.PMS_TOS_ID AND target.PMS_COI_ID = source.PMS_COI_ID
				WHEN NOT MATCHED
				THEN INSERT(
						PMS_COI_ID,
						PMS_TOS_ID
					)
					VALUES(
						source.PMS_COI_ID,
						source.PMS_TOS_ID
					)
				WHEN NOT MATCHED BY SOURCE
				THEN DELETE;


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
END