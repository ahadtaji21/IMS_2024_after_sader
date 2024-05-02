<template>
  <v-row>
    <v-col :cols="12">
      <v-card class="mx-my-0 pa-0">
        <v-tabs
          v-model="tabIsVisible"
          background-color="primary darken-1"
          dark
          v-if="showOutpTabs"
        >
          <div class="vertical-center">
            <v-btn
              color="secondary lighten-2"
              class="grey--text text--darken-3"
              @click="returnToProcessList()"
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
          <v-tab-item key="MAINDATA">
            <v-card class="mx-my-0 pa-0">
              <ap-ovgmain-data
                :formDataExt="formData"
                :isUpdatableForm="!onlyRead"
                @saveDialog="saveDialoge"
                @closeDialogOVG="returnToProcessList"
                :versioned="versioned"
              ></ap-ovgmain-data>
            </v-card>
          </v-tab-item>

          <!-- INDICATORS -->

          <!-- <v-tab-item key="INDICATORS">
            <ap-indicator
              :onlyRead="onlyRead"
              :formData="formData"
              :showItemDetails="true"
              :versioned="versioned"
            ></ap-indicator>
          </v-tab-item> -->
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
import AnnualPlanOverallGoalMainData from "./AnnualPlanOverallGoalMainDataForm.vue";
// import Indicators from "./AnnualPlanOverallGoalIndicatorForm.vue";

import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../axiosapi";
import { EventBus } from "../../../event-bus";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
export default Vue.extend({
  components: {
    "ap-ovgmain-data": AnnualPlanOverallGoalMainData,
    // "ap-indicator": Indicators,
  },
  props: {
    // selectedAnnualPlan: {
    //   type: Object
    // },
    formData: {
      type: Object,
    },

    onlyRead: {
      type: Boolean,
      required: true,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  data() {
    return {
      showOutpTabs: true,
      sIconBack: "mdi-chevron-double-left",
      tabIsVisible: null,
      localEditObjDesc: "",
      editedObjMainDataItem: {},
      tabsItems: [
        { code: "MAINDATA", descr: "Main Data" },
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

    returnToProcessList() {
      let localThis: any = this as any;
      EventBus.$emit("showActTabs");
      EventBus.$emit("hideOutpDetails");

      //EventBus.$emit("refreshBreadCumb", 1);
    },
    saveDialoge() {
      let localThis = this as any;
      localThis.$emit("reloadOverallGoal");
    },
  },
  mounted() {
    let localThis = this as any;
    localThis.localEditObjDesc = localThis.editObjDesc;
    localStorage.SelectedObj = localThis.localEditObjDesc;
    EventBus.$on("hideOutpTabs", (data: any) => {
      localThis.showOutpTabs = false;
    });

    EventBus.$on("showOutpTabs", (data: any) => {
      localThis.showOutpTabs = true;
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
