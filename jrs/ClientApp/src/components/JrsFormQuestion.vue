<template>
    <v-form
        v-model="formIsValid"
        ref="form"
        lazy-validation   
    >
        <div v-if="formData.length > 0">   
           <!-- <v-row v-for="(item, i) in formData" :key="i">
                    <v-col>-->
              <v-row v-for="(row, i) in getFieldMatrixForTab(formData)" :key="'ROW-'+i">
                    <v-col v-for="(field, j) in getFieldMatrixForTab(formData)[i]" :key="'COL-'+j">
                        <jrs-field-question
                            :field="field" 
                            :ForEvaluation="ForEvaluation"
                            ></jrs-field-question>
                    </v-col>
                </v-row>  
        </div>
    </v-form>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { translate } from "../i18n";
    import { JrsFormField, JrsFormFieldQuestion, JrsFormTab, JrsFormFieldSelect } from '../models/JrsForm';
    import JrsFieldQuestion from './JrsFieldQuestion.vue';

    export default Vue.extend({
        props:{
            /**
             * List of fields for the form. Items bust be of thipe "JrsFormField" or one of the extensions.
             */
            formFields:{
                type:  Array as () => JrsFormFieldQuestion[],
                required: false
            },
            /**
             * Object with te data of the form. If the form is for new item, "formData" must have the porperties but "empty".
             */
            formData: {
                type: Array,
                required: true
            },

            ForEvaluation:{
                type: Boolean,
                required: false
            },
            /**
             * Title of the form.
             */
            formTitle:{
                type: String,
                required: false,
                default: undefined
            },
            /**
             * Subtitle of the form.
             */
            formSubtitle:{
                type: String,
                required: false,
                default: undefined
            },
            /**
             * Number of columns fo the form.
             * Excepted values: {1, 2, 3}
             */
            nbrOfColumns:{
                type: Number,
                required: false,
                default: 2,
                validator: function (value:any) {
                    // The value must match one of these values
                    return [1,2,3,4].indexOf(value) !== -1
                }
            }
        },
        components: {
            'jrs-field-question': JrsFieldQuestion
        },
        data(){
            return {
                formIsValid: false,
                formTabsModel: null,
                defaultTabCode: 'MAIN'
            }
        },
        computed:{
            /**
             * Fields with translated labels and hints.
             */
            translatedFields(){
                   return this.formFields
            },
            /**
             * Matrix of the fields divided in rows and columns.
             */
            fieldsMatrix(){
                let localThis:any = this as any;
                let breakpoint = localThis.$vuetify.breakpoint.name;

                let list = (this as any).translatedFields;
                let width = (this as any).nbrOfColumns;

                // Depending on screensize, limit the number of field in a row
                if(breakpoint == 'xs' || breakpoint == 'sm'){
                    width = 1;
                }

                if(!list || !width){
                    return undefined;
                }

                return (this as any).listToMatix(list, width);
            }
        },
        methods:{
            /**
             * Convert a list to a matrix of a given size.
             * 
             * @param list the list that must be converted
             * @param elementPerSubarray size of the sub arrays in the matrix
             * @returns A matrix of with == "elementPerSubarray" containing the elements of "list"
             */
            listToMatix(list:Array<any>, elementsPerSubarray:number){
                let matrix:Array<any> = [];
                let i:number , k:number;

                for( i=0, k=-1; i<list.length; i++){
                    if(i % elementsPerSubarray === 0){
                        k++;
                        matrix[k] = [];
                    }
                    matrix[k].push(list[i]);
                }
                return matrix;
            },
            /**
             * Setup "holder" property in formData to hold the default value of a select field.
             * 
             * @param field - select filed that requires default value
             */
            setupSelectDefaultValue(field:JrsFormFieldSelect){
                let lookupItemKey = field.itemKey;
                let fieldValue = (this as any).formData[field.name];
                // var def:Array<any> = [];
                // def = field.selectItems.find( (item:any) => item[lookupItemKey] == fieldValue );

                //Multi select override. Expects formatted object from server
                const lookupName:string =field.lookupName ? field.lookupName : "selectHolder_" + field.name;
                if(field.multiple == true && fieldValue){
                    (this as any).formData[lookupName] = (this as any).formData[field.name];
                }else if(!field.multiple){
                    (this as any).formData[lookupName] = field.selectItems.find( (item:any) => item[lookupItemKey] == fieldValue );
                }

            },
            /**
             * Get distinct form tab codes.
             * 
             * @param fields - array of JrsFormFields from which to extract the distinct tabs of the form
             */
            getFormTabs(fields:Array<JrsFormField>){
                let distinctTabs:Array<JrsFormTab> = [];
                let defaultTab:JrsFormTab = { tabCode: (this as any).defaultTabCode, tabName: (this as any).defaultTabCode };

                distinctTabs = fields.reduce( (distinctList:any, curr:any) => {
                    let newTab:JrsFormTab = Object.assign({}, defaultTab);

                    // If field has tab defined
                    if( curr.tabCode ){
                        newTab.tabCode = curr.tabCode;
                        newTab.tabName = curr.tabTranslationKey != undefined ? translate(curr.tabTranslationKey).toString() : newTab.tabCode;
                    }

                    // If newTab is not in list
                    if( distinctList.findIndex( (item:JrsFormTab) => item.tabCode == newTab.tabCode ) == -1 ){
                        // If MAIN tab the it must be first
                        if(newTab.tabCode == (this as any).defaultTabCode){
                            distinctList.unshift({
                                tabCode: newTab.tabCode,
                                tabName: newTab.tabName
                            });
                        }else{
                            distinctList.push({
                                tabCode: newTab.tabCode,
                                tabName: newTab.tabName
                            });
                        }
                    }

                    return distinctList;
                }, distinctTabs);

                return distinctTabs;

            },
            /**
             * Get a field matrix for a given TabCode.
             * 
             * @param tabCode - TabCode for which to recover the fields
             * @param fields - Array of the JrsFormFields to filter by the tabCode
             */
            getFieldMatrixForTab(fields:any){
                
                let localThis:any = this as any;


                let width = (this as any).nbrOfColumns;
                
                // Depending on screensize, limit the number of field in a row
                
                if(!fields || !width){
                    return undefined;
                }
                  
                return (this as any).listToMatix(fields, width);
            },
            /**
             * Validate form fields.
             */
            validateForm(){
                return (this as any).$refs.form.validate();
            },
            /**
             * Reset form fields.
             */
            resetForm(){
                let localThis:any = this as any;

                try{
                    //Clear formData object
                    for (var propName in localThis.formData){
                        if (localThis.formData.hasOwnProperty(propName)){
                            delete localThis.formData[propName];
                        }
                    }
                }catch(err){
                    console.log('Encountered error: ', err);
                }

                return (this as any).$refs.form.reset();
            },
            /**
             * Update select holder field.
             */
            updateSelectHolder(payload:any){
                let localThis:any = this as any;
                let {holderName, objectValue} = payload;
                localThis.formData[holderName] = objectValue;
          

            }
        }
    })
</script>

<style scoped>

</style>