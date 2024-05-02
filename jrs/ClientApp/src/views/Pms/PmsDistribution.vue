<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col>
                    <!-- PAGE INFO -->
                        <v-row>
                            <v-col :cols="12">
                                <h1>
                                    {{ $t("views.view-distribution.title") }}
                                </h1>
                                <p>
                                    {{
                                        $t("views.view-distribution.view-info")
                                    }}
                                </p>
                            </v-col>
                        </v-row>
                        <div class="d-flex align-center">
                            <v-tooltip top>
                                <template v-slot:activator="{ on }">
                                    <v-btn
                                        color="secondary lighten-2"
                                        class="grey--text text--darken-3 mr-2"
                                        @click="selectBreadcrumb(selectedBreadcrumIndex-1)"
                                        :disabled="selectedBreadcrumIndex==0"
                                        v-on="on"
                                        small
                                    >
                                        <v-icon small left>mdi-arrow-collapse-left</v-icon>
                                        <span>{{$t("views.view-distribution.btn-return")}}</span>
                                    </v-btn>
                                </template>
                                <span>{{$t("views.view-distribution.btn-return-extend")}}</span>
                            </v-tooltip>
                            <v-chip-group v-model="selectedBreadcrumIndex" active-class="primary--text text--accent-4">
                                <template v-for="(navigationItem,navItemIndex) in breadCrumbitems">
                                    <span v-if="navItemIndex>0" :key="'divider_'+navItemIndex" class="align-self-center">/</span>
                                    <v-chip link small :key="navigationItem.itemType" class="mx-1" @click="selectBreadcrumb(navItemIndex)">
                                        <template v-if="navigationItem.itemType == 'instanceList'">{{ $t("views.view-distribution.process-instance-list") }}</template>
                                        <template v-if="navigationItem.itemType == 'processInstance'">{{ $t("views.view-distribution.process-instance") }}: {{ navigationItem.item.TITLE }}</template>
                                        <template v-if="navigationItem.itemType == 'distribution'">{{ $t("views.view-distribution.distribution") }}: {{ navigationItem.item.ITEM_DESCR }}</template>
                                    </v-chip>
                                </template>
                            </v-chip-group>
                        </div>



                    <!-- PROCESS INSTANCE SELECTION -->
                    <template v-if="selectedProcessInstances.length==0">
                        

                        <!-- MAIN FILTER -->
                        <v-card outlined class="lv-0" dense>
                            <v-card-title>Filters</v-card-title>
                            <v-card-text>
                                <v-row justify="center">
                                    <v-col :md="4" :cols="12">
                                        <jrs-field
                                            :field="processFilterField"
                                            v-model="processFilter"
                                            v-if="processFilterField"
                                        ></jrs-field>
                                    </v-col>
                                    <v-col :md="4" :cols="12">
                                        <jrs-field
                                            :field="dateFromFilterField"
                                            v-model="dateFromFilter"
                                            v-if="dateFromFilterField"
                                        ></jrs-field>
                                    </v-col>
                                    <v-col :md="4" :cols="12">
                                        <jrs-field
                                            :field="dateToFilterField"
                                            v-model="dateToFilter"
                                            v-if="dateToFilterField"
                                        ></jrs-field>
                                    </v-col>
                                </v-row>
                            </v-card-text>
                        </v-card>


                        <!-- PROCESS INSTANCE TABLE -->
                        <v-row>
                            <v-col>
                                <jrs-simple-table
                                    v-model="selectedProcessInstances"
                                    viewName="PMS_VI_PROCESS_INSTANCE_FOR_USER"
                                    :dataSourceCondition="processInstanceCondition"
                                    selectable
                                    hideActions
                                ></jrs-simple-table>
                            </v-col>
                        </v-row>
                    </template>
                    <!-- DISTRIBUTION LIST AND SELECTION -->
                    <template v-if="selectedProcessInstances.length!=0 && selectedDistribution.length==0">
                        <!-- DISTRIBUTION DEFINITION -->
                        <jrs-simple-table
                            v-model="selectedDistribution"
                            viewName="PMS_VI_DISTRIBUTION"
                            :title="distributionTableTitle"
                            :dataSourceCondition="distributionCondition"
                            :nbrOfFormColumns="3"
                            :selectable="true"
                            :savePayload="{processInstance: selectedProcessInstances[0].PROCESS_INST_ID}"
                        >
                            <template v-slot:simple-table-item-actions="{simpleItemRowItem}">
                                <v-icon 
                                    small
                                    @click="downloadReceipt(simpleItemRowItem)"
                                >mdi-file-download-outline</v-icon>
                            </template>
                        </jrs-simple-table>
                    </template>

                    <!-- DISTRIBUTION DETAIL LIST -->
                    <template v-if="selectedDistribution.length!=0">
                        <jrs-simple-table
                            viewName="PMS_VI_DISTRIBUTION_DETAIL"
                            :title="distributionDetailTableTitle"
                            :dataSourceCondition="distributionDetailCondition"
                            :savePayload="{distributionId: selectedDistribution[0].ID}"
                            :nbrOfFormColumns="1"
                        ></jrs-simple-table>
                    </template>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from "vue";
    import { mapGetters, mapActions } from "vuex";
    import {
        GenericSqlSelectPayload,
        GenericSqlPayload,
        SqlActionType,
    } from "../../axiosapi/api";
    import { JrsFormFieldSelect, JrsFormField } from "../../models/JrsForm";
    import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
    import JrsField from "../../components/JrsField.vue";
    import FormMixin from "../../mixins/FormMixin";
    import UtilMix from "../../mixins/UtilMix";

    export default Vue.extend({
        components: {
            "jrs-simple-table": JrsSimpleTable,
            "jrs-field": JrsField,
        },
        mixins: [FormMixin,UtilMix],
        data: () => ({
            selectedProcessInstances: [],
            selectedDistribution: [],
            processList: [],
            processFilterField: null,
            processFilter: null,
            dateFromFilterField: null,
            dateFromFilter: null,
            dateToFilterField: null,
            dateToFilter: null,
            distributionReceiptTemplateTypeCode: 'DISTRIBUTION_RECEIPT',
            availableTemplates:[],
            selectedBreadcrumIndex: null,
        }),
        computed: {
            ...mapGetters({
                getUserUid: "getUserUid",
                getCurrentOffice: "getCurrentOffice",
            }),
            processInstanceCondition() {
                const localThis: any = this as any;
                let conditions: string[] = [];
                // Add base conditions
                conditions.push(`JRS_EMPLOYEE='${localThis.getUserUid}'`);
                conditions.push("IS_DISTRIBUTION=1");

                // Add filter conditions
                if (localThis.processFilter) {
                    conditions.push(`PROCESS_ID = ${localThis.processFilter}`);
                }
                if (localThis.dateFromFilter) {
                    conditions.push(
                        `CAST(ACTIVITY_DATE as DATE) >= '${
                            localThis.dateFromFilter.toISOString().split("T")[0]
                        }'`
                    );
                }
                if (localThis.dateToFilter) {
                    conditions.push(
                        `CAST(ACTIVITY_DATE as DATE) <= '${
                            localThis.dateToFilter.toISOString().split("T")[0]
                        }'`
                    );
                }
                
                return conditions.join(" AND ");
            },
            distributionCondition(){
                const localThis: any = this as any;
                if(localThis.selectedProcessInstances.length==0){
                    return null;
                }
                const {PROCESS_INST_ID} = localThis.selectedProcessInstances[0];
                let conditions: string[] = [];
                conditions.push(`PROCESS_INSTANCE_ID=${PROCESS_INST_ID}`);
                return conditions.join(" AND ");
            },
            distributionTableTitle(){
                const localThis: any = this as any;
                if(localThis.selectedProcessInstances.length==0){
                    return null;
                }
                const {ANNUAL_PLAN_CODE, TITLE, ACTIVITY_DATE} = localThis.selectedProcessInstances[0];
                return `${TITLE} (${ANNUAL_PLAN_CODE}) - ${localThis.$d(new Date(ACTIVITY_DATE))}`
            },
            distributionDetailCondition(){
                const localThis: any = this as any;
                if(localThis.selectedDistribution.length==0 || localThis.selectedProcessInstances==0){
                    return null;
                }
                const distributionId:number = localThis.selectedDistribution[0].ID;
                let conditions: string[] = [];
                conditions.push(`DISTRIBUTION_ID=${distributionId}`);
                return conditions.join(" AND ");
            },
            distributionDetailTableTitle(){
                const localThis: any = this as any;
                if(localThis.selectedDistribution.length==0 || localThis.selectedProcessInstances==0){
                    return null;
                }
                const {ACTIVITY_DATE} = localThis.selectedProcessInstances[0];
                const {ITEM_DESCR} = localThis.selectedDistribution[0];
                return `${ITEM_DESCR} - ${localThis.$d(new Date(ACTIVITY_DATE))}`
            },
            breadCrumbitems(){
                const localThis:any = this as any;
                let items:any[]= [
                    {itemType:'instanceList', item: null},
                    ...localThis.selectedProcessInstances.map((selProcessInst:any) => ({itemType:'processInstance', item:selProcessInst})),
                    ...localThis.selectedDistribution.map((selDis:any) => ({itemType:'distribution', item:selDis}))
                    ];
                return items;
            }
        },
        watch:{
            breadCrumbitems(newBreadcrumItems:any){
                const localThis:any = this as any;
                localThis.selectedBreadcrumIndex = newBreadcrumItems.length -1;
            }
        },
        methods: {
            ...mapActions({
                showNewSnackbar: "showNewSnackbar",
            }),
            ...mapActions("apiHandler", {
                getGenericSelect: "getGenericSelect",
                execSPCall: "execSPCall",
                getTemplate:"getTemplate",
                downloadDocxFromTemplate: "downloadDocxFromTemplate",
            }),
            /**
             * Setup the filter fields.
             */
            async setupFilterFields() {
                const localThis:any = this as any;
                let tmpDateField:any;
                // Setup Process filter fields
                const processFieldConds: string = [
                    `JRS_EMPLOYEE = '${localThis.getUserUid}'`,
                    "IS_DISTRIBUTION=1",
                ].join(" AND ");

                let tmpProcessField = localThis.parseJrsFormField({
                    column_name: "PROCESS_SELECT",
                    type: "select",
                    required: true,
                    select_items_datasource: "PMS_VI_PROCESS_FOR_USER",
                    itemKey: "PROCESS_ID",
                    itemText: "PROCESS_FULL_DESCR",
                    dataSourceCondition: processFieldConds,
                    // list_order: 1,
                    form_order: 1,
                });
                tmpProcessField.label = localThis
                    .$t("datasource.pms.process-instance.process")
                    .toString();
                const tmpSelectFields: any[] = localThis.setupSelectFields(
                    [tmpProcessField],
                    localThis.getCurrentOffice.HR_OFFICE_ID
                );
                localThis.processFilterField = tmpSelectFields[0];

                // Setup From Date filter fields
                tmpDateField = localThis.parseJrsFormField({
                    column_name: "FROM_DATE_FILTER",
                    type: "date",
                    form_order: 2,
                });
                tmpDateField.label = localThis
                    .$t("views.view-distribution.filter-from-date")
                    .toString();
                localThis.dateFromFilterField = { ...tmpDateField };

                
                // Setup To Date filter fields
                tmpDateField = localThis.parseJrsFormField({
                    column_name: "TO_DATE_FILTER",
                    type: "date",
                    form_order: 3,
                });
                tmpDateField.label = localThis
                    .$t("views.view-distribution.filter-to-date")
                    .toString();
                localThis.dateToFilterField = { ...tmpDateField };
            },
            /**
             * Get the available templates for the current office.
             */
            async getOfficeTemplates(){
                const localThis: any = this as any;
                console.log('getting Teamplates');
                try {
                    const templates:any[] = await localThis.getTemplate({
                        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
                        templateTypeCode: localThis.distributionReceiptTemplateTypeCode
                    });
                    // if(templates.length == 0){
                    //     throw new Error(localThis.$t("error.no-valid-template").toString());
                    // }
                    return Promise.resolve(templates);
                } catch (err) {
                    // localThis.showNewSnackbar({ typeName: "error", text: err, });
                    return Promise.reject(err);
                }
            },
            /**
             * Download distribution receipt.
             * @param rowItem distribution to download receipt for.
             */
            async downloadReceipt(rowItem:any){
                const localThis: any = this as any;
                localThis.showNewSnackbar({
                    typeName: "info",
                    text: localThis.$t("info-message.preparing-document").toString(),
                });
                const fileName:string = `receipt_${new Date().toISOString().split(/[-:.]/).join('')}.docx`;
                const distributionId:number = rowItem.ID;
                const queryCondition:string = `DISTRIBUTION_ID = ${distributionId}`;
                let templateId:number = 0;
                try {
                    // Check if available templates
                    if(localThis.availableTemplates.length == 0){
                        throw new Error(localThis.$t("error.no-valid-template").toString());
                    }
                    templateId = localThis.availableTemplates[0].templateId;

                    // Download document
                    const docBytes:any[] = await localThis.downloadDocxFromTemplate({templateId, queryCondition});
                    localThis.downloadFileFromBytes(docBytes, fileName);
                    
                } catch (err) {
                    localThis.showNewSnackbar({ typeName: "error", text: err, });
                }
            },
            selectBreadcrumb(selectedIndex:number){
                const localThis:any = this as any;
                const targetBreadCrumb = localThis.breadCrumbitems[selectedIndex];

                // Set obects based on selected breadcrumb
                if(targetBreadCrumb.itemType=='instanceList'){
                    localThis.selectedDistribution = [];
                    localThis.selectedProcessInstances = [];
                }else if(targetBreadCrumb.itemType=='processInstance'){
                    localThis.selectedDistribution = [];
                }else if(targetBreadCrumb.itemType=='distribution'){
                    console.log('TODO: Implement');
                }

            }
        },
        mounted() {
            const localThis: any = this as any;
            localThis.setupFilterFields();

            // Load available templates
            localThis.getOfficeTemplates()
                .then((templates:any[]) => {
                    localThis.availableTemplates = templates;
                })
                .catch((err:any) => {
                    localThis.showNewSnackbar({ typeName: "error", text: err });
                });
        },
    });
</script>

<style scoped>
</style>