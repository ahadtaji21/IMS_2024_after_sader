
CREATE PROCEDURE dbo.PMS_SP_DEL_TYPE_OF_SERVICE
	@JSON_PAYLOAD NVARCHAR(MAX), 
	@RET_JSON     NVARCHAR(MAX) = NULL OUTPUT
AS
BEGIN
	/***********************************************************
	* Description: Logical deletion of Type of services.
	* Date:   03/27/2020
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
		PMS_TOS_ID INT, 
        PMS_TOS_CODE varchar(50), 
        PMS_TOS_DESCRIPTION varchar(max), 
        PMS_TOS_ENABLED bit
    );

	------------------------------------------------------------------------------------
	--Recover payload data
	------------------------------------------------------------------------------------
    INSERT INTO @INPUT_DATASET
    (
        PMS_TOS_ID,
        PMS_TOS_CODE,
        PMS_TOS_DESCRIPTION,
        PMS_TOS_ENABLED
    )
	SELECT jd.PMS_TOS_ID,
		jd.PMS_TOS_CODE,
		jd.PMS_TOS_DESCRIPTION,
		jd.PMS_TOS_ENABLED
	FROM OPENJSON(@PAYLOAD)
	WITH (
		PMS_TOS_ID int,
		PMS_TOS_CODE varchar(50),
		PMS_TOS_DESCRIPTION varchar(max),
		PMS_TOS_ENABLED bit
	) as jd


	BEGIN TRANSACTION
	BEGIN TRY
		------------------------------------------------------------------------------------
		--Logical deletion
		------------------------------------------------------------------------------------
		UPDATE ptos
		SET ptos.PMS_TOS_DELETED = 1
		FROM @INPUT_DATASET dataset
			INNER JOIN dbo.PMS_TYPE_OF_SERVICE ptos ON dataset.PMS_TOS_ID = ptos.PMS_TOS_ID;


		------------------------------------------------------------------------------------
		--Compile return object
		------------------------------------------------------------------------------------
		SET @RET_JSON = '{ "status":"success", "error_code":0, "message":"Successfully deleted the type of service." }';
		COMMIT;

	END TRY
	BEGIN CATCH
		PRINT 'catch'
		------------------------------------------------------------------------------------
		--Compile return object
		------------------------------------------------------------------------------------
		SET @RET_JSON = '{ "status":"error", "error_code":"'+CAST(@@ERROR AS VARCHAR(10))+'", "message":"'+error_message()+'" }';
		rollback;

	END CATCH
END