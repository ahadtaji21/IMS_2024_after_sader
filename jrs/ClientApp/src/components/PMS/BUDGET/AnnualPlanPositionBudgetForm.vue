<template>
  <div id="app">
    <v-app id="inspire">
      <v-container class="grey lighten-5">
        <v-card>
          <v-card-title
            primary-title
          >Year:{{year}} - Position: {{position}} - Currency:{{currencyCode}}</v-card-title>

          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <label for="bdgt">{{$t('datasource.pms.annual-plan.objectives.item-budget')}}{{' : '}}</label>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <currency-input id="bdgt" :class="'inputfield'" v-model="bdg" v-bind="options"></currency-input>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3" v-if="false">
              <label
                for="allocated"
              >{{$t('datasource.pms.annual-plan.objectives.item-percentage')}}{{' : '}}</label>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <input v-model="percentage" @keypress="isNumber($event)" :class="'inputfield'" />
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
              <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-btn
                    v-on="on"
                    @click="splitBudgetAllMonths()"
                  >{{$t('datasource.pms.annual-plan.objectives.split-budget-all')}}</v-btn>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.split-budget-all-desc') }}</span>
              </v-tooltip>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
              <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-btn
                    v-on="on"
                    @click="splitBudgetSelectedMonths()"
                  >{{$t('datasource.pms.annual-plan.objectives.split-budget-sel')}}</v-btn>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.split-budget-sel-desc') }}</span>
              </v-tooltip>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3"></v-col>
          </v-row>
          <v-spacer></v-spacer>
          <v-row no-gutters>
            <template v-for="n in 12">
              <v-col :key="n" :class="'headerStyle'">
                <v-checkbox
                  v-model="selected[n-1]"
                  :label="rowDesk[n]"
                  value
                  hide-details
                  class="shrink ma-0 pa-0"
                  @change="checkboxUpdated($event, n)"
                ></v-checkbox>
              </v-col>
            </template>
          </v-row>

          <v-row no-gutters :key="rowKey">
            <template v-for="n in 12">
              <v-col :key="n" :class="'headerStyle'">
                <currency-input
                  :style="selected[n-1] ? {'font-size':'12px','width':'85px', 'background-color': 'whitesmoke' } : {'width':'85px', 'background-color': 'white' }"
                  v-model="budget[n-1]"
                  v-bind="options"
                  :readonly="!selected[n-1]"
                  @change="refreshTotal()"
                ></currency-input>
              </v-col>
            </template>
          </v-row>
          <v-card-actions>
            <v-btn color="primary" @click="saveBudget()">{{ $t('general.save') }}</v-btn>
          </v-card-actions>
          <v-card-actions>
            <v-btn
              color="secondary darken-1"
              text
              @click="clearField()"
            >X {{ $t('general.close') }}</v-btn>
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
import FormMixin from "../../../mixins/FormMixin";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
interface PayLoadD {
  ID: number;
  ACTIVITY_ID: number;
  RESOURCE_ID: number;
  POSITION_ID: number;
  PERCENTAGE: number;
  YEAR: number;
  SELECTED: string;
  BUDGET: number;
  CURRENCY: string;
}
export default Vue.extend({
  props: {
    selectedActivityId: {
      type: Number,
      required: false,
    },
    position: {
      type: String,
      required: false,
    },
    position_id: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      bdg: 0,
      percentage: 100,
      locale: "en",
      actDesc: "",
      currencyCode: "EUR",
      year: 0,
      rowKey: 0,
      rowDesk: [],
      selected: [],
      budget: [],
    };
  },
  mixins: [FormMixin],
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
    checkboxUpdated(newValue: any, idx: number) {
      let localThis: any = this as any;

      var count = localThis.selected.filter(function (s: any) {
        return s;
      }).length;
      let i: number;
      //    for (i = 0; i < 12; i++) {
      //       if(localThis.selected[i]==true)
      //     localThis.budget.push(localThis.bdg / count);
      //     else
      //     localThis.budget.push(0);

      //   }

      for (i = 0; i < 12; i++) {
        if (localThis.selected[i] == false) {
          localThis.bdg -= localThis.budget[i];
          localThis.budget[i] = 0;
        }
      }
    },
    refreshTotal() {
      let localThis: any = this as any;
      localThis.bdg = localThis.budget.reduce(
        (acc: any, item: any) => acc + item,
        0
      );
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
    splitBudgetSelectedMonths() {
      let localThis: any = this as any;
      localThis.budget = [];
      var count = localThis.selected.filter(function (s: any) {
        return s;
      }).length;
      let i: number;
      for (i = 0; i < 12; i++) {
        if (localThis.selected[i] == true)
          localThis.budget.push(localThis.bdg / count);
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
          let payLoad = {} as PayLoadD;
          let i: number;
          payLoad.ACTIVITY_ID = localThis.selectedActivityId;
          payLoad.RESOURCE_ID = 0;
          payLoad.POSITION_ID = localThis.position_id;
          payLoad.PERCENTAGE = localThis.percentage;
          payLoad.YEAR = localThis.year;
          payLoad.SELECTED = "";
          for (i = 0; i < localThis.selected.length; i++) {
            if (localThis.selected[i] == true) {
              if (localThis.budget[i] != null) {
                payLoad.SELECTED +=
                  i +
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
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },
    // getPositionScheduling() {
    //   let localThis: any = this as any;
    //   localThis.resetSelected();
    //   localThis.activityList = [];

    //   let j: number = 0;
    //   let selectPayload: GenericSqlSelectPayload = {
    //     viewName: "PMS_ACTIVITY_SCHEDULING",
    //     colList: null,
    //     whereCond: `ACTIVITY_ID = ${localThis.selectedActivityId} AND RESOURCE_ID = ${localThis.position_id}  AND YEAR=${localThis.year}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
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
    getBudget() {
      let localThis: any = this as any;
      localThis.resetSelected();
      var budgetList: any = [];
      localThis.budget = [];
      let i: number = 0;
      for (i = 0; i < 12; i++) {
        localThis.budget[i] = null;
        localThis.selected[i] = false;
      }

      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_ACTIVITY_BUDGET",
        colList: null,
        whereCond: `ACTIVITY_ID = ${localThis.selectedActivityId} AND POSITION_ID = ${localThis.position_id} AND YEAR=${localThis.year}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID",
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
            for (k = 0; k < budgetList.length; k++) {
              localThis.percentage = budgetList[k].PERCENTAGE;
              localThis.selected[budgetList[k].MONTH - 1] = true;
              localThis.budget[budgetList[k].MONTH - 1] = budgetList[k].VALUE;
              localThis.bdg +=budgetList[k].VALUE;
            }
          }
          localThis.rowKey = localThis.getRnd();
        });
    },

    resetSelected() {
      let localThis: any = this as any;
      let i: number = 0;
      for (i = 0; i < 12; i++) {
        localThis.selected[i] = false;
      }
    },
    updateValue(newVal: String) {
      (this as any).$emit("UpdateBudgetPosition", newVal);
    },
    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;

      localThis.updateValue(null);
    },
  },
  mounted() {
    let localThis = this as any;
    //localThis.getPositionScheduling();
    localThis.getBudget();
  },
  beforeMount() {
    let localThis = this as any;
    localThis.resetSelected();
    localThis.actDesc = localStorage.SelectedAct;

    localThis.rowDesk[1] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.jan"
    );
    localThis.rowDesk[2] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.feb"
    );
    localThis.rowDesk[3] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.mar"
    );
    localThis.rowDesk[4] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.apr"
    );
    localThis.rowDesk[5] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.may"
    );
    localThis.rowDesk[6] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.jun"
    );
    localThis.rowDesk[7] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.jul"
    );
    localThis.rowDesk[8] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.aug"
    );
    localThis.rowDesk[9] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.sep"
    );
    localThis.rowDesk[10] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.oct"
    );
    localThis.rowDesk[11] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.nov"
    );
    localThis.rowDesk[12] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.dec"
    );
    let ap = {} as any;
    ap = localThis.$store.getters.getAnnualPlan;
    localThis.year = ap.start_year;
    localThis.currencyCode = ap.currency;
  },
});
</script>

<style scoped>


</style>