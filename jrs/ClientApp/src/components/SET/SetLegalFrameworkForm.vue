<template>
	<div>
		<v-row>
			<!-- VERSIONS -->
			<v-col :cols="3" v-if="showVersions" class="version-column">
				<v-list>
					<v-subheader class="text-uppercase">{{
						$t("datasource.hrm.agreement.select-version")
					}}</v-subheader>
					<v-divider></v-divider>

					<v-list-item-group
						v-model="currentVersionIndex"
						color="primary"
					>
						<v-list-item
							v-for="version in versionList"
							:key="version.SET_LEGAL_ID"
						>
							<v-list-item-content>
								Version:
								{{
									new Date(
										version.SET_LEGAL_DATE
									).toLocaleString()
								}}
								{{ version.HR_BIODATA_FULL_NAME }}
							</v-list-item-content>
						</v-list-item>
					</v-list-item-group>
				</v-list>
			</v-col>
			<!-- LEGAL FRAMEWORK FORM-->
			<v-col>
                <jrs-form
                    v-if="versionList[currentVersionIndex] && lfFormFields.length > 0 && 1==2"
                    :formData="versionList[currentVersionIndex]"
                    :formFields="lfFormFields"
                    :nbrOfColumns="4"
                ></jrs-form>
                <jrs-readonly-detail
                    v-if="showVersions && versionList[currentVersionIndex] && lfFormFields.length"
                    :detailFields="lfFormFields.filter((ele)=>{
						return ele.name != 'SET_LEGAL_COMMENTS' 
						&& ele.name != 'SET_LEGAL_KIND_ACT_ID' && ele.name != 'SET_LEGAL_KIND_ACT_OTHER'
						&& ele.name != 'SET_LEGAL_DOCUMENT'
					})"
                    :detailData="versionList[currentVersionIndex]"
                    :nbrOfColumns="1"
                ></jrs-readonly-detail>
            </v-col>
		</v-row>
	</div>
</template>

<script lang="ts">
	import Vue from "vue";
	import { mapActions, mapGetters } from "vuex";
	import { GenericSqlSelectPayload } from "../../axiosapi";
	import FormMixin from "../../mixins/FormMixin";
	import JrsForm from "../JrsForm.vue";
    import JrsReadonyDetail from "../JrsReadonyDetail.vue"

	export default Vue.extend({
        components:{
            "jrs-form": JrsForm,
            "jrs-readonly-detail": JrsReadonyDetail,
        },
		props: {
			countryId: {
				type: Number,
				required: false,
				default: 0,
			},
			legalTypeId: {
				type: Number,
				required: false,
				default: 0,
			},
			/**
			 * Show readonly review of previous verisons.
			 * @note If the True, then "countryId" and "legalTypeId" are expected to be true.
			 */
			showVersions: {
				type: Boolean,
				required: false,
				default: false,
			},
		},
		mixins: [FormMixin],
		data: () => ({
			legalTypes: [],
			versionList: [],
			currentVersionIndex: 0,
			lfFormFields: null,
		}),
		methods: {
			...mapActions({
				showNewSnackbar: "showNewSnackbar",
			}),
			...mapActions("apiHandler", {
				getGenericSelect: "getGenericSelect",
			}),
			/**
			 * Get all versions of legal framework for a specific country
			 */
			async getLegalVersions(countryId: number, legalTypeId: number) {
				const localThis: any = this as any;
				const selectPayload: GenericSqlSelectPayload = {
					viewName: "SET_VI_SETTLEMENT_LEGAL_BY_COUNTRY",
					whereCond: `SET_LEGAL_COUNTRY_ID = ${countryId} AND SET_LEGAL_TYPE_ID = ${legalTypeId}`,
					orderStmt: `SET_LEGAL_DATE`,
				};
				try {
					const apiResult: any = await localThis.getGenericSelect({
						genericSqlPayload: selectPayload,
					});
					// Set form structure
					localThis.lfFormFields = apiResult.columns
						.filter((header: any) => !header.hide_in_form)
						.map((field: any) =>
							localThis.setupSelectField(
								localThis.parseJrsFormField(field)
							)
						);
					// Set legal versions
					if (!apiResult.table_data) {
						throw new Error(
							"Error in retrieving previous versions of legal framework"
						);
					}
					return apiResult.table_data;
				} catch (err) {
					localThis.showNewSnackbar({
						typeName: "error",
						text: err.message,
					});
					return [];
				}
			},
		},
		mounted() {
			const localThis: any = this as any;
			// Manage version review
			if (
				localThis.showVersions &&
				localThis.countryId &&
				localThis.legalTypeId
			) {
				localThis
					.getLegalVersions(localThis.countryId, localThis.legalTypeId)
					.then((versLis: Array<any>) => {
						localThis.versionList = [...versLis];
					});
			}
		},
	});
</script>

<style scoped>
	.version-column {
		border-right: solid 1px rgba(0, 0, 0, 0.12);
	}
</style>