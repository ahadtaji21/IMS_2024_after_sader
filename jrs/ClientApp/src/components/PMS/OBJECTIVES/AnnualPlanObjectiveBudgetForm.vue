<template>
  <div>
    <div v-if="!showObjDetails">
      <!-- SRV SELECTION-->
      <v-row>
        <v-col :cols="6">
          <v-select
            :items="budgetYear"
            :label="'Budget Year'"
            item-text="text"
            item-value="value"
          ></v-select>
        </v-col>
      </v-row>
      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headers"
            :items="objectiveList"
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
            v-model="selectedObj"
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
                    <v-card-title>Objective: {{ editObjDesc }}</v-card-title>
                    <v-card-text>
                      <ap-budget
                        :selectedObjectId="editObjId"
                        :key="Math.ceil(Math.random() * 1000)"
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
              <tbody>
                <tr v-for="item in items" :key="item.ID" style="height: 10px">
                  <td style="height: 10px">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <span v-on="on">{{ item.DESCRIPTION | subStr }}</span>
                      </template>
                      <span>{{ item.DESCRIPTION }}</span>
                    </v-tooltip>
                  </td>

                  <td
                    :class="
                      item.MONTH_DETAILS[0] != undefined && item.MONTH_DETAILS[0].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[0] != undefined &&
                        item.MONTH_DETAILS[0].VALUE != undefined &&
                        item.MONTH_DETAILS[0].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[0].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[0].CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MONTH_DETAILS[1] != undefined && item.MONTH_DETAILS[1].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[1] != undefined &&
                        item.MONTH_DETAILS[1].VALUE != undefined &&
                        item.MONTH_DETAILS[1].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[1].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[1].CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MONTH_DETAILS[2] != undefined && item.MONTH_DETAILS[2].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[2] != undefined &&
                        item.MONTH_DETAILS[2].VALUE != undefined &&
                        item.MONTH_DETAILS[2].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[2].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[2].CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MONTH_DETAILS[3] != undefined && item.MONTH_DETAILS[3].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[3] != undefined &&
                        item.MONTH_DETAILS[3].VALUE != undefined &&
                        item.MONTH_DETAILS[3].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[3].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[3].CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MONTH_DETAILS[4] != undefined && item.MONTH_DETAILS[4].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[4] != undefined &&
                        item.MONTH_DETAILS[4].VALUE != undefined &&
                        item.MONTH_DETAILS[4].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[4].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[4].CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MONTH_DETAILS[5] != undefined && item.MONTH_DETAILS[5].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[5] != undefined &&
                        item.MONTH_DETAILS[5].VALUE != undefined &&
                        item.MONTH_DETAILS[5].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[5].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[5].CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MONTH_DETAILS[6] != undefined && item.MONTH_DETAILS[6].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[6] != undefined &&
                        item.MONTH_DETAILS[6].VALUE != undefined &&
                        item.MONTH_DETAILS[6].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[6].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[6].CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MONTH_DETAILS[7] != undefined && item.MONTH_DETAILS[7].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[7] != undefined &&
                        item.MONTH_DETAILS[7].VALUE != undefined &&
                        item.MONTH_DETAILS[7].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[7].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[7].CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MONTH_DETAILS[8] != undefined && item.MONTH_DETAILS[8].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[8] != undefined &&
                        item.MONTH_DETAILS[8].VALUE != undefined &&
                        item.MONTH_DETAILS[8].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[8].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[8].CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MONTH_DETAILS[9] != undefined && item.MONTH_DETAILS[9].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[9] != undefined &&
                        item.MONTH_DETAILS[9].VALUE != undefined &&
                        item.MONTH_DETAILS[9].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[9].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[9].CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MONTH_DETAILS[10] != undefined &&
                      item.MONTH_DETAILS[10].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[10] != undefined &&
                        item.MONTH_DETAILS[10].VALUE != undefined &&
                        item.MONTH_DETAILS[10].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[10].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[10].CURRENCY"
                      :class="'valuedBudget'"
                      :readonly="true"
                    ></currency-input>
                  </td>
                  <td
                    :class="
                      item.MONTH_DETAILS[11] != undefined &&
                      item.MONTH_DETAILS[11].SELECTED
                        ? 'selectedCell'
                        : 'notSelectedCell'
                    "
                  >
                    <currency-input
                      v-if="
                        item.MONTH_DETAILS[11] != undefined &&
                        item.MONTH_DETAILS[11].VALUE != undefined &&
                        item.MONTH_DETAILS[11].CURRENCY != undefined
                      "
                      v-model="item.MONTH_DETAILS[11].VALUE"
                      v-bind="options"
                      :currency="item.MONTH_DETAILS[11].CURRENCY"
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
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                        >mdi-delete</v-icon
                      >
                    </template>
                    <span>{{
                      $t("datasource.pms.annual-plan.objectives.delete-service")
                    }}</span>
                  </v-tooltip>
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
import ObjectiveBudget from "../ACTIVITIES/AnnualPlanActivityForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

interface OBJECTIVE_MONTH {
  SELECTED: boolean;
  VALUE: any;
  CURRENCY: string;
}
interface objectItem {
  ID: number;
  OBJECTIVE_ID: number;

  DESCRIPTION: string;
  YEAR: number;
  MONTH: number;
  MONTH_DETAILS: OBJECTIVE_MONTH[];
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

    "ap-budget": ObjectiveBudget,
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

  data() {
    return {
      budgetYear: [],
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

    reloadActivities(item: any) {
      let localThis = this as any;

      localThis.getAnnualPlanObjectives();
    },
    getAnnualPlanObjectives() {
      let localThis: any = this as any;
      localThis.objectiveList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_OBJECTIVE_BUDGET";
      let wherecond: string = `AP_ID = ${localThis.selectedObjectId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_OBJECTIVE_BUDGET_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "OBJ_ID",
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
                if (localThis.objectiveList[i].ID == item.OBJ_ID) {
                  k = i;
                  break;
                }
              }
              if (k == -1) {
                let objItem = {} as objectItem;
                objItem.ID = item.OBJ_ID;
                if (item.DESCRIPTION == undefined) {
                  // objItem.CODE = item.PMS_COI_CODE;
                  objItem.DESCRIPTION = "";
                } else {
                  // objItem.CODE = item.PMS_COI_CODE + " - " + item.TITLE;
                  objItem.DESCRIPTION = item.DESCRIPTION;
                }

                objItem.OBJECTIVE_ID = item.OBJ_ID;
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

                localThis.objectiveList.push(objItem);
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
      localThis.getAnnualPlanObjectives();
      localThis.budgetMode = false;
    },

    editBudget(item: any) {
      let localThis: any = this as any;
      localThis.budgetMode = true;
      localThis.editObjId = item.ID;
      localThis.editObjDesc = item.DESCRIPTION;
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
    for (year =  Number.parseInt(ap.start_year); year <  Number.parseInt(ap.end_year) + 1; year++) {
      let yearN = {} as any;
      yearN.text = year.toString();
      yearN.value = year.toString();
      localThis.budgetYear.push(yearN);
    }
  },
  mounted() {
    let localThis: any = this as any;
    localThis.tblName = "PMS_ACTIVITY";
    localThis.setupHeaders();
    if (localThis.selectedObjectId > 0) {
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
  font-size: 12px;
  width: 85px;
}
</style>
