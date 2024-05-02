<template>
    <div>
        <!-- QUESTION -->
        <v-card elevation="3"   shaped class="mx-auto">
        <div v-if="field.QUESTION_TYPE=='OPENQUESTION' || field.QUESTION_TYPE=='DATE' || field.QUESTION_TYPE=='NUMBER'">
        <v-text-field
            shaped
            :value ="field.QUESTION_TEXT"
            field="Question"
            readonly
            filled
          
        ></v-text-field>

          <!-- TEXT QUESTION OPEN QUESTION -->
        <v-text-field
            :value="ForEvaluation  ? ''  : field.ANSWER"
            :disabled="ForEvaluation  ? false  : true"
        ></v-text-field>
        </div>
        </v-card>
        <v-card elevation="3" shaped class="mx-auto">
        <div v-if="field.QUESTION_TYPE=='TRUEFALSE'">
        <v-text-field
            shaped
            :value ="field.QUESTION_TEXT"
            field="Question"
            readonly
            filled
           
        ></v-text-field>
        <!-- TEXT TRUE FALSE ANSWER-->
         <v-radio-group
            class="mt-0 ml-1 mb-1"
            v-model="answer_checkbox"
            hide-details
            :disabled="ForEvaluation  ? false  : true"
          >
            <v-radio
              value="answer_checkbox_true"
              label="True"
              v-model="ok_false"
            ></v-radio>
            <v-radio
              value="answer_checkbox_false"
              label="False"
              v-model="ok_true"
              
            ></v-radio>
          </v-radio-group>
    
        </div>
         </v-card>

        <v-card elevation="3"   shaped class="mx-auto">
        <div v-if="field.QUESTION_TYPE=='MULTICHOICE'">
        <v-text-field
            shaped
            :value ="field.QUESTION_TEXT"
            field="Question"
            readonly
            filled
           
        ></v-text-field>
        <!-- TEXT TRUE FALSE ANSWER-->
         <v-radio-group
            class="mt-0 ml-1 mb-1"
            v-model="answer_checkbox"
            hide-details
            v-for="(i,item) in field.MULTI_ANSWER"
            :key="i.MULTI_ANS+i.CORRECT_ANS"
            :disabled="ForEvaluation  ? false  : true"
          >
        
             <v-radio
              v-if="item+1 == random_multi_answer"
              :value="i.CORRECT_ANS"
              :label="i.CORRECT_ANS"
            ></v-radio>
            <v-radio
              :value="i.MULTI_ANS"
              :label="i.MULTI_ANS"
            ></v-radio>
            
          </v-radio-group>
       
        </div>
         </v-card>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { translate } from '../i18n';

    export default Vue.extend({
        components:{
           
          
        },
        props:{
            value:{
                type: [String, Number, Boolean, Array, Object, Date, Function, Symbol]
            },
            field:{
                type: Object,
                required: true
            },
            ForEvaluation:{
                type: Boolean,
                required: false
            }
           
        },
        data(){
            
            return {
                // fieldValue: null
                showPasswords: false,
                fieldAnswer: "",
                falsevalue: 0,
                truevalue: 1,
                answer_open_question: ""
                           
            }
        },
        computed:{
            fieldValue: {
                get(){
                    return (this as any).value;
                    
                },
                set(newVal:any){
                    //(this as any).fiedlValue = newVal;
                    (this as any).updateValue(newVal);
                }
            },
            ok_false:{
                    get(){
                         
                     let localThis:any = this as any;
                     let ok=false;
                     if(!localThis.ForEvaluation){
                    
                        if(localThis.field.ANSWER=='false'){
                           ok= !localThis.field.ANSWER
                        }else{
                           ok= localThis.field.ANSWER
                        }
                    }
                    return  ok;
                
                    }
            },
            ok_true:{
                 get(){
                      
                    let localThis:any = this as any;   
                    let ok=true;   
                    if(!localThis.ForEvaluation){
                        if(localThis.field.ANSWER=='true'){
                            ok= !localThis.field.ANSWER
                        }else{
                            ok= localThis.field.ANSWER
                        }   
                    }   
                    return  ok;
                 }
            },
            answer_checkbox:{
                 get(){
                      let localThis:any = this as any;
                    return  localThis.ForEvaluation ? '' : localThis.field.ANSWER
                }
               
            },

            random_multi_answer:{
                  get(){
                       let localThis:any = this as any;
                    return  Math.floor(Math.random() * localThis.field.MULTI_ANSWER.length) + 1 
                }
               
            },

       
            
            requiredTextFieldRule(){
                return [ (v:any) => !!v || translate('error.required-field')]
            },
            /**
             * Number of decimal values allowed for Number type field.
             */
            numberDecimalPrecision(){
                let localThis:any = this as any;
                let fieldConfigs = localThis.field.additional_config ? localThis.field.additional_config : {};
                
                return fieldConfigs.decimalPrecision ? fieldConfigs.decimalPrecision : 0;
            },
            /**
             * Number of character allowed in text and password type fields.
             */
            textCounterValue(){
                let localThis:any = this as any;
                let fieldConfigs = localThis.field.additional_config ? localThis.field.additional_config : {};
                
                let fieldCounter:number = localThis.field.counter ? localThis.field.counter : undefined;
                let configCounter:number = fieldConfigs.counterValue ? fieldConfigs.counterValue : undefined;

                return fieldCounter || configCounter || false;
            }
        },
        methods: {
            /**
             * Emit value changes to parent component.
             */
            updateValue(newVal: any) {
                let localThis:any = this as any;

                // Not multi "select like" fields
                let isSelect = localThis.field.type == 'select' && !localThis.field.multiple;
                let isAutoSelect = localThis.field.type == 'auto' && !localThis.field.multiple;

                if(isSelect || isAutoSelect){
                    let selectValue:any = localThis.calcSelectValue(newVal);
                    localThis.$emit("input", selectValue);
                }else{
                    localThis.$emit("input", newVal);
                }
            },
        

            RandomCorrectMultiAnswer(random: Boolean){
                 let localThis:any = this as any;
                 if(random==false){
                    localThis.random_multi_answer = true;
                    return true;
                 }else{
                     return false;
                 }
            },

            setAnswerFalse(value: any){
                let localThis : any = (this as any)
                localThis.answer_checkbox = value;
            },
            /**
             * Emit the full objet to parent component.
             */
            emitFullObject(obj:any){
                let emitPayload:any = { holderName: (this as any).field.lookupName, objectValue: obj };
                (this as any).$emit("changeFullObject", emitPayload);
            },
            /**
             * Return the select value recovedere from the full object.
             */
            calcSelectValue(a:any){
                (this as any).emitFullObject(a);
                return a ? a[(this as any).field.itemKey] : undefined;
            },

           /* mounted(){
                 let localThis : any = (this as any)
                  
                 if(localThis.ForEvaluation){
                     localThis.answer_open_question = ""
                 }else{
                     localThis.answer_open_question = localThis.field.ANSWER
                 }


            }*/

         
        },
    })


</script>

<style scoped>

</style>