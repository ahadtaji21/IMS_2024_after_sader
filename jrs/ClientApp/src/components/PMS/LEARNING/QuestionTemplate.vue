<template>

 <div class="text-center">
    <v-dialog
      scrollable
      v-model="hideDialog"
      width="500"
    >
      <v-card>
        <v-card-title>
          {{title+' : '+title_exam}}
        </v-card-title>
        <v-card-text>
    
       <div
          class="text-center"
          v-if="progress"
          style="margin: 0px; padding: 0px"
            >
              <v-progress-circular
                indeterminate
                color="primary"
              ></v-progress-circular>
        </div>
             <jrs-question-form v-if="progress==false" :formData="formData" :ForEvaluation="ForEvaluation" :nbrOfColumns="2" >
                         
             </jrs-question-form >
         
        </v-card-text>

        <v-divider></v-divider>

        <v-card-actions>
          <v-spacer></v-spacer>
          
          <v-btn
            v-if="ForEvaluation"
            color="primary"
            text
            @click="dialogActionClose()"
          >
            Complete Test
          </v-btn>
          <v-btn
            v-if="!ForEvaluation"
            color="primary"
            text
            @click="dialogActionClose()"
          >
            Close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

  </div>

</template>
<script lang="ts">
import Vue from 'vue'
import JrsQuestionForm from '../../JrsFormQuestion.vue'
import { mapGetters, mapActions } from "vuex";
import {
  GenericSqlSelectPayload,
} from "../../../axiosapi/api";

export default Vue.extend({
  components: {
   'jrs-question-form':JrsQuestionForm
  },

  computed: {
    computedDatasourceCondition(){
      let localThis:any = this as any;
      let condition:string = '1=1';
      condition = localThis.columnCond + ' = ' +localThis.testId;

      return condition;
    },
  },

  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
      getJRSTableStructure: "getJRSTableStructure"
    }),
     Answer(formData: any,id: Number){
     
     },
     dialogActionClose() {
      let localThis: any = (this as any);
  
      localThis.hideDialog = !localThis.noHidedialog
      return localThis.hideDialog;
    },
     getQuestions(id: Number, view: string){
        let localThis: any = this as any;
        localThis.activityList = [];
        let selectPayload: GenericSqlSelectPayload = {
     
        viewName: view,
        colList: null,
        whereCond: localThis.computedDatasourceCondition,
        orderStmt: localThis.columnCond
      };
      
      // active prgress circular component
      localThis.progress = true; 
      
      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          // disable prgress circular component
          localThis.progress = false; 
          //Setup data
          if (res.table_data) {
              res.table_data.forEach((item: any) => {
                 localThis.formData.push(item);
                 localThis.title_exam = item.EXAM_NAME
            });
          }else{
               localThis.title = "No questions for"
               localThis.title_exam = ""   
          }
        }).catch((err: any) => {
          // disable prgress circular component
          localThis.progress = false; 
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message
          }); 
        });      
     },

  },
     props:{
            /**
             * Id of test
             */
            testId:{
                type:  Number,
                required: true
            },
            viewName: {
                type: String,
                required: true
            },
            columnCond:{
                type: String,
                required: false
            },
            noHidedialog:{
                type: Boolean,
                required: true,
                default: false
            },
            ForEvaluation:{
              type : Boolean,
              required: false,
              default: false
            },
            
       
        },
 
  data() {
    return {
      content: "<h1>Some initial content</h1>",
      formFields: [],
      formData: [],
      headers: [],
      positionList: [],
      title_exam:"",
      tableFilter: "",
      selectedPos: [],
      hideDialog: false,
      title: "Template Questions",
      itemKey: "",
      position_CRUD: {},
      progress:false
    };
  },



  mounted(){
        let localThis: any = this as any;
        localThis.hideDialog = localThis.noHidedialog;
      
        if(localThis.testId != undefined && localThis.testId != 0){
           localThis.getQuestions(localThis.testId,localThis.viewName)
        }
  },

 
})
</script>

<style scoped>

</style>