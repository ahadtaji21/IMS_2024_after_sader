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
			<!-- MAIN CONTENT -->
			<v-row>
				<v-col :cols="12">
					<v-card>
						<v-card-title>
							<v-toolbar dense color="primary" elevation="2" dark>
								<v-btn-toggle
									v-model="viewToggle"
									mandatory
									dense
									borderless
								>
									<v-btn color="secondary" value="tree">
										<v-icon left
											>mdi-file-tree-outline</v-icon
										>
										<span class="hidden-sm-and-down">{{
											$t(
												"views.sfip-plan-overview.toggle-view-tree"
											)
										}}</span>
									</v-btn>
									<v-btn color="secondary" value="table">
										<v-icon left>mdi-table</v-icon>
										<span class="hidden-sm-and-down">{{
											$t(
												"views.sfip-plan-overview.toggle-view-table"
											)
										}}</span>
									</v-btn>
								</v-btn-toggle>
								<v-btn
									color="secondary lighten-2"
									class="grey--text text--darken-3 mx-1"
									@click="downloadPlanDetail(selectedPlanId)"
									:disabled="plans.length == 0"
								>
									<v-icon>mdi-file-download-outline</v-icon>
									<span>{{
										$t(
											"views.sfip-plan-overview.btn-download-overview"
										)
									}}</span>
								</v-btn>
								<v-btn
									color="secondary lighten-2"
									class="grey--text text--darken-3 mx-1"
									@click="toggleTreeExpansion"
									:disabled="
										plans.length == 0 ||
										viewToggle == 'table'
									"
								>
									<v-icon v-if="treeIsExpanded"
										>mdi-arrow-collapse-vertical</v-icon
									>
									<v-icon v-else
										>mdi-arrow-expand-vertical</v-icon
									>
									<span>{{
										$t(
											"views.sfip-plan-overview.btn-collapse"
										)
									}}</span>
								</v-btn>
							</v-toolbar>
						</v-card-title>

						<!-- TREE -->
						<v-card-text v-if="viewToggle == 'tree'">
							<v-row v-if="plans.length > 0">
								<v-col :cols="4" class="tree-column">
									<h4 class="h4">
										{{
											$t(
												"views.sfip-plan-overview.plan-tree"
											)
										}}
									</h4>
									<v-treeview
										:items="plans"
										item-key="itemKey"
										item-text="title"
										dense
										hoverable
										:return-object="true"
										activatable
										:active.sync="treeSelection"
										:multiple-active="false"
										selection-type="independent"
										ref="planTree"
										style="cursor: pointer"
										v-if="plans.length > 0"
									>
										<template v-slot:prepend="{ item }">
											<v-icon>{{
												calcIcon(item.categoryCode)
											}}</v-icon>
										</template>
									</v-treeview>
								</v-col>
								<!-- <v-divider vertical></v-divider> -->
								<v-col :cols="8" class="detail-column">
									<div
										id="item-deti"
										v-if="currentDetail.viewName"
									>
										<h4 class="h4">
											{{
												$t(
													"views.sfip-plan-overview.item-detail"
												)
											}}
										</h4>
										<jrs-simple-readonly-detail
											:viewName="currentDetail.viewName"
											:whereCondition="
												currentDetail.condition
											"
											:nbrOfColumns="1"
											:key="`${treeSelection[0].categoryCode}_${treeSelection[0].itemKey}`"
											v-if="currentDetail.viewName"
										></jrs-simple-readonly-detail>
									</div>

									<div
										id="activities"
										v-if="
											treeSelection.length > 0 &&
											treeSelection[0].categoryCode ==
												'INDICATOR'
										"
									>
										<h4 class="h4">
											{{
												$t(
													"views.sfip-plan-overview.activities"
												)
											}}
										</h4>
										<!-- <jrs-field
											:field="activityFilterFields[0]"
											v-model="selectedActivityOfficeId"
										></jrs-field> -->
										<v-divider></v-divider>
										<jrs-simple-table
											viewName="SFIP_VI_ACTIVITIE"
											:dataSourceCondition="`INDICATOR_ID = ${treeSelection[0].itemData.id}`"
											:hiddenColumns="[
												'GOAL_CODE',
												'OBJECTIVE_CODE',
												'INDICATOR_CODE',
												'HR_OFFICE_LEGAL_NAME',
												'FORMULATION',
											]"
											ignoreOfficeFilter
										></jrs-simple-table>
									</div>
								</v-col>
							</v-row>
							<v-row v-else>
								<v-col :cols="12">
									<v-alert
										outlined
										type="info"
										color="info"
										prominent
										border="left"
										class="ml-3 mr-3"
									>
										{{
											$t(
												"views.sfip-plan-overview.no-indicator-defined"
											)
										}}
									</v-alert>
								</v-col>
							</v-row>
						</v-card-text>
						<!-- TABLE -->
						<v-card-text v-if="viewToggle == 'table'">
							<jrs-simple-table
								viewName="SFIP_VI_PLAN_OVERVIEW"
								:dataSourceCondition="tableCondition"
								hideActions
							></jrs-simple-table>
						</v-card-text>
					</v-card>
				</v-col>
			</v-row>
		</v-container>
	</v-content>
</template>

<script lang="ts">
	import Vue from "vue";
	import { mapGetters, mapActions } from "vuex";
	import UtilMix from "../../mixins/UtilMix";
	import FormMixin from "../../mixins/FormMixin";
	import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
	import SfipPlanSelectionForm from "../../components/SFIP/SfipPlanSelectionForm.vue";
	import JrsSimpleReadonlyDetail from "../../components/JrsSimpleReadonlyDetail.vue";
	// import JrsField from "../../components/JrsField.vue";
	import {
		SfipPlan,
		SfipPrioritySet,
		SfipPriority,
		SfipGoal,
		SfipObjective,
		SfipIndicator,
		SfipIndicatorAdheranceItem,
		SfipTreeItem,
		SfipItemCategory,
	} from "../../models/SfipObjects";
	import { GenericSqlSelectPayload } from "../../axiosapi";
	import {
		fieldType,
		JrsFormField,
		JrsFormFieldSelect,
	} from "../../models/JrsForm";

	interface DetailInfo {
		viewName: string;
		condition: string;
	}

	export default Vue.extend({
		components: {
			"jrs-simple-table": JrsSimpleTable,
			"sfip-plan-selection-form": SfipPlanSelectionForm,
			"jrs-simple-readonly-detail": JrsSimpleReadonlyDetail,
			// "jrs-field": JrsField,
		},
		mixins: [UtilMix, FormMixin],
		data: () => ({
			selectedPlanId: 0 as number,
			plans: [],
			treeSelection: [] as SfipTreeItem[],
			treeIsExpanded: false,
			selectedItem: null as SfipIndicator | null,
			// tmp: [],
			viewToggle: "tree",
			activityFilterFields: [] as (JrsFormField | JrsFormFieldSelect)[],
			selectedActivityOfficeId: 0,
			regionalOfficeCodes: [] as string[],
		}),
		computed: {
			...mapGetters({
				getCurrentOffice: "getCurrentOffice",
			}),
			tableCondition(): string {
				return `PLAN_ID = ${this.selectedPlanId}`;
			},
			/**
			 * Current detail information based on treeview selection.
			 */
			currentDetail(): DetailInfo {
				let detInfo: DetailInfo = {} as DetailInfo;
				const treeItem: SfipTreeItem =
					this.treeSelection.length > 0
						? this.treeSelection[0]
						: ({} as SfipTreeItem);

				if (!treeItem) {
					return detInfo;
				}
				switch (treeItem?.categoryCode) {
					case SfipItemCategory.Indicator:
						detInfo = {
							viewName: "SFIP_VI_INDICATOR",
							condition: `ID = ${treeItem.itemData.id}`,
						};
						break;
					case SfipItemCategory.Objective:
						detInfo = {
							viewName: "SFIP_VI_OBJECTIVE",
							condition: `ID = ${treeItem.itemData.id}`,
						};
						break;
					case SfipItemCategory.Goal:
						detInfo = {
							viewName: "SFIP_VI_GOAL",
							condition: `ID = ${treeItem.itemData.id}`,
						};
						break;
					case SfipItemCategory.Priority:
						detInfo = {
							viewName: "SFIP_VI_PRIORITY",
							condition: `ID = ${treeItem.itemData.id}`,
						};
						break;
					case SfipItemCategory.PrioritySet:
						detInfo = {
							viewName: "SFIP_VI_PRIORITY_SET",
							condition: `PRIORITY_SET_ID = ${treeItem.itemData.id}`,
						};
						break;
					case SfipItemCategory.Plan:
						detInfo = {
							viewName: "SFIP_VI_PLAN_OVERVIEW",
							condition: `PLAN_ID = ${treeItem.itemData.id}`,
						};
						break;

					default:
						break;
				}
				return { ...detInfo };
			},
		},
		watch: {
			async selectedPlanId(newData: any, oldData: any) {
				const localThis: any = this as any;
				if (newData) {
					try {
						// Load tree data
						const planData = await localThis.getPlanData(newData);
						localThis.plans = localThis.transformPlans(planData);
					} catch (error) {
						localThis.showNewSnackbar({
							typeName: "error",
							text: error,
						});
					}
				} else {
					localThis.plans = [];
				}
			},
			treeSelection(
				newSelection: SfipTreeItem[],
				oldSelection: SfipTreeItem[]
			) {
				const newItem = newSelection?.[0];
				const oldItem = oldSelection?.[0];
				const selectionChanged =
					JSON.stringify(newItem) != JSON.stringify(oldItem);
				const isIndicator = newItem.categoryCode === "INDICATOR";
				if (newItem && selectionChanged && isIndicator) {
					this.setupActivityFilters(newItem);
				}
			},
		},
		methods: {
			...mapActions({
				showNewSnackbar: "showNewSnackbar",
			}),
			...mapActions("apiHandler", {
				getGenericSelect: "getGenericSelect",
			}),
			async getRegionalOfficeCodes() {
				// Get office codes
				const selectPayload: GenericSqlSelectPayload = {
					viewName: "HR_VI_OFFICE",
					whereCond: "IS_INTERNATIONAL=1 OR IS_REGIONAL=1",
					ignoreOfficeFilter: true,
				};
				try {
					const queryResult = await this.getGenericSelect({
						genericSqlPayload: selectPayload,
					});
					const regionCodes: string[] = queryResult?.table_data.map(
						(officeData: any) => officeData.HR_OFFICE_CODE
					);
					// const regionCodes: String[] = [
					// 	"MENA",
					// 	"SAS",
					// 	"EAR",
					// 	"SAR",
					// 	"WAF",
					// 	"APR",
					// 	"EUR",
					// 	"LAC",
					// 	"NAR",
					// ];
					return regionCodes;
				} catch (error) {
					this.showNewSnackbar({ typeName: "error", text: error });
					throw error;
				}
			},
			/**
			 * Download plan information in table form.
			 */
			async downloadPlanDetail(planId: number) {
				const localThis: any = this as any;
				const tableCond: string = `PLAN_ID = ${planId}`;
				const dateString: string = new Date().toLocaleDateString("en-US");
				const fileName: string = `SFIP_DETAILS_${dateString}.xlsx`;
				try {
					localThis.downloadExcelForView(
						"SFIP_VI_PLAN_OVERVIEW",
						tableCond,
						fileName,
						this.getCurrentOffice.HR_OFFICE_ID
					);
				} catch (error) {
					this.showNewSnackbar({ typeName: "error", text: error });
				}
			},
			async getPlanData(): Promise<any[]> {
				let selectPayload: GenericSqlSelectPayload = {
					viewName: "SFIP_VI_PLAN_OVERVIEW",
					whereCond: `PLAN_ID = ${this.selectedPlanId}`,
					officeId: this.getCurrentOffice.HR_OFFICE_ID,
				};
				try {
					const queryResult = await this.getGenericSelect({
						genericSqlPayload: selectPayload,
					});
					return queryResult.table_data || [];
				} catch (error) {
					//this.showNewSnackbar({ typeName: "error", text: error });
					throw error;
				}
			},
			/**
			 * Return list of Adherance items for the given row.
			 * @param { any } row The row containint information of the indicataor and it's adherance.
			 * @returns { SfipIndicatorAdheranceItem[] } List of adherance items for the indicator in the row
			 */
			getIndicatorAdherance(row: any) {
				return this.regionalOfficeCodes.map((regCode) => {
					const parsedCode: string = regCode as string;
					const adherance: SfipIndicatorAdheranceItem = {
						indicatorId: row.INDICATOR_ID,
						officeCode: parsedCode,
						hasAdhered: row[parsedCode],
					};
					return adherance;
				});
			},
			getTreeItems(planData: Array<any>) {
				const indicatorTreeItems: SfipTreeItem[] = [];
				const objectiveTreeItems: SfipTreeItem[] = [];
				const goalTreeItems: SfipTreeItem[] = [];
				const priorityTreeItems: SfipTreeItem[] = [];
				const prioritySetTreeItems: SfipTreeItem[] = [];
				const planTreeItems: SfipTreeItem[] = [];

				planData.forEach((row: any) => {
					// Indicator
					if (
						!indicatorTreeItems.some(
							(treeItem: SfipTreeItem) =>
								(treeItem.itemData as SfipIndicator).id ==
								row.INDICATOR_ID
						)
					) {
						const tmpInd: SfipIndicator = {
							id: row.INDICATOR_ID,
							code: row.INDICATOR_CODE,
							formulationHtml: row.INDICATOR_FORMULATION,
							objectiveId: row.OBJECTIVE_ID,
							adherance: this.getIndicatorAdherance(row),
						};
						const indicatorTreeItem: SfipTreeItem = {
							itemKey: `IND_${row.INDICATOR_ID}`,
							title: `Indicator: ${row.INDICATOR_CODE}`,
							category: "Indicator",
							categoryCode: SfipItemCategory.Indicator,
							itemData: tmpInd,
							children: [],
						};
						indicatorTreeItems.push(indicatorTreeItem);
					}

					// Objective
					if (
						!objectiveTreeItems.some(
							(treeItem: SfipTreeItem) =>
								(treeItem.itemData as SfipObjective).id ==
								row.OBJECTIVE_ID
						)
					) {
						const tmpObj: SfipObjective = {
							id: row.OBJECTIVE_ID,
							code: row.OBJECTIVE_CODE,
							formulation: row.OBJECTIVE_FORMULATION,
							goalId: row.GOAL_ID,
						};
						const objectTreeItem:SfipTreeItem = {
							itemKey: `OBJ_${row.OBJECTIVE_ID}`,
							title: `Objective: ${row.OBJECTIVE_CODE}`,
							category: "Objective",
							categoryCode: SfipItemCategory.Objective,
							itemData: tmpObj,
							children: [],
						};
						objectiveTreeItems.push(objectTreeItem);
					}

					// Goal
					if (
						!goalTreeItems.some(
							(treeItem: SfipTreeItem) =>
								(treeItem.itemData as SfipGoal).id == row.GOAL_ID
						)
					) {
						const tmpGoal: SfipGoal = {
							id: row.GOAL_ID,
							code: row.GOAL_CODE,
							formulation: row.GOAL_FORMULATION,
							priorityId: row.PRIORITY_ID,
						};
						const goalTreeItem: SfipTreeItem = {
							itemKey: `GOAL_${row.GOAL_ID}`,
							title: `Goal: ${row.GOAL_CODE}`,
							category: "Goal",
							categoryCode: SfipItemCategory.Goal,
							itemData: tmpGoal,
							children: [],
						};
						goalTreeItems.push(goalTreeItem);
					}

					// Priority
					if (
						!priorityTreeItems.some(
							(treeItem: SfipTreeItem) =>
								(treeItem.itemData as SfipPriority).id ==
								row.PRIORITY_ID
						)
					) {
						const tmpPriority = {
							id: row.PRIORITY_ID,
							code: row.PRIORITY_CODE,
							name: row.PRIORITY_NAME,
							formulation: row.PRIORITY_FORMULATION,
							prioritySetId: row.PRIORITY_SET_ID,
						};
						const priorityTreeItem: SfipTreeItem = {
							itemKey: `PRIORITY_${row.PRIORITY_ID}`,
							title: `Priority: ${row.PRIORITY_CODE}`,
							category: "Priority",
							categoryCode: SfipItemCategory.Priority,
							itemData: tmpPriority,
							children: [],
						};
						priorityTreeItems.push(priorityTreeItem);
					}

					// Priority Set
					if (
						!prioritySetTreeItems.some(
							(treeItem: SfipTreeItem) =>
								(treeItem.itemData as SfipPrioritySet).id ==
								row.PRIORITY_SET_ID
						)
					) {
						const tmpPrioritySet: SfipPrioritySet = {
							id: row.PRIORITY_SET_ID,
							name: row.PRIORITY_SET_NAME,
							containingPlans: [],
						};
						// Add parents
						tmpPrioritySet.containingPlans = [
							...tmpPrioritySet.containingPlans,
							row.PLAN_ID,
						];
						const prioritySetTreeItem: SfipTreeItem = {
							itemKey: `PRISET_${row.PRIORITY_SET_ID}`,
							title: `Priority Set: ${row.PRIORITY_SET_NAME}`,
							category: "Priority Set",
							categoryCode: SfipItemCategory.PrioritySet,
							itemData: tmpPrioritySet,
							children: [],
						};
						prioritySetTreeItems.push(prioritySetTreeItem);
					}

					// Plan
					if (
						!planTreeItems.some(
							(treeItem: SfipTreeItem) =>
								(treeItem.itemData as SfipPlan).id == row.PLAN_ID
						)
					) {
						const tmpPlan: SfipPlan = {
							id: row.PLAN_ID,
							code: row.PLAN_CODE,
							startYear: row.PLAN_START_YEAR,
							endYear: row.PLAN_END_YEAR,
						};
						const planTreeItem: SfipTreeItem = {
							itemKey: `PLAN_${row.PLAN_ID}`,
							title: `Plan: ${row.PLAN_CODE}`,
							category: "Plan",
							categoryCode: SfipItemCategory.Plan,
							itemData: tmpPlan,
							children: [],
						};
						planTreeItems.push(planTreeItem);
					}
				});

				// Sort lists
				indicatorTreeItems.sort((a: SfipTreeItem, b: SfipTreeItem) =>
					a.title > b.title ? 1 : -1
				);
				objectiveTreeItems.sort((a: SfipTreeItem, b: SfipTreeItem) =>
					a.title > b.title ? 1 : -1
				);
				goalTreeItems.sort((a: SfipTreeItem, b: SfipTreeItem) =>
					a.title > b.title ? 1 : -1
				);
				priorityTreeItems.sort((a: SfipTreeItem, b: SfipTreeItem) =>
					a.title > b.title ? 1 : -1
				);
				prioritySetTreeItems.sort((a: SfipTreeItem, b: SfipTreeItem) =>
					a.title > b.title ? 1 : -1
				);
				planTreeItems.sort((a: SfipTreeItem, b: SfipTreeItem) =>
					a.title > b.title ? 1 : -1
				);
				return {
					indicatorTreeItems,
					objectiveTreeItems,
					goalTreeItems,
					priorityTreeItems,
					prioritySetTreeItems,
					planTreeItems,
				};
			},
			/**
			 * Recursively get childrent TreeItems.
			 * @param { SfipTreeItem } indicatorTreeItems List of tree items for indicators.
			 * @param { SfipTreeItem } objectiveTreeItems List of tree items for objectives.
			 * @param { SfipTreeItem } goalTreeItems List of tree items for goals.
			 * @param { SfipTreeItem } priorityTreeItems List of tree items for priorities.
			 * @param { SfipTreeItem } prioritySetTreeItems List of tree items for prioritySets.
			 * @param { SfipTreeItem } planTreeItems List of tree items for plans.
			 * @param { string } objectCode code of the object category to get children for.
			 * @param { number } parentId ID of the parent to get children for.
			 * @return { SfipTreeItem[] }
			 */
			getTreeChildren(
				indicatorTreeItems: SfipTreeItem[],
				objectiveTreeItems: SfipTreeItem[],
				goalTreeItems: SfipTreeItem[],
				priorityTreeItems: SfipTreeItem[],
				prioritySetTreeItems: SfipTreeItem[],
				planTreeItems: SfipTreeItem[],
				objectCode: string,
				parentId: number | null
			): SfipTreeItem[] {
				const getChildren = (
					childCode: string,
					itemId: number
				): SfipTreeItem[] => {
					return this.getTreeChildren(
						indicatorTreeItems,
						objectiveTreeItems,
						goalTreeItems,
						priorityTreeItems,
						prioritySetTreeItems,
						planTreeItems,
						childCode,
						itemId
					);
				};

				if (objectCode == "PLAN") {
					return planTreeItems.map((treeItem: SfipTreeItem) => {
						const children: SfipTreeItem[] = [
							...getChildren(
								"PRIORITY_SET",
								(treeItem.itemData as SfipPlan).id
							),
						];
						treeItem.children = children;
						return treeItem;
					});
				}
				// Return Priority Set
				if (objectCode == "PRIORITY_SET" && parentId) {
					return prioritySetTreeItems
						.filter((treeItem: SfipTreeItem) =>
							(
								treeItem.itemData as SfipPrioritySet
							).containingPlans.includes(parentId)
						)
						.map((treeItem: SfipTreeItem) => {
							treeItem.children = [
								...getChildren(
									"PRIORITY",
									(treeItem.itemData as SfipPrioritySet).id
								),
							];
							return treeItem;
						});
				}
				// Return Priority
				if (objectCode == "PRIORITY") {
					return priorityTreeItems
						.filter(
							(treeItem: SfipTreeItem) =>
								(treeItem.itemData as SfipPriority).prioritySetId ==
								parentId
						)
						.map((treeItem: SfipTreeItem) => {
							treeItem.children = [
								...getChildren(
									"GOAL",
									(treeItem.itemData as SfipPriority).id
								),
							];
							return treeItem;
						});
				}
				// Return Goals
				if (objectCode == "GOAL") {
					return goalTreeItems
						.filter(
							(treeItem: SfipTreeItem) =>
								(treeItem.itemData as SfipGoal).priorityId ==
								parentId
						)
						.map((treeItem: SfipTreeItem) => {
							treeItem.children = [
								...getChildren(
									"OBJECTIVE",
									(treeItem.itemData as SfipGoal).id
								),
							];
							return treeItem;
						});
				}
				// Return Objectives
				if (objectCode == "OBJECTIVE") {
					return objectiveTreeItems
						.filter(
							(treeItem: SfipTreeItem) =>
								(treeItem.itemData as SfipObjective).goalId ==
								parentId
						)
						.map((treeItem: SfipTreeItem) => {
							treeItem.children = [
								...getChildren(
									"INDICATOR",
									(treeItem.itemData as SfipObjective).id
								),
							];
							return treeItem;
						});
				}
				// Return Indicators
				if (objectCode == "INDICATOR") {
					return indicatorTreeItems.filter(
						(treeItem: SfipTreeItem) =>
							(treeItem.itemData as SfipIndicator).objectiveId ==
							parentId
					);
				}
				return [] as SfipTreeItem[];
			},
			transformPlans(planData: Array<any>) {
				const {
					indicatorTreeItems,
					objectiveTreeItems,
					goalTreeItems,
					priorityTreeItems,
					prioritySetTreeItems,
					planTreeItems,
				} = this.getTreeItems(planData);

				return this.getTreeChildren(
					indicatorTreeItems,
					objectiveTreeItems,
					goalTreeItems,
					priorityTreeItems,
					prioritySetTreeItems,
					planTreeItems,
					"PLAN",
					null
				);
			},
			/**
			 * Set or clear the selected indicator.
			 * @param ind indicator to select.
			 */
			selectIndicator(ind: SfipIndicator | null) {
				this.selectedItem = ind;
			},
			calcIcon(catCode: SfipItemCategory) {
				let iconCode: string = "";
				switch (catCode) {
					case SfipItemCategory.Plan:
						iconCode = "mdi-clipboard-edit-outline";
						break;
					case SfipItemCategory.PrioritySet:
						iconCode = "mdi-hexagon-multiple";
						break;
					case SfipItemCategory.Priority:
						iconCode = "mdi-hexagon";
						break;
					case SfipItemCategory.Goal:
						iconCode = "mdi-target";
						break;
					case SfipItemCategory.Objective:
						iconCode = "mdi-checkbox-marked-circle";
						break;
					case SfipItemCategory.Indicator:
						iconCode = "mdi-card-text-outline";
						break;

					default:
						break;
				}
				return iconCode;
			},
			toggleTreeExpansion() {
				const tree: any = this.$refs.planTree;
				tree.updateAll(!this.treeIsExpanded);
				this.treeIsExpanded = !this.treeIsExpanded;
			},
			setupActivityFilters(indicatorTreeItem: SfipTreeItem) {
				// const adheringOfficeCodes: string = this.treeSelection[0].adherance
				const itemData: SfipIndicator =
					indicatorTreeItem?.itemData as SfipIndicator;
				const adheringOfficeCodes: string = itemData.adherance
					.filter(
						(adherance: SfipIndicatorAdheranceItem) =>
							adherance.hasAdhered
					)
					.map(
						(adherance: SfipIndicatorAdheranceItem) =>
							`'${adherance.officeCode}'`
					)
					.join(",");
				const officeCondition: string = adheringOfficeCodes
					? `HR_OFFICE_CODE IN (${adheringOfficeCodes})`
					: "";

				const fieldsInfo: any[] = [
					{
						column_name: "ACTIVITY_OFFICE_FILTER",
						type: fieldType.select,
						required: false,
						select_items_datasource: "HR_VI_OFFICE",
						itemKey: "HR_OFFICE_ID",
						itemText: "HR_OFFICE_LEGAL_NAME",
						dataSourceCondition: `(IS_REGIONAL=1 OR IS_INTERNATIONAL=1) AND ${officeCondition}`,
						form_order: 1,
						labelTranslationKey: "views.sfip-plan-overview.office",
						readonly: false,
					},
					{
						column_name: "SHOW_CHILDREN",
						type: fieldType.checkbox,
						required: false,
						form_order: 2,
						labelTranslationKey:
							"views.sfip-plan-overview.include-sub-offices",
						readonly: false,
					},
				];

				const tmpFields = fieldsInfo.map((info: any) => {
					let tmpField: JrsFormField | JrsFormFieldSelect =
						FormMixin.methods.parseJrsFormField(info);

					tmpField.label = this.$t(
						tmpField.labelTranslationKey || ""
					).toString();
					return tmpField;
				});

				this.activityFilterFields = FormMixin.methods.setupSelectFields(
					tmpFields,
					this.getCurrentOffice.HR_OFFICE_ID
				);
			},
		},
		async mounted() {
			// this.setupActivityFilters();
			try {
				this.regionalOfficeCodes = await this.getRegionalOfficeCodes();
			} catch (error) {
				console.error(error);
			}
		},
	});
</script>

<style scoped>
	.tree-column {
		border-right: solid 1px rgba(0, 0, 0, 0.12);
	}
	.detail-column {
		max-height: 50em;
		overflow: auto;
	}
</style>