<template>
  <!-- componente che mostra la lista delle voci dell'annual plan che necessitano di 
finanziamento e per ogni voce da la possibilità vederne i finanziamenti attuali -->

  <v-container class="grey lighten-5" fluid>
    <div>
      <v-row>
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >ANNUAL PLAN:</v-col
        >
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">{{
          displayData.AP_CODE
        }}</v-col>
      </v-row>
      <v-row>
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >OBJECTIVE:</v-col
        >
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">{{
          displayData.OBJ_DESC
        }}</v-col>
      </v-row>
      <v-row>
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >SERVICE:</v-col
        >
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">{{
          displayData.SRV_DESC
        }}</v-col>
      </v-row>
      <v-row>
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >ACTIVITY:</v-col
        >
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">{{
          displayData.ACT_DESC
        }}</v-col>
      </v-row>
    </div>
    <div v-if="!showMap">
      <v-row>
        <v-col :class="'headerStyleGapGrid'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <span v-on="on">
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  @click="returnToGapList()"
                >
                  <v-icon>{{ sIconBack }}</v-icon>
                </v-btn>
              </span>
            </template>
            <span>{{ $t("datasource.gmt.budget.back-to-list") }}</span>
          </v-tooltip>
        </v-col>
        <v-col
          :class="'headerStyleGapGrid'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 11"
        ></v-col>
      </v-row>
      <v-row>
        <v-col :class="'headerStyleGapGrid'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >B. Line ID</v-col
        >
        <v-col :class="'headerStyleGapGrid'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Chart Of Account Id</v-col
        >
        <v-col :class="'headerStyleGapGrid'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
          >Description</v-col
        >
        <v-col :class="'headerStyleGapGrid'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Unit Type</v-col
        >
        <v-col :class="'headerStyleGapGrid'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Unit</v-col
        >
        <v-col :class="'headerStyleGapGrid'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Unit Time</v-col
        >
        <v-col :class="'headerStyleGapGrid'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Timing</v-col
        >
        <v-col :class="'headerStyleGapGrid'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Percentage</v-col
        >
        <v-col :class="'headerStyleGapGrid'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >Total</v-col
        >
        <v-col :class="'headerStyleGapGrid'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
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
      <v-row align-center v-if="showWait">
        <v-col justify-center :cols="12">
          <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
        </v-col>
      </v-row>
      <gmt-act-budgt-line
        v-for="n in budgetLines.length"
        :key="n + rowKey"
        :budgetLine="budgetLines[n - 1]"
        :idx="n"
        @UpdateMap="UpdateMap(budgetLines[n - 1])"
        @UpdateFormData="FillFormData(budgetLines[n - 1])"
        :currencyCode="currencyCode"
        :year="year"
        :annual_plan_id="annual_plan_id"
        :donorList="donorList"
        :formData="formData"
      ></gmt-act-budgt-line>
    </div>
    <div v-else>
      <item-budget-map
        @closeMap="closeMap"
        :selectedProjectId="annual_plan_id"
        :formData="formData"
        :activityId="idActivity"
        @reload="reload"
        :key="keyRnd"
        :year="year"
      ></item-budget-map>
    </div>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../../i18n";
import { EventBus } from "../../../../event-bus";
import Budget from "./GMTGapDetailsLineForm.vue";
import ItemMap from "./GMTGapMapForm.vue";
import {
  IDonor,
  IBudgetItemUPD,
  IBudgetItemGET,
  IPayLoadDataITEM,
} from "./../../../PMS/PMSSharedInterfaces";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";

export default Vue.extend({
  components: {
    "gmt-act-budgt-line": Budget,
    "item-budget-map": ItemMap,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
    year: {
      type: Number,
      required: true,
    },
    currencyCode: {
      type: String,
      required: true,
    },
    annual_plan_id: {
      type: Number,
      required: true,
    },
    displayData: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      idActivity: 0,
      keyRnd: 0,
      formData: {},
      showWait: false,
      donorList: [] as IDonor[],
      showMap: false,
      sIconBack: "mdi-chevron-double-left",
      TotalActivityBudget: 0,
      selectedResource: "",
      selectedResourceId: 0,
      budgetMode: false,
      resourceId: {},
      editMode: false,
      actDesc: "",

      rowKey: 0,
      budgetLines: [],
      headerStyleGapGrid: {
        color: "gray",
        fontFamily: "Roboco",
        fontSize: "14px",
      },
    };
  },
  computed: {
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
        currency: localThis.currencyCode,
      };
    },
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setDonorList: "setDonorList",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    returnToGapList() {
      let localThis = this as any;
      localThis.$emit("showGapList");
    },
    closeMap() {
      let localThis = this as any;
      localThis.showMap = false;
      localThis.getBudget();
      localThis.getDonorsWithGrantsOnProject();
    },
    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },

    reload() {
      let localThis: any = this as any;
      localThis.getDonorsWithGrantsOnProject();
      localThis.keyRnd = Math.ceil(Math.random() * 1000);
    },
    FillFormData(item: any) {
      let localThis: any = this as any;
      localThis.formData = {} as any;
      if (item != null) {
        localThis.formData.ACTIVITY_ID = item.ACTIVITY_ID;
        localThis.formData.PMS_JRS_COI_ID = item.PMS_JRS_COI_ID;
        localThis.formData.PMS_JRS_TOS_ID = item.PMS_JRS_TOS_ID;
        localThis.formData.PMS_JRS_COA_ID = item.PMS_JRS_COA_ID;
      } else {
        localThis.formData.ACTIVITY_ID = 0;
        localThis.formData.PMS_JRS_COI_ID = 0;
        localThis.formData.PMS_JRS_TOS_ID = 0;
        localThis.formData.PMS_JRS_COA_ID = 0;
      }
    },

    UpdateMap(item: any) {
      let localThis: any = this as any;
      localThis.FillFormData(item);
      localThis.showMap = true;
    },

    UpdateBudgetList() {
      let localThis: any = this as any;
      localThis.getBudget();
    },
    closeDialog(item: String) {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.editedItemDialog = {};
      //if (item != null) localThis.UpdateItem(item);
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
    getDonorsWithGrantsOnProject() {
      let localThis = this as any;
      localThis.showWait = true;
      localThis.donorList = [] as IDonor[];
      localThis.setDonorList(localThis.donorList);

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONOR_PROJECTS_GRANT",
        //colList: null,
        whereCond: `PROJECT_ID='${localThis.annual_plan_id}'`,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.donorList.push(item);
            });
          }
        })
        .then((res: any) => {
          localThis.setDonorList(localThis.donorList);
          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          console.log(err);
          return err;
        });
    },
    getBudget() {
      let localThis: any = this as any;
      localThis.showWait = true;

      var budgetList: any = [];
      var budgetItems = [] as IBudgetItemGET[];
      localThis.TotalActivityBudget = 0;
      let i: number = 0;

      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_ACTIVITY_BUDGET",
        colList: null,
        whereCond: `ACTIVITY_ID = ${localThis.selectedObjectId} AND YEAR=${localThis.year}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
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
            let result = budgetList.map((x: any) => x.RESOURCE_ID);
            var uniqueId = result.filter(function (itm: any, i: number, a: any) {
              return i == a.indexOf(itm);
            });
            let j: number = 0;
            if (uniqueId == undefined) return;
            for (j = 0; j < uniqueId.length; j++) {
              let resultItm = budgetList.filter(function (x: any) {
                return x.RESOURCE_ID == uniqueId[j];
              });
              //let resultItm = budgetList.map((x: any) => x.RESOURCE_ID==uniqueId[j]);
              let k: number = 0;
              var budgetItem = {} as IBudgetItemGET;
              budgetItem.SELECTED = [];
              budgetItem.MONTHBUDGET = [];
              budgetItem.MONTHFUNDED = [];
              budgetItem.MODIFIED = [];
              for (i = 0; i < 12; i++) {
                budgetItem.SELECTED.push(false);
                budgetItem.MONTHBUDGET.push(0);
                budgetItem.MONTHFUNDED.push(0);
                budgetItem.MODIFIED.push(false);
              }
              budgetItem.ACTIVITY_ID = localThis.selectedObjectId;
              budgetItem.YEAR = localThis.year;
              budgetItem.YEARBUDGET = 0;
              budgetItem.PERCENTAGE = 0;
              budgetItem.UNIT = 0;
              budgetItem.TIMING = 0;
              budgetItem.TIME_UNIT = "";
              budgetItem.UNIT_TYPE = "";
              budgetItem.PMS_JRS_COA_DESCRIPTION = "";
              budgetItem.PMS_JRS_COA_ID = 0;
              budgetItem.PMS_JRS_COA_CODE = "";
              budgetItem.PMS_JRS_COI_ID = 0;
              budgetItem.PMS_JRS_TOS_ID = 0;

              for (k = 0; k < resultItm.length; k++) {
                budgetItem.PERCENTAGE = resultItm[k].PERCENTAGE;
                budgetItem.UNIT = resultItm[k].UNIT;
                budgetItem.TIMING = resultItm[k].TIMING;
                budgetItem.TIME_UNIT = resultItm[k].TIME_UNIT;
                budgetItem.UNIT_TYPE = resultItm[k].UNIT_TYPE;
                budgetItem.PMS_JRS_COA_DESCRIPTION = resultItm[k].PMS_JRS_COA_DESCRIPTION;
                budgetItem.PMS_JRS_COA_CODE = resultItm[k].PMS_JRS_COA_CODE;
                budgetItem.PMS_JRS_COA_ID = resultItm[k].PMS_JRS_COA_ID;
                budgetItem.UNIT_PRICE = resultItm[k].UNIT_PRICE;
                budgetItem.BUDGET = 0;

                budgetItem.CURRENCY = resultItm[k].CURRENCY;
                budgetItem.SELECTED[resultItm[k].MONTH - 1] = true;
                if (resultItm[k].VALUE != undefined && resultItm[k].VALUE != null)
                  budgetItem.MONTHBUDGET[resultItm[k].MONTH - 1] = resultItm[k].VALUE;
                if (resultItm[k].FUNDED != undefined && resultItm[k].FUNDED != null)
                  budgetItem.MONTHFUNDED[resultItm[k].MONTH - 1] = resultItm[k].FUNDED;
                budgetItem.YEARBUDGET += resultItm[k].VALUE;
                budgetItem.PMS_JRS_COI_ID = resultItm[k].PMS_COI_ID;
                budgetItem.PMS_JRS_TOS_ID = resultItm[k].PMS_TOS_ID;
              }
              localThis.TotalActivityBudget += budgetItem.YEARBUDGET;
              budgetItems.push(budgetItem);
            }
            localThis.budgetLines = budgetItems;
          }
          localThis.rowKey = localThis.getRnd();
        })
        .then((res: any) => {
          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          console.log(err);
          return err;
        });
    },
  },

  mounted() {
    let localThis = this as any;
    localThis.getBudget();
  },
  beforeMount() {
    let localThis = this as any;
    localThis.FillFormData(null);
    localThis.getDonorsWithGrantsOnProject();
    localThis.actDesc = localStorage.SelectedAct;
  },
});
</script>

<style scoped></style>
