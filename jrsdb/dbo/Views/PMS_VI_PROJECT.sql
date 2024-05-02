
CREATE VIEW dbo.PMS_VI_PROJECT
AS
    /***********************************************************
    * Description: List of mandatory data defining a Project.
    * Date:   2020-04-01
    * Author: n.migliore
    *
    * Changes
    * Date		Modified By			Comments
    ************************************************************
    *
    ************************************************************/
        
    SELECT
        ID,
        CODE,
        DESCR,
        LOCATION_DESCR,
        COUNTRY_ADMIN_AREA_ID,
        AREA.IMS_ADMIN_AREA_NAME,
        START_YEAR,
        STATUS_ID,
        STAT.IMS_STATUS_NAME
    FROM PMS_PROJECT AS PROJ
        INNER JOIN IMS_ADMIN_AREA AS AREA ON AREA.IMS_ADMIN_AREA_ID = COUNTRY_ADMIN_AREA_ID
        INNER JOIN IMS_STATUS AS STAT ON STAT.IMS_STATUS_ID = PROJ.STATUS_ID