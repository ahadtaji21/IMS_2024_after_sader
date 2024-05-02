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
              @click="returnToOutputList()"
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
              <ap-out-main-data
                :formData="formData"
                @saveOutputMainData="saveOutputMainData"
                :key="Math.ceil(Math.random() * 1000)"
                :onlyRead="onlyRead"
              ></ap-out-main-data>
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
import AnnualPlanOutputMainData from "./AnnualPlanOutputMainDataForm.vue";
// import Indicators from "./AnnualPlanOutputIndicatorForm.vue";

import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../axiosapi";
import { EventBus } from "../../../event-bus";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
export default Vue.extend({
  components: {
    "ap-out-main-data": AnnualPlanOutputMainData,
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
      showOutpTabs: true,
      sIconBack: "mdi-chevron-double-left",
      tabIsVisible: null,
      localEditObjDesc: "",
      editedObjMainDataItem: {},
      tabsItems: [
        { code: "MAINDATA", descr: "Output Data" },
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
    saveOutputMainData(item: any) {
      let localThis: any = this as any;
      //localThis.returnToOutputList();
      localThis.$emit("saveOutpData", item);
    },

    returnToOutputList() {
      let localThis: any = this as any;
      //EventBus.$emit("showActTabs");

      EventBus.$emit("hideOutpDetails");
      EventBus.$emit("showActTabs");

      //EventBus.$emit("refreshBreadCumb", 1);
    },
    closeObjMainData() {
      let localThis = this as any;
      EventBus.$emit("closeObjectiveDetails");
    },

    SaveObjMainData(value: any) {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-update")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-add")
        .toString();

      let id = 0;
      let msg = msgUpd;
      if (value["ID"] == undefined) {
        msg = msgAdd;
      } else {
        id = Number(value["ID"]);
      }
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OUTCOME_OBJECTIVE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(value),
          };

          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              //   localThis.editedObjMainDataItem = localThis.itemModel;
              //   localThis.editedObjMainDataItem.ANNUAL_PLAN_ID =
              //     localThis.selectedAnnualPlan.id;

              EventBus.$emit("reloadObjectives", value);
              localThis.localEditObjDesc = value["DESCRIPTION"];
              localStorage.SelectedObj = value["DESCRIPTION"];
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
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
