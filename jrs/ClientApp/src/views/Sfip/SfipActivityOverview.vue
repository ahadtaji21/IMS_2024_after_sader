<template>
	<v-content>
		<v-container fluid fill-height>
			<!-- FILTER -->
			<v-row>
				<v-col :cols="12">
					<sfip-plan-selection-form
						v-model="selectedPlanId"
					></sfip-plan-selection-form>
				</v-col>
			</v-row>
			<!-- TABLE -->
			<v-row>
				<v-col :cols="12">
					<jrs-simple-table
						viewName="SFIP_VI_ACTIVITY_OVERVIEW"
						v-model="selectedPlan"
						:dataSourceCondition="tableCondition"
						hideActions
					></jrs-simple-table>
				</v-col>
			</v-row>
		</v-container>
	</v-content>
</template>

<script lang="ts">
	import Vue from "vue";
	import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
	import SfipPlanSelectionForm from "../../components/SFIP/SfipPlanSelectionForm.vue";

	export default Vue.extend({
		components: {
			"jrs-simple-table": JrsSimpleTable,
			"sfip-plan-selection-form": SfipPlanSelectionForm,
		},
		data: () => ({
			selectedPlanId: 0 as number,
		}),
		computed: {
			tableCondition() {
				return `PLAN_ID = ${this.selectedPlanId}`;
			},
		},
	});
</script>

<style scoped>
</style>