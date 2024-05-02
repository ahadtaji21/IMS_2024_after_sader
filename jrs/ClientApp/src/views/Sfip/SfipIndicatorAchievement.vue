<template>
	<v-content>
		<v-container fluid fill-height>
			<v-row>
				<v-col :cols="12">
					<sfip-plan-selection-form
						v-model="selectedPlanId"
					></sfip-plan-selection-form>
				</v-col>
				<v-col :cols="12">
					<jrs-simple-table
						viewName="SFIP_VI_INDICATOR_ACHIEVEMENT"
						:dataSourceCondition="tableCondition"
					></jrs-simple-table>
				</v-col>
			</v-row>
		</v-container>
	</v-content>
</template>

<script lang="ts">
	import Vue from "vue";
	import SfipPlanSelectionForm from "../../components/SFIP/SfipPlanSelectionForm.vue";
	import JrsSimpleTable from "../../components/JrsSimpleTable.vue";

	export default Vue.extend({
		components: {
			"sfip-plan-selection-form": SfipPlanSelectionForm,
			"jrs-simple-table": JrsSimpleTable,
		},
		data: () => ({
			selectedPlanId: 0 as number,
		}),
		computed: {
			tableCondition() {
				return `SFIP_ID = ${this.selectedPlanId}`;
			},
		},
	});
</script>

<style scoped>
</style>