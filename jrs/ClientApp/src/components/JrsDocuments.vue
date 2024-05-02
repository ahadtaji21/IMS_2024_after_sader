<template>
    <div class="d-flex">
          <v-row justify="center" >
              <!--show-size-->
        <v-file-input
              :truncate-length="truncate_length"
              counter
              :accept="acceptExtensions"
              :label="label"
              chips
              :rules="extensionRules"
              v-model="fileUpload"
              :persistent-hint='true'
              :hint="'Accept Ext. :'+acceptExtensions"
              @change="changeValue"
              @click:clear = "clearField"
      >

        <template slot="append">
             <!--<v-icon :size="30"  color="primary" class="--darken-1 mx-1"  @click="UploadFileBlobAzure()">mdi-cloud-upload</v-icon> -->
             <v-icon  :size="30"  color="secondary" class="--darken-1 mx-1"  v-if="pathFileRoot != ''" @click="DownloadFileBlobStorage()">mdi-cloud-download</v-icon>
  </template>
      </v-file-input>
      </v-row>
    </div>
</template>


<script lang="ts">
    import Vue from 'vue'
    import { mapActions, mapGetters } from "vuex";
    import { GenericSqlSelectPayload  } from "../axiosapi/api";
    import UtilMix from "../mixins/UtilMix";
    import {IapiBlobStorageDownload} from "../store/apiHandler"
    import { translate } from '../i18n';

    export default Vue.extend({
        props:{
            value: {
                type: [Number,String, Object, Array],
                required: false
            },
            label: {
                type: String,
                required: false,
                default: null
            },
            tableNameToSaveDocument:{
                type: String,
                required: false,
                default: ""
            },
            columnNameToSaveDocument:{
                type: String,
                required: false,
                default: ""
            },
            truncate_length:{
                type: String,
                required: false,
                default: "15"
            },
            acceptExtensions:{
                type: String,
                required: false,
                default: ".pdf, .xls, .xlsx, .doc, .docx, .jpg, .jpeg, .png, .gif"
            },
            FolderSaveDocument:{
                type: String,
                required: false,
                default: "General"
            },
            documentTypeId:{
                type: Number,
                required: false,
                default: 2
            },
            OfficeCode:{
                type: String,
                required: false,
                default: null
            },
            is_required:{
                type: Boolean,
                required: false,
                default: false
            }

        },
        data(){
            return {
                editMode: false,
                fileUpload: [],
                uniqueFileName: '',
                pathFileRoot: "",
                nameContainer: "",
                idOffice: 0,
                guidFile : "",
                fileName: "",
                currentExtension: "",
                acceptExtensionStr: "",
                rules: [],
                id_document: 0
            }
        },
        mixins:[UtilMix],
        computed:{
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
            extensionRules(){
                let localThis:any = this as any;
                let rules = [];
                

                // Check if the decimal precision is correct
                
                 const extensionRule = (v:File) => { 
                    if(v && v.name){    
                        let extension = v.name.toLowerCase().split('.').pop();
                     
                        if(localThis.acceptExtensions.includes(extension)){
                            return true
                        }else{
                            return  `File hasn't the correct extension (Accept : ${localThis.acceptExtensions})`
                        }
                    }else{
                        return  true
                    }
                 }
                rules.push(extensionRule);
                if(localThis.is_required){
                     rules.push( (v:any) => !!v || translate('error.required-field').toString())
                }

                return rules;
            },
        },
        methods: {
            ...mapGetters({
              getCurrentOffice: 'getCurrentOffice',
              activeModule: "getActiveModule",
              getUserUid:"getUserUid"

            }),
              ...mapActions({
            showNewSnackbar: "showNewSnackbar"
            }),
            ...mapActions("apiHandler", {
                getGenericSelect: "getGenericSelect",
                uploadBlobStorageFile: "uploadBlobStorageFile",
                uploadOverwriteBlobStorageFile: "uploadOverwriteBlobStorageFile",
                downloadBlobStorageFile: "downloadBlobStorageFile"
            
            }),

            changeValue(arg:any){
                 let localThis:any = this as any;
                 this.$emit("filechange", { "File": arg ? arg : null ,"ID":   localThis.id_document})
            },

            /**
             * Emit value changes to parent component.
             */
            updateValue(newVal: any) {
                (this as any).$emit("input", newVal);
            },
            /**
             * Clear field data.
             */
            clearField(){
                let localThis:any = this as any;
                localThis.fileUpload = null;
                localThis.id_document = null;
                localThis.pathFileRoot = "";
                localThis.rules = [];
            },
            /**
             * Get table data.
             */
           getData(DocumentId:any) {
                let localThis: any = this as any;
                
           
                let selectPayload: GenericSqlSelectPayload = {
                    viewName: "ST_DOCUMENT",
                    colList: null,
                    whereCond: `ID = ${DocumentId}`,
                    orderStmt: "ID",
                    officeId: localThis.getCurrentOffice.HR_OFFICE_ID
                };

                return localThis.getGenericSelect({ genericSqlPayload: selectPayload })
                    .then((res: any) => {     
                        
                               return res.table_data[0];
                    })
                    .catch((err: any) => {
                              localThis.showNewSnackbar({ typeName: "error", text: err });
                    });
            },

            getOfficeIdFromCode(officeCode:any){
                let localThis: any = this as any;
                let selectPayload: GenericSqlSelectPayload = {
                    viewName: "HR_VI_OFFICE",
                    colList: null,
                    whereCond:`HR_OFFICE_CODE = ${officeCode}`,
                    orderStmt: null
                };
                
                return localThis.getGenericSelect({ genericSqlPayload: selectPayload })
                    .then((res: any) => {  

                               localThis.idOffice = res.table_data[0].HR_OFFICE_ID;
                            
                    })
                    .catch((err: any) => {
                        localThis.showNewSnackbar({ typeName: "error", text: err });
                    });
            },



        DownloadFileBlobStorage(){
                         let localThis: any = this as any;
                           localThis.getData(localThis.value)
                                .then( (res:any) => {
                                        if(res){
                                            localThis.nameContainer = res.ST_CONTAINER_NAME;
                                            localThis.pathFileRoot = res.ST_PATH_ROOT;
                                            localThis.currentExtension = res.ST_EXTENSION;
                                            localThis.fileName = res.ST_FILE_NAME;     

                                            let payload: IapiBlobStorageDownload = {
                                            nameContainer: localThis.nameContainer,
                                            pathRoot:  localThis.pathFileRoot,
                                            extension: localThis.currentExtension.replace('.',''),
                                            fileName: localThis.fileName
                                                };
                                                
                                            localThis
                                                .downloadBlobStorageFile(payload)
                                                .then((res:any) => {      
                                                    var sampleArr = localThis._base64ToArrayBuffer(res,payload.extension);
                                                    localThis.saveByteArray(localThis.fileName, sampleArr);    
                                                    })
                                                .catch((err: any) => {
                                                    localThis.showNewSnackbar({ typeName: "error", text: err });
                                            });
                            }        
                     })
                         
                }

            
        },
        mounted() {
            // GET INFO DOCUMENT FROM SPECIFIC ID OF DOCUMENT (FROM ST_DOCUMENT)
            let localThis: any = this as any;
            if(localThis.value){
                  localThis.id_document = localThis.value;
                  localThis.getData(localThis.value)
                        .then( (res:any) => {
                                if(res){
                                    localThis.nameContainer = res.ST_CONTAINER_NAME;
                                    localThis.idOffice = res.ST_OFFICE_ID;
                                    localThis.uniqueFileName = res.ST_UNIQUE_FILE_NAME;
                                    localThis.pathFileRoot = res.ST_PATH_ROOT;
                                    localThis.currentExtension = res.ST_EXTENSION;
                                    localThis.guidFile = res.ST_GUID;
                                    localThis.fileName = res.ST_FILE_NAME;
                                    localThis.fileUpload = new File([""], res.ST_FILE_NAME);     
                                         
                                }
                                 localThis.changeValue(null)
                            });
             }else{
                     localThis.id_document = null;
                     localThis.changeValue(null)
              }
        }

        
    
    })
</script>
