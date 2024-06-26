﻿CREATE Procedure [dbo].[SP_INS_BIODATA]
(
@JRS_ID varchar(100),
@PI_GUID uniqueidentifier,
@PI_Title varchar(max),
@PI_Surname varchar(max),
@PI_Middle_Name varchar(max),
@PI_Name varchar(max),
@PI_Gender varchar(max),
@PI_Birth_Date date,
@PI_Birth_Place varchar(max),
@PI_Nationality varchar(max),
@PI_Religious bit,
@PI_Country varchar(max),
@PI_District varchar(max),
@PI_City varchar(max),
@PI_Address varchar(max), 
@PI_Doc_type varchar(max),
@PI_Doc_Number varchar(max),
@PI_Doc_Expiry_Date date,
@EI_Position_Scope varchar(max),
@EI_Function varchar(max),
@EI_Level varchar(max),
@EI_Job_Title varchar(max),
@EI_Contract_Type varchar(max),
@EI_Work_Hours varchar(max),
@EI_Employment_Date_from date null,
@EI_Employment_Date_to date null,
@EI_Region varchar(max),
@EI_Country varchar(max),
@EI_City varchar(max),
@EI_District varchar(max),
@EI_Department varchar(max),
@EI_Project_code varchar(max)
)

AS
BEGIN
DECLARE @HR_BIODATA_GENDER_ID INT =-1
DECLARE @HR_BIODATA_PERSONAL_TITLE_ID INT =-1
DECLARE @HR_DOCUMENT_TYPE_ID INT =-1
DECLARE @HR_AGREEMENT_TYPE_ID INT =-1
DECLARE @HR_DEPARTMENT_ID INT =-1
DECLARE @HR_TITLE_FUNCTION_ID INT = -1
DECLARE @HR_TITLE_SCOPE_ID INT = -1
DECLARE @HR_AGREEMENT_ID INT = -1
DECLARE @HR_TITLE_LEVEL_ID INT = -1
DECLARE @HR_POSITION_PROJECT_ID INT = -1
DECLARE @PI_COUNTRY_IMS_ADMIN_AREA_ID INT = -1
DECLARE @PI_DISTRICT_IMS_ADMIN_AREA_ID INT = -1
DECLARE @PI_CITY_IMS_ADMIN_AREA_ID INT = -1
DECLARE @EI_REGION_IMS_ADMIN_AREA_ID INT = -1
DECLARE @EI_COUNTRY_IMS_ADMIN_AREA_ID INT = -1
DECLARE @EI_CITY_IMS_ADMIN_AREA_ID INT = -1
DECLARE @HR_BIODATA_PERMANENT_LOCATION_ID INT = -1

DECLARE @IMS_ADMIN_AREA_TYPE_ID_REGIO INT = -1
DECLARE @IMS_ADMIN_AREA_TYPE_ID_COUNT INT = -1
DECLARE @IMS_ADMIN_AREA_TYPE_ID_PROVI INT = -1
DECLARE @IMS_ADMIN_AREA_TYPE_ID_MUNIC INT = -1
DECLARE @HR_POSITION_LOCATION_ID INT = -1
DECLARE @INSERTED_ID INT

SELECT @IMS_ADMIN_AREA_TYPE_ID_REGIO=[IMS_ADMIN_AREA_TYPE_ID]
	FROM [dbo].[IMS_ADMIN_AREA_TYPE]
	WHERE [IMS_ADMIN_AREA_TYPE_CODE]='REGIO'
SELECT @IMS_ADMIN_AREA_TYPE_ID_COUNT=[IMS_ADMIN_AREA_TYPE_ID]
	FROM [dbo].[IMS_ADMIN_AREA_TYPE]
	WHERE [IMS_ADMIN_AREA_TYPE_CODE]='COUNT'
SELECT @IMS_ADMIN_AREA_TYPE_ID_PROVI=[IMS_ADMIN_AREA_TYPE_ID]
	FROM [dbo].[IMS_ADMIN_AREA_TYPE]
	WHERE [IMS_ADMIN_AREA_TYPE_CODE]='PROVI'
SELECT @IMS_ADMIN_AREA_TYPE_ID_MUNIC=[IMS_ADMIN_AREA_TYPE_ID]
	FROM [dbo].[IMS_ADMIN_AREA_TYPE]
	WHERE [IMS_ADMIN_AREA_TYPE_CODE]='MUNIC'
BEGIN TRY
BEGIN TRAN
--REGION
SELECT 
  @EI_REGION_IMS_ADMIN_AREA_ID= [IMS_ADMIN_AREA_ID]
  FROM [dbo].[IMS_ADMIN_AREA] WHERE
  [IMS_ADMIN_AREA_NAME]=@PI_Country AND
  [IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID] =@IMS_ADMIN_AREA_TYPE_ID_COUNT

--END REGION
--COUNTRY
--PRINT '@EI_Country:'+@EI_Country
--PRINT 'TYPE:'+convert(varchar,@IMS_ADMIN_AREA_TYPE_ID_COUNT)

SELECT 

  @EI_COUNTRY_IMS_ADMIN_AREA_ID= [IMS_ADMIN_AREA_ID]
  FROM [dbo].[IMS_ADMIN_AREA] WHERE
  [IMS_ADMIN_AREA_NAME]=@EI_Country AND
  [IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID] =@IMS_ADMIN_AREA_TYPE_ID_COUNT

SELECT 
  @PI_COUNTRY_IMS_ADMIN_AREA_ID= [IMS_ADMIN_AREA_ID]
  FROM [dbo].[IMS_ADMIN_AREA] WHERE
  [IMS_ADMIN_AREA_NAME]=@PI_Country AND
  [IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID] =@IMS_ADMIN_AREA_TYPE_ID_COUNT
--END COUNTRY
--DISTRICT
--PRINT 'District:'+@PI_District
--PRINT 'TYPE:'+convert(varchar,@IMS_ADMIN_AREA_TYPE_ID_PROVI)

SELECT 
  @PI_DISTRICT_IMS_ADMIN_AREA_ID= [IMS_ADMIN_AREA_ID]
  FROM [dbo].[IMS_ADMIN_AREA] WHERE
  [IMS_ADMIN_AREA_NAME]=@PI_District AND
  [IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID] =@IMS_ADMIN_AREA_TYPE_ID_PROVI

--PRINT 'FIRST TRY:'+convert(varchar,@PI_DISTRICT_IMS_ADMIN_AREA_ID)
--PRINT '@PI_COUNTRY_IMS_ADMIN_AREA_ID:'+convert(varchar,@PI_COUNTRY_IMS_ADMIN_AREA_ID)

IF @PI_DISTRICT_IMS_ADMIN_AREA_ID=-1
BEGIN
	EXEC [dbo].[SP_INS_ADMIN_AREA] 
	@PI_COUNTRY_IMS_ADMIN_AREA_ID,
	@IMS_ADMIN_AREA_TYPE_ID_PROVI,
	@PI_District,
	null,
	null,
	null
--PRINT 'INSERTED DISTRICT'

SELECT 
  @PI_DISTRICT_IMS_ADMIN_AREA_ID= [IMS_ADMIN_AREA_ID]
  FROM [dbo].[IMS_ADMIN_AREA] WHERE
  [IMS_ADMIN_AREA_NAME]=@PI_District AND
  [IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID] =@IMS_ADMIN_AREA_TYPE_ID_PROVI
END
--PRINT '@PI_DISTRICT_IMS_ADMIN_AREA_ID:'+convert(varchar,@PI_DISTRICT_IMS_ADMIN_AREA_ID)
--END DISTRICT
--CITY
--PRINT 'CITY:'+@EI_City
--PRINT 'TYPE:'+convert(varchar,@IMS_ADMIN_AREA_TYPE_ID_MUNIC)

SELECT 
  @EI_CITY_IMS_ADMIN_AREA_ID= [IMS_ADMIN_AREA_ID]
  FROM [dbo].[IMS_ADMIN_AREA] WHERE
  [IMS_ADMIN_AREA_NAME]=@EI_City AND
  [IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID] =@IMS_ADMIN_AREA_TYPE_ID_MUNIC
--PRINT 'FIRST TRY:'+convert(varchar,@EI_CITY_IMS_ADMIN_AREA_ID)
--PRINT '@EI_COUNTRY_IMS_ADMIN_AREA_ID:'+convert(varchar,@EI_COUNTRY_IMS_ADMIN_AREA_ID)


IF @EI_CITY_IMS_ADMIN_AREA_ID=-1
BEGIN
	EXEC [dbo].[SP_INS_ADMIN_AREA] 
	@EI_COUNTRY_IMS_ADMIN_AREA_ID,
	@IMS_ADMIN_AREA_TYPE_ID_MUNIC,
	@EI_City,
	null,
	null,
	null
--PRINT 'INSERTED CITY'
SELECT 
  @EI_CITY_IMS_ADMIN_AREA_ID= [IMS_ADMIN_AREA_ID]
  FROM [dbo].[IMS_ADMIN_AREA] WHERE
  [IMS_ADMIN_AREA_NAME]=@EI_City AND
  [IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID] =@IMS_ADMIN_AREA_TYPE_ID_MUNIC
END
--PRINT '@EI_CITY_IMS_ADMIN_AREA_ID:'+convert(varchar,@EI_CITY_IMS_ADMIN_AREA_ID)

SELECT 
  @PI_CITY_IMS_ADMIN_AREA_ID= [IMS_ADMIN_AREA_ID]
  FROM [dbo].[IMS_ADMIN_AREA] WHERE
  [IMS_ADMIN_AREA_NAME]=@PI_City AND
  [IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID] =@IMS_ADMIN_AREA_TYPE_ID_MUNIC
IF @PI_CITY_IMS_ADMIN_AREA_ID=-1
BEGIN
	EXEC [dbo].[SP_INS_ADMIN_AREA] 
	@PI_DISTRICT_IMS_ADMIN_AREA_ID,
	@IMS_ADMIN_AREA_TYPE_ID_MUNIC,
	@PI_City,
	null,
	null,
	null
SELECT 
  @PI_CITY_IMS_ADMIN_AREA_ID= [IMS_ADMIN_AREA_ID]
  FROM [dbo].[IMS_ADMIN_AREA] WHERE
  [IMS_ADMIN_AREA_NAME]=@PI_City AND
  [IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID] =@IMS_ADMIN_AREA_TYPE_ID_PROVI
END
--END CITY

SELECT @HR_POSITION_PROJECT_ID=[IMS_PROJECT_ID]
  FROM [dbo].[IMS_PROJECT] WHERE
  UPPER([IMS_PROJECT_CODE])=UPPER(@EI_Project_code)

SELECT @HR_TITLE_LEVEL_ID=[HR_TITLE_LEVEL_ID]
  FROM [dbo].[HR_TITLE_LEVEL] WHERE
  UPPER([HR_TITLE_LEVEL_DESCR]) =@EI_Level

SELECT @HR_TITLE_FUNCTION_ID=[HR_TITLE_FUNCTION_ID]
  FROM [dbo].[HR_TITLE_FUNCTION] WHERE
  UPPER([HR_TITLE_FUNCTION_DESCR])=UPPER(@EI_Function)

SELECT @HR_TITLE_SCOPE_ID=[HR_TITLE_SCOPE_ID]
  FROM [dbo].[HR_TITLE_SCOPE] WHERE  
  UPPER([HR_TITLE_SCOPE_DESCR])=UPPER(@EI_Position_Scope)

SELECT @HR_DEPARTMENT_ID=[HR_DEPARTMENT_ID]
  FROM [dbo].[HR_DEPARTMENT] WHERE
UPPER([HR_DEPARTMENT_DESCR])=UPPER(@EI_Department)

SELECT @HR_AGREEMENT_TYPE_ID=[HR_AGREEMENT_TYPE_ID]
  FROM [dbo].[HR_AGREEMENT_TYPE] WHERE
  UPPER([HR_AGREEMENT_TYPE_NAME])=UPPER(@EI_Contract_Type)

SELECT @HR_BIODATA_GENDER_ID=HR_GENDER_ID FROM
[dbo].[HR_GENDER] WHERE
UPPER(HR_GENDER_DESCR)=UPPER(@PI_Gender)

SELECT @HR_BIODATA_PERSONAL_TITLE_ID=HR_PERSONAL_TITLE_ID FROM
[dbo].[HR_PERSONAL_TITLE] WHERE
UPPER(HR_PERSONAL_TITLE_DESCR)=UPPER(@PI_Title)

SELECT @HR_DOCUMENT_TYPE_ID=HR_DOCUMENT_TYPE_ID FROM
[dbo].[HR_DOCUMENT_TYPE] WHERE
UPPER(HR_DOCUMENT_TYPE_DESCR)=UPPER(@PI_Doc_type)


INSERT INTO [dbo].[IMS_USER]
           ([IMS_USER_UID]
           ,[IMS_USER_CREATION_DATE])
     VALUES
           (@PI_GUID
           ,getdate())


INSERT INTO [dbo].[IMS_LOCATION]
           ([IMS_LOCATION_ADMIN_AREA_ID_1]
           ,[IMS_LOCATION_ADMIN_AREA_ID_2]
           ,[IMS_LOCATION_ADMIN_AREA_ID_3]
           ,[IMS_LOCATION_ADDRESS]
           ,[IMS_LOCATION_HISTORIC_ADDRESS]
           ,[IMS_LOCATION_LAT]
           ,[IMS_LOCATION_LON]
           ,[IMS_LOCATION_CODE])
     VALUES
           (@PI_COUNTRY_IMS_ADMIN_AREA_ID
           ,@PI_DISTRICT_IMS_ADMIN_AREA_ID
           ,@PI_CITY_IMS_ADMIN_AREA_ID
           ,@PI_Address
           ,null
           ,null
           ,null
           ,null)
SELECT @HR_BIODATA_PERMANENT_LOCATION_ID = @@IDENTITY

INSERT INTO [dbo].[IMS_LOCATION]
           ([IMS_LOCATION_ADMIN_AREA_ID_1]
           ,[IMS_LOCATION_ADMIN_AREA_ID_2]
           ,[IMS_LOCATION_ADMIN_AREA_ID_3]
           ,[IMS_LOCATION_ADDRESS]
           ,[IMS_LOCATION_HISTORIC_ADDRESS]
           ,[IMS_LOCATION_LAT]
           ,[IMS_LOCATION_LON]
           ,[IMS_LOCATION_CODE])
     VALUES
           (@EI_REGION_IMS_ADMIN_AREA_ID
           ,@EI_COUNTRY_IMS_ADMIN_AREA_ID
           ,@EI_CITY_IMS_ADMIN_AREA_ID
           ,''
           ,null
           ,null
           ,null
           ,null)
SELECT  @HR_POSITION_LOCATION_ID = @@IDENTITY

INSERT INTO [dbo].[HR_BIODATA]
           ([HR_BIODATA_REGNUMBER]
           ,[HR_BIODATA_PERSONAL_TITLE_ID]
           ,[HR_BIODATA_SURNAME]
           ,[HR_BIODATA_MIDDLE_NAME]
           ,[HR_BIODATA_NAME]
           ,[HR_BIODATA_GENDER_ID]
           ,[HR_BIODATA_BIRTH_DATE]
           ,[HR_BIODATA_BIRTH_PLACE]
           ,[HR_BIODATA_NATIONALITY]
           ,[HR_BIODATA_RELIGIOUS]
           ,[HR_BIODATA_PERMANENT_LOCATION_ID]
		   ,[HR_BIODATA_USER_UID]
           )
     VALUES
           (@JRS_ID
           ,@HR_BIODATA_PERSONAL_TITLE_ID
           ,@PI_Surname
           ,@PI_Middle_Name
           ,@PI_Name
           ,@HR_BIODATA_GENDER_ID
           ,@PI_Birth_Date
           ,@PI_Birth_Place
           ,@PI_Nationality
           ,@PI_Religious
           ,@HR_BIODATA_PERMANENT_LOCATION_ID
		   ,@PI_GUID
		   )
SELECT @INSERTED_ID=@@IDENTITY
INSERT INTO [dbo].[HR_DOCUMENT]
           ([HR_BIODATA_ID]
           ,[HR_DOCUMENT_TYPE_ID]
           ,[HR_DOCUMENT_NUMBER]
           ,[HR_DOCUMENT_EXPIRY_DATE])
     VALUES
           (@INSERTED_ID
           ,@HR_DOCUMENT_TYPE_ID
           ,@PI_Doc_Number
           ,@PI_Doc_Expiry_Date)

INSERT INTO [dbo].[HR_AGREEMENT]
           ([IMS_USER_UID]
           ,[HR_EMPLOYEMENT_DATE_FROM]
           ,[HR_EMPLOYEMENT_DATE_TO]
           ,[HR_WORK_HOURS])
     VALUES
           (@PI_GUID,@EI_Employment_Date_from,@EI_Employment_Date_to,@EI_Work_Hours)
SELECT @HR_AGREEMENT_ID=@@IDENTITY
INSERT INTO [dbo].[HR_POSITION]
           ([HR_POSITION_STATUS_ID]
           ,[HR_POSITION_SCOPE_ID]
           ,[HR_POSITION_FUNCTION_ID]
           ,[HR_POSITION_LEVEL_ID]
           ,[HR_POSITION_DEPARTMENT_ID]
           ,[HR_POSITION_DESCR]
           ,[HR_POSITION_SUPERIOR_POSITION_ID]
           ,[HR_POSITION_SKILLS]
           ,[HR_POSITION_PROJECT_ID]
           ,[HR_POSITION_AGREEMENT_ID]
           ,[HR_POSITION_AGREEMENT_TYPE_ID]
		   ,[HR_POSITION_LOCATION_ID])
     VALUES
           (5
           ,@HR_TITLE_SCOPE_ID
           ,@HR_TITLE_FUNCTION_ID
           ,@HR_TITLE_LEVEL_ID
           ,@HR_DEPARTMENT_ID
           ,@EI_Job_Title
           ,null
           ,null
           ,@HR_POSITION_PROJECT_ID
           ,@HR_AGREEMENT_ID
           ,@HR_AGREEMENT_TYPE_ID
		   ,@HR_POSITION_LOCATION_ID)
COMMIT
END TRY
BEGIN CATCH
	ROLLBACK;
	THROW; 
END CATCH
END