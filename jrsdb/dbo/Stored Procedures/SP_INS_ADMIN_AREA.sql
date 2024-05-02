

-- =============================================
-- Author:      Nicola Migliore
-- Create date: 2020-01-23
-- Description: Inserts a new "Admin Area" in the hierarchy.
-- =============================================
-- Parameters:
--   @parent_admin_id - id of parent admin area
--   @admin_type_id - id of type of admin area
--   @admin_name - name of admin area
--   @admin_native_name - native name of admin area
--   @admin_membership - term of memberchip of admin area (e.g. citizenship)
--
-- Returns:    No return value
-- =============================================
-- Change History:
--	...
-- =============================================
CREATE procedure [dbo].[SP_INS_ADMIN_AREA](
	@parent_admin_id int
	,@admin_type_id int
	,@admin_name varchar(25)
	,@admin_native_name varchar(25) = null
	,@admin_membership varchar(50) = null
	,@admin_native_membership varchar(50) = null
)
as
begin
	
	declare @parentNode hierarchyid
		,@siblingNode hierarchyid;

	--Get parent node
	select @parentNode = IMS_ADMIN_AREA_NODE
	from IMS_ADMIN_AREA as a
	where a.IMS_ADMIN_AREA_ID = @parent_admin_id;

	--Get sibling node
	select @siblingNode = max(a.IMS_ADMIN_AREA_NODE)
	from IMS_ADMIN_AREA as a
	where a.IMS_ADMIN_AREA_NODE.GetAncestor(1) = @parentNode;

	begin transaction
	begin try
		--Chekc if already exists
		if exists(	select 1
					from IMS_ADMIN_AREA as old
					where old.IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID = @admin_type_id
						and old.IMS_ADMIN_AREA_NAME = @admin_name)
		begin
			raiserror('Aborting. Admin Area already registered', 16, 1);
		end

		--Insert new Admin Area
		insert into IMS_ADMIN_AREA(
			IMS_ADMIN_AREA_NODE
			,IMS_ADMIN_AREA_NAME
			,IMS_ADMIN_AREA_NATIVE_NAME
			,IMS_ADMIN_AREA_ADMIN_AREA_TYPE_ID
			,IMS_ADMIN_AREA_MEMBERSHIP_TERM
			,IMS_ADMIN_AREA_NATIVE_MEMBERSHIP_TERM
		)
		values (
			@parentNode.GetDescendant(@siblingNode, null)
			,@admin_name
			,@admin_native_name
			,@admin_type_id
			,@admin_membership
			,@admin_native_membership
		);

		commit;
		print 'Successfully inserted new Admin Area: ' + @admin_name;
	end try
	begin catch
		IF @@TRANCOUNT>1 
		BEGIN
			THROW 
		END
		ELSE
		BEGIN
			rollback
		END
		print error_message();
	end catch
end;