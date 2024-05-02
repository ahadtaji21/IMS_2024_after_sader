<template>
  <v-container fluid fill-height>
    <v-row v-if="showWait">
      <v-col justify-center :cols="12">
        <div class="text-center" style="margin: 0px; padding: 0px">
          <v-progress-circular indeterminate color="primary"></v-progress-circular>
        </div>
      </v-col>
    </v-row>
    <v-row align-center>
      <v-col justify-center :cols="12">
        <div v-if="!showDocDetails">
          <v-data-table
            :headers="columns"
            :items="docs"
            item-key="name"
            multi-sort
            :search="tableFilter"
            class="text-capitalize elevation-2"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h3>{{ $tc("datasource.gmt.grant.gt-doc-supp-docs", 2) }}</h3>
                <v-spacer></v-spacer>
                <v-dialog
                  v-model="dialog"
                  persistent
                  max-width="1100px"
                  v-if="module == 'GMT' && authorizedRole == true"
                >
                  <template v-slot:activator="{ on }">
                    <v-btn
                      color="secondary lighten-2"
                      class="grey--text text--darken-3"
                      v-on="on"
                    >
                      <v-icon>mdi-plus-circle-outline</v-icon>New
                    </v-btn>
                  </template>
                  <gmt-doc-details
                    :selectedGrantId="selectedGrantId"
                    :onlyRead="!(module == 'GMT' && authorizedRole == true)"
                    @closeEdit="closeEdit"
                    :key="Math.floor(Math.random() * 256)"
                  ></gmt-doc-details>
                </v-dialog>
                <v-spacer></v-spacer>
                <span
                  v-if="
                    module == 'GMT' &&
                    authorizedRole == true &&
                    getCurrentOffice.HR_OFFICE_ID == 1 //Il template lo vede solo l'ufficio IO
                  "
                >
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <v-btn
                        color="secondary lighten-2"
                        class="grey--text text--darken-3"
                        v-on="on"
                        @click="generateAgreement(item)"
                      >
                        <v-icon>mdi-file-word</v-icon>Agreement Letter
                      </v-btn>
                    </template>
                    <span>{{ $t("datasource.gmt.grant.gt-agreement-letter") }}</span>
                  </v-tooltip>
                </span>

                <v-spacer></v-spacer>

                <v-text-field
                  v-model="tableFilter"
                  append-icon="mdi-magnify"
                  :label="$t('general.search')"
                  hide-details
                  clearable
                  outlined
                  dense
                  class="white"
                  color="secondary darken-2"
                ></v-text-field>
              </div>
            </template>
            <template v-slot:[`item.action`]="{ item }">
              <div>
                <v-icon small class="mr-2" @click="editItem(item)"
                  >mdi-circle-edit-outline</v-icon
                >
                <span v-if="module == 'GMT' && authorizedRole == true">
                  <v-icon small @click="deleteItem(item)">mdi-delete</v-icon>
                </span>
              </div>
            </template>
          </v-data-table>
        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col align-center :cols="12">
        <div v-if="showDocDetails">
          <v-dialog v-model="showDocDetails" persistent max-width="1100px">
            <gmt-doc-details
              :selectedGrantId="selectedGrantId"
              :onlyRead="!(module == 'GMT' && authorizedRole == true)"
              @closeEdit="closeEdit"
              :gmtDocId="docId"
              :imsID="imsID"
              :docDescription="docDescription"
              :docFileName="docFileName"
            ></gmt-doc-details>
          </v-dialog>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { Configuration, GMTApi } from "./../../../../../axiosapi";
import { translate, i18n } from "./../../../../../i18n";
//import SearchTable from "./FUNDED/GMTGrantProjectsSearch.vue";
import DocDetails from "./GMTDocForm.vue";
import { EventBus } from "./../../../../../event-bus";
import UtilMix from "../../../../../mixins/UtilMix";
import { mapActions } from "vuex";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "./../../../../../axiosapi/api";

export default Vue.extend({
  props: {
    selectedGrantId: {
      type: Number,
      required: true,
    },
  },
  mixins: [UtilMix],
  components: {
    // "search-table": SearchTable,
    "gmt-doc-details": DocDetails,
  },
  data() {
    return {
      module: "",
      docDescription: "",
      docFileName: "",
      authorizedRole: false,
      showWait: false,
      showDocDetails: false,
      docId: 0,
      imsID: 0,
      dialog: false,
      keyDialog: 0,
      columnsTemplate: [
        {
          text: "Code",
          textKey: "datasource.gmt.grant.gt-doc-name",
          align: "center",
          value: "GMT_DOC_NAME",
          sortable: true,
          filterable: true,
        },

        {
          text: "Description",
          textKey: "datasource.gmt.grant.gt-doc-description",
          align: "center",
          value: "GMT_DOC_DESCRIPTION",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.gmt.grant.gt-doc-upload-date",
          align: "center",
          value: "GMT_DOC_UPLOAD_DATE",
          sortable: true,
          filterable: true,
        },

        { text: "Actions", value: "action", sortable: false },
      ],
      docsObj: {},
      docs: [],
      descriptions: [],
      tableFilter: "",
      editedItem: {},
    };
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
      getCurrentOffice: "getCurrentOffice",
    }),

    columns() {
      let cols = (this as any).columnsTemplate.map((col: any) => {
        if (col.value === "action") {
          return col;
        }
        //  if (col.value === "pmsCoiDescription") {
        //      let newColDesc = col;
        //     newColDesc.text = translate(col.textKey);
        //     newColDesc.value = "pmsCoiCode";
        //     return newColDesc;
        // }

        let newCol = col;
        newCol.text = translate(col.textKey);

        return newCol;
      });

      return cols;
    },
  },

  mounted() {
    let localThis: any = this;
    localThis.getGrantDocuments();
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
    var role: any = localThis.getCurrentRole.ROLE_CODE;
    if (role == "GM") {
      //Grant Manager
      localThis.authorizedRole = true;
    }
    EventBus.$on("userRoleUpdated", (to: any) => {
      localThis.authorizedRole = false;
      if (to.ROLE_CODE == "GM") {
        //Grant Manager
        localThis.authorizedRole = true;
      }
    });
  },

  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setGrant: "setGrant",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),

    generateAgreement(item: any) {
      let localThis = this as any;
      var id: number = 0;
      let ap = {} as any;

      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: GMTApi = new GMTApi(config);
      localThis.showWait = true;
      let doc: any;
      return api
        .apiGMTGetGMTTemplateGet(config.baseOptions)
        .then((res) => {
          doc = res.data;
          const byteCharacters = atob(doc);
          const byteNumbers = new Array(byteCharacters.length);
          for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
          }
          const byteArray = new Uint8Array(byteNumbers);
          const blob = new Blob([byteArray], {
            type:
              "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
          });

          var link = document.createElement("a");

          var url = URL.createObjectURL(blob);
          link.setAttribute("href", url);
          link.setAttribute("download", "gm.docx");
          link.style.visibility = "hidden";
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
        })
        .catch((err) => {
          // console.error(err);
          return "";
        })
        .finally(() => (localThis.showWait = false));
    },
    getGrantDocuments() {
      let localThis = this as any;

      localThis.docs = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_GRANT_DOCUMENT",
        //colList: null,
        whereCond: `GMT_GRANT_ID='${localThis.selectedGrantId}'`,
        orderStmt: "GMT_DOC_UPLOAD_DATE",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
                if (item.GMT_DOC_UPLOAD_DATE !== undefined) {
                    const date = new Date(item.GMT_DOC_UPLOAD_DATE);
                    const months = [
                        'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
                        'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
                    ];
                    const day = date.getDate();
                    const month = months[date.getMonth()];
                    const year = date.getFullYear();
                    item.GMT_DOC_UPLOAD_DATE = `${day} ${month} ${year}`;
                }
              localThis.docs.push(item);
            });
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          });
        })
        .finally(() => (localThis.showWait = false));
    },
    closeEdit() {
      let localThis = this as any;
      localThis.dialog = false;
      localThis.showDocDetails = false;
      localThis.getGrantDocuments();
    },
    closeDialog(item: string) {
      let localThis = this as any;
      localThis.dialog = false;
      if (item == null) return;

      let payLoadD = {} as any;
      payLoadD.ID_GRANT = localThis.selectedGrantId;
      payLoadD.ID_PROJECT = Number.parseInt(item);

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_GRANT_PROJECT",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.getGrantDocuments();
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
    closeDialogeAndUpdateList(value: any) {
      let localThis = this as any;
      localThis.getGrantDocuments();
      localThis.dialog = false;
      localThis.keyDialog = Math.ceil(Math.random() * 1000);
    },
    editItem(item: any) {
      let localThis: any = this as any;
      localThis.showDocDetails = true;
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      localThis.docId = localThis.editedItem.GMT_DOC_ID;
      localThis.imsID = localThis.editedItem.ID;
      localThis.docDescription = localThis.editedItem.GMT_DOC_DESCRIPTION;
      localThis.docFileName = localThis.editedItem.GMT_DOC_NAME;
    },
    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.showWait = true;
          let payLoadD = {} as any;
          payLoadD.ID = item.ID;

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_DEL_GRANT_DOCUMENT",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              localThis.showWait = false;
              localThis.getGrantDocuments();
            })

            .catch((err: any) => {
              localThis
                .showNewSnackbar({
                  typeName: "error",
                  text: err.message,
                })
                .finally(() => (localThis.showWait = false));
            });
        }
      });
    },
  },
});
</script>
<style scoped>
.jrs-table-top {
  width: 100%;
  height: 3.5em;
  padding: 0 1em;
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}
</style>
