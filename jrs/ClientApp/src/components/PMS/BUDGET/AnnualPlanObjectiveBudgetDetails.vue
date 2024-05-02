<template>
  <div>
    <div v-if="!showObjDetails">
      <!-- SRV SELECTION-->
      <v-row>
        <v-col :cols="6">
          <v-select
            v-model="selectedYear"
            :items="budgetYear"
            :label="'Budget Year'"
            item-text="text"
            item-value="value"
            @change="yearChanged"
          ></v-select>
        </v-col>
        <v-col :cols="2"></v-col>
        <v-col :cols="4">
          <!-- <label id="lbl" :class="'v-label theme--light'">Estimated Year Budget: </label>
          <currency-input
            v-model="estimatedYearBudget.BUDGET"
            :readonly="false"
            v-bind="options"
            :currency="'USD'"
            :class="'Budget'"
            aria-labelledby="lbl"
            :rules="false"
          ></currency-input> -->
        </v-col>
      </v-row>
      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headers"
            :items="objectiveList"
            item-key="IDX"
            sort-by="['objective', 'service']"
            multi-sort
            :search="tableFilter"
            :items-per-page="1000"
            :key="keyRnd"
            style="
               {
                'font-size':'14px','width': '85px';
                overflow-x: auto !important;
              }
            "
            class="text-capitalize"
            v-model="selectedObj"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h3>{{ $t("datasource.pms.annual-plan.objectives.budget") }}</h3>

                <v-spacer></v-spacer>
                <v-dialog
                  v-model="budgetMode"
                  persistent
                  retain-focus
                  :scrollable="true"
                  :overlay="false"
                  transition="dialog-transition"
                >
                  <item-budget
                    v-model="resourceId"
                    @UpdateBudgetItem="closeBudgetDialogReload"
                    @CloseDialog="closeBudgetDialog"
                    :selectedActivityId="selectedActivityId"
                    :resource="selectedResource"
                    :resource_id="selectedResourceId"
                    :selectedActivityItemId="selectedActivityItemId"
                    :key="Math.ceil(Math.random() * 1000)"
                  ></item-budget>
                </v-dialog>
                <v-spacer></v-spacer>
                <a>Download Data </a>
                <v-img
                  alt="Download Data"
                  class="shrink mr-2"
                  contain
                  :src="require('../../../assets/download-csv-file-icon.png')"
                  transition="scale-transition"
                  width="40"
                  @click="downloadCSV"
                  style="cursor: pointer"
                />

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
              <tr v-for="(item, index) in items" :key="item.IDX">
                <td :class="[edited[index] == true ? 'editClass' : 'descClass']">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.OBJ_DESC | subStr }}</span>
                    </template>
                    <span>{{ item.OBJ_DESC }}</span>
                  </v-tooltip>
                </td>
                <!-- <td :class="[edited[index] == true ? 'editClass' : 'descClass']">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.SRV_TITLE | subStr }}</span>
                    </template>
                    <span>{{ item.SRV_TITLE }}</span>
                  </v-tooltip>
                </td> -->
                <td :class="[edited[index] == true ? 'editClass' : 'descClass']">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.ACT_DESC | subStr }}</span>
                    </template>
                    <span>{{ item.ACT_DESC }}</span>
                  </v-tooltip>
                </td>
                <!-- <td :class="[edited[index] == true ? 'editClass' : 'descClass']">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_COI_CODE | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_COI_CODE }}</span>
                  </v-tooltip>
                </td> -->
                <!-- <td :class="[edited[index] == true ? 'editClass' : 'descClass']">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_COI_DESCRIPTION | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_COI_DESCRIPTION }}</span>
                  </v-tooltip>
                </td> -->
                <td :class="[edited[index] == true ? 'editClass' : 'descClass']">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_TOS_CODE | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_TOS_CODE }}</span>
                  </v-tooltip>
                </td>
                <!-- <td :class="[edited[index] == true ? 'editClass' : 'descClass']">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_TOS_DESCRIPTION | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_TOS_DESCRIPTION }}</span>
                  </v-tooltip>
                </td> -->
                <td :class="[edited[index] == true ? 'editClass' : 'descClass']">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.RESOURCE | subStr }}</span>
                    </template>
                    <span>{{ item.RESOURCE }}</span>
                  </v-tooltip>
                </td>
                <td :class="[edited[index] == true ? 'editClass' : 'descClass']">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.SHOPPING_ELEMENT | subStr }}</span>
                    </template>
                    <span>{{ item.SHOPPING_ELEMENT }}</span>
                  </v-tooltip>
                </td>
                <td :class="[edited[index] == true ? 'editClass' : 'descClass']">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_JRS_COA_CODE | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_JRS_COA_CODE }}</span>
                  </v-tooltip>
                </td>
                <td :class="[edited[index] == true ? 'editClass' : 'descClass']">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_JRS_COA_DESCRIPTION | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_JRS_COA_DESCRIPTION }}</span>
                  </v-tooltip>
                </td>
                <td :class="[edited[index] == true ? 'editClassWritable' : 'descClass']">
                  <v-text-field
                    v-if="edited[index]"
                    v-model="item.PMS_COA_CUSTOM_DESC"
                    hide-details
                  ></v-text-field>

                  <v-tooltip bottom v-if="!edited[index]">
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_COA_CUSTOM_DESC | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_COA_CUSTOM_DESC }}</span>
                  </v-tooltip>
                </td>
                <td :class="[edited[index] == true ? 'editClassWritable' : 'descClass']">
                  <v-select
                    v-if="edited[index]"
                    v-model="item.PMS_DEPARTMENT"
                    :items="department"
                    :label="$t('datasource.pms.annual-plan.objectives.department')"
                    item-text="text"
                    item-value="value"
                    :rules="[(v) => !!v || 'Item is required']"
                    required
                  ></v-select>

                  <v-tooltip bottom v-if="!edited[index]">
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_DEPARTMENT_DESC | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_DEPARTMENT_DESC }}</span>
                  </v-tooltip>
                </td>
                <!-- <td :class="[edited[index] == true ? 'editClassWritable' : 'descClass']">
                  <v-select
                    v-if="edited[index]"
                    v-model="item.PMS_FUNCTION_CODE"
                    :items="functioncode"
                    :label="$t('datasource.pms.annual-plan.objectives.function-code')"
                    item-text="text"
                    item-value="value"
                    :rules="[(v) => !!v || 'Item is required']"
                    required
                  ></v-select>

                  <v-tooltip bottom v-if="!edited[index]">
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_FUNCTION_CODE_DESC | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_FUNCTION_CODE_DESC }}</span>
                  </v-tooltip>
                </td> -->
                <td :class="[edited[index] == true ? 'editClassWritable' : 'descClass']">
                  <v-select
                    v-if="edited[index]"
                    v-model="item.PMS_SUB_TRANS"
                    :items="subtrans"
                    :label="$t('datasource.pms.annual-plan.objectives.sub-trans')"
                    item-text="text"
                    item-value="value"
                    required
                  ></v-select>
                  <v-tooltip bottom v-if="!edited[index]">
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_SUB_TRANS | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_SUB_TRANS }}</span>
                  </v-tooltip>
                </td>
                <td :class="[edited[index] == true ? 'editClassWritable' : 'descClass']">
                  <v-select
                    v-if="edited[index]"
                    v-model="item.PMS_UNIT_TYPE"
                    :items="unittype"
                    :label="$t('datasource.pms.annual-plan.objectives.item-number-type')"
                    item-text="text"
                    item-value="value"
                    :rules="[(v) => !!v || 'Item is required']"
                    required
                  ></v-select>

                  <v-tooltip bottom v-if="!edited[index]">
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_UNIT_TYPE_DESC | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_UNIT_TYPE_DESC }}</span>
                  </v-tooltip>
                </td>

                <td :class="[edited[index] == true ? 'editClassWritable' : 'descClass']">
                  <v-text-field
                    v-if="edited[index]"
                    v-model="item.PMS_ITEM_NUMBER"
                    @keypress="isNumber($event)"
                    @change="itemNumberChanged(item, index)"
                    hide-details
                  ></v-text-field>

                  <v-tooltip bottom v-if="!edited[index]">
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_ITEM_NUMBER | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_ITEM_NUMBER }}</span>
                  </v-tooltip>
                </td>

                <td :class="[edited[index] == true ? 'editClassWritable' : 'descClass']">
                  <v-select
                    v-if="edited[index]"
                    v-model="item.PMS_UNIT_TYPE2"
                    :items="unittype2"
                    :label="$t('datasource.pms.annual-plan.objectives.item-number-type')"
                    item-text="text"
                    item-value="value"
                    :rules="[(v) => !!v || 'Item is required']"
                    required
                  ></v-select>

                  <v-tooltip bottom v-if="!edited[index]">
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_UNIT_TYPE_DESC2 | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_UNIT_TYPE_DESC2 }}</span>
                  </v-tooltip>
                </td>
                <td :class="[edited[index] == true ? 'editClassWritable' : 'descClass']">
                  <v-text-field
                    v-if="edited[index]"
                    v-model="item.PMS_ITEM_NUMBER2"
                    @keypress="isNumber($event)"
                    @change="itemNumberChanged2(item, index)"
                    hide-details
                  ></v-text-field>

                  <v-tooltip bottom v-if="!edited[index]">
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_ITEM_NUMBER2 | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_ITEM_NUMBER2 }}</span>
                  </v-tooltip>
                </td>

                <td :class="[edited[index] == true ? 'editClassWritable' : 'descClass']">
                  <currency-input
                    v-model="item.PMS_ITEM_PRICE"
                    v-bind="options"
                    :currency="item.PMS_CURRENCY"
                    :class="'valuedBudget'"
                    :readonly="false"
                    @change="itemPriceChanged(item, index)"
                  ></currency-input>
                </td>
                <!-- :class="
                    item.MONTH_DETAILS[i] != undefined && item.MONTH_DETAILS[i].SELECTED
                      ? 'selectedCell'
                      : 'notSelectedCell'
                  " -->
                <td
                  v-for="(n, i) in 12"
                  :key="i"
                  :class="[edited[index] == true ? 'editClass' : 'descClass']"
                >
                  <currency-input
                    v-if="
                      item.MONTH_DETAILS[i] != undefined &&
                      item.MONTH_DETAILS[i].VALUE != undefined &&
                      item.MONTH_DETAILS[i].CURRENCY != undefined
                    "
                    v-model="item.MONTH_DETAILS[i].VALUE"
                    v-bind="options"
                    :currency="item.MONTH_DETAILS[i].CURRENCY"
                    :class="'valuedBudget'"
                    :readonly="false"
                  ></currency-input>
                </td>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="editRow(index)" v-on="on"
                      >mdi-pencil</v-icon
                    >
                  </template>
                  <span>{{ $t("datasource.pms.annual-plan.objectives.edit-row") }}</span>
                </v-tooltip>

                <!-- <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-icon small class="mr-2" @click="editActivities(item);" v-on="on">mdi-chart-line</v-icon>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.edit-service-activities') }}</span>
              </v-tooltip>-->
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="commitRow(item, index)" v-on="on"
                      >mdi-floppy</v-icon
                    >
                  </template>
                  <span>{{ $t("datasource.pms.annual-plan.objectives.save-row") }}</span>
                </v-tooltip>
                <v-tooltip bottom v-if="item.PMS_JRS_COA_ID != undefined">
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="budgetResource(item)" v-on="on"
                      >mdi-wallet</v-icon
                    >
                  </template>
                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.budget-item")
                  }}</span>
                </v-tooltip>
                <!-- <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                      >mdi-delete</v-icon
                    >
                  </template>
                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.delete-service")
                  }}</span>
                </v-tooltip> -->
              </tr>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import JrsTable from "../../JrsTable.vue";
import { JrsHeader } from "../../../models/JrsTable";
import ObjectiveBudget from "../ACTIVITIES/AnnualPlanActivityForm.vue";
import ItemBudget from "../BUDGET/AnnualPlanItemBudgetForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import FormMixin from "../../../mixins/FormMixin";
import NavMixin from "../../../mixins/NavMixin";
const Papa = require("papaparse");
const fs = require("fs");
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
import {
  ImsApi,
  PmsAnnualPlanApi,
  ImsLookupsApi,
  Configuration,
  NavIntegrationApi,
  NavBudget1,
} from "../../../axiosapi";
interface OBJECTIVE_MONTH {
  SELECTED: boolean;
  VALUE: any;
  CURRENCY: string;
}
interface objectItem {
  IDX: number;
  AP_ID: number;
  OBJ_ID: number;
  SRV_ID: number;
  ACT_ID: number;
  PMS_COI_ID: number;
  PMS_TOS_ID: number;
  PMS_JRS_COA_ID: number;
  OBJ_DESC: string;
  SRV_TITLE: string;
  ACT_DESC: string;
  PMS_COI_CODE: string;
  PMS_COI_DESCRIPTION: string;
  PMS_TOS_CODE: string;
  PMS_TOS_DESCRIPTION: string;
  PMS_JRS_COA_CODE: string;
  PMS_JRS_COA_DESCRIPTION: string;
  PMS_COA_CUSTOM_DESC: string;
  PMS_DEPARTMENT: string;
  PMS_DEPARTMENT_DESC: string;
  PMS_FUNCTION_CODE: string;
  PMS_FUNCTION_CODE_DESC: string;
  PMS_SUB_TRANS: string;
  PMS_UNIT_TYPE: string;
  PMS_UNIT_TYPE_DESC: string;
  PMS_UNIT_TYPE2: string;
  PMS_UNIT_TYPE_DESC2: string;
  PMS_ITEM_NUMBER: number;
  PMS_ITEM_NUMBER2: number;
  PMS_ITEM_PRICE: number;
  PMS_CURRENCY: string;
  RESOURCE: string;
  SHOPPING_ELEMENT: string;
  YEAR: number;
  MONTH: number;
  MONTH_DETAILS: OBJECTIVE_MONTH[];
  AI_ID: number;
}

interface objectItemExport {
  OBJECTIVE: string;
  SERVICE: string;
  ACTIVITY: string;
  CATEGORY_OF_INT_CODE: string;
  CATEGORY_OF_INT_DESC: string;
  TYPE_OF_SERVICE_CODE: string;
  TYPE_OF_SERVICE_DESC: string;
  CHART_OF_ACC_CODE: string;
  CHART_OF_ACC_DESC: string;
  CHART_OF_ACC_CUSTOM_DESC: string;
  DEPARTMENT_DESC: string;
  FUNCTION_CODE_DESC: string;
  UNIT_TYPE_DESC: string;
  UNIT_TYPE_DESC2: string;
  SUB_TRANS: string;
  ITEM_NUMBER: number;
  ITEM_NUMBER2: number;
  ITEM_PRICE: number;
  RESOURCE: string;
  SHOPPING_ELEMENT: string;

  JAN: string;
  FEB: string;
  MAR: string;
  APR: string;
  MAY: string;
  JUN: string;
  JUL: string;
  AUG: string;
  SEP: string;
  OCT: string;
  NOV: string;
  DEC: string;
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
    "item-budget": ItemBudget,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
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
  mixins: [FormMixin, NavMixin],
  data() {
    return {
      department: [],
      edited: [],
      unittype: [],
      unittype2: [],
      subtrans: [],
      keyRnd: 0,
      bdg: 0,
      estimatedYearBudget: {},
      electedYear: "",
      budgetYear: [],
      resourceId: {},
      json_data: "",
      selectedActivityId: 0,
      selectedActivityItemId: 0,
      selectedResource: "",
      selectedResourceId: 0,
      editObjId: 0,
      budgetMode: false,
      showActTabs: true,
      editObjDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showObjDetails: false,
      selectedObj: [],
      tableFilter: "",
      tblName: null,
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
      objectiveList: [] as objectItem[],
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
    editRow(index: number) {
      let localThis: any = this as any;
      localThis.edited[index] = true;
      localThis.keyRnd = Math.ceil(Math.random() * 1000);
    },
    itemNumberChanged(item: any, index: number) {
      let localThis: any = this as any;
      var tmp: string = item.PMS_COA_CUSTOM_DESC;
      let i: number;
      for (i = 0; i < 12; i++) {
        if (item.MONTH_DETAILS[i].VALUE != undefined) {
          item.MONTH_DETAILS[i].VALUE =
            item.PMS_ITEM_NUMBER * item.PMS_ITEM_NUMBER2 * item.PMS_ITEM_PRICE;
        }
      }
      item.PMS_COA_CUSTOM_DESC = tmp + " ";
    },
    itemNumberChanged2(item: any, index: number) {
      let localThis: any = this as any;
      var tmp: string = item.PMS_COA_CUSTOM_DESC;
      let i: number;
      for (i = 0; i < 12; i++) {
        if (item.MONTH_DETAILS[i].VALUE != undefined) {
          item.MONTH_DETAILS[i].VALUE =
            item.PMS_ITEM_NUMBER * item.PMS_ITEM_NUMBER2 * item.PMS_ITEM_PRICE;
        }
      }
      item.PMS_COA_CUSTOM_DESC = tmp + " "; //force grid to refresh
    },
    itemPriceChanged(item: any, index: number) {
      let localThis: any = this as any;
      var tmp: string = item.PMS_COA_CUSTOM_DESC;
      let i: number;
      for (i = 0; i < 12; i++) {
        if (item.MONTH_DETAILS[i].VALUE != undefined) {
          item.MONTH_DETAILS[i].VALUE =
            item.PMS_ITEM_NUMBER * item.PMS_ITEM_NUMBER2 * item.PMS_ITEM_PRICE;
        }
      }
      item.PMS_COA_CUSTOM_DESC = tmp + " "; //force grid to refresh
    },
    commitRow(item: any, index: number) {
      let localThis: any = this as any;
      localThis.saveBudget(item);
      localThis.edited[index] = false;
      localThis.keyRnd = Math.ceil(Math.random() * 1000);
    },

    saveBudget(item: any) {
      let localThis: any = this as any;

      // let isValid = true;
      // isValid = (localThis.$refs.myForm as Vue & {
      //   validate: () => boolean;
      // }).validate();
      // if (!isValid) return;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-activity-budget")
        .toString();

      let id = 0;
      let msg = msgUpd;

      this.$confirm(msg).then((res) => {
        if (res) {
          localThis.showWait = true;
          localThis
            .updateBudgetOnNav(
              "true",
              item.AI_ID,
              localThis.selectedYear,
              localThis.showWait
            )
            .then((resA: any) => {
              localThis.showWait = true;
              let payLoad = {} as any;
              let i: number;
              payLoad.ACTIVITY_ID = item.ACT_ID;
              payLoad.RESOURCE_ID = item.PMS_JRS_COA_ID;
              payLoad.POSITION_ID = 0;
              payLoad.PERCENTAGE = 0;
              payLoad.YEAR = localThis.selectedYear;
              payLoad.SELECTED = "";
              payLoad.DESCRIPTION = item.PMS_COA_CUSTOM_DESC.trim();
              payLoad.UNIT_PRICE = item.PMS_ITEM_PRICE;
              payLoad.TIME_UNIT = ""; //localThis.selected_timingtype;
              payLoad.TIMING = 1; // localThis.timingnumber;
              payLoad.UNIT = item.PMS_ITEM_NUMBER;
              payLoad.UNIT2 = item.PMS_ITEM_NUMBER2;
              payLoad.UNIT_TYPE = item.PMS_UNIT_TYPE;
              payLoad.UNIT_TYPE2 = item.PMS_UNIT_TYPE2;
              payLoad.DEPARTMENT = item.PMS_DEPARTMENT;
              payLoad.FUNCTION_CODE = item.PMS_FUNCTION_CODE;
              payLoad.SUB_TRANS = item.PMS_SUB_TRANS;
              payLoad.CURRENCY = item.PMS_CURRENCY;
              var budgetfound = false;
              for (i = 0; i < 12; i++) {
                if (
                  item.MONTH_DETAILS[i].VALUE != undefined &&
                  item.MONTH_DETAILS[i].VALUE != 0
                ) {
                  budgetfound = true;

                  payLoad.SELECTED +=
                    i +
                    1 +
                    ";" +
                    item.MONTH_DETAILS[i].VALUE +
                    "^" +
                    item.PMS_CURRENCY +
                    ":";
                }
              }
              if (budgetfound == false) {
                localThis.showNewSnackbar({
                  typeName: "error",
                  text: "Budget non inserted",
                });
                return;
              }
              payLoad.ACTIVITY_ITEM_REL_ID = item.AI_ID;
              let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_ACTIVITY_BUDGET_V3",
                actionType: SqlActionType.NUMBER_0,
                jsonPayload: JSON.stringify(payLoad),
              };

              localThis
                .execSPCall(savePayload)
                .then((res: any) => {
                  localThis.showNewSnackbar({
                    typeName: "success",
                    text: res.message,
                  }); // Feedback to user
                })
                .then((res: any) => {
                  localThis
                    .updateBudgetOnNav(
                      "false",
                      item.AI_ID,
                      localThis.selectedYear,
                      localThis.showWait
                    )
                    .then((res1: any) => {
                      // localThis.updateValue(null);
                    });
                })
                .catch((err: any) => {
                  localThis.showWait = false;
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message,
                  }); // Feedback to user
                });
            });
        }
      });
    },

    /*   interface objectItemExport {
  OBJECTIVE: string;
  SERVICE: string;
  ACTIVITY: string;
  "CATEGORY OF INT CODE": string;
  "CATEGORY OF INT DESC": string;
  "TYPE OF SERVICE CODE": string;
  "TYPE OF SERVICE DESC": string;
  "CHART OF ACC. CODE": string;
  "CHART OF ACC. DESC": string;
  "CHART OF ACC. CUSTOM DESC": string;
  JAN: string;
  FEB: string;
  MAR: string;
  APR: string;
  MAY: string;
  JUN: string;
  JUL: string;
  AUG: string;
  SEP: string;
  OCT: string;
  NOV: string;
  DEC: string;
}
*/

    downloadCSV() {
      let localThis: any = this as any;

      var exportList = [] as objectItemExport[];
      var i: number = 0;
      for (i = 0; i < localThis.objectiveList.length; i++) {
        var exportItem = {} as objectItemExport;
        var itm = {} as objectItem;
        itm = localThis.objectiveList[i];
        exportItem.OBJECTIVE = localThis.removeUndefined(itm.OBJ_DESC);
        exportItem.SERVICE = localThis.removeUndefined(itm.SRV_TITLE);
        exportItem.ACTIVITY = localThis.removeUndefined(itm.ACT_DESC);
        exportItem.CATEGORY_OF_INT_CODE = localThis.removeUndefined(itm.PMS_COI_CODE);
        exportItem.CATEGORY_OF_INT_DESC = localThis.removeUndefined(
          itm.PMS_COI_DESCRIPTION
        );
        exportItem.RESOURCE = localThis.removeUndefined(itm.RESOURCE);
        exportItem.SHOPPING_ELEMENT = localThis.removeUndefined(itm.SHOPPING_ELEMENT);
        exportItem.TYPE_OF_SERVICE_CODE = localThis.removeUndefined(itm.PMS_TOS_CODE);
        exportItem.TYPE_OF_SERVICE_DESC = localThis.removeUndefined(
          itm.PMS_COI_DESCRIPTION
        );
        exportItem.CHART_OF_ACC_CODE = localThis.removeUndefined(itm.PMS_JRS_COA_CODE);
        exportItem.CHART_OF_ACC_DESC = localThis.removeUndefined(
          itm.PMS_JRS_COA_DESCRIPTION
        );
        exportItem.CHART_OF_ACC_CUSTOM_DESC = localThis.removeUndefined(
          itm.PMS_COA_CUSTOM_DESC.trim()
        );
        exportItem.DEPARTMENT_DESC = localThis.removeUndefined(itm.PMS_DEPARTMENT_DESC);
        exportItem.FUNCTION_CODE_DESC = localThis.removeUndefined(
          itm.PMS_FUNCTION_CODE_DESC
        );
        exportItem.UNIT_TYPE_DESC = localThis.removeUndefined(itm.PMS_UNIT_TYPE_DESC);
        exportItem.UNIT_TYPE_DESC2 = localThis.removeUndefined(itm.PMS_UNIT_TYPE_DESC2);
        exportItem.SUB_TRANS = localThis.removeUndefined(itm.PMS_SUB_TRANS);
        exportItem.ITEM_NUMBER = localThis.removeUndefined(itm.PMS_ITEM_NUMBER);
        exportItem.ITEM_NUMBER2 = localThis.removeUndefined(itm.PMS_ITEM_NUMBER2);
        exportItem.ITEM_PRICE = localThis.removeUndefined(itm.PMS_ITEM_PRICE);

        exportItem.JAN = localThis.removeUndefined(itm.MONTH_DETAILS[0].VALUE + "");
        exportItem.FEB = localThis.removeUndefined(itm.MONTH_DETAILS[1].VALUE + "");
        exportItem.MAR = localThis.removeUndefined(itm.MONTH_DETAILS[2].VALUE + "");
        exportItem.APR = localThis.removeUndefined(itm.MONTH_DETAILS[3].VALUE + "");
        exportItem.MAY = localThis.removeUndefined(itm.MONTH_DETAILS[4].VALUE + "");
        exportItem.JUN = localThis.removeUndefined(itm.MONTH_DETAILS[5].VALUE + "");
        exportItem.JUL = localThis.removeUndefined(itm.MONTH_DETAILS[6].VALUE + "");
        exportItem.AUG = localThis.removeUndefined(itm.MONTH_DETAILS[7].VALUE + "");
        exportItem.SEP = localThis.removeUndefined(itm.MONTH_DETAILS[8].VALUE + "");
        exportItem.OCT = localThis.removeUndefined(itm.MONTH_DETAILS[9].VALUE + "");
        exportItem.NOV = localThis.removeUndefined(itm.MONTH_DETAILS[10].VALUE + "");
        exportItem.DEC = localThis.removeUndefined(itm.MONTH_DETAILS[11].VALUE + "");
        exportList.push(exportItem);
      }

      var js = JSON.stringify(exportList);

      var blob = new Blob([Papa.unparse(js)], {
        type: "text/csv;charset=utf-8;",
      });

      var link = document.createElement("a");

      var url = URL.createObjectURL(blob);
      link.setAttribute("href", url);
      link.setAttribute("download", "budget.csv");
      link.style.visibility = "hidden";
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    },
    reloadActivities(item: any) {
      let localThis = this as any;
      localThis.getEstimatedYearBudget();
      localThis.getAnnualPlanObjectives();
    },
    yearChanged() {
      let localThis: any = this as any;
      localThis.getEstimatedYearBudget();
      localThis.getAnnualPlanObjectives();
    },
    getEstimatedYearBudget() {
      let localThis: any = this as any;
      localThis.estimatedYearBudget = {} as any;
      let view: string = "PMS_VI_ANNUAL_PLAN_YEAR_BUDGET";
      let wherecond: string = `AP_ID = ${localThis.selectedObjectId} AND YEAR=${localThis.selectedYear}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_YEAR_BUDGET_VER";
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
              localThis.estimatedYearBudget.ID = item.PMS_ACTIVITY_YEAR_BUDGET_ID;
              localThis.estimatedYearBudget.YEAR = item.YEAR;
              localThis.estimatedYearBudget.BUDGET = item.BUDGET;
              localThis.estimatedYearBudget.ACTIVITY_ID = item.PMS_ACTIVITY_ID;
              localThis.estimatedYearBudget.CURRENCY = item.CURRENCY;
              localThis.keyRnd = Math.ceil(Math.random() * 1000);
              localThis.bdg = item.BUDGET;
            });
          }
        });
    },

    getAnnualPlanObjectives() {
      let localThis: any = this as any;
      localThis.objectiveList = [] as objectItem[];
      localThis.edited = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_OBJECTIVE_BUDGET_DETAILS_V2";
      let wherecond: string = `AP_ID = ${localThis.selectedObjectId} AND (YEAR IS NULL OR YEAR=${localThis.selectedYear})`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_OBJECTIVE_BUDGET_DETAILS_VER_V2";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "OBJ_ID,SRV_ID,ACT_ID,PMS_COI_ID,PMS_TOS_ID,PMS_JRS_COA_ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let i: number = 0;
              let k: number = -1;

              for (i = 0; i < localThis.objectiveList.length; i++) {
                if (
                  localThis.objectiveList[i].AI_ID == item.AI_ID &&
                  localThis.objectiveList[i].OBJ_ID == item.OBJ_ID &&
                  localThis.objectiveList[i].SRV_ID == item.SRV_ID &&
                  localThis.objectiveList[i].ACT_ID == item.ACT_ID &&
                  localThis.objectiveList[i].PMS_COI_ID == item.PMS_COI_ID &&
                  localThis.objectiveList[i].PMS_TOS_ID == item.PMS_TOS_ID &&
                  localThis.objectiveList[i].PMS_JRS_COA_ID == item.PMS_JRS_COA_ID
                ) {
                  k = i;
                  break;
                }
              }
              if (k == -1) {
                let objItem = {} as objectItem;
                objItem.AI_ID = item.AI_ID;
                objItem.OBJ_ID = item.OBJ_ID;
                objItem.SRV_ID = item.SRV_ID;
                objItem.ACT_ID = item.ACT_ID;
                objItem.PMS_COI_ID = item.PMS_COI_ID;
                objItem.PMS_TOS_ID = item.PMS_TOS_ID;
                objItem.PMS_JRS_COA_ID = item.PMS_JRS_COA_ID;

                objItem.OBJ_DESC = item.OBJ_DESC + "";
                objItem.SRV_TITLE = item.SRV_TITLE + "";
                objItem.ACT_DESC = item.ACT_DESC + "";

                objItem.PMS_COI_CODE = item.PMS_COI_CODE + "";
                objItem.PMS_COI_DESCRIPTION = item.PMS_COI_DESCRIPTION + "";
                objItem.PMS_TOS_CODE = item.PMS_TOS_CODE + "";
                objItem.PMS_TOS_DESCRIPTION = item.PMS_TOS_DESCRIPTION + "";
                objItem.PMS_JRS_COA_CODE = item.PMS_JRS_COA_CODE + "";
                objItem.PMS_JRS_COA_DESCRIPTION = item.PMS_JRS_COA_DESCRIPTION + "";
                objItem.PMS_COA_CUSTOM_DESC = item.PMS_COA_CUSTOM_DESC + "";
                objItem.PMS_DEPARTMENT = item.PMS_DEPARTMENT + "";
                objItem.PMS_DEPARTMENT_DESC = item.PMS_DEPARTMENT_DESC + "";
                objItem.PMS_FUNCTION_CODE = item.PMS_FUNCTION_CODE + "";
                objItem.PMS_FUNCTION_CODE_DESC = item.PMS_FUNCTION_CODE_DESC + "";
                objItem.PMS_SUB_TRANS = item.PMS_SUB_TRANS + "";
                objItem.PMS_ITEM_NUMBER = item.PMS_ITEM_NUMBER;
                objItem.PMS_ITEM_NUMBER2 = item.PMS_ITEM_NUMBER2;
                objItem.PMS_ITEM_PRICE = item.PMS_ITEM_PRICE;
                objItem.PMS_CURRENCY = item.PMS_CURRENCY;
                objItem.PMS_UNIT_TYPE = item.PMS_UNIT_TYPE + "";
                objItem.PMS_UNIT_TYPE_DESC = item.PMS_UNIT_TYPE_DESC + "";
                objItem.PMS_UNIT_TYPE2 = item.PMS_UNIT_TYPE2 + "";
                objItem.PMS_UNIT_TYPE_DESC2 = item.PMS_UNIT_TYPE_DESC2 + "";
                objItem.RESOURCE = item.RESOURCE + "";
                objItem.SHOPPING_ELEMENT = item.SHOPPING_ELEMENT + "";

                objItem.YEAR = item.YEAR;
                objItem.MONTH_DETAILS = [] as OBJECTIVE_MONTH[];
                let x: number = 0;
                for (x = 0; x < 12; x++) {
                  objItem.MONTH_DETAILS.push({} as OBJECTIVE_MONTH);
                }

                var MonthDetails = {} as OBJECTIVE_MONTH;
                MonthDetails.SELECTED = true;
                MonthDetails.VALUE = item.VALUE;
                MonthDetails.CURRENCY = item.CURRENCY;
                objItem.MONTH_DETAILS[item.MONTH - 1] = MonthDetails;
                objItem.IDX = localThis.objectiveList.length + 1;
                localThis.objectiveList.push(objItem);
                localThis.edited.push(false);
              } else {
                localThis.objectiveList[k].MONTH_DETAILS[
                  item.MONTH - 1
                ] = {} as OBJECTIVE_MONTH;
                localThis.objectiveList[k].MONTH_DETAILS[item.MONTH - 1].SELECTED = true;
                localThis.objectiveList[k].MONTH_DETAILS[item.MONTH - 1].VALUE =
                  item.VALUE;
                localThis.objectiveList[k].MONTH_DETAILS[item.MONTH - 1].CURRENCY =
                  item.CURRENCY;
              }
            });
            localThis.json_data =
              "{   'glossary': {   'title': 'example glossary', 		'GlossDiv': {     'title': 'S', 			'GlossList': { 'GlossEntry': { 'ID': 'SGML', 					'SortAs': 'SGML', 					'GlossTerm': 'Standard Generalized Markup Language', 					'Acronym': 'SGML', 					'Abbrev': 'ISO 8879:1986', 					'GlossDef': {   'para': 'A meta-markup language, used to create markup languages such as DocBook.', 						'GlossSeeAlso': ['GML', 'XML'] }, 					'GlossSee': 'markup' }     }   }   } }"; // JSON.stringify(localThis.objectiveList);
          }
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
      localThis.selectedObj = [];
      localThis.selectedObj.push(item);
    },

    closeBudget() {
      let localThis: any = this as any;
      localThis.getEstimatedYearBudget();
      localThis.getAnnualPlanObjectives();
      localThis.budgetMode = false;
    },
    closeBudgetDialogReload(item: string) {
      let localThis: any = this as any;
      localThis.budgetMode = false;
      localThis.getEstimatedYearBudget();
      localThis.getAnnualPlanObjectives();
    },
    closeBudgetDialog(item: string) {
      let localThis: any = this as any;
      localThis.budgetMode = false;
    },
    removeUndefined: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      else return string;
    },
    // editBudget(item: any) {
    //   let localThis: any = this as any;
    //   localThis.budgetMode = true;
    //   localThis.editObjId = item.IDX;
    //   localThis.editObjDesc = item.DESCRIPTION;
    //   localThis.selectedActivityId = item.ACT_ID;
    // },
    budgetResource(item: any) {
      let localThis = this as any;

      localThis.selectedResource = item.PMS_JRS_COA_DESCRIPTION;
      localThis.selectedResourceId = item.PMS_JRS_COA_ID;
      localThis.editObjId = item.IDX;
      localThis.editObjDesc = item.DESCRIPTION;
      localThis.selectedActivityId = item.ACT_ID;
      localThis.selectedActivityItemId = item.AI_ID;

      localThis.budgetMode = !localThis.budgetMode;
    },
    setupHeaders() {
      let localThis: any = this as any;
      let descWidth: string;
      descWidth = "140px";

      let tmp: JrsHeader[] = [
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.objective")
            .toString(),
          value: "OBJ_DESC",
          align: "center",
          divider: true,
          width: descWidth,
        },
        // {
        //   text: this.$i18n.t("datasource.pms.annual-plan.objectives.service").toString(),
        //   value: "SRV_TITLE",
        //   align: "center",
        //   divider: true,
        //   width: descWidth,
        // },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.activity").toString(),
          value: "ACT_DESC",
          align: "center",
          divider: true,
          width: descWidth,
        },

        // {
        //   text: this.$i18n.t("datasource.pms.annual-plan.objectives.coi-code").toString(),
        //   value: "PMS_COI_CODE",
        //   align: "center",
        //   divider: true,
        //   width: descWidth,
        // },
        // {
        //   text: this.$i18n.t("datasource.pms.annual-plan.objectives.coi-desc").toString(),
        //   value: "PMS_COI_DESCRIPTION",
        //   align: "center",
        //   divider: true,
        //   width: descWidth,
        // },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.tos-code").toString(),
          value: "PMS_TOS_CODE",
          align: "center",
          divider: true,
          width: descWidth,
        },
        // {
        //   text: this.$i18n.t("datasource.pms.annual-plan.objectives.tos-desc").toString(),
        //   value: "PMS_TOS_DESCRIPTION",
        //   align: "center",
        //   divider: true,
        //   width: descWidth,
        // },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.resource").toString(),
          value: "RESOURCE",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.staff-position")
            .toString(),
          value: "SHOPPING_ELEMENT",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.coa-code").toString(),
          value: "PMS_JRS_COA_CODE",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.coa-desc").toString(),
          value: "PMS_JRS_COA_DESCRIPTION",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.description")
            .toString(),
          value: "PMS_COA_CUSTOM_DESC",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.department")
            .toString(),
          value: "PMS_DEPARTMENT",
          align: "center",
          divider: true,
          width: descWidth,
        },
        // {
        //   text: this.$i18n
        //     .t("datasource.pms.annual-plan.objectives.function-code")
        //     .toString(),
        //   value: "PMS_FUNCTION_CODE",
        //   align: "center",
        //   divider: true,
        //   width: descWidth,
        // },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.sub-trans")
            .toString(),
          value: "PMS_SUB_TRANS",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.item-number-type-1")
            .toString(),
          value: "PMS_UNIT_TYPE",
          align: "center",
          divider: true,
          width: descWidth,
        },

        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.item-number-unit-1")
            .toString(),
          value: "PMS_ITEM_NUMBER",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.item-number-type-2")
            .toString(),
          value: "PMS_UNIT_TYPE2",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.item-number-unit-2")
            .toString(),
          value: "PMS_ITEM_NUMBER2",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.item-budget")
            .toString(),
          value: "PMS_ITEM_PRICE",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.jan").toString(),
          value: "jan",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.feb").toString(),
          value: "feb",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.mar").toString(),
          value: "mar",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.apr").toString(),
          value: "apr",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.may").toString(),
          value: "may",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.jun").toString(),
          value: "jun",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.jul").toString(),
          value: "jul",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.aug").toString(),
          value: "aug",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.sep").toString(),
          value: "sep",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.oct").toString(),
          value: "oct",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.nov").toString(),
          value: "nov",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.dec").toString(),
          value: "dec",
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

    getList(listName: any) {
      let localThis = this as any;

      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsGetGeneriListByCompanyGet(
          listName,
          localThis.getCurrentOffice.HR_OFFICE_CODE,
          config.baseOptions
        )
        .then((res) => {
          switch (listName) {
            case "UNIT_TYPE1":
              localThis.unittype = res.data;
              break;
            case "UNIT_TYPE2":
              localThis.unittype2 = res.data;
              break;
            case "TIME_UNIT":
              localThis.timingtype = res.data;
              break;
            case "DEPARTMENT":
              localThis.department = res.data;
              break;
            case "FUNCTION_CODE":
              localThis.functioncode = res.data;
              break;
            case "SUB_TRANS":
              localThis.subtrans = res.data;
              break;
          }
          //obj = res.data;
          //alert(res.data[0].pmsTosDescription);
          return res;
        })
        .catch((err) => {
          // console.error(err);
          return [];
        })
        .finally(() => (localThis.isLoading = false));
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
    localThis.getList("DEPARTMENT");
    localThis.getList("FUNCTION_CODE");
    localThis.getList("UNIT_TYPE1");
    localThis.getList("UNIT_TYPE2");
    localThis.getList("SUB_TRANS");
    localThis.tblName = "PMS_ACTIVITY";
    localThis.setupHeaders();
    if (localThis.selectedObjectId > 0) {
      localThis.getEstimatedYearBudget();
      localThis.getAnnualPlanObjectives();
    }

    EventBus.$on("closeActivityDetails", (data: any) => {
      localThis.showObjDetails = false;
      localThis.reloadActivities();
      EventBus.$emit("showObjTabs");
      // EventBus.$emit("sectActiveTab", "ACTIVITIES");
    });

    EventBus.$on("reloadActivities", (data: any) => {
      localThis.reloadActivities(data);
    });
  },
  filters: {
    subStr: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      if (string.length > 25) return string.substring(0, 25) + "...";
      else return string;
    },
  },
  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
    }),
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
      localThis.getAnnualPlanObjectives();
      localThis.setupHeaders();
    },
    // showActivityDetails(to: boolean) {
    //   let localThis: any = this as any;
    //   localThis.showObjDetails = to;
    // },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getEstimatedYearBudget();
      localThis.getAnnualPlanObjectives();
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
  font-size: 10px;
  width: 85px;
}
.descClass {
  font-size: 10px;
  width: 150px !important;
  height: 10px;
  border-style: solid;
  border-width: 1px;
  border-color: whitesmoke;
}
.editClass {
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
.editClassWritable {
  font-size: 10px;
  width: 150px !important;
  height: 10px;
  border-style: solid;
  border-width: 1px;
  border-color: red;
}
tr {
  height: 50px !important;
}
</style>
