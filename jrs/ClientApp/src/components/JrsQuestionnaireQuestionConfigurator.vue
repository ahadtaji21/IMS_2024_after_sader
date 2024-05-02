<template>
	<div>
		<v-card>
			<v-card-title>
				<div class="d-flex flex-row" style="width: 100%">
					<span
						>{{ questionnaire.code }} -
						{{ questionnaire.title }}</span
					>
					<v-spacer></v-spacer>
					<v-tooltip top>
						<template v-slot:activator="{ on }">
							<v-btn
								icon
								color="primary"
								v-on="on"
								@click="updateAllPannels(true)"
							>
								<v-icon>mdi-arrow-expand-vertical</v-icon>
							</v-btn>
						</template>
						<span>{{
							$t("views.view-questionnaire-config.expand-all")
						}}</span>
					</v-tooltip>
					<v-tooltip top>
						<template v-slot:activator="{ on }">
							<v-btn
								icon
								color="primary"
								v-on="on"
								@click="updateAllPannels(false)"
							>
								<v-icon>mdi-arrow-collapse-vertical</v-icon>
							</v-btn>
						</template>
						<span>{{
							$t("views.view-questionnaire-config.collapse-all")
						}}</span>
					</v-tooltip>
				</div>
			</v-card-title>
			<v-card-text>
				<v-expansion-panels popout multiple v-model="expandedTabPanels">
					<v-expansion-panel
						v-for="(questTab, qtIndex) in currentTabList"
						:key="`GROUPING_${questTab.code}`"
					>
						<v-expansion-panel-header
							:color="
								expandedTabPanels.includes(qtIndex)
									? 'primary white--text'
									: ''
							"
						>
							<div>
								{{ questTab.descr }} (No. Questions in tab:
								{{ getFieldsByTab(questTab.id).length }})
							</div>
						</v-expansion-panel-header>
						<v-expansion-panel-content>
							<v-list>
								<v-list-item
									v-for="questQuestion in getFieldsByTab(
										questTab.id
									)"
									:key="`QUESTION_${questTab.code}_${questQuestion.id}`"
								>
									<!-- <div>DATA: {{questQuestion}}</div> -->
									<div
										class="
											d-flex
											flex-row
											justify-space-between
											mb-5
										"
										style="width: 100%"
									>
										<!-- QUESTION TEXT -->
										<v-tooltip top>
											<template v-slot:activator="{ on }">
												<span v-on="on">{{
													trucateString(
														questQuestion.question
															.questionText,
														100
													)
												}}</span>
											</template>
											<span>{{
												questQuestion.question
													.questionText
											}}</span>
										</v-tooltip>

										<v-spacer></v-spacer>

										<!-- TAB SELECTION -->
										<v-select
											:label="'Tab'"
											:id="`QUEST_TAB_${questQuestion.id}`"
											v-model="questQuestion.tabId"
											:items="currentTabList"
											hide-details
											outlined
											dense
											item-value="id"
											item-text="descr"
											class="flex-grow-0 mr-3"
											style="max-width: 20em"
										></v-select>

										<!-- REUIRED QUESTION -->
										<v-checkbox
											:label="'Mandatory'"
											:id="`QUEST_REQUIRED_${questQuestion.id}`"
											v-model="questQuestion.isRequired"
											hide-details
											outlined
											dense
											class="flex-grow-0 mr-3 my-1"
										></v-checkbox>

										<!-- POSITION SELECTION -->
										<v-text-field
											:value="
												questQuestion.ordinalPosition
											"
											number
											hide-details
											outlined
											dense
											style="width: 4em"
											class="flex-grow-0 mr-3"
											:key="`ORDER_INDEX_${questQuestion.id}`"
											@input="
												getDebouncedOrderChange(() => {
													moveToIndex(
														$event,
														questQuestion.id
													);
												}, 1000)()
											"
										></v-text-field>
									</div>
									<v-divider class="mt-3"></v-divider>
								</v-list-item>
							</v-list>
						</v-expansion-panel-content>
					</v-expansion-panel>
				</v-expansion-panels>
			</v-card-text>
			<v-divider class="mt-3"></v-divider>
			<v-card-actions>
				<slot
					name="actions"
					:saveChanges="updateQuestionnaireQuestion"
				></slot>
				<v-btn
					color="primary"
					class="mt-2 mx-1"
					@click="updateQuestionnaireQuestion()"
					:disabled="disableSave"
					>{{ $t("general.save") }}</v-btn
				>
			</v-card-actions>
		</v-card>
	</div>
</template>

<script lang="ts">
	import Vue from "vue";
	import { mapActions, mapGetters } from "vuex";
	import {
		ImsQuestionnaire,
		ImsQuestionnaireQuestion,
		ImsQuestionnaireTab,
	} from "../axiosapi";

	export default Vue.extend({
		props: {
			questionnaireId: {
				type: Number,
				required: true,
			},
		},
		data: () => ({
			questionnaire: {},
			expandedTabPanels: [],
			questionOrderTimeout: null,
			qqListSnapshot: [],
			disableSave: false,
		}),
		computed: {
			...mapGetters({
				getCurrentOffice: "getCurrentOffice",
			}),
			currentQuestionnaireQuestionList(): ImsQuestionnaireQuestion[] {
				const localThis: any = this as any;
				if (!localThis.questionnaire) {
					return [];
				}
				let ret: ImsQuestionnaireQuestion[] =
					localThis.questionnaire?.questionnaireQuestionList || [];
				ret = ret.map((qq: ImsQuestionnaireQuestion) => {
					if (!qq.tabId) {
						qq.tabId = 0;
					}
					return qq;
				});
				return ret;
			},
			currentTabList(): ImsQuestionnaireTab[] {
				const localThis: any = this as any;
				if (!localThis.questionnaire) {
					return [];
				}
				const defaultTab: ImsQuestionnaireTab = {
					id: 0,
					code: "MAIN",
					descr: "Main Data",
					questionnaireId: localThis.questionnaire.id,
				};
				let ret: ImsQuestionnaireTab[] =
					localThis.questionnaire?.questionnaireTabList || [];
				ret = ret.sort((a: ImsQuestionnaireTab, b: ImsQuestionnaireTab) => {
					const posA: number = a.ordinalPosition || 0;
					const posB: number = b.ordinalPosition || 0;
					return posA < posB ? -1 : 1;
				});
				return [defaultTab, ...ret];
			},
		},
		methods: {
			...mapActions({
				showNewSnackbar: "showNewSnackbar",
			}),
			...mapActions("apiHandler", {
				execSPCall: "execSPCall",
				getQuestionnaire: "getQuestionnaire",
				updateQuestionnaireQuestions: "updateQuestionnaireQuestions",
			}),
			/**
			 * Update all Questionnaire-Tab expansion pannels either to enxpand or collapse all.
			 */
			updateAllPannels(expandAll: boolean = true) {
				const localThis: any = this as any;
				// Collapse all
				if (!expandAll) {
					localThis.expandedTabPanels = [];
				} else {
					const tabCount: number = localThis.currentTabList.length + 1;
					localThis.expandedTabPanels = Array.from(
						new Array(tabCount),
						(x, i) => i
					);
				}
			},
			/**
			 * Truncate string and add ellipsis is string is longer than required.
			 * @param {string} str String to format.
			 * @param {number} len Max length required for the string.
			 * @returns Formatted string.
			 */
			trucateString(str: string, len: number) {
				len = len || 128;
				if (str.length <= len) {
					return str;
				}
				return str.substring(0, len) + "...";
			},
			/**
			 * Get a list of fields based on a Questionnaire-Tab
			 */
			getFieldsByTab(tabId: number) {
				const localThis: any = this as any;
				const ret: ImsQuestionnaireQuestion[] =
					localThis.currentQuestionnaireQuestionList
						.filter((qq: ImsQuestionnaireQuestion) => qq.tabId == tabId)
						.sort(
							(
								a: ImsQuestionnaireQuestion,
								b: ImsQuestionnaireQuestion
							) => {
								const posA: number = a.ordinalPosition || 0;
								const posB: number = b.ordinalPosition || 0;
								return posA < posB ? -1 : 1;
							}
						);
				return ret;
			},
			// Save new version of the snapshot based on the current questionnaire configuration.
			saveSnapshot() {
				const localThis: any = this as any;
				localThis.qqListSnapshot = (
					localThis.questionnaire?.questionnaireQuestionList || []
				).map((qq: ImsQuestionnaireQuestion) => {
					if (!qq.tabId) {
						qq.tabId = 0;
					}
					return JSON.stringify(qq);
				});
			},
			/**
			 * Get Questionnaire structure.
			 * Set "questionnaire" data property.
			 */
			async getQuestionnaireStructure() {
				const localThis: any = this as any;
				const officeId: number = localThis.getCurrentOffice.HR_OFFICE_ID;
				try {
					const tmpQuest: ImsQuestionnaire =
						await localThis.getQuestionnaire({
							officeId: officeId,
							questionnaireId: localThis.questionnaireId,
						});
					localThis.questionnaire = tmpQuest;

					// Setup initial snapshot of Questionnaire-Quesiton list
					localThis.saveSnapshot();
				} catch (error) {
					// Feedback to user
					localThis.showNewSnackbar({
						typeName: "error",
						text: error,
					});
				}
			},
			/**
			 * Update the Questionnaire-Question configuration.
			 */
			async updateQuestionnaireQuestion() {
				const localThis: any = this as any;
				const questionnaireId: number = localThis.questionnaire.id;
				// Disable save
				localThis.disableSave = true;

				// Get updated list
				const qqListToUpdate: ImsQuestionnaireQuestion[] = (
					localThis.questionnaire?.questionnaireQuestionList || []
				)
					.filter(
						(qq: ImsQuestionnaireQuestion) =>
							!localThis.qqListSnapshot.includes(JSON.stringify(qq))
					)
					.map((qq: ImsQuestionnaireQuestion) => {
						// Manage null Tab assign
						if (qq.tabId == 0) {
							qq.tabId = null;
						}
						return qq;
					});

				// If there are no changes, don't send data.
				if (qqListToUpdate.length == 0) {
					localThis.disableSave = false;
					// Feedback to user
					localThis.showNewSnackbar({
						typeName: "info",
						text: localThis.$t(
							"views.view-questionnaire-config.no-changes"
						),
					});
					return;
				}

				// Update Questionnaire-Questions
				localThis
					.updateQuestionnaireQuestions({
						questionnaireId: questionnaireId,
						qqList: qqListToUpdate,
					})
					.then(() => {
						// Setup initial snapshot of Questionnaire-Quesiton list
						localThis.saveSnapshot();
						// Feedback to user
						localThis.showNewSnackbar({
							typeName: "success",
							text: localThis.$t(
								"views.view-questionnaire-config.succesful-questionnaire-update"
							),
						});
					})
					.catch((error: any) => {
						// Feedback to user
						localThis.showNewSnackbar({
							typeName: "error",
							text: error,
						});
					})
					.finally(() => {
						localThis.disableSave = false;
					});
			},
			/**
			 * Move question to a new index in the list.
			 * @param {number} fromIndex Index of the starting position of the question.
			 * @param {number} toIndex Index of the arrival position of the question.
			 */
			moveToIndex(toIndex: number, qestQuestionId: number) {
				const localThis: any = this as any;
				localThis.questionnaire.questionnaireQuestionList =
					localThis.currentQuestionnaireQuestionList.map(
						(qq: ImsQuestionnaireQuestion) => {
							if (qq.id == qestQuestionId) {
								qq.ordinalPosition = toIndex;
							}
							return qq;
						}
					);
			},
			/**
			 * Debounce order changing.
			 */
			getDebouncedOrderChange(func: Function, wait: number) {
				const localThis: any = this as any;
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
					localThis.questionOrderTimeout = setTimeout(later, wait);
				};
			},
		},
		mounted() {
			const localThis: any = this as any;
			localThis.getQuestionnaireStructure();
		},
	});
</script>

<style scoped>
</style>