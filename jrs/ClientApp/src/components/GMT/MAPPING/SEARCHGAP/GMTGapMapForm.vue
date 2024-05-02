<template>
  <!-- componente che mostra la lista dei mesi e per ogni mese carica i donors che hanno grants -->
  <div>
    <v-row align-center v-if="showWait">
      <v-col justify-center :cols="12">
        <div
          class="text-center"
          v-if="showWait"
          style="margin: 0px; padding: 0px"
        >
          <v-progress-circular
            indeterminate
            color="primary"
          ></v-progress-circular>
        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col
        :class="'headerStyleGapGridNoBorders'"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
      >
        <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <span v-on="on">
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3"
                @click="closeMap()"
              >
                <v-icon>{{ sIconBack }}</v-icon>
              </v-btn>
            </span>
          </template>
          <span>{{ $t("datasource.gmt.budget.back-to-list") }}</span>
        </v-tooltip>
      </v-col>
      <v-col
        :class="'headerStyleGapGridNoBorders'"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 10"
      >
      </v-col>
      <v-col
        :class="'headerStyleGapGridNoBorders'"
        class="text-right"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
      >
        <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <span v-on="on">
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3"
                @click="UndoModifies()"
              >
                <v-icon>mdi-redo-variant</v-icon>
              </v-btn>
            </span>
          </template>
          <span>{{
            $t("datasource.pms.annual-plan.objectives.undo-inline-modifies")
          }}</span>
        </v-tooltip>
      </v-col>
    </v-row>

    <v-row :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
      <v-col :class="'headerStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >Month</v-col
      >
      <v-col :class="'headerStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 8">
        <v-row
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
          no-gutters
          :class="'headerStyleNoExtBorder'"
        >
          <v-col
            :class="'headerStyleNoExtBorder'"
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
            >Donor</v-col
          >
          <v-col
            :class="'headerStyleNoExtBorder'"
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
            >Grant</v-col
          >
          <v-col
            :class="'headerStyleNoExtBorder'"
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
            >Initial (T)</v-col
          >
          <v-col
            :class="'headerStyleNoExtBorder'"
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
            >Actual (T)</v-col
          >
          <v-col
            :class="'headerStyleNoExtBorder'"
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
            >Max Allocable (T)%</v-col
          >
          <v-col
            :class="'headerStyleNoExtBorder'"
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
            >Assigned (M)%</v-col
          >
          <v-col
            :class="'headerStyleNoExtBorder'"
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
            >Assigned (M)#</v-col
          >
          <v-col
            :class="'headerStyleNoExtBorder1'"
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
            >Action</v-col
          >
        </v-row>
      </v-col>
      <v-col
        :class="'headerStyleNoLeftB'"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >JRS Needs
      </v-col>
      <v-col :class="'headerStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >Allocated</v-col
      >
      <v-col :class="'headerStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >Gap</v-col
      >
    </v-row>
    <v-row
      v-for="(sm, n) in summary"
      :key="n + 1"
      class="justify-center align-center"
      :class="'rowStyleBB'"
    >
      <v-col justify-center :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">
        <v-card class="pa-2" outlined tile>{{ rowDesk[n + 1] }}</v-card>
      </v-col>

      <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 8">
        <gmt-donor-fund-map
          :selectedProjectId="selectedProjectId"
          :formData="formData"
          :year="year"
          :RefreshAfterModifies="RefreshAfterModifies"
          :month="n + 1"
        ></gmt-donor-fund-map>
      </v-col>
      <v-col
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        :class="'dataStyleBlack'"
      >
        <span v-if="sm.PMS_CURRENCY_CODE != ''"
          ><currency-input
            v-model="sm.NEEDS"
            v-bind="options"
            :currency="sm.PMS_CURRENCY_CODE"
            :readonly="true"
          ></currency-input
        ></span>
        <span v-else> N/A </span>
      </v-col>
      <v-col
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        :class="'dataStyleGreen'"
      >
        <span
          v-if="sm.GMT_CURRENCY_CODE != undefined && sm.GMT_CURRENCY_CODE != ''"
          ><currency-input
            v-model="sm.ALLOCATED"
            v-bind="options"
            :currency="sm.GMT_CURRENCY_CODE"
            :readonly="true"
          ></currency-input
        ></span>
        <span v-else>N/A</span>
      </v-col>
      <v-col
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        :class="'dataStyleRed'"
      >
        <span v-if="sm.PMS_CURRENCY_CODE != ''"
          ><currency-input
            v-model="sm.GAP"
            v-bind="options"
            :currency="sm.PMS_CURRENCY_CODE"
            :readonly="true"
          ></currency-input
        ></span>
        <span v-else>N/A</span>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../../i18n";
import { EventBus } from "../../../../event-bus";
import GMTDonorFundMap from "./GMTGapDonorsFundMapForm.vue";
import {
  IBudgetItemUPD,
  IBudgetItemGET,
  IPayLoadDataITEM,
  IBudgetDetails,
} from "./../../../PMS/PMSSharedInterfaces";

import {
  ImsLookupsApi,
  GenericSqlSelectPayload,
  Configuration,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi";
interface ITEM_SUM {
  PROJECT_ID: number;
  ACTIVITY_ID: number;
  RESOURCE_ID: number;
  MONTH: number;
  PMS_CURRENCY_CODE: string;
  GMT_CURRENCY_CODE: string;
  ALLOCATED: number;
  GAP: number;
  NEEDS: number;
  ALLOCATED_ORIGINAL: number;
}
export default Vue.extend({
  components: {
    "gmt-donor-fund-map": GMTDonorFundMap,
  },
  props: {
    selectedProjectId: {
      type: Number,
      required: true,
    },
     activityId: {
      type: Number,
      required: true,
    },

    year: {
      type: Number,
      required: true,
    },
    currencyCode: {
      type: String,
      required: false,
    },

    formData: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      fundingStates: [],
      showWait: false,
      mappedDonorLineList: [],
      summary: [],
      rowDesk: [],
      sIconBack: "mdi-chevron-double-left",
      TotalActivityBudget: 0,
      budgetMode: false,
      resourceId: {},
      editMode: false,
      actDesc: "",

      rowKey: 0,

      selected: [],
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

    closeMap() {
      let localThis = this as any;
      localThis.$emit("closeMap");
    },
    UndoModifies() {
      let localThis = this as any;
      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-undo-modifies")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.$emit("reload");
        }
      });
    },
    RefreshAfterModifies() {
      let localThis = this as any;
      localThis.$emit("reload");
    },

    UpdateBudgetList() {
      let localThis: any = this as any;
      localThis.getBudget();
    },
    closeDialog(item: String) {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.editedItemDialog = {};
      // if (item != null) localThis.UpdateItem(item);
    },
    getList(listName: any) {
      let localThis = this as any;
      localThis.showWait = true;
      if (
        localThis.fundingStates != undefined &&
        localThis.fundingStates.length > 0
      ) {
        localThis.showWait = false;
        return;
      }
      const config: Configuration =
        localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsListNameGet(listName, config.baseOptions)
        .then((res: any) => {
          switch (listName) {
            case "FUNDING_STATUS":
              localThis.fundingStates = res.data;
              break;
          }
          //obj = res.data;
          //alert(res.data[0].pmsTosDescription);
          return res;
        })
        .catch((err: any) => {
          // console.error(err);
          return [];
        })
        .finally(() => {
          EventBus.$emit("budgetdataloaded", localThis.mappedDonorLineList);
          EventBus.$emit("statuslistloaded", localThis.fundingStates);
          localThis.showWait = false;
        });
    },
    LoadData() {
      let localThis: any = this as any;

      localThis.showWait = true;

      localThis.mappedDonorLineList = [] as IBudgetDetails[];

      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_ACTIVITY_BUDGET_DETAILS",
        colList: null,
        whereCond: `ACTIVITY_ID = ${localThis.formData.ACTIVITY_ID}  AND JRS_COA_ID = ${localThis.formData.PMS_JRS_COA_ID} AND JRS_COI_ID = ${localThis.formData.PMS_JRS_COI_ID} AND JRS_TOS_ID = ${localThis.formData.PMS_JRS_TOS_ID} AND (${localThis.activityId}=0 OR GMT_ACTIVITY_ID=${localThis.activityId})`, // AND YEAR = ${localThis.year}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.mappedDonorLineList.push(item);
            });
          }
          //localThis.rowKey = localThis.getRnd();
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          });
        })
        .finally(() => {
          localThis.getList("FUNDING_STATUS");
        });
    },

    getSummary() {
      let localThis = this as any;

      localThis.summary = [] as ITEM_SUM[];

      let i: number = 0;
      for (i = 0; i < 12; i++) {
        let obj = {} as ITEM_SUM;
        obj.PROJECT_ID = -1;
        obj.ACTIVITY_ID = -1;
        obj.RESOURCE_ID = -1;
        obj.MONTH = -1;
        obj.PMS_CURRENCY_CODE = "";
        obj.GMT_CURRENCY_CODE = "";
        obj.ALLOCATED = 0;
        obj.GAP = 0;
        obj.NEEDS = 0;
        obj.ALLOCATED_ORIGINAL=0;

        localThis.summary.push(obj);
      }
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONOR_PROJECTS_GRANT_SUMMARY",
        //colList: null,
        whereCond: `PROJECT_ID=${localThis.selectedProjectId} AND ACTIVITY_ID=${localThis.formData.ACTIVITY_ID}  AND RESOURCE_ID=${localThis.formData.PMS_JRS_COA_ID}`,
        orderStmt: "MONTH",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.summary[item.MONTH - 1].PROJECT_ID = item.PROJECT_ID;
              localThis.summary[item.MONTH - 1].ACTIVITY_ID = item.ACTIVITY_ID;
              localThis.summary[item.MONTH - 1].RESOURCE_ID = item.RESOURCE_ID;
              localThis.summary[item.MONTH - 1].MONTH = item.MONTH;
              localThis.summary[item.MONTH - 1].PMS_CURRENCY_CODE =
                item.PMS_CURRENCY_CODE;
              localThis.summary[item.MONTH - 1].GMT_CURRENCY_CODE =
                item.GMT_CURRENCY_CODE;
              localThis.summary[item.MONTH - 1].ALLOCATED = item.ALLOCATED;
              localThis.summary[item.MONTH - 1].GAP = item.GAP;
              localThis.summary[item.MONTH - 1].NEEDS = item.NEEDS;
              localThis.summary[item.MONTH - 1].ALLOCATED_ORIGINAL = item.ALLOCATED;
            });
          }
        });
    },
    closeBudgetDialogAndUpdList(item: string) {
      let localThis: any = this as any;
      localThis.budgetMode = false;
      //   localThis.getBudget();
    },
    closeBudgetDialog(item: string) {
      let localThis: any = this as any;
      localThis.budgetMode = false;
    },
  },

  mounted() {
    let localThis = this as any;
    localThis.LoadData();

    EventBus.$on(
      "ReloadSummary",
      (month: number, previous_available: number, actual_available: number,currency_code:string) => {
        let localThis = this as any;
        localThis.summary[month - 1].GMT_CURRENCY_CODE = currency_code;
        localThis.summary[month - 1].ALLOCATED = localThis.summary[month - 1].ALLOCATED_ORIGINAL+previous_available-actual_available;
        localThis.summary[month - 1].GAP = localThis.summary[month - 1].NEEDS-localThis.summary[month - 1].ALLOCATED;
     
      }
    );
  },
  beforeMount() {
    let localThis = this as any;
    localThis.getSummary();
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
  },
});
</script>

<style scoped>
</style>