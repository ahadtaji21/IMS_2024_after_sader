<template>
  <v-row>
    <v-col :cols="12">
      <v-card class="mx-my-0 pa-0">
        <v-tabs
          v-model="tabIsVisible"
          background-color="primary darken-1"
          dark
          v-if="showObjectTabs"
        >
          <div class="vertical-center">
            <v-btn
              color="secondary lighten-2"
              class="grey--text text--darken-3"
              @click="returnToObjList()"
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
          <v-tab-item key="MAINDATA" v-if="!cascade">
            <v-card class="mx-my-0 pa-0">
              <ap-obj-main-data
                @closeDialoge="closeObjMainData"
                @closeDialogeAndSaveObj="SaveObjMainData"
                @closeDialogeAndSaveObjFromMain="SaveObjMainDataFromMain"
                :formDataExt="editObjMainDataItem"
                :isUpdatableForm="isUpdatableForm"
                :versioned="versioned"
              ></ap-obj-main-data>
            </v-card>
          </v-tab-item>

          <!-- NARRATIVE -->
          <v-tab-item key="NARRATIVE">
            <ap-narrative
              :selectedObjectId="editObjId"
              :tableName="tblName"
              :isUpdatableForm="isUpdatableForm"
              :versioned="versioned"
              :narrativeTipology="'Objective'"
            ></ap-narrative>
          </v-tab-item>
          <!-- SERVICES -->
          <v-tab-item key="SERVICES" v-if="!cascade">
            <ap-services
              :onlyRead="onlyRead"
              :selectedObjectId="editObjId"
              :formDataInput="editObjMainDataItem"
              :showServiceDetails="showSrvDetails"
              :isUpdatableForm="isUpdatableForm"
              :versioned="versioned"
            ></ap-services>
          </v-tab-item>
          <!-- INDICATORS -->
          <!-- <v-tab-item key="INDICATORS">
            <ap-indicator
              :onlyRead="onlyRead"
              :selectedObjectiveId="editObjId"
              :showItemDetails="true"
              :versioned="versioned"
              :isUpdatableForm="isUpdatableForm"
            ></ap-indicator>
          </v-tab-item> -->
          <!-- BUDGET -->
          <!-- <v-tab-item key="BUDGET">
            <ap-budget
              :selectedObjectId="editObjId"
              :showServiceDetails="showSrvDetails"
              :key="Math.ceil(Math.random() * 1000)"
              :isUpdatableForm="isUpdatableForm"
              :onlyRead="onlyRead"
              :versioned="versioned"
            ></ap-budget>
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
import AnnualPlanObjectiveMainData from "./AnnualPlanObjectiveMainDataForm.vue";
import Narrative from "../ANNUALPLAN/AnnualPlanNarrative.vue";
import Services from "../SERVICES/AnnualPlanServiceForm.vue";
// import Indicators from "./AnnualPlanObjectiveIndicatorForm.vue";
import Budget from "../SERVICES/AnnualPlanServicesBudgetForm.vue";
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
    "ap-obj-main-data": AnnualPlanObjectiveMainData,
    "ap-services": Services,
    // "ap-budget": Budget,
    // "ap-indicator": Indicators,
  },
  props: {
    // selectedAnnualPlan: {
    //   type: Object
    // },
    editObjMainDataItem: {
      type: Object,
    },
    editObjId: {
      type: Number,
    },
    editObjDesc: {
      type: String,
      required: true,
    },
    showSrvDetails: {
      type: Boolean,
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
      showObjectTabs: true,
      sIconBack: "mdi-chevron-double-left",
      tblName: "PMS_OBJECTIVE",
      tabIsVisible: null,
      localEditObjDesc: "",
      editedObjMainDataItem: {},
      saveNarrative: false,
      tabsItems: [],
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

    returnToObjList() {
      let localThis: any = this as any;

      localThis.$emit("hideObjDetails");

      //EventBus.$emit("saveNarrative");
      localThis.saveNarrative = true;

      EventBus.$emit("refreshBreadCumb", 1);
    },
    closeObjMainData() {
      let localThis = this as any;
      localThis.$emit("closeObjectiveDetails");
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
          let i: number = 0;
          let value1 = {} as any;
          if (value.OUTCOME == undefined) return;
          for (i = 0; i < value.OUTCOME.length; i++) {
            value1 = value;
            value1.OUTCOME = [];
            value1.OUTCOME[0] = value.OUTCOME[i];

            let savePayload: GenericSqlPayload = {
              spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OUTCOME_OBJECTIVE",
              actionType: SqlActionType.NUMBER_0,
              jsonPayload: JSON.stringify(value1),
            };

            const retvalue = localThis
              .execSPCall(savePayload)
              .then((res: any) => {
                return res;
              })
              .catch((err: any) => {
                localThis.showNewSnackbar({
                  typeName: "error",
                  text: err.message,
                }); // Feedback to user
                throw err;
              });
            localThis.showNewSnackbar({
              typeName: "success",
              text: "Outcome objective inserted",
            }); // Feedback to user

            localThis.$emit("reloadObjectives", value);
            localThis.localEditObjDesc = value["DESCRIPTION"];
            localStorage.SelectedObj = value["DESCRIPTION"];
          }
        }
      });
    },

    SaveObjMainDataFromMain(value: any) {
      let localThis = this as any;

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

          localThis.$emit("reloadObjectives", value);
          localThis.localEditObjDesc = value["DESCRIPTION"];
          localStorage.SelectedObj = value["DESCRIPTION"];
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
  },
  mounted() {
    let localThis = this as any;
    localThis.localEditObjDesc = localThis.editObjDesc;
    localStorage.SelectedObj = localThis.localEditObjDesc;
    if (localThis.cascade == false) {
      localThis.tabsItems = [
        { code: "MAINDATA", descr: "Objective Info" },
        { code: "NARRATIVE", descr: "Objective Description" },
        { code: "SERVICES", descr: "Services" },
        // { code: "INDICATORS", descr: "Indicators" },
        // { code: "BUDGET", descr: "Budget" },
      ];
    } else {
      localThis.tabsItems = [{ code: "NARRATIVE", descr: "Objective Description" }];
    }
    EventBus.$on("hideObjecTabs", (data: any) => {
      localThis.showObjectTabs = false;
    });

    EventBus.$on("showObjecTabs", (data: any) => {
      localThis.showObjectTabs = true;
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
