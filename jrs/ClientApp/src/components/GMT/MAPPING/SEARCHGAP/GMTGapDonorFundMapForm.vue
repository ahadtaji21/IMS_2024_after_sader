<template>
  <div style="margin: 0px; padding: 0px">
    <v-row :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
      <v-col
        :class="'dataStyleClear'"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
        >{{ donor.GMT_DONOR_NAME }}</v-col
      >
      <v-col
        :class="'dataStyleClear'"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >{{ donor.GRANT_CODE }}
      </v-col>
      <v-col
        :class="'dataStyleClear'"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
      >
        <currency-input
          v-model="donor.GRANT_INITIAL_AMOUNT"
          v-bind="options"
          :currency="donor.GRANT_CURRENCY_CODE"
          :readonly="true"
        ></currency-input>
      </v-col>
      <v-col
        :class="'dataStyleClear'"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
      >
        <currency-input
          v-model="donor.AVAILABLE"
          v-bind="options"
          :currency="donor.GRANT_CURRENCY_CODE"
          :readonly="true"
        ></currency-input>
      </v-col>
      <v-col
        :class="'dataStyleClear'"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >{{ round((100 * donor.AVAILABLE) / donor.GRANT_INITIAL_AMOUNT, 2) }}%
      </v-col>
      <v-col
        :class="'dataStyleClear'"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >{{ monthAllocatedPercentage() }}%
      </v-col>
      <v-col
        :class="'dataStyleClear'"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
      >
        <currency-input
          v-model="monthAllocatedValue"
          v-bind="options"
          :currency="donor.GRANT_CURRENCY_CODE"
          :readonly="true"
        ></currency-input>
      </v-col>
      <v-col
        :class="'dataStyleClear'"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
      >
        <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <v-icon small class="mr-4" @click="OpenDonorDetails()" v-on="on"
              >mdi-google-circles-extended</v-icon
            >
          </template>
          <span>{{ $t("datasource.gmt.budget.gap-details") }}</span>
        </v-tooltip>
        <!-- <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <v-icon small class="mr-2" @click="editAssignments()" v-on="on"
              >mdi-circle-edit-outline</v-icon
            >
          </template>
          <span>{{ $t("datasource.gmt.budget.gap-assignment-details") }}</span>
        </v-tooltip> -->
      </v-col>
      <!-- <v-dialog
        v-model="matchMode"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        transition="dialog-transition"
      >
        <gmt-coi-tos-coa
          v-model="linkedData"
          @UpdateBudgetItem="closeBudgetDialog"
          @closeDialoge="closeLinkedDataDialog"
          :formData="formData"
          :donorId="donor.GMT_DONOR_ID"
          :key="Math.ceil(Math.random() * 1000)"
        ></gmt-coi-tos-coa>
      </v-dialog> -->
    </v-row>
    <v-row align-center v-if="showDetails & showWait">
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
    <div v-if="showDetails">
      <v-row :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
        <v-col
          :class="'headerStyleGapGrid'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >COI</v-col
        >
        <v-col
          :class="'headerStyleGapGrid'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >TOS</v-col
        >
        <v-col
          :class="'headerStyleGapGrid'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
          >COA Code</v-col
        >
        <v-col
          :class="'headerStyleGapGrid'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
          >COA Desc</v-col
        >
        <v-col
          :class="'headerStyleGapGrid'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
          >Fund State</v-col
        >
        <v-col
          :class="'headerStyleGapGrid'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 3"
        >
        </v-col>
        <v-col
          :class="'headerStyleGapGrid'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <v-icon
                small
                class="mr-4"
                @click="saveBudget()"
                v-on="on"
                style="color: white"
                >mdi-content-save</v-icon
              >
            </template>
            <span>{{
              $t("datasource.pms.annual-plan.objectives.save-inline-modifies")
            }}</span>
          </v-tooltip>
        </v-col>
      </v-row>
      <v-row
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
        v-for="(map, k) in mpDonorLineList"
        :key="k"
      >
        <v-col
          :class="'dataStyleClear'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <span v-on="on">{{ map.GMT_COI_CODE | subStr }}</span>
            </template>
            <span>{{ map.GMT_COI_CODE }}</span>
          </v-tooltip>
        </v-col>
        <v-col
          :class="'dataStyleClear'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <span v-on="on">{{ map.GMT_TOS_DESCRIPTION | subStr }}</span>
            </template>
            <span>{{ map.GMT_TOS_DESCRIPTION }}</span>
          </v-tooltip>
        </v-col>
        <v-col
          :class="'dataStyleClear'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
        >
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <span v-on="on">{{ map.ACC_CODE | subStr }}</span>
            </template>
            <span>{{ map.ACC_CODE }}</span>
          </v-tooltip>
        </v-col>
        <v-col
          :class="'dataStyleClear'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
        >
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <span v-on="on">{{ map.ACC_DESCRIPTION | subStr }}</span>
            </template>
            <span>{{ map.ACC_DESCRIPTION }}</span>
          </v-tooltip>
        </v-col>
        <v-col
          :class="'dataStyleClear'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
        >
          <v-select
            :items="fundingStates"
            item-text="text"
            item-value="value"
            v-model="map.FUNDING_STATE_ID"
            hide-details
            class="ma-0 pa-0"
          ></v-select>
        </v-col>
        <v-col
          :class="'dataStyleClear'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >
          <v-text-field
            v-model="map.ASSIGNED_PERCENTAGE"
            @keypress="isNumber($event)"
            @blur="checkPercentage(k)"
            hide-details
            class="ma-0 pa-0"
            :class="map.MODIFIED == false ? 'dataStyle' : 'rowStyleModified'"
          />
        </v-col>
        <v-col
          :class="'dataStyleClear'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
        >
          <currency-input
            v-model="map.ASSIGNED_VALUE"
            v-bind="options"
            :readonly="true"
          ></currency-input>
        </v-col>
        <v-col
          :class="'dataStyleClear'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >
        </v-col>
      </v-row>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../../i18n";
import FormMixin from "../../../../mixins/FormMixin";
// import matchCOITOSCOA from "./GMTGapLinkCOITOSCOAForm.vue";
import { EventBus } from "../../../../event-bus";
import { IBudgetDetails } from "./../../../PMS/PMSSharedInterfaces";
import {
  ImsApi,
  GenericSqlApi,
  ImsLookupsApi,
  Configuration,
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi";

export default Vue.extend({
  components: {
    // "gmt-coi-tos-coa": matchCOITOSCOA,
  },
  props: {
    month: {
      type: Number,
      required: true,
    },
    donorOriginal: {
      type: Object,
      required: true,
    },
    formData: {
      type: Object,
      required: true,
    },
    year: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      monthAllocatedValue: 0,
      budgetModified: false,
      donor: {} as any,
      fundingStates: [],
      assignedPercentage: 0,
      mpDonorLineList: [],
      showWait: false,
      showDetails: false,
      linkedData: {},
      matchMode: false,
      mpDonorLineListStarting: [],
      mpDonorLineListPrevious: [],
      donorPrevious: [],
    };
  },
  mixins: [FormMixin],
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

    checkPercentage(k: number) {
      let localThis = this as any;
      let originalAssigmentPercentage: number =
        localThis.mpDonorLineListPrevious[k].ASSIGNED_PERCENTAGE;
      if (
        originalAssigmentPercentage ==
        localThis.mpDonorLineList[k].ASSIGNED_PERCENTAGE
      )
        return;
      localThis.mpDonorLineList[k].MODIFIED = true;
      let percentageReservedToGroup: number = localThis.getActualPercentageReservedToGroup();

      localThis.mpDonorLineList[k].ASSIGNED_PERCENTAGE = localThis.round(
        localThis.mpDonorLineList[k].ASSIGNED_PERCENTAGE,
        2
      );
      if (localThis.mpDonorLineList[k].ASSIGNED_PERCENTAGE > 100)
        localThis.mpDonorLineList[k].ASSIGNED_PERCENTAGE = 100;

      let totalPercAssignedToOthers: number = 0;
      let i: number = 0;
      for (i = 0; i < localThis.mpDonorLineList.length; i++) {
        if (i != k)
          totalPercAssignedToOthers +=
            localThis.mpDonorLineList[i].ASSIGNED_PERCENTAGE;
      }
      let totalPercAssigned: number =
        totalPercAssignedToOthers +
        localThis.mpDonorLineList[k].ASSIGNED_PERCENTAGE;
      if (
        localThis.mpDonorLineList[k].ASSIGNED_PERCENTAGE -
          originalAssigmentPercentage >
        percentageReservedToGroup
      ) {
        localThis.mpDonorLineList[k].ASSIGNED_PERCENTAGE = localThis.round(
          percentageReservedToGroup + originalAssigmentPercentage,
          2
        );
      }

      localThis.mpDonorLineList[k].ASSIGNED_VALUE = localThis.round(
        (localThis.mpDonorLineList[k].ASSIGNED_PERCENTAGE *
          localThis.mpDonorLineList[k].INITIAL_GRANT_AMOUNT) /
          100,
        2
      );

      localThis.checkGroupPercentage();
      localThis.mpDonorLineListPrevious[k] = JSON.parse(
        JSON.stringify(localThis.mpDonorLineList[k])
      );
    },

    getInitialPercentageReservedToGroup() {
      let localThis = this as any;
      let i: number = 0;
      return localThis.round(
        (100 * localThis.donorPrevious.AVAILABLE) /
          localThis.donor.GRANT_INITIAL_AMOUNT,
        2
      );
    },
    getActualPercentageReservedToGroup() {
      let localThis = this as any;
      let i: number = 0;
      return localThis.round(
        (100 * localThis.donor.AVAILABLE) /
          localThis.donor.GRANT_INITIAL_AMOUNT,
        2
      );
    },

    checkGroupPercentage() {
      let localThis = this as any;
      let i: number = 0;
      let totalAssignedActual: number = 0;
      totalAssignedActual = localThis.getDonorMonthAssigned();
      let totalAssignedInitial: number = 0;
      totalAssignedInitial = localThis.getDonorMonthOriginalAssigned();
      localThis.donor.AVAILABLE =
        localThis.donor.AVAILABLE + totalAssignedInitial - totalAssignedActual;



       EventBus.$emit(
        "ReloadSummary",
        localThis.month,
        localThis.donorPrevious.AVAILABLE,
        localThis.donor.AVAILABLE,
        localThis.donor.GRANT_CURRENCY_CODE
      );
      EventBus.$emit(
        "DonorAvailabilityChanged",
        localThis.donor.GMT_DONOR_ID,
        localThis.donor.AVAILABLE,
        localThis.month
      );


      //localThis.$emit("UpdateBudgetList",localThis.mpDonorLineList[k]);
    },
    monthAllocatedPercentage() {
      let localThis = this as any;
      let i: number = 0;

      let tot: number = 0;
      tot = localThis.getDonorMonthAssigned();

      localThis.monthAllocatedValue = tot;

      return localThis.round(
        (100 * tot) / localThis.donor.GRANT_INITIAL_AMOUNT,
        2
      );
    },
    getDonorMonthOriginalAssigned() {
      let localThis = this as any;
      let i: number = 0;
      let tot: number = 0;
      if (localThis.mpDonorLineListPrevious == undefined) return 0;
      for (i = 0; i < localThis.mpDonorLineListPrevious.length; i++) {
        tot += localThis.mpDonorLineListPrevious[i].ASSIGNED_VALUE;
      }
      return tot;
    },
    getDonorMonthAssigned() {
      let localThis = this as any;
      let i: number = 0;
      let tot: number = 0;
      for (i = 0; i < localThis.mpDonorLineList.length; i++) {
        tot += localThis.mpDonorLineList[i].ASSIGNED_VALUE;
      }
      return tot;
    },
    editAssignments: function () {},

    AssociateDCOITOSCOA() {
      let localThis = this as any;
      // localThis.formData = {} as any;
      // localThis.formData.JRS_COI_ID = localThis.formData.PMS_JRS_COI_ID;
      // localThis.formData.JRS_TOS_ID = localThis.formData.PMS_JRS_TOS_ID;
      // localThis.formData.JRS_COA_ID = localThis.formData.PMS_JRS_COA_ID;
      localThis.matchMode = true;
    },
    OpenDonorDetails() {
      let localThis: any = this as any;
      localThis.showDetails = !localThis.showDetails;
    },

    LoadData(mappedDonorLineList: any) {
      let localThis: any = this as any;
      localThis.mpDonorLineList = [] as IBudgetDetails[];
      localThis.mpDonorLineListStarting = [] as IBudgetDetails[];
      localThis.mpDonorLineListPrevious = [] as IBudgetDetails[];
      //localThis.showWait = true;

      mappedDonorLineList.forEach((item: any) => {
        if (
          item.MONTH == localThis.month &&
          item.DNR_ID == localThis.donor.GMT_DONOR_ID &&
          item.GRANT_PRJ_REL_ID == localThis.donor.GRANT_PRJ_REL_ID
        ) {
          item.ASSIGNED_PERCENTAGE = localThis.round(
            (item.ASSIGNED_VALUE / item.INITIAL_GRANT_AMOUNT) * 100,
            2
          );
          item.MODIFIED = false;
          localThis.mpDonorLineList.push(item);
        }
      });
      localThis.mpDonorLineListStarting = JSON.parse(
        JSON.stringify(localThis.mpDonorLineList)
      );
      localThis.mpDonorLineListPrevious = JSON.parse(
        JSON.stringify(localThis.mpDonorLineList)
      );
    },

    saveBudget() {
      let localThis = this as any;

      const config: Configuration =
        localThis.$store.getters["auth/getApiConfig"];
      const api: GenericSqlApi = new GenericSqlApi(config);

      for (let i: number = 0; i < localThis.mpDonorLineList.length; i++) {
        if (
          localThis.mpDonorLineList[i].ASSIGNED_VALUE > 0 &&
          localThis.mpDonorLineList[i].FUNDING_STATE_ID == ""
        ) {
          localThis.showNewSnackbar({
            typeName: "error",
            text: "Choose the Funding State",
          });
          return;
        }
      }
      localThis.showWait = true;
      let payLoadD = {} as any;

      payLoadD.BUDGET = localThis.mpDonorLineList;
      // payLoadD.ACTIVITY_ID = item.ACTIVITY_ID;
      // payLoadD.FUNDING_STATE_ID = item.FUNDING_STATE_ID;
      // payLoadD.VALUE = item.ASSIGNED_VALUE;
      // payLoadD.GMT_MAPPING_ID = item.ID;
      // payLoadD.BUDGET_ID = item.BUDGET_ID;
      payLoadD.MONTH = localThis.month;
      payLoadD.YEAR = localThis.year;
      payLoadD.CURRENCY = localThis.donor.GRANT_CURRENCY_CODE;

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_BUDGET_MAPPING",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      return api
        .genericSqlSPCall(savePayload)
        .then((res: any) => {
          if (res.data.rows) {
            let x: number = 0;
            res.data.rows.forEach((item: any) => {
              for (
                let i: number = 0;
                i < localThis.mpDonorLineList.length;
                i++
              ) {
                if (
                  localThis.mpDonorLineList[i].ID ==
                  item.GMT_MAPPING_ID
                )
                  localThis.mpDonorLineList[i].BUDGET_ID = item.id;
              }
            });
          }

          localThis.showNewSnackbar({
            typeName: "success",
            text: res.data.message,
          }); // Feedback to user
          localThis.showWait = false;
          // localThis.$emit("RefreshAfterModifies",localThis.month).then((res: any) => {
          //   localThis.showDetails=true;
          //});
          for (let i: number = 0; i < localThis.mpDonorLineList.length; i++) {
            localThis.mpDonorLineList[i].MODIFIED = false;
          }
          localThis.mpDonorLineListStarting = JSON.parse(
            JSON.stringify(localThis.mpDonorLineList)
          );
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.data.message,
          }); // Feedback to user
        });
    },

    closeBudgetDialog(item: string) {
      let localThis: any = this as any;
      localThis.matchMode = false;
      (this as any).$emit("UpdateBudgetList");
    },
    budgetItem(item: any) {
      let localThis = this as any;
      localThis.matchMode = !localThis.matchMode;
    },
    closeLinkedDataDialog() {
      let localThis = this as any;
      localThis.matchMode = false;
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.donor = JSON.parse(JSON.stringify(localThis.donorOriginal));
    localThis.donorPrevious = JSON.parse(
      JSON.stringify(localThis.donorOriginal)
    );
    localThis.donorOriginalLocal = JSON.parse(
      JSON.stringify(localThis.donorOriginal)
    );
    //localThis.mpDonorLineList = JSON.parse(JSON.stringify(data))
    EventBus.$on("budgetdataloaded", (data: any) => {
      localThis.LoadData(data);
    });
    EventBus.$on("statuslistloaded", (data: any) => {
      localThis.fundingStates = data;
    });
    localThis.donor.GMT_DONOR_ID, localThis.donor.AVAILABLE, localThis.month;
    EventBus.$on(
      "DonorAvailabilityChanged",
      (_gmt_donor_id: number, _available: number, _month: number) => {
        if (
          localThis.donor.GMT_DONOR_ID == _gmt_donor_id &&
          localThis.month != _month
        ) {
          // console.log(
          //   `GMT_DONOR_ID: ${localThis.donor.GMT_DONOR_ID} -  ${_gmt_donor_id} - ${localThis.month} - ${_month} - ${_available}`
          // );
          localThis.donor.AVAILABLE = JSON.parse(JSON.stringify(_available));
          localThis.donorPrevious.AVAILABLE = JSON.parse(
            JSON.stringify(_available)
          );
          localThis.donorOriginalLocal.AVAILABLE = JSON.parse(
            JSON.stringify(_available)
          );
        }
      }
    );
  },
  filters: {
    subStr: function (string: any) {
      if (string.length < 10) return string;
      return string.substring(0, 7) + "...";
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
});
</script>

<style scoped>
</style>