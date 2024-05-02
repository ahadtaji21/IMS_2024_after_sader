<template>
  <v-card>
    <v-card-title primary-title>
      <v-row no-gutters>
        <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6" :class="'headerStyleH'">
          <v-row no-gutters
            ><v-col> Project Code: {{ project_code }} </v-col></v-row
          >
          <v-row no-gutters
            ><v-col>
              Category of Intervention: {{ coi_code }} - {{ coi_description }}
            </v-col></v-row
          >
          <v-row no-gutters
            ><v-col>
              Type of Service: {{ tos_code }} - {{ tos_description }}
            </v-col></v-row
          >
        </v-col>
        <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6" :class="'headerStyleH'">
          <v-row no-gutters
            ><v-col> Resource Code: {{ itemDetails.CODE }} </v-col></v-row
          >
          <v-row no-gutters
            ><v-col> Resource Description: {{ itemDetails.DESCRIPTION }} </v-col></v-row
          >
          <v-row no-gutters
            ><v-col> Currency:{{ currencyCode }} </v-col></v-row
          >
        </v-col>
      </v-row>
    </v-card-title>
    <div class="d-flex flex-row justify-center" v-if="showWait">
      <v-progress-circular indeterminate></v-progress-circular>
    </div>
    <v-card>
      <v-container fluid>
        <v-form v-model="formValid" ref="myForm" class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 4">
              <v-select
                v-model="selectedYear"
                :items="budgetYear"
                :label="'Budget Year'"
                item-text="text"
                item-value="value"
                :readonly="readOnlyYear"
                @change="yearChanged"
              ></v-select>
            </v-col>
          </v-row>

          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <v-select
                v-model="selected_department"
                :items="department"
                :label="$t('datasource.pms.annual-plan.objectives.department')"
                item-text="text"
                item-value="value"
                :rules="[(v) => !!v || 'Item is required']"
                required
              ></v-select>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <v-select
                v-model="selected_functioncode"
                :items="functioncode"
                :label="$t('datasource.pms.annual-plan.objectives.function-code')"
                item-text="text"
                item-value="value"
                :rules="[(v) => !!v || 'Item is required']"
                required
              ></v-select>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <v-select
                v-model="selected_subtrans"
                :items="subtrans"
                :label="$t('datasource.pms.annual-plan.objectives.sub-trans')"
                item-text="text"
                item-value="value"
              ></v-select>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <v-text-field
                v-model="customdescription"
                :label="
                  $t('datasource.pms.annual-plan.objectives.item-custom-description')
                "
                :rules="rules.required"
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <v-select
                :items="unittype1"
                :label="$t('datasource.pms.annual-plan.objectives.item-number-type-1')"
                :rules="[(v) => !!v || 'Item is required']"
                v-model="selected_unittype1"
                required
              ></v-select>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <v-text-field
                v-model="unitnumber1"
                @keypress="isNumber($event)"
                :label="$t('datasource.pms.annual-plan.objectives.item-number-unit-1')"
                @change="splitBudgetSelectedMonths()"
                :rules="rules.required"
              />
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <v-select
                :items="unittype2"
                :label="$t('datasource.pms.annual-plan.objectives.item-number-type-2')"
                :rules="[(v) => !!v || 'Item is required']"
                v-model="selected_unittype2"
                required
              ></v-select>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <v-text-field
                v-model="unitnumber2"
                @keypress="isNumber($event)"
                :label="$t('datasource.pms.annual-plan.objectives.item-number-unit-2')"
                @change="splitBudgetSelectedMonths()"
                :rules="rules.required"
              />
            </v-col>
          </v-row>
          <v-row>
            <!-- <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                v-model="timingnumber"
                @keypress="isNumber($event)"
                :label="$t('datasource.pms.annual-plan.objectives.item-timing-unit')"
                :rules="rules.required"
                @change="updateYearBudget()"
              />
            </v-col> -->

            <!-- <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <label
                for="allocated"
              >{{$t('datasource.pms.annual-plan.objectives.item-percentage')}}{{' : '}}</label>
            </v-col>-->
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 4" v-if="false">
              <v-text-field
                v-model="percentage"
                @keypress="isNumber($event)"
                :label="$t('datasource.pms.annual-plan.objectives.item-percentage')"
                :rules="rules.required"
                @change="
                  CheckMax100();
                  updateYearBudget();
                "
              />
            </v-col>

            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">
              <label for="bdgt">{{
                $t("datasource.pms.annual-plan.objectives.item-budget")
              }}</label>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <currency-input
                id="bdgt"
                :class="'inputfieldIBF'"
                v-model="bdg"
                v-bind="options"
                @change="splitBudgetSelectedMonths()"
              ></currency-input>
            </v-col>

            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 1" v-if="false">
              <label for="ybdgt"
                >{{ $t("datasource.pms.annual-plan.objectives.year-budget")
                }}{{ " : " }}</label
              >
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3" v-if="false">
              <currency-input
                id="ybdgt"
                :class="'inputfieldIBFRO'"
                v-model="ybdg"
                v-bind="options"
                :readonly="true"
              ></currency-input>
            </v-col>

            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
              <label for="ybdgt"
                >{{ $t("datasource.pms.annual-plan.objectives.assigned-budget")
                }}{{ " : " }}</label
              >
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
              <currency-input
                id="abdgt"
                :class="'inputfieldIBFRO'"
                v-model="abdg"
                v-bind="options"
                :readonly="true"
              ></currency-input>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"></v-col>
          </v-row>

          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
              <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-btn v-on="on" @click="splitBudgetAllMonths()">{{
                    $t("datasource.pms.annual-plan.objectives.split-budget-all")
                  }}</v-btn>
                </template>
                <span>{{
                  $t("datasource.pms.annual-plan.objectives.split-budget-all-desc")
                }}</span>
              </v-tooltip>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
              <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-btn v-on="on" @click="splitBudgetSelectedMonths()">{{
                    $t("datasource.pms.annual-plan.objectives.split-budget-sel")
                  }}</v-btn>
                </template>
                <span>{{
                  $t("datasource.pms.annual-plan.objectives.split-budget-sel-desc")
                }}</span>
              </v-tooltip>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3"></v-col>
          </v-row>
          <v-spacer></v-spacer>
          <v-row no-gutters>
            <template v-for="n in 12">
              <v-col :key="n" :class="'headerStyleIBF'">
                <v-checkbox
                  v-model="selected[n - 1]"
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
              <v-col :key="n" :class="'headerStyleIBF'">
                <currency-input
                  :style="
                    selected[n - 1]
                      ? {
                          'font-size': '12px',
                          width: '85px',
                          'background-color': 'whitesmoke',
                        }
                      : { width: '85px', 'background-color': 'white' }
                  "
                  v-model="budget[n - 1]"
                  v-bind="options"
                  :readonly="true"
                  @change="refreshTotal()"
                ></currency-input>
              </v-col>
            </template>
          </v-row>
          <v-card-actions>
            <v-row>
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">
                <v-btn color="secondary darken-1" text @click="closeDialog()"
                  >X {{ $t("general.close") }}</v-btn
                >
              </v-col>
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">
                <v-btn v-if="!onlyRead" color="primary" @click="saveBudget()">{{
                  $t("general.save")
                }}</v-btn>
              </v-col>
            </v-row>
          </v-card-actions>
        </v-form>
      </v-container>
    </v-card>
  </v-card>
</template>

<script lang="ts">
// da luca: pannell risorse: associo ai processi le risorse (dell'email) senza budget
//pannello budget che diventa overview budget: ai processi ai quali ho assegnato le risorse, do la possibilità di assegnare item postin con budget
//e sono questi che mando su nav

//nuova assegnazione del budget: decido numero e costo unitario ed assegno numero*costo unitario ad 1 o più mesi.
//costo mensile readonly ma solo assegnato da numero*costo unitario
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../i18n";
import FormMixin from "../../../mixins/FormMixin";
import NavMixin from "../../../mixins/NavMixin";
import { EventBus } from "../../../event-bus";
import { IBudgetItemUPD, IBudgetItemGET } from "./../PMSSharedInterfaces";
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
interface ItemDet {
  CODE: string;
  DESCRIPTION: string;
}

export default Vue.extend({
  props: {
    selectedActivityId: {
      type: Number,
      required: false,
    },

    selectedActivityItemId: {
      type: Number,
      required: true,
    },

    selectedYearExt: {
      type: String,
      required: false,
      default: "",
    },

    resource: {
      type: String,
      required: false,
    },
    resource_id: {
      type: Number,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      annal_plan_id: 0,
      showWait: false,
      project_code: "",
      coi_code: "",
      coi_description: "",
      tos_code: "",
      tos_description: "",
      readOnlyYear: false,
      budgetYear: [],
      selectedYear: "",
      itemDetails: {},
      formValid: false,
      rules: {
        required: [(value: any) => !!value || "Required."],
      },
      selected_timingtype: "",
      selected_unittype1: "",
      selected_unittype2: "",
      selected_department: "",
      selected_functioncode: "",
      selected_subtrans: "",
      timingnumber: 0,
      customdescription: "",
      timingtype: [],
      unitnumber1: 0,
      unitnumber2: 0,
      unittype1: [],
      unittype2: [],
      department: [],
      functioncode: [],
      subtrans: [],
      abdg: 0,
      ybdg: 0,
      bdg: 0,
      percentage: 100,
      locale: "en",
      actDesc: "",
      currencyCode: "EUR",
      rowKey: 0,
      rowDesk: [],
      selected: [],
      budget: [],
    };
  },
  mixins: [FormMixin, NavMixin],

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
    updateYearBudget() {
      let localThis: any = this as any;
      localThis.ybdg =
        (localThis.bdg *
          localThis.unitnumber1 *
          localThis.unitnumber2 *
          //localThis.timingnumber *
          localThis.percentage) /
        100;
    },

    checkboxUpdated(newValue: any, idx: number) {
      let localThis: any = this as any;

      var count = localThis.selected.filter(function (s: any) {
        return s;
      }).length;
      let i: number;

      for (i = 0; i < 12; i++) {
        if (localThis.selected[i] == false) {
          localThis.abdg -= localThis.budget[i];
          localThis.budget[i] = 0;
        } else {
          if (localThis.budget[i] == 0) {
            localThis.budget[i] =
              localThis.unitnumber1 * localThis.unitnumber2 * localThis.bdg;
            localThis.abdg +=
              localThis.unitnumber1 * localThis.unitnumber2 * localThis.bdg;
          }
        }
      }
    },
    refreshTotal() {
      let localThis: any = this as any;
      localThis.abdg = localThis.budget.reduce((acc: any, item: any) => acc + item, 0);
    },

    CheckMax100() {
      let localThis: any = this as any;
      if (localThis.percentage > 100) localThis.percentage = 100;
    },

    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    splitBudgetAllMonths() {
      let localThis: any = this as any;
      localThis.abdg = 0;
      localThis.budget = [];
      let i: number;
      for (i = 0; i < 12; i++) {
        // localThis.budget.push(localThis.ybdg / 12);
        // localThis.abdg += localThis.ybdg / 12;

        localThis.budget.push(
          localThis.unitnumber1 * localThis.unitnumber2 * localThis.bdg
        );
        localThis.abdg += localThis.unitnumber1 * localThis.unitnumber2 * localThis.bdg;
        localThis.selected[i] = true;
      }
    },
    splitBudgetSelectedMonths() {
      let localThis: any = this as any;
      localThis.budget = [];
      localThis.abdg = 0;
      var count = localThis.selected.filter(function (s: any) {
        return s;
      }).length;
      let i: number;
      for (i = 0; i < 12; i++) {
        if (localThis.selected[i] == true) {
          // localThis.budget.push(localThis.ybdg / count);
          // localThis.abdg += localThis.ybdg / count;
          localThis.budget.push(
            localThis.unitnumber1 * localThis.unitnumber2 * localThis.bdg
          );
          localThis.abdg += localThis.unitnumber1 * localThis.unitnumber2 * localThis.bdg;
        } else localThis.budget.push(0);
      }
    },

    getPrjData() {
      let localThis = this as any;

      localThis.showWait = true;

      let view: string = "PMS_VI_ANNUAL_PLAN_COI_TOS_DETAILS_FOR_NAV";
      let wherecond: string = `AP_ID = ${localThis.annal_plan_id} AND AI_ID = ${localThis.selectedActivityItemId}`;
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
              localThis.coi_code = item.PMS_COI_CODE;
              localThis.coi_description = item.PMS_COI_DESCRIPTION;
              localThis.tos_code = item.PMS_TOS_CODE;
              localThis.tos_description = item.PMS_TOS_DESCRIPTION;
              localThis.project_code = item.CODE;
            });
            localThis.showWait = false;
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          let errore: any = err.message;
          if (err.response != undefined && err.response.data != undefined) {
            if (err.response.data.message != undefined)
              errore += " - " + err.response.data.message;
          }
          localThis.showNewSnackbar({ typeName: "error", text: errore });
          throw err;
          //
        });
    },
    saveBudget() {
      let localThis: any = this as any;

      let isValid = true;
      isValid = (localThis.$refs.myForm as Vue & {
        validate: () => boolean;
      }).validate();
      if (!isValid) return;
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
              localThis.selectedActivityItemId,
              localThis.selectedYear,
              localThis.showWait
            )
            .then((resA: any) => {
              localThis.showWait = true;
              let payLoad = {} as any;
              let i: number;
              payLoad.ACTIVITY_ID = localThis.selectedActivityId;
              payLoad.RESOURCE_ID = localThis.resource_id;
              payLoad.POSITION_ID = 0;
              payLoad.PERCENTAGE = localThis.percentage;
              payLoad.YEAR = localThis.selectedYear;
              payLoad.SELECTED = "";
              payLoad.DESCRIPTION = localThis.customdescription;
              payLoad.UNIT_PRICE = localThis.bdg;
              payLoad.TIME_UNIT = ""; //localThis.selected_timingtype;
              payLoad.TIMING = 1; // localThis.timingnumber;
              payLoad.UNIT = localThis.unitnumber1;
              payLoad.UNIT2 = localThis.unitnumber2;
              payLoad.UNIT_TYPE = localThis.selected_unittype1;
              payLoad.UNIT_TYPE2 = localThis.selected_unittype2;
              payLoad.DEPARTMENT = localThis.selected_department;
              payLoad.FUNCTION_CODE = localThis.selected_functioncode;
              payLoad.SUB_TRANS = localThis.selected_subtrans;
              payLoad.CURRENCY = localThis.currencyCode;
              var budgetfound = false;
              for (i = 0; i < localThis.selected.length; i++) {
                if (localThis.selected[i] == true) {
                  if (localThis.budget[i] != null) {
                    budgetfound = true;

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
              if (budgetfound == false) {
                localThis.showNewSnackbar({
                  typeName: "error",
                  text: "Budget non inserted",
                });
                return;
              }
              payLoad.ACTIVITY_ITEM_REL_ID = localThis.selectedActivityItemId;
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
                      localThis.selectedActivityItemId,
                      localThis.selectedYear,
                      localThis.showWait
                    )
                    .then((res1: any) => {
                      localThis.updateValue(null);
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

    yearChanged() {
      let localThis: any = this as any;
      localThis.getItemDetails();
      localThis.getBudget();
    },
    getItemDetails() {
      let localThis: any = this as any;
      localThis.resetSelected();
      localThis.activityList = [];

      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_JRS_CHART_OF_ACCOUNT",
        colList: null,
        whereCond: `PMS_JRS_COA_ID = ${localThis.resource_id}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            localThis.itemDetails = {} as ItemDet;
            res.table_data.forEach((item: any) => {
              localThis.itemDetails.CODE = item.PMS_JRS_COA_CODE;
              localThis.itemDetails.DESCRIPTION = item.PMS_JRS_COA_DESCRIPTION;
            });
          }
        });
    },
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
        viewName: "PMS_VI_ACTIVITY_BUDGET_V1",
        colList: null,
        whereCond: `AI_ID = ${localThis.selectedActivityItemId} AND YEAR=${localThis.selectedYear}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
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
            if (budgetList.length > 0) {
              localThis.bdg = budgetList[0].UNIT_PRICE;
              localThis.percentage = budgetList[0].PERCENTAGE;
              localThis.unitnumber1 = budgetList[0].UNIT;
              localThis.unitnumber2 = budgetList[0].UNIT2;
              localThis.customdescription = budgetList[0].DESCRIPTION;
              localThis.timingnumber = budgetList[0].TIMING;
              localThis.selected_timingtype = budgetList[0].TIME_UNIT;
              localThis.selected_unittype1 = budgetList[0].UNIT_TYPE;
              localThis.selected_unittype2 = budgetList[0].UNIT_TYPE2;
              localThis.selected_department = budgetList[0].DEPARTMENT;
              localThis.selected_functioncode = budgetList[0].FUNCTION_CODE;
              localThis.selected_subtrans = budgetList[0].SUB_TRANS;
            }
            localThis.updateYearBudget();
            for (k = 0; k < budgetList.length; k++) {
              localThis.selected[budgetList[k].MONTH - 1] = true;
              localThis.budget[budgetList[k].MONTH - 1] = budgetList[k].VALUE;
              localThis.abdg += budgetList[k].VALUE;
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
      (this as any).$emit("UpdateBudgetItem", newVal);
    },
    /**
     * Clear field data.
     */
    closeDialog() {
      let localThis: any = this as any;

      (this as any).$emit("CloseDialog");
    },
    getList(listName: any) {
      let localThis = this as any;

      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      let company: string = localThis.getCurrentOffice.HR_OFFICE_CODE.substring(0, 3);
      return api
        .apiImsLookupsGetGeneriListByCompanyGet(listName, company, config.baseOptions)
        .then((res) => {
          switch (listName) {
            case "UNIT_TYPE1":
              localThis.unittype1 = res.data;
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

  mounted() {
    let localThis = this as any;
    localThis.getItemDetails();
    localThis.getList("UNIT_TYPE1");
    localThis.getList("UNIT_TYPE2");
    localThis.getList("TIME_UNIT");
    localThis.getList("DEPARTMENT");
    localThis.getList("FUNCTION_CODE");
    localThis.getList("SUB_TRANS");
    //localThis.getResourceScheduling();
    localThis.getBudget();
    localThis.updateYearBudget();
    if (localThis.selectedYearExt != "") {
      localThis.selectedYear = localThis.selectedYearExt;
      localThis.readOnlyYear = true;
    }
  },
  beforeMount() {
    let localThis = this as any;
    localThis.resetSelected();
    localThis.actDesc = localStorage.SelectedAct;

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
    localThis.currencyCode = ap.currency;
    localThis.annal_plan_id = ap.annal_plan_id;
    localThis.getPrjData();

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
});
</script>

<style scoped>
.inputfieldIBF {
  font-size: 14px;
  width: 200px;
  background-color: whitesmoke;
  border-width: 1px;
}
.inputfieldIBFRO {
  font-size: 14px;
  width: 200px;
  background-color: yellowgreen;
}
.headerStyleIBF {
  color: gray;
  font-family: "Montserrat";
  font-size: 14px;
  border: 1px;
  border-style: solid;
  border-color: lightgray;
  padding: 0px;
  margin: 0px;
}

.headerStyleH {
  color: black;
  font-family: "Montserrat";
  font-size: 14px;
  border: 1px;
  border-style: solid;
  border-color: lightgray;
  padding: 0px;
  margin: 0px;
}
</style>
