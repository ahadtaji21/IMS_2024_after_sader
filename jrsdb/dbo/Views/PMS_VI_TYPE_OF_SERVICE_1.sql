
CREATE VIEW [dbo].[PMS_VI_TYPE_OF_SERVICE]
AS 
	/***********************************************************
	* Description: List of Type of serivices.
	* Date:   03/26/2020
	* Author: n.migliore
	*
	* Changes
	* Date		Modified By			Comments
	************************************************************
	*	2020-03-30		n.migliore		Added "coi_relations_placeholder"
	*									to set reations in "PMS_COI_TOS_REL".
	************************************************************/
	
	SELECT tos.PMS_TOS_ID,
		tos.PMS_TOS_CODE,
		tos.PMS_TOS_DESCRIPTION,
		tos.PMS_TOS_ENABLED,
		(	SELECT rel.PMS_COI_ID, rel.PMS_TOS_ID
			FROM dbo.PMS_COI_TOS_REL AS rel
			WHERE tos.PMS_TOS_ID = rel.PMS_TOS_ID
			FOR json PATH ) AS coi_relations_placeholder
	FROM dbo.PMS_TYPE_OF_SERVICE tos
	WHERE tos.PMS_TOS_DELETED = 0