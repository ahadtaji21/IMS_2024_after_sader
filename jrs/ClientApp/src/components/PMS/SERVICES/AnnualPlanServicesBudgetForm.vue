<template>
  <div>
    <div v-if="!showSrvDetails">
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
          ></v-select>
        </v-col>
        <v-col :cols="5"></v-col>
        <v-col :cols="4">
          <label id="lbl" :class="'v-label theme--light'">Estimated Year Budget: </label>
          <currency-input
            v-model="estimatedYearBudget.BUDGET"
            :readonly="false"
            v-bind="options"
            :currency="'USD'"
            :class="'Budget'"
            aria-labelledby="lbl"
            :rules="false"
          ></currency-input>
        </v-col>
      </v-row>
      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headers"
            :items="serviceList"
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
            v-model="selectedSrv"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h3>{{ $t("datasource.pms.annual-plan.objectives.services") }}</h3>

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
                    <v-card-title>Service: {{ editSrvDesc }}</v-card-title>
                    <v-card-text>
                      <ap-budget
                        :onlyRead="onlyRead"
                        :selectedObjectId="editSrvId"
                        :key="Math.ceil(Math.random() * 1000)"
                        :selectedYearExt="selectedYear"
                        :versioned="versioned"
                        :showNew="false"
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
              <tbody>
                <tr v-for="item in items" :key="item.ID" style="height: 10px">
                  <td style="height: 10px">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <span v-on="on">{{ item.CODE | subStr }}</span>
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
                  <!-- <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-icon small class="mr-2" @click="editActivities(item);" v-on="on">mdi-chart-line</v-icon>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.edit-service-activities') }}</span>
              </v-tooltip>-->
                  <!-- <v-tooltip bottom>
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
                      $t("datasource.pms.annual-plan.objectives.delete-service")
                    }}</span>
                  </v-tooltip> -->
                </tr>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import JrsTable from "../../../components/JrsTable.vue";
import { JrsHeader } from "../../../models/JrsTable";
import ServiceBudget from "../ACTIVITIES/AnnualPlanActivityForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

interface SERVICE_MONTH {
  SELECTED: boolean;
  VALUE: any;
  CURRENCY: string;
}
interface serviceItem {
  ID: number;
  SERVICE_ID: number;
  CODE: string;
  DESCRIPTION: string;
  YEAR: number;
  JAN: SERVICE_MONTH;
  FEB: SERVICE_MONTH;
  MAR: SERVICE_MONTH;
  APR: SERVICE_MONTH;
  MAY: SERVICE_MONTH;
  JUN: SERVICE_MONTH;
  JUL: SERVICE_MONTH;
  AUG: SERVICE_MONTH;
  SEP: SERVICE_MONTH;
  OCT: SERVICE_MONTH;
  NOV: SERVICE_MONTH;
  DEC: SERVICE_MONTH;
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

    "ap-budget": ServiceBudget,
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
    // showActivityDetails: {
    //   type: Boolean,
    //   required: true,
    // },

    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },

  data() {
    return {
      estimatedYearBudget: {},
      selectedYear: "",
      budgetYear: [],
      editSrvId: 0,
      budgetMode: false,
      showActTabs: true,
      editSrvDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showSrvDetails: false,
      selectedSrv: [],
      tableFilter: "",
      tblName: null,
      selectedSrvivity: null,
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
      serviceList: [] as serviceItem[],
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

    reloadActivities(item: any) {
      let localThis = this as any;
      localThis.getEstimatedYearBudget();
      localThis.getObjectServices();
    },

    yearChanged() {
      let localThis = this as any;
      localThis.getEstimatedYearBudget();
      localThis.getObjectServices();
    },
    getEstimatedYearBudget() {
      let localThis: any = this as any;
      localThis.estimatedYearBudget = {} as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_OBJECTIVE_YEAR_BUDGET",
        colList: null,
        whereCond: `OBJ_ID = ${localThis.selectedObjectId} AND YEAR=${localThis.selectedYear}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
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
    getObjectServices() {
      let localThis: any = this as any;
      localThis.selectedSrvivity = null;
      localThis.serviceList = [];
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_ANNUAL_PLAN_OBJECTIVE_SERVICE",
        colList: null,
        whereCond: `OBJ_ID = ${localThis.selectedObjectId}  AND (YEAR IS NULL OR YEAR=${localThis.selectedYear})`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "SRV_ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let i: number = 0;
              let k: number = -1;
              for (i = 0; i < localThis.serviceList.length; i++) {
                if (localThis.serviceList[i].ID == item.SRV_ID) {
                  k = i;
                  break;
                }
              }
              if (k == -1) {
                let srvItem = {} as serviceItem;
                srvItem.ID = item.SRV_ID;
                if (item.TITLE == undefined) {
                  srvItem.CODE = item.PMS_COI_CODE;
                  srvItem.DESCRIPTION = "";
                } else {
                  srvItem.CODE = item.PMS_COI_CODE + " - " + item.TITLE;
                  srvItem.DESCRIPTION = item.TITLE;
                }

                srvItem.SERVICE_ID = item.SRV_ID;
                srvItem.YEAR = item.YEAR;
                switch (item.MONTH) {
                  case 1:
                    srvItem.JAN = {} as SERVICE_MONTH;
                    srvItem.JAN.SELECTED = true;
                    srvItem.JAN.VALUE = item.VALUE;
                    srvItem.JAN.CURRENCY = item.CURRENCY;
                    break;
                  case 2:
                    srvItem.FEB = {} as SERVICE_MONTH;
                    srvItem.FEB.SELECTED = true;
                    srvItem.FEB.VALUE = item.VALUE;
                    srvItem.FEB.CURRENCY = item.CURRENCY;
                    break;
                  case 3:
                    srvItem.MAR = {} as SERVICE_MONTH;
                    srvItem.MAR.SELECTED = true;
                    srvItem.MAR.VALUE = item.VALUE;
                    srvItem.MAR.CURRENCY = item.CURRENCY;
                    break;
                  case 4:
                    srvItem.APR = {} as SERVICE_MONTH;
                    srvItem.APR.SELECTED = true;
                    srvItem.APR.VALUE = item.VALUE;
                    srvItem.APR.CURRENCY = item.CURRENCY;
                    break;
                  case 5:
                    srvItem.MAY = {} as SERVICE_MONTH;
                    srvItem.MAY.SELECTED = true;
                    srvItem.MAY.VALUE = item.VALUE;
                    srvItem.MAY.CURRENCY = item.CURRENCY;
                    break;
                  case 6:
                    srvItem.JUN = {} as SERVICE_MONTH;
                    srvItem.JUN.SELECTED = true;
                    srvItem.JUN.VALUE = item.VALUE;
                    srvItem.JUN.CURRENCY = item.CURRENCY;
                    break;
                  case 7:
                    srvItem.JUL = {} as SERVICE_MONTH;
                    srvItem.JUL.SELECTED = true;
                    srvItem.JUL.VALUE = item.VALUE;
                    srvItem.JUL.CURRENCY = item.CURRENCY;
                    break;
                  case 8:
                    srvItem.AUG = {} as SERVICE_MONTH;
                    srvItem.AUG.SELECTED = true;
                    srvItem.AUG.VALUE = item.VALUE;
                    srvItem.AUG.CURRENCY = item.CURRENCY;
                    break;
                  case 9:
                    srvItem.SEP = {} as SERVICE_MONTH;
                    srvItem.SEP.SELECTED = true;
                    srvItem.SEP.VALUE = item.VALUE;
                    srvItem.SEP.CURRENCY = item.CURRENCY;
                    break;
                  case 10:
                    srvItem.OCT = {} as SERVICE_MONTH;
                    srvItem.OCT.SELECTED = true;
                    srvItem.OCT.VALUE = item.VALUE;
                    srvItem.OCT.CURRENCY = item.CURRENCY;
                    break;
                  case 11:
                    srvItem.NOV = {} as SERVICE_MONTH;
                    srvItem.NOV.SELECTED = true;
                    srvItem.NOV.VALUE = item.VALUE;
                    srvItem.NOV.CURRENCY = item.CURRENCY;
                    break;
                  case 12:
                    srvItem.DEC = {} as SERVICE_MONTH;
                    srvItem.DEC.SELECTED = true;
                    srvItem.DEC.VALUE = item.VALUE;
                    srvItem.DEC.CURRENCY = item.CURRENCY;
                    break;
                }

                localThis.serviceList.push(srvItem);
              } else {
                switch (item.MONTH) {
                  case 1:
                    localThis.serviceList[k].JAN = {} as SERVICE_MONTH;
                    localThis.serviceList[k].JAN.SELECTED = true;
                    localThis.serviceList[k].JAN.VALUE = item.VALUE;
                    localThis.serviceList[k].JAN.CURRENCY = item.CURRENCY;
                    break;

                  case 2:
                    localThis.serviceList[k].FEB = {} as SERVICE_MONTH;
                    localThis.serviceList[k].FEB.SELECTED = true;
                    localThis.serviceList[k].FEB.VALUE = item.VALUE;
                    localThis.serviceList[k].FEB.CURRENCY = item.CURRENCY;
                    break;
                  case 3:
                    localThis.serviceList[k].MAR = {} as SERVICE_MONTH;
                    localThis.serviceList[k].MAR.SELECTED = true;
                    localThis.serviceList[k].MAR.VALUE = item.VALUE;
                    localThis.serviceList[k].MAR.CURRENCY = item.CURRENCY;
                    break;
                  case 4:
                    localThis.serviceList[k].APR = {} as SERVICE_MONTH;
                    localThis.serviceList[k].APR.SELECTED = true;
                    localThis.serviceList[k].APR.VALUE = item.VALUE;
                    localThis.serviceList[k].APR.CURRENCY = item.CURRENCY;
                    break;
                  case 5:
                    localThis.serviceList[k].MAY = {} as SERVICE_MONTH;
                    localThis.serviceList[k].MAY.SELECTED = true;
                    localThis.serviceList[k].MAY.VALUE = item.VALUE;
                    localThis.serviceList[k].MAY.CURRENCY = item.CURRENCY;
                    break;
                  case 6:
                    localThis.serviceList[k].JUN = {} as SERVICE_MONTH;
                    localThis.serviceList[k].JUN.SELECTED = true;
                    localThis.serviceList[k].JUN.VALUE = item.VALUE;
                    localThis.serviceList[k].JUN.CURRENCY = item.CURRENCY;
                    break;
                  case 7:
                    localThis.serviceList[k].JUL = {} as SERVICE_MONTH;
                    localThis.serviceList[k].JUL.SELECTED = true;
                    localThis.serviceList[k].JUL.VALUE = item.VALUE;
                    localThis.serviceList[k].JUL.CURRENCY = item.CURRENCY;
                    break;
                  case 8:
                    localThis.serviceList[k].AUG = {} as SERVICE_MONTH;
                    localThis.serviceList[k].AUG.SELECTED = true;
                    localThis.serviceList[k].AUG.VALUE = item.VALUE;
                    localThis.serviceList[k].AUG.CURRENCY = item.CURRENCY;
                    break;
                  case 9:
                    localThis.serviceList[k].SEP = {} as SERVICE_MONTH;
                    localThis.serviceList[k].SEP.SELECTED = true;
                    localThis.serviceList[k].SEP.VALUE = item.VALUE;
                    localThis.serviceList[k].SEP.CURRENCY = item.CURRENCY;
                    break;
                  case 10:
                    localThis.serviceList[k].OCT = {} as SERVICE_MONTH;
                    localThis.serviceList[k].OCT.SELECTED = true;
                    localThis.serviceList[k].OCT.VALUE = item.VALUE;
                    localThis.serviceList[k].OCT.CURRENCY = item.CURRENCY;
                    break;
                  case 11:
                    localThis.serviceList[k].NOV = {} as SERVICE_MONTH;
                    localThis.serviceList[k].NOV.SELECTED = true;
                    localThis.serviceList[k].NOV.VALUE = item.VALUE;
                    localThis.serviceList[k].NOV.CURRENCY = item.CURRENCY;
                    break;
                  case 12:
                    localThis.serviceList[k].DEC = {} as SERVICE_MONTH;
                    localThis.serviceList[k].DEC.SELECTED = true;
                    localThis.serviceList[k].DEC.VALUE = item.VALUE;
                    localThis.serviceList[k].DEC.CURRENCY = item.CURRENCY;
                    break;
                }
              }
            });
          }
        }
      );
    },

    // deleteItem(item: any) {
    //   let localThis = this as any;
    //   localThis.showNewSnackbar({
    //     typeName: "error",
    //     text: "To Not be Implemented",
    //   });
    // },
    editActivities(item: any) {
      let localThis: any = this as any;
      localThis.selectedSrv = [];
      localThis.selectedSrvivity = item.ID;
      localThis.selectedSrv.push(item);
    },

    closeBudget() {
      let localThis: any = this as any;
      localThis.getEstimatedYearBudget();
      localThis.getObjectServices();
      localThis.budgetMode = false;
    },

    editBudget(item: any) {
      let localThis: any = this as any;
      localThis.budgetMode = true;
      localThis.editSrvId = item.ID;
      localThis.editSrvDesc = item.CODE;
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
    localThis.selectedSrvivity = null;
    localThis.tblName = "PMS_ACTIVITY";
    localThis.setupHeaders();
    if (localThis.selectedObjectId > 0) {
      localThis.getEstimatedYearBudget();
      localThis.getObjectServices();
    }

    EventBus.$on("closeActivityDetails", (data: any) => {
      localThis.showSrvDetails = false;
      localThis.reloadActivities();
      EventBus.$emit("showSrvTabs");
      // EventBus.$emit("sectActiveTab", "ACTIVITIES");
    });

    EventBus.$on("reloadActivities", (data: any) => {
      localThis.reloadActivities(data);
    });
  },
  filters: {
    subStr: function (string: any) {
      return string.substring(0, 15) + "...";
    },
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
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getEstimatedYearBudget();
      localThis.getObjectServices();
      localThis.setupHeaders();
    },
    // showActivityDetails(to: boolean) {
    //   let localThis: any = this as any;
    //   localThis.showSrvDetails = to;
    // },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getEstimatedYearBudget();
      localThis.getObjectServices();
    },

    editMode(to: boolean, from: boolean) {},
  },
});
</script>
<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
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
  border-width: 1px;
  border-color: gainsboro;
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
