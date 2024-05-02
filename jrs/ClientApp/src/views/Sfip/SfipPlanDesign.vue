<template>
	<v-content>
		<v-container fluid fill-height>
			<v-row>
				<v-col :cols="12">
					<jrs-simple-table
						viewName="SFIP_VI_IMPLEMENTATION_PLAN"
						v-model="selectedPlan"
						:dataSourceCondition="'IS_DELETED = 0'"
						selectable
					>
						<template
							v-slot:simple-table-item-actions="{
								simpleItemRowItem: item,
							}"
						>
							<v-tooltip top>
								<template v-slot:activator="{ on }">
									<v-icon
										small
										v-on="on"
										@click="
											downloadPlanDetail(
												item.IMPLEMENTATION_PLAN_ID
											)
										"
										>mdi-file-download-outline</v-icon
									>
								</template>
								<span>{{
									$t(
										"datasource.sfip.implementation-plan.btn-donwload-plan"
									)
								}}</span>
							</v-tooltip>
							<v-tooltip top>
								<template v-slot:activator="{ on }">
									<v-icon
										small
										v-on="on"
										@click="
											downloadActivitySchedule(
												item.IMPLEMENTATION_PLAN_ID
											)
										"
										>mdi-briefcase-download-outline</v-icon
									>
								</template>
								<span>{{
									$t(
										"datasource.sfip.implementation-plan.btn-donwload-schedule"
									)
								}}</span>
							</v-tooltip>
						</template>
					</jrs-simple-table>
				</v-col>
			</v-row>
			<v-row>
				<v-col :cols="12">
					<v-stepper
						v-model="currentStep"
						alt-labels
						v-if="selectedPlanId"
					>
						<v-stepper-header>
							<template v-for="step in steps">
								<v-stepper-step
									:key="`STEP_${step.no}`"
									:step="step.no"
									:editable="currentStep > step.no"
									@click="setStep(step.no, null, true)"
								>
									<span>
										{{ $t(step.titleKey) }}
										<span v-if="step.selectedItemLabel"
											>:
											{{ step.selectedItemLabel }}</span
										>
									</span>
								</v-stepper-step>
								<v-divider
									:key="`STEP_LINE_${step.no}`"
								></v-divider>
							</template>
						</v-stepper-header>

						<v-stepper-items>
							<v-stepper-content
								v-for="step in stepContents"
								:step="step.no"
								:key="`STEP_CONTENT_${step.no}`"
							>
								<p v-if="step.promptKey">
									{{ $t(step.promptKey) }}
								</p>
								<jrs-simple-table
									:viewName="step.tableViewName"
									:dataSourceCondition="
										step.tableWhereCondition
									"
									:title="step.tableTitle"
									:savePayload="step.savePayload"
									:columnList="step.columnList"
									:hiddenFields="step.hiddenFields"
									:ignoreOfficeFilter="
										step.ignoreOfficeFilter
									"
								>
									<template
										v-slot:simple-table-item-actions="{
											simpleItemRowItem: rowItem,
										}"
										v-if="step.showDrilldown"
									>
										<v-tooltip top>
											<template v-slot:activator="{ on }">
												<v-icon
													small
													v-on="on"
													@click="
														setStep(
															step.no,
															rowItem
														)
													"
													>mdi-chevron-double-down</v-icon
												>
											</template>
											<span>{{
												$t(
													"general.crud.tooltip-drill-down"
												)
											}}</span>
										</v-tooltip>
									</template>
								</jrs-simple-table>
							</v-stepper-content>
						</v-stepper-items>
					</v-stepper>
				</v-col>
			</v-row>
		</v-container>
	</v-content>
</template>

<script lang="ts">
	import Vue from "vue";
	import { mapGetters, mapActions } from "vuex";
	import UtilMix from "../../mixins/UtilMix";
	import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
	import {
		GenericSqlSelectPayload,
		GenericExcelColumnLabel,
	} from "../../axiosapi/api";
	import { JrsHeader } from "../../models/JrsTable";

	interface IStepHead {
		no: number;
		titleKey: string;
		selectedItemLabel: string | null;
	}
	interface IStepContent {
		no: number;
		tableViewName: string | null;
		tableTitle?: string | null;
		tableWhereCondition: string | null;
		showDrilldown: boolean;
		columnList?: Array<string>;
		hiddenFields?: Array<string>;
		savePayload?: any;
		promptKey?: string;
		ignoreOfficeFilter?: boolean;
	}

	export default Vue.extend({
		components: {
			"jrs-simple-table": JrsSimpleTable,
		},
		data: () => ({
			selectedPlan: [],
			currentStep: 1,
			hiddenIndicatorColumns: [
				"ID",
				"CODE",
				"FORMULATION",
				"OBJECTIVE_ID",
				"VALUE_1",
				"VALUE_2",
				"VALUE_3",
				"VALUE_4",
				"VALUE_5",
				"IS_DELETED",
				"OFFICE_RELATIONS",
			],
			steps: [
				{
					no: 1,
					titleKey: "datasource.sfip.priority-set.title",
					selectedItemLabel: null,
				},
				{
					no: 2,
					titleKey: "datasource.sfip.priority.title",
					selectedItemLabel: null,
				},
				{
					no: 3,
					titleKey: "datasource.sfip.goal.title",
					selectedItemLabel: null,
				},
				{
					no: 4,
					titleKey: "datasource.sfip.objective.title",
					selectedItemLabel: null,
				},
				{
					no: 5,
					titleKey: "datasource.sfip.indicator.title",
					selectedItemLabel: null,
				},
			] as Array<IStepHead>,
			currPrioritySet: null as any,
			currPriority: null as any,
			currGoal: null as any,
			currObjective: null as any,
			currIndicator: null as any,
		}),
		computed: {
			...mapGetters({
				getCurrentOffice: "getCurrentOffice",
			}),
			selectedPlanId() {
				const plan: any =
					this.selectedPlan.length > 0 ? this.selectedPlan[0] : {};
				return plan.IMPLEMENTATION_PLAN_ID;
			},
			/**
			 * list of table information.
			 */
			stepContents(): Array<IStepContent> {
				const stepHeads: Array<IStepHead> = this.steps;
				let stepContents: Array<IStepContent> = stepHeads.map(
					(head: IStepHead) => {
						let content: IStepContent = {
							no: head.no,
							tableViewName: null,
							tableWhereCondition: null,
							showDrilldown: true,
						};

						switch (head.no) {
							// Case "Priotiry Set"
							case 1:
								// Set table
								content.tableViewName = "SFIP_VI_PRIORITY_SET";
								content.tableTitle = this.$t("views.sfip-plan-design.priority-set-title").toString();
								// Set condition
								content.tableWhereCondition = null;
								break;

							// Case "Priotiry"
							case 2:
								// Set table props
								content.tableViewName = "SFIP_VI_PRIORITY";
								content.tableTitle = this.$t("views.sfip-plan-design.priority-title").toString();
								if (this.currPrioritySet) {
									content.hiddenFields = ["PRIORITY_SET_ID"];
									content.savePayload = {
										prioritySetId:
											this.currPrioritySet?.PRIORITY_SET_ID,
									};
									content.tableWhereCondition = `PRIORITY_SET_ID = ${this.currPrioritySet.PRIORITY_SET_ID}`;
								}
								break;

							// Case "Goal"
							case 3:
								// Set table
								content.tableViewName = "SFIP_VI_GOAL";
								content.tableTitle = this.$t("views.sfip-plan-design.goal-title").toString();
								// Set condition
								if (this.currPriority) {
									content.hiddenFields = ["PRIORITY_ID"];
									content.savePayload = {
										priorityId: this.currPriority?.ID,
									};
									content.tableWhereCondition = `PRIORITY_ID = ${this.currPriority.ID}`;
								}
								break;

							// Case "Objective"
							case 4:
								// Set table
								content.tableViewName = "SFIP_VI_OBJECTIVE";
								content.tableTitle = this.$t("views.sfip-plan-design.objective-title").toString();
								// Set condition
								if (this.currGoal) {
									content.hiddenFields = ["GOAL_ID"];
									content.savePayload = {
										goalId: this.currGoal?.ID,
									};
									content.tableWhereCondition = `GOAL_ID = ${this.currGoal.ID}`;
								}
								break;

							// Case "Indicator"
							case 5:
								// Set table props
								content.tableViewName = "SFIP_VI_INDICATOR";
								content.showDrilldown = false;
								if (this.currObjective) {
									content.hiddenFields = [
										"OBJECTIVE_ID",
										"SFIP_ID",
									];
									content.savePayload = {
										objectiveId: this.currObjective.ID,
										sfipId: this.selectedPlanId,
									};
									content.tableWhereCondition = `OBJECTIVE_ID = ${this.currObjective.ID} AND SFIP_ID = ${this.selectedPlanId}`;
									content.ignoreOfficeFilter = true;
								}
								// Set prompt
								content.promptKey =
									"views.sfip-plan-design.indicators-prompt";
								break;

							default:
								break;
						}
						return content;
					}
				);

				return stepContents;
			},
		},
		mixins: [UtilMix],
		methods: {
			...mapActions({
				showNewSnackbar: "showNewSnackbar",
			}),
			...mapActions("apiHandler", {
				getJRSTableStructure: "getJRSTableStructure",
				generateExcelForTable: "generateExcelForTable",
			}),
			downloadPlanDetail(planId: number) {
				const tableCond: string = `PLAN_ID = ${planId}`;
				const dateString: string = new Date().toLocaleDateString("en-US");
				const fileName: string = `SFIP_DETAILS_${dateString}.xlsx`;
				this.downloadExcelForView(
					"SFIP_VI_PLAN_OVERVIEW",
					tableCond,
					fileName
				);
			},
			downloadActivitySchedule(planId: number) {
				const tableCond: string = `PLAN_ID = ${planId}`;
				const dateString: string = new Date().toLocaleDateString("en-US");
				const fileName: string = `ACTIVITY_SCHEDULE_${dateString}.xlsx`;
				this.downloadExcelForView(
					"SFIP_VI_ACTIVITY_OVERVIEW",
					tableCond,
					fileName
				);
			},
			/**
			 * Download Excel file for a given JrsTable
			 */
			async downloadExcelForView(
				tableName: string,
				tableCondition: string,
				fileName: string
			) {
				const localThis: any = this as any;

				try {
					// Get column labels
					const tableStructPayload: GenericSqlSelectPayload = {
						viewName: tableName,
					};
					const tableStruct: any = await this.getJRSTableStructure({
						genericSqlPayload: tableStructPayload,
					});
					const cols: Array<JrsHeader> = tableStruct.columns;
					const colList: string = cols
						.filter((header: JrsHeader) => !header.is_hidden)
						.sort((a: JrsHeader, b: JrsHeader) => {
							let order_A: number = a.list_order
								? a.list_order
								: 1000;
							let order_B: number = b.list_order
								? b.list_order
								: 1000;
							return order_A < order_B ? -1 : 1;
						})
						.reduce(
							(retStr: string, col: JrsHeader) =>
								`${retStr},${col.column_name}`,
							""
						)
						.substring(1);
					const colLabels: Array<GenericExcelColumnLabel> = cols
						.filter((col: JrsHeader) => !col.is_hidden)
						.map((col: JrsHeader) => {
							let ret = {
								columnName: col.column_name,
								columnLabel: col.translationtKey
									? localThis.$t(col.translationtKey).toString()
									: null,
							};
							return ret;
						});

					// Get excel document bytes
					const byteArray: Array<any> = await this.generateExcelForTable({
						viewName: tableName,
						whereCond: tableCondition,
						orderStmt: undefined,
						officeId: this.getCurrentOffice.HR_OFFICE_ID,
						colList: colList,
						colLabels: colLabels,
					});

					// Download File
					const dateString: string = new Date().toLocaleDateString(
						"en-US"
					);
					localThis.downloadFileFromBytes(byteArray, fileName);
				} catch (error) {
					this.showNewSnackbar({ typeName: "error", text: error });
				}
			},
			setStep(
				stepNumber: number,
				rowItem: any | null,
				skipStepAdvance: boolean = false
			) {
				// Return if step is not editable
				if (this.currentStep < stepNumber) {
					return;
				}

				// Advance current step
				if (!skipStepAdvance && stepNumber < 5) {
					this.currentStep = stepNumber + 1;
				}

				// Set current selected items
				switch (stepNumber) {
					case 1:
						// this.setStepLabel(stepNumber, rowItem ? rowItem.PRIORITY_SET_CODE : null);
						this.setStepLabel(stepNumber, rowItem?.PRIORITY_SET_CODE);
						this.currPrioritySet = rowItem;
						// Clear following step selections selection
						this.currPriority = null;
						this.currGoal = null;
						this.currObjective = null;
						this.currIndicator = null;
						break;
					case 2:
						this.setStepLabel(stepNumber, rowItem?.CODE);
						this.currPriority = rowItem;
						// Clear following step selections selection
						this.currGoal = null;
						this.currObjective = null;
						this.currIndicator = null;
						break;
					case 3:
						this.setStepLabel(stepNumber, rowItem?.CODE);
						this.currGoal = rowItem;
						// Clear following step selections selection
						this.currObjective = null;
						this.currIndicator = null;
						break;
					case 4:
						// this.setStepLabel(stepNumber, rowItem?.CODE);
						this.setStepLabel(stepNumber, rowItem?.OBJ_CODE);
						this.currObjective = rowItem;
						// Clear following step selections selection
						this.currIndicator = null;
						break;
					case 5:
						this.setStepLabel(stepNumber, rowItem?.CODE);
						this.currIndicator = rowItem;
						// Clear following step selections selection
						break;

					default:
						break;
				}
			},
			setStepLabel(stepNumber: number, label: string | null) {
				const oldSteps: Array<IStepHead> = this.steps;
				this.steps = oldSteps.map((step: IStepHead) => {
					// Update label if necessary
					if (step.no == stepNumber) {
						step.selectedItemLabel = label;
					}
					// Clear following labels
					if (step.no > stepNumber) {
						step.selectedItemLabel = null;
					}
					return step;
				});
			},
		},
	});
</script>

<style scoped>
</style>