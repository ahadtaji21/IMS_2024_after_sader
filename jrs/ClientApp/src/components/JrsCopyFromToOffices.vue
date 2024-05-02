<template>
  <v-row justify="center">
    <v-dialog
      ref="dialog"
      v-model="modalCopy"
      :return-value.sync="fieldValue"
      persistent
      max-width="450"
    >
      <template v-slot:activator="{ on, attrs }">
                    <v-btn color="secondary lighten-2"   v-bind="attrs"
                    v-on="on" class="grey--text text--darken-3" >
              <v-icon>mdi-content-copy</v-icon>Copy from one office
            </v-btn>
      </template>
      <v-card>
            <v-card-title class="primary lighten-2 black--text">
                      data copy procedure
        </v-card-title>
        <v-card-text>
            <v-row>
             <v-col
          
              >
              <v-alert
         outlined
      type="warning"
      prominent
      border="left"
      color="primary"
    >
    Copy data <br /> from -> : {{selectedOffice}}  <br /> to -> :  {{getCurrentOffice.HR_OFFICE_LEGAL_NAME}}

    </v-alert>


           </v-col>
           </v-row>
              <v-row>
             <v-col
              
              >
              <v-select
                    :items="offices"
                    item-key="HR_OFFICE_ID"
                    item-text="HR_OFFICE_LEGAL_NAME"
                    v-model="officeFrom"
                    return-object
                    dense
                    outlined
                    label="select office from which copy data"
                    class="px-7 mt-3 text-capitalize"
             ></v-select>

           </v-col>
           </v-row>
        </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
                  <v-btn
                            color="secondary darken-1"
                            text
                            @click="modalCopy = false"
                            >X {{ $t('general.close') }}</v-btn>
                   <v-btn
                    text
                    color="primary"
                    @click="copy()"
                >{{ $t('general.copy-and-save') }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

  </v-row>
</template>
<script lang="ts">
    import Vue from 'vue'
    import { mapActions, mapGetters } from "vuex";
    import { GenericSqlSelectPayload  } from "../axiosapi/api";

    export default Vue.extend({
        data(){
            return {
                modalCopy: false,
                officeFrom: [],
                offices: [],
                selectedOffice: ''
            }
        },
        computed: {
              ...mapGetters({
                getCurrentOffice: "getCurrentOffice"
                }),
            fieldValue: {
                get(){
                    let localThis:any = this as any;
                    return localThis.value;
                },
                set(newVal:any){
                    //(this as any).fiedlValue = newVal;
                    (this as any).updateValue(newVal);
                }
            },
        },
        watch:{
             officeFrom(to: any, from: any) {
                    let localThis: any = this as any;
                    if(to)
                    localThis.selectedOffice = to.HR_OFFICE_LEGAL_NAME ;            
                }


        },
        methods:{
            ...mapActions("apiHandler", {
                getGenericSelect: "getGenericSelect",
                copyConfigFromToOffices: "copyConfigFromToOffices"
            }),
              ...mapActions({
            showNewSnackbar: "showNewSnackbar"
            }),
            
            getAllOffice(){
                    let localThis: any = this as any;

                    let selectPayload: GenericSqlSelectPayload = {
                        viewName: "HR_VI_OFFICE",
                        colList: null,
                        whereCond: `HR_OFFICE_ID != ${localThis.getCurrentOffice.HR_OFFICE_ID}`,
                    };

                    return this.getGenericSelect({ genericSqlPayload: selectPayload })
                        .then((res: any) => {
                        //Setup data
                        if (res.table_data) {
                            res.table_data.forEach((item: any) => {
                                 localThis.offices.push(item);
                            });
                        }
                        }).catch((err:any)=>{
                                    console.error(err);
                        });
            },
            updateValue(newVal: number) {
                (this as any).$emit("input", newVal);
            },

            copy(){
                    let localThis: any = this as any;
                    const tableName:string = localThis.tableName;
                    const columnNameOffice:string = localThis.columnNameOffice;
                    const idOfficeFrom:number = localThis.officeFrom.HR_OFFICE_ID;
                    const idOfficeTo: number = localThis.getCurrentOffice.HR_OFFICE_ID;
                    const columnNamePK: string = localThis.columnNamePK;
                    if(localThis.officeFrom.HR_OFFICE_ID){
                    localThis.copyConfigFromToOffices({
                        tableName        :tableName, 
                        columnNameOffice :columnNameOffice, 
                        idOfficeFrom     :idOfficeFrom, 
                        idOfficeTo       :idOfficeTo, 
                        columnNamePK : columnNamePK
                        })
                        .then((res:any) => {
                          localThis.showNewSnackbar({ typeName: "success", text: res }); // Feedback to user
                          
                          localThis.modalCopy = false;
                          this.$emit("refreshData");
                        })
                        .catch((err: any) => {
                         localThis.showNewSnackbar({ typeName: "error", text: err });
                        });
                    }
            },

            
            
        },
        mounted(){
             let localThis: any = this as any;
             localThis.getAllOffice()
        },
        props:{
            tableName:{
                type: String,
                required: true
            },
            columnNameOffice:{
                type: String,
                required: true
            },
            columnNamePK:{
                type: String,
                required: true
            }

        }
     
        
    })
</script>

<style scoped>

</style>