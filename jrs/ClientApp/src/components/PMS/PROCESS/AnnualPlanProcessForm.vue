<template>
  <div>
    <div v-if="!showProcessDetails">
      <!-- ITM SELECTION-->
      <v-data-table
        :headers="headers"
        :items="processList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="itemsPerPage"
        style="
           {
            'font-size':'14px','width': '85px';
          }
        "
        class="text-capitalize"
        v-model="selectedItm"
      >
        <template v-slot:top>
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{ $t("datasource.pms.annual-plan.processes") }}</h3>

            <v-spacer></v-spacer>
            <v-spacer></v-spacer>
            <v-dialog
              v-if="showNew"
              v-model="editMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              transition="dialog-transition"
            >
              <template v-slot:activator="{ on }">
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  v-on="on"
                  @click="editMode = !editMode"
                  v-if="!onlyRead"
                >
                  <v-icon>mdi-plus-circle-outline</v-icon>New
                </v-btn>
              </template>

              <search-table
                v-model="processId"
                @UpdateServiceProcessList="updateProcesses"
                :formDataExt="formDataExt"
                :selectedProcessList="processList"
                :key="Math.ceil(Math.random() * 1000)"
              ></search-table>
            </v-dialog>

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
        <template v-slot:body="{ items }">
          <tbody style="border: solid">
            <tr v-for="item in items" :key="item.ID" style="height: 10px">
              <td>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <span v-on="on">{{ item.DESCRIPTION | subStr }}</span>
                  </template>
                  <span>{{ item.DESCRIPTION }}</span>
                </v-tooltip>
              </td>
              <td style="text-align: center">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="editProcess(item)" v-on="on"
                      >mdi-circle-edit-outline</v-icon
                    >
                  </template>

                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.edit-activity")
                  }}</span>
                </v-tooltip>

                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon
                      v-if="!onlyRead"
                      small
                      class="mr-2"
                      @click="deleteItem(item)"
                      v-on="on"
                      >mdi-delete</v-icon
                    >
                  </template>
                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.delete-activity")
                  }}</span>
                </v-tooltip>
              </td>
            </tr>
          </tbody>
        </template>
      </v-data-table>
    </div>
    <div>
      <div v-if="showProcessDetails">
        <ap-act-details
          :actMainDataItem="formData"
          :key="showProcessDetails"
          :editActId="editId"
          :editProcId="editProcId"
          :editActDesc="editActDesc"
          :selectedObjectId="selectedObjectId"
          :onlyRead="onlyRead"
          :versioned="versioned"
        ></ap-act-details>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import SearchTable from "./AnnualPlanProcessSearchForm.vue";
// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
import ProcessDetails from "./AnnualPlanProcessDetailsForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import { IPayLoadDataITEM, IPayLoadDataOUTPUT } from "./../PMSSharedInterfaces";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

// interface ItemData {
//   ID: number | null;
//   COI: {} | null;
//   TOS: {} | null;
//   DATE_FROM: Date | null;
//   DATE_TO: Date | null;
//   OCCURRENCE: {} | null;
//   OBJECTIVE_ID: number | null;
// }

export default Vue.extend({
  components: {
    "search-table": SearchTable,
    "ap-act-details": ProcessDetails,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },

    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
    showNew: {
      type: Boolean,
      default: true,
    },
    formDataExt: {
      type: Object,
      required: false,
    },
  },
  data() {
    return {
      selectedAct: [],
      selectedActivity: null,
      editActId: 0,
      editProcId: 0,
      showActTabs: true,
      processId: 0,
      showItmTabs: true,
      selectedOutput: "",
      selectedOutputId: 0,
      editItmDesc: "",
      editedItem: {},
      editId: null,
      editObj: null,
      showProcessDetails: false,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: -1,
      showItemNumber: 0,
      selectedItm: [],
      tableFilter: "",
      selectedItem: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      isLoading: false,
      headers: [],

      formData: {},
      coi: [],
      tos: [],
      processList: [],
      occ: [],
    };
  },

  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    closeDialog(item: String) {
      let localThis: any = this as any;
      localThis.editMode = false;
      if (item != null) localThis.UpdateItem(item);
    },
    closeOutputDialog(item: string) {
      let localThis: any = this as any;
      localThis.showProcessDetails = false;
      EventBus.$emit("showActTabs");
    },
    closeOutputDialogReload(item: string) {
      let localThis: any = this as any;
      localThis.showProcessDetails = false;
      EventBus.$emit("showActTabs");
      localThis.reloadItems(null);
    },
    UpdateItem(item: string) {
      let ap = {} as any;
      let localThis = this as any;
      let payLoadD = {} as IPayLoadDataOUTPUT;

      payLoadD.ACTIVITY_ID = localThis.selectedObjectId;
      payLoadD.OUTPUT_ID = Number.parseInt(item);

      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_PROCESS_OUTPUT",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.itemsPerPage = -1;
          localThis.getProcesses();
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },

    reloadItems(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = -1;
      localThis.getProcesses();
    },
    getProcesses() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.processList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_SERVICE_PROCESS";
      let wherecond: string = `SERVICE_ID = ${localThis.selectedObjectId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_SERVICE_PROCESS_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "DESCRIPTION",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.processList.push(item);
            });
          }
        }
      );
    },
    updateProcesses(item: any) {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog = {};
      if (item == null) return;
      let i: number = 0;
      let j: number = 0;
      for (i = 0; i < item.length; i++) {
        let payLoadD: any = {};
        payLoadD.PROCESS_ID = Number.parseInt(item[i].PMS_PROCESS_ID);
        payLoadD.SERVICE_ID = localThis.selectedObjectId;

        let savePayload: GenericSqlPayload = {
          spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OBJECTIVE_SERVICE_PROCESS",
          actionType: SqlActionType.NUMBER_0,
          jsonPayload: JSON.stringify(payLoadD),
        };
        const response = localThis
          .execSPCall(savePayload)
          .then((res: any) => {
            j++;
            if (j == item.length) localThis.getProcesses();

            return res;
            //localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
          })
          .catch((err: any) => {
            localThis.showNewSnackbar({
              typeName: "error",
              text: err.message,
            }); // Feedback to user
            throw err;
          });
      }
      localThis.itemsPerPage = -1;

      localThis.showNewSnackbar({ typeName: "success", text: "Processes updated" }); // Feedback to user
    },
    processUpdated() {
      let localThis: any = this as any;
      localThis.getProcesses();
      localThis.showNewSnackbar({ typeName: "success", text: "Processes updated" }); // Feedback to user
    },

    editProcess(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.selectedAct = [];
      //localThis.editMode = true;
      localThis.editId = item.ID;
      localThis.editProcId = item.PMS_PROCESS_ID;
      localThis.editActDesc = item.DESCRIPTION;
      localThis.showProcessDetails = true;
      localThis.selectedAct.push(item);
      localThis.activityList = [];

      EventBus.$emit("hideSrvTabs");
      localStorage.SelectedAct = localThis.shortDesc(item.DESCRIPTION);
      EventBus.$emit("refreshBreadCumb", 4);
      localThis.formData.ID = item.ID;
      localThis.formData.DESCRIPTION = item.DESCRIPTION;
      localThis.formData.SERVICE_ID = localThis.selectedObjectId;
      localThis.formData.ACTIVITY_TYPE_ID = item.ACTIVITY_TYPE_ID + "";
      localThis.formData.PROCESS_ID = item.PMS_PROCESS_ID;
      localThis.formData.PMS_LOGIC_MODEL_CODE = item.PMS_LOGIC_MODEL_CODE;
      localThis.formData.ACTIVITY_TYPE_NAME = item.ACTIVITY_TYPE_NAME;
    },
    shortDesc(string: any) {
      if (string.length < 35) return string;
      return string.substring(0, 32) + "...";
    },
    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as any;
          payLoadD.ID = item.ID;

          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_PROCESS",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              if (res != undefined && (res + "").indexOf('"status":"error"') != -1) {
                localThis.showNewSnackbar({
                  typeName: "error",
                  text:
                    "The process can't be deleted; Delete the connected resources first",
                });
              } else {
                localThis.itemsPerPage = -1;
                localThis.getProcesses();
                localThis.showNewSnackbar({
                  typeName: "success",
                  text: res.message,
                }); // Feedback to user
              }
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.pms.annual-plan.process").toString(),
          value: "DESCRIPTION",
          align: "center",
          divider: true,
        },
        // {
        //   text: this.$i18n.t("pms.process-value").toString(),
        //   value: "PMS_OUTPUT_VALUE",
        //   align: "center",
        //   divider: true,
        // },

        {
          text: "Actions",
          value: "action",
          align: "center",
          sortable: false,
        },
      ];

      localThis.headers = tmp;
    },
  },
  beforeDestroy() {
    //do something before destroying vue instance
    EventBus.$off("hideActTabs");
    EventBus.$off("showActTabs");
  },
  mounted() {
    let localThis: any = this as any;
    localThis.selectedItem = null;
    localThis.setupHeaders();

    if (localThis.selectedObjectId > 0) {
      localThis.getProcesses();
    }
    EventBus.$on("closeActivityDetails", (data: any) => {
      localThis.showProcessDetails = false;
      localThis.reloadItems();
      EventBus.$emit("showSrvTabs");
      // EventBus.$emit("sectActiveTab", "ACTIVITIES");
    });
    // EventBus.$on("closeItemDetails", (data: any) => {
    //   localThis.showProcessDetails = false;
    //   localThis.reloadItems();
    //   EventBus.$emit("showActTabs");
    // });

    EventBus.$on("hideOutpDetails", (data: any) => {
      localThis.showProcessDetails = true;
    });

    EventBus.$on("reloadItems", (data: any) => {
      localThis.reloadItems(data);
    });
  },
  computed: {
    language() {
      return i18n.locale;
    },
  },
  filters: {
    subStr: function (string: any) {
      if (string.length < 80) return string;
      return string.substring(0, 80) + "...";
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getProcesses();
      localThis.setupHeaders();
    },

    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getProcesses();
    },

    editMode(to: boolean, from: boolean) {},
  },
});
</script>
<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
.narrative-section {
  margin-bottom: 1em;
}
.overall-node {
  margin: 1em 0.5em;
}
.jrs-table-top {
  width: 100%;
  height: 3.5em;
  padding: 0 1em;
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}
.selectedRow {
  background-color: whitesmoke;
  font-weight: bold;
  border-width: 1px;
}
.tablerow {
  text-align: center;
  height: 3.5em;
  padding: 0 1em;
  text-align: center;
  border: solid;
  border-width: 1px;
  border-color: rgba(0, 0, 0, 0.12);
  box-sizing: border-box;
  border-left: none;
}
</style>
