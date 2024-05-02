<template >
<v-container fluid fill-height>
     <v-row align="center" >
                <!-- PROJECT SELECT -->
                        <v-col class="d-flex" cols="12" :sm="5">
                          <v-row>
                            
                          <v-col :cols="12">
                            <h3 v-if="action!='adding'">{{$t('datasource.pms.annual-plan.ap-project')}}:</h3>
                            <h3 v-if="action!='adding'">{{project_descr}}</h3>
                            <v-autocomplete
                                v-model="project"
                                :label="$t('views.view-activity-instance.project-select')"
                                :items="projectList"
                                 filled
                                item-value="PROJECT_ID"
                                item-text="PROJECT_DESCR"
                                :key="project.PROJECT_ID"
                                return-object
                                @change="project_descr=project.PROJECT_DESCR"
                            ></v-autocomplete>
                         
                              </v-col>
                             
                        </v-row>
                         </v-col>
                        <!-- ACTIVITY SELECT -->
                        <v-col class="d-flex" cols="12" :sm="4">
                            <v-row>
                            
                            <v-col :cols="12">
                            <h3 v-if="action!='adding'">{{$t('datasource.pms.annual-plan.objectives.activity')}}:</h3>
                            <h3 v-if="action!='adding'">{{activity_descr}}</h3>
                            <v-autocomplete
                                v-model="activity"
                                :key="activity.ACTIVITY_ID"
                                :label="$t('views.view-activity-instance.activity-select')"
                                :disabled="!project.PROJECT_ID"
                                :items="activityList.filter((item) => item.PROJECT_ID == project.PROJECT_ID)"
                                item-value="ACTIVITY_ID"
                                item-text="ACTIVITY_DESCR"
                                return-object
                                filled
                                @change="ActivityInstanceSelected=0,activity_descr=activity.ACTIVITY_DESCR"
                            > </v-autocomplete>
                             </v-col>
                        </v-row>
                        </v-col>
   <!-- ACTIVITY INSTANCE SELECT -->
     <v-col class="d-flex" cols="12" :sm="3">
        <jrs-search-table
                  :hint="$t('views.view-activity-instance.activity-instance-select')"
                  v-model="ActivityInstanceSelected"
                  :key="ActivityInstanceSelected"
                  label="Activity Instance"
                  :dataSourceCondition="`ACTIVITY_ID = ${activity.ACTIVITY_ID == undefined ? activitySelectedId : activity.ACTIVITY_ID}`"
                  data-source="PMS_VI_ACTIVITY_INSTANCE"
                  itemValue="INSTANCE_ID"
                  itemText="DATE_ACTIVITY_INSTANCE"
                
           ></jrs-search-table> 
      </v-col>

    <v-col class="d-flex" cols="12" :sm="9">
       <v-text-field
              v-model="name_test"
              :label="$t('general.exam-name')"
              hide-details
              clearable
              outlined
              dense
              class="white"
              color="secondary darken-2"
              required
            ></v-text-field>
    </v-col>

     <v-col class="d-flex" cols="12" :sm="3" >
                 <jrs-search-table
                  v-if="!hidePlacementInterval"
                  hint="select the placement interval for placement test"
                  v-model="placement_interval_id"
                  :key="placement_interval_id"
                  label="Placement Interval"
                  data-source="PMS_PLACEMENT_INTERVAL"
                  itemValue="PLACEMENT_INTERVAL_ID"
                  itemText="PLACEMENT_LEVEL_DESCRIPTION"
                  :savePayload="{userUid:getUserUid, officeId:getCurrentOffice.HR_OFFICE_ID}"
           ></jrs-search-table> 
           <v-slider
              v-model="passing_score" 
              max="10"
              min="0"
             :label="$t('general.exam-score')+' : '+passing_score"
             v-if="!hidePassingScore"
        ></v-slider>
         
      </v-col>
     </v-row>
        
     <v-row  align="center"  >
       <v-col  class="d-flex" cols="12" sm="12">
        <v-expansion-panels  light >
      <v-expansion-panel
      >
        <v-expansion-panel-header :class="classAddQuestion" style="color:white" >
        {{titleAddQuestion}}
          <template >
          </template>
        </v-expansion-panel-header>
      </v-expansion-panel>
       <v-expansion-panel
        v-for="(item,i) in questions_added"
        :key="i"
        
      >
        <v-expansion-panel-header  class="primary" style="color:white" >
          Question : {{item.QUESTION_TEXT}}
        </v-expansion-panel-header>
         <v-expansion-panel-content > Type : {{item.QUESTION_TYPE}}</v-expansion-panel-content>
        <v-expansion-panel-content v-if="item.ANSWER!= undefined && item.ANSWER!= ''">Correct Answer : {{item.ANSWER}} </v-expansion-panel-content>
        <v-expansion-panel-content  v-if="item.ANSWER_SCORE!= undefined">Score :  {{item.ANSWER_SCORE}}</v-expansion-panel-content>
      </v-expansion-panel>
    </v-expansion-panels>
     </v-col>
       <v-col class="d-flex" cols="12" sm="3">
          
       <v-btn  medium color="orange" @click="OpenQuestionsForm()" dark >
          <v-icon v-if="!activation_questions" dark>mdi-plus</v-icon> 
          <v-icon v-if="activation_questions" dark>mdi-minus</v-icon>
          Add Questions
       </v-btn>      
       <v-btn v-if="questions_added.length>1" class="ml-2"  medium color="orange" @click="activation_order_questions = true" dark >
          <v-icon dark>mdi-elevator</v-icon> 
          Order Questions
       </v-btn>
        </v-col>
      <v-col class="d-flex flex-row-reverse"  >
         <v-btn color="primary" medium  @click="save(questions_added)" dark >
          Save 
         </v-btn>
       </v-col>
      
       </v-row>
   
      <v-dialog
            v-model="activation_order_questions"
            persistent
            retain-focus
            :overlay="false"
            max-width="60em"
            transition="dialog-transition"

        >


          <v-card>
         <v-card-title>Sort the questions</v-card-title>
                <v-card-text>
                         <v-row  align="center"  >
       <v-col  class="d-flex" cols="12" sm="12">
        <v-expansion-panels  light>
      <v-expansion-panel
      >
        <v-expansion-panel-header :class="classAddQuestion" style="color:white" >
        {{titleAddQuestion}}
          <template >
          </template>
        </v-expansion-panel-header>
      </v-expansion-panel>
       <v-expansion-panel
        v-for="(item,i) in questions_added"
        :key="i"
        
      >
      <div class="text-center">
      <v-btn
              color="secondary"
              small
              dark
              v-if="i != 0" @click="OrderQuestion(item,i)"
            >
               <v-icon dark>mdi-elevator</v-icon> 
            </v-btn>
        </div>
    
        <v-expansion-panel-header  class="primary" style="color:white" >
          Question : {{item.QUESTION_TEXT}}
        </v-expansion-panel-header>
         <v-expansion-panel-content > Type : {{item.QUESTION_TYPE}}</v-expansion-panel-content>
        <v-expansion-panel-content v-if="item.ANSWER!= undefined && item.ANSWER!= ''">Correct Answer : {{item.ANSWER}} </v-expansion-panel-content>
        <v-expansion-panel-content  v-if="item.ANSWER_SCORE!= undefined">Score :  {{item.ANSWER_SCORE}}</v-expansion-panel-content>
      </v-expansion-panel>
    </v-expansion-panels>
     </v-col>

      
       </v-row>
          </v-card-text>
              <v-card-actions>
          
           <v-btn
                        color="primary"
                        text
                        @click="activation_order_questions = false"
                    >{{ $t('general.confirm') }}</v-btn>
    
            </v-card-actions>
    </v-card>

      </v-dialog>

    
      <v-dialog
            v-model="activation_questions"
            persistent
            retain-focus
            :overlay="false"
            max-width="60em"
            transition="dialog-transition"
        >
    <v-card>
         <v-card-title></v-card-title>
                <v-card-text>
                  <jrs-question
                                :viewName="'PMS_VI_QUESTION_ANSWER'"
                                :hideNewBtn="true"
                                :hideActions="false"
                                :dataSourceCondition="'FREQUENCY_OF_QUESTION = 0'"
                                :orderCondition="'QUESTION_ID DESC'"
                                @AddQuestions="AddQuestions"
                                :initialQuestions ="questions_added"
                                :action="action"
                  ></jrs-question>
          </v-card-text>
          <v-card-actions>
                    <v-btn
                        color="secondary darken-1"
                        text
                        @click="activation_questions = false"
                    >X {{ $t('general.close') }}</v-btn>
            </v-card-actions>
    </v-card>
    </v-dialog>
      
  </v-container>

</template>

<script lang="ts" >
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../../axiosapi/api";
import SearchTable from "../../JrsSearchTable.vue";
import JrsQuestion from "../../JrsQuestion.vue";
import { debug } from "webpack";



interface payLoadDataPlacementTest {
  PLACEMENT_TEST_ID: number | 0;
  EXAM_NAME: String | null;
  PLACEMENT_INTERVAL_ID: number | null;
  QUESTION_ID: number | null;
  ANSWER_ID: number | null;
  ACTIVITY_INSTANCE_ID :number | null;
}

interface payLoadDataPlacementTestQuestions{
  PLACEMENT_TEST_ID: number | 0;
  QUESTION_ID: number | null;
}

interface payLoadDataExamQuestions{
  EXAM_ID: number | 0;
  QUESTION_ID: number | null;
}
interface payLoadDataEvaluationQuestions{
  EVALUATION_ID: number | 0;
  QUESTION_ID: number | null;
}

interface payLoadDataExam {
  EXAM_ID: number | 0;
  EXAM_NAME: String | null;
  PASSING_SCORE: number | null;
  QUESTION_ID: number | null;
  ANSWER_ID: number | null;
  ACTIVITY_INSTANCE_ID :number | null;
}

interface payLoadDataEvaluation{
  EVALUATION_ID: number | 0;
  EXAM_NAME: String | null;
  PASSING_SCORE: number | null;
  QUESTION_ID: number | null;
  ANSWER_ID: number | null;
  ACTIVITY_INSTANCE_ID :number | null;
}

export default Vue.extend({
  name: "PlacementTestExamEvaluation",
  components: {
      //"form-question": PlacementTestQuestionForm,
      "jrs-search-table": SearchTable,
      "jrs-question": JrsQuestion,
  },
  props:{

    placement_test_id: {
      type : Number,
      required: false,
    },

    forceRerender:{
      type : Function,
    },
    exam_id:{
      type : Number,
      required: false,
    },

    typeAssignment:{
      type : String,
      required: false,
    },

    action: {
      type: String,
      required: true,
      default: 'adding'
    }

  },
  data() {
    return {
      projectList: [],
      activityList: [],
      project:{},
      activity: {},
      project_descr: "",
      activity_descr: "",
      activitySelectedId: 0,
      activityInstanceSelectedId: 0,
      questions_added: [],
      activityTypeEducation: 1,
      activation_questions:false,
      activation_order_questions:false,
      name_test: "",
      placement_interval_id: 0,
      passing_score: 0,
      hidePlacementInterval: false,
      hidePassingScore: false,
      classAddQuestion: "orange",
      titleAddQuestion:"Add Questions"
    };
   
  },

  
  computed: {
     ...mapGetters({
                getCurrentOffice: 'getCurrentOffice',
                 getUserUid: 'getUserUid'
            }),   
     ActivityInstanceSelected:{
        
      get(){
        let localThis: any = this as any;
        return localThis.activityInstanceSelectedId;
      },
      set(val){
         let localThis: any = this as any;
         localThis.activityInstanceSelectedId = val;
  
      
      }
    },

    sortedArray: function() {
      let localThis: any = this as any;
      function compare(a:any, b:any) {
        if (a.name < b.name)
          return -1;
        if (a.name > b.name)
          return 1;
        return 0;
      }
      

      return localThis.questions_added.sort(compare);
  }

  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),
    AddQuestions(questionsAdded: any){
  
        let localThis: any = this as any;
          localThis.questions_added = []
          localThis.questions_added = questionsAdded
    
    },
    OrderQuestion(item:any,i:any){
      
      let localThis: any = this as any;
    
      [ localThis.questions_added[i-1],  localThis.questions_added[i]] = [ localThis.questions_added[i],  localThis.questions_added[i-1]];
      localThis.questions_added.sort((a:any,b:any)=> a=b )
   
    },
    OpenQuestionsForm(){
        let localThis: any = this as any;
        localThis.classAddQuestion = "orange"; 
        localThis.titleAddQuestion = "Add questions!";
        localThis.activation_questions=!  localThis.activation_questions;
    },
    forceRerenderTable: function () {
           // Do your stuff
           this.forceRerender();
   },
    
   getProjectsActivities(officeId:number){
                let localThis:any = this as any;
                let selectPayload:GenericSqlSelectPayload = {
                    viewName: "PMS_VI_OFFICE_PROJECT_ACTIVITIES",
                    officeId: officeId,
                    whereCond: `ACTIVITY_TYPE_ID = 1`,
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
                                 return{
                                    PROJECT_ID: row.PROJECT_ID,
                                    ACTIVITY_ID: row.ACTIVITY_ID,
                                    ACTIVITY_NAME: row.ACTIVITY_NAME,
                                    ACTIVITY_DESCR: row.ACTIVITY_DESCR,
                                    IS_EDUCATION: row.IS_EDUCATION,
                                    IS_DISTRIBUTION: row.IS_DISTRIBUTION,
                                    IS_WELFARE: row.IS_WELFARE,
                                    
                                 };
                                
                               
                            
                            });
                            
                            localThis.projectList = [...projList];
                            localThis.activityList = [...actList];
                            //debugger
                        }
                    }).catch((err: any) => {
                   
                        localThis.showNewSnackbar({
                          typeName: "error",
                          text: err.message
                          
                        }); // Feedback to user
                    });
            },
            

    GetPlacementTest(placement_test_id: Number){
          
      let localThis: any = this as any;
      localThis.activityList = [];

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_QUESTION_ANSWER_PLACEMENT_TEST",
        colList: null,
        whereCond: `PLACEMENT_TEST_ID = ${placement_test_id}`,
        orderStmt: "PLACEMENT_TEST_ID"
      };
      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {

          //Setup data
          if (res.table_data) {
              res.table_data.forEach((item: any) => {
                 localThis.name_test= item.EXAM_NAME
                 localThis.activitySelectedId= item.ACTIVITY_ID
                 localThis.activityInstanceSelectedId = item.ACTIVITY_INSTANCE_ID
                 localThis.questions_added.push(item);
                 localThis.activity_descr = item.DESCRIPTION
                 localThis.project_descr = item.PROJECT_DESCRIPTION
                 localThis.placement_interval_id = item.PLACEMENT_INTERVAL_ID;
            });
          }
          
        }
        
      ).catch((err: any) => {

          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message
            
          }); // Feedback to user
      });

    },

    GetExam(exam_id: Number){
          
      let localThis: any = this as any;
      localThis.activityList = [];

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_QUESTION_ANSWER_EXAM",
        colList: null,
        whereCond: `EXAM_ID = ${exam_id}`,
        orderStmt: "EXAM_ID"
      };
      
      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {

          //Setup data
          if (res.table_data) {
              res.table_data.forEach((item: any) => {
                 
                 localThis.name_test= item.EXAM_NAME
                 localThis.activitySelectedId= item.ACTIVITY_ID
                 localThis.activityInstanceSelectedId = item.ACTIVITY_INSTANCE_ID
                 localThis.activity_descr = item.DESCRIPTION
                 localThis.project_descr = item.PROJECT_DESCRIPTION
                 localThis.questions_added.push(item);
                 
                 localThis.passing_score = item.PASSING_SCORE;
            });
  
          }
          
        }
        
      ).catch((err: any) => {
  
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message
            
          }); // Feedback to user
      });

    },

    GetEvaluation(evaluation_id: Number){
          
      let localThis: any = this as any;
      localThis.activityList = [];

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_QUESTION_ANSWER_EVALUATION",
        colList: null,
        whereCond: `EVALUATION_ID = ${evaluation_id}`,
        orderStmt: "EVALUATION_ID"
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {

          //Setup data
          if (res.table_data) {
              res.table_data.forEach((item: any) => {
                 localThis.name_test= item.EXAM_NAME
                 localThis.activitySelectedId= item.ACTIVITY_ID
                 localThis.activityInstanceSelectedId = item.ACTIVITY_INSTANCE_ID
                 localThis.questions_added.push(item);
                 localThis.activity_descr = item.DESCRIPTION
                 localThis.project_descr = item.PROJECT_DESCRIPTION
                 localThis.passing_score = item.PASSING_SCORE;
            });
        
          }
          
        }
        
      ).catch((err: any) => {

          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message
            
          }); // Feedback to user
      });
    },

    save(item: any){
         
        let localThis = this as any;
         
        if(item.length != 0 ){
          if(localThis.typeAssignment == "PlacementTest"){
            localThis.AddUpdPlacementTest(item)
          }else if(localThis.typeAssignment == "Exam"){
            localThis.AddUpdExam(item)
          }else if(localThis.typeAssignment == "Evaluation"){
            localThis.AddUpdEvaluation(item)
          }
        }else{
          localThis.classAddQuestion = "red";
          localThis.titleAddQuestion = "Please, Add questions!"
        }
    },

    AddUpdPlacementTest(item: any){
      
      let localThis = this as any;
      let payLoadD = {} as payLoadDataPlacementTest;
      let payQuestions = [] as any;
     
      let actiontype: SqlActionType;
      let msg = "";
      let msgUpd: string = this.$i18n
        .t("datasource.pms.learning.placement-test.confirm-update-placement-test")
        .toString();
      let msgAdd: string = this.$i18n
         .t("datasource.pms.learning.placement-test.confirm-add-placement-test")
        .toString();
      
      if(localThis.action ==  'update'){
          let type = SqlActionType.NUMBER_1
          actiontype=type
          msg = msgUpd;
          
      }else if(localThis.action ==  'adding'){
          let type = SqlActionType.NUMBER_0
          actiontype=type
          msg = msgAdd;
            
      }
      
     this.$confirm(msg).then(res => {
      
      if (res) {
         
          payLoadD.PLACEMENT_TEST_ID=localThis.placement_test_id;
          payLoadD.EXAM_NAME=localThis.name_test;
          payLoadD.ACTIVITY_INSTANCE_ID = Number.parseInt(localThis.ActivityInstanceSelected);
          payLoadD.PLACEMENT_INTERVAL_ID = Number.parseInt(localThis.placement_interval_id);
          
             item.forEach((element: any) => {    
              let payLoadQuestions= {} as payLoadDataPlacementTestQuestions;  
              payLoadQuestions.PLACEMENT_TEST_ID = localThis.placement_test_id;
              payLoadQuestions.QUESTION_ID = element.QUESTION_ID;
              
              payQuestions.push({
                PLACEMENT_TEST_ID: payLoadQuestions.PLACEMENT_TEST_ID,
                QUESTION_ID: payLoadQuestions.QUESTION_ID
              })
          });
        
      
          let saveData = {
                    questions: payQuestions,
                    rows : payLoadD
                  };
           
          let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_DEL_PLACEMENT_TEST",
                actionType: actiontype,
                jsonPayload: JSON.stringify(saveData),
                userUid: this.getUserUid,
                officeId: this.getCurrentOffice.HR_OFFICE_ID
              };
                 

              localThis
                .execSPCall(savePayload)
                .then((result: any) => {
                  localThis.showNewSnackbar({ typeName: "success", text: result.message }); // Feedback to user
                  if(localThis.action == 'update'){
                    localThis.forceRerenderTable()
                  }
                  
                }).catch((err: any) => {
             
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message
                   
                  }); // Feedback to user
                });
          }
       });
    },

      AddUpdExam(item: any){
      
      let localThis = this as any;
      let payLoadD = {} as payLoadDataExam;
      let payQuestions = [] as any;

      let actiontype: SqlActionType;
      let msg = "";
      let msgUpd: string = this.$i18n
        .t("datasource.pms.learning.exam.confirm-update-exam")
        .toString();
      let msgAdd: string = this.$i18n
         .t("datasource.pms.learning.exam.confirm-add-exam")
        .toString();
    
      if(localThis.action ==  'update'){
          let type = SqlActionType.NUMBER_1
          actiontype=type
          msg = msgUpd;
          
      }else if(localThis.action ==  'adding'){
          let type = SqlActionType.NUMBER_0
          actiontype=type
          msg = msgAdd;
            
      }
       
     this.$confirm(msg).then(res => {
      
      if (res) {
        payLoadD.EXAM_ID=localThis.exam_id;
          payLoadD.EXAM_NAME=localThis.name_test;
          payLoadD.ACTIVITY_INSTANCE_ID = Number.parseInt(localThis.activityInstanceSelectedId);
          payLoadD.PASSING_SCORE = Number.parseInt(localThis.passing_score);
          
             item.forEach((element: any) => {    
              let payLoadQuestions= {} as payLoadDataExamQuestions;  
              payLoadQuestions.EXAM_ID = localThis.placement_test_id;
              payLoadQuestions.QUESTION_ID = element.QUESTION_ID;
              payQuestions.push({
                PLACEMENT_TEST_ID: payLoadQuestions.EXAM_ID,
                QUESTION_ID: payLoadQuestions.QUESTION_ID
              })
          });
        
      
          let saveData = {
                    questions: payQuestions,
                    rows : payLoadD
                  };
           
          let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_DEL_EXAM",
                actionType: actiontype,
                jsonPayload: JSON.stringify(saveData),
                userUid: this.getUserUid,
                officeId: this.getCurrentOffice.HR_OFFICE_ID,
              };
                 

              localThis
                .execSPCall(savePayload)
                .then((result: any) => {
                  localThis.showNewSnackbar({ typeName: "success", text: result.message }); // Feedback to user
                  if(localThis.action == 'update'){
                    localThis.forceRerenderTable()
                  }
                }).catch((err: any) => {
             
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message
                   
                  }); // Feedback to user
                });
          }
       });
    },

    AddUpdEvaluation(item: any){
    
      let localThis = this as any;
      let payLoadD = {} as payLoadDataEvaluation;
      let payQuestions = [] as any;

      let actiontype: SqlActionType;
      let msg = "";
      let msgUpd: string = this.$i18n
        .t("datasource.pms.learning.evaluation.confirm-update-exam")
        .toString();
      let msgAdd: string = this.$i18n
         .t("datasource.pms.learning.evaluation.confirm-add-exam")
        .toString();
    
      if(localThis.action ==  'update'){
          let type = SqlActionType.NUMBER_1
          actiontype=type
          msg = msgUpd;
          
      }else if(localThis.action ==  'adding'){
          let type = SqlActionType.NUMBER_0
          actiontype=type
          msg = msgAdd;
            
      }
       
     this.$confirm(msg).then(res => {
      
      if (res) {
        payLoadD.EVALUATION_ID=localThis.exam_id;
          payLoadD.EXAM_NAME=localThis.name_test;
          payLoadD.ACTIVITY_INSTANCE_ID = Number.parseInt(localThis.activityInstanceSelectedId);
          payLoadD.PASSING_SCORE = Number.parseInt(localThis.passing_score);
          
             item.forEach((element: any) => {    
              let payLoadQuestions= {} as payLoadDataEvaluationQuestions;  
              payLoadQuestions.EVALUATION_ID = localThis.evaluation_id;
              payLoadQuestions.QUESTION_ID = element.QUESTION_ID;
              payQuestions.push({
                EVALUATION_ID: payLoadQuestions.EVALUATION_ID,
                QUESTION_ID: payLoadQuestions.QUESTION_ID
              })
          });
        
      
          let saveData = {
                    questions: payQuestions,
                    rows : payLoadD,
                  };
           
          let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_DEL_EVALUATION",
                actionType: actiontype,
                jsonPayload: JSON.stringify(saveData),
                userUid: this.getUserUid,
                officeId: this.getCurrentOffice.HR_OFFICE_ID,
              };
                 

              localThis
                .execSPCall(savePayload)
                .then((result: any) => {
                  localThis.showNewSnackbar({ typeName: "success", text: result.message }); // Feedback to user
                  if(localThis.action == 'update'){
                    localThis.forceRerenderTable()
                  }
                }).catch((err: any) => {
             
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message
                   
                  }); // Feedback to user
                });
          }
       });
    },

  },

  mounted() {
       
      let localThis: any = this as any; 
   
         switch(localThis.typeAssignment) { 
                case "PlacementTest": { 
                    if(localThis.action == 'update'){
                       localThis.hidePlacementInterval= false;
                       localThis.hidePassingScore=true;
                       localThis.GetPlacementTest(localThis.placement_test_id) 
                     }
                       localThis.hidePlacementInterval= false;
                       localThis.hidePassingScore=true;
                      
                    break; 
                } 
                case "Exam": { 
                      if(localThis.action == 'update'){
                        localThis.hidePlacementInterval= true;
                        localThis.hidePassingScore=false;
                        localThis.GetExam(localThis.exam_id)  
                      }
                       localThis.hidePlacementInterval= true;
                       localThis.hidePassingScore=false;
                         
                    break; 
                } 
                case "Evaluation":{
                     if(localThis.action == 'update'){
                        localThis.hidePlacementInterval= true;
                        localThis.hidePassingScore=false;
                        localThis.GetEvaluation(localThis.exam_id)  
                      }
                       localThis.hidePlacementInterval= true;
                       localThis.hidePassingScore=false;
                      
                     //TODO; 
                     break;
                }
                default: { 
                   
                    break; 
                } 
              } 
      
      localThis.getProjectsActivities(localThis.getCurrentOffice.HR_OFFICE_ID);
     
  },
 
});
</script>

<style scoped>

</style>