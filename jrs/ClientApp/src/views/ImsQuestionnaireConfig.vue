<template>
	<v-content>
		<v-container fluid fill-height>
			<v-row>
				<v-col :cols="12">
					<v-tabs
						v-model="activeTab"
						background-color="primary darken-1"
						dark
						centered
					>
						<v-tab
							v-for="tabData in tabsData"
							:key="tabData.code"
							>{{ $t(tabData.labelKey) }}</v-tab
						>
					</v-tabs>
					<v-tabs-items v-model="activeTab" class="tab-item-wrapper">
						<v-tab-item
							v-for="tabData in tabsData"
							:key="tabData.code"
						>
							<jrs-simple-table
								:viewName="tabData.tableName"
								:nbrOfFormColumns="
									tabData.tableName == 'IMS_VI_QUESTIONNAIRE'
										? 3
										: 1
								"
								:overrideFormMaxWidth="'65em'"
							>
								<!-- QUESTIONNAIRE ACTIONS -->
								<template
									v-if="
										tabData.tableName ==
										'IMS_VI_QUESTIONNAIRE'
									"
									v-slot:simple-table-item-actions="{
										simpleItemRowItem,
										refreshData,
									}"
								>
									<v-icon
										@click="
											setupQuestionOrderDialog(
												simpleItemRowItem.QUESTION_RELATIONS,
												simpleItemRowItem.ID,
												refreshData
											)
										"
										>mdi-elevator</v-icon
									>
								</template>
							</jrs-simple-table>
						</v-tab-item>
					</v-tabs-items>
				</v-col>
			</v-row>
			<!-- QUESTION ORDER DIALOG -->
			<v-dialog
				v-model="showQuestionOrderDialog"
				persistent
				retain-focus
				:overlay="false"
				max-width="100em"
				class="scrollable-dialog"
				transition="dialog-transition"
			>
				<jrs-questionnair-question
					:questionnaireId="currentQuestionnaireId"
                    v-if="currentQuestionnaireId"
				>
					<template v-slot:actions>
						<v-btn
							color="secondary darken-1"
							class="mt-2 mr-1"
							text
							@click="closeConfigurator()"
							>X {{ $t("general.close") }}</v-btn
						>
					</template>
				</jrs-questionnair-question>
			</v-dialog>
		</v-container>
	</v-content>
</template>

<script lang="ts">
	import Vue from "vue";
	import { mapGetters, mapActions } from "vuex";
	import { GenericSqlPayload, SqlActionType } from "../axiosapi/api";
	import UtilMix from "../mixins/UtilMix";
	import JrsSimpleTable from "../components/JrsSimpleTable.vue";
	import JrsQuestionnaireQuestionConfigurator from "../components/JrsQuestionnaireQuestionConfigurator.vue";
	import JrsField from "../components/JrsField.vue";

	export default Vue.extend({
		components: {
			"jrs-simple-table": JrsSimpleTable,
			"jrs-questionnair-question": JrsQuestionnaireQuestionConfigurator,
			// "jrs-field": JrsField,
		},
		mixins: [UtilMix],
		data: () => ({
			isSaving: false,
			activeTab: null,
			tabsData: [
				{
					code: "QUESTIONNAIRE_TYPE",
					labelKey: "datasource.ims.questionnaire-type.title",
					tableName: "IMS_QUESTIONNAIRE_TYPE",
				},
				{
					code: "QUESTIONNAIRE",
					labelKey: "datasource.ims.questionnaire.title",
					tableName: "IMS_VI_QUESTIONNAIRE",
				},
				{
					code: "QUESTIONNAIRE_TAB",
					labelKey: "datasource.ims.questionnaire-tab.title",
					tableName: "IMS_VI_QUESTIONNAIRE_TAB",
				},
				{
					code: "QUESTION",
					labelKey: "datasource.ims.question.title",
					tableName: "IMS_VI_QUESTION",
				},
				{
					code: "QUESTION_ANSWER_OPTIONS",
					labelKey: "datasource.ims.answer-options.title",
					tableName: "IMS_VI_QUESTION_ANSWER_OPTIONS",
				},
					{
					code: "QUESTION_ANSWER_OPTIONS_MATRIX",
					labelKey: "datasource.ims.answer-options-matrix.title",
					tableName: "IMS_VI_QUESTION_ANSWER_OPTIONS_MATRIX",
				},
			],
			showQuestionOrderDialog: false,
			currentQuestionList: [],
			currentQuestionnaireId: null,
			currentTabList: [],
			tableRefreshFunc: null,
			questionOrderTimeout: null,
		}),
		computed: {
			...mapGetters({
				getUserUid: "getUserUid",
				getCurrentOffice: "getCurrentOffice",
			}),
		},
		methods: {
			...mapActions({
				showNewSnackbar: "showNewSnackbar",
			}),
			...mapActions("apiHandler", {
				execSPCall: "execSPCall",
				getQuestionnaireTabs: "getQuestionnaireTabs",
			}),
			// setupQuestionnaireTabSelectors(){
			//     {
			// 			column_name: "QUESTIONNAIRE_TYPE_SELECT",
			// 			type: "select",
			// 			required: true,
			// 			select_items_datasource: "IMS_QUESTIONNAIRE_TYPE",
			// 			itemKey: "ID",
			// 			itemText: "CODE",
			// 			dataSourceCondition: null,
			// 			form_order: 1,
			// 			labelTranslationKey:
			// 				"views.view-questionnaire-compilation.filter-questionnaire-type",
			// 		}
			// },
            closeConfigurator(){
                const localThis:any = this as any;
                // Close ModalForm
                localThis.showQuestionOrderDialog = false;
                // Clear "current" variables
                localThis.currentQuestionnaireId = null;
                localThis.currentQuestionList = [];
                // Refresh table data
                localThis.tableRefreshFunc();
            },
			async setupQuestionOrderDialog(
				questionList: any[],
				questionnaireId: Number,
				refreshFunc: Function
			) {
				const localThis: any = this as any;
				localThis.currentQuestionnaireId = questionnaireId;
				localThis.showQuestionOrderDialog = true;
				localThis.tableRefreshFunc = refreshFunc;

				// localThis.currentQuestionList = [...questionList.map((questionItem:any, index) => ({
				//     ...questionItem,
				//     ...{ ORDINAL_POSITION: index },
				// }))];
				// localThis.currentQuestionnaireId = questionnaireId;
				// localThis.showQuestionOrderDialog = true;
				// localThis.tableRefreshFunc = refreshFunc;

				// // Get available tabs
				// const officeId:number = localThis.getCurrentOffice.HR_OFFICE_ID;
				// localThis.currentTabList = await localThis.getQuestionnaireTabs({officeId, questionnaireId});
			},
			/**
			 * Switch the order of two questions.
			 * @param {number} index_a Index of first item.
			 * @param {number} index_b Index of second item.
			 */
			switchOrder(index_a: any, index_b: any) {
				const localThis: any = this as any;
				// Get current items
				const [item_a, item_b] = [
					localThis.currentQuestionList[index_a],
					localThis.currentQuestionList[index_b],
				];
				// Create new versions of the items
				const new_a = { ...item_a, ...{ ORDINAL_POSITION: index_b } };
				const new_b = { ...item_b, ...{ ORDINAL_POSITION: index_a } };
				// Assign new items in the correct oerder
				localThis.currentQuestionList[index_a] = new_b;
				localThis.currentQuestionList[index_b] = new_a;
				// Assign new copy of array to triggert view update
				localThis.currentQuestionList = [...localThis.currentQuestionList];
			},
			/**
			 * Move question to a new index in the list.
			 * @param {number} fromIndex Index of the starting position of the question.
			 * @param {number} toIndex Index of the arrival position of the question.
			 */
			moveToIndex(fromIndex: number, toIndex: number) {
				const localThis: any = this as any;
				console.log("Moving...");
				// Check if index is valid
				if (
					toIndex < 0 ||
					toIndex > localThis.currentQuestionList.length - 1
				) {
					// Reset values
					localThis.currentQuestionList = [
						...localThis.currentQuestionList.map(
							(item: any, index: number) => ({
								...item,
								...{ ORDINAL_POSITION: index },
							})
						),
					];

					// Feedback to user
					localThis.showNewSnackbar({
						typeName: "error",
						text: localThis.$t("error.invalid-index"),
					});
					return;
				}
				// Move to index
				const questionIndexItem: any = localThis.currentQuestionList.splice(
					fromIndex,
					1
				)[0];
				localThis.currentQuestionList.splice(toIndex, 0, questionIndexItem);
				// Assign new ORDINAL_POSITION
				localThis.currentQuestionList = localThis.currentQuestionList.map(
					(item: any, index: number) => ({
						...item,
						...{ ORDINAL_POSITION: index },
					})
				);
			},
			saveQuestionOrder() {
				const localThis: any = this as any;
				localThis.isSaving = true;

				//Prepare payload
				const saveData = {
					external_data: {
						QUESTIONNAIRE_ID: localThis.currentQuestionnaireId,
					},
					rows: localThis.currentQuestionList,
				};
				const savePayload: GenericSqlPayload = {
					spName: "IMS_SP_INS_UPD_DEL_QUESTIONNAIRE_QUESTION",
					actionType: SqlActionType.NUMBER_1, // UPDATE
					jsonPayload: JSON.stringify(saveData),
					userUid: localThis.getUserUid,
					officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
				};

				// Save Data
				localThis
					.execSPCall(savePayload)
					.then((res: any) => {
						// Feedback to user
						localThis.showNewSnackbar({
							typeName: res.status,
							text: res.message,
						});

						// Close ModalForm
                        localThis.closeConfigurator();
					})
					.catch((err: any) => {
						// Feedback to user
						localThis.showNewSnackbar({
							typeName: "error",
							text: err.message,
						});
					})
					.finally(() => {
						localThis.isSaving = false;
					});
			},
			getDebouncedOrderChange(func: Function, wait: number) {
				// var ;
				const localThis: any = this as any;
				console.log(
					`Initialized Timeout: ${localThis.questionOrderTimeout}`
				);
				return () => {
					const context: any = this as any;
					const args = arguments;

					//function to be called after timeout
					let later = () => {
						localThis.questionOrderTimeout = null;
						func.apply(context, args);
					};

					// Set new timeout
					clearTimeout(localThis.questionOrderTimeout);
					console.log(`Old Timeout: ${localThis.questionOrderTimeout}`);
					localThis.questionOrderTimeout = setTimeout(later, wait);
					console.log(`New Timeout: ${localThis.questionOrderTimeout}`);
				};
			},
		},
		mounted() {
			const localThis: any = this as any;
		},
	});
</script>

<style scoped>
	.tab-item-wrapper {
		padding: 0.2em 1em 1em 1em;
	}
	.question-order-item {
		border: solid 1px coral;
		border-radius: 25px;
	}
	.question-order-wrapper {
		width: 100%;
		/* border-top: 1px solid rgba(0, 0, 0, 0.12);
		border-bottom: 1px solid rgba(0, 0, 0, 0.12); */
		margin: 5px;
	}

	.scrollable-dialog {
		height: 75vh;
	}

	.scrollable-dialog-card {
		height: 75vh;
		display: flex;
		flex-direction: column;
	}
	.scrollable-card-text {
		flex-grow: 1;
		overflow: auto;
	}
</style>