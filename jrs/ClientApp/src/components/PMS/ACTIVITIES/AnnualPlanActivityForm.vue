<template>
  <div>
    <div v-if="!showActDetails">
      <!-- SRV SELECTION-->
      <v-row>
        <v-col :cols="3">
          <v-select
            v-model="selectedYear"
            :items="budgetYear"
            :label="'Budget Year'"
            item-text="text"
            item-value="value"
            @change="yearChanged"
            :readonly="selectYearReadOnly"
          ></v-select>
        </v-col>
        <v-col :cols="5"></v-col>
        <v-col :cols="4">
          <label id="lbl" style="color: black">Estimated Year Budget: </label>
          <currency-input
            v-model="estimatedYearBudget.BUDGET"
            :readonly="false"
            v-bind="options"
            :currency="estimatedYearBudget.CURRENCY"
            :class="'Budget'"
            aria-labelledby="lbl"
            :rules="false"
            style="color: black; background-color: white"
            :key="keyRnd"
          ></currency-input>
        </v-col>
      </v-row>

      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headers"
            :items="activityList"
            item-key="ID"
            multi-sort
            :search="tableFilter"
            :items-per-page="100"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedAct"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h3>{{ $t("datasource.pms.annual-plan.processes") }}</h3>

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
                    :selectedProcessList="serviceProcessList"
                    :key="Math.ceil(Math.random() * 1000)"
                  ></search-table>
                </v-dialog>
                <!-- <v-dialog
                  v-model="editMode"
                  persistent
                  retain-focus
                  :scrollable="true"
                  :overlay="false"
                  :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
                  transition="dialog-transition"
                >
                  <template v-slot:activator="{ on }" v-if="!onlyRead">
                    <v-btn
                      color="secondary lighten-2"
                      class="grey--text text--darken-3"
                      v-on="on"
                      @click="editMode = !editMode"
                    >
                      <v-icon>mdi-plus-circle-outline</v-icon>Select
                    </v-btn>
                  </template>
                  <pms-addact-form
                    @closeProcessDialoge="closeDialog"
                    @closeProcessDialogeAndSave="closeDialog"
                    @refreshAct="getServiceActivities"
                    :formDataExt="editedItemDialog"
                    :selectedObjectId="selectedObjectId"
                    :key="Math.ceil(Math.random() * 1000)"
                  ></pms-addact-form>
                </v-dialog> -->

                <v-spacer></v-spacer>
                <v-dialog
                  v-model="budgetMode"
                  persistent
                  retain-focus
                  :scrollable="true"
                  :overlay="false"
                  transition="dialog-transition"
                >
                  <v-card>
                    <v-card-title>{{ editActDesc }}</v-card-title>
                    <v-card-text>
                      <ap-budget
                        :selectedObjectId="editActId"
                        :key="Math.ceil(Math.random() * 1000)"
                        :selectedYearExt="selectedYear"
                        :versioned="versioned"
                      ></ap-budget>
                    </v-card-text>
                    <v-card-actions>
                      <v-btn color="secondary darken-1" text @click="closeBudget"
                        >X {{ $t("general.close") }}</v-btn
                      >
                    </v-card-actions>
                  </v-card>
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
              <tbody :style="{ 'overflow-x': 'auto !important' }">
                <tr v-for="item in items" :key="item.ID" style="height: 10px">
                  <td>
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <span v-on="on">{{ item.DESCRIPTION | subStr }}</span>
                      </template>
                      <span>{{ item.DESCRIPTION }}</span>
                    </v-tooltip>
                  </td>

                  <td
                    :class="
                      item.JAN != undefined && item.JAN.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.JAN != undefined &&
                        item.JAN.VALUE != undefined &&
                        item.JAN.CURRENCY != undefined
                      "
                      v-model="item.JAN.VALUE"
                      v-bind="options"
                      :currency="item.JAN.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.FEB != undefined && item.FEB.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.FEB != undefined &&
                        item.FEB.VALUE != undefined &&
                        item.FEB.CURRENCY != undefined
                      "
                      v-model="item.FEB.VALUE"
                      v-bind="options"
                      :currency="item.FEB.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MAR != undefined && item.MAR.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MAR != undefined &&
                        item.MAR.VALUE != undefined &&
                        item.MAR.CURRENCY != undefined
                      "
                      v-model="item.MAR.VALUE"
                      v-bind="options"
                      :currency="item.MAR.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.APR != undefined && item.APR.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.APR != undefined &&
                        item.APR.VALUE != undefined &&
                        item.APR.CURRENCY != undefined
                      "
                      v-model="item.APR.VALUE"
                      v-bind="options"
                      :currency="item.APR.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MAY != undefined && item.MAY.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MAY != undefined &&
                        item.MAY.VALUE != undefined &&
                        item.MAY.CURRENCY != undefined
                      "
                      v-model="item.MAY.VALUE"
                      v-bind="options"
                      :currency="item.MAY.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.JUN != undefined && item.JUN.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.JUN != undefined &&
                        item.JUN.VALUE != undefined &&
                        item.JUN.CURRENCY != undefined
                      "
                      v-model="item.JUN.VALUE"
                      v-bind="options"
                      :currency="item.JUN.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.JUL != undefined && item.JUL.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.JUL != undefined &&
                        item.JUL.VALUE != undefined &&
                        item.JUL.CURRENCY != undefined
                      "
                      v-model="item.JUL.VALUE"
                      v-bind="options"
                      :currency="item.JUL.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.AUG != undefined && item.AUG.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.AUG != undefined &&
                        item.AUG.VALUE != undefined &&
                        item.AUG.CURRENCY != undefined
                      "
                      v-model="item.AUG.VALUE"
                      v-bind="options"
                      :currency="item.AUG.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.SEP != undefined && item.SEP.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.SEP != undefined &&
                        item.SEP.VALUE != undefined &&
                        item.SEP.CURRENCY != undefined
                      "
                      v-model="item.SEP.VALUE"
                      v-bind="options"
                      :currency="item.SEP.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.OCT != undefined && item.OCT.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.OCT != undefined &&
                        item.OCT.VALUE != undefined &&
                        item.OCT.CURRENCY != undefined
                      "
                      v-model="item.OCT.VALUE"
                      v-bind="options"
                      :currency="item.OCT.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.NOV != undefined && item.NOV.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.NOV != undefined &&
                        item.NOV.VALUE != undefined &&
                        item.NOV.CURRENCY != undefined
                      "
                      v-model="item.NOV.VALUE"
                      v-bind="options"
                      :currency="item.NOV.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.DEC != undefined && item.DEC.SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.DEC != undefined &&
                        item.DEC.VALUE != undefined &&
                        item.DEC.CURRENCY != undefined
                      "
                      v-model="item.DEC.VALUE"
                      v-bind="options"
                      :currency="item.DEC.CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <span v-if="!onlyRead">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                          >mdi-circle-edit-outline</v-icon
                        >
                      </template>
                      <span>{{
                        $t("datasource.pms.annual-plan.objectives.edit-activity")
                      }}</span>
                    </v-tooltip>
                  </span>
                  <span v-if="onlyRead">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                          >mdi-eye</v-icon
                        >
                      </template>
                      <span>{{
                        $t("datasource.pms.annual-plan.objectives.edit-activity")
                      }}</span>
                    </v-tooltip>
                  </span>
                  <span v-if="!onlyRead">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <v-icon small class="mr-2" @click="editBudget(item)" v-on="on"
                          >mdi-wallet</v-icon
                        >
                      </template>
                      <span>{{
                        $t("datasource.pms.annual-plan.objectives.budget-item")
                      }}</span>
                    </v-tooltip>
                  </span>
                  <!-- <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-icon small class="mr-2" @click="editActivities(item);" v-on="on">mdi-chart-line</v-icon>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.edit-activity-activities') }}</span>
              </v-tooltip>-->
                  <span v-if="!onlyRead">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                          >mdi-delete</v-icon
                        >
                      </template>
                      <span>{{
                        $t("datasource.pms.annual-plan.objectives.delete-activity")
                      }}</span>
                    </v-tooltip>
                  </span>
                </tr>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
    </div>
    <div>
      <div v-if="showActDetails">
        <ap-act-details
          :actMainDataItem="formData"
          :key="showActDetails"
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
import { mapActions } from "vuex";
import JrsTable from "../../../components/JrsTable.vue";
import { JrsHeader } from "../../../models/JrsTable";
import ActivityBudget from "./AnnualPlanActivityPlanForm.vue";
import Narrative from "../ANNUALPLAN/AnnualPlanNarrative.vue";
import ActivityDetails from "../PROCESS/AnnualPlanProcessDetailsForm.vue";
//import ActivityMainData from "./AnnualPlanActivityMainDataForm.vue";
import SearchProcessForm from "../PROCESS/AnnualPlanProcessSearchForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import { IEstimatedYearBudget } from "./../PMSSharedInterfaces";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

interface ACTIVITY_MONTH {
  SELECTED: boolean;
  VALUE: any;
  CURRENCY: string;
}
interface activityItem {
  ID: number;
  ACTIVITY_TYPE_ID: string;
  PROCESS_ID: number;
  ACTIVITY_TYPE_NAME: string;
  SERVICE_ID: number;
  DESCRIPTION: string;
  PMS_LOGIC_MODEL_CODE: string;
  YEAR: number;
  JAN: ACTIVITY_MONTH;
  FEB: ACTIVITY_MONTH;
  MAR: ACTIVITY_MONTH;
  APR: ACTIVITY_MONTH;
  MAY: ACTIVITY_MONTH;
  JUN: ACTIVITY_MONTH;
  JUL: ACTIVITY_MONTH;
  AUG: ACTIVITY_MONTH;
  SEP: ACTIVITY_MONTH;
  OCT: ACTIVITY_MONTH;
  NOV: ACTIVITY_MONTH;
  DEC: ACTIVITY_MONTH;
}

// interface ActivityData {
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
    // "jrs-table": JrsTable,

    "ap-act-details": ActivityDetails,
    //"pms-addact-form": ProcessMainData,
    "search-table": SearchProcessForm,
    "ap-budget": ActivityBudget,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
    selectedYearExt: {
      type: String,
      required: false,
      default: "",
    },
    formDataExt: {
      type: Object,
      required: false,
    },
    showActivityDetails: {
      type: Boolean,
      required: false,
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
  },

  data() {
    return {
      estimatedYearBudget: {},
      keyRnd: 0,
      selectYearReadOnly: false,
      processId: 0,
      serviceProcessList: [],
      budgetYear: [],
      selectedYear: "",
      editActId: 0,
      editProcId: 0,
      budgetMode: false,
      showActTabs: true,
      editActDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showActDetails: false,
      selectedAct: [],
      tableFilter: "",
      tblName: null,
      selectedActivity: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      headers: [],

      formData: {},
      coi: [],
      tos: [],
      activityList: [] as activityItem[],
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
    closeDialog() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog = {};
    },

    getEstimatedYearBudget() {
      let localThis: any = this as any;
      localThis.estimatedYearBudget = {} as any;
      let view: string = "PMS_VI_SERVICE_YEAR_BUDGET";
      let wherecond: string = `SERVICE_ID = ${localThis.selectedObjectId} AND YEAR=${localThis.selectedYear}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_SERVICE_YEAR_BUDGET_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null,
      };
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.estimatedYearBudget.YEAR = item.YEAR;
              localThis.estimatedYearBudget.BUDGET = item.BUDGET;
              //localThis.estimatedYearBudget.ACTIVITY_ID = item.PMS_ACTIVITY_ID;
              localThis.estimatedYearBudget.CURRENCY = localThis.currencyCode;
              localThis.keyRnd = Math.ceil(Math.random() * 1000);
            });
          }
        });
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
            if (j == item.length) {
              localThis.getEstimatedYearBudget();
              localThis.getServiceActivities();
            }

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
    },
    // updateProcesses(item: any) {
    //   let localThis: any = this as any;
    //   localThis.editMode = false;
    //   localThis.dialog = false;
    //   localThis.editedItemDialog = {};
    //   if (item == null) return;
    //   let payLoadD: any = {};
    //   payLoadD.PROCESS_ID = Number.parseInt(item);
    //   payLoadD.SERVICE_ID = localThis.selectedObjectId;

    //   let savePayload: GenericSqlPayload = {
    //     spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OBJECTIVE_SERVICE_PROCESS",
    //     actionType: SqlActionType.NUMBER_0,
    //     jsonPayload: JSON.stringify(payLoadD),
    //   };
    //   localThis
    //     .execSPCall(savePayload)
    //     .then((res: any) => {
    //       localThis.itemsPerPage = 15;
    //       localThis.getEstimatedYearBudget();
    //       localThis.getServiceActivities();
    //       localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
    //     })
    //     .catch((err: any) => {
    //       localThis.showNewSnackbar({
    //         typeName: "error",
    //         text: err.message,
    //       }); // Feedback to user
    //     });
    // },

    reloadActivities(item: any) {
      let localThis = this as any;

      localThis.getServiceActivities();
    },
    yearChanged() {
      let localThis: any = this as any;
      localThis.getEstimatedYearBudget();
      localThis.getServiceActivities();
    },
    getServiceProcessList() {
      let localThis: any = this as any;
      localThis.serviceProcessList = [];
      let view: string = "PMS_VI_ANNUAL_PLAN_SERVICE_PROCESS_LIST";
      let wherecond: string = `SERVICE_ID = ${localThis.selectedObjectId} `;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_SERVICE_PROCESS_LIST_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID",
      };
      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.serviceProcessList.push(item);
            });
          }
        }
      );
    },
    getServiceActivities() {
      let localThis: any = this as any;
      localThis.getServiceProcessList().then((res: any) => {
        localThis.getSrvAct();
      });
    },

    getSrvAct() {
      let localThis: any = this as any;
      localThis.selectedActivity = null;
      localThis.activityList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_SERVICE_PROCESS_BUDGET";
      let wherecond: string = `SERVICE_ID = ${localThis.selectedObjectId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_SERVICE_PROCESS_BUDGET_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          let idx: number = 0;
          let tmp_table_data = [] as any;
          if (localThis.serviceProcessList == undefined) return;
          for (idx = 0; idx < localThis.serviceProcessList.length; idx++) {
            let process = localThis.serviceProcessList[idx];
            let found: boolean = false;
            res.table_data.forEach((item: any) => {
              if (process.ID == item.ID && item.YEAR == localThis.selectedYear) {
                found = true;
                tmp_table_data.push(item);
              }
            });
            if (!found) {
              res.table_data.forEach((item: any) => {
                if (process.ID == item.ID && item.YEAR == null) {
                  found = true;
                  item.YEAR = localThis.selectedYear;
                  tmp_table_data.push(item);
                }
              });
            }
            if (!found) {
              res.table_data.forEach((item: any) => {
                if (process.ID == item.ID) {
                  found = true;
                  item.YEAR = localThis.selectedYear;
                  item.MONTH = null;
                  item.YEAR = null;
                  item.VALUE = null;
                  item.CURRENCY = null;
                  tmp_table_data.push(item);
                }
              });
            }
          }

          tmp_table_data.forEach((item: any) => {
            let i: number = 0;
            let k: number = -1;
            for (i = 0; i < localThis.activityList.length; i++) {
              if (localThis.activityList[i].ID == item.ID) {
                k = i;
                break;
              }
            }
            if (k == -1) {
              let actItem = {} as activityItem;
              actItem.ID = item.ID;
              actItem.DESCRIPTION = item.DESCRIPTION;
              actItem.PMS_LOGIC_MODEL_CODE = item.PMS_LOGIC_MODEL_CODE;
              actItem.SERVICE_ID = item.SERVICE_ID;
              actItem.PROCESS_ID = item.PMS_PROCESS_ID;
              actItem.ACTIVITY_TYPE_ID = item.ACTIVITY_TYPE_ID + "";
              actItem.ACTIVITY_TYPE_NAME = item.ACTIVITY_TYPE_NAME;
              actItem.YEAR = item.YEAR;
              switch (item.MONTH) {
                case 1:
                  actItem.JAN = {} as ACTIVITY_MONTH;
                  actItem.JAN.SELECTED = true;
                  actItem.JAN.VALUE = item.VALUE;
                  actItem.JAN.CURRENCY = item.CURRENCY;
                  break;
                case 2:
                  actItem.FEB = {} as ACTIVITY_MONTH;
                  actItem.FEB.SELECTED = true;
                  actItem.FEB.VALUE = item.VALUE;
                  actItem.FEB.CURRENCY = item.CURRENCY;
                  break;
                case 3:
                  actItem.MAR = {} as ACTIVITY_MONTH;
                  actItem.MAR.SELECTED = true;
                  actItem.MAR.VALUE = item.VALUE;
                  actItem.MAR.CURRENCY = item.CURRENCY;
                  break;
                case 4:
                  actItem.APR = {} as ACTIVITY_MONTH;
                  actItem.APR.SELECTED = true;
                  actItem.APR.VALUE = item.VALUE;
                  actItem.APR.CURRENCY = item.CURRENCY;
                  break;
                case 5:
                  actItem.MAY = {} as ACTIVITY_MONTH;
                  actItem.MAY.SELECTED = true;
                  actItem.MAY.VALUE = item.VALUE;
                  actItem.MAY.CURRENCY = item.CURRENCY;
                  break;
                case 6:
                  actItem.JUN = {} as ACTIVITY_MONTH;
                  actItem.JUN.SELECTED = true;
                  actItem.JUN.VALUE = item.VALUE;
                  actItem.JUN.CURRENCY = item.CURRENCY;
                  break;
                case 7:
                  actItem.JUL = {} as ACTIVITY_MONTH;
                  actItem.JUL.SELECTED = true;
                  actItem.JUL.VALUE = item.VALUE;
                  actItem.JUL.CURRENCY = item.CURRENCY;
                  break;
                case 8:
                  actItem.AUG = {} as ACTIVITY_MONTH;
                  actItem.AUG.SELECTED = true;
                  actItem.AUG.VALUE = item.VALUE;
                  actItem.AUG.CURRENCY = item.CURRENCY;
                  break;
                case 9:
                  actItem.SEP = {} as ACTIVITY_MONTH;
                  actItem.SEP.SELECTED = true;
                  actItem.SEP.VALUE = item.VALUE;
                  actItem.SEP.CURRENCY = item.CURRENCY;
                  break;
                case 10:
                  actItem.OCT = {} as ACTIVITY_MONTH;
                  actItem.OCT.SELECTED = true;
                  actItem.OCT.VALUE = item.VALUE;
                  actItem.OCT.CURRENCY = item.CURRENCY;
                  break;
                case 11:
                  actItem.NOV = {} as ACTIVITY_MONTH;
                  actItem.NOV.SELECTED = true;
                  actItem.NOV.VALUE = item.VALUE;
                  actItem.NOV.CURRENCY = item.CURRENCY;
                  break;
                case 12:
                  actItem.DEC = {} as ACTIVITY_MONTH;
                  actItem.DEC.SELECTED = true;
                  actItem.DEC.VALUE = item.VALUE;
                  actItem.DEC.CURRENCY = item.CURRENCY;
                  break;
              }

              localThis.activityList.push(actItem);
            } else {
              switch (item.MONTH) {
                case 1:
                  localThis.activityList[k].JAN = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].JAN.SELECTED = true;
                  localThis.activityList[k].JAN.VALUE = item.VALUE;
                  localThis.activityList[k].JAN.CURRENCY = item.CURRENCY;
                  break;

                case 2:
                  localThis.activityList[k].FEB = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].FEB.SELECTED = true;
                  localThis.activityList[k].FEB.VALUE = item.VALUE;
                  localThis.activityList[k].FEB.CURRENCY = item.CURRENCY;
                  break;
                case 3:
                  localThis.activityList[k].MAR = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].MAR.SELECTED = true;
                  localThis.activityList[k].MAR.VALUE = item.VALUE;
                  localThis.activityList[k].MAR.CURRENCY = item.CURRENCY;
                  break;
                case 4:
                  localThis.activityList[k].APR = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].APR.SELECTED = true;
                  localThis.activityList[k].APR.VALUE = item.VALUE;
                  localThis.activityList[k].APR.CURRENCY = item.CURRENCY;
                  break;
                case 5:
                  localThis.activityList[k].MAY = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].MAY.SELECTED = true;
                  localThis.activityList[k].MAY.VALUE = item.VALUE;
                  localThis.activityList[k].MAY.CURRENCY = item.CURRENCY;
                  break;
                case 6:
                  localThis.activityList[k].JUN = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].JUN.SELECTED = true;
                  localThis.activityList[k].JUN.VALUE = item.VALUE;
                  localThis.activityList[k].JUN.CURRENCY = item.CURRENCY;
                  break;
                case 7:
                  localThis.activityList[k].JUL = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].JUL.SELECTED = true;
                  localThis.activityList[k].JUL.VALUE = item.VALUE;
                  localThis.activityList[k].JUL.CURRENCY = item.CURRENCY;
                  break;
                case 8:
                  localThis.activityList[k].AUG = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].AUG.SELECTED = true;
                  localThis.activityList[k].AUG.VALUE = item.VALUE;
                  localThis.activityList[k].AUG.CURRENCY = item.CURRENCY;
                  break;
                case 9:
                  localThis.activityList[k].SEP = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].SEP.SELECTED = true;
                  localThis.activityList[k].SEP.VALUE = item.VALUE;
                  localThis.activityList[k].SEP.CURRENCY = item.CURRENCY;
                  break;
                case 10:
                  localThis.activityList[k].OCT = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].OCT.SELECTED = true;
                  localThis.activityList[k].OCT.VALUE = item.VALUE;
                  localThis.activityList[k].OCT.CURRENCY = item.CURRENCY;
                  break;
                case 11:
                  localThis.activityList[k].NOV = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].NOV.SELECTED = true;
                  localThis.activityList[k].NOV.VALUE = item.VALUE;
                  localThis.activityList[k].NOV.CURRENCY = item.CURRENCY;
                  break;
                case 12:
                  localThis.activityList[k].DEC = {} as ACTIVITY_MONTH;
                  localThis.activityList[k].DEC.SELECTED = true;
                  localThis.activityList[k].DEC.VALUE = item.VALUE;
                  localThis.activityList[k].DEC.CURRENCY = item.CURRENCY;
                  break;
              }
            }
          });
        }
      );
    },

    deleteItem(item: any) {
      let localThis = this as any;
      localThis.showNewSnackbar({
        typeName: "error",
        text: "To be Implemented",
      });
    },
    editActivities(item: any) {
      let localThis: any = this as any;
      localThis.selectedAct = [];
      localThis.selectedActivity = item.ID;
      localThis.selectedAct.push(item);
    },

    closeBudget() {
      let localThis: any = this as any;
      localThis.getEstimatedYearBudget();
      localThis.getServiceActivities();
      localThis.budgetMode = false;
    },

    editBudget(item: any) {
      let localThis: any = this as any;
      localThis.budgetMode = true;
      localThis.editActId = item.ID;
      localThis.editActDesc = item.DESCRIPTION;
    },

    editItem(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.selectedAct = [];
      //localThis.editMode = true;
      localThis.editId = item.ID;
      localThis.editProcId = item.PROCESS_ID;
      localThis.editActDesc = item.DESCRIPTION;
      localThis.showActDetails = true;

      localThis.selectedAct.push(item);
      localThis.activityList = [];

      EventBus.$emit("hideSrvTabs");
      localStorage.SelectedAct = localThis.shortDesc(item.DESCRIPTION);
      EventBus.$emit("refreshBreadCumb", 4);
      localThis.formData.ID = item.ID;
      localThis.formData.DESCRIPTION = item.DESCRIPTION;
      localThis.formData.SERVICE_ID = localThis.selectedObjectId;
      localThis.formData.ACTIVITY_TYPE_ID = item.ACTIVITY_TYPE_ID + "";
      localThis.formData.PROCESS_ID = item.PROCESS_ID;
      localThis.formData.PMS_LOGIC_MODEL_CODE = item.PMS_LOGIC_MODEL_CODE;
      localThis.formData.ACTIVITY_TYPE_NAME = item.ACTIVITY_TYPE_NAME;
    },
    editNarrative(item: any) {
      let localThis: any = this as any;
      localThis.selectedAct = [];
      localThis.selectedActivity = item.ID;
      localThis.selectedAct.push(item);
    },
    shortDesc(string: any) {
      if (string.length < 35) return string;
      return string.substring(0, 32) + "...";
    },
    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("general.name").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.jan").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.feb").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.mar").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.apr").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.may").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.jun").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.jul").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.aug").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.sep").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.oct").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.nov").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.dec").toString(),
          value: "",
          align: "center",
          divider: true,
        },

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
  beforeMount() {
    let localThis: any = this as any;
    let ap: any = localThis.$store.getters.getAnnualPlan;
    let year: number;
    localThis.budgetYear = [];
    for (
      year = Number.parseInt(ap.start_year);
      year < Number.parseInt(ap.end_year) + 1;
      year++
    ) {
      let yearN = {} as any;
      yearN.text = year.toString();
      yearN.value = year.toString();
      localThis.budgetYear.push(yearN);
    }
    localThis.selectedYear = localThis.budgetYear[0].value;
  },
  mounted() {
    let localThis: any = this as any;
    localThis.selectedActivity = null;
    localThis.tblName = "PMS_ACTIVITY";
    localThis.getEstimatedYearBudget();
    if (localThis.selectedYearExt != "") {
      localThis.selectedYear = localThis.selectedYearExt;
      localThis.selectYearReadOnly = true;
    }
    localThis.setupHeaders();
    if (localThis.selectedObjectId > 0) {
      localThis.getEstimatedYearBudget();
      localThis.getServiceActivities();
    }

    EventBus.$on("closeActivityDetails", (data: any) => {
      localThis.showActDetails = false;
      localThis.reloadActivities();
      EventBus.$emit("showSrvTabs");
      // EventBus.$emit("sectActiveTab", "ACTIVITIES");
    });

    EventBus.$on("reloadActivities", (data: any) => {
      localThis.reloadActivities(data);
    });
  },
  computed: {
    language() {
      return i18n.locale;
    },
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
      };
    },
  },
  filters: {
    subStr: function (string: any) {
      if (string.length < 35) return string;
      return string.substring(0, 32) + "...";
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getServiceActivities();
      localThis.setupHeaders();
    },
    showActivityDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showActDetails = to;
    },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getEstimatedYearBudget();
      localThis.getServiceActivities();
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
.selectedCell {
  background-color: whitesmoke;
  color: black;
  font-size: 12px;
  border-style: solid;
  border-color: gainsboro;
  border-width: 1px;
  height: 10px;
  width: 85px;
  padding: 0px;
  margin: 0px;
}
.notSelectedCell {
  background-color: white;
  font-size: 1px;
  border-style: solid;
  border-width: 1px;
  border-color: gainsboro;
  height: 10px;
  width: 85px;
}
.valuedBudget {
  font-size: 12px;
  width: 85px;
}
</style>
