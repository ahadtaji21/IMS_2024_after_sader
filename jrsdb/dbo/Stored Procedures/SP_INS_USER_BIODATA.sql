
-- =============================================
-- Author:      Nicola Migliore
-- Create date: 2020-01-23
-- Description: Inserts a new "Admin Area" in the hierarchy.
-- =============================================
-- Parameters:
--   @userUID - uid of user. FK to dbo.IMS_USER
--   @surname - user surname
--   @middleName - user middlename
--   @personalTitleId - user title (e.g. "Mr"). FK to dbo.HR_PERSONAL_TITLE
--   @gender - user title (e.g. "Male"). FK to dbo.HR_GENDER
--   @birthDate - user birth date
--   @birthPlace - user birth place
--   @nationality - JSON list of nationalities
--   @religious - is user a religious figure?
--   @permanentLocationId - user's living location
--   @identificationDocs - JSON list of identification documents
--
-- Returns:
--   @resMessage - Description of result/error
--   @resStatus - Status code: { 0 : Success } or ERROR CODE
-- =============================================
-- Change History:
--	...
-- =============================================
CREATE PROCEDURE dbo.SP_INS_USER_BIODATA
    @userUID UNIQUEIDENTIFIER = null
    ,@surname NVARCHAR(150)
    ,@middleName NVARCHAR(150) = null
    ,@name NVARCHAR(150)
    ,@personalTitleId int = NULL
    ,@genderId int = null
    ,@birthDate date
    ,@birthPlace varchar(150)
    ,@nationality varchar(2000)
    ,@religious bit = 0
    ,@permanentLocationId int = NULL
    ,@identificationDocs varchar(2000)
    ,@resMessage nvarchar(150) output
    ,@resStatus int output
AS
BEGIN
    BEGIN TRY

        declare @TMP_USER_INFO table ( TMP_USER_UID UNIQUEIDENTIFIER);

        ---------------------------------------------------------------------------------
        --Check for formal errors
        ---------------------------------------------------------------------------------
        --Check existance of user and biodata
        if @userUID is not NULL
            and EXISTS (    select 1
                            from dbo.IMS_USER as u
                                inner join dbo.HR_BIODATA as biodata on biodata.HR_BIODATA_USER_UID = u.IMS_USER_UID
                            where u.IMS_USER_UID = @userUID )
        BEGIN
            set @resStatus = -1;
            set @resMessage = 'Aborting. User UID already linked to BIODATA.';
            return;
        END;

        --Check lookups
        if @personalTitleId is not null
            and not exists( select 1
                            from HR_PERSONAL_TITLE as persTitle
                            where persTitle.HR_PERSONAL_TITLE_ID = @personalTitleId)
        BEGIN
            set @resStatus = -1;
            set @resMessage = 'Aborting. Provided "personal title" not defined.';
            return;
        END;
        if @genderId is not null
            and not exists( select 1
                            from HR_GENDER as gender
                            where gender.HR_GENDER_ID = @genderId)
        BEGIN
            set @resStatus = -1;
            set @resMessage = 'Aborting. Provided "gender" not defined.';
            return;
        END;

        --raiserror('Aborting. Admin Area already registered', 16, 1);

        BEGIN TRANSACTION

        ---------------------------------------------------------------------------------
        --Set optional values
        ---------------------------------------------------------------------------------
        --Recover @genderId
        SELECT @genderId = ISNULL(@genderId, HR_GENDER_ID)
        FROM HR_GENDER AS GENDER
        WHERE GENDER.HR_GENDER_CODE = 'U';

        --Generate @userUID
        IF @userUID is NULL
        BEGIN
            insert into dbo.IMS_USER ( IMS_USER_UID, IMS_USER_CREATION_DATE )
            OUTPUT inserted.IMS_USER_UID into @TMP_USER_INFO
            VALUES (newid(), getdate());
        END

        --If created, set new user UID
        SELECT @userUID = ISNULL(TMP.TMP_USER_UID, @userUID)
        FROM @TMP_USER_INFO AS TMP;


        ---------------------------------------------------------------------------------
        --Insert BIODATA
        ---------------------------------------------------------------------------------
        insert into HR_BIODATA (
            HR_BIODATA_USER_UID
            ,HR_BIODATA_PERSONAL_TITLE_ID
            ,HR_BIODATA_SURNAME
            ,HR_BIODATA_MIDDLE_NAME
            ,HR_BIODATA_NAME
            ,HR_BIODATA_GENDER_ID
            ,HR_BIODATA_BIRTH_DATE
            ,HR_BIODATA_BIRTH_PLACE
            ,HR_BIODATA_NATIONALITY
            ,HR_BIODATA_RELIGIOUS
            ,HR_BIODATA_PERMANENT_LOCATION_ID
            ,HR_BIODATA_IDENTIFICATION_DOCUMENTS
        )
        VALUES (
            @userUID
            ,@personalTitleId
            ,@surname
            ,@middleName
            ,@name
            ,@genderId
            ,@birthDate
            ,@birthPlace
            ,@nationality
            ,@religious
            ,@permanentLocationId
            ,@identificationDocs
        );

        --Return and close transaction
        set @resStatus = 0;
        set @resMessage = 'Successfully inserte new User: ' + @name + ' ' + @surname;
        commit;

    END TRY
    BEGIN CATCH
        rollback
		
        set @resStatus = error_number();
        set @resMessage = error_message();

    END CATCH
END