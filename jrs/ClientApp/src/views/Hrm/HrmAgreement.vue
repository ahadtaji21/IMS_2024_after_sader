<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col>
                    <jrs-simple-table
                        viewName="HR_VI_AGREEMENT"
                        :nbrOfFormColumns="2"
                    >
                        <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">
                            <v-icon 
                                v-if="detailFields && simpleItemRowItem.AMMENDMENT_HISTORY"
                                small
                                @click="openDialog(simpleItemRowItem)"
                            >mdi-history</v-icon>
                            <!-- <v-icon 
                                small
                                @click="downloadDocument(simpleItemRowItem)"
                            >mdi-file-download-outline</v-icon> -->
                        </template>
                    </jrs-simple-table>
                    
                    <!-- DETAIL DIALOG -->
                    <v-dialog
                        v-model="dialog"
                        persistent
                        retain-focus
                        :scrollable="true"
                        :overlay="false"
                        :max-width="'60em'"
                        transition="dialog-transition"
                    >
                        <v-card>
                            <v-card-title>
                                <span class="capital-first-letter">{{$t('general.detail')}}</span>
                            </v-card-title>
                            <v-card-text>
                                <v-row>
                                    <v-col :cols="3" style="border-right:solid 1px rgba(0, 0, 0, 0.12)">
                                        <v-list>
                                            <v-subheader class="text-uppercase">{{$t('datasource.hrm.agreement.select-version')}}</v-subheader>
                                            <v-divider></v-divider>
                                            <v-list-item-group
                                                v-model="currentVersion"
                                                color="primary"
                                            >
                                                <v-list-item
                                                    v-for="(version, vIndex) in versions"
                                                    :key="vIndex"
                                                >
                                                    <v-list-item-content
                                                        @click="detailData = version.agreement"
                                                    >
                                                        Version nbr. {{version.version_nbr}}
                                                    </v-list-item-content>
                                                </v-list-item>
                                            </v-list-item-group>
                                        </v-list>
                                    </v-col>
                                    <v-col :cols="9">
                                        <!-- <h1 class="capital-first-letter">{{$t('general.detail')}}</h1> -->
                                        <jrs-readonly-detail
                                            v-if="detailFields && currentVersion != null && versions[currentVersion]"
                                            :detailFields="detailFields"
                                            :detailData="versions[currentVersion].agreement"
                                        ></jrs-readonly-detail>
                                    </v-col>
                                </v-row>
                            </v-card-text>
                            <v-card-actions>
                                <v-btn color="secondary darken-1" class="mt-2 mr-1" text @click="dialog=false">X {{ $t('general.close') }}</v-btn>
                            </v-card-actions>
                        </v-card>
                    </v-dialog>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from 'vue'
    import { mapActions, mapGetters } from 'vuex';
    import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
    import JrsReadonlyDetail from "../../components/JrsReadonyDetail.vue";
    import FormMixin from "../../mixins/FormMixin";
    import UtilMix from "../../mixins/UtilMix";
    import { JrsFormField, JrsFormFieldSelect, JrsFormFieldSearch } from "../../models/JrsForm";
    import {
        GenericSqlSelectPayload,
        GenericSqlPayload,
        SqlActionType
    } from "../../axiosapi/api";

    export default Vue.extend({
        components:{
            "jrs-simple-table":JrsSimpleTable,
            "jrs-readonly-detail":JrsReadonlyDetail
        },
        data(){
            return {
                dialog: false,
                detailData: {},
                detailFields: {},
                versions:[],
                currentVersion: 0
            }
        },
        mixins: [FormMixin, UtilMix],
        computed:{
            ...mapGetters({
                getCurrentOffice: 'getCurrentOffice'
            }),
        },
        methods:{
            ...mapActions({
                showNewSnackbar: "showNewSnackbar"
            }),
            ...mapActions("apiHandler", {
                getGenericSelect: "getGenericSelect",
                execSPCall: "execSPCall",
                getJRSTableStructure: "getJRSTableStructure",
                downloadDocxFromTemplate: "downloadDocxFromTemplate",
                getTemplate:"getTemplate"
            }),
            openDialog(rowData:any){
                let localThis:any = this as any;
                // localThis.detailData = rowData;
                localThis.versions = JSON.parse(rowData.AMMENDMENT_HISTORY);
                localThis.dialog = true;
            },
            getFormStruct(){
                let localThis: any = this as any;
                let selectPayload: GenericSqlSelectPayload = {
                    viewName: "HR_VI_AGREEMENT",
                    colList: null,
                    whereCond: null,
                    orderStmt: null
                };
                return localThis
                    .getJRSTableStructure({ genericSqlPayload: selectPayload })
            },
            /**
             * Download Word document with agreement info.
             * @param rowItem Agreement information to download document for
             */
            async downloadDocument(rowItem:any){
                const localThis: any = this as any;

                localThis.showNewSnackbar({
                    typeName: "info",
                    text: localThis.$t("info-message.preparing-document").toString(),
                });
                const fileName:string = "AGREEMENT.docx";
                const agreementId:number = rowItem.HR_AGREEMENT_ID;
                const queryCondition:string = `HR_AGREEMENT_ID = ${agreementId}`;

                let templateId:number = 0;
                let templates:Array<any> = [];
                try {
                    // Get valid template list
                    templates = await localThis.getTemplate({
                        officeId: localThis.getCurrentOffice.HR_OFFICE_ID
                    })
                    .then((templateList:any) => {
                        if(templateList.length == 0){
                            throw new Error(localThis.$t("error.no-valid-template").toString());
                        }
                        return templateList;
                    })
                    // Get template id
                    templateId = templates[0].templateId;
                } catch (err) {
                    localThis.showNewSnackbar({ typeName: "error", text: err, });
                    return;
                }

                // Download document
                const docBites:any[] = await localThis.downloadDocxFromTemplate({templateId, queryCondition});
                const byteBuffer:any = localThis._base64ToArrayBuffer(docBites);
                localThis.saveByteArray(fileName, byteBuffer);
            },
        },
        mounted(){
            let localThis:any = this as any;
            localThis.getFormStruct()
                .then( (res:any) => {
                    let tmpFormFileds:Array<any> = res.columns
                        .filter((header: any) => !header.hide_in_form)
                        .map((field:any) => localThis.parseJrsFormField(field));
                    localThis.detailFields = localThis.setupSelectFields(tmpFormFileds, localThis.getCurrentOffice.HR_OFFICE_ID, localThis.fieldDatasourceConditions);
                    return tmpFormFileds;
                })
                .then( (res:any) => {
                    localThis.detailFields = res;
                });
        }
    })
</script>

<style scoped>

</style>