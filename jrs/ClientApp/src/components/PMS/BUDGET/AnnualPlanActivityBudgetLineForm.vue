<template>
  <div id="bl">
    <v-row align-center v-if="showWait">
      <v-col justify-center :cols="12">
        <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
          <v-progress-circular indeterminate color="primary"></v-progress-circular>
        </div>
      </v-col>
    </v-row>
    <v-row :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        idx
      }}</v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        localBudgetLine.PMS_JRS_COA_CODE
      }}</v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
        >{{ localBudgetLine.PMS_JRS_COA_DESCRIPTION }} -
        {{ localBudgetLine.DESCRIPTION }}</v-col
      >
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        localBudgetLine.UNIT_TYPE
      }}</v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        localBudgetLine.UNIT
      }}</v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"></v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"></v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        localBudgetLine.PERCENTAGE
      }}</v-col>
      <v-col
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        :class="
          !localBudgetLine.MODIFIED.includes(true) ? 'dataStyle' : 'rowStyleModified'
        "
      >
        <currency-input
          :style="{ 'font-size': '12px', 'font-weight': 'bold' }"
          v-model="localBudgetLine.YEARBUDGET"
          v-bind="options"
          :readonly="true"
        ></currency-input>
      </v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
        <v-tooltip bottom>
          <template v-if="!onlyRead" v-slot:activator="{ on }">
            <v-icon small class="mr-4" @click="UndoModifies()" v-on="on"
              >mdi-redo-variant</v-icon
            >
          </template>
          <span>{{
            $t("datasource.pms.annual-plan.objectives.undo-inline-modifies")
          }}</span>
        </v-tooltip>
        <v-tooltip bottom>
          <template v-if="!onlyRead" v-slot:activator="{ on }">
            <v-icon small class="mr-4" @click="budgetItem()" v-on="on">mdi-pencil</v-icon>
          </template>
          <span>{{
            $t("datasource.pms.annual-plan.objectives.modify-budget-details")
          }}</span>
        </v-tooltip>
        <v-tooltip bottom>
          <template v-if="!onlyRead" v-slot:activator="{ on }">
            <v-icon small class="mr-4" @click="saveBudget()" v-on="on"
              >mdi-content-save</v-icon
            >
          </template>
          <span>{{
            $t("datasource.pms.annual-plan.objectives.save-inline-modifies")
          }}</span>
        </v-tooltip>
        <v-tooltip bottom>
          <template v-if="!onlyRead" v-slot:activator="{ on }">
            <v-icon small class="mr-4" @click="deleteBudget()" v-on="on"
              >mdi-delete</v-icon
            >
          </template>
          <span>{{
            $t("datasource.pms.annual-plan.objectives.delete-budget-line")
          }}</span>
        </v-tooltip>
      </v-col>
    </v-row>

    <v-row :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
      <template v-for="n in 12">
        <v-col
          :key="n"
          :class="'headerStyle'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >
          <input
            type="checkbox"
            id="n"
            v-model="localBudgetLine.SELECTED[n - 1]"
            @change="checkboxUpdated($event, n - 1)"
          />
          <label for="n" class="vlabel">{{ rowDesk[n] }}</label>
          <!-- <v-checkbox
            v-model="selected[n-1]"
            :label="rowDesk[n]"
            value
            hide-details
            color="white"
            light
            style="vlabel"
            @change="checkboxUpdated($event, n)"
          ></v-checkbox>-->
        </v-col>
      </template>
    </v-row>
    <v-row :key="rowKey" :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
      <v-col
        v-for="n in 12"
        :class="!localBudgetLine.MODIFIED[n - 1] ? 'rowStyle' : 'rowStyleModified'"
        :key="n"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
      >
        <div
          :style="
            localBudgetLine.SELECTED[n - 1]
              ? { 'background-color': 'whitesmoke' }
              : { 'background-color': 'green' }
          "
        >
          <currency-input
            :style="
              localBudgetLine.SELECTED[n - 1]
                ? { 'font-size': '12px' }
                : { 'font-size': '12px' }
            "
            v-model="localBudgetLine.MONTHBUDGET[n - 1]"
            v-bind="options"
            @change="refreshTotal(n - 1)"
          ></currency-input>
        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col></v-col>
    </v-row>

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
        @UpdateBudgetItem="closeBudgetDialog"
        @CloseDialog="closeDialog"
        :selectedActivityId="localBudgetLine.ACTIVITY_ID"
        :resource="localBudgetLine.PMS_JRS_COA_DESCRIPTION"
        :selectedActivityItemId="localBudgetLine.AI_ID"
        :resource_id="localBudgetLine.PMS_JRS_COA_ID"
        :key="Math.ceil(Math.random() * 1000)"
      ></item-budget>
    </v-dialog>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../i18n";
import FormMixin from "../../../mixins/FormMixin";
import NavMixin from "../../../mixins/NavMixin";
import { EventBus } from "../../../event-bus";
import ItemBudget from "./../BUDGET/AnnualPlanItemBudgetForm.vue";
import {
  IBudgetItemGET,
  IBudgetItemUPD,
  IPayLoadDataITEM,
} from "./../PMSSharedInterfaces";
import {
  ImsApi,
  PmsAnnualPlanApi,
  ImsLookupsApi,
  Configuration,
  NavIntegrationApi,
  NavBudget1,
} from "../../../axiosapi";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

export default Vue.extend({
  components: {
    "item-budget": ItemBudget,
  },
  props: {
    selectedActivityId: {
      type: Number,
      required: false,
    },
    resource: {
      type: String,
      required: false,
    },
    resource_id: {
      type: Number,
      required: false,
    },
    budgetLine: {
      type: Object,
      required: true,
    },
    idx: {
      type: Number,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },

    year: {
      type: String,
      required: true,
    },
  },

  data() {
    return {
      yearInt: 0,
      showWait: false,
      localBudgetLine: {},
      resourceId: {},
      budgetMode: false,
      bdg: 0,
      percentage: 0,
      locale: "en",
      actDesc: "",
      currencyCode: "EUR",
      //year: 0,
      rowKey: 0,
      rowDesk: [],
      selected: [],
      budget: [],
    };
  },
  mixins: [FormMixin, NavMixin],
  computed: {
    language() {
      return i18n.locale;
    },
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
        currency: localThis.currencyCode,
        // ,
        // valueRange: this.valueRangeEnabled
        //   ? { min: this.valueRange[0], max: this.valueRange[1] }
        //   : undefined,
        // precision: this.precisionEnabled
        //   ? (this.precisionRangeEnabled ? { min: this.precisionRange[0], max: this.precisionRange[1] } : this.precisionFixed)
        //   : undefined,
        // distractionFree: this.distractionFree
        //   ? {
        //     hideNegligibleDecimalDigits: this.hideNegligibleDecimalDigits,
        //     hideCurrencySymbol: this.hideCurrencySymbol,
        //     hideGroupingSymbol: this.hideGroupingSymbol
        //   } : false,
        // autoDecimalMode: this.autoDecimalMode,
        // valueAsInteger: this.valueAsInteger,
        // allowNegative: this.allowNegative
      };
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.locale = newLanguage;
      localThis.options.locale = newLanguage;
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
    closeBudgetDialog(item: string) {
      let localThis: any = this as any;
      localThis.budgetMode = false;
      (this as any).$emit("UpdateBudgetList");
    },
    closeDialog(item: string) {
      let localThis: any = this as any;
      localThis.budgetMode = false;
    },
    budgetItem(item: any) {
      let localThis = this as any;
      localThis.budgetMode = !localThis.budgetMode;
    },
    checkboxUpdated(newValue: any, idx: number) {
      let localThis: any = this as any;
      localThis.localBudgetLine.MODIFIED[idx] = true;
      var count = localThis.localBudgetLine.SELECTED.filter(function (s: any) {
        return s;
      }).length;
      let i: number;
      localThis.bdg = 0;
      for (i = 0; i < 12; i++) {
        if (localThis.localBudgetLine.SELECTED[i] == false) {
          // localThis.bdg -= localThis.localBudgetLine.MONTHBUDGET[i];
          localThis.localBudgetLine.MONTHBUDGET[i] = 0;
        } else {
          localThis.bdg += localThis.localBudgetLine.MONTHBUDGET[i];
        }
      }
      localThis.localBudgetLine.YEARBUDGET = localThis.bdg;
    },
    refreshTotal(idx: number) {
      let localThis: any = this as any;
      localThis.bdg = 0;
      localThis.localBudgetLine.MODIFIED[idx] = true;
      localThis.bdg = localThis.localBudgetLine.MONTHBUDGET.reduce(
        (acc: any, item: any) => acc + item,
        0
      );
      localThis.localBudgetLine.YEARBUDGET = localThis.bdg;
    },

    UndoModifies() {
      let localThis: any = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-undo-modifies")
        .toString();

      let id = 0;
      let msg = msgUpd;

      this.$confirm(msg).then((res) => {
        localThis.updateValue();
      });
    },
    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    splitBudgetAllMonths() {
      let localThis: any = this as any;
      localThis.budget = [];
      let i: number;
      for (i = 0; i < 12; i++) {
        localThis.budget.push(localThis.bdg / 12);
        localThis.selected[i] = true;
      }
    },

    deleteBudget() {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-budget-line")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis
            .updateBudgetOnNav(
              "true",
              localThis.localBudgetLine.AI_ID,
              localThis.yearInt,
              localThis.showWait
            )
            .then((res1: any) => {
              let payLoadD = {} as any;
              ap = localThis.$store.getters.getAnnualPlan;
              payLoadD.ID_ANNUAL_PLAN = ap.annal_plan_id;
              // payLoadD.ID_ACTIVITY_ITEM = localThis.localBudgetLine.PMS_JRS_COA_ID;
              payLoadD.AI_ID = localThis.localBudgetLine.AI_ID;
              payLoadD.YEAR = localThis.yearInt;

              let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_DEL_ANNUAL_PLAN_RESOURCE_BUDGET_V1",
                actionType: SqlActionType.NUMBER_0,
                jsonPayload: JSON.stringify(payLoadD),
              };
              localThis
                .execSPCall(savePayload)
                .then((res: any) => {
                  localThis.itemsPerPage = 15;
                  localThis.showNewSnackbar({
                    typeName: "success",
                    text: res.message,
                  }); // Feedback to user
                  localThis.updateValue();
                })
                .catch((err: any) => {
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message,
                  }); // Feedback to user
                });
            });
        }
      });
    },

    editItem() {},
    splitBudgetSelectedMonths() {
      let localThis: any = this as any;
      localThis.budget = [];
      var count = localThis.selected.filter(function (s: any) {
        return s;
      }).length;
      let i: number;
      for (i = 0; i < 12; i++) {
        if (localThis.selected[i] == true) localThis.budget.push(localThis.bdg / count);
        else localThis.budget.push(0);
      }
    },
    saveBudget() {
      let localThis: any = this as any;
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
              localThis.localBudgetLine.AI_ID,
              localThis.yearInt,
              localThis.showWait
            )
            .then((resA: any) => {
              localThis.showWait = true;
              let payLoad = {} as any;
              let i: number;
              payLoad.ACTIVITY_ITEM_REL_ID = localThis.localBudgetLine.AI_ID;
              payLoad.ACTIVITY_ID = localThis.localBudgetLine.ACTIVITY_ID;
              payLoad.RESOURCE_ID = localThis.localBudgetLine.PMS_JRS_COA_ID;
              payLoad.POSITION_ID = 0;
              payLoad.PERCENTAGE = localThis.localBudgetLine.PERCENTAGE;
              payLoad.YEAR = localThis.localBudgetLine.YEAR;
              payLoad.SELECTED = "";
              payLoad.UNIT_PRICE = localThis.localBudgetLine.UNIT_PRICE;
              payLoad.DESCRIPTION = localThis.localBudgetLine.DESCRIPTION;
              payLoad.DEPARTMENT = localThis.localBudgetLine.DEPARTMENT;
              payLoad.FUNCTION_CODE = localThis.localBudgetLine.FUNCTION_CODE;
              payLoad.TIME_UNIT = localThis.localBudgetLine.TIME_UNIT;
              payLoad.TIMING = localThis.localBudgetLine.TIMING;
              payLoad.UNIT = localThis.localBudgetLine.UNIT;
              payLoad.UNIT_TYPE = localThis.localBudgetLine.UNIT_TYPE;
              payLoad.CURRENCY = localThis.localBudgetLine.CURRENCY;
              for (i = 0; i < localThis.localBudgetLine.SELECTED.length; i++) {
                if (localThis.localBudgetLine.SELECTED[i] == true) {
                  if (localThis.localBudgetLine.MONTHBUDGET[i] != null) {
                    payLoad.SELECTED +=
                      i +
                      1 +
                      ";" +
                      localThis.localBudgetLine.MONTHBUDGET[i] +
                      "^" +
                      localThis.localBudgetLine.CURRENCY +
                      ":";
                  }
                }
              }
              let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_ACTIVITY_BUDGET_V1",
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
                      localThis.localBudgetLine.AI_ID,
                      localThis.yearInt,
                      localThis.showWait
                    )
                    .then((res1: any) => {
                      localThis.updateValue(null);
                    });
                })

                .catch((err: any) => {
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message,
                  }); // Feedback to user
                });
            });
        }
      });
    },

    updateValue() {
      (this as any).$emit("UpdateBudgetList");
    },
    // getResourceScheduling() {
    //   let localThis: any = this as any;
    //   localThis.resetSelected();
    //   localThis.activityList = [];

    //   let j: number = 0;
    //   let selectPayload: GenericSqlSelectPayload = {
    //     viewName: "PMS_ACTIVITY_SCHEDULING",
    //     colList: null,
    //     whereCond: `ACTIVITY_ID = ${localThis.selectedActivityId} AND RESOURCE_ID = ${localThis.resource_id}  AND YEAR=${localThis.year}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
    //     orderStmt: "ID",
    //   };

    //   return localThis
    //     .getGenericSelect({ genericSqlPayload: selectPayload })
    //     .then((res: any) => {
    //       //Setup data
    //       if (res.table_data) {
    //         let x: number = 0;
    //         res.table_data.forEach((item: any) => {
    //           localThis.activityList.push(item);
    //         });
    //         let i: number = 0;
    //         for (i = 0; i < localThis.activityList.length; i++) {
    //           localThis.selected[localThis.activityList[i].MONTH - 1] = true;
    //         }
    //       }
    //       localThis.rowKey = localThis.getRnd();
    //     });
    // },
    // getBudget() {
    //   let localThis: any = this as any;
    //   localThis.resetSelected();
    //   var budgetList: any = [];
    //   localThis.budget = [];
    //   let i: number = 0;
    //   for (i = 0; i < 12; i++) {
    //     localThis.budget[i] = null;
    //     localThis.selected[i] = false;
    //   }

    //   let j: number = 0;
    //   let selectPayload: GenericSqlSelectPayload = {
    //     viewName: "PMS_VI_ACTIVITY_BUDGET",
    //     colList: null,
    //     whereCond: `ACTIVITY_ID = ${localThis.selectedActivityId} AND RESOURCE_ID = ${localThis.resource_id} AND YEAR=${localThis.year}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
    //     orderStmt: "ID",
    //   };

    //   return localThis
    //     .getGenericSelect({ genericSqlPayload: selectPayload })
    //     .then((res: any) => {
    //       //Setup data
    //       if (res.table_data) {
    //         let x: number = 0;
    //         res.table_data.forEach((item: any) => {
    //           budgetList.push(item);
    //         });
    //         let k: number = 0;
    //         for (k = 0; k < budgetList.length; k++) {
    //           localThis.percentage = budgetList[k].PERCENTAGE;
    //           localThis.selected[budgetList[k].MONTH - 1] = true;
    //           localThis.budget[budgetList[k].MONTH - 1] = budgetList[k].VALUE;
    //           localThis.bdg += budgetList[k].VALUE;
    //         }
    //       }
    //       localThis.rowKey = localThis.getRnd();
    //     });
    // },

    resetSelected() {
      let localThis: any = this as any;
      let i: number = 0;
      for (i = 0; i < 12; i++) {
        localThis.selected[i] = false;
      }
    },
  },
  mounted() {
    let localThis = this as any;
    localThis.yearInt = Number.parseInt(localThis.year);
    //localThis.getResourceScheduling();
    //localThis.getBudget();
  },
  beforeMount() {
    let localThis = this as any;
    localThis.resetSelected();
    localThis.actDesc = localStorage.SelectedAct;
    localThis.localBudgetLine = localThis.budgetLine;
    localThis.localBudgetLine.MODIFIED = [];
    let i: number = 0;
    for (i = 0; i < 12; i++) {
      localThis.localBudgetLine.MODIFIED.push(false);
    }
    localThis.rowDesk[1] = this.$i18n.t("datasource.pms.annual-plan.objectives.jan");
    localThis.rowDesk[2] = this.$i18n.t("datasource.pms.annual-plan.objectives.feb");
    localThis.rowDesk[3] = this.$i18n.t("datasource.pms.annual-plan.objectives.mar");
    localThis.rowDesk[4] = this.$i18n.t("datasource.pms.annual-plan.objectives.apr");
    localThis.rowDesk[5] = this.$i18n.t("datasource.pms.annual-plan.objectives.may");
    localThis.rowDesk[6] = this.$i18n.t("datasource.pms.annual-plan.objectives.jun");
    localThis.rowDesk[7] = this.$i18n.t("datasource.pms.annual-plan.objectives.jul");
    localThis.rowDesk[8] = this.$i18n.t("datasource.pms.annual-plan.objectives.aug");
    localThis.rowDesk[9] = this.$i18n.t("datasource.pms.annual-plan.objectives.sep");
    localThis.rowDesk[10] = this.$i18n.t("datasource.pms.annual-plan.objectives.oct");
    localThis.rowDesk[11] = this.$i18n.t("datasource.pms.annual-plan.objectives.nov");
    localThis.rowDesk[12] = this.$i18n.t("datasource.pms.annual-plan.objectives.dec");
    let ap = {} as any;
    ap = localThis.$store.getters.getAnnualPlan;
    //localThis.year = ap.start_year;
    localThis.currencyCode = ap.currency;
  },
});
</script>

<style scoped></style>
