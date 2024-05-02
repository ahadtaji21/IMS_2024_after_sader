
CREATE VIEW dbo.IMS_VI_ADMIN_AREA_BY_TYPE
AS
    /***********************************************************
    * Description: List of "Admin area types" and the relative "Admin areas".
    * Date:   2020-04-01
    * Author: n.migliore
    *
    * Changes
    * Date		Modified By			Comments
    ************************************************************
    *
    ************************************************************/
    
    SELECT AAT.IMS_ADMIN_AREA_TYPE_ID,
        AAT.IMS_ADMIN_AREA_TYPE_CODE,
        AAT.IMS_ADMIN_AREA_TYPE_DESCR,
        AA.IMS_ADMIN_AREA_ID,
        AA.IMS_ADMIN_AREA_NAME
    FROM dbo.IMS_ADMIN_AREA AS AA
        INNER JOIN IMS_ADMIN_AREA_TYPE AS AAT ON AAT.IMS_ADMIN_AREA_TYPE_ID = AA.IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID