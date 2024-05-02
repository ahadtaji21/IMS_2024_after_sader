<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                            <jrs-simple-table
                                :viewName="'HR_VI_TIMESHEET_SUPERVISOR'"
                                :selectable="false"
                                :dataSourceCondition="`HR_TIMESHEET_USER_VALIDATE_UID = '${getUserUid}'`"   
                                :hideNewBtn="true"  
                                :nbrOfFormColumns="1"
                                :overrideFormMaxWidth="'75em'"
                                
                            >         
                          <!--  <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">       
                            <v-icon 
                                small
                                @click="showDetail(simpleItemRowItem)"
                            >mdi-eye-outline</v-icon>
                        </template>-->
                        </jrs-simple-table>
                </v-col>
            </v-row>
                <!-- DETAILS DIALOG -->

       <!-- DETAILS DIALOG -->
       <!--   <v-dialog
            v-model="detailDialog"
            persistent
            retain-focus
            :scrollable="true"
            :overlay="false"
            :max-width="'60em'"
            transition="dialog-transition"
          >
            <v-card>
              <v-card-text>
                <v-row>
                  <v-col :cols="12">
                    <h1 class="capital-first-letter">{{$t('general.detail')}}</h1>
                    <jrs-readonly-detail
                      v-if="formFieldsTimesheet"
                      :detailFields="formFieldsTimesheet"
                      :detailData="detailData"
                    ></jrs-readonly-detail>
                  </v-col>
                </v-row>
              </v-card-text>
              <v-card-actions>
                <v-btn
                  color="secondary darken-1"
                  class="mt-2 mr-1"
                  text
                  @click="detailDialog=false"
                >X {{ $t('general.cancel') }}</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>-->
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { mapGetters,mapActions } from 'vuex';
    import JrsSimpleTable from '../../../components/JrsSimpleTable.vue';
    import JrsReadonyDetail from "../../../components/JrsReadonyDetail.vue";
    import FormMixin from "../../../mixins/FormMixin";
    /*import {
        GenericSqlSelectPayload,
        GenericSqlPayload,
        SqlActionType,
        } from "../../../axiosapi/api";*/

    export default Vue.extend({
        components:{
            'jrs-simple-table': JrsSimpleTable,
           // "jrs-readonly-detail": JrsReadonyDetail,
        },
        mixins: [FormMixin],
        data(){
            return {
                formFieldsTimesheet: {},
             //   detailData: {},
               // detailDialog:false
            }
        },
        methods:{
              ...mapActions("apiHandler", {
                getGenericSelect: "getGenericSelect",
                execSPCall: "execSPCall",
                getJRSTableStructure: "getJRSTableStructure",
                // getFormStruct: "getFormStruct"
                }),
              /*  getFormStruct(view: string) {
                    let localThis: any = this as any;
                    let selectPayload: GenericSqlSelectPayload = {
                        viewName: view,
                        colList: null,
                        whereCond: null,
                        orderStmt: null,
                    };
                      
                    localThis
                        .getJRSTableStructure({ genericSqlPayload: selectPayload })
                        .then((res: any) => {
                        // Setup CRUD info
                      //  localThis.biodata_CRUD = res.crud_info;

                        // Setup Form Fields
                        let tmpFormFileds: Array<any> = res.columns
                            .filter((header: any) => header.hide_in_form && header.readonly)
                            .map((field: any) => localThis.parseJrsFormField(field));
                        localThis.formFieldsTimesheet = localThis.setupSelectFields(tmpFormFileds);
                        });
                  
                  },
                  showDetail(data:any){

                        let localThis:any = this as any;
                        let data_copy = Object.assign({},data);
                        localThis.detailData = data_copy;
                        localThis.detailDialog = true;
                        },*/      
                },

               /* mounted(){
                        let localThis:any = this as any;
                        localThis.getFormStruct("HR_VI_TIMESHEET_SUPERVISOR");
                },*/
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