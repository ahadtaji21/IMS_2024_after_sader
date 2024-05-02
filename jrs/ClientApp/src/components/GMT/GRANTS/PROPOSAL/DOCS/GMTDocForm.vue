<template>
  <v-card>
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <v-card>
      <v-container fluid>
        <v-form ref="myForm" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-document
                v-model="fieldValue"
                :documentTypeId="typeIdDocument"
                :OfficeCode="officeCodeDocument"
                :FolderSaveDocument="folderSaveDocument"
                :is_required="true"
                :label="$t('datasource.gmt.grant.gt-doc-supp-docs')"
                @filechange="emitfilechange"
              >
              </jrs-document>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.grant.gt-doc-description')"
                v-model="description"
                :counter="100"
              ></v-text-field>
            </v-col>
          </v-row>

          <v-card-actions>
            <v-btn color="secondary darken-1" text @click="closeEdit()"
              >X {{ $t("general.close") }}</v-btn
            >
            <div v-if="!onlyRead">
              <v-btn color="primary" @click="saveDocument(newFormData)">{{
                $t("general.save")
              }}</v-btn>
            </div>
          </v-card-actions>
        </v-form>
      </v-container>
    </v-card>
  </v-card>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters } from "vuex";
import { Configuration } from "../../../../../axiosapi";
import { translate, i18n } from "../../../../../i18n";
//import SearchTable from "./FUNDED/GMTGrantProjectsSearch.vue";
import DocDetails from "./GMTDocForm.vue";
import { EventBus } from "../../../../../event-bus";
import { mapActions } from "vuex";
import UtilMix from "../../../../../mixins/UtilMix";
import FormMixin from "../../../../../mixins/FormMixin";
import JrsDocument from "../../../../JrsDocuments.vue";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";
export default Vue.extend({
  props: {
    selectedGrantId: {
      type: Number,
      required: true,
    },
    acceptExtensions: {
      type: String,
      required: false,
      default: ".pdf, .xls, .xlsx, .doc, .docx, .jpg, .jpeg, .png, .gif",
    },
    docDescription: {
      type: String,
      required: false,
      default: "",
    },
    docFileName: {
      type: String,
      required: false,
      default: "",
    },

    documentTypeId: {
      type: Number,
      required: false,
      default: 6,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
    gmtDocId: {
      type: Number,
      required: false,
      default: 0,
    },
    imsID: {
      type: Number,
      required: false,
      default: 0,
    },
  },
  components: {
    // "search-table": SearchTable,
    "jrs-document": JrsDocument,
  },
  mixins: [FormMixin, UtilMix],
  data() {
    return {
      formValid: {},
      filePayLoad: {},
      newFormData: {},
      showWait: false,
      typeIdDocument: null,
      description: "",
      folderSaveDocument: "GMT",
      officeCodeDocument: "",
      fieldValue: 0,
    };
  },
  beforeMount() {
    let localThis: any = this as any;
    if (localThis.gmtDocId != 0) {
      localThis.fieldValue = localThis.gmtDocId;
      localThis.description = localThis.docDescription;
      localThis.filename = localThis.docFileName;
    }
  },
  mounted() {
    let localThis: any = this as any;
    localThis.officeCodeDocument = localThis.getCurrentOffice.HR_OFFICE_CODE;
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    closeEdit() {
      let localThis: any = this as any;
      localThis.$emit("closeEdit");
    },
    // saveDocument() {
    //   let localThis: any = this as any;
    //   localThis.$emit("closeEdit");
    // },
    emitfilechange(a: any) {
      let localThis: any = this as any;
      localThis.filePayLoad = {
        holderName: "fileHolder_SupportDocument", //+ (this as any).field.name,
        objectValue: a,
      };
      // localThis.$emit("changeFullObject", emitPayload);
    },
    async saveDocument(saveData: any) {
      let localThis: any = this as any;
      localThis.showWait = true;

      let return_document_component = localThis.filePayLoad.holderName;
      // let id_document = localThis.filePayLoad.id_document;
      let document_id = localThis.gmtDocId;

      if (return_document_component && localThis.filePayLoad.objectValue.File) {
        let office_code_config = localThis.getCurrentOffice.HR_OFFICE_CODE;
        document_id = await localThis.UploadFileBlobAzureFromForm(
          localThis.filePayLoad.objectValue.ID,
          localThis.getCurrentOffice.HR_OFFICE_CODE,
          localThis.folderSaveDocument,
          localThis.documentTypeId,
          localThis.filePayLoad.objectValue.File
        );

        // The classic saving whitout field "DOCUEMNT"
        if (document_id == undefined || document_id == 0 || document_id == "") {
          localThis.showNewSnackbar({
            typeName: "error",
            text: "Error uploading the File",
          });
          localThis.showWait = false;
          return;
        } else {
          localThis.showWait = false;
          localThis.filename = localThis.filePayLoad.objectValue.File.name;
        }
      }
      if (document_id == 0) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "Error: Select the File to upload",
        });
        localThis.showWait = false;
        return;
      }
      localThis.showWait = true;
      let spName: string = "SP_GMT_INS_UPD_GRANT_DOCUMENT";
      let sdata = {} as any;

      sdata.GMT_DOC_ID = document_id;
      sdata.GMT_DOC_NAME = localThis.filename;
      sdata.GMT_DOC_DESCRIPTION = localThis.description;
      sdata.GMT_GRANT_ID = localThis.selectedGrantId;
      sdata.ID = localThis.imsID;

      let savePayload: GenericSqlPayload = {
        spName: spName,
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(sdata),
        userUid: localThis.getUserUid,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: res.status,
            text: res.message,
          }); // Feedback to user
          localThis.$emit("closeEdit");

          // TODO: Fix application freeze on form reset. (error:150)
          // if (formResetFunc) {
          //   formResetFunc(); // Reset Form
          // }

          // Reset new Object if actionType CREATE
          // if (actionType == SqlActionType.NUMBER_0) {
          //   // localThis.ResetObject(localThis.newFormData);
          //   localThis.resetNewFormData();
          // }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        })
        .finally(() => {
          localThis.showWait = false;
        });
    },
  },

  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
    }),
    userLocale() {
      return i18n.locale;
    },
  },
});
</script>
