

create PROCEDURE [dbo].[PMS_SP_INS_UPD_PROJECT]
	@JSON_PAYLOAD NVARCHAR(MAX),
	@RET_JSON NVARCHAR(MAX) OUTPUT
AS
BEGIN
	/***********************************************************
	* Description: 
	* Date:   04/02/2020
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
		CODE VARCHAR(50),
		DESCR VARCHAR(250),
		LOCATION_DESCR VARCHAR(250),
		COUNTRY_ADMIN_AREA_ID INT,
		START_YEAR INT,
		STATUS_ID INT
	)


	------------------------------------------------------------------------------------
	--Recover payload data
	------------------------------------------------------------------------------------
	INSERT INTO @INPUT_DATASET
	(
	    ID,
	    CODE,
	    DESCR,
	    LOCATION_DESCR,
	    COUNTRY_ADMIN_AREA_ID,
	    START_YEAR,
	    STATUS_ID
	)
	SELECT jd.ID,
		jd.CODE,
		jd.DESCR,
		jd.LOCATION_DESCR,
		jd.COUNTRY_ADMIN_AREA_ID,
		jd.START_YEAR,
		jd.STATUS_ID
	FROM OPENJSON(@PAYLOAD)
	WITH (
		ID INT,
		CODE VARCHAR(50),
		DESCR VARCHAR(250),
		LOCATION_DESCR VARCHAR(250),
		COUNTRY_ADMIN_AREA_ID INT,
		START_YEAR INT,
		STATUS_ID INT
	)AS	jd


	BEGIN TRANSACTION
	BEGIN TRY
		------------------------------------------------------------------------------------
		--Create or update data
		------------------------------------------------------------------------------------
		--Set project data
		MERGE dbo.PMS_PROJECT AS target
			USING @INPUT_DATASET AS source
			ON target.ID = source.ID
				WHEN MATCHED
				THEN UPDATE SET
					target.CODE = source.CODE,
					target.DESCR = source.DESCR,
					target.LOCATION_DESCR = source.LOCATION_DESCR,
					target.COUNTRY_ADMIN_AREA_ID = source.COUNTRY_ADMIN_AREA_ID,
					target.STATUS_ID = source.STATUS_ID,
					target.START_YEAR = source.START_YEAR
				WHEN NOT MATCHED
				THEN
					INSERT(
						CODE,
						DESCR,
						LOCATION_DESCR,
						COUNTRY_ADMIN_AREA_ID,
						STATUS_ID,
						START_YEAR
					)
					VALUES(
						source.CODE,
						source.DESCR,
						source.LOCATION_DESCR,
						source.COUNTRY_ADMIN_AREA_ID,
						source.STATUS_ID,
						source.START_YEAR
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