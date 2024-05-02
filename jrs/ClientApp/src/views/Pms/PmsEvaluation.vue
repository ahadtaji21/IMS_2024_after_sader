<template>
  <v-content>
    <v-container fluid fill-height>
      <h1>Evaluation</h1>
      <v-row>
        <v-col :cols="12">
          <v-card>
            <v-tabs v-model="tabIsVisible"  @change="forceRerender(),forceRerenderEvaluation()" background-color="primary darken-1" dark>
              <v-tab v-for="item in tabsItems" :key="item.code" >{{ item.descr }} </v-tab>
            </v-tabs>
            <v-tabs-items   v-model="tabIsVisible" style="padding: 0.5em">
                <v-tab-item key="NEW_EVALUATION"  >
                  <plc-form-test
                    :action="'adding'"
                    :typeAssignment= "typeAssignment"
                    :key="componentEvaluationForm" 
                  >
                    
                  </plc-form-test>
                  </v-tab-item>
                  <v-tab-item  key="EVALUATIONS">
                     
                  <jrs-simple-table
                    viewName="PMS_VI_EVALUATION"
                    :hideNewBtn="true"
                    :hideActions="true"
                    :clientPagination="true"
                    :showSearchField="true"
                    :key= "componentKeyTables"
                  
                    
                  >
                  
                  <!-- ROW ACTIONS -->
                  <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">
                    <v-icon @click="selectEvaluationItem(simpleItemRowItem),forceRerenderEvaluation();">mdi-circle-edit-outline</v-icon>
                    <v-icon @click="DelEvaluation(simpleItemRowItem); forceRerenderEvaluation();">mdi-delete</v-icon>
                    <v-icon @click="ViewQuestionTemplate(simpleItemRowItem); forceRerenderEvaluation();">mdi-text-box-search-outline</v-icon>
                  </template>
                  <!-- TABLE HEADER -->
                  </jrs-simple-table>
                   
          
          
                  </v-tab-item>
                
                 

            </v-tabs-items>
          </v-card>
        </v-col>
      </v-row>
       <jrs-question-template
               :testId="evaluation_id"
               viewName="PMS_VI_QUESTION_ANSWER_EVALUATION"
               columnCond="EVALUATION_ID"
               :noHidedialog="activation_questions_template"
               :key="contKeyQuestions"
               :ForEvaluation="false"
          >
         </jrs-question-template>
 
      <v-dialog
            v-model="activation_update_evaluation"
            max-width="60em"
            transition="dialog-transition"
            persistent
            retain-focus
            :scrollable="true"
           
        >
        
    <v-card>
         <v-card-title><v-row><v-col>Evaluation</v-col> <v-col  class="d-flex flex-row-reverse"  > <v-btn
                        color="secondary darken-1"
                        text
                        @click="activation_update_evaluation= false"
                    >X {{ $t('general.close') }}</v-btn></v-col></v-row></v-card-title>
         
                <v-card-text>

                  <plc-form-test
                    :exam_id="evaluation_id"
                    :action="action"
                    :key="componentEvaluationForm" 
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
import FormTest from "../../components/PMS/LEARNING/PlacementExamEvaluationForm.vue";


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
import { debug } from 'webpack';
import JrsQuestionTemplate from "../../components/PMS/LEARNING/QuestionTemplate.vue"
//TMP
interface payLoadDataEvaluation{
  EVALUATION_ID: number | 0;
  EVALUATION_NAME: String | null;
  PASSING_SCORE: number | null;
  QUESTION_ID: number | null;
  ACTIVITY_ID :number | null;
}

export default {
  name: "home",
  components: {
       "plc-form-test": FormTest,
       "jrs-simple-table":JrsSimpleTable,
       "jrs-question-template": JrsQuestionTemplate
     
  },
 

  data() {
    return {
      serviceRes: null,
      tabIsVisible: null,
      tabsItems: [
        { code: "NEW_EVALUATION", descr: "New Evaluation Test" },
        { code: "EVALUATIONS", descr: "Evaluations" }
      
      ],
      genericQueryData: false,
      tmpSelectedRows: [],
      evaluation_id:  0,
      componentExamForm: 0,
      componentEvaluationForm: 0,
      evaluation: {},
      activation_questions_template: false,
      contKeyQuestions:0,
      typeAssignment: "Evaluation",
      action: "adding",
      activation_update_evaluation: false,
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
    

    
    DelEvaluation(item: any){
       
      let localThis = this as any;
      let payLoadD = {} as payLoadDataEvaluation;
      let check_ins_one=1;
    
      let msgDel: string = localThis.$i18n
        .t("datasource.pms.learning.evaluation.confirm-delete-exam")
        .toString();
    
       
       localThis.$confirm(msgDel).then((res:any) => {
       if (res) {
          
          payLoadD.EVALUATION_ID=item.EVALUATION_ID;
          payLoadD.EVALUATION_NAME=item.EVALUATION_NAME;
          payLoadD.PASSING_SCORE = item.PASSING_SCORE;
          let saveData = {
                    check_one_time : check_ins_one,
                    question_id: 0,
                    rows : payLoadD
                  };
           
          let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_DEL_EVALUATION",
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

    selectEvaluationItem(item: any){
        
      let localThis: any = this as any;
      localThis.evaluation = Object.assign({}, localThis.evaluation, item);        
      localThis.evaluation_id = item.EVALUATION_ID
      localThis.action = "update"
      localThis.forceRerenderEvaluation();
      localThis.activation_update_evaluation = !localThis.activation_update_evaluation;
    },

    ViewQuestionTemplate(item: any){
      let localThis: any = this as any;      
      localThis.evaluation_id = item.EVALUATION_ID
      localThis.contKeyQuestions ++;
      localThis.activation_questions_template = true;
    },
    
    forceRerender() {
      let localThis : any = this as any;
      localThis.componentKeyTables += 1;  
    },
    forceRerenderEvaluation() {
      let localThis : any = this as any;
      localThis.componentEvaluationForm++;  
    },
  }
};
</script>
