<template>
	<v-card class="elevation-0" outlined dense>
		<v-card-title>{{ $t("general.filters") }}</v-card-title>
		<v-divider></v-divider>
		<v-card-text class="py-0">
			<jrs-form
				:formFields="filterFields"
				:formData="filterData"
			></jrs-form>
		</v-card-text>
	</v-card>
</template>

<script lang="ts">
	import Vue from "vue";
	import { mapActions } from "vuex";
	import { GenericSqlSelectPayload } from "../../axiosapi/api";
	import {
		fieldType,
		JrsFormField,
		JrsFormFieldSelect,
	} from "../../models/JrsForm";
	import JrsForm from "../../components/JrsForm.vue";

	export default Vue.extend({
		components: {
			"jrs-form": JrsForm,
		},
		props:{
			value: {
				type: Number,
				required:true,
				default: null,
			}
		},
		data: () => ({
			filterFields: [] as Array<JrsFormField | JrsFormFieldSelect>,
			filterData: {} as any,
		}),
		computed:{
			/**
			 * Selected SFIP Plan.
			 */
			selectedPlanId() {
				const filterData:any = { ...this.filterData };
				return filterData.sfipPlanId;
			},
		},
		watch:{
			selectedPlanId(to:number, from:number){
				if(to != from){
					this.updateValue(to || 0);
				}
			}
		},
		methods:{
			...mapActions("apiHandler", {
				getGenericSelect: "getGenericSelect",
			}),
			/**
			 * Emit updated value.
			 */
           updateValue(newVal: any) {
                this.$emit("input", newVal);
            },
			/**
			 * Setup the filter form.
			 */
			async setupFilterForm() {
				const currYear: number = new Date().getFullYear();

				// Get initial filter SFIP Plans
				const selectPayload: GenericSqlSelectPayload = {
					viewName: "SFIP_STRATEGIC_FRAMEWORK_IMPLEMENTATION_PLAN",
					colList: "ID,CODE",
					whereCond: `${currYear} >= START_YEAR AND ${currYear} <= END_YEAR`,
				};
				const sfipPlans: Array<any> = (
					await this.getGenericSelect({
						genericSqlPayload: selectPayload,
					})
				).table_data;

				// Setup filter fields
				this.filterFields = [
					{
						type: fieldType.date,
						name: "year",
						label: this.$t(
							"views.sfip-activity-definition.filter-year"
						).toString(),
						additional_config: { OnlyYear: true },
						form_order: 10,
					},
					{
						type: fieldType.select,
						name: "sfipPlanId",
						label: this.$t(
							"views.sfip-activity-definition.filter-sfip-plan"
						).toString(),
						dataSource: "SFIP_STRATEGIC_FRAMEWORK_IMPLEMENTATION_PLAN",
						selectItems: sfipPlans,
						itemKey: "ID",
						itemText: "CODE",
						additional_config: {
							dependencyRules: [
								{
									dependencyType: "filter",
									masterFieldName: "year",
									targetFieldName: "sfipPlanId",
									targetCondition:
										"##year## >= START_YEAR AND ##year## <= END_YEAR",
								},
							],
						},
						form_order: 20,
					},
				];

				// Setup default values of filter data
				const defSfipPlan: any =
					sfipPlans.length == 1 ? sfipPlans[0].ID : null;
				this.filterData = {
					year: currYear,
					sfipPlanId: defSfipPlan,
				};
			},
		},
		mounted() {
			this.setupFilterForm();
		},
	});
</script>

<style scoped>
</style>