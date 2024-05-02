<template>
  <div>
    <v-tabs
      v-model="active_tab"
      background-color="primary darken-1"
      dark
      class="mx-0 my-0 pa-0"
      v-if="showSrvTabs"
    >
      <div class="vertical-center">
        <v-btn
          color="secondary lighten-2"
          class="grey--text text--darken-3"
          @click="returnToServiceList()"
        >
          <v-icon>{{ sIconBack }}</v-icon>
        </v-btn>
      </div>
      <v-tab v-for="item in activeTabs" :key="item.id">{{ item.descr }}</v-tab>
      <v-spacer></v-spacer>

      <v-chip class="ma-2" color="default">
        <v-avatar left>
          <v-icon>mdi-cog</v-icon>
        </v-avatar>
        {{ localEditSrvDesc }}
      </v-chip>
    </v-tabs>
    <v-tabs-items v-model="active_tab" :key="rndVar">
      <v-tab-item key="MAINDATA">
        <v-card>
          <ap-srv-main-data
            @closeEdit="returnToServiceList"
            @refreshDesc="refreshDesc"
            @refreshObjSrv="refreshObjSrv"
            :formDataExt="editSrvMainDataItem"
            :selectedObjectId="selectedObjectId"
            :onlyRead="onlyRead"
            :versioned="versioned"
            :cascade="cascade"
          ></ap-srv-main-data>
        </v-card>
      </v-tab-item>

      <!-- NARRATIVE -->
      <v-tab-item key="NARRATIVE">
        <ap-narrative
          :selectedObjectId="editSrvId"
          :tableName="tblName"
          :isUpdatableForm="isUpdatableForm"
          :versioned="versioned"
          :narrativeTipology="'Service'"

        ></ap-narrative>
      </v-tab-item>

      <!-- Category of Intervention -->
      <!-- <v-tab-item key="INDICATORS">
        <ap-indicators :onlyRead="onlyRead" :selectedObjectId="editSrvId" :showIndicatorDetails="true" :isUpdatableForm="isUpdatableForm"></ap-indicators>
      </v-tab-item> -->
      <!-- <v-tab-item key="LOCATION">
        <ap-locations
          :onlyRead="onlyRead"
          :selectedObjectId="editSrvId"
          :key="Math.ceil(Math.random() * 1000)"
          :locationId="editSrvMainDataItem.LOCATION_ID"
          :locationDetails="editSrvMainDataItem.LOCATION_DETAILS"
        ></ap-locations>
      </v-tab-item> -->

      <!-- <v-tab-item key="POSITIONS">
        <ap-positions :selectedObjectId="editSrvId" :showPositionDetails="true"></ap-positions>
      </v-tab-item> -->
      <!-- <v-tab-item key="RESOURCES">
        <ap-resources :selectedObjectId="editSrvId" :showItemDetails="true"></ap-resources>
      </v-tab-item> -->

      <v-tab-item key="PROCESSES" v-if="!cascade">
        <ap-processes
          :onlyRead="onlyRead"
          :selectedObjectId="editSrvId"
          :showActivityDetails="true"
          :formDataExt="editSrvMainDataItem"
          :versioned="versioned"
        ></ap-processes>
      </v-tab-item>
    </v-tabs-items>
  </div>
</template>

<script lang="ts">
import axios from "axios";
import Vue from "vue";
import { mapActions } from "vuex";
import { translate } from "../../../i18n";
import AnnualPlanServiceMainData from "./AnnualPlanServiceMainDataForm.vue";
import Narrative from "../ANNUALPLAN/AnnualPlanNarrative.vue";
import Indicators from "../INDICATORS/AnnualPlanIndicatorForm.vue";
import Locations from "../LOCATIONS/AnnualPlanLocationForm.vue";
import Resources from "../ITEMS/AnnualPlanItemForm.vue";
// import Positions from "../POSITIONS/AnnualPlanPositionForm.vue";
//import Processes from "../ACTIVITIES/AnnualPlanActivityForm.vue"; //Con budget
import Processes from "../PROCESS/AnnualPlanProcessForm.vue"; //Con budget
// import Services from "../SERVICES/AnnualPlanServiceForm.vue";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../axiosapi";
import { EventBus } from "../../../event-bus";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
export default Vue.extend({
  components: {
    "ap-narrative": Narrative,
    "ap-srv-main-data": AnnualPlanServiceMainData,
    //"ap-indicators": Indicators,
    //"ap-locations": Locations,
    // "ap-resources": Resources,
    // "ap-positions": Positions,
    "ap-processes": Processes,
  },
  props: {
    // selectedAnnualPlan: {
    //   type: Srvect
    // },
    editSrvMainDataItem: {
      type: Object,
    },
    editSrvId: {
      type: Number,
    },
    editSrvDesc: {
      type: String,
      required: true,
    },
    selectedObjectId: {
      type: Number,
      required: true,
    },
    isUpdatableForm: {
      type: Boolean,
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
    cascade: {
      type: Boolean,
      default: false,
      required: false,
    },
  },
  data() {
    return {
      active_tab: null,
      saveNarrative: false,
      rndVar: 0,
      //positionsInserted: false,
      showSrvTabs: true,
      tblName: "PMS_SERVICE",
      sIconBack: "mdi-chevron-double-left",
      tabIsVisible: null,
      editedSrvMainDataItem: {},
      localEditSrvDesc: "",
      tabsItems: [
        { id: 0, code: "MAINDATA", descr: "Service Info", active: true },
        { id: 1, code: "NARRATIVE", descr: "Service Description", active: true },
        // { id:2,code: "INDICATORS", descr: "Indicators", active: true },
        // { id: 2, code: "LOCATION", descr: "Location", active: true },
        // { id:4,code: "POSITIONS", descr: "Positions", active: true },
        // { id:5,code: "RESOURCES", descr: "Resources", active: true },
        { id: 2, code: "PROCESSES", descr: "Processes", active: true },
      ],
    };
  },
  methods: {
    refreshDesc(item: any) {
      let localThis: any = this as any;
      localThis.localEditSrvDesc =
        item.COI.PMS_COI_CODE + "-" + item.TOS.PMS_TOS_DESCRIPTION;
      localStorage.SelectedSrv = localThis.localEditSrvDesc;
      EventBus.$emit("reloadSelectedServices", item);
    },
    refreshObjSrv() {
      let localThis: any = this as any;
      //EventBus.$emit("saveNarrative");
      EventBus.$emit("reloadServices");
    },
    returnToServiceList() {
      let localThis: any = this as any;
      //localThis.$emit("saveNarrative");
      localThis.saveNarrative = true;
      EventBus.$emit("closeServiceDetails");
    },
  },
  watch: {
    editSrvMainDataItem(to: any) {
      let localThis = this as any;
    },
  },
  computed: {
    activeTabs: function () {
      let localThis = this as any;
      return localThis.tabsItems.filter(function (u: any) {
        return u.active;
      });
    },
  },

  mounted() {
    let localThis = this as any;

    if (localThis.cascade == false) {
      localThis.tabsItems = [
        { id: 0, code: "MAINDATA", descr: "Service Info", active: true },
        { id: 1, code: "NARRATIVE", descr: "Service Description", active: true },

        { id: 2, code: "PROCESSES", descr: "Processes", active: true },
      ];
    } else {
      localThis.tabsItems = [
        { id: 0, code: "MAINDATA", descr: "Service Info", active: true },
        { id: 1, code: "NARRATIVE", descr: "Service Description", active: true },
      ];
    }

    localThis.localEditSrvDesc = localThis.editSrvDesc;
    localStorage.SelectedSrv = localThis.localEditSrvDesc;
    EventBus.$on("hideSrvTabs", (data: any) => {
      localThis.showSrvTabs = false;
    });

    EventBus.$on("showSrvTabs", (data: any) => {
      localThis.showSrvTabs = true;
      EventBus.$emit("refreshBreadCumb", 2);
    });
    // EventBus.$on("positionOnService", (data: number) => {
    //   if (data > 0) {
    //     localThis.tabsItems[5].active = true;
    //     localThis.positionsInserted = true;
    //   } else localThis.positionsInserted = false;
    // });

    // EventBus.$on("sectActiveTab", (data: string) => {
    //   localThis.rndVar = Math.ceil(Math.random() * 1000);
    //   switch(data)
    //   {
    //   case "PROCESSES":

    //   if (localThis.tabsItems[5].active==true)
    //     localThis.active_tab=6;
    //   else
    //     localThis.active_tab=5;
    //   break;
    //   }
    // });
  },
});
</script>
<style scoped>
.vertical-center {
  margin-right: 0.2rem;
  margin-top: 0.3rem;
}
</style>
