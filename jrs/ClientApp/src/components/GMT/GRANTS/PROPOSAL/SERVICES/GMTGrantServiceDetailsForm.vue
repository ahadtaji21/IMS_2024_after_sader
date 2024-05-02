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
          <v-icon>{{sIconBack}}</v-icon>
        </v-btn>
      </div>
      <v-tab v-for="item in activeTabs" :key="item.id">{{ item.descr }}</v-tab>
      <v-spacer></v-spacer>

      <v-chip class="ma-2" color="default">
        <v-avatar left>
          <v-icon>mdi-cog</v-icon>
        </v-avatar>
        {{localEditSrvDesc}}
      </v-chip>
    </v-tabs>
    <v-tabs-items v-model="active_tab" :key="rndVar" >
      <v-tab-item key="MAINDATA">
        <v-card>
          <gt-srv-main-data
            @closeEdit="returnToServiceList"
            @refreshDesc="refreshDesc"
            @refreshObjSrv="refreshObjSrv"
            :formDataExt="editSrvMainDataItem"
            :selectedObjectId="selectedObjectId"
            :onlyRead="onlyRead"
          ></gt-srv-main-data>
        </v-card>
      </v-tab-item>

      <!-- NARRATIVE -->
      <v-tab-item key="NARRATIVE">
        <gt-narrative :onlyRead="onlyRead" :selectedObjectId="editSrvId" :tableName="tblName" :isUpdatableForm="isUpdatableForm"></gt-narrative>
      </v-tab-item>

      <!-- Category of Intervention -->
      <!-- <v-tab-item key="INDICATORS">
        <gt-indicators :selectedObjectId="editSrvId" :showIndicatorDetails="true" :isUpdatableForm="isUpdatableForm"></gt-indicators>
      </v-tab-item> -->
      <v-tab-item key="LOCATION">
        <gt-locations
          :selectedObjectId="editSrvId"
          :key="Math.ceil(Math.random()*1000)"
          :locationId="editSrvMainDataItem.LOCATION_ID"
          :locationDetails="editSrvMainDataItem.LOCATION_DETAILS"
          :onlyRead="onlyRead"
        ></gt-locations>
      </v-tab-item>

      <!-- <v-tab-item key="POSITIONS">
        <gt-positions :selectedObjectId="editSrvId" :showPositionDetails="true"></gt-positions>
      </v-tab-item> -->
      <!-- <v-tab-item key="RESOURCES">
        <gt-resources :selectedObjectId="editSrvId" :showItemDetails="true"></gt-resources>
      </v-tab-item> -->

      <v-tab-item key="ACTIVITIES">
        <gt-activities :onlyRead="onlyRead" :selectedObjectId="editSrvId" :showActivityDetails="true"></gt-activities>
      </v-tab-item>
    </v-tabs-items>
  </div>
</template>

<script lang="ts">
import axios from "axios";
import Vue from "vue";
import { mapActions } from "vuex";
import { translate } from "../../../../../i18n";
import ServiceMainData from "./GMTGrantServiceMainDataForm.vue";
import Narrative from "../NARRATIVE/GMTGrantNarrative.vue";
//import Indicators from "../INDICATORS/AnnualPlanIndicatorForm.vue";
import Locations from "../LOCATIONS/GMTGrantLocationForm.vue";
//import Resources from "../ITEMS/AnnualPlanItemForm.vue";
// import Positions from "../POSITIONS/AnnualPlanPositionForm.vue";
 import Activities from "../ACTIVITIES/GMTGrantActivityForm.vue";
// import Services from "../SERVICES/AnnualPlanServiceForm.vue";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../../../axiosapi";
import { EventBus } from "../../../../../event-bus";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../../../../axiosapi/api";
export default Vue.extend({
  components: {
    "gt-narrative": Narrative,
    "gt-srv-main-data": ServiceMainData,
    //"gt-indicators": Indicators,
    "gt-locations": Locations,
    // "gt-resources": Resources,
    // "gt-positions": Positions,
    "gt-activities": Activities
  },
  props: {
    // selectedAnnualPlan: {
    //   type: Srvect
    // },
    editSrvMainDataItem: {
      type: Object
    },
    editSrvId: {
      type: Number
    },
    editSrvDesc: {
      type: String,
      required: true
    },
    selectedObjectId: {
      type: Number,
      required: true
    },
    isUpdatableForm: {
      type: Boolean,
      required:true
    },
    onlyRead:{
      type:Boolean,
      required:false,
      default:false

    }
  },
  data() {
    return {
      active_tab:null,
      rndVar:0,
      //positionsInserted: false,
      showSrvTabs: true,
      tblName: "GMT_SERVICE",
      sIconBack: "mdi-chevron-double-left",
      tabIsVisible: null,
      editedSrvMainDataItem: {},
      localEditSrvDesc: "",
      tabsItems: [
        { id:0, code: "MAINDATA", descr: "Main Data", active: true },
        { id:1,code: "NARRATIVE", descr: "Narrative", active: true },
        // { id:2,code: "INDICATORS", descr: "Indicators", active: true },
        { id:3,code: "LOCATION", descr: "Location", active: true },
        // { id:4,code: "POSITIONS", descr: "Positions", active: true },
        // { id:5,code: "RESOURCES", descr: "Resources", active: true },
        { id:4,code: "ACTIVITIES", descr: "Activities", active: true }
      ]
    };
  },
  methods: {
    refreshDesc(item: any) {
      let localThis: any = this as any;
      localThis.localEditSrvDesc =
        item.COI.GMT_COI_CODE + "-" + item.TOS.GMT_TOS_DESCRIPTION;
      localStorage.SelectedSrv = localThis.localEditSrvDesc;
    },
    refreshObjSrv() {
      let localThis: any = this as any;
      EventBus.$emit("reloadServices");
    },
    returnToServiceList() {
      let localThis: any = this as any;
      EventBus.$emit("closeServiceDetails");
    }
  },
  watch: {
    editSrvMainDataItem(to: any) {
      let localThis = this as any;
    }
  },
  computed: {
    activeTabs: function() {
      let localThis = this as any;
      return localThis.tabsItems.filter(function(u: any) {
        return u.active;
      });
    }
  },

  mounted() {
    let localThis = this as any;
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
    //   case "ACTIVITIES":

    //   if (localThis.tabsItems[5].active==true)
    //     localThis.active_tab=6;
    //   else
    //     localThis.active_tab=5;
    //   break;
    //   }
    // });
  }
});
</script>
<style scoped>
.vertical-center {
  margin-right: 0.2rem;
  margin-top: 0.3rem;
}
</style>