<template>
  <div >
    <v-tabs v-model="tabIsVisible" background-color="primary darken-1" dark v-if="showObjectTabs">
      <div class="vertical-center">
        <v-btn
          color="secondary lighten-2"
          class="grey--text text--darken-3"
          @click="returnToActivityList()"
        >
          <v-icon>{{sIconBack}}</v-icon>
        </v-btn>
      </div>

      <v-tab v-for="item in tabsItems" :key="item.code">{{ item.descr }}</v-tab>
      <v-spacer></v-spacer>
      <v-chip class="ma-2" color="default">
        <v-avatar left>
          <v-icon>mdi-chart-line</v-icon>
        </v-avatar>
        {{localEditActDesc}}
      </v-chip>
    </v-tabs>
    <v-tabs-items v-model="tabIsVisible" style="padding-left: 0em">
      <v-tab-item key="MAINDATA">
        <v-card>
          <gt-addact-form
            @closeDialoge="returnToActivityList"
            :formDataExt="editActMainDataItem"
            :selectedObjectId="selectedObjectId"
            @refreshAct="refreshActDesc"
            :onlyRead="onlyRead"
          ></gt-addact-form>
        </v-card>
      </v-tab-item>

      <!-- NARRATIVE -->
      <v-tab-item key="NARRATIVE">
        <gt-narrative :onlyRead="onlyRead" :selectedObjectId="editActId" :tableName="tblName" :key="Math.ceil(Math.random() * 1000)"></gt-narrative>
      </v-tab-item>
 <!-- MAPPING -->
      <v-tab-item key="MAPPING">
        <gt-mapping :onlyRead="onlyRead" :selectedGMTActivitytId="editActId"  :key="Math.ceil(Math.random() * 1000)"></gt-mapping >
      </v-tab-item> 


      <!-- POSITIONS -->
      <!-- <v-tab-item key="POSITIONS">
        <gt-positions :selectedObjectId="editActId" :showPositionDetails="true" :key="Math.ceil(Math.random() * 1000)"></gt-positions>
      </v-tab-item>  -->
      <!-- RESOURCES -->
     <!-- <v-tab-item key="RESOURCES">
        <gt-resources :selectedObjectId="editActId" :showItemDetails="true" :key="Math.ceil(Math.random() * 1000)"></gt-resources>
      </v-tab-item> -->
      <!-- BUDGET -->
      <!-- <v-tab-item key="BUDGET">
        <gt-schedule :selectedObjectId="editActId"  :key="Math.ceil(Math.random() * 1000)"></gt-schedule>
      </v-tab-item> -->
       <!-- BUDGET -->
      <!-- <v-tab-item key="BUDGET">
        <gt-budget :selectedObjectId="editActId" :key="Math.ceil(Math.random() * 1000)"></gt-budget>
      </v-tab-item> -->
    </v-tabs-items>
  </div>
</template>

<script lang="ts">
import axios from "axios";
import Vue from "vue";
import { mapActions } from "vuex";
import { translate } from "../../../../../i18n";
import ActivityMainData from "./GMTGrantActivityMainDataForm.vue";
// import ActivitySchedule from "./AnnualPlanActivityPlanForm.vue";
import Narrative from "../NARRATIVE/GMTGrantNarrative.vue";
// import Resources from "../ITEMS/AnnualPlanItemForm.vue";
// import Budget from "../BUDGET/AnnualPlanBudgetForm.vue";
// import Positions from "../POSITIONS/AnnualPlanPositionForm.vue";

import Mapping from "./GMTGrantActivityMappingForm.vue";
import { EventBus } from "../../../../../event-bus";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../../../axiosapi";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../../../../axiosapi/api";
export default Vue.extend({
  components: {
    "gt-addact-form": ActivityMainData,
    "gt-narrative": Narrative,
     "gt-mapping": Mapping,
    // "gt-schedule": ActivitySchedule,
    // "gt-resources": Resources,
    // "gt-positions": Positions
    //"gt-budget": Budget
    //,
    // "gt-ind-main-data": AnnualPlanActivityMainData
  },
  props: {
    // selectedAnnualPlan: {
    //   type: Object
    // },
    editActMainDataItem: {
      type: Object
    },
    editActId: {
      type: Number
    },
    editActDesc: {
      type: String,
      required: true
    },

    selectedObjectId: {
      type: Number,
      required: true
    },
    onlyRead:{
      type:Boolean,
      required:false,
      default:false
    }
  },
  data() {
    return {
      showObjectTabs: true,
      sIconBack: "mdi-chevron-double-left",
      tblName: "GMT_ACTIVITY",
      tabIsVisible: null,
      localEditActDesc: "",
      editedActMainDataItem: {},
      tabsItems: [
        { code: "MAINDATA", descr: "Main Data" },
        { code: "NARRATIVE", descr: "Narrative" },
       { code: "MAPPING", descr: "Mapping"},
        //{ code: "RESOURCES", descr: "Resources" },
        // { code: "TIMELINE", descr: "Timeline" },
        //{ code: "BUDGET", descr: "Budget" },
      ],
      oldAct: {}
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),
    hideIndecTabs() {
      let localThis: any = this as any;
      localThis.showObjectTabs = false;
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
    }
  },
  mounted() {
    let localThis = this as any;
    localThis.localEditActDesc = localThis.editActDesc;
    localStorage.SelectedAct = localThis.localEditActDesc;
    localThis.editActMainDataItem.DESCRIPTION = localThis.editActDesc;
    localThis.editActMainDataItem.SERVICE_ID = localThis.selectedObjectId;
    localThis.editActMainDataItem.ID = localThis.editActId;
    EventBus.$on("hideSrvTabs", (data: any) => {
      //localThis.showSrvTabs = false;
    });

    // EventBus.$on("showSrvTabs", (data: any) => {
    //   localThis.showSrvTabs = true;

    //   EventBus.$emit("refreshBreadCumb", 2);
    // });
  },
  watch: {
    editActDesc(desc: String) {
      let localThis = this as any;
      localThis.localEditActDesc = desc;
      localStorage.SelectedAct = localThis.localEditActDesc;
      EventBus.$emit("refreshBreadCumb", 2);
    }
  }
});
</script>
<style scoped>
.vertical-center {
  margin-right: 0.2rem;
  margin-top: 0.3rem;
}
</style>