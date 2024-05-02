

CREATE VIEW [dbo].[IMS_VI_MENU]
AS

/***********************************************************
* Description: List of all menu.
* Date:   2020-04-20
* Author: n.migliore
*
* Changes
* Date		Modified By			Comments
************************************************************
*
************************************************************/

     SELECT M.ID, 
            M.DESCR, 
            M.URL, 
            M.PARENT_MENU_ID, 
            PARENT.DESCR AS PARENT_DESCR, 
            M.LABEL_KEY, 
            M.ICON_CODE
     FROM dbo.MENU AS M
          LEFT JOIN dbo.MENU AS PARENT ON PARENT.ID = M.PARENT_MENU_ID;