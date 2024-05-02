-- =============================================
-- Author:      Nicola Migliore
-- Create date: 2020-02-26
-- Description: Returns the menus for a given user.
-- =============================================
-- Parameters:
--   @USER_UID - uid of the user for which to recorver the menus.
--
-- Returns:    List of valid menu
-- =============================================
-- Change History:
--	...
-- =============================================
CREATE FUNCTION dbo.IMS_UF_MENU_FOR_USER (
    @USER_UID UNIQUEIDENTIFIER
)
RETURNS @MENU_LIST TABLE(
	[ID] [int] NOT NULL,
	[DESCR] [nvarchar](100) NOT NULL,
	[URL] [nvarchar](100) NULL,
	[PARENT_MENU_ID] [int] NULL,
	[LABEL_KEY] [varchar](100) NOT NULL,
	[ICON_CODE] [varchar](50) NULL
)
AS
BEGIN

DECLARE @LEFES TABLE ( MENU_ID INT )

--Recover valid menu leafes for user
INSERT INTO @LEFES ( MENU_ID )
SELECT M.ID
FROM MENU AS M
		INNER JOIN IMS_ROLE_MENU AS RM ON RM.IMS_ROLE_MENU_MENU_ID = M.ID
		INNER JOIN IMS_USER_RIGHTS AS UR ON UR.IMS_USER_RIGHTS_USER_ROLE_ID = RM.IMS_ROLE_MENU_USER_ROLE_ID
	WHERE UR.IMS_USER_RIGHTS_USER_UID = @USER_UID;

--Recover tree definition
WITH MENU_TREE(MENU_ID, PARENT_ID, LVL, ROOT_ID)
AS (
	SELECT ID, PARENT_MENU_ID, 1, ID
	FROM MENU AS MENU
	UNION ALL
	SELECT PARENT.ID, PARENT.PARENT_MENU_ID, TREE.LVL + 1, ROOT_ID
	FROM MENU_TREE AS TREE
		INNER JOIN MENU AS PARENT ON PARENT.ID = TREE.PARENT_ID
)

--Insert valid leafes and their parents
INSERT INTO @MENU_LIST(
		ID,
		DESCR,
		URL,
		PARENT_MENU_ID,
		LABEL_KEY,
		ICON_CODE
	)
SELECT DISTINCT M.ID,
		M.DESCR,
		M.URL,
		M.PARENT_MENU_ID,
		M.LABEL_KEY,
		M.ICON_CODE
FROM MENU_TREE AS MT
	INNER JOIN MENU AS M ON M.ID = MT.MENU_ID
WHERE EXISTS (	SELECT 1
				FROM @LEFES AS LEAF
				WHERE MT.ROOT_ID = LEAF.MENU_ID);

	RETURN;
END;