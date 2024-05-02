<template>
  <v-row>
    <v-col :cols="12">
      <v-card class="mx-my-0 pa-0">
        <v-tabs
          v-model="tabIsVisible"
          background-color="primary darken-1"
          dark
          v-if="showCCTabs"
        >
          <div class="vertical-center">
            <v-btn
              color="secondary lighten-2"
              class="grey--text text--darken-3"
              @click="returnToCCList()"
            >
              <v-icon>{{ sIconBack }}</v-icon>
            </v-btn>
          </div>

          <v-tab v-for="item in tabsItems" :key="item.code">{{ item.descr }}</v-tab>
          <v-spacer></v-spacer>
          <v-chip class="ma-2" color="default">
            <v-avatar left>
              <v-icon>mdi-target</v-icon>
            </v-avatar>
            {{ localEditObjDesc }}
          </v-chip>
        </v-tabs>
        <v-tabs-items v-model="tabIsVisible">
           <!-- NARRATIVE -->
          <v-tab-item key="NARRATIVE">
            <ap-narrative
              :selectedObjectId="editObjId"
              :tableName="tblName"
              :isUpdatableForm="!onlyRead"
              :versioned="versioned"
              :narrativeTipology="'Principles, values, criteria  cross cutting'"
            ></ap-narrative>
          </v-tab-item>
        </v-tabs-items>
      </v-card>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import axios from "axios";
import Vue from "vue";
import { mapActions } from "vuex";
import { translate } from "../../../i18n";
import Narrative from "../ANNUALPLAN/AnnualPlanNarrative.vue";
// import Indicators from "./AnnualPlanCCIndicatorForm.vue";

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
    // "ap-indicator": Indicators,
  },
  props: {


    editObjId: {
      type: Number,
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
      tblName: "PMS_CC",
      showCCTabs: true,
      sIconBack: "mdi-chevron-double-left",
      tabIsVisible: null,
      localEditObjDesc: "",
      editedObjMainDataItem: {},
      tabsItems: [
       { code: "NARRATIVE", descr: "Principles, values, criteria  cross cutting description" },
        // { code: "INDICATORS", descr: "Indicators" },
      ],
      oldObj: {},
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
    saveCCMainData(item: any) {
      let localThis: any = this as any;
      //localThis.returnToCCList();
      localThis.$emit("saveCCData", item);
    },

    returnToCCList() {
      let localThis: any = this as any;
      //EventBus.$emit("showActTabs");

      EventBus.$emit("hideCCDetails");
      EventBus.$emit("showActTabs");

      //EventBus.$emit("refreshBreadCumb", 1);
    },


  },
  mounted() {
    let localThis = this as any;
    localThis.localEditObjDesc = localThis.editObjDesc;
    localStorage.SelectedObj = localThis.localEditObjDesc;
    EventBus.$on("hideCCTabs", (data: any) => {
      localThis.showCCTabs = false;
    });

    EventBus.$on("showCCTabs", (data: any) => {
      localThis.showCCTabs = true;
      EventBus.$emit("refreshBreadCumb", 1);
    });
  },
  watch: {
    editObjDesc(desc: String) {
      let localThis = this as any;
      localThis.localEditObjDesc = desc;
      localStorage.SelectedObj = localThis.localEditObjDesc;
      EventBus.$emit("refreshBreadCumb", 1);
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
