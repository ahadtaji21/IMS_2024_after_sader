<template>
	<v-content>
		<v-container fluid fill-height>
			<v-row>
				<v-col :cols="12">
					<sfip-plan-selection-form
						v-model="selectedPlanId"
					></sfip-plan-selection-form>
				</v-col>
			</v-row>
			<v-row v-if="selectedPlanId">
				<v-col :cols="12">
					<v-card>
						<v-card-title>
							<v-toolbar dense color="primary" elevation="2" dark>
								<v-btn
									color="secondary lighten-2"
									class="grey--text text--darken-3 mx-1"
									@click="toggleTreeExpansion"
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
						<v-card-text>
							<v-row>
								<v-col :cols="4" class="tree-column">
									<v-treeview
										v-if="planTreeItems"
										:items="planTreeItems"
										item-key="itemKey"
										item-text="title"
										dense
										hoverable
										:return-object="true"
										:active.sync="treeSelection"
										:multiple-active="false"
										selection-type="independent"
										ref="planTree"
										style="cursor: pointer"
									>
										<template v-slot:prepend="{ item }">
											<v-icon
												v-if="item"
												@click="
													onTreeSelectionChanged([
														item,
													])
												"
												>{{
													calcIcon(item.categoryCode)
												}}</v-icon
											>
										</template>
										<template v-slot:label="{ item }">
											<div
												v-if="item"
												@click="
													onTreeSelectionChanged([
														item,
													])
												"
												class="tree-item-label"
											>
												<span>{{ item.title }}</span>
											</div>
										</template>
										<template v-slot:append="{ item }">
											<!-- ADD ACTIVITY BTN -->
											<v-tooltip
												top
												v-if="
													item &&
													checkIfIndicator(
														item.categoryCode
													)
												"
											>
												<template
													v-slot:activator="{ on }"
												>
													<v-btn
														color="primary"
														icon
														plain
														v-on="on"
														@click="
															openNewActivityForm(
																item
															)
														"
														><v-icon large
															>mdi-plus-circle-outline</v-icon
														></v-btn
													>
												</template>
												<span>{{
													$t(
														"views.sfip-activity-definition.btn-add"
													)
												}}</span>
											</v-tooltip>
											<!-- EDIT ACTIVITY BTN -->
											<v-tooltip
												top
												v-if="
													item &&
													checkIfActivity(
														item.categoryCode
													)
												"
											>
												<template
													v-slot:activator="{ on }"
												>
													<v-btn
														color="primary"
														icon
														plain
														v-on="on"
														@click="
															openEditActivityForm(
																item
															)
														"
														><v-icon large
															>mdi-circle-edit-outline</v-icon
														></v-btn
													>
												</template>
												<span>{{
													$t(
														"views.sfip-activity-definition.btn-edit"
													)
												}}</span>
											</v-tooltip>
										</template>
									</v-treeview>
								</v-col>
								<v-col :cols="8" class="d-flex flex-column justify-space-between">
									<div
										id="item-detail"
										v-if="
											currentDetail.viewName &&
											!showActivityForm
										"
									>
										<h3 class="h4">
											{{
												$t(
													"views.sfip-plan-overview.item-detail"
												)
											}}
										</h3>
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
										v-if="showActivityForm"
									>
										<jrs-form
											class="elevation-2 pa-5"
											:formFields="activityFormFields"
											:formData="activityFormData"
											:nbrOfColumns="1"
											:formTitle="
												currentActionType == 0
													? $t(
															'views.sfip-activity-definition.new-activity'
													  )
													: $t(
															'views.sfip-activity-definition.edit-activity'
													  )
											"
											v-if="
												showActivityForm &&
												activityFormFields.length > 0
											"
										>
											<template
												v-slot:form-actions="{
													validateFunc,
												}"
											>
												<v-btn
													color="secondary darken-1"
													class="mt-2 mr-1"
													text
													@click="closeActivityForm()"
													>X
													{{
														$t("general.close")
													}}</v-btn
												>
												<v-btn
													color="primary"
													class="mt-2 mx-1"
													@click="
														saveActivity(
															activityFormData,
															validateFunc
														)
													"
													>Save</v-btn
												>
											</template>
										</jrs-form>
									</div>
									<!-- ACTIVITY SCHEDULE -->
									<div v-if="showSchedule" class="mt-4">
										<act-schedule-editor :activityId="treeSelection[0].itemData.id"></act-schedule-editor>
										<!-- <jrs-simple-table
											viewName="SFIP_VI_ACTIVITY_SCHEDULE"
											:savePayload="{
												activityId: selectedActivityId,
											}"
											:dataSourceCondition="`ACTIVITY_ID = ${selectedActivityId}`"
											:hiddenFields="['ACTIVITY_ID']"
											:hiddenColumns="['ACTIVITY_DESCR', 'CODE']"
											:showSearchField="false"
										></jrs-simple-table> -->
									</div>
								</v-col>
							</v-row>
						</v-card-text>
					</v-card>
				</v-col>
			</v-row>
		</v-container>
	</v-content>
</template>

<script lang="ts">
	import Vue from "vue";
	import { mapActions, mapGetters } from "vuex";
	import {
		SfipPlan,
		SfipPrioritySet,
		SfipPriority,
		SfipGoal,
		SfipObjective,
		SfipIndicator,
		SfipActivity,
		GenericSqlSelectPayload,
		SqlActionType,
		GenericSqlPayload,
	} from "../../axiosapi/api";
	import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
	import SfipPlanSelectionForm from "../../components/SFIP/SfipPlanSelectionForm.vue";
	import JrsForm from "../../components/JrsForm.vue";
	import JrsSimpleReadonlyDetail from "../../components/JrsSimpleReadonlyDetail.vue";
	import { JrsFormField } from "../../models/JrsForm";
	import { SfipTreeItem, SfipItemCategory } from "../../models/SfipObjects";
	import FormMixin from "../../mixins/FormMixin";
	import SfipActivityScheduleEditor from "../../components/SFIP/SfipActivityScheduleEditor.vue";
	import SfipActivityOverviewVue from "./SfipActivityOverview.vue";

	interface DetailInfo {
		viewName: string;
		condition: string;
	}

	export default Vue.extend({
		components: {
			// "jrs-simple-table": JrsSimpleTable,
			"sfip-plan-selection-form": SfipPlanSelectionForm,
			"jrs-form": JrsForm,
			"jrs-simple-readonly-detail": JrsSimpleReadonlyDetail,
			"act-schedule-editor": SfipActivityScheduleEditor
		},
		mixins: [FormMixin],
		data: () => ({
			selectedActivities: [] as Array<any>,
			selectedPlanId: 0 as number,
			treeIsExpanded: false,
			planTreeItems: null as any,
			treeSelection: [] as SfipTreeItem[],
			activityFormFields: [] as JrsFormField[],
			activityCrudInfo: {} as {
				create_sp: string;
				update_sp: string;
				delete_sp: string;
			},
			activityFormData: {} as any,
			showActivityForm: false,
			canSave: true,
			currentActionType: 0 as SqlActionType,
		}),
		computed: {
			...mapGetters(["getUserUid", "getCurrentOffice"]),
			/**
			 * Selected Activity.
			 */
			selectedActivityId() {
				const selectedItem: SfipTreeItem = this.treeSelection[0];
				const selectedIsActivity =
					selectedItem?.categoryCode == SfipItemCategory.Activity;
				if (selectedItem && selectedIsActivity) {
					const activity: SfipActivity = selectedItem.itemData;
					return activity.id;
				} else {
					return null;
				}
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
					case SfipItemCategory.Activity:
						detInfo = {
							viewName: "SFIP_VI_ACTIVITIE",
							condition: `ID = ${treeItem.itemData.id}`,
						};
						break;
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
			/**
			 * Determine if activity schedule table should be visible.
			 */
			showSchedule(): boolean {
				const treeItemIsSelected = this.treeSelection.length > 0;
				const selectedItem: SfipTreeItem|null = treeItemIsSelected
					? this.treeSelection[0]
					: null;
				const selectionIsActivity:boolean =
					!!selectedItem && this.checkIfActivity(selectedItem.categoryCode);
				return selectionIsActivity;
			},
		},
		watch: {
			selectedPlanId(newPlanId: number, oldPlanId) {
				if (newPlanId && newPlanId != oldPlanId) {
					this.loadPlanTree();
					this.treeIsExpanded = false;
					this.treeSelection = [];
				}
			},
		},
		methods: {
			...mapActions("apiHandler", {
				getGenericSelect: "getGenericSelect",
				getJRSTableStructure: "getJRSTableStructure",
				getPlanTree: "getPlanTree",
				execSPCall: "execSPCall",
			}),
			...mapActions({
				showNewSnackbar: "showNewSnackbar",
			}),
			checkIfIndicator(itemCategory: SfipItemCategory) {
				return itemCategory == SfipItemCategory.Indicator;
			},
			checkIfActivity(itemCategory: SfipItemCategory) {
				return itemCategory == SfipItemCategory.Activity;
			},
			toggleTreeExpansion() {
				const tree: any = this.$refs.planTree;
				tree.updateAll(!this.treeIsExpanded);
				this.treeIsExpanded = !this.treeIsExpanded;
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
			/**
			 * Converts SFIP plan tree to a tree of SfipTreeItem for use in TreeView.
			 */
			parsePlanTree(
				planTreeItems: any[],
				currentCategory: SfipItemCategory
			): SfipTreeItem[] {
				let treeItems: SfipTreeItem[] = [];
				switch (currentCategory) {
					case SfipItemCategory.PrioritySet:
						treeItems = planTreeItems.map((set: SfipPrioritySet) => ({
							itemKey: `PRISET_${set.code}`,
							title: `Priority Set: ${set.code}`,
							category: "Priority Set",
							categoryCode: SfipItemCategory.PrioritySet,
							itemData: set,
							children: set.priorityList
								? this.parsePlanTree(
										set.priorityList,
										SfipItemCategory.Priority
								  )
								: [],
						}));
						break;
					case SfipItemCategory.Priority:
						treeItems = planTreeItems.map((priority: SfipPriority) => ({
							itemKey: `PRIORITY_${priority.code}`,
							title: `Priority: ${priority.code}`,
							category: "Priority",
							categoryCode: SfipItemCategory.Priority,
							itemData: priority,
							children: priority.goalList
								? this.parsePlanTree(
										priority.goalList,
										SfipItemCategory.Goal
								  )
								: [],
						}));
						break;
					case SfipItemCategory.Goal:
						treeItems = planTreeItems.map((goal: SfipGoal) => ({
							itemKey: `GOAL_${goal.code}`,
							title: `Goal: ${goal.code}`,
							category: "Goal",
							categoryCode: SfipItemCategory.Goal,
							itemData: goal,
							children: goal.objectiveList
								? this.parsePlanTree(
										goal.objectiveList,
										SfipItemCategory.Objective
								  )
								: [],
						}));
						break;
					case SfipItemCategory.Objective:
						treeItems = planTreeItems.map((obj: SfipObjective) => ({
							itemKey: `OBJ_${obj.code}`,
							title: `Objective: ${obj.code}`,
							category: "Objective",
							categoryCode: SfipItemCategory.Objective,
							itemData: obj,
							children: obj.indicatorList
								? this.parsePlanTree(
										obj.indicatorList,
										SfipItemCategory.Indicator
								  )
								: [],
						}));
						break;
					case SfipItemCategory.Indicator:
						treeItems = planTreeItems.map((ind: SfipIndicator) => ({
							itemKey: `IND_${ind.code}`,
							title: `Indicator: ${ind.code}`,
							category: "Indicator",
							categoryCode: SfipItemCategory.Indicator,
							itemData: ind,
							children: ind.activityList
								? this.parsePlanTree(
										ind.activityList,
										SfipItemCategory.Activity
								  )
								: [],
						}));
						break;
					case SfipItemCategory.Activity:
						treeItems = planTreeItems.map((act: SfipActivity) => ({
							itemKey: `ACT_${act.code}`,
							title: `Activity: ${act.code}`,
							category: "Activity",
							categoryCode: SfipItemCategory.Activity,
							itemData: act,
							children: [],
						}));
						break;
					default:
						break;
				}
				return treeItems;
			},
			async loadPlanTree() {
				try {
					const tmpPlanTree = await this.getPlanTree({
						planId: this.selectedPlanId,
						officeId: this.getCurrentOffice.HR_OFFICE_ID,
						includeActivities: true,
					});
					this.planTreeItems = this.parsePlanTree(
						tmpPlanTree,
						SfipItemCategory.PrioritySet
					);
				} catch (error) {
					this.showNewSnackbar({ typeName: "error", text: error });
				}
			},
			/**
			 * Set new tree selection and close Activity form if any item in the tree,
			 * other than the current Activity Indicator Indicator is clicked.
			 */
			onTreeSelectionChanged(selectedItems: SfipTreeItem[]) {
				this.treeSelection = selectedItems;
				if (this.showActivityForm) {
					this.closeActivityForm();
				}
			},
			openNewActivityForm(indicatorItem: SfipTreeItem) {
				if (indicatorItem.categoryCode == SfipItemCategory.Indicator) {
					this.treeSelection = [indicatorItem];
					this.currentActionType = SqlActionType.NUMBER_0;
					const indicator: SfipIndicator = indicatorItem.itemData;
					this.activityFormData = { INDICATOR_ID: indicator?.id };
					this.showActivityForm = true;
				}
			},
			async openEditActivityForm(activityItem: SfipTreeItem) {
				if (activityItem.categoryCode != SfipItemCategory.Activity) {
					return;
				}
				this.treeSelection = [activityItem];
				this.currentActionType = SqlActionType.NUMBER_1;
				try {
					const activity: SfipActivity = activityItem.itemData;
					const payload: GenericSqlSelectPayload = {
						viewName: "SFIP_VI_ACTIVITIE",
						whereCond: `ID = ${activity.id}`,
					};
					const queryResult = await this.getGenericSelect({
						genericSqlPayload: payload,
					});
					this.activityFormData = queryResult.table_data[0];
					this.showActivityForm = true;
				} catch (error) {
					this.showNewSnackbar({
						typeName: "error",
						text: error.message,
					});
				}
			},
			closeActivityForm() {
				this.showActivityForm = false;
				this.activityFormData = {};
			},
			async setupActivityForm() {
				const payload: GenericSqlSelectPayload = {
					viewName: "SFIP_VI_ACTIVITIE",
				};
				try {
					const tableStructure = await this.getJRSTableStructure({
						genericSqlPayload: payload,
					});
					this.activityCrudInfo = tableStructure.crud_info;

					let tmpFormFileds: Array<any> = tableStructure.columns
						.filter(
							(field: any) =>
								!field.hide_in_form &&
								field.column_name != "INDICATOR_ID"
						)
						.map((field: any) =>
							FormMixin.methods.parseJrsFormField(field)
						);

					// Setup selectFields
					this.activityFormFields = FormMixin.methods.setupSelectFields(
						tmpFormFileds,
						this.getCurrentOffice.HR_OFFICE_ID
					);
					this.canSave = true;
				} catch (error) {
					this.showNewSnackbar({ typeName: "error", text: error });
				}
			},
			async saveActivity(
				saveData: any,
				formValidateFunc: Function
			) {
				if (!formValidateFunc) {
					return;
				}
				this.canSave = false;
				const actionType = this.currentActionType || 0;
				const spName = SqlActionType.NUMBER_0
					? this.activityCrudInfo.create_sp
					: this.activityCrudInfo.update_sp;

				const savePayload: GenericSqlPayload = {
					spName: spName,
					actionType: actionType,
					jsonPayload: JSON.stringify(saveData),
					userUid: this.getUserUid,
					officeId: this.getCurrentOffice.HR_OFFICE_ID,
				};
				try {
					const res = await this.execSPCall(savePayload);
					this.showNewSnackbar({
						typeName: res.status,
						text: res.message,
					});

					if (actionType == SqlActionType.NUMBER_0) {
						this.activityFormData = {};
					} else {
						this.onTreeSelectionChanged(this.treeSelection);
					}
					this.loadPlanTree();
				} catch (error) {
					this.showNewSnackbar({
						typeName: "error",
						text: error.message,
					});
				}
			},
		},
		async mounted() {
			this.setupActivityForm();
		},
	});
</script>

<style scoped>
	.tree-column {
		border-right: solid 1px rgba(0, 0, 0, 0.12);
	}
	.tree-item-label {
		height: 40px;
		display: flex;
		align-items: center;
	}
</style>