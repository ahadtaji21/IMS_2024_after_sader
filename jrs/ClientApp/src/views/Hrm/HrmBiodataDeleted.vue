<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                 
                            <jrs-simple-table
                                :viewName="'HR_VI_BIODATA_DELETED'"
                                :savePayload="{userUid:getUserUid, officeId:getCurrentOffice.HR_OFFICE_ID}"
                                :hideActions="true"
                            >
                                <template v-slot:simple-table-item-actions="{ simpleItemRowItem,refreshData }">
                                    <v-icon 
                                         class="mx-2"
                                        v-if="!simpleItemRowItem.IMS_USER_IS_ANONYMIZED"
                                        @click="openOpenDialogActivation(simpleItemRowItem,refreshData)"
                                    >mdi-account-check</v-icon>
                                    <v-icon 
                                         class="mx-2"
                                        v-if="!simpleItemRowItem.IMS_USER_IS_ANONYMIZED"
                                        @click="openOpenDialogAnonymization(simpleItemRowItem,refreshData)"
                                    >mdi-account-key</v-icon>
                                </template>
                        </jrs-simple-table>
               
                       
                </v-col>
            </v-row>
             <!--- ANONYMIZATION DIALOG -->
                <v-dialog
                v-model="openDialogAnonymization"
                persistent
                retain-focus
                :overlay="false"
                max-width="45em"
                transition="dialog-transition"
                >
                <v-card>
                    <v-card-title primary-title class="text-capitalize">{{ $t('general.crud.user-anonymization') }}</v-card-title>
                        <v-alert
                                outlined
                                type="warning"
                                color="secondary"
                                prominent
                                border="left"
                                class="ml-3 mr-3"
                                >
                               {{ $t('general.crud.user-anonymization-attention') }}
                                </v-alert>
                    <v-card-text class="capital-first-letter">{{ $t('general.crud.user-anonymization-confirm') }}
                         <v-checkbox :label="$t('general.confirm')" v-model="check"></v-checkbox>
                    </v-card-text>
                     
                         
                    <v-card-actions>
                    <v-btn
                        text
                        color="secondary darken-1"
                       
                        @click="openOpenDialogAnonymization([]); check = false"
                    >X {{ $t('general.close') }}</v-btn>
                    <v-btn  :disabled="!check" color="primary" @click="AnonymizationUser()">{{ $t('general.anonymize') }}</v-btn>
                    </v-card-actions>
                </v-card>
                </v-dialog>
                <!--- ACTIVATE DIALOG -->
                <v-dialog
                v-model="openDialogActivate"
                persistent
                retain-focus
                :overlay="false"
                max-width="45em"
                transition="dialog-transition"
                >
                <v-card>
                    <v-card-title primary-title class="text-capitalize">{{ $t('general.crud.activate-user') }}</v-card-title>
                    <v-card-text class="capital-first-letter">{{ $t('general.crud.activate-user-confirm') }}</v-card-text>
                    <v-card-actions>
                    <v-btn
                        text
                        color="secondary darken-1"
                        @click="openOpenDialogActivation([])"
                    >X {{ $t('general.close') }}</v-btn>
                    <v-btn color="primary" @click="ActivateUser()">{{ $t('general.active') }}</v-btn>
                    </v-card-actions>
                </v-card>
                </v-dialog>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { mapGetters , mapActions} from 'vuex';
    import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
    import JrsSimpleTable from '../../components/JrsSimpleTable.vue';

    export default Vue.extend({
        components:{
            'jrs-simple-table': JrsSimpleTable
        },
        data(){
            return {
                tabModel: null,
                check: false,
                tables: [
                    {name: 'HR_VI_BIODATA_DELETED', code:"TYPE", tab_key:"datasource.hrm.biodata.deleted.title"},
                ],
                selectedUser: [],
                openDialogAnonymization: false,
                openDialogActivate : false,
                refreshData: Function
            }
        },
        methods:{
                ...mapActions({
                    showNewSnackbar: "showNewSnackbar"
                }),
                ...mapActions("apiHandler", {
                    getGenericSelect: "getGenericSelect",
                    execSPCall: "execSPCall"
                }),
                openOpenDialogAnonymization(userData:any, functionRefresh:any){
                     let localThis:any = this as any;
           
                    localThis.selectedUser = userData;
                    localThis.openDialogAnonymization = !localThis.openDialogAnonymization;
                    localThis.refreshData = functionRefresh;
                },
                openOpenDialogActivation(userData:any, functionRefresh:any){
                     let localThis:any = this as any;
           
                    localThis.selectedUser = userData;
                    localThis.openDialogActivate = !localThis.openDialogActivate;
                    localThis.refreshData = functionRefresh;
                },
                AnonymizationUser(){
                        let localThis:any = this as any;
                        localThis.isSaving=true;
                        let savePayload: GenericSqlPayload = {
                            spName: 'HR_SP_INS_UPD_BIODATA_ANONYMIZATION',
                            jsonPayload: JSON.stringify(localThis.selectedUser),
                            userUid: localThis.getUserUid,
                            officeId: localThis.getCurrentOffice.HR_OFFICE_ID
                            };

                        localThis.execSPCall(savePayload)
                            .then( (res:any) => {
                            localThis.showNewSnackbar({
                                typeName: "success",
                                text: res.message
                            });
                            localThis.openDialogAnonymization = !localThis.openDialogAnonymization;
                            localThis.refreshData();
                            })
                            .catch((err: any) => {
                            localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
                            }).finally(()=>{
                            localThis.isSaving=false;
                            localThis.check = false;
                            });
                },
                 ActivateUser(){
                        let localThis:any = this as any;
                        localThis.isSaving=true;
                        let savePayload: GenericSqlPayload = {
                            spName: 'HR_SP_INS_UPD_BIODATA_DELETE',
                            jsonPayload: JSON.stringify(localThis.selectedUser),
                            userUid: localThis.getUserUid,
                            officeId: localThis.getCurrentOffice.HR_OFFICE_ID
                            };

                        localThis.execSPCall(savePayload)
                            .then( (res:any) => {
                            localThis.showNewSnackbar({
                                typeName: "success",
                                text: res.message
                            });
                            localThis.openDialogActivate = !localThis.openDialogActivate;
                            localThis.refreshData();
                            })
                            .catch((err: any) => {
                            localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
                            }).finally(()=>{
                            localThis.isSaving=false;
                            });
                }
                
        },
        computed: {
            ...mapGetters({
            getUserUid: 'getUserUid',
            getCurrentOffice: 'getCurrentOffice'
            })
        }
    })
</script>

<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
</style>