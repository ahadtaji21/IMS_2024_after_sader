
CREATE VIEW dbo.PMS_VI_CATEGORY_OF_INTERVENTION
AS
	/***********************************************************
	* Description: List of categories of intevention
	* Date:   03/30/2020
	* Author: n.migliore
	*
	* Changes
	* Date		Modified By			Comments
	************************************************************
	*
	************************************************************/
	SELECT coi.PMS_COI_ID,
		coi.PMS_COI_CODE,
		coi.PMS_COI_ENABLED,
		coi.PMS_COI_DESCRIPTION_LABEL_ID,
		labels.IMS_LABELS_VALUE,
		NULL AS coi_relations_placeholder,
		il.IMS_LANGUAGE_LOCALE
	FROM dbo.PMS_CATEGORY_OF_INTERVENTION coi
		INNER JOIN dbo.IMS_LABELS labels ON labels.IMS_LABELS_ID = coi.PMS_COI_DESCRIPTION_LABEL_ID
		INNER JOIN dbo.IMS_LANGUAGE il ON labels.IMS_LABELS_LANGUAGE_ID = il.IMS_LANGUAGE_ID
	WHERE coi.PMS_COI_DELETED = 0