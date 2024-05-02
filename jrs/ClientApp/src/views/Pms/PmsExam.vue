<template>
  <v-content>
    <v-container fluid fill-height>
      <h1>Exam</h1>
      <v-row>
        <v-col :cols="12">
          <v-card>
            <v-tabs v-model="tabIsVisible" @change="forceRerender(),forceRerenderExam()" background-color="primary darken-1" dark>
              <v-tab v-for="item in tabsItems" :key="item.code" @change="forceRerender()">{{ item.descr }} </v-tab>
            </v-tabs>
            <v-tabs-items   v-model="tabIsVisible" style="padding: 0.5em">
                <v-tab-item key="NEW_EXAM"  >
                  <plc-form-test
                    :action="'adding'"
                    :typeAssignment= "typeAssignment"
                    :key="componentExamForm" 
                  >
                    
                  </plc-form-test>
                  </v-tab-item>
                  <v-tab-item  key="EXAMS">
                     
                  <jrs-simple-table
                    viewName="PMS_VI_EXAM"
                
                    :hideNewBtn="true"
                    :hideActions="true"
                    :clientPagination="true"
                    :showSearchField="true"
                    :key= "componentKeyTables"
                    
                  >
                  
                  <!-- ROW ACTIONS -->
                  <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">
                    <v-icon @click="selectExamItem(simpleItemRowItem),forceRerenderExam();">mdi-circle-edit-outline</v-icon>
                    <v-icon @click="DelExam(simpleItemRowItem); forceRerenderExam();">mdi-delete</v-icon>
                    <v-icon @click="ViewQuestionTemplate(simpleItemRowItem); forceRerenderExam();">mdi-text-box-search-outline</v-icon>
                  </template>
                  <!-- TABLE HEADER -->
                  </jrs-simple-table>
                   
          
          
                  </v-tab-item>
                
                 

            </v-tabs-items>
          </v-card>
        </v-col>
      </v-row>
       <jrs-question-template
               :testId="exam_id"
               viewName="PMS_VI_QUESTION_ANSWER_EXAM"
               columnCond="EXAM_ID"
               :noHidedialog="activation_questions_template"
               :key="contKeyQuestions"
               :ForEvaluation="false"
          >
         </jrs-question-template>
 
      <v-dialog
            v-model="activation_update_exam"
            max-width="60em"
            transition="dialog-transition"
            persistent
            retain-focus
            :scrollable="true"
           
        >
        
    <v-card>
         <v-card-title><v-row><v-col>Exam</v-col> <v-col  class="d-flex flex-row-reverse"  > <v-btn
                        color="secondary darken-1"
                        text
                        @click="activation_update_exam= false"
                    >X {{ $t('general.close') }}</v-btn></v-col></v-row></v-card-title>
         
                <v-card-text>

                  <plc-form-test
                    :exam_id="exam_id"
                    :action="action"
                    :key="componentExamForm" 
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
import { debug } from 'webpack';
import JrsQuestionTemplate from "../../components/PMS/LEARNING/QuestionTemplate.vue"
//TMP
interface payLoadDataExam{
  EXAM_ID: number | 0;
  EXAM_NAME: String | null;
  PASSING_SCORE: number | null;
  QUESTION_ID: number | null;
  ACTIVITY_ID :number | null;
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
        { code: "NEW_EXAM", descr: "New Exam Test" },
        { code: "EXAMS", descr: "Exams" }
      
      ],
      genericQueryData: false,
      tmpSelectedRows: [],
      placement_test_id:  0,
      placement_test: {},
      exam_id:  0,
      componentExamForm: 0,
      exam: {},
      activation_questions_template: false,
      contKeyQuestions:0,
      typeAssignment: "Exam",
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
    
    DelExam(item: any){
       
      let localThis = this as any;
      let payLoadD = {} as payLoadDataExam;
      let check_ins_one=1;
    
      let msgDel: string = localThis.$i18n
        .t("datasource.pms.learning.exam.confirm-delete-exam")
        .toString();
    
       
       localThis.$confirm(msgDel).then((res:any) => {
       if (res) {
          
          payLoadD.EXAM_ID=item.EXAM_ID;
          payLoadD.EXAM_NAME=item.EXAM_NAME;
          payLoadD.PASSING_SCORE = item.PASSING_SCORE;
          let saveData = {
                    check_one_time : check_ins_one,
                    question_id: 0,
                    rows : payLoadD
                  };
           
          let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_DEL_EXAM",
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

    selectExamItem(item: any){
        
      let localThis: any = this as any;
      localThis.exam = Object.assign({}, localThis.exam, item);        
      localThis.exam_id = item.EXAM_ID
      localThis.action = "update"
      localThis.forceRerenderExam();
      localThis.activation_update_exam = !localThis.activation_update_exam;
    },

    ViewQuestionTemplate(item: any){
      let localThis: any = this as any;      
      localThis.exam_id = item.EXAM_ID
      localThis.contKeyQuestions ++;
      localThis.activation_questions_template = true;
    },
    
    forceRerender() {
      let localThis : any = this as any;
      localThis.componentKeyTables += 1;  
    },
    forceRerenderExam() {
      let localThis : any = this as any;
      localThis.componentExamForm += 1;  
    },

  },
};
</script>
