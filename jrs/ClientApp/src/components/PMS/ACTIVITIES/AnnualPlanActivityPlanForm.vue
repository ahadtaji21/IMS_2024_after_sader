<template>
  <div id="apap">
    <v-container class="grey lighten-5">
      {{ selectedObjectId }}
      <v-row>
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
          <v-select
            v-model="selectedYear"
            :items="budgetYear"
            :label="'Budget Year'"
            :readonly="selectYearReadOnly"
            item-text="text"
            :style="'background-color:white;margin:2px'"
            item-value="value"
            @change="yearChanged"
          ></v-select>
        </v-col>
        <v-col
          :class="'headerStyleRes'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          :style="'color:#4e76aa'"
        >
          {{ bdg }}
        </v-col>
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 4">
          <br />

          <label id="lbl" style="color: white">Estimated Year Budget: </label>
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
          &nbsp;
          <v-btn
            v-if="!onlyRead"
            color="secondary lighten-2"
            class="grey--text text--darken-3"
            @click="saveEstimatedBudget"
          >
            <v-icon>mdi-content-save</v-icon>Save
          </v-btn>
        </v-col>
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 4">
        </v-col>
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">
          <br />
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
                v-if="!onlyRead"
                color="secondary lighten-2"
                class="grey--text text--darken-3"
                v-on="on"
                @click="editMode = !editMode"
              >
                <v-icon>mdi-plus-circle-outline</v-icon>New
              </v-btn>
            </template>

            <search-table
              v-model="resourceId"
              @UpdateItem="closeDialog"
              :selectedActivityId="selectedObjectId"
              :key="Math.ceil(Math.random() * 1000)"
            ></search-table>
          </v-dialog>
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
              @UpdateBudgetItem="closeBudgetDialogAndUpdList"
              @CloseDialog="closeBudgetDialog"
              :selectedActivityId="selectedObjectId"
              :resource="selectedResource"
              :selectedYearExt="selectedYear"
              :resource_id="selectedResourceId"
              :selectedActivityItemId="selectedActivityItemId"
              :key="Math.ceil(Math.random() * 1000)"
            ></item-budget>
          </v-dialog>
        </v-col>
      </v-row>
      <v-row>
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >B. Line ID</v-col
        >
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Chart Of Account Id</v-col
        >
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
          >Description</v-col
        >
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Unit Type</v-col
        >
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Unit</v-col
        >
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">
        </v-col>
        <v-col
          :class="'headerStyleRes'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        ></v-col>
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Percentage</v-col
        >
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Total</v-col
        >
        <v-col :class="'headerStyleRes'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
          <currency-input
            :style="{
              'font-size': '18px',
              'font-weight': 'bold',
              color: 'white',
            }"
            v-model="TotalActivityBudget"
            v-bind="options"
            :readonly="true"
          ></currency-input>
        </v-col>
      </v-row>
      <v-row>
        <v-col></v-col>
      </v-row>
      <template v-for="n in budgetLines.length">
        <pms-act-budgt-line
          :key="n + rowKey"
          :budgetLine="budgetLines[n - 1]"
          :idx="n"
          @UpdateBudgetList="UpdateBudgetList"
          :onlyRead="onlyRead"
          :year="selectedYear"
        ></pms-act-budgt-line>
      </template>
    </v-container>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import Budget from "../BUDGET/AnnualPlanActivityBudgetLineForm.vue";
import ItemBudget from "./../BUDGET/AnnualPlanItemBudgetForm.vue";
import {
  IBudgetItemUPD,
  IBudgetItemGET,
  IPayLoadDataITEM,
  IEstimatedYearBudget,
} from "./../PMSSharedInterfaces";
import SearchTable from "./../ITEMS/AnnualPlanItemSearchForm.vue";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

export default Vue.extend({
  components: {
    "pms-act-budgt-line": Budget,
    "search-table": SearchTable,
    "item-budget": ItemBudget,
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
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      keyRnd: 0,
      bdg: 0,
      estimatedYearBudget: {},
      selectedActivityItemId: 0,
      selectYearReadOnly: false,
      selectedYear: "",
      budgetYear: [],
      TotalActivityBudget: 0,
      selectedResource: "",
      selectedResourceId: 0,
      budgetMode: false,
      resourceId: {},
      editMode: false,
      actDesc: "",
      rowKey: 0,
      budgetLines: [],
      selected: [],
      headerStyleRes: {
        color: "gray",
        fontFamily: "Roboco",
        fontSize: "14px",
      },
    };
  },
  computed: {
    options() {
      let localThis: any = this as any;
      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;

      localThis.currencyCode = ap.currency;
      return {
        locale: localThis.locale,
        currency: localThis.currencyCode,
      };
    },
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
    saveEstimatedBudget() {
      let localThis: any = this as any;
      if (localThis.estimatedYearBudget.BUDGET == undefined) return;

      let id = 0;
      if (localThis.estimatedYearBudget.ID != undefined)
        id = localThis.estimatedYearBudget.ID;

      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-estimated-year-budget")
        .toString();
      let msg = msgUpd;
      this.$confirm(msg).then((res) => {
        if (res) {
          let payLoad = {} as IEstimatedYearBudget;
          let i: number;
          payLoad.ACTIVITY_ID = localThis.selectedObjectId;
          payLoad.ID = id;
          payLoad.YEAR = localThis.selectedYear;
          payLoad.BUDGET = localThis.estimatedYearBudget.BUDGET;
          payLoad.CURRENCY = localThis.currencyCode;
          let savePayload: GenericSqlPayload = {
            spName:
              "PMS_SP_INS_UPD_ANNUAL_PLAN_OBJECTIVE_SERVICE_ACTIVITY_ESTIMATED_YEAR_BUDGET",
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
              localThis.getEstimatedYearBudget();
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
    UpdateBudgetList() {
      let localThis: any = this as any;
      localThis.getBudget();
    },
    closeDialog(item: String) {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.editedItemDialog = {};
      if (item != null) localThis.UpdateItem(item);
    },
    UpdateItem(item: string) {
      let ap = {} as any;
      let localThis = this as any;
      let payLoadD = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      payLoadD.ID_ACTIVITY = localThis.selectedObjectId;
      payLoadD.ID_ITEM = Number.parseInt(item);
      payLoadD.START_YEAR = Number.parseInt(ap.start_year);
      payLoadD.END_YEAR = Number.parseInt(ap.end_year);
      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_ANNUAL_PLAN_RESOURCE_ITEM_V1",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.budgetMode = !localThis.budgetMode;
          localThis.selectedResource = "";
          localThis.selectedResourceId = Number.parseInt(item);
          localThis.selectedActivityItemId = Number.parseInt(res.INSERTED_ID);
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
    closeBudgetDialogAndUpdList(item: string) {
      let localThis: any = this as any;
      localThis.budgetMode = false;
      localThis.getBudget();
    },
    closeBudgetDialog(item: string) {
      let localThis: any = this as any;
      localThis.budgetMode = false;
    },
    yearChanged() {
      let localThis: any = this as any;
      localThis.getEstimatedYearBudget();
      localThis.getBudget();
    },

    getEstimatedYearBudget() {
      let localThis: any = this as any;
      localThis.estimatedYearBudget = {} as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_ACTIVITY_YEAR_BUDGET",
        colList: null,
        whereCond: `PMS_ACTIVITY_ID = ${localThis.selectedObjectId} AND YEAR=${localThis.selectedYear}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
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

    getBudget() {
      let localThis: any = this as any;

      localThis.resetSelected();

      var budgetList: any = [];
      var budgetItems = [] as IBudgetItemGET[];
      localThis.budgetLines = [];
      localThis.budget = [];
      localThis.TotalActivityBudget = 0;
      let i: number = 0;
      for (i = 0; i < 12; i++) {
        localThis.budget[i] = null;
        localThis.selected[i] = false;
      }

      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_ACTIVITY_BUDGET_V1",
        colList: null,
        whereCond: `ACTIVITY_ID = ${localThis.selectedObjectId} AND YEAR=${localThis.selectedYear}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_JRS_COA_CODE",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              budgetList.push(item);
            });
            let result = budgetList.map((x: any) => x.AI_ID);
            var uniqueId = result.filter(function (itm: any, i: number, a: any) {
              return i == a.indexOf(itm);
            });
            let j: number = 0;
            if (uniqueId == undefined) return;
            for (j = 0; j < uniqueId.length; j++) {
              let resultItm = budgetList.filter(function (x: any) {
                return x.AI_ID == uniqueId[j];
              });
              //let resultItm = budgetList.map((x: any) => x.AI_ID==uniqueId[j]);
              let k: number = 0;
              var budgetItem = {} as IBudgetItemGET;
              budgetItem.SELECTED = [];
              budgetItem.MONTHBUDGET = [];
              budgetItem.MODIFIED = [];
              for (i = 0; i < 12; i++) {
                budgetItem.SELECTED.push(false);
                budgetItem.MONTHBUDGET.push(0);
                budgetItem.MODIFIED.push(false);
              }

              budgetItem.ACTIVITY_ID = localThis.selectedObjectId;
              budgetItem.YEAR = localThis.selectedYear;
              budgetItem.YEARBUDGET = 0;
              budgetItem.PERCENTAGE = 0;
              budgetItem.UNIT = 0;
              budgetItem.TIMING = 0;
              budgetItem.TIME_UNIT = "";
              budgetItem.UNIT_TYPE = "";
              budgetItem.PMS_JRS_COA_DESCRIPTION = "";
              budgetItem.DESCRIPTION = "";
              budgetItem.PMS_JRS_COA_ID = 0;
              budgetItem.PMS_JRS_COA_CODE = "";
              budgetItem.AI_ID = 0;
              budgetItem.AIR_ID = 0;

              for (k = 0; k < resultItm.length; k++) {
                budgetItem.PERCENTAGE = resultItm[k].PERCENTAGE;
                budgetItem.UNIT = resultItm[k].UNIT;
                budgetItem.TIMING = resultItm[k].TIMING;
                budgetItem.TIME_UNIT = resultItm[k].TIME_UNIT;
                budgetItem.UNIT_TYPE = resultItm[k].UNIT_TYPE;
                budgetItem.PMS_JRS_COA_DESCRIPTION = resultItm[k].PMS_JRS_COA_DESCRIPTION;
                budgetItem.DESCRIPTION = resultItm[k].DESCRIPTION;
                budgetItem.PMS_JRS_COA_CODE = resultItm[k].PMS_JRS_COA_CODE;
                budgetItem.PMS_JRS_COA_ID = resultItm[k].PMS_JRS_COA_ID;
                budgetItem.UNIT_PRICE = resultItm[k].UNIT_PRICE;
                budgetItem.BUDGET = 0;
                budgetItem.CURRENCY = resultItm[k].CURRENCY;
                budgetItem.SELECTED[resultItm[k].MONTH - 1] = true;
                budgetItem.MONTHBUDGET[resultItm[k].MONTH - 1] = resultItm[k].VALUE;
                budgetItem.YEARBUDGET += resultItm[k].VALUE;
                budgetItem.AI_ID = resultItm[k].AI_ID;
                budgetItem.AIR_ID = resultItm[k].AIR_ID;
              }
              localThis.TotalActivityBudget += budgetItem.YEARBUDGET;
              budgetItems.push(budgetItem);
            }
            localThis.budgetLines = budgetItems;
          }
          localThis.rowKey = localThis.getRnd();
        });
    },
    resetSelected() {
      let localThis: any = this as any;
      let i: number = 0;
      for (i = 0; i < 13; i++) {
        localThis.selected[i] = false;
      }
    },
  },
  mounted() {
    let localThis = this as any;
    if (localThis.selectedYearExt != "") {
      localThis.selectedYear = localThis.selectedYearExt;
      localThis.selectYearReadOnly = true;
    }
    localThis.getEstimatedYearBudget();
    localThis.getBudget();
  },

  beforeMount() {
    let localThis = this as any;
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
    localThis.resetSelected();
    localThis.actDesc = localStorage.SelectedAct;

    localThis.selectedYear = ap.start_year;
  },
});
</script>

<style scoped>
.headerStyleRes {
  color: white;
  font-family: "Montserrat";
  font-size: 14px;
  border: 1px;
  border-style: solid;
  border-color: lightgray;
  padding: 0px;
  margin: 0px;
  background-color: #4e76aa;
}
</style>
