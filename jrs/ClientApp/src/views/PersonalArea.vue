<template>
    <v-content>
        <v-container fluid fill-height v-if="userUid">
            <v-row align-center>
                <!-- PERSONAL DATA -->
                <v-col justify-center :cols="12" :lg="6">
                    <v-card>
                        <v-card-title>{{$t('views.view-personal-area.personal-data')}}</v-card-title>
                        <v-card-text>
                            <hrm-biodata-form
                                :userUid="userUid"
                                :readonly="true"
                            ></hrm-biodata-form>
                        </v-card-text>
                    </v-card>
                </v-col>
                <v-col justify-center :cols="12" :lg="6">
                    <!-- TIMESHEET DATA -->
                    <jrs-simple-table
                        viewName="HR_VI_TIMESHEET"
                        :nbrOfFormColumns="3"
                        :selectable="false"
                        :dataSourceCondition="`HR_BIODATA_USER_UID = '${userUid}'`"
                    ></jrs-simple-table>
                </v-col>
            </v-row>
            <v-row align-center>
                <v-col justify-center :cols="12">
                    <!-- POSITION DATA -->
                    <jrs-simple-table
                        viewName="HR_VI_POSITION_HR"
                        :dataSourceCondition="`EXISTS(SELECT 1 FROM HR_AGREEMENT WHERE HR_AGREEMENT_ID = HR_POSITION_AGREEMENT_ID AND IMS_USER_UID = '${userUid}')`"
                        :columnList="positionColList"
                        :hiddenColumns="positionHiddenCols"
                        :title="$t('views.view-personal-area.position-hisotry')"
                        :readonly="true"
                    >
                    </jrs-simple-table>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { mapGetters } from 'vuex';
    import JrsSimpleTable from '../components/JrsSimpleTable.vue';
    import HrmBiodataForm from '../components/HRM/HrmBiodataForm.vue';

    export default Vue.extend({
        components:{
            'jrs-simple-table': JrsSimpleTable,
            'hrm-biodata-form': HrmBiodataForm,
        },
        data(){
            return{
                userUid: null,
                biodataActiveTab: null,
                showpositionDetails: false,
                selectedPosition: null,
                positionColList: ['HR_POSITION_ID','HR_POSITION_SCOPE_ID','HR_POSITION_FUNCTION_ID','HR_POSITION_LEVEL_ID','HR_POSITION_FULL_TITLE_NATIVE','HR_POSITION_NATIVE_LANGUAGE_ID','HR_POSITION_STATUS_ID','HR_POSITION_DEPARTMENT_ID','HR_POSITION_PROJECT_ID','HR_POSITION_AGREEMENT_TYPE_ID','HR_POSITION_AGREEMENT_ID','HR_POSITION_LOCATION_ID','HR_POSITION_SUPERIOR_POSITION_ID','HR_POSITION_RELATION_TYPE_LOOKUP_ID','IMS_STATUS_DESCR','HR_DEPARTMENT_DESCR','PROJECT_CODE','HR_POSITION_DESCR','HR_POSITION_FULL_TITLE','SUPERIOR_POSIITON_DESCR','HR_OFFICE_CODE','HR_EMPLOYEMENT_DATE_FROM','HR_EMPLOYEMENT_DATE_TO'
                ],
                positionHiddenCols:['ACTIVITY_ID','HR_TITLE_SCOPE_DESCR','HR_TITLE_FUNCTION_DESCR','HR_TITLE_LEVEL_DESCR','HR_BIODATA_FULL_NAME','IMS_STATUS_DESCR','SUPERIOR_POSIITON_DESCR','HR_POSITION_BUDGET_AMOUNT'],
                HR_BIODATA_ID: 1,
            }
        },
        methods:{
            ...mapGetters(["getUserUid"]),
            setupPositionReadonly(positionId:any){
                this.showpositionDetails = true;
                this.selectedPosition = positionId;
            },
        },
        mounted(){
            // this.userUid = 'e48060f3-9ad4-4a84-a6f9-6c3dc6df5ef2';
            this.userUid = this.getUserUid();
        }
    })
</script>

<style scoped>

</style>