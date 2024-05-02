<template>
	<v-content>
		<v-container fluid fill-height>
			<v-row>
				<v-col :cols="12">
					<h1 class="text-capitalize">
						{{
							questionnaireFromUrl
								? questionnaireFromUrl.title
								: $t("views.quest-review.title")
						}}
					</h1>
					<p style="max-width: 60em">
						{{
							questionnaireFromUrl
								? questionnaireFromUrl.descr
								: $t("views.quest-review.page-descr")
						}}
					</p>
				</v-col>
			</v-row>
			<v-row class="justify-center">
				<v-col :cols="12">
					<v-card class="elevation-0" outlined dense>
						<v-card-title>
							<span>{{ $t('general.filters') }}</span>
						</v-card-title>
						<v-divider></v-divider>
						<v-card-text class="py-0">
							<!-- FILTER FORM -->
							<jrs-form
								:formFields="filterFormFields"
								:formData="filterFormData"
								:nbrOfColumns="4"
								ref="filter-form"
							>
								<!-- ACTIONS -->
								<template v-slot:form-actions>
									<v-btn
										@click="downloadResults()"
										color="primary"
										class="ma-2"
										>Download results</v-btn
									>
									<v-btn
										@click="setupInstanceTable"
										color="primary"
										class="ma-2"
										>Load table</v-btn
									>
								</template>
							</jrs-form>
						</v-card-text>
					</v-card>
				</v-col>
			</v-row>
			<v-row>
				<v-col :cols="12">
					<jrs-table
						:title="`${$t('views.quest-review.instance-table')}`"
						:headers="instTableHeader"
						:rows="instTableRows"
						:showSearchbar="true"
						pkName="id"
					>
						<template v-slot:itemActions="{ rowItem }">
							<div
								class="d-flex justify-center"
								style="width: 100%"
							>
								<!-- EDIT ICON -->
								<!-- <v-tooltip top>
									<template v-slot:activator="{ on }">
										<v-icon
											@click="openDialog(rowItem, false)"
											class="mx-1"
											small
											v-on="on"
											>mdi-circle-edit-outline</v-icon
										>
									</template>
									<span>{{
										$t("general.crud.tooltip-edit")
									}}</span>
								</v-tooltip> -->
								<!-- READONLY ICON -->
								<v-tooltip top>
									<template v-slot:activator="{ on }">
										<v-icon
											@click="openDialog(rowItem, true)"
											class="mx-1"
											small
											v-on="on"
											>mdi-eye-outline</v-icon
										>
									</template>
									<span>{{
										$t("general.crud.tooltip-view")
									}}</span>
								</v-tooltip>
							</div>
						</template>
					</jrs-table>
				</v-col>
			</v-row>
			<v-dialog
				v-model="showDialog"
				persistent
				retain-focus
				:scrollable="true"
				:overlay="false"
				:max-width="'55em'"
				transition="dialog-transition"
				v-if="selectedQuestInstance"
			>
				<v-card>
					<v-card>
						<v-card-text>
							<jrs-questionnaire
								:questionnaireId="
									selectedQuestInstance.questionnaireId
								"
								:questionnaireData="selectedQuestInstance"
								:readonly="dialogIsReadonly"
							></jrs-questionnaire>
						</v-card-text>
						<v-card-actions>
							<v-btn
								text
								color="secondary darken-1"
								@click="closeDialog()"
								>X {{ $t("general.close") }}</v-btn
							>
						</v-card-actions>
					</v-card>
				</v-card>
			</v-dialog>
		</v-container>
	</v-content>
</template>

<script lang="ts">
	import Vue from "vue";
	import { mapGetters, mapActions } from "vuex";
	import { JrsFormField, JrsFormFieldSelect } from "../models/JrsForm";
	import FormMixin from "../mixins/FormMixin";
	import UtilMix from "../mixins/UtilMix";
	import JrsTable from "../components/JrsTable.vue";
	import JrsForm from "../components/JrsForm.vue";
	import JrsQuestionnaire from "../components/JrsQuestionnaire.vue";
	import {
		GenericConditionRule,
		GenericExcelColumnLabel,
		GenericSqlSelectPayload,
		ImsQuestionnaire,
		ImsQuestionnaireInstance,
		RuleOperator,
	} from "../axiosapi";

	export default Vue.extend({
		components: {
			"jrs-table": JrsTable,
			"jrs-questionnaire": JrsQuestionnaire,
			"jrs-form": JrsForm,
		},
		props: {
			questionnaireCode: {
				type: String,
				required: false,
				default: null,
			},
		},
		data: () => ({
			// questTypeFilterField: null,
			// questionnaireFilterField: null,
			selectedQuestInstance: null,
			instTableHeader: [
				// { value: "id", text: "id" },
				{ value: "questionnaireCode", text: "Questionnaire Code" },
				{ value: "questionnaireTitle", text: "Questionnaire Title" },
				{ value: "fillInDate", text: "Fill in Date", isDateTime: true },
				{ value: "reviewDate", text: "Review Date" },
				{ value: "statusName", text: "Status" },
				{ text: "Actions", value: "action" },
			],
			instTableRows: [],
			instanceList: [],
			dialogIsReadonly: true,
			showDialog: false,
			filterGender: 0,
			additionalFiltersConfig: [],
			filterValues: null,
			filterFormFields: [],
			filterFormData: {
				QUESTIONNAIRE_SELECT: null,
				SUBMIT_DATE_FROM: null,
			},
			canReviewQuestionnaire: false,
			questionnaireFromUrl: null,
		}),
		mixins: [FormMixin, UtilMix],
		computed: {
			...mapGetters({
				getCurrentOffice: "getCurrentOffice",
				getLookupsByTypeCode: "getLookupsByTypeCode",
				getUserLocale: "getUserLocale",
			}),
			selectedQuestionnaireId() {
				const localThis: any = this as any;
				return localThis.filterFormData?.QUESTIONNAIRE_SELECT;
			},
			conditionRules() {
				const localThis: any = this as any;
				let ruleList: GenericConditionRule[] = [];
				//const op: RuleOperator = { sqlOperator: "eq" };
				const op: RuleOperator = RuleOperator.NUMBER_2;

				// Submission date
				if (localThis.filterFormData.SUBMIT_DATE_FROM) {
					const dateVal: any = localThis.filterFormData.SUBMIT_DATE_FROM;
					const dateString: string =
						dateVal instanceof Date ? dateVal.toISOString() : dateVal;
					ruleList.push({
						operandA: "FILL_IN_DATE",
						operandB: `CAST('${dateString}' as date)`,
						operator: RuleOperator.NUMBER_3,
					});
				}
				if (localThis.filterFormData.SUBMIT_DATE_TO) {
					const dateVal: any = localThis.filterFormData.SUBMIT_DATE_TO;
					const dateString: string =
						dateVal instanceof Date ? dateVal.toISOString() : dateVal;
					ruleList.push({
						operandA: "FILL_IN_DATE",
						operandB: `CAST('${dateString}' as date)`,
						operator: RuleOperator.NUMBER_1,
					});
				}

				// Gender condition
				if (localThis.filterFormData.GENDER) {
					ruleList.push({
						operandA: "RESPONDENT_GENDER_ID",
						operandB: localThis.filterFormData.GENDER,
						operator: op,
					});
				}

				// Age range
				if (localThis.filterFormData.MIN_AGE) {
					ruleList.push({
						operandA: "RESPONDENT_AGE",
						operandB: localThis.filterFormData.MIN_AGE,
						operator: RuleOperator.NUMBER_3,
					});
				}
				if (localThis.filterFormData.MAX_AGE) {
					ruleList.push({
						operandA: "RESPONDENT_AGE",
						operandB: localThis.filterFormData.MAX_AGE,
						operator: RuleOperator.NUMBER_1,
					});
				}
				return ruleList;
			},
		},
		// watch: {
		// 	selectedQuestionnaireId(to: number, from: number) {
		// 		if (to != 0 && to != from) {
		// 			this.setupInstanceTable();
		// 		}
		// 	},
		// },
		methods: {
			...mapActions("apiHandler", [
				"getQuestionnaireInstances",
				"generateExcelForQuestionnaire",
				"getQuestionnaireInstancesWithConditions",
				"getOfficeQuestionnaireList",
			]),
			...mapActions({
				showNewSnackbar: "showNewSnackbar",
			}),
			/**
			 * Get a list of questionnaires available for the current office.
			 * @param {Number} officeId - Current office.
			 * @returns {Promise<ImsQuestionnaire[]>} A list of available questionnaires.
			 */
			async getAvailableQuestionnaires(
				officeId: number
			): Promise<ImsQuestionnaire[]> {
				const localThis: any = this as any;
				try {
					const questionnaireList: ImsQuestionnaire[] =
						await localThis.getOfficeQuestionnaireList({ officeId });
					return questionnaireList;
				} catch (error) {
					localThis.showNewSnackbar({
						typeName: "error",
						text: error,
					});
					throw error;
				}
			},
			// /**
			//  * Check if the current office is connected to questionnaire coming from url.
			//  * @param {Number} officeId - Current office ID.
			//  * @param {Strign} questCode - Code of the questionnaire to check.
			//  * @returns {Boolean} If the office should have access to the questionnaire results.
			//  */
			// async hasQuestionnaireRights(officeId:number, questCode:string):Promise<boolean>{
			// 	const localThis:any = this as any;
			// 	try {
			// 		const questionnaireList:ImsQuestionnaire[] = await localThis.getOfficeQuestionnaireList({ officeId });
			// 		return questionnaireList.some((quest:ImsQuestionnaire) => quest.code == questCode);
			// 	} catch (error) {
			// 		localThis.showNewSnackbar({
			// 			typeName: "error",
			// 			text: error,
			// 		});
			// 		throw error;
			// 	}
			// },
			/**
			 * Setup filter form fields.
			 */
			setupFilterFields() {
				const localThis: any = this as any;
				let fieldsInfo: any[] = [
					// Date From
					{
						column_name: "SUBMIT_DATE_FROM",
						type: "date",
						required: false,
						form_order: 20,
						labelTranslationKey: "views.quest-review.filter-date-from",
					},
					// Date To
					{
						column_name: "SUBMIT_DATE_TO",
						type: "date",
						required: false,
						form_order: 30,
						labelTranslationKey: "views.quest-review.filter-date-to",
					},
					// Min Age
					{
						column_name: "MIN_AGE",
						type: "number",
						required: false,
						form_order: 40,
						labelTranslationKey: "views.quest-review.filter-min-age",
						// operator: RuleOperators.gte,
					},
					// Max Age
					{
						column_name: "MAX_AGE",
						type: "number",
						required: false,
						form_order: 50,
						labelTranslationKey: "views.quest-review.filter-max-age",

						// operator: RuleOperators.lte,
					},
					// Gender
					{
						column_name: "GENDER",
						type: "select",
						required: false,
						select_lookup_code: "GENDER",
						itemKey: "IMS_LOOKUP_ID",
						itemText: "IMS_LOOKUP_VALUE",
						form_order: 60,
						labelTranslationKey: "views.quest-review.filter-gender",

						// operator: RuleOperators.eq,
					},
				];

				// Add questionnaire type selection if there is no specific Questionnaire Code defined.
				if (!localThis.questionnaireCode) {
					fieldsInfo = [
						// Questionnaire Type
						{
							column_name: "QUESTIONNAIRE_TYPE_SELECT",
							type: "select",
							required: true,
							select_items_datasource: "IMS_QUESTIONNAIRE_TYPE",
							itemKey: "ID",
							itemText: "CODE",
							dataSourceCondition: null,
							form_order: 1,
							labelTranslationKey:
								"views.quest-review.filter-questionnaire-type",
							additional_config: '{"col_span": 6}',
						},
						// Questionnaire
						{
							column_name: "QUESTIONNAIRE_SELECT",
							type: "select",
							required: true,
							select_items_datasource: "IMS_QUESTIONNAIRE",
							itemKey: "ID",
							itemText: "TITLE",
							dataSourceCondition: `ID IN ( SELECT QO.QUESTIONNAIRE_ID FROM IMS_QUESTIONNAIRE_OFFICE AS QO WHERE QO.OFFICE_ID = ${localThis.getCurrentOffice.HR_OFFICE_ID})`,
							form_order: 2,
							labelTranslationKey:
								"views.quest-review.filter-questionnaire",
							additional_config: JSON.stringify({
								col_span: 6,
								dependencyRules: [
									{
										dependencyType: "filter",
										masterFieldName:
											"QUESTIONNAIRE_TYPE_SELECT",
										targetFieldName: "QUESTIONNAIRE_SELECT",
										targetCondition:
											"QUESTIONNAIRE_TYPE = ##QUESTIONNAIRE_TYPE_SELECT##",
									},
								],
							}),
						},
						...fieldsInfo,
					];
				}

				let filterFields: JrsFormFieldSelect[] = fieldsInfo.map(
					(info: any) => {
						let tmpField: JrsFormFieldSelect =
							localThis.parseJrsFormField(info);
						tmpField.label = localThis
							.$t(tmpField.labelTranslationKey)
							.toString();
						return tmpField;
					}
				);

				const tmpSelectFields: JrsFormFieldSelect[] =
					localThis.setupSelectFields(
						filterFields,
						localThis.getCurrentOffice.HR_OFFICE_ID
					);

				// localThis.questTypeFilterField = tmpSelectFields[0];
				// localThis.questionnaireFilterField = tmpSelectFields[1];

				localThis.filterFormFields = [...tmpSelectFields];
			},
			/**
			 * Setup columns and data for Questoinnaire-Instace table
			 */
			async setupInstanceTable() {
				const localThis: any = this as any;
				// Check validity of questionnaire selection

				if (!localThis.selectedQuestionnaireId) {
					// Feedback to user
					localThis.showNewSnackbar({
						typeName: "error",
						text: localThis.$t("error.no-valid-questionnaire"),
					});
					return;
				}
				// const instanceInfo: any = await localThis.getQuestionnaireInstances(
				// 	{
				// 		officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
				// 		questionnaireId: localThis.selectedQuestionnaireId,
				// 	}
				// );
				try {
					const payload: any = {
						officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
						questionnaireId: localThis.selectedQuestionnaireId,
						conditionRules: localThis.conditionRules,
					};
					const instanceInfo: any =
						await localThis.getQuestionnaireInstancesWithConditions(
							payload
						);
					localThis.instanceList = instanceInfo.instances;
					localThis.instTableRows = instanceInfo.instanceRows;
				} catch (error) {
					// Feedback to user
					localThis.showNewSnackbar({
						typeName: "error",
						text: error,
					});
				}
			},
			/**
			 * Show readonly questionnaire instance data.
			 */
			openDialog(instanceRow: any, setReadonly: boolean) {
				const localThis: any = this as any;
				localThis.selectedQuestInstance = localThis.instanceList.find(
					(inst: ImsQuestionnaireInstance) => inst.id == instanceRow.id
				);
				localThis.dialogIsReadonly = setReadonly;
				localThis.showDialog = true;
			},
			closeDialog() {
				const localThis: any = this as any;
				localThis.selectedQuestInstance = null;
				localThis.dialogIsReadonly = true;
				localThis.showDialog = false;
			},
			/**
			 * Download results of the requested questionnaire
			 */
			async downloadResults() {
				const localThis: any = this as any;
				const fileName: string = `Questionnaire_Results_${new Date()
					.toISOString()
					.split(/[-:.]/)
					.join("")}.xlsx`;

				// Check validity of questionnaire selection
				if (!localThis.selectedQuestionnaireId) {
					// Feedback to user
					localThis.showNewSnackbar({
						typeName: "error",
						text: localThis.$t("error.no-valid-questionnaire"),
					});
					return;
				}
				try {
					const colLabels: Array<GenericExcelColumnLabel> = [
						{
							columnName: "QUESTIONNAIRE_CODE",
							columnLabel: localThis
								.$t("views.quest-review.questionnaire-code")
								.toString(),
						},
						{
							columnName: "FILL_IN_DATE",
							columnLabel: localThis
								.$t("views.quest-review.submit-date")
								.toString(),
						},
						{
							columnName: "FILL_IN_USER_DESCR",
							columnLabel: localThis
								.$t("views.quest-review.submit-user")
								.toString(),
						},
						{
							columnName: "FILL_IN_HOUSEHOLD_DESCR",
							columnLabel: localThis
								.$t("views.quest-review.submit-household")
								.toString(),
						},
						{
							columnName: "SELECTED_PROJECT_DESCR",
							columnLabel: localThis
								.$t("views.quest-review.project")
								.toString(),
						},
						{
							columnName: "SELECTED_SETTLEMENT_DESCR",
							columnLabel: localThis
								.$t("views.quest-review.settlement")
								.toString(),
						},
					];
					const bytes: Array<any> =
						await localThis.generateExcelForQuestionnaire({
							questionnaireId: localThis.selectedQuestionnaireId,
							conditionRules: localThis.conditionRules,
							columnLabels: colLabels,
						});
					localThis.downloadFileFromBytes(bytes, fileName);
				} catch (error) {
					// Feedback to user
					localThis.showNewSnackbar({
						typeName: "error",
						text: error,
					});
				}
			},
		},
		async mounted() {
			const localThis: any = this as any;
			localThis.setupFilterFields();
			localThis.filterValues = {
				GENDER: null,
				MIN_AGE: null,
				MAX_AGE: null,
			};

			// Verify if URL defines a specific questionnaire to review
			if (localThis.questionnaireCode) {
				const availableQuestionnaires: ImsQuestionnaire[] =
					await localThis.getAvailableQuestionnaires(
						localThis.getCurrentOffice.HR_OFFICE_ID
					);
				localThis.questionnaireFromUrl = availableQuestionnaires.find(
					(quest: ImsQuestionnaire) =>
						quest.code == localThis.questionnaireCode
				);
				localThis.canReviewQuestionnaire = localThis.questionnaireFromUrl
					? true
					: false;

				// Redirect to 404 if office has no access to questionnaire results
				if (localThis.canReviewQuestionnaire) {
					localThis.filterFormData.QUESTIONNAIRE_SELECT =
						localThis.questionnaireFromUrl.id;
				} else {
					const locale: string = localThis.getUserLocale;
					localThis.$router.push(
						`/${locale}/404?mk=questionnaire-rights`
					);
				}
			}

			// Default date filter to current year
			const now: Date = new Date();
			const year: number = now.getUTCFullYear();
			const fromDateDefault: Date = new Date(`${year}-01-01`);
			console.log(new Date(`${year}-01-01`));
			localThis.filterFormData.SUBMIT_DATE_FROM =
				fromDateDefault.toLocaleDateString();
		},
	});
</script>

<style scoped>
</style>