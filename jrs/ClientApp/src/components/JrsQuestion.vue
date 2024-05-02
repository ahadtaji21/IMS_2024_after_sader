<template>
  <div >
     <v-sheet
             class="mx-auto mb-3"
            elevation="12"  
        >
    <v-container v-if="hideNewBtn">
    <v-row >
      <h2 class="ml-2">New Question</h2>
      </v-row>
      <v-row>
       <v-col>
      <v-text-field
            :name ="'Question'"
            :label ="'Question Text'"
            v-model="question_text_new"
        
            required
      ></v-text-field>
       </v-col>
  

         <v-col>
        <!-- SELECT --> 
        <v-select
            :label ="'Type'"
            v-model="question_type_id_new"
            :items="question_types"
            :item-value="'QUESTION_TYPE_ID'"
            :item-text="'QUESTION_TYPE'"
            required
        ></v-select>

       </v-col>
      </v-row>
      <v-row>
             <v-btn color="primary" class="mt-2 mx-1"  @click="saveNewQuestion(0,question_text_new,question_type_id_new);" >Add to list</v-btn>
        </v-row>
    </v-container>
    </v-sheet>
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
      
    <jrs-table
      :title="title || $t(tableTitleKey) || viewName"
      :headers="headers"
      :rows="rows"
      :iconActions="iconActions"
      :pkName="pkName"
      v-model="selectedRows"
      :showSearchbar="showSearchField"
      :multiSelect="multiSelect"
      :selectable="selectable"
      v-if="progress==false"
      @optionsChange="updateOptions"
      @filterChange="updateWhereCondition"
      :serverItemsLength="rowNbr"
      :isLoading="tableIsLoading"
      
    >
      <!-- TABLE QUESTIONS MORE FREQUENCY -->
      <template v-slot:tableHeader  >
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
                                :dataSourceCondition ="'FREQUENCY_OF_QUESTION > 0'"
                                :orderCondition="'FREQUENCY_OF_QUESTION DESC'"
                                :hideNewBtn="false"
                                :hideActions="false"
                                :initialQuestions="initialQuestions"
                                @AddQuestionsFHPQ="AddQuestionsFHPQ"
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
                   <template >
                      <v-btn @click="activation_questions=true" color="secondary lighten-2" class="grey--text text--darken-3 ml-10" v-if="hideNewBtn">
                      <v-icon>mdi-plus-circle-outline</v-icon>Hight Previous Question
                   </v-btn>
                 </template>  
        <slot name="simple-table-header"></slot>
     
        </template>

      <!--ROW ACTIONS-->
      <template v-slot:itemActions="{rowItem}">
         
      
          <v-checkbox  class="shrink"  
              @change="checkQuestion(rowItem,rowItem.QUESTION_ID,false)"      
              :key="rowItem.QUESTION_ID"
              :disabled="rowItem.ANSWER != '' && rowItem.ANSWER != null && rowItem.ANSWER != undefined && rowItem.ANSWER != ' ' ? false : true"
              v-model="rowItem.CHECKED"
          ></v-checkbox>

            <v-icon  @click="openAnswerForm(rowItem)" medium>{{rowItem.BELONG_TO_ONE_TEST==0 ? 'mdi-tooltip-edit' : 'mdi-tooltip-text'}}</v-icon>
 
        <!-- UPDATE ICON -->
        <jrs-modal-form
          :formFields="formFields"
          :formData="rowItem"
          :nbrOfColumns="nbrOfFormColumns"
          v-if="crudInfo.update_sp && !hideActions"
        >
         
          <template  v-slot:activation>
            <v-icon class="mt-5"  medium>{{rowItem.BELONG_TO_ONE_TEST==0 ? 'mdi-circle-edit-outline' : 'mdi-magnify-scan'}}</v-icon>
          </template>
        
          <!-- <template v-slot:modal-form-actions="{ closeModalFunc, validateFunc, resetFunc }"> -->
          <template v-slot:modal-form-actions="{ closeModalFunc, validateFunc }">
      
            <v-btn color="secondary darken-1" class="mt-2 mr-1" text @click="closeModalFunc(); ">X {{ $t('general.close') }}</v-btn>
            <v-btn color="primary" class="mt-2 mx-1" v-if="rowItem.BELONG_TO_ONE_TEST!=0 ? false :true"  @click="saveData(rowItem, 1, closeModalFunc, validateFunc);">Save</v-btn>
  
          </template>
          

        </jrs-modal-form>

       
        <!-- DELETE ICON -->
        <v-icon  @click="toggleDeleteDialog(rowItem)" v-if="crudInfo.delete_sp && !hideActions && rowItem.BELONG_TO_ONE_TEST==0 ? true : false " >mdi-delete</v-icon>

        <!-- SLOT ACTIONS -->
        <slot name="simple-table-item-actions" v-bind:simpleItemRowItem="rowItem"></slot>
      </template>
    

    </jrs-table>
    


    <!--- DELETE DIALOG -->
    <v-dialog
      v-model="deleteDialog"
      persistent
      retain-focus
      :overlay="false"
      max-width="45em"
      transition="dialog-transition"
    >
      <v-card>
        <v-card-title primary-title class="text-capitalize">{{ $t('general.crud.item-deletion') }}</v-card-title>
        <v-card-text class="capital-first-letter">{{ $t('general.crud.item-delete-confirm') }}</v-card-text>
        <v-card-actions>
          <v-btn
            text
            color="secondary darken-1"
            @click="toggleDeleteDialog(rowItem)"
          >X {{ $t('general.close') }}</v-btn>
         
          <v-btn color="primary" @click="deleteData()">{{ $t('general.delete') }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    
     <!-- OPEN QUESTION / TRUE FALSE / NUMBER / DATE ANSWER FORM -->
           <v-dialog
                v-model="activation_answer"
                persistent
                retain-focus
                :overlay="false"
                max-width="45em"
                transition="dialog-transition"
              >
         <v-card>
               
         
        <v-card-title primary-title class="text-capitalize"></v-card-title>
        <v-card-text class="capital-first-letter">
        <v-text-field
            v-if="activation_open_number"
            :name ="'answer'"
            :label ="'answer'"
            v-model="answerText"
            :readonly="belong_to_one_test != 0 ? true : false"
            :persistent-hint ="false"
            :required ="true" 
            :append-icon="'mdi-tooltip-edit'"
            :type="activation_number ? 'number' : 'text'"
        ></v-text-field>
        <v-switch
        v-model="answerText"
        v-if="activation_true_false"
        :label=" answerText? 'true':'false'"
      ></v-switch>
          <jrs-date-picker
               label="Answer"
               v-model="answerText"
               v-if="activation_date"
            ></jrs-date-picker>
            
         <v-slider
              v-model="answerScore" 
              max="10"
              min="0"
              :label="'Score :'+answerScore"   
              :readonly="belong_to_one_test != 0 ? true : false"  
             required
        ></v-slider>
        </v-card-text>
        <v-card-actions>
          <v-btn
            text
            color="secondary darken-1"
            @click="activation_answer=!activation_answer"
          >X {{ $t('general.close') }}</v-btn>
          <v-btn color="primary" v-if="belong_to_one_test!=0 ? false :true" @click="AddUpdAnswer(selectedAnswerId,String(answerText),answerScore,selectedQuestionId,questionType)">{{ $t('general.answer') }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

  <!-- MULTICHOICE QUESTION ANSWER FOMR -->
      <v-dialog
                v-model="activation_multi_answers"
                persistent
                retain-focus
                :overlay="false"
                max-width="45em"
                transition="dialog-transition"
              >
         <v-card>
        <v-card-title primary-title class="text-capitalize"></v-card-title>
        <v-card-text class="capital-first-letter">
          <v-row  >
            <v-col class="d-flex" cols="12" sm="12">
              <v-slider
              v-model="answerScore" 
              max="10"
              min="0"
              :label="'Score :'+answerScore"     
              :readonly="belong_to_one_test != 0 ? true : false"
             required
        ></v-slider>
            </v-col>
            <v-col class="d-flex" cols="12" sm="12">
               <v-text-field
                    :name ="'Correct Answer'"
                    :label ="'answer'"
                    color="success"
                    v-model="answerText"
                    :readonly="belong_to_one_test != 0 ? true : false"
                    :persistent-hint ="false"
                    :required ="true" 
                    :append-icon="'mdi-tooltip-edit'"
                
              ></v-text-field>
                        
           </v-col>   
            <v-col class="d-flex" cols="12" sm="9">
               <v-text-field
                    :name ="'Multi Answer'"
                    :label ="'answer'"
                    :readonly="belong_to_one_test != 0 ? true : false"
                    color="error"
                    v-model="answerMultiText"
                    :persistent-hint ="false"
                    :required ="true" 
                    :append-icon="'mdi-tooltip-edit'"
                ></v-text-field>
                        
           </v-col>   
             
           <v-col class="d-flex" cols="12" sm="2">
             <v-btn color="secondary lighten-2" @click="AddMultiAnswers(answerMultiText)" class="grey--text text--darken-3" >
              <v-icon>mdi-plus-circle-outline</v-icon>Add Answer
            </v-btn>
            </v-col>   
            
        </v-row>
        
            <v-chip   
            v-for="(item,i) in multi_answers" 
              :key="i"
              class="ma-2"
              close
              color="error"
              text-color="white"
              @click:close="multi_answers.splice(i, 1)"
            >
            {{item}}
            </v-chip>
          </v-card-text>
        <v-card-actions>
          <v-btn
            text
            color="secondary darken-1"
            @click="activation_multi_answers=!activation_multi_answers"
          >X {{ $t('general.close') }}</v-btn>
          <v-btn color="primary" v-if="belong_to_one_test!=0 ? false :true" @click="AddUpdMultiAnswer(selectedAnswerId,answerText,multi_answers,answerScore,selectedQuestionId, questionType)">{{ $t('general.answer') }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
         
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import JrsTable from "../components/JrsTable.vue";
import JrsModalForm from "../components/JrsModalForm.vue";
import FormMixin from '../mixins/FormMixin';
import JrsDatePicker from "../components/JrsDatePicker.vue";
import { JrsHeader } from "../models/JrsTable";
import { JrsFormField, JrsFormFieldSelect, JrsFormFieldSearch } from "../models/JrsForm";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../axiosapi/api";
import { debug } from "webpack";
interface payLoadDataAnswer {
  ANSWER_ID: Number | 0;
  ANSWER: String | null;
  ANSWER_SCORE: Number | 0;
}

interface payLoadDataQuestion {
  QUESTION_ID: number | null;
  QUESTION_TEXT: string |null;
  QUESTION_TYPE_ID: number |null;
  ANSWER_ID: number | null;
}

interface payLoadQuestionType {
  QUESTION_TYPE_ID: number |null;
  QUESTION_TYPE: string | null;
}

interface payLoadMultiAnswer {
   QUESTION_ID: number |null;
   MULTI_ANSWER: string | null;
}

interface Question {
          QUESTION_ID: number | null,
          QUESTION_TEXT: string | null,
          ANSWER: string | null,
          QUESTION_TYPE_ID: number | null,
          BELONG_TO_ONE_TEST: boolean | null,
          FREQUENCY_OF_QUESTION: number | null,
          QUESTION_TYPE: string | null,
          ANSWER_SCORE: number | null,
          CHECKED: Boolean | null
        }
export default Vue.extend({
  components: {
    "jrs-table": JrsTable,
    "jrs-modal-form": JrsModalForm,
    "jrs-date-picker": JrsDatePicker
  },
  name: 'jrs-question',
  props: {
    /**
     * Model to be passed as v-model of v-data-table. 
     */
    value:{
        type: Array,
        required: false
    },

    action:{
      type: String,
      required: false
    },
    /**
     * Name of the view for which to recover table and form definition.
     */
    viewName: {
      type: String,
      required: true
    },
    /**
     * Title to display on table header.
     */
    title: {
      type: String,
      required: false
    },
    /**
     * Number of columns fo the form.
     */
    nbrOfFormColumns: {
      type: Number,
      required: false,
      default: 2
    },
    /**
     * Is the table selectable.
     */
    selectable: {
      type: Boolean,
      required: false,
      default: false
    },

    initialQuestions:{
        type: Array,
        required: false,
        default:Array
    },
    /**
     * Is the table a multiselect.
     * WARNING: requires "selectable":true.
     */
    multiSelect: {
      type: Boolean,
      required: false,
      default: false
    },
    /**
     * Allows the parent component to pass a condition for data-source.
     */
    dataSourceCondition: {
      type: String,
      required: false,
      default: null
    },
    /**
     * Allows the parent component to pass a list of desired columns.
     */
    columnList: {
      type: [String , Array],
      required: false,
      default: null
    },
    /**
     * Allows to disable "New" button.
     */
    hideNewBtn: {
      type: Boolean,
      required: false,
      default: false
    },
    /**
     * Allows to hide "Search" Text field.
     */
     showSearchField: {
      type: Boolean,
      required: false,
      default: true
    },
    orderCondition:{
      type: String,
      required: false,
      default: ""
    },
    /**
     * Allows to disable "Action" column.
     */
    hideActions: {
      type: Boolean,
      required: false,
      default: false
    },
    /**
     * Data to be added to the saving payload. 
     */
    savePayload: {
      type: Object,
      required: false,
      default: null
    },
   
    /**
     * Array of conditions for the datasources of "select-like" fields.
     */
    fieldDatasourceConditions: {
      type: Array,
      required: false,
      default: null,
      validator: (val:Array<any>) => {
        let valid:boolean = true;
        val.forEach( (field:any) => {
          if(!field.field_name || !field.field_condition){
            valid = false;
          }
        })
        return valid;
      }
    },
    /**
     * Determines if the table should use client-side pagination
     */
    clientPagination:{
      type: Boolean,
      required: false,
      default: false
    }
  },
  
  mixins:[FormMixin],
  data() {
    return {
      headers: [],
      activation_questions:false,
      rows: [],
      pkName: "",
      question_text_new: "",
      question_type_id_new: null,
      question_types:[],
      formFields: [],
      crudInfo: {},
      newFormData: {},
      iconActions: [],
      deleteDialog: false,
      deleteItem: {},
      tableTitleKey: '',
      selectedRows: [],
      selectedQuestions: [],
      selectedQuestionsRow: [],
      selectedAnswerId: 0,
      selectedQuestionId: 0,
      rows_checked:[],
      multi_answers: [],
      index_answers: [1],
      tableOptions: {
        page: 0,
        itemsPerPage: 5,
        sortBy: [],
        sortDesc: [],
        groupBy: [],
        groupDesc: [],
      },
      completeRows: [],
      activation_true_false: false,
      activation_date:false,
      activation_open_number: false,
      activation_number: false,
      answerText: "",
      questionType: "",
      answerMultiText: [],
      answerScore: 0,
      filterCondition: '',
      rowNbr: undefined,
      firstOptionUpdateDone: false,
      tableIsLoading: true,
      activation_answer: false,
      activation_multi_answers: false,
      label_true_false: "false",
      belong_to_one_test: 0,
      progress: false
    };
  },
  computed:{
    ...mapGetters({
      getCurrentOffice: 'getCurrentOffice',
      getUserUid: 'getUserUid'
    }),
  
    tableColumns(){
      let localThis:any = this as any;
      let cols:string = localThis.columnList;

      // Calculate columnList
      if(localThis.columnList && typeof localThis.columnList == 'object'){
        cols = localThis.columnList.reduce( (prev:string, curr:string) => {
          return prev.length == 0 ? curr : prev + ',' + curr;
        }, '');
      }
      return cols;
    },
    computedDatasourceCondition(){
      let localThis:any = this as any;
      let condition:string = '1=1';
      condition += localThis.dataSourceCondition ? ' AND ' + localThis.dataSourceCondition : '';
      condition += localThis.filterCondition ? ' AND ' + localThis.filterCondition : '';

      return condition;
    },
  },
  watch: {
    viewName(to:string, form:string){
      //Reload data if viewName is updated
      (this as any).getData();
    },
    dataSourceCondition(to:string, from:string){
      //Reload data if datasource condition is updated
      (this as any).getData();
    },
    selectedRows(to:any, from:any){
        (this as any).updateValue(to);
    },

  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),

    retrieveLookupByTyepCode(code: string) {
      return (this as any).$store.getters.getLookupsByTypeCode(code);
    },
    checkQuestion(row:any ,value: any, isDelete: any) {
      
      let localThis: any = this as any;
      let index_;
    
      var exists = localThis.selectedQuestions.some(function(field:any,index:any) {

          if( field === value){
            index_ = index; 
            return value;
          }
        });
      if (!exists && !isDelete) {
          localThis.selectedQuestions.push(value);
          localThis.selectedQuestionsRow.push(row);
          localThis.rows.CHECKED = true;
      }else{
          localThis.selectedQuestions.splice(index_,1);
          localThis.selectedQuestionsRow.splice(index_,1);
          localThis.rows.CHECKED = false;
      }
        this.$emit('AddQuestions', localThis.selectedQuestionsRow)  
        this.$emit('AddQuestionsFHPQ',row,value)  
    },

    AddQuestionsFHPQ(row:any,value:any){
      
      let localThis: any = this as any;
      let index_;
      
    
      var exists = localThis.selectedQuestions.some(function(field:any,index:any) {

          if( field === value){
            index_ = index; 
            return value;
          }
        });
      if (!exists ) {
          localThis.selectedQuestions.push(value);
          localThis.selectedQuestionsRow.push(row);
          localThis.rows.CHECKED = true;
      }else{
          localThis.selectedQuestions.splice(index_,1);
          localThis.selectedQuestionsRow.splice(index_,1);
          localThis.rows.CHECKED = false;
      }
       this.$emit('AddQuestions', localThis.selectedQuestionsRow)  
    },

    openAnswerForm(item:any){
          let localThis: any = this as any;
          
          localThis.answerText = item.ANSWER
          localThis.answerScore = item.ANSWER_SCORE
          localThis.selectedAnswerId = item.ANSWER_ID
          localThis.selectedQuestionId = item.QUESTION_ID
          localThis.questionType = item.QUESTION_TYPE
          localThis.belong_to_one_test = item.BELONG_TO_ONE_TEST
          if(item.QUESTION_TYPE_ID == 2){  //openquestion
             localThis.activation_answer= !localThis.activation_answer 
             localThis.activation_open_number = true;
             localThis.activation_number = false;
             localThis.activation_date = false ;
             localThis.activation_true_false = false;
          }else if(item.QUESTION_TYPE_ID == 1){  //multichoice question
             localThis.activation_multi_answers= !localThis.activation_multi_answers 
             localThis.getMultipleAnswer(localThis.selectedQuestionId)     
          }else if(item.QUESTION_TYPE_ID == 3){ //true false question  
             localThis.activation_answer= !localThis.activation_answer 
             
             if(item.ANSWER == 'true'){ 
                 localThis.answerText =true;
             }else{
                 localThis.answerText =false;
             }
             localThis.activation_true_false= true 
             localThis.activation_date = false;
             localThis.activation_open_number = false ;
          }else if(item.QUESTION_TYPE_ID==4){  //number question
                localThis.activation_answer= !localThis.activation_answer 
                localThis.activation_open_number = true;
                localThis.activation_number = true;
                localThis.activation_date = false ;
                localThis.activation_true_false = false;

          }else if(item.QUESTION_TYPE_ID==5){  //date question
               localThis.activation_answer= !localThis.activation_answer;
               let newValue:Date = (typeof localThis.answerText == 'string' ) ?  (String(new Date(localThis.answerText)) === 'Invalid Date' ?  new Date() :  localThis.answerText ) : new Date(); 
               localThis.answerText = newValue ;
               localThis.activation_date = true;
               localThis.activation_open_number = false ;
               localThis.activation_true_false = false;
          }

    },

    getMultipleAnswer(question_id: any){
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: 'PMS_QUESTION_ANSWER_MULTICHOICE',
        colList: localThis.tableColumns,
        whereCond: `QUESTION_ID = ${question_id}`,
        orderStmt: null
      };

      // Display loading bar
      localThis.tableIsLoading = true;
      localThis.multi_answers= []

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
             localThis.tableIsLoading = false;
             if (res.table_data) {
              res.table_data.forEach((item: any) => {
                 localThis.multi_answers.push(item.MULTI_ANSWER);
            });
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },

    AddMultiAnswers(answer: any){
      let localThis: any = (this as any);
      if(answer != ""){
            localThis.multi_answers.push(answer)
      }
    },
    getData() {
        
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: localThis.viewName,
        colList: localThis.tableColumns,
        whereCond: localThis.computedDatasourceCondition,
        orderStmt: localThis.orderCondition,
        itemNumber: !localThis.clientPagination ? localThis.tableOptions.itemsPerPage : undefined,
        skipNumber: !localThis.clientPagination ? ((localThis.tableOptions.page == 0 ? 0 :localThis.tableOptions.page-1) * localThis.tableOptions.itemsPerPage) : undefined,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID
      };

      // Display loading bar
      localThis.tableIsLoading = true;
      localThis.progress = true;

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis.progress = false;
          // Remove loading bar
          localThis.tableIsLoading = false;

          //Setup CRUD info
          localThis.crudInfo = res.crud_info;
          localThis.tableTitleKey = res.crud_info.tableTitleKey;

          //Manage pagination
          if(!localThis.clientPagination){
            localThis.rowNbr = res.item_count;
          }

          //Setup Header
          localThis.headers = res.columns.filter(
            (header: JrsHeader) => !header.is_hidden
          ).sort(
            (a:JrsHeader, b:JrsHeader) => {
              let order_A:number = a.list_order ? a.list_order : 1000;
              let order_B:number = b.list_order ? b.list_order : 1000;
              return order_A < order_B ? -1 : 1;
          });

          if(!localThis.hideActions || !!this.$scopedSlots['simple-table-item-actions']){
            localThis.headers.push({ text: "Actions", value: "action" });
          }
          localThis.pkName = res.columns.find(
            (header: JrsHeader) => header.is_pk
          ).value;
          //localThis.rows = res.table_data || [];
          
         if(res.table_data){
          res.table_data.forEach((item: any) => {
            let Question = {} as Question
            let checked = false;
            localThis.initialQuestions.forEach((quest:any)=>{ if(quest.QUESTION_ID == item.QUESTION_ID){checked = true}});
            Question.QUESTION_ID=item.QUESTION_ID;
            Question.QUESTION_TEXT = item.QUESTION_TEXT;
            Question.ANSWER = item.ANSWER;
            Question.ANSWER_SCORE = item.ANSWER_SCORE;
            Question.QUESTION_TYPE_ID=item.QUESTION_TYPE_ID;
            Question.QUESTION_TYPE = item.QUESTION_TYPE;
            Question.BELONG_TO_ONE_TEST = item.BELONG_TO_ONE_TEST;
            Question.FREQUENCY_OF_QUESTION=item.FREQUENCY_OF_QUESTION;
            Question.QUESTION_TYPE = item.QUESTION_TYPE
            Question.ANSWER = item.ANSWER;
            Question.CHECKED = checked;
            localThis.rows.push(Question)
          })
       

          let tmpFormFileds:Array<any> = res.columns
            .filter((header: any) => !header.hide_in_form)
            .map((field:any) => localThis.parseJrsFormField(field));
          localThis.formFields = localThis.setupSelectFields(tmpFormFileds, localThis.getCurrentOffice.HR_OFFICE_ID, localThis.fieldDatasourceConditions);
         }
        })
        .catch((err: any) => {
          localThis.progress = false;
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },


    getCompleteData() {
        
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: localThis.viewName,
        colList: localThis.tableColumns,
        whereCond: localThis.computedDatasourceCondition,
        orderStmt: localThis.orderCondition,
        itemNumber:null,
        skipNumber:null,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID
      };

      // Display loading bar
      localThis.tableIsLoading = true;

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
            
          // Remove loading bar
          localThis.tableIsLoading = false;

          //Setup CRUD info
          localThis.crudInfo = res.crud_info;
          localThis.tableTitleKey = res.crud_info.tableTitleKey;

          //localThis.rows = res.table_data || [];
          
          if(res.table_data){
             res.table_data.forEach((item: any) => {
             let Question = {} as Question
             let checked= false;
             localThis.initialQuestions.forEach((quest:any)=>{ if(quest.QUESTION_ID == item.QUESTION_ID){checked = true}});
             Question.QUESTION_ID=item.QUESTION_ID;
             Question.QUESTION_TEXT = item.QUESTION_TEXT;
             Question.ANSWER = item.ANSWER;
             Question.ANSWER_SCORE = item.ANSWER_SCORE;
             Question.QUESTION_TYPE_ID=item.QUESTION_TYPE_ID;
             Question.QUESTION_TYPE = item.QUESTION_TYPE;
             Question.BELONG_TO_ONE_TEST = item.BELONG_TO_ONE_TEST;
             Question.FREQUENCY_OF_QUESTION=item.FREQUENCY_OF_QUESTION;
             Question.QUESTION_TYPE = item.QUESTION_TYPE
             Question.ANSWER = item.ANSWER;
             Question.CHECKED = checked;
             localThis.completeRows.push(Question)
          })
          }
        }).catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },

    /**
     * Query DB for dataset qnd refresh rows without calculating again the headers and form definition
     */
    refreshData(){
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: localThis.viewName,
        colList: localThis.tableColumns,
        whereCond: localThis.computedDatasourceCondition,
        orderStmt: localThis.orderCondition,
        itemNumber: !localThis.clientPagination ? localThis.tableOptions.itemsPerPage : undefined,
        skipNumber: !localThis.clientPagination ? ((localThis.tableOptions.page == 0 ? 0 :localThis.tableOptions.page-1) * localThis.tableOptions.itemsPerPage) : undefined,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID
      };

      // Display loading bar
      localThis.tableIsLoading = true;
          
     

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          // Remove loading bar
          localThis.tableIsLoading = false;

          //Manage pagination
          if(!localThis.clientPagination){
            localThis.rowNbr = res.item_count;
          }
          
         // localThis.copyrowsBefore =  localThis.rows;
          localThis.completeRows.forEach((item:any) => {
               localThis.rows.forEach((quest:any) => {
                  if(item.QUESTION_ID == quest.QUESTION_ID){
                    item.CHECKED = quest.CHECKED;
                  }
               })
             
          })
          localThis.rows = [];
          if(res.table_data != undefined){
          res.table_data.forEach((item: any) => {
            let Question = {} as Question
            let check = false;
            localThis.completeRows.forEach((row:any) => {if(row.QUESTION_ID == item.QUESTION_ID){ check=row.CHECKED }});
            Question.QUESTION_ID=item.QUESTION_ID;
            Question.QUESTION_TEXT = item.QUESTION_TEXT;
            Question.ANSWER = item.ANSWER;
            Question.ANSWER_SCORE = item.ANSWER_SCORE;
            Question.QUESTION_TYPE_ID=item.QUESTION_TYPE_ID;
            Question.QUESTION_TYPE = item.QUESTION_TYPE;
            Question.BELONG_TO_ONE_TEST = item.BELONG_TO_ONE_TEST;
            Question.FREQUENCY_OF_QUESTION=item.FREQUENCY_OF_QUESTION;
            Question.QUESTION_TYPE = item.QUESTION_TYPE
            Question.ANSWER = item.ANSWER;
            Question.CHECKED = check;
            
            localThis.rows.push(Question)
          })
          }
        //   localThis.copyrowsAfter = localThis.copyrowsBefore;
         
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },

    AddUpdAnswer(answer_id:Number, answer:String,score:Number,question_id:Number,question_type: String){
         let localThis = this as any;
         let payLoadD = {} as payLoadDataAnswer;
         let check_ins_one=1;
         let actiontype: SqlActionType;
          
          
  
          if(answer_id != undefined){
              payLoadD.ANSWER_ID = answer_id;
          }else{
              payLoadD.ANSWER_ID = 0;
          }

          if(question_type == 'DATE'){
             let index = answer.indexOf( ":" );
             payLoadD.ANSWER=answer.substring(0,index-3);
          }else{
             payLoadD.ANSWER=answer;
          }

          payLoadD.ANSWER_SCORE=score;
             let saveData = {
                    question_id: question_id,
                    rows : payLoadD,
                    multi_answer: "",
                    question_type: question_type
                  };

          let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_ANSWER",
                actionType: SqlActionType.NUMBER_0,
                jsonPayload: JSON.stringify(saveData),
                userUid: this.getUserUid,
                officeId: this.getCurrentOffice.HR_OFFICE_ID
              };
              localThis
                .execSPCall(savePayload)
                .then((res: any) => {
                  localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
                  localThis.tableIsLoading = false;
                }).catch((err: any) => {
             
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message
                   
                  }); // Feedback to user
                });
                  localThis.activation_answer=!localThis.activation_answer;
                  localThis.refreshData(); // Refresh table data
                 

        },
    AddUpdMultiAnswer(answer_id:Number, answer:String,multi_answers:any,score:Number,question_id:number,question_type: String){
       let localThis = this as any;
         let payLoadD = {} as payLoadDataAnswer;
         let check_ins_one=1;
         let actiontype: SqlActionType;
         let payMultiAnswer = [] as any;
          
          
        
          if(answer_id != undefined){
              payLoadD.ANSWER_ID = answer_id;
          }else{
              payLoadD.ANSWER_ID = 0;
          }
          payLoadD.ANSWER=answer;
          payLoadD.ANSWER_SCORE=score;
          
          multi_answers.forEach((element: any) => {    
              let payLoadMultiAnswerQuestion= {} as payLoadMultiAnswer;  
              payLoadMultiAnswerQuestion.QUESTION_ID = question_id;
              payLoadMultiAnswerQuestion.MULTI_ANSWER = element;
              payMultiAnswer.push({
                QUESTION_ID: payLoadMultiAnswerQuestion.QUESTION_ID,
                MULTI_ANSWER: payLoadMultiAnswerQuestion.MULTI_ANSWER
              })
          });
             let saveData = {
                    question_id: question_id,
                    rows : payLoadD,
                    multi_answer: payMultiAnswer,
                    question_type: question_type
                  };
         
              let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_ANSWER",
                actionType: SqlActionType.NUMBER_0,
                jsonPayload: JSON.stringify(saveData),
                userUid: this.getUserUid,
                officeId: this.getCurrentOffice.HR_OFFICE_ID
              };
              localThis
                .execSPCall(savePayload)
                .then((res: any) => {
                  localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
                       localThis.tableIsLoading = false;
                }).catch((err: any) => {
             
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message
                   
                  }); // Feedback to user
                });
     
                  localThis.activation_multi_answers=!localThis.activation_multi_answers;
                  localThis.refreshData(); // Refresh table data

    },
    saveData(
      saveData: any,
      actionType: SqlActionType,
      formClosingFunc: Function,
      formValidateFunc: Function,
      formResetFunc?: Function
    ) {
      //Check form validity
      if (formValidateFunc()) {
         
        let localThis: any = this as any;
        let spName: string =
          actionType == SqlActionType.NUMBER_0
            ? localThis.crudInfo.create_sp
            : localThis.crudInfo.update_sp;

        //Add external data to payload
        
        if(localThis.savePayload){
          saveData = {
            external_data : localThis.savePayload,
            rows : saveData
          };
        }

        let savePayload: GenericSqlPayload = {
          spName: spName,
          actionType: actionType,
          jsonPayload: JSON.stringify(saveData),
          userUid: this.getUserUid,
          officeId: this.getCurrentOffice.HR_OFFICE_ID

        };

        localThis.execSPCall(savePayload)
          .then((res: any) => {
            localThis.showNewSnackbar({ typeName: res.status, text: res.message }); // Feedback to user
            formClosingFunc(); // Close ModalForm
            localThis.refreshData(); // Refresh table data

            // Reset new Object if actionType CREATE
            if(actionType == SqlActionType.NUMBER_0){
              localThis.ResetObject(localThis.newFormData);
            }
          })
          .catch((err: any) => {
            localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
          });
        }
    },

 
    deleteData() {
      let localThis: any = this as any;
       
      //Check if there is data to delete
       let payLoadD = {} as payLoadDataQuestion;
   
      if (localThis.deleteItem) {
        let spName: string = localThis.crudInfo.delete_sp;
        let deleteData:any = localThis.deleteItem;
        
          payLoadD.QUESTION_ID=localThis.deleteItem.QUESTION_ID;
          payLoadD.QUESTION_TEXT=localThis.deleteItem.QUESTION_TEXT;
          payLoadD.QUESTION_TYPE_ID = localThis.deleteItem.QUESTION_TYPE_ID;
          payLoadD.ANSWER_ID = localThis.deleteItem.ANSWER_ID;
          
        //Add external data to payload
        if(localThis.savePayload){
          deleteData = {
            external_data : localThis.savePayload,
            rows : payLoadD
          };
        }

        let deletePayload: GenericSqlPayload = {
          spName: spName,
          actionType: SqlActionType.NUMBER_2,
          jsonPayload: JSON.stringify(deleteData),
          userUid: this.getUserUid,
          officeId: this.getCurrentOffice.HR_OFFICE_ID
        };

        localThis.execSPCall(deletePayload)
          .then((res: any) => {
            localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
            localThis.toggleDeleteDialog(); // Close Dialog
            localThis.checkQuestion( localThis.deleteItem, localThis.deleteItem.QUESTION_ID,true)
            localThis.refreshData(); // Refresh table data
          }).catch((err:any) => {
            localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
          });
      }
    },

    getQuestionType(){
      let localThis: any = this as any;
      
  
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_QUESTION_TYPE",
        colList: null,
        whereCond: null,
        orderStmt: null
      };
      
      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
   
          //Setup data
          if (res.table_data) {
              res.table_data.forEach((item: any) => {
                 let payLoadD = {} as payLoadQuestionType;
                 payLoadD.QUESTION_TYPE_ID = item.QUESTION_TYPE_ID;
                 payLoadD.QUESTION_TYPE = item.QUESTION_TYPE;
                 localThis.question_types.push({
                    QUESTION_TYPE_ID: payLoadD.QUESTION_TYPE_ID,
                    QUESTION_TYPE: payLoadD.QUESTION_TYPE
                  })
            });
          
          }
          
        }
        
      );
    },

    saveNewQuestion(question_id:any,question_text:any,question_type_id:any){
        let localThis = this as any;
         let payLoadD = {} as payLoadDataQuestion;
         let check_ins_one=1;
         let actiontype: SqlActionType;
          
          
          payLoadD.QUESTION_ID=question_id;
          payLoadD.QUESTION_TEXT=question_text;
          payLoadD.QUESTION_TYPE_ID=question_type_id;
          
          if(question_text != "" && question_type_id != null){
                      let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_QUESTION",
                actionType: SqlActionType.NUMBER_0,
                jsonPayload: JSON.stringify(payLoadD),
                userUid: this.getUserUid,
                officeId: this.getCurrentOffice.HR_OFFICE_ID
                
              };
              localThis
                .execSPCall(savePayload)
                .then((res: any) => {
                  localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
                  localThis.refreshData();
                }).catch((err: any) => {
             
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message
                   
                  }); // Feedback to user
                });
              localThis.getData(); // Refresh table data
          }

        },
    toggleDeleteDialog(deleteItem?: any) {
      (this as any).deleteDialog = !(this as any).deleteDialog;
      (this as any).deleteItem = deleteItem ? deleteItem : {};
    },
    updateValue(newVal:any){
        // Emit update of v-model to parent component.
        (this as any).$emit('input', newVal);
    },
    /**
     * Update the tabel options.
     * @param opt - new options
     */
    updateOptions(opt:any){
      let localThis:any = this as any;
      
      // Skip the first update
      if(localThis.firstOptionUpdateDone){
        Object.assign(localThis.tableOptions, opt);

        // Ordering
        let ordering:string = '';
        if(opt.sortBy.length > 0){
          let colsOrder:Array<string> =  opt.sortBy
            .map( (colName:string, index:number) => `${colName} ${opt.sortDesc[index] ? 'DESC' : ''}` );
          ordering = colsOrder.join(',');
        }
       // localThis.orderCondition = ordering;
        
        //Refresh data-source
        if(!localThis.clientPagination){
          localThis.refreshData();
        }
      }else{
        localThis.firstOptionUpdateDone = true;
      }
    },
    /**
     * Handle filter condition changes.
     * @param filter - new query condition
     */
    updateWhereCondition(filter:string){
      let localThis:any = this as any;
      localThis.filterCondition = filter;

      //Refresh data-source
      if(!localThis.clientPagination){
         localThis.refreshData();
      }
    },
    /**
     * Reset all properties of the given object.
     * @param targetObject - Object to reset
     */
    ResetObject(targetObject:any){
      //Reset value of the form properties
      for (const property in targetObject) {
        targetObject[property] = null;
      }
    },
    test() {
     // console.log("Test...");
    }
  },
  mounted() {
    let localThis: any = this as any;
    
    if(localThis.action == 'update'){
      localThis.initialQuestions.forEach((item:any)=>{localThis.selectedQuestions.push(item.QUESTION_ID)});
      localThis.initialQuestions.forEach((item:any)=>{localThis.selectedQuestionsRow.push(item)});
    }

    localThis.getData();
    localThis.getCompleteData();
    localThis.getQuestionType();
       
  }
});
</script>

<style scoped>
</style>