<template>
  <div>
    <div v-if="!showOutpDetails">
      <!-- ITM SELECTION-->
      <v-data-table
        :headers="headers"
        :items="outputList"
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
            <h3>{{ $t("datasource.pms.annual-plan.objectives.outputs") }}</h3>

            <v-spacer></v-spacer>
            <v-dialog
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
                v-model="outputId"
                @UpdateOutput="closeDialog"
                :selectedProcessId="selectedProcessId"
                :key="Math.ceil(Math.random() * 1000)"
                :selectedOutputList="outputList"
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
              <td class="tablerow">{{ item.PMS_OUTPUT_DESC }}</td>
              <!-- <td class="tablerow">{{ item.VALUE }}</td> -->
              <td style="text-align: center">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="editOutput(item)" v-on="on"
                      >mdi-circle-edit-outline</v-icon
                    >
                  </template>

                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.output-details")
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
                    $t("datasource.pms.annual-plan.objectives.delete-output")
                  }}</span>
                </v-tooltip>
              </td>
            </tr>
          </tbody>
        </template>
      </v-data-table>
    </div>
    <div>
      <div v-if="showOutpDetails">
        <item-output-details
          @saveOutpData="UpdateItem"
          :formData="formData"
          :versioned="versioned"
          :onlyRead="onlyRead"
          :key="Math.ceil(Math.random() * 1000)"
        ></item-output-details>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import SearchTable from "./AnnualPlanOutputSearch.vue";
// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
import ItemOutput from "./AnnualPlanOutputDetailsForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import { IPayLoadDataITEM, IPayLoadDataOUTPUT } from "../PMSSharedInterfaces";

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
    "item-output-details": ItemOutput,
  },
  props: {
    selectedProcessId: {
      type: Number,
      required: true,
    },
    selectedActivityId: {
      type: Number,
      required: true,
    },
    showItemDetails: {
      type: Boolean,
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
  },
  data() {
    return {
      outputId: {},
      showItmTabs: true,
      selectedOutput: "",
      selectedOutputId: 0,
      editItmDesc: "",
      editedItem: {},
      editId: null,
      editObj: null,
      showOutpDetails: false,
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
      outputList: [],
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
      if (item != null) localThis.UpdateItems(item);
    },
    closeOutputDialog(item: string) {
      let localThis: any = this as any;
      localThis.showOutpDetails = false;
      EventBus.$emit("showActTabs");
    },
    closeOutputDialogReload(item: string) {
      let localThis: any = this as any;
      localThis.showOutpDetails = false;
      EventBus.$emit("showActTabs");
      localThis.reloadItems(null);
    },
    UpdateItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;
      let payLoadD = {} as any;

      if (item.selectedProcessOutputRelId == undefined)
        payLoadD.PMS_ACTIVITY_OUTPUT_REL_ID = 0;
      else payLoadD.PMS_ACTIVITY_OUTPUT_REL_ID = item.selectedProcessOutputRelId;

      payLoadD.ACTIVITY_ID = localThis.selectedActivityId;
      payLoadD.OUTPUT_ID = item.selectedOutputId;
      payLoadD.VALUE = item.value;

      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_PROCESS_OUTPUT",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      const response = localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          //localThis.getActivityOutputs();
          localThis.reloadItems(null);
          return res;
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
          throw err;
        });

      localThis.itemsPerPage = -1;
      localThis.showNewSnackbar({ typeName: "success", text: "Output Updated" }); // Feedback to user
    },
    UpdateItems(item: any) {
      let ap = {} as any;
      let localThis = this as any;
      let payLoadD = {} as any;
      let i: number = 0;
      let j: number = 0;
      for (i = 0; i < item.length; i++) {
        if (item[i].PMS_ACTIVITY_OUTPUT_REL_ID == undefined)
          payLoadD.PMS_ACTIVITY_OUTPUT_REL_ID = 0;
        else
          payLoadD.PMS_ACTIVITY_OUTPUT_REL_ID = Number.parseInt(
            item[i].PMS_ACTIVITY_OUTPUT_REL_ID
          );
        payLoadD.ACTIVITY_ID = localThis.selectedActivityId;
        payLoadD.OUTPUT_ID = Number.parseInt(item[i].PMS_OUTPUT_ID);
        payLoadD.VALUE = item[i].VALUE;

        let savePayload: GenericSqlPayload = {
          spName: "PMS_SP_INS_PROCESS_OUTPUT",
          actionType: SqlActionType.NUMBER_0,
          jsonPayload: JSON.stringify(payLoadD),
        };
        const response = localThis
          .execSPCall(savePayload)
          .then((res: any) => {
            j++;
            if (j == item.length) localThis.getActivityOutputs();
            return res;
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
      localThis.showNewSnackbar({ typeName: "success", text: "Output Updated" }); // Feedback to user
    },

    reloadItems(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = -1;
      localThis.getActivityOutputs();
    },
    getActivityOutputs() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.outputList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ACTIVITY_OUTPUT";
      let wherecond: string = `PMS_ACTIVITY_ID = ${localThis.selectedActivityId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ACTIVITY_OUTPUT_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_OUTPUT_DESC",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.outputList.push(item);
            });
          }
        }
      );
    },

    editOutput(item: any) {
      let localThis = this as any;

      localThis.selectedOutput = item.PMS_OUTPUT_DESC;
      localThis.selectedOutputId = item.PMS_OUTPUT_ID;
      localThis.formData.selectedProcessOutputRelId = item.PMS_ACTIVITY_OUTPUT_REL_ID;
      localThis.formData.selectedProcessId = localThis.selectedProcessId;
      localThis.formData.selectedActivityId = localThis.selectedActivityId;
      localThis.formData.selectedOutputId = localThis.selectedOutputId;
      localThis.formData.selectedOutput = localThis.selectedOutput;
      localThis.formData.value = item.VALUE;
      localThis.showOutpDetails = true;
      EventBus.$emit("hideActTabs");
      //EventBus.$emit("showOutpTabs");
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
          payLoadD.PMS_ACTIVITY_OUTPUT_REL_ID = item.PMS_ACTIVITY_OUTPUT_REL_ID;

          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_PROCESS_OUTPUT",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = -1;
              localThis.getActivityOutputs();
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
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
          text: this.$i18n.t("pms.output-description").toString(),
          value: "PMS_OUTPUT_DESC",
          align: "center",
          divider: true,
        },
        // {
        //   text: this.$i18n.t("pms.output-value").toString(),
        //   value: "VALUE",
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

    if (localThis.selectedActivityId > 0) {
      localThis.getActivityOutputs();
    }

    EventBus.$on("closeItemDetails", (data: any) => {
      localThis.showOutpDetails = false;
      localThis.reloadItems();
      EventBus.$emit("showActTabs");
    });

    EventBus.$on("hideOutpDetails", (data: any) => {
      localThis.showOutpDetails = false;
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
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getActivityOutputs();
      localThis.setupHeaders();
    },
    showItemDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showOutpDetails = to;
    },
    selectedActivityId(to: number) {
      let localThis: any = this as any;
      localThis.selectedActivityId = to;
      localThis.getActivityOutputs();
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
