CREATE PROCEDURE dbo.HR_SP_INS_UPD_BIODATA @JSON_PAYLOAD NVARCHAR(MAX), 
                                           @RET_JSON     NVARCHAR(MAX) = NULL OUTPUT
AS
    BEGIN
        DECLARE @ACTION_TYPE NVARCHAR(25)= JSON_VALUE(@JSON_PAYLOAD, '$.actionType');
        DECLARE @PAYLOAD NVARCHAR(MAX)= JSON_VALUE(@JSON_PAYLOAD, '$.jsonPayload');
        DECLARE @JSON_DATA TABLE
        (PRIMARY_KEY                         INT, 
         HR_BIODATA_PERSONAL_TITLE_LOOKUP_ID INT, 
         HR_BIODATA_SURNAME                  NVARCHAR(150), 
         HR_BIODATA_MIDDLE_NAME              NVARCHAR(150), 
         HR_BIODATA_NAME                     NVARCHAR(150), 
         HR_BIODATA_GENEDER_LOOKUP_ID        INT, 
         HR_BIODATA_BIRTH_DATE               DATE, 
         HR_BIODATA_BIRTH_PLACE              NVARCHAR(150)
        );

        --Recover payload data
        INSERT INTO @JSON_DATA(
          PRIMARY_KEY, 
          HR_BIODATA_PERSONAL_TITLE_LOOKUP_ID, 
          HR_BIODATA_SURNAME, 
          HR_BIODATA_MIDDLE_NAME, 
          HR_BIODATA_NAME, 
          HR_BIODATA_GENEDER_LOOKUP_ID, 
          HR_BIODATA_BIRTH_DATE, 
          HR_BIODATA_BIRTH_PLACE
        )
        VALUES
        (JSON_VALUE(@JSON_PAYLOAD, '$.jsonPayload.PRIMARY_KEY'), -- PRIMARY_KEY - INT
         JSON_VALUE(@JSON_PAYLOAD, '$.jsonPayload.HR_BIODATA_PERSONAL_TITLE_LOOKUP_ID'), -- HR_BIODATA_PERSONAL_TITLE_LOOKUP_ID - INT
         JSON_VALUE(@JSON_PAYLOAD, '$.jsonPayload.HR_BIODATA_SURNAME'), -- HR_BIODATA_SURNAME - NVARCHAR
         JSON_VALUE(@JSON_PAYLOAD, '$.jsonPayload.HR_BIODATA_MIDDLE_NAME'), -- HR_BIODATA_MIDDLE_NAME - NVARCHAR
         JSON_VALUE(@JSON_PAYLOAD, '$.jsonPayload.HR_BIODATA_NAME'), -- HR_BIODATA_NAME - NVARCHAR
         JSON_VALUE(@JSON_PAYLOAD, '$.jsonPayload.HR_BIODATA_GENEDER_LOOKUP_ID'), -- HR_BIODATA_GENEDER_LOOKUP_ID - INT
         JSON_VALUE(@JSON_PAYLOAD, '$.jsonPayload.HR_BIODATA_BIRTH_DATE'), -- HR_BIODATA_BIRTH_DATE - date
         JSON_VALUE(@JSON_PAYLOAD, '$.jsonPayload.HR_BIODATA_BIRTH_PLACE') -- HR_BIODATA_BIRTH_PLACE - NVARCHAR
        );

        --Creation/Update
        IF @ACTION_TYPE = 'Create'
           OR @ACTION_TYPE = 'Update'
            BEGIN
                MERGE dbo.HR_BIODATA AS target
                USING @JSON_DATA AS source
                ON target.HR_BIODATA_ID = source.PRIMARY_KEY
                    WHEN MATCHED
                    THEN UPDATE SET 
                                    target.HR_BIODATA_PERSONAL_TITLE_LOOKUP_ID = source.HR_BIODATA_PERSONAL_TITLE_LOOKUP_ID, 
                                    target.HR_BIODATA_SURNAME = source.HR_BIODATA_SURNAME, 
                                    target.HR_BIODATA_MIDDLE_NAME = source.HR_BIODATA_MIDDLE_NAME, 
                                    target.HR_BIODATA_NAME = source.HR_BIODATA_NAME, 
                                    target.HR_BIODATA_GENEDER_LOOKUP_ID = source.HR_BIODATA_GENEDER_LOOKUP_ID, 
                                    target.HR_BIODATA_BIRTH_DATE = source.HR_BIODATA_BIRTH_DATE, 
                                    target.HR_BIODATA_BIRTH_PLACE = source.HR_BIODATA_BIRTH_PLACE
                    WHEN NOT MATCHED
                    THEN
                      INSERT(
                  HR_BIODATA_PERSONAL_TITLE_LOOKUP_ID, 
                  HR_BIODATA_SURNAME, 
                  HR_BIODATA_MIDDLE_NAME, 
                  HR_BIODATA_NAME, 
                  HR_BIODATA_GENEDER_LOOKUP_ID, 
                  HR_BIODATA_BIRTH_DATE, 
                  HR_BIODATA_BIRTH_PLACE)
                      VALUES
                (source.HR_BIODATA_PERSONAL_TITLE_LOOKUP_ID, 
                 source.HR_BIODATA_SURNAME, 
                 source.HR_BIODATA_MIDDLE_NAME, 
                 source.HR_BIODATA_NAME, 
                 source.HR_BIODATA_GENEDER_LOOKUP_ID, 
                 source.HR_BIODATA_BIRTH_DATE, 
                 source.HR_BIODATA_BIRTH_PLACE
                );
        END;

		--Return result
        SET @RET_JSON = '{ "status":"success", "error_code":0, "message":"Successfully updated data." }';
    END;