<template>
  <div id="app">
    <v-app id="inspire">
      <v-container class="grey lighten-5">
        <v-card>
          <v-card-title primary-title>Year:{{year}} - Currency:{{currencyCode}}</v-card-title>
          <v-row no-gutters>
            <template v-for="n in 13">
              <v-col :key="n">
                <v-card class="pa-2" outlined tile :style="headerStyle">{{rowDesk[n]}}</v-card>
              </v-col>
            </template>
          </v-row>
          <v-row no-gutters :key="rowKey">
            <template v-for="n in 13">
              <v-col :key="n">
                <currency-input
                  :style="selected[n] ? {'font-size':'12px','width':'85px', 'background-color': 'whitesmoke' } : {'width':'85px', 'background-color': 'white' }"
                  v-model="budget[n]"
                  v-bind="options"
                  :readonly="!selected[n]"
                ></currency-input>
              </v-col>
            </template>
          </v-row>
          <v-card-actions>
            <v-btn color="primary" @click="saveBudget()">{{ $t('general.save') }}</v-btn>
          </v-card-actions>
        </v-card>
      </v-container>
    </v-app>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../../axiosapi/api";
interface PayLoadD {
  ID: number;
  ACTIVITY_ID: number;
  YEAR: number;
  SELECTED: string;
  BUDGET: number;
  CURRENCY: string;
}
export default Vue.extend({
  props: {
    selectedObjectId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      locale: "en",
      actDesc: "",
      currencyCode: "EUR",
      year: 0,
      rowKey: 0,
      rowDesk: [],
      selected: [],
      budget: [],
      headerStyle: {
        color: "gray",
        fontFamily: "Roboco",
        fontSize: "14px"
      }
    };
  },
  computed: {
    language() {
      return i18n.locale;
    },
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
        currency: localThis.currencyCode
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
    }
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.locale = newLanguage;
      localThis.options.locale = newLanguage;
    }
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),

    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    saveBudget() {
      let localThis: any = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-activity-budget")
        .toString();

      let id = 0;
      let msg = msgUpd;

      this.$confirm(msg).then(res => {
        if (res) {
          let payLoad = {} as PayLoadD;
          let i: number;
          payLoad.ACTIVITY_ID = localThis.selectedObjectId;

          payLoad.YEAR = localThis.year;
          payLoad.SELECTED = "";
          for (i = 0; i < localThis.selected.length; i++) {
            if (localThis.selected[i] == true) {
              if (localThis.budget[i] != null) {
                payLoad.SELECTED +=
                  i -
                  1 +
                  ";" +
                  localThis.budget[i] +
                  "^" +
                  localThis.currencyCode +
                  ":";
              }
            }
          }
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_ACTIVITY_BUDGET",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoad)
          };

          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message
              }); // Feedback to user
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message
              }); // Feedback to user
            });
        }
      });
    },
    getActivityScheduling() {
      let localThis: any = this as any;
      localThis.resetSelected();
      localThis.activityList = [];

      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_ACTIVITY_SCHEDULING",
        colList: null,
        whereCond: `ACTIVITY_ID = ${localThis.selectedObjectId} AND YEAR=${localThis.year}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID"
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.activityList.push(item);
            });
            let i: number = 0;
            for (i = 0; i < localThis.activityList.length; i++) {
              localThis.selected[localThis.activityList[i].MONTH + 1] = true;
            }
          }
          localThis.rowKey = localThis.getRnd();
        });
    },
    getBudget() {
      let localThis: any = this as any;
      localThis.resetSelected();
      var budgetList:any = [];
      localThis.budget = [];
      let i: number = 0;
      for (i = 2; i < 14; i++) {
        localThis.budget[i] = null;
      }

      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_ACTIVITY_BUDGET",
        colList: null,
        whereCond: `ACTIVITY_ID = ${localThis.selectedObjectId} AND YEAR=${localThis.year}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID"
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
            let k: number = 0;
            for (k = 0; k < budgetList.length;k++) {
              localThis.budget[budgetList[k].MONTH + 1] =
                budgetList[k].VALUE;
            }
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
    }
  },
  mounted() {
    let localThis = this as any;
    localThis.getActivityScheduling();
    localThis.getBudget();
  },
  beforeMount() {
    let localThis = this as any;
    localThis.resetSelected();
    localThis.actDesc = localStorage.SelectedAct;
    localThis.rowDesk[1] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.activity"
    );
    localThis.rowDesk[2] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.jan"
    );
    localThis.rowDesk[3] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.feb"
    );
    localThis.rowDesk[4] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.mar"
    );
    localThis.rowDesk[5] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.apr"
    );
    localThis.rowDesk[6] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.may"
    );
    localThis.rowDesk[7] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.jun"
    );
    localThis.rowDesk[8] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.jul"
    );
    localThis.rowDesk[9] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.aug"
    );
    localThis.rowDesk[10] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.sep"
    );
    localThis.rowDesk[11] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.oct"
    );
    localThis.rowDesk[12] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.nov"
    );
    localThis.rowDesk[13] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.dec"
    );
    let ap = {} as any;
    ap = localThis.$store.getters.getAnnualPlan;
    localThis.year = ap.start_year;
    localThis.currencyCode = ap.currency;
  }
});
</script>

<style scoped>
</style>