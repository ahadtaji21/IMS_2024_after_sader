<template>
	<v-content>
		<v-container fluid fill-height>
			<v-row>
				<v-col :cols="12">
					<h1 class="text-capitalize">
						{{ $t("views.view-questionnaire-compilation.title") }}
					</h1>
					<p>
						{{
							$t(
								"views.view-questionnaire-compilation.page-descr"
							)
						}}
					</p>
				</v-col>
			</v-row>
			<v-row class="justify-center">
				<v-col :cols="12">
					<!-- MAIN FILTER -->
					<v-card class="elevation-0" outlined dense>
						<v-card-title>Filters</v-card-title>
						<v-card-text class="pa-0">
							<v-row justify="center">
								<v-col :md="4" :cols="12">
									<jrs-field
										:field="questTypeFilterField"
										v-model="selectedQuestionnaireTypeId"
										@input="setupFilterFields()"
										v-if="questTypeFilterField"
									></jrs-field>
								</v-col>
								<v-col :md="4" :cols="12">
									<jrs-field
										:field="questionnaireFilterField"
										v-model="selectedQuestionnaireId"
										v-if="questionnaireFilterField"
									></jrs-field>
								</v-col>
								<v-col :md="8" :cols="12">
									<div class="d-flex flex-row align-center">
										<v-text-field
											v-model="searchCode"
											:label="
												$t(
													'views.view-questionnaire-compilation.search-by-code'
												)
											"
											hide-details
											clearable
											outlined
											class="white"
											color="primary"
										>
											<template
												v-slot:append-outer
												class="mt-0"
											>
											</template>
										</v-text-field>
										<v-tooltip top>
											<template v-slot:activator="{ on }">
												<v-btn
													class="ml-3"
													color="primary"
													v-on="on"
													:disabled="!searchCode"
													@click="
														getQuestionnaireFromCode(
															searchCode
														)
													"
												>
													<span>Search</span>
													<v-icon>mdi-magnify</v-icon>
												</v-btn>
											</template>
											<span>{{
												$t(
													"views.view-questionnaire-compilation.search-by-code"
												)
											}}</span>
										</v-tooltip>
									</div>
								</v-col>
							</v-row>
						</v-card-text>
					</v-card>
				</v-col>
			</v-row>
			<v-row>
				<v-col class="d-flex justify-center">
					<v-card
						v-if="selectedQuestionnaireId"
						style="width: 80em"
					>
						<v-card-text>
							<jrs-questionnaire
								:questionnaireId="selectedQuestionnaireId"
								:key="`QUESTIONNAIRE_${selectedQuestionnaireId}`"
							></jrs-questionnaire>
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
	import { fieldType, JrsFormFieldSelect } from "../models/JrsForm";
	import FormMixin from "../mixins/FormMixin";
	import JrsQuestionnaire from "../components/JrsQuestionnaire.vue";
	import JrsForm from "../components/JrsForm.vue";
	import JrsField from "../components/JrsField.vue";
	import JrsSimpleTableVue from "../components/JrsSimpleTable.vue";
	import { ImsQuestionnaire } from "../axiosapi";

	export default Vue.extend({
		components: {
			// "jrs-simple-table": JrsSimpleTableVue,
			"jrs-questionnaire": JrsQuestionnaire,
			// "jrs-form": JrsForm,
			"jrs-field": JrsField,
		},
		mixins: [FormMixin],
		data: () => ({
			questTypeFilterField: null,
			questionnaireFilterField: null,
			selectedQuestionnaireTypeId: 0,
			selectedQuestionnaireId: 0,
			searchCode: "",
		}),
		computed: {
			...mapGetters({
				getCurrentOffice: "getCurrentOffice",
			}),
		},
		watch: {
			searchCode(to: string, from: string) {
				if (to != from && (!to || !from)) {
					const localThis: any = this as any;
					localThis.setupFilterFields();
				}
			},
		},
		methods: {
			...mapActions({
				showNewSnackbar: "showNewSnackbar",
			}),
			...mapActions("apiHandler", {
				getQuestionnaire: "getQuestionnaire",
			}),
			/**
			 * Setup filter form fields.
			 */
			setupFilterFields() {
				const localThis: any = this as any;
				const questionnaireCondition: string[] = [
					`ID IN ( SELECT QO.QUESTIONNAIRE_ID FROM IMS_QUESTIONNAIRE_OFFICE AS QO WHERE QO.OFFICE_ID = ${localThis.getCurrentOffice.HR_OFFICE_ID})`,
					`QUESTIONNAIRE_TYPE = ${localThis.selectedQuestionnaireTypeId}`,
				];
				const fieldsInfo: any[] = [
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
							"views.view-questionnaire-compilation.filter-questionnaire-type",
						readonly: localThis.searchCode ? true : false,
					},
					{
						column_name: "QUESTIONNAIRE_SELECT",
						type: "select",
						required: true,
						select_items_datasource: "IMS_QUESTIONNAIRE",
						itemKey: "ID",
						itemText: "TITLE",
						dataSourceCondition: questionnaireCondition.join(" AND "),
						form_order: 2,
						labelTranslationKey:
							"views.view-questionnaire-compilation.filter-questionnaire",
						readonly: localThis.searchCode ? true : false,
					},
				];

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

				localThis.questTypeFilterField = tmpSelectFields[0];
				localThis.questionnaireFilterField = tmpSelectFields[1];

				// Clear field values
				if (localThis.searchCode) {
					localThis.selectedQuestionnaireTypeId = 0;
					localThis.selectedQuestionnaireId = 0;
				}
			},
			/**
			 * Get questionnaire definition fro the provided questionnaire code.
			 */
			async getQuestionnaireFromCode(qCode: string) {
				const localThis: any = this as any;
				const officeId: number = localThis.getCurrentOffice.HR_OFFICE_ID;
				try {
					const tmpQuestionnaire: ImsQuestionnaire =
						await localThis.getQuestionnaire({
							officeId,
							questionnaireCode: qCode,
						});
					if (!tmpQuestionnaire) {
						const errorTransKey: string =
							"views.view-questionnaire-compilation.questionnaire-not-found";
						throw new Error(localThis.$t(errorTransKey).ToString());
					}
					localThis.selectedQuestionnaireId = tmpQuestionnaire.id;
				} catch (err) {
					localThis.showNewSnackbar({
						typeName: "error",
						text: err,
					});
				}
			},
		},
		mounted() {
			const localThis: any = this as any;
			localThis.setupFilterFields();
		},
	});
</script>

<style scoped>
</style>