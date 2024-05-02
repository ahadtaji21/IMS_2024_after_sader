<template>
  <v-content>
    <v-container fluid fill-height>
      <h1>Placement Test</h1>
      <v-row>
        <v-col :cols="12">
          <v-card>
            <v-tabs @change="forceRerender(),forceRerenderPlacementTest()" v-model="tabIsVisible"  background-color="primary darken-1" dark>
              <v-tab v-for="item in tabsItems" :key="item.code" >{{ item.descr }} </v-tab>
            </v-tabs>
            <v-tabs-items   v-model="tabIsVisible" style="padding: 0.5em">
                <v-tab-item key="NEW_PLACEMENT_TEST"  >
                  <plc-form-test
                    :action="'adding'"
                    :typeAssignment= "typeAssignment"
                    :key="componentPlacementTestForm" 
                  >
                    
                  </plc-form-test>
                  </v-tab-item>
                  <v-tab-item  key="PLACEMENT_TESTS">
                     
                  <jrs-simple-table
                    viewName="PMS_VI_PLACEMENT_TEST"
                
                    :hideNewBtn="true"
                    :hideActions="true"
                    :clientPagination="true"
                    :showSearchField="true"
                    :key= "componentKeyTables"
                    
                  >
                  
                  <!-- ROW ACTIONS -->
                  <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">
                    <v-icon @click="selectPlacementTestItem(simpleItemRowItem),forceRerenderPlacementTest();">mdi-circle-edit-outline</v-icon>
                    <v-icon @click="DelPlacementTest(simpleItemRowItem); forceRerenderPlacementTest();">mdi-delete</v-icon>
                    <v-icon @click="ViewQuestionTemplate(simpleItemRowItem); forceRerenderPlacementTest();">mdi-text-box-search-outline</v-icon>
                  </template>
                  <!-- TABLE HEADER -->
                  </jrs-simple-table>
                   
          
          
                  </v-tab-item>
                
                 
      
            </v-tabs-items>
          </v-card>
        </v-col>
      </v-row>
        <jrs-question-template
               :testId="placement_test_id"
               viewName="PMS_VI_QUESTION_ANSWER_PLACEMENT_TEST"
               columnCond="PLACEMENT_TEST_ID"
               :noHidedialog="activation_questions_template"
               :key="cont"
               :ForEvaluation="false"
          >
         </jrs-question-template>
    
      <v-dialog
            v-model="activation_update_placement_test"
            max-width="60em"
            transition="dialog-transition"
            persistent
            retain-focus
            :scrollable="true"
           
        >
        
    <v-card>
         <v-card-title><v-row><v-col>Placement Test</v-col> <v-col  class="d-flex flex-row-reverse"  > <v-btn
                        color="secondary darken-1"
                        text
                        @click="activation_update_placement_test = false"
                    >X {{ $t('general.close') }}</v-btn></v-col></v-row></v-card-title>
         
                <v-card-text>

                  <plc-form-test
                    :placement_test_id="placement_test_id"
                    :action="action"
                    :key="componentPlacementTestForm" 
                    :typeAssignment= "typeAssignment"
                    :forceRerender="forceRerender"
                >
                </plc-form-test>
               
          </v-card-text>
    </v-card>
    </v-dialog>

 
    </v-container>
  </v-content>


</template>

<script lang="ts">
// @ is an alias to /src
// import HelloWorld from "@/components/HelloWorld.vue";
import axios from "axios";
import Vue from "vue";
import { mapActions } from "vuex";

import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import JrsModalForm from "../../components/JrsModalForm.vue";
import JrsLocationPicker from "../../components/JrsLocationPicker.vue";
import JrsSearchTable from "../../components/JrsSearchTable.vue";
import JrsDatePicker from "../../components/JrsDatePicker.vue";
import JrsInputNbr from "../../components/JrsInputNbr.vue";
import PlacementTest from "../../components/PMS/LEARNING/PlacementExamEvaluationForm.vue";


import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../axiosapi/api";
//TMP
import { JrsHeader } from "../../models/JrsTable";
import { i18n } from "../../i18n";
import {
  JrsFormField,
  JrsFormFieldText,
  JrsFormFieldSelect,
} from "../../models/JrsForm";
import JrsQuestionTemplate from "../../components/PMS/LEARNING/QuestionTemplate.vue"
//TMP
interface payLoadDataPlacementTest {
  PLACEMENT_TEST_ID: number | 0;
  EXAM_NAME: String | null;
  PLACEMENT_INTERVAL_ID: number | null;
  QUESTION_ID: number | null;
  ACTIVITY_ID :number | null;
}

interface payLoadDataExam {
  EXAM_ID: number | 0;
  EXAM_NAME: String | null;
  PASSING_SCORE :number | null;
  QUESTION_ID: number | null;
}
export default {
  name: "home",
  components: {
       "plc-form-test": PlacementTest,
       "jrs-simple-table":JrsSimpleTable,
       "jrs-question-template": JrsQuestionTemplate
     
  },
 

  data() {
    return {
      serviceRes: null,
      tabIsVisible: null,
      tabsItems: [
        { code: "NEW_PLACEMENT_TEST", descr: "New Placement Test" },
        { code: "PLACEMENT_TESTS", descr: "Placement Test" }
      
      ],
      genericQueryData: false,
      tmpSelectedRows: [],
      placement_test_id:  0,
      cont: 0,
      placement_test: {},
      activation_questions_template: false,
      componentExamForm: 0,
      componentPlacementTestForm: 0,
      typeAssignment: "PlacementTest",
      action: "adding",
      activation_update_placement_test: false,
      activation_update_exam: false,
      value_test: {},
      componentKeyTables: 0
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),

    DelPlacementTest(item: any){
       
      let localThis = this as any;
      let payLoadD = {} as payLoadDataPlacementTest;
      let check_ins_one=1;
    
      let msgDel: string = localThis.$i18n
        .t("datasource.pms.learning.placement-test.confirm-delete-placement-test")
        .toString();
      
     
     localThis.$confirm(msgDel).then((res:any) => {
     if (res) {
          
          payLoadD.PLACEMENT_TEST_ID=item.PLACEMENT_TEST_ID;
          payLoadD.EXAM_NAME="";
          payLoadD.ACTIVITY_ID = 0;
          payLoadD.PLACEMENT_INTERVAL_ID = 0;
          let saveData = {
                    check_one_time : check_ins_one,
                    question_id: 0,
                    rows : payLoadD
                  };
           
          let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_DEL_PLACEMENT_TEST",
                actionType: SqlActionType.NUMBER_2,
                jsonPayload: JSON.stringify(saveData)
              };
              localThis
                .execSPCall(savePayload)
                .then((res: any) => {
                  localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
                  localThis.forceRerender();
                }).catch((err: any) => {
             
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message
                   
                  }); // Feedback to user
                });
              
          }
       });

    },

    selectPlacementTestItem(item: any) {
      
      let localThis: any = this as any;
      localThis.placement_test = Object.assign({}, localThis.placement_test, item);        
      localThis.placement_test_id = item.PLACEMENT_TEST_ID
      localThis.action = "update"
      localThis.forceRerenderPlacementTest();
      localThis.activation_update_placement_test = true;
      
      
    },

    ViewQuestionTemplate(item: any){
      let localThis: any = this as any;      
      localThis.placement_test_id = item.PLACEMENT_TEST_ID
      localThis.cont ++;
      localThis.activation_questions_template = true;
    },
    
    forceRerender() {
      let localThis : any = this as any;
      localThis.componentKeyTables += 1;  
    },

    forceRerenderPlacementTest(){
       let localThis : any = this as any;
      localThis.componentPlacementTestForm ++;  
    }
  },
};
</script>
