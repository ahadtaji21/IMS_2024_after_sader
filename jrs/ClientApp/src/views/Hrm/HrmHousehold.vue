<template>
  <v-content>
      <v-container fluid fill-height>
          <v-row>
              <v-col :cols="12">
                <!-- TABLE -->
                <jrs-simple-table
                    v-model="selectedItems"
                    :viewName="viewName"
                    :selectable="false"
                    :hideNewBtn="true"
                    :hideActions="true"
                    v-if="selectedItems.length == 0"
                >
                    <!-- TABLE HEADER -->
                    <template v-slot:simple-table-header>
                        <v-btn
                          color="secondary lighten-2"
                          class="grey--text text--darken-3"
                          @click="selectedItems.push({})"
                        >
                          <v-icon>mdi-plus-circle-outline</v-icon>
                          {{$t('general.new')}}
                        </v-btn>
                    </template>

                    <!-- ROW ACTIONS -->
                    <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">
                        <v-icon @click="selectRow(simpleItemRowItem)">mdi-circle-edit-outline</v-icon>
                    </template>
                </jrs-simple-table>

                <!-- FORM -->
                <div
                    v-if="selectedItems.length > 0"
                    class="elevation-1"
                >
                    <v-toolbar color="primary darken-1" dark>
                        <v-spacer></v-spacer>
                        <v-btn color="secondary" class="ma-2" @click="closeDetail()">
                          <v-icon>mdi-table</v-icon>
                          {{ $t('datasource.hrm.household.btn-to-list') }}
                        </v-btn>

                        <template v-slot:extension>
                          <v-tabs v-model="activeTab"
                            centered
                            background-color="primary darken-1"
                            dark
                          >
                            <v-tab key="BASIC">{{ $t('datasource.hrm.household.basic-info') }}</v-tab>
                            <v-tab v-if="selectedItems.length > 0 && selectedItems[0].HR_HOUSEHOLD_UID" key="DOCUMENT">{{$t('datasource.hrm.document.title') }}</v-tab>
                            <v-tab
                              key="COMPONENTS"
                              v-if="selectedItems.length > 0 && selectedItems[0].HR_HOUSEHOLD_UID"
                            >{{ $t('datasource.hrm.household.components') }}</v-tab>
                            <v-tab
                              key="CASE-VISITS"
                              v-if="selectedItems.length > 0 && selectedItems[0].HR_HOUSEHOLD_UID"
                            >{{ $t('datasource.pms.activity-instance.tab-case-visit') }}</v-tab>
                          </v-tabs>
                        </template>
                    </v-toolbar>
                    <v-tabs-items
                      v-model="activeTab"
                      class="tab-item-wrapper"
                    >
                      <!-- BASIC INFO -->
                      <v-tab-item key="BASIC">
                        <jrs-form
                          :formData="formData"
                          :formFields="formFields"
                          :nbrOfColumns="2"
                        >
                          <template v-slot:form-actions="{ validateFunc, resetFunc }">
                            <v-btn
                              color="primary"
                              :disabled="isSaving"
                              @click="saveFormData(formData, formData.PRIMARY_KEY ? 1 : 0, validateFunc)"
                              class="ma-2"
                            >{{ $t('general.save') }}</v-btn>
                            <v-btn
                              color="primary"
                              @click="resetFunc()"
                              class="ma-2"
                            >{{ $t('general.reset') }}</v-btn>
                          </template>
                        </jrs-form>
                      </v-tab-item>

                      <!-- ID DOCUMENT -->
                      <!-- fieldDatasourceConditions = condition that aims to filter the select of Document Type based on the class of Household -->
                        <v-tab-item key="DOCUMENT">
                          <jrs-simple-table
                            viewName="HR_VI_DOCUMENT_BY_TYPE"
                            :dataSourceCondition="docCondition"
                            :savePayload="{HR_HOUSEHOLD_ID: selectedItems[0].HR_HOUSEHOLD_UID, userUid:getUserUid, officeId:getCurrentOffice.HR_OFFICE_ID}"
                            v-if="selectedItems.length > 0 && selectedItems[0].HR_HOUSEHOLD_UID"
                            :fieldDatasourceConditions="[{field_name:'HR_DOCUMENT_TYPE_ID',field_condition:`HR_DOCUMENT_CLASS_DESCR = '${docHouseholdTag}'`}]"
                            ref="table_docs"
                          ></jrs-simple-table>
                        </v-tab-item>
                      <!-- HOUSEHOLD COMPONENTS -->
                      <v-tab-item key="COMPONENTS">
                        <jrs-simple-table
                          viewName="HR_VI_HOUSEHOLD_MEMBERS"
                          columnList="ID,USER_UID,FROM_DATE,TO_DATE,HR_BIODATA_FULL_NAME"
                          :dataSourceCondition="householdFilterCondition"
                          :savePayload="{HR_HOUSEHOLD_UID: selectedItems[0].HR_HOUSEHOLD_UID}"
                          v-if="selectedItems.length > 0 && selectedItems[0].HR_HOUSEHOLD_UID"
                          ref="table_members"
                        ></jrs-simple-table>
                      </v-tab-item>

                      <!-- CASE VISIT -->
                      <v-tab-item key="CASE-VISITS">
                        <jrs-simple-table
                          viewName="HR_VI_HOUSEHOLD_CASE_VISIT"
                          :dataSourceCondition="householdFilterCondition"
                          :savePayload="{HR_HOUSEHOLD_UID: selectedItems[0].HR_HOUSEHOLD_UID}"
                          ::nbrOfFormColumns="5"
                          v-if="selectedItems.length > 0 && selectedItems[0].HR_HOUSEHOLD_UID"
                          ref="table_case_visits"
                        
                        >
                      <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">
          
                        <!-- BTN - OPEN BIODATA READONLY POPUP -->
                        <v-tooltip top>
                          <template v-slot:activator="{ on, attrs }">
                            <v-btn
                              icon
                              @click="showDetail(simpleItemRowItem);"
                              v-bind="attrs"
                              v-on="on"
                            >
                              <v-icon>mdi-eye-outline</v-icon>
                            </v-btn>
                          </template>
                          <span>{{$t('datasource.pms.case-visit.notes')}}</span>
                        </v-tooltip>

                      </template>
                     </jrs-simple-table>
                  </v-tab-item>
                </v-tabs-items>
            </div>
  
        <!--DETAILS-NOTES-CASE-VISIT-->
                     
          <v-dialog
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
                    <h1 class="capital-first-letter">{{$t('datasource.pms.case-visit.notes')}}</h1>
                    <v-textarea
                    name="input-7-1"
                    auto-grow
                    v-html="detailData"
                    append-icon="mdi-comment"
                  ></v-textarea>
                
                  </v-col>
                </v-row>
                
              </v-card-text>
              <v-card-actions>
                <v-btn
                  color="secondary darken-1"
                  class="mt-2 mr-1"
                  text
                  @click="detailDialog=false"
                >X {{ $t('general.close') }}</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>


              </v-col>
          </v-row>
      </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import JrsForm from "../../components/JrsForm.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import FormMixin from "../../mixins/FormMixin";

export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "jrs-form": JrsForm,
  },
  mixins: [FormMixin],
  data(){
      return {
          viewName: 'HR_VI_HOUSEHOLD',
          selectedItems: [],
          activeTab: '',
          formFields: [],
          formData: {},
          crud_info: {},
          householdFilterCondition: null,
          docCondition: null,
          detailData: {},
          detailDialog: false,
          isSaving:false,
          docHouseholdTag:"HOUSEHOLD"
          
      };
  },

  computed:{
    ...mapGetters({
      getUserUid: 'getUserUid',
      getCurrentOffice: 'getCurrentOffice'
    })

    
  
  },
  watch: {
      selectedItems(to:Array<any>, from:Array<any>){
          let localThis: any = this as any;

          if(to.length > 0){
            // Set form data
            localThis.formData = to[0];
  
            // Set Household and Interview components filter
            localThis.householdFilterCondition = `HOUSEHOLD_UID = '${ to[0].HR_HOUSEHOLD_UID ? to[0].HR_HOUSEHOLD_UID : 0 }'`;
            localThis.docCondition =  `HR_HOUSEHOLD_ID = '${to[0].HR_HOUSEHOLD_UID ? to[0].HR_HOUSEHOLD_UID : 0}'`;
          }
      }
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
      getJRSTableStructure: "getJRSTableStructure",
    }),
    /**
     * Select biodata row
     */
    selectRow(row: any) {
      (this as any).selectedItems.push(row);
    },
    /**
     * Close Detail section and open list
     */
    closeDetail() {
      let localThis: any = this as any;
      while (localThis.selectedItems.length > 0) {
        localThis.selectedItems.pop();
      }
    },
    /**
     * Get structure of the Biodata Form
     */
    getFormStruct() {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: localThis.viewName,
        colList: null,
        whereCond: null,
        orderStmt: null,
      };
      // localThis.formFields = localThis
        localThis.getJRSTableStructure({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          // Setup CRUD info
          localThis.crud_info = res.crud_info;

          // Setup Form Fields
          let tmpFormFileds: Array<any> = res.columns
            .filter((header: any) => !header.hide_in_form)
            .map((field: any) => localThis.parseJrsFormField(field));
          localThis.formFields = localThis.setupSelectFields(tmpFormFileds);
        });
    },
    /**
     * Open detail dialog with the selected data.
     * @param data The data to dispaly in the detail dialog
     */
    showDetail(data:any){
      let localThis:any = this as any;
      localThis.detailData = '<div>'+data.NOTES+ '</div>';
  
      localThis.detailDialog = true;
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
      // Check form validity
      if(!formValidateFunc()){
        return null;
      }

      let localThis:any = this as any;
      localThis.isSaving=true;
      let spName:string = actionType == SqlActionType.NUMBER_0 
        ? localThis.crud_info.create_sp
        : localThis.crud_info.update;

      let savePayload: GenericSqlPayload = {
          spName: spName,
          actionType: actionType,
          jsonPayload: JSON.stringify(saveData),
          userUid: localThis.getUserUid,
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID
        };

      localThis
        .execSPCall(savePayload)
        .then( (res:any) => {
          localThis.showNewSnackbar({
            typeName: "succes",
            text: res.message
          });

          if(formResetFunc){
            formResetFunc();
          }

          //If "new" then load the data
          if (actionType == SqlActionType.NUMBER_0 && res.pk_id) {
            localThis.formData.HR_HOUSEHOLD_UID = res.pk_id;
            // localThis.formData.HR_BIODATA_USER_UID = res.HR_BIODATA_USER_UID;
            localThis.selectedItems.push(localThis.formData);
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        }).finally(()=>{
          localThis.isSaving=false;
        });
      
    },

    // getComponents(householdUid:string){

    // }
  },
  mounted(){
    let localThis:any = this as any;
    localThis.getFormStruct();
  }
});
</script>

<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
</style>