<template>
  <div>
    <v-tabs
      v-model="tabIsVisible"
      background-color="primary darken-1"
      dark
      v-if="showActTabs"
    >
      <div class="vertical-center">
        <v-btn
          color="secondary lighten-2"
          class="grey--text text--darken-3"
          @click="returnToActivityList()"
        >
          <v-icon>{{ sIconBack }}</v-icon>
        </v-btn>
      </div>

      <v-tab v-for="item in tabsItems" :key="item.code">{{ item.descr }}</v-tab>
      <v-spacer></v-spacer>
      <v-chip class="ma-2" color="default">
        <v-avatar left>
          <v-icon>mdi-chart-line</v-icon>
        </v-avatar>
        {{ localEditActDesc }}
      </v-chip>
    </v-tabs>
    <v-tabs-items v-model="tabIsVisible" style="padding-left: 1em">
      <v-tab-item key="MAINDATA">
        <v-card>
          <pms-act-form :formData="actMainDataItem"></pms-act-form>
        </v-card>
      </v-tab-item>

      <!-- NARRATIVE -->
      <v-tab-item key="NARRATIVE" v-if="false">
        <ap-narrative
          :isUpdatableForm="!onlyRead"
          :selectedObjectId="editActId"
          :tableName="tblName"
          :key="Math.ceil(Math.random() * 1000)"
          :versioned="versioned"
        ></ap-narrative>
      </v-tab-item>

      <!-- POSITIONS -->
      <v-tab-item key="POSITIONS" v-if="module == 'IMP'">
        <ap-positions
          :onlyRead="onlyRead"
          :selectedObjectId="editActId"
          :showPositionDetails="true"
          :key="Math.ceil(Math.random() * 1000)"
          :versioned="versioned"
        ></ap-positions>
      </v-tab-item>
      <!-- OUTPUTS -->
      <v-tab-item key="OUTPUTS">
        <ap-output
          :onlyRead="onlyRead"
          :selectedActivityId="editActId"
          :selectedProcessId="editProcId"
          :showItemDetails="true"
          :versioned="versioned"
        ></ap-output>
      </v-tab-item>
      <!-- RESOURCES -->
      <v-tab-item key="RESOURCES" v-if="false">
        <ap-resources
          :onlyRead="onlyRead"
          :selectedObjectId="editActId"
          :showItemDetails="true"
          :key="Math.ceil(Math.random() * 1000)"
          :versioned="versioned"
        ></ap-resources>
      </v-tab-item>

      <!-- BUDGET -->
      <v-tab-item key="BUDGET" v-if="false">
        <ap-schedule
          :onlyRead="onlyRead"
          :selectedObjectId="editActId"
          :key="Math.ceil(Math.random() * 1000)"
          :versioned="versioned"
        ></ap-schedule>
      </v-tab-item>
      <!-- BUDGET -->
      <!-- <v-tab-item key="BUDGET">
        <ap-budget :selectedObjectId="editActId" :key="Math.ceil(Math.random() * 1000)"></ap-budget>
      </v-tab-item> -->
    </v-tabs-items>
  </div>
</template>

<script lang="ts">
import axios from "axios";
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import { translate } from "../../../i18n";
//import ActivityMainData from "./AnnualPlanActivityMainDataForm.vue";
import ProcessMainData from "./AnnualPlanProcessMainDataForm.vue";
import ActivitySchedule from "../ACTIVITIES/AnnualPlanActivityPlanForm.vue";
import Narrative from "../ANNUALPLAN/AnnualPlanNarrative.vue";
import Output from "../OUTPUTS/AnnualPlanOutputForm.vue";
import Resources from "../ITEMS/AnnualPlanItemForm.vue";
// import Budget from "../BUDGET/AnnualPlanBudgetForm.vue";
import Positions from "../POSITIONS/AnnualPlanPositionForm.vue";
import { EventBus } from "../../../event-bus";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../axiosapi";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
export default Vue.extend({
  components: {
    "pms-act-form": ProcessMainData,
    "ap-narrative": Narrative,
    "ap-schedule": ActivitySchedule,
    "ap-resources": Resources,
    "ap-positions": Positions,
    "ap-output": Output,
    //"ap-budget": Budget
    //,
    // "ap-ind-main-data": AnnualPlanActivityMainData
  },
  props: {
    // selectedAnnualPlan: {
    //   type: Object
    // },
    actMainDataItem: {
      type: Object,
    },
    editActId: {
      type: Number,
    },
    editProcId: {
      type: Number,
    },
    editActDesc: {
      type: String,
      required: true,
    },

    selectedObjectId: {
      type: Number,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  data() {
    return {
      posOnlyRead: false,
      moodule: "",
      showActTabs: true,
      sIconBack: "mdi-chevron-double-left",
      tblName: "PMS_ACTIVITY",
      tabIsVisible: null,
      localEditActDesc: "",
      editedActMainDataItem: {},
      tabsItems: [
        { code: "MAINDATA", descr: "Process Info" },
        // { code: "NARRATIVE", descr: "Narrative" },
        //", descr: "Positions" },
        { code: "OUTPUTS", descr: "Outputs" },
        // { code: "RESOURCES", descr: "Resources" },
        // { code: "TIMELINE", descr: "Timeline" },
        // { code: "BUDGET", descr: "Budget" },
      ],
      oldAct: {},
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
    hideIndecTabs() {
      let localThis: any = this as any;
      localThis.showActTabs = false;
    },

    returnToActivityList() {
      let localThis: any = this as any;
      EventBus.$emit("closeActivityDetails");
    },

    refreshActDesc(item: any) {
      let localThis: any = this as any;
      localThis.localEditActDesc = item.DESCRIPTION;
      localStorage.SelectedAct = item.DESCRIPTION;
      EventBus.$emit("refreshBreadCumb", 2);
    },
  },
  beforeMount() {
    let localThis = this as any;
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
    if (localThis.module == "IMP") {
      localThis.tabsItems = [
        { code: "MAINDATA", descr: "Process Info" },
        // { code: "NARRATIVE", descr: "Narrative" },
        { code: "POSITIONS", descr: "Positions" },
        { code: "OUTPUTS", descr: "Outputs" },
        // { code: "RESOURCES", descr: "Resources" },
        // { code: "BUDGET", descr: "Budget" },
      ];
      localThis.posOnlyRead = false;
    } else {
      localThis.tabsItems = [
        { code: "MAINDATA", descr: "Process Info" },
        // { code: "NARRATIVE", descr: "Narrative" },
        { code: "OUTPUTS", descr: "Outputs" },
        // { code: "RESOURCES", descr: "Resources" },
        // { code: "BUDGET", descr: "Budget" },
      ];
      localThis.posOnlyRead = localThis.onlyRead;
    }
  },
  mounted() {
    let localThis = this as any;
    localThis.localEditActDesc = localThis.editActDesc;
    localStorage.SelectedAct = localThis.localEditActDesc;
    // localThis.actMainDataItem.DESCRIPTION = localThis.editActDesc;
    // localThis.actMainDataItem.SERVICE_ID = localThis.selectedObjectId;
    // localThis.actMainDataItem.ID = localThis.editActId;
    EventBus.$on("hideActTabs", (data: any) => {
      localThis.showActTabs = false;
    });

    EventBus.$on("showActTabs", (data: any) => {
      localThis.showActTabs = true;

      //EventBus.$emit("refreshBreadCumb", 2);
    });
  },
  computed: {
    ...mapGetters({
      getActiveModule: "getActiveModule",
    }),
  },
  watch: {
    editActDesc(desc: String) {
      let localThis = this as any;
      localThis.localEditActDesc = desc;
      localStorage.SelectedAct = localThis.localEditActDesc;
      EventBus.$emit("refreshBreadCumb", 2);
    },
  },
});
</script>
<style scoped>
.vertical-center {
  margin-right: 0.2rem;
  margin-top: 0.3rem;
}
</style>
