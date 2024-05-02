<template>
	<v-card>
		<v-card-title>
			<h2>
				{{ $t("datasource.sfip.activity-schedule.schedule-editor") }}
			</h2>
			<v-spacer></v-spacer>
			<v-btn color="primary" class="mx-1" @click="addNewScheduleItem">
				<v-icon>mdi-plus-circle-outline</v-icon>
				{{ $t("general.new") }} </v-btn
			><v-btn color="primary" class="mx-1" @click="saveAllScheduleItems">
				<v-icon>mdi-content-save</v-icon> {{ $t("general.save") }}
			</v-btn>
		</v-card-title>
		<v-card-text>
			<!-- SAMPLE -->

			<v-list dense class="schedule-item-list">
				<v-list-item
					v-for="(item, itemIndex) in scheduleItems"
					:key="itemIndex"
					class="schedule-item"
				>
					<v-list-item-icon>
						<v-icon color="primary" small v-if="item.wasEdited">
							mdi-brightness-1
						</v-icon>
					</v-list-item-icon>

					<v-list-item-content>
						<v-list-item-title
							:class="{
								'item-delited': item.scheduleData.isDeleted,
								'warning--text text--darken-2':
									item.scheduleData.isDeleted,
							}"
							v-if="!item.isInEdit"
							>{{ item.scheduleData.scheduleYear }} -
							{{
								item.scheduleData.scheduleQuarterLookup
									.imsLookupValue
							}}
						</v-list-item-title>
						<div
							class="d-flex flex-row justify-start"
							v-if="item.isInEdit"
						>
							<jrs-field
								:field="scheduleFormFields[0]"
								v-model="
									item.scheduleData[
										scheduleFormFields[0].name
									]
								"
								class="mx-1"
								@input="onFieldInput(item)"
							></jrs-field>
							<jrs-field
								:field="scheduleFormFields[1]"
								v-model="
									item.scheduleData[
										scheduleFormFields[1].name
									]
								"
								class="mx-1"
								@input="onFieldInput(item)"
								@changeFullObject="onLookupChange(item, $event)"
							></jrs-field>
						</div>
					</v-list-item-content>

					<v-list-item-icon>
						<v-tooltip
							top
							v-if="!item.isInEdit"
							:key="`edit-action-${itemIndex}`"
						>
							<template v-slot:activator="{ on }">
								<v-btn
									color="primary"
									icon
									plain
									v-on="on"
									@click="openEditItem(item)"
								>
									<v-icon large
										>mdi-circle-edit-outline</v-icon
									>
								</v-btn>
							</template>
							<span>{{
								$t(
									"views.sfip-activity-definition.btn-edit-schedule"
								)
							}}</span>
						</v-tooltip>
						<v-tooltip top v-if="item.isInEdit">
							<template v-slot:activator="{ on }">
								<v-btn
									color="primary"
									icon
									plain
									v-on="on"
									@click="closeEditItem(item)"
								>
									<v-icon large
										>mdi-check-circle-outline</v-icon
									>
								</v-btn>
							</template>
							<span>{{ $t("general.confirm") }}</span>
						</v-tooltip>
						<v-tooltip
							top
							v-if="
								(!item.isInEdit &&
									!item.scheduleData.isDeleted) ||
								(item.scheduleData.id == 0 &&
									!item.scheduleData.isDeleted)
							"
							:key="`delete-action-${itemIndex}`"
						>
							<template v-slot:activator="{ on }">
								<v-btn
									color="primary"
									icon
									plain
									v-on="on"
									@click="deleteItem(item, itemIndex)"
								>
									<v-icon large
										>mdi-delete-circle-outline</v-icon
									>
								</v-btn>
							</template>
							<span>{{ $t("general.delete") }}</span>
						</v-tooltip>
						<v-tooltip
							top
							v-if="!item.isInEdit && item.scheduleData.isDeleted"
							:key="`restore-action-${itemIndex}`"
						>
							<template v-slot:activator="{ on }">
								<v-btn
									color="primary"
									icon
									plain
									v-on="on"
									@click="restoreItem(item)"
								>
									<v-icon large>mdi-delete-restore</v-icon>
								</v-btn>
							</template>
							<span>{{ $t("general.restore") }}</span>
						</v-tooltip>
					</v-list-item-icon>
				</v-list-item>
			</v-list>
		</v-card-text>
	</v-card>
</template>

<script lang="ts">
	import Vue from "vue";
	import { mapActions, mapGetters } from "vuex";
	import {
		GenericSqlSelectPayload,
		ImsLookup,
		SfipActivitySchedule,
	} from "../../axiosapi";
	import { JrsFormField } from "../../models/JrsForm";
	import FormMixin from "../../mixins/FormMixin";
	import JrsField from "../JrsField.vue";

	export interface ScheduleItem {
		scheduleData: SfipActivitySchedule;
		isInEdit: boolean;
		wasEdited: boolean;
	}

	export default Vue.extend({
		components: {
			"jrs-field": JrsField,
		},
		props: {
			activityId: {
				type: Number,
				required: true,
			},
		},
		mixins: [FormMixin],
		data: () => ({
			scheduleItems: [] as ScheduleItem[],
			scheduleFormFields: [] as JrsFormField[],
			scheduleFormData: {} as SfipActivitySchedule,
			showScheduleForm: false,
		}),
		watch: {
			async activityId(newActivityId: number, oldActivityId: number) {
				console.log("activity change");
				if (!newActivityId || newActivityId === oldActivityId) {
					return;
				}
				this.loadActivityScheduleList();
			},
		},
		computed: {
			...mapGetters(["getLookupsByTypeCode"]),
			quarters(): ImsLookup[] {
				return this.getLookupsByTypeCode("QUARTER");
			},
		},
		methods: {
			...mapActions("apiHandler", {
				getActivitySchedule: "getActivitySchedule",
				updateActivityScheduleList: "updateActivityScheduleList",
			}),
			...mapActions({
				showNewSnackbar: "showNewSnackbar",
			}),
			async loadActivityScheduleList() {
				try {
					const tmpSchedules = await this.getActivitySchedule(
						this.activityId
					);
					this.scheduleItems = tmpSchedules.map(
						(schedule: SfipActivitySchedule) => ({
							scheduleData: schedule,
							isInEdit: false,
							wasEdited: false,
						})
					);
				} catch (error) {
					this.showNewSnackbar({ typeName: "error", text: error });
				}
			},
			async saveAllScheduleItems() {
				// Parse schedule items
				let itemsToSave = (this.scheduleItems as ScheduleItem[])
					.filter((item: ScheduleItem) => item.wasEdited)
					.map((item: ScheduleItem) => ({ ...item.scheduleData }));

				if (itemsToSave.length > 0) {
					try {
						// save all items
						await this.updateActivityScheduleList({
							activityId: this.activityId,
							scheduleList: itemsToSave,
						});
					} catch (error) {
						this.showNewSnackbar({ typeName: "error", text: error });
					}
				}

				this.loadActivityScheduleList();
			},
			addNewScheduleItem() {
				const defaultQuarter: ImsLookup = this.quarters[0];
				const newSchedule: SfipActivitySchedule = {
					id: 0,
					activityId: this.activityId,
					scheduleYear: undefined,
					scheduleQuarterLookupId: undefined,
					isDeleted: false,
					activity: undefined,
					scheduleQuarterLookup: defaultQuarter,
				};
				this.scheduleItems.push({
					scheduleData: newSchedule,
					isInEdit: true,
					wasEdited: false,
				});
			},
			openEditItem(item: ScheduleItem) {
				item.isInEdit = true;
			},
			closeEditItem(item: ScheduleItem) {
				item.isInEdit = false;
			},
			deleteItem(item: ScheduleItem, itemIndex: number) {
				const isNewEntry = item.scheduleData.id === 0;
				if (isNewEntry) {
					let tmpItems: ScheduleItem[] = [...this.scheduleItems];
					tmpItems.splice(itemIndex, 1);
					this.scheduleItems = [...tmpItems];
				} else {
					item.scheduleData.isDeleted = true;
					item.wasEdited = true;
				}
			},
			restoreItem(item: ScheduleItem) {
				item.scheduleData.isDeleted = false;
				item.wasEdited = true;
			},
			onFieldInput(scheduleItem: ScheduleItem) {
				scheduleItem.wasEdited = true;
			},
			onLookupChange(
				scheduleItem: ScheduleItem,
				eventPayload: { holderName: string; objectValue: any }
			) {
				const lookupData: ImsLookup = eventPayload.objectValue;
				scheduleItem.scheduleData.scheduleQuarterLookup = lookupData;
			},
		},
		async mounted() {
			if (this.activityId) {
				this.loadActivityScheduleList();
			}
			// this.setupScheduleForm();
			let tmpFormFileds: Array<any> = [
				(this as any).parseJrsFormField({
					table_name: "SFIP_VI_ACTIVITY_SCHEDULE",
					column_name: "scheduleYear",
					js_type: "Numeric",
					type: "date",
					required: false,
					translationtKey: "datasource.sfip.activity-schedule.year",
					readonly: false,
					list_order: 4,
					form_order: 4,
					additional_config: '{"OnlyYear": true}',
				}),
				(this as any).parseJrsFormField({
					table_name: "SFIP_VI_ACTIVITY_SCHEDULE",
					column_name: "scheduleQuarterLookupId",
					js_type: "Numeric",
					type: "select",
					required: true,
					translationtKey:
						"datasource.sfip.activity-schedule.quarter-code",
					is_hidden: true,
					select_lookup_code: "QUARTER",
					readonly: false,
					list_order: 5,
					form_order: 5,
					additional_config: "{}",
				}),
				(this as any).parseJrsFormField({
					table_name: "SFIP_VI_ACTIVITY_SCHEDULE",
					column_name: "isDeleted",
					js_type: "Boolean",
					type: "checkbox",
					required: true,
					is_hidden: true,
					hide_in_form: true,
					readonly: false,
					list_order: 7,
					form_order: 7,
					additional_config: "{}",
					text: "Delete Entry",
				}),
			];
			// Setup selectFields
			this.scheduleFormFields = (this as any).setupSelectFields(
				tmpFormFileds
			);
		},
	});
</script>

<style scoped>
	.schedule-item-list {
		min-width: 50em;
	}
	.schedule-item {
		border-bottom: solid 1px rgba(0, 0, 0, 0.12);
		padding: 0.5em;
	}
	.item-delited {
		text-decoration: line-through;
	}
</style>