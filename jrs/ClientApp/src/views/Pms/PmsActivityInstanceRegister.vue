<template>
    <v-container>
        <v-content fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <!-- PAGE INFO -->
                    <v-row>
                        <v-col :cols="12">
                            <h1>{{$t('views.view-activity-instance.title')}}</h1>
                            <p>{{$t('views.view-activity-instance.view-info')}}</p>
                        </v-col>
                    </v-row>
                    <!-- MAIN FILTER -->
                    <v-row>
                        <!-- PROJECT SELECT -->
                        <v-col :cols="6">
                            <v-autocomplete
                                v-model="project"
                                :label="$t('views.view-activity-instance.project-select')"
                                :items="projectList"
                                item-value="PROJECT_ID"
                                item-text="PROJECT_DESCR"
                                return-object
                            ></v-autocomplete>
                        </v-col>
                        <!-- ACTIVITY SELECT -->
                        <v-col :cols="6">
                            <v-autocomplete
                                v-model="activity"
                                :label="$t('views.view-activity-instance.activity-select')"
                                :disabled="!project.PROJECT_ID"
                                :items="activityList.filter((item) => item.PROJECT_ID == project.PROJECT_ID)"
                                item-value="ACTIVITY_ID"
                                item-text="ACTIVITY_DESCR"
                                return-object
                            ></v-autocomplete>
                        </v-col>
                    </v-row>
                    <v-divider></v-divider>

                    <!-- DATA ENTRY -->
                    <v-row v-if="computedactInstanceFormFields && Object.keys(activity).length > 0">
                        <v-col :cols="12">
                            <jrs-form
                                :formData="actInstance"
                                :formFields="computedactInstanceFormFields"
                                :key="'ACTIVITY_'+activity.ACTIVITY_ID"
                            >
                                <template v-slot:form-actions="{ validateFunc, resetFunc }">
                                    <v-btn
                                        color="primary"
                                        :disabled="isSaving"
                                        @click="saveFormData(actInstance, actInstance.ID ? 1 : 0, validateFunc)"
                                        class="ma-2"
                                    >{{ $t('general.save') }}</v-btn>
                                    <v-btn
                                        color="primary"
                                        @click="resetFunc()"
                                        class="ma-2"
                                    >{{ $t('general.reset') }}</v-btn>
                                </template>
                            </jrs-form>
                        </v-col>
                    </v-row>
                </v-col>
            </v-row>
        </v-content>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { mapGetters, mapActions } from 'vuex';
    import JrsForm from '../../components/JrsForm.vue';
    import {
        GenericSqlSelectPayload,
        GenericSqlPayload,
        SqlActionType,
    } from "../../axiosapi/api";
    import FormMixin from "../../mixins/FormMixin";
    import { JrsFormField, JrsFormFieldSearch } from "../../models/JrsForm"

    export default Vue.extend({
        components:{
            "jrs-form": JrsForm
        },
        mixins: [FormMixin],
        data(){
            return {
                projectList: [],
                project: {},
                activityList: [],
                activity: {},
                actInstance_crud_info: {},
                actInstanceFormFields: [],
                actInstance: {},
                distribution: {},
                isSaving:false
            }
        },
        computed:{
            ...mapGetters({
                getCurrentOffice: 'getCurrentOffice'
            }),
            computedActInstance(){
                let localThis:any = this as any;
                return Object.assign({}, localThis.actInstance);
            },
            computedactInstanceFormFields(){
                let localThis:any = this as any;

                return localThis.actInstanceFormFields.filter( (field:any) => {
                    let val:boolean = false;
                    switch (field.tabCode) {
                        case 'CASE_VISIT':
                        case 'VISIT_NOTE':
                            val = localThis.activity.IS_WELFARE ? true : false;
                            break;
                        case 'DISTR':
                            val = localThis.activity.IS_DISTRIBUTION ? true : false;
                            break;
                        case 'EDU_ASSESS':
                        case 'EVALUATION':
                            val = localThis.activity.IS_EDUCATION ? true : false;
                            break;
                        default:
                            val = true;
                            break;
                    }
                    return val;
                });
            }
        },
        watch:{
            activity:{
                deep:true,
                handler(newVal:any){
                    let localThis:any = this as any;
                    
                    // Clear activity Instance
                    localThis.clearActivityInstance();

                    // Setup information for save payload
                    localThis.actInstance.ACTIVITY_ID = newVal.ACTIVITY_ID;
                    localThis.actInstance.IS_DISTRIBUTION = newVal.IS_DISTRIBUTION;
                    localThis.actInstance.IS_EDUCATION = newVal.IS_EDUCATION;
                    localThis.actInstance.IS_WELFARE = newVal.IS_WELFARE;
                }
            },
            /**
             * Watch changes to the activity Object.
             * Watching the properties of the object and finding the differences resulted complicated tue to some limitations on the vue API (From https://vuejs.org/v2/api/#vm-watch : "Note: when mutating (rather than replacing) an Object or an Array, the old value will be the same as new value because they reference the same Object/Array. Vue doesn’t keep a copy of the pre-mutate value.")
             * To solve the problem this solution was implemented: https://github.com/vuejs/vue/issues/2164#issuecomment-432872718
             * 
             */
            computedActInstance:{
                deep: true,
                handler(to:any, from:any){
                    let localThis:any = this as any;

                    // Manage change in Case Visit Household
                    if(to.CASE_VISIT_HOUSEHOLD_UID != from.CASE_VISIT_HOUSEHOLD_UID && localThis.actInstanceFormFields){
                        if(!to.CASE_VISIT_HOUSEHOLD_UID){
                            // Clear datasource condition
                            localThis.actInstanceFormFields
                                .find( (field:JrsFormField) => field.name == 'CASE_VISIT_BENEFICIARY_USER_UID')
                                .dataSourceCondition = null;
                        }else{
                            localThis.actInstanceFormFields
                                .find( (field:JrsFormField) => field.name == 'CASE_VISIT_BENEFICIARY_USER_UID')
                                .dataSourceCondition = `HOUSEHOLD_UID = '${to.CASE_VISIT_HOUSEHOLD_UID}'`;
                        }
                    }

                    // Manage change in Distribution
                    if(to.DISTRIBUTION_HOUSEHOLD_UID != from.DISTRIBUTION_HOUSEHOLD_UID && localThis.actInstanceFormFields){
                        if(!to.DISTRIBUTION_HOUSEHOLD_UID){
                            // Clear datasource condition
                            localThis.actInstanceFormFields
                                .find( (field:JrsFormField) => field.name == 'DISTRIBUTION_BENEFICIARY_USER_UID')
                                .dataSourceCondition = null;
                        }else{
                            localThis.actInstanceFormFields
                                .find( (field:JrsFormField) => field.name == 'DISTRIBUTION_BENEFICIARY_USER_UID')
                                .dataSourceCondition = `HOUSEHOLD_UID = '${to.DISTRIBUTION_HOUSEHOLD_UID}'`;
                        }
                    }
                }
            }
        },
        methods:{
            ...mapActions({
                showNewSnackbar: "showNewSnackbar",
            }),
            ...mapActions("apiHandler", {
                getGenericSelect: "getGenericSelect",
                execSPCall: "execSPCall",
                getJRSTableStructure: "getJRSTableStructure",
            }),
            /**
             * Return a list of active projects for the provided user and office.
             */
            getProjectsActivities(officeId:number){
                let localThis:any = this as any;
                let selectPayload:GenericSqlSelectPayload = {
                    viewName: "PMS_VI_OFFICE_PROJECT_ACTIVITIES",
                    officeId: officeId
                }

                return localThis
                    .getGenericSelect({ genericSqlPayload: selectPayload })
                    .then((res:any) => {
                        if(res.item_count > 0){
                            // Setup projectList
                            let projList = res.table_data.map( (row:any) => {
                                return {
                                    PROJECT_ID: row.PROJECT_ID,
                                    PROJECT_CODE: row.PROJECT_CODE,
                                    PROJECT_DESCR: row.PROJECT_DESCR
                                };
                            });
                            // Setup activityList
                            let actList = res.table_data.map( (row:any) => {
                                return {
                                    PROJECT_ID: row.PROJECT_ID,
                                    ACTIVITY_ID: row.ACTIVITY_ID,
                                    ACTIVITY_NAME: row.ACTIVITY_NAME,
                                    ACTIVITY_DESCR: row.ACTIVITY_DESCR,
                                    IS_EDUCATION: row.IS_EDUCATION,
                                    IS_DISTRIBUTION: row.IS_DISTRIBUTION,
                                    IS_WELFARE: row.IS_WELFARE
                                };
                            });
    
                            localThis.projectList = [...projList];
                            localThis.activityList = [...actList];
                        }
                    })
            },
            /**
             * Get structure of form.
             */
            getFormStruct(viewName:string, formStructPropName:string, crudInfoPropName:string) {
            let localThis: any = this as any;
            let selectPayload: GenericSqlSelectPayload = {
                viewName: viewName,
                colList: null,
                whereCond: null,
                orderStmt: null,
            };

            localThis
                .getJRSTableStructure({ genericSqlPayload: selectPayload })
                .then((res: any) => {
                // Setup CRUD info
                Object.assign(localThis[crudInfoPropName], res.crud_info);

                // Setup Form Fields
                let tmpFormFileds: Array<any> = res.columns
                    .filter((header: any) => !header.hide_in_form)
                    .map((field: any) => localThis.parseJrsFormField(field));
                localThis[formStructPropName] = localThis.setupSelectFields(tmpFormFileds);
                });
            },
            /**
             * Save form data.
             */
            saveFormData(
                saveData: any,
                actionType: SqlActionType,
                formValidateFunc: Function,
                formResetFunc?: Function
            ){
                // Check for validity
                if(!formValidateFunc()){
                    return null;
                }

                let localThis:any = this as any;
                localThis.isSaving = true;
                let spName:string = 'PMS_SP_INS_UPD_ACTIVITY_INSTANCE_EXTENDED';

                let savePayload: GenericSqlPayload = {
                    spName: spName,
                    actionType: actionType,
                    jsonPayload: JSON.stringify(saveData),
                };

                localThis.execSPCall(savePayload)
                    .then( (res:any) => {
                        localThis.showNewSnackbar({
                            type: res.status,
                            text: res.message
                        });

                        if(formResetFunc){
                            formResetFunc();
                        }

                        //If "new" then set the data
                        if (actionType == SqlActionType.NUMBER_0 && res.pk_id) {
                            // Activity instance id
                            localThis.actInstance.ID = res.pk_id;

                            // Case visit id
                            localThis.actInstance.CASE_VISIT_ID = res.case_visit_id;
                        }
                    })
                    .catch((err: any) => {
                        localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
                    }).finally(()=>{
                        localThis.isSaving = false;
                    });
            },
            /**
             * Clear all the properties of the activity instance
             */
            clearActivityInstance(){
                let localThis:any = this as any;

                //Clear old activity instance
                for (var propName in localThis.actInstance){
                    if (localThis.actInstance.hasOwnProperty(propName)){
                        delete localThis.actInstance[propName];
                    }
                }
            }

        },
        mounted(){
            let localThis:any = this as any;
            localThis.getProjectsActivities(localThis.getCurrentOffice.HR_OFFICE_ID);

            localThis.getFormStruct('PMS_VI_ACTIVITY_INSTANCE_FORM', 'actInstanceFormFields', 'actInstance_crud_info');
        }
    })
</script>

<style scoped>

</style>